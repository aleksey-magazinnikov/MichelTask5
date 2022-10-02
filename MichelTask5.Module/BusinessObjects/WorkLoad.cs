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
    [RuleObjectExists("AnotherWorkLoadExists", DefaultContexts.Save, "User.Oid == CurrentUserId()", InvertResult = true,
        CustomMessageTemplate = "Another WorkLoad already exists.")]
    [RuleCriteria("CannotDeleteWorkLoad", DefaultContexts.Delete, "False",
        CustomMessageTemplate = "Cannot delete WorkLoad.")]
    [Appearance("DatesVisible", Visibility = ViewItemVisibility.Hide, Criteria = "Plan = 1", Context = "DetailView", TargetItems = "FromDate,ToDate")]
    public class WorkLoad : BaseObject
    {
        public WorkLoad(Session session) : base(session)
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

        PermissionPolicyUser GetCurrentUser() => Session.GetObjectByKey<PermissionPolicyUser>(SecuritySystem.CurrentUserId);

        private BindingList<WorkLoadItem> _items = null;

        [CollectionOperationSet(AllowAdd = false, AllowRemove = false)]
        public BindingList<WorkLoadItem> Items => _items ?? (_items = new BindingList<WorkLoadItem>());

        public int GetPeriodInDays(M_Plan linkPlan)
        {
            int days = 0;
            if (linkPlan != null)
            {
                switch (linkPlan.Period)
                {
                    case Enums.PeriodType.Days:
                        days = linkPlan.Number;
                        break;

                    case Enums.PeriodType.Weeks:
                        days = linkPlan.Number * 7;
                        break;
                    case Enums.PeriodType.Months:
                        days = (linkPlan.NextDate - linkPlan.BaseDate).Days;
                        break;
                }
            }

            return days;
        }

        public WorkLoadItem CreateWorkLoadItem(PlanEquipmentLink link, string currentUserName, object currentUser, int? sequence,
            DateTime? dueDate)
        {
            var workLoadItem = new WorkLoadItem(Session)
            {
                PlanNumber = link.LinkPlan != null ? link.LinkPlan.M_Plan_Num : String.Empty,
                OperationNumber = link.Operation != null ? link.Operation.M_Operation_Num : String.Empty,
                Equipment = link.LinkEquipment.EquipmentName,
                DueDate = link.LinkPlan?.NextDate ?? DateTime.MinValue,
                PlanId = link.LinkPlan?.Oid ?? Guid.Empty,
                OperationId = link.Operation?.Oid ?? Guid.Empty,
                EquipmentId = link.LinkEquipment?.Oid ?? Guid.Empty,
                UserId = Guid.Parse(currentUser.ToString()),
                UserName = currentUserName,
                WorkLoad = this,
                SeparateWorkOrderPerEquipment = link.LinkPlan?.SeparateWorkOrderPerEquipment ?? false,
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
                WorkLoad = this,
                SeparateWorkOrderPerEquipment = link.LinkPlan?.SeparateWorkOrderPerEquipment ?? false,
                Periodic = link.LinkPlan?.Usage == UsageType.Periodic,
                Threshold = link.LinkPlan?.Usage == UsageType.Threshold,
                BaseValue = counterValue ?? default
            };

            workLoadItem.Save();

            return workLoadItem;
        }
    }
}