using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
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
            set { SetPropertyValue(nameof(fromDate), ref fromDate, value); }
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

        PermissionPolicyUser GetCurrentUser() => Session.GetObjectByKey<PermissionPolicyUser>(SecuritySystem.CurrentUserId);

        private BindingList<WorkLoadItem> _items = null;

        [CollectionOperationSet(AllowAdd = false, AllowRemove = false)]
        public BindingList<WorkLoadItem> Items => _items ?? (_items = new BindingList<WorkLoadItem>());

        public int GetPeriodInDays(PlanEquipmentLink link)
        {
            int days = 0;
            if (link != null)
            {
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
                Sequential = link.LinkPlan != null && link.LinkPlan.FrequencyType == FrequencyType.Sequential

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
    }
}