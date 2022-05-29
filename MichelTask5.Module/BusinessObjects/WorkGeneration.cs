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
    public class WorkGeneration : BaseObject
    {
        public WorkGeneration(Session session) : base(session)
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
        public BindingList<WorkLoadItem> Items
        {
            get
            {
                if (_items == null)
                {
                    _items = new BindingList<WorkLoadItem>();

                }

                return _items;
            }
        }

        public int GetPeriodInDays(M_Plan plan)
        {
            int days = 0;
            switch (plan.Period)
            {
                case PeriodType.Days:
                    days = plan.Number;
                    break;

                case PeriodType.Weeks:
                    days = plan.Number * 7;
                    break;
                case PeriodType.Months:
                    days = (plan.NextDate - plan.BaseDate).Days;
                    break;
            }

            return days;
        }

        public WorkLoadItem CreateWorkLoadItem(M_Plan plan, string currentUserName, object currentUser, DateTime? dueDate)
        {
            var workLoadItem = new WorkLoadItem(Session)
            {
                PlanNumber = plan.M_Plan_Num,
                OperationNumber = plan.Plan_Operation.M_Operation_Num,
                Equipment = plan.Equipments.FirstOrDefault()?.EquipmentName,
                DueDate = plan.NextDate,
                PlanId = plan.Oid,
                OperationId = plan.Plan_Operation.Oid,
                EquipmentId = plan.Equipments.FirstOrDefault()?.Oid,
                UserId = Guid.Parse(currentUser.ToString()),
                UserName = currentUserName,
                SeparateWorkOrderPerEquipment = plan.SeparateWorkOrderPerEquipment
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
