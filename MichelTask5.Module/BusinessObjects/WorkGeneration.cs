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
  [RuleObjectExists("AnotherWorkGenerationExists", DefaultContexts.Save, "User.Oid == CurrentUserId()", InvertResult = true,
      CustomMessageTemplate = "Another WorkGeneration already exists.")]
  [RuleCriteria("CannotDeleteWorkGeneration", DefaultContexts.Delete, "False",
      CustomMessageTemplate = "Cannot delete WorkGeneration.")]
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
