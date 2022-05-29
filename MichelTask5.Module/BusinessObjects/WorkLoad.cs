using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace MichelTask5.Module.BusinessObjects
{
    [DefaultClassOptions, NavigationItem("Maintenance")]
    public class WorkLoad : BaseObject
    {
        public WorkLoad(Session session) : base(session)
        {
        }

        DateTime fromDate;
        [ImmediatePostData]
        public DateTime FromDate
        {
            get { return fromDate; }
            set { SetPropertyValue(nameof(fromDate), ref fromDate, value); }
        }

        DateTime toDate;
        [ImmediatePostData]
        public DateTime ToDate
        {
            get { return toDate; }
            set { SetPropertyValue(nameof(ToDate), ref toDate, value); }
        }

        private BindingList<WorkLoadItem> _items = null;

        [CollectionOperationSet(AllowAdd = false, AllowRemove = false)]
        public BindingList<WorkLoadItem> Items => _items ?? (_items = new BindingList<WorkLoadItem>());

        public int GetPeriodInDays(PlanEquipmentLink link)
        {
            int days = 0;
            switch (link.LinkPlan.Period)
            {
                case PeriodType.Days:
                    days = link.LinkPlan.Number;
                    break;

                case PeriodType.Weeks:
                    days = link.LinkPlan.Number * 7;
                    break;
                case PeriodType.Months:
                    days = (link.LinkPlan.NextDate - link.LinkPlan.BaseDate).Days;
                    break;
            }

            return days;
        }

        public WorkLoadItem CreateWorkLoadItem(PlanEquipmentLink link, string currentUserName, object currentUser,
            DateTime? dueDate)
        {
            var workLoadItem = new WorkLoadItem(Session)
            {
                PlanNumber = link.LinkPlan.M_Plan_Num,
                OperationNumber = link.Operation.M_Operation_Num,
                Equipment = link.LinkEquipment.EquipmentName,
                DueDate = link.LinkPlan.NextDate,
                PlanId = link.LinkPlan.Oid,
                OperationId = link.Operation.Oid,
                EquipmentId = link.LinkEquipment.Oid,
                UserId = Guid.Parse(currentUser.ToString()),
                UserName = currentUserName,
                SeparateWorkOrderPerEquipment = link.LinkPlan.SeparateWorkOrderPerEquipment
            };
            if (dueDate != null)
            {
                workLoadItem.DueDate = dueDate.Value;
            }

            workLoadItem.Save();

            return workLoadItem;
        }
    }
}