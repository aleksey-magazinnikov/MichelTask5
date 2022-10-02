using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace MichelTask5.Module.BusinessObjects
{
    [DefaultClassOptions, NavigationItem("Maintenance")]
    [RuleObjectExists("AnotherWorkGenerationExists", DefaultContexts.Save, "User.Oid == CurrentUserId()",
        InvertResult = true,
        CustomMessageTemplate = "Another WorkGeneration already exists.")]
    [RuleCriteria("CannotDeleteWorkGeneration", DefaultContexts.Delete, "False",
        CustomMessageTemplate = "Cannot delete WorkGeneration.")]
    [Appearance("DatesVisible", Visibility = ViewItemVisibility.Hide, Criteria = "Plan = 1", Context = "DetailView", TargetItems = "FromDate,ToDate")]
    public class WorkGeneration : BaseObject
    {
        public WorkGeneration(Session session) : base(session)
        {
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            User = GetCurrentUser();
        }

        protected override void OnSaving()
        {
            base.OnSaving();
            User = GetCurrentUser();
        }

        DateTime fromDate;

        [ImmediatePostData]
        public DateTime FromDate
        {
            get { return fromDate; }
            set { SetPropertyValue(nameof(FromDate), ref fromDate, value); }
        }

        DateTime toDate;

        [ImmediatePostData]
        public DateTime ToDate
        {
            get { return toDate; }
            set { SetPropertyValue(nameof(ToDate), ref toDate, value); }
        }

        PermissionPolicyUser user;

        [ModelDefault("AllowEdit", "False")]
        public PermissionPolicyUser User
        {
            get => user;
            set => SetPropertyValue("User", ref user, value);
        }

        private Enums.PlanType plan;
        [ImmediatePostData]
        public Enums.PlanType Plan
        {
            get => plan;
            set => SetPropertyValue("Plan", ref plan, value);
        }

        PermissionPolicyUser GetCurrentUser() =>
            Session.GetObjectByKey<PermissionPolicyUser>(SecuritySystem.CurrentUserId);

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

        private BindingList<WorkGenerationItem> _listOfGeneratedWorkOrders = null;

        [CollectionOperationSet(AllowAdd = false, AllowRemove = false)]
        public BindingList<WorkGenerationItem> ListOfGeneratedWorkOrders
        {
            get
            {
                if (_listOfGeneratedWorkOrders == null)
                {
                    _listOfGeneratedWorkOrders = new BindingList<WorkGenerationItem>();

                }

                return _listOfGeneratedWorkOrders;
            }
        }

        public int GetPeriodInDays(M_Plan plan)
        {
            int days = 0;
            switch (plan.Period)
            {
                case Enums.PeriodType.Days:
                    days = plan.Number;
                    break;

                case Enums.PeriodType.Weeks:
                    days = plan.Number * 7;
                    break;
                case Enums.PeriodType.Months:
                    days = (plan.NextDate - plan.BaseDate).Days;
                    break;
            }

            return days;
        }

        public WorkLoadItem CreateWorkLoadItem(PlanEquipmentLink link, string currentUserName, object currentUser,
            int? sequence,
            DateTime? dueDate)
        {
            if (link.LinkPlan == null)
            {
                return null;
            }

            var workLoadItem = new WorkLoadItem(Session)
            {
                PlanNumber = link.LinkPlan.M_Plan_Num,
                OperationNumber = link.Operation != null ? link.Operation.M_Operation_Num : String.Empty,
                Equipment = link.LinkEquipment.EquipmentName,
                DueDate = link.LinkPlan.NextDate,
                PlanId = link.LinkPlan.Oid,
                OperationId = link.Operation?.Oid ?? Guid.Empty,
                EquipmentId = link.LinkEquipment.Oid,
                UserId = Guid.Parse(currentUser.ToString()),
                UserName = currentUserName,
                SeparateWorkOrderPerEquipment = link.LinkPlan.SeparateWorkOrderPerEquipment,
                Sequential = link.LinkPlan != null && link.LinkPlan.FrequencyType == Enums.FrequencyType.Sequential,
                Superpose = link.LinkPlan != null && link.LinkPlan.Superpose_Plan
            };
            if (dueDate != null)
            {
                workLoadItem.DueDate = dueDate.Value;
            }

            if (sequence != null && workLoadItem.Sequential)
            {
                workLoadItem.Sequence = sequence.Value;
            }

            workLoadItem.Save();

            return workLoadItem;
        }

        public WorkLoadItem CreateWorkLoadItemFromCPlan(CPlanEquipmentLink link, string currentUserName,
            object currentUser, float? counterValue)
        {
            if (link.LinkPlan == null)
            {
                return null;
            }

            var workLoadItem = new WorkLoadItem(Session)
            {
                PlanNumber = link.LinkPlan != null ? link.LinkPlan.C_Plan_Num : String.Empty,
                OperationNumber = link.Operation != null ? link.Operation.M_Operation_Num : String.Empty,
                Equipment = link.LinkEquipment.EquipmentName,
                DueDate = DateTime.Today,
                PlanId = link.LinkPlan?.Oid ?? Guid.Empty,
                OperationId = link.Operation?.Oid ?? Guid.Empty,
                EquipmentId = link.LinkEquipment?.Oid ?? Guid.Empty,
                UserId = Guid.Parse(currentUser.ToString()),
                UserName = currentUserName,
                SeparateWorkOrderPerEquipment = link.LinkPlan?.SeparateWorkOrderPerEquipment ?? false,
                Periodic = link.LinkPlan?.Usage == UsageType.Periodic,
                Threshold = link.LinkPlan?.Usage == UsageType.Threshold,
                BaseValue = counterValue ?? default
            };

            workLoadItem.Save();

            return workLoadItem;
        }

        public WorkGenerationItem CreateWorkGenerationItem(M_Plan mPlan, string currentUserName, object currentUser,
            Work_Order workOrder)
        {
            var workGenerationItem = new WorkGenerationItem(Session)
            {
                PlanNumber = mPlan.M_Plan_Num,
                OperationNumber = mPlan.Plan_Operation != null ? mPlan.Plan_Operation.M_Operation_Num : String.Empty,
                PlanId = mPlan.Oid,
                OperationId = mPlan.Plan_Operation?.Oid ?? Guid.Empty,
                UserId = Guid.Parse(currentUser.ToString()),
                UserName = currentUserName,
                WorkOrder = workOrder,
                WorkGeneration = this
            };

            workGenerationItem.Save();

            return workGenerationItem;
        }

        public WorkGenerationItem CreateWorkGenerationItem(C_Plan cPlan, string currentUserName, object currentUser,
            Work_Order workOrder)
        {
            var workGenerationItem = new WorkGenerationItem(Session)
            {
                PlanNumber = cPlan.C_Plan_Num,
                OperationNumber = cPlan.Plan_Operation != null ? cPlan.Plan_Operation.M_Operation_Num : String.Empty,
                PlanId = cPlan.Oid,
                OperationId = cPlan.Plan_Operation?.Oid ?? Guid.Empty,
                UserId = Guid.Parse(currentUser.ToString()),
                UserName = currentUserName,
                WorkOrder = workOrder,
                WorkGeneration = this
            };

            workGenerationItem.Save();

            return workGenerationItem;
        }
    }
}
