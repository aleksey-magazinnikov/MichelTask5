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
        public DateTime LastDate
        {
            get { return toDate; }
            set { SetPropertyValue(nameof(toDate), ref toDate, value); }
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

        [Action(ToolTip = "Generate Work Load", Caption = "Generate Work Load")]
        public void GenerateWorkLoad()
        {
            Items.Clear();
            // var colDelete = new XPCollection<WorkLoadItem>(Session);
            var colDelete = Session.GetObjects(Session.GetClassInfo(typeof(WorkLoadItem)), null, null, 0, false, true);
            foreach (var item in colDelete)
            {
                Session.Delete(item);
            }

            if (fromDate <= DateTime.MinValue || toDate <= DateTime.MinValue)
            {
                return;
            }

            var collection = Session.GetObjects(Session.GetClassInfo(typeof(PlanEquipmentLink)), CriteriaOperator.Parse("LinkPlan.Active_Plan == 'true' and LinkPlan.Plan_Status == 1 and LinkPlan.NextDate >= ? and LinkPlan.NextDate <= ?", FromDate, toDate), null, 0, false, true);
            foreach (PlanEquipmentLink link in collection)
            {
                if (link.LinkPlan.Rolling_Plan)
                {
                    var linkPlanNextDate = link.LinkPlan.NextDate;

                    while (linkPlanNextDate <= toDate)
                    {
                        var days = GetPeriodInDays(link);
                        var workLoadItem = CreateWorkLoadItem(link, linkPlanNextDate);
                        Items.Add(workLoadItem);

                        linkPlanNextDate = linkPlanNextDate.AddDays(days);
                    }
                }
                else
                {
                    var workLoadItem = CreateWorkLoadItem(link, null);
                    Items.Add(workLoadItem);
                }
            }
        }

        private int GetPeriodInDays(PlanEquipmentLink link)
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

        private WorkLoadItem CreateWorkLoadItem(PlanEquipmentLink link, DateTime? dueDate)
        {
            var workLoadItem = new WorkLoadItem(Session)
            {
                PlanNumber = link.LinkPlan.M_Plan_Num,
                OperationNumber = link.Operation.M_Operation_Num,
                Equipment = link.LinkEquipment.EquipmentName,
                DueDate = link.LinkPlan.NextDate,
                PlanId = link.LinkPlan.Oid,
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