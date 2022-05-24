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

            if (fromDate <= DateTime.MinValue || toDate <= DateTime.MinValue)
            {
                return;
            }

            var collection = Session.GetObjects(Session.GetClassInfo(typeof(M_Plan)), CriteriaOperator.Parse("Active_Plan == 'true' and Plan_Status == 1 and NextDate >= ? and NextDate <= ?", FromDate, toDate), null, 0, false, true);
            foreach (M_Plan plan in collection)
            {
                if (plan.Rolling_Plan)
                {
                    var linkPlanNextDate = plan.NextDate;

                    while (linkPlanNextDate <= toDate)
                    {
                        var days = GetPeriodInDays(plan);
                        var workLoadItem = CreateWorkLoadItem(plan, linkPlanNextDate);
                        Items.Add(workLoadItem);

                        linkPlanNextDate = linkPlanNextDate.AddDays(days);
                    }
                }
                else
                {
                    var workLoadItem = CreateWorkLoadItem(plan, null);
                    Items.Add(workLoadItem);
                }
            }
        }

        private int GetPeriodInDays(M_Plan plan)
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

        private WorkLoadItem CreateWorkLoadItem(M_Plan plan, DateTime? dueDate)
        {
            var workLoadItem = new WorkLoadItem(Session)
            {
                PlanNumber = plan.M_Plan_Num,
                OperationNumber = plan.Plan_Operation.M_Operation_Num,
                Equipment = plan.Equipments.FirstOrDefault()?.EquipmentName,
                DueDate = plan.NextDate,
                PlanId = plan.Oid,
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
