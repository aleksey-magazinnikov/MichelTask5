using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace MichelTask5.Module.BusinessObjects
{
    [Appearance("Completed1", TargetItems = "Work_Request", Criteria = "Status = 'Completed'", FontStyle = FontStyle.Strikeout, FontColor = "ForestGreen")]
    [Appearance("Completed2", TargetItems = "*;Status;AssignedToDepartment", Criteria = "Status = 'Completed'", Enabled = false)]
    [Appearance("InProgress", TargetItems = "Work_Request;AssignedToDepartment", Criteria = "Status = 'InProgress'", BackColor = "LemonChiffon")]
    [Appearance("Deferred", TargetItems = "Work_Request", Criteria = "Status = 'Deferred'", BackColor = "MistyRose")]
    [Appearance("NotCompleted", TargetItems = "*", Criteria = "Status != 'Completed' and DueDate < Today()", FontColor = "Red")]
    [NavigationItem("Maintenance")]
    [RuleCriteria("EndDate >= StartDate")]
    public class Work_Order :BaseObject
    {
        public Work_Order(Session session) : base(session) { }
        string work_Request;
        public string Work_Request
        {
            get { return work_Request; }
            set { SetPropertyValue(nameof(Work_Request), ref work_Request, value); }
        }
        string request_Description;
        [Size(SizeAttribute.Unlimited)]
        public string Request_Description
        {
            get { return request_Description; }
            set { SetPropertyValue(nameof(Request_Description), ref request_Description, value); }
        }
        [Association, Aggregated]
        public XPCollection<WoTask> Tasks
        {
            get { return GetCollection<WoTask>(nameof(Tasks)); }
        }
        WorkDepartment assignedToDepartment;
        
        [ImmediatePostData(true)]
        public WorkDepartment AssignedToDepartment
        {
            get { return assignedToDepartment; }
            set { SetPropertyValue(nameof(AssignedToDepartment), ref assignedToDepartment, value); }
        }
        DateTime work_orderDate;
        public DateTime Work_orderDate
        {
            get { return work_orderDate; }
            set { SetPropertyValue(nameof(work_orderDate), ref work_orderDate, value); }
        }
        
        WoTaskStatus status;

        public WoTaskStatus Status
        {
            get
            {
                if (!IsLoading && !IsSaving && status == WoTaskStatus.NotStarted)
                {
                    UpdateTasksStatus(false);
                }

                return status;
            }
        }

        WorkSite site;
        public WorkSite Site
        {
            get { return site; }
            set { SetPropertyValue(nameof(Site), ref site, value); }
        }
        
        M_Operation operation;
        [ModelDefault("AllowEdit", "False")]
        public M_Operation Operation
        {
            get { return operation; }
            set { SetPropertyValue(nameof(Operation), ref operation, value); }
        }
        
        M_Plan plan;
        [ModelDefault("AllowEdit", "False")]
        public M_Plan Plan
        {
            get { return plan; }
            set { SetPropertyValue(nameof(M_Plan), ref plan, value); }
        }

        C_Plan cPlan;
        [ModelDefault("AllowEdit", "False")]
        public C_Plan CPlan
        {
            get { return cPlan; }
            set { SetPropertyValue(nameof(CPlan), ref cPlan, value); }
        }

        WO_Prefix prefix;
        public WO_Prefix Prefix
        {
            get { return prefix; }
            set { SetPropertyValue(nameof(Prefix), ref prefix, value); }
        }
        string wO_Num;
        [Size(8), ModelDefault("AllowEdit", "False")]
        public string WO_Num
        {
            get { return wO_Num; }
            set { SetPropertyValue(nameof(WO_Num), ref wO_Num, value); }
        }
        string workOrder_Number;
        [Size(17), ModelDefault("AllowEdit", "False")]
        public string WorkOrder_Number
        {
            get { return workOrder_Number; }
            set { SetPropertyValue(nameof(WorkOrder_Number), ref workOrder_Number, value); }
        }

        private int? wO_Tasks = null;
        public int? WO_Tasks
        {
            get
            {
                if (!IsLoading && !IsSaving && wO_Tasks == null)
                {
                    UpdateTasksCount(false);
                    WorkOrder_Number = GenerateWorkNumber();
                }

                return wO_Tasks;
            }
        }

        private DateTime plannedstartDate;
        public DateTime PlannedStartDate
        {
            get { return plannedstartDate; }
            set { SetPropertyValue(nameof(PlannedStartDate), ref plannedstartDate, value); }
        }

        DateTime startDate;
        public DateTime StartDate
        {
            get { return startDate; }
            set { SetPropertyValue(nameof(StartDate), ref startDate, value); }
        }

        DateTime dueDate;
        public DateTime DueDate
        {
            get { return dueDate; }
            set { SetPropertyValue(nameof(DueDate), ref dueDate, value); }
        }

        private DateTime fPlannedEndDate;
        public DateTime PlannedEndDate
        {
            get { return fPlannedEndDate; }
            set { SetPropertyValue(nameof(PlannedEndDate), ref fPlannedEndDate, value); }
        }

        private DateTime fEndDate;
        public DateTime EndDate
        {
            get { return fEndDate; }
            set { SetPropertyValue(nameof(EndDate), ref fEndDate, value); }
        }

        private int _Sequence;

        [Browsable(false)]
        public int Sequence
        {
            get { return _Sequence; }
            set { SetPropertyValue("Sequence", ref _Sequence, value); }
        }
        
        protected override void OnLoaded()
        {
            Reset();
            base.OnLoaded();
        }

        private void Reset()
        {
            wO_Tasks = null;
        }

        private string GenerateWorkNumber()
        {
            return Prefix != null ? Prefix.Work_Prefix_Code + "-" + WO_Num + "-" + WO_Tasks : null;
        }

        private DateTime GetNextDate(M_Plan p)
        {
            var nextDate = DateTime.Today;
            switch (p.Period)
            {
                case Enums.PeriodType.Days:
                    nextDate = p.BaseDate.AddDays(plan.Number);
                    break;
                case Enums.PeriodType.Weeks:
                    nextDate = p.BaseDate.AddDays(plan.Number * 7);
                    break;
                case Enums.PeriodType.Months:
                    nextDate = p.BaseDate.AddMonths(plan.Number);
                    break;
            }

            return nextDate;
        }

        private void UpdateSequence()
        {
            if (Session.IsNewObject(this))
            {
                var max = Session.Evaluate(Session.GetClassInfo(GetType()), CriteriaOperator.Parse("Max(Sequence)"),
                    null);
                if (max == null)
                {
                    Sequence = 1;
                }
                else
                {
                    Sequence = Convert.ToInt32(max) + 1;
                }

                WO_Num = DateTime.Today.ToString("yy") + $"{Sequence:000000}";
            }
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            UpdateSequence();
        }

        protected override void OnSaving()
        {
            base.OnSaving();
            WorkOrder_Number = GenerateWorkNumber();
            UpdateSequence();
            if (plan != null && Status == WoTaskStatus.Completed)
            {
                plan.Plan_Status = 1;
                if (plan.FrequencyType == Enums.FrequencyType.Regular)
                {
                    plan.BaseDate = fEndDate;
                    plan.NextDate = GetNextDate(plan);
                }
            }
            if (cPlan != null && Status == WoTaskStatus.Completed && cPlan.Usage == UsageType.Periodic)
            { 
                cPlan.Plan_Status = 1;
            }
        }

        public void UpdateTasksCount(bool forceChangeEvents)
        {
            int? oldTasksCount = wO_Tasks;
            wO_Tasks = Convert.ToInt32(Evaluate(CriteriaOperator.Parse("Tasks.Count")));
            if (forceChangeEvents)
                OnChanged(nameof(WO_Tasks), oldTasksCount, wO_Tasks);
        }

        public void UpdateTasksStatus(bool forceChangeEvents)
        {
            var oldTasksStatus = status;
            var oldEndDate = fEndDate;
            status = WoTaskStatus.NotStarted;
            if (Tasks.FirstOrDefault(t => t.Status == WoTaskStatus.Deferred) != null)
            {
                status = WoTaskStatus.Deferred;
            }
            else if (Tasks.FirstOrDefault(t => t.Status == WoTaskStatus.InProgress) != null)
            {
                status = WoTaskStatus.InProgress;
            }
            else if (Tasks.Any() && Tasks.All(t => t.Status == WoTaskStatus.Completed))
            {
                status = WoTaskStatus.Completed;
                fEndDate = Tasks.Max(t => t.EndDate);
            }

            if (forceChangeEvents)
            {
                OnChanged(nameof(Status), oldTasksStatus, status);
            }
        }

        public void UpdatePlannedEndDate(bool forceChangeEvents)
        {
            DateTime oldDate = fPlannedEndDate;
            var firstOrDefault = Tasks.FirstOrDefault();
            if (firstOrDefault != null)
            {
                DateTime tempDate = firstOrDefault.PlannedEndDate;
                foreach (var task in Tasks)
                    if (task.PlannedEndDate > tempDate)
                        tempDate = task.PlannedEndDate;
                fPlannedEndDate = tempDate;
                if (forceChangeEvents)
                    OnChanged(nameof(PlannedEndDate), oldDate, fPlannedEndDate);
            }
        }

        public void UpdatePlannedStartDate(bool forceChangeEvents)
        {
            DateTime oldDate = plannedstartDate;
            var firstOrDefault = Tasks.FirstOrDefault();
            if (firstOrDefault != null)
            {
                DateTime tempDate = firstOrDefault.PlannedStartDate;
                foreach (var task in Tasks)
                    if (task.PlannedStartDate > tempDate)
                        tempDate = task.PlannedStartDate;
                plannedstartDate = tempDate;
                if (forceChangeEvents)
                    OnChanged(nameof(PlannedStartDate), oldDate, plannedstartDate);
            }
        }
    }
    public enum WoTaskStatus
    {
        NotStarted = 0,
        InProgress = 1,
        Completed = 2,
        Deferred = 3
    }
}
