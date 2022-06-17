using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace MichelTask5.Module.BusinessObjects
{
    [NavigationItem("Maintenance")]
    //
    // ...
    [Appearance("Completed1", TargetItems = "Subject",
        Criteria = "Status = 'Completed'", FontStyle = FontStyle.Strikeout, FontColor = "ForestGreen")]
    [Appearance("Completed2", TargetItems = "*;Status;AssignedTo",
        Criteria = "Status = 'Completed'", Enabled = false)]
    [Appearance("InProgress", TargetItems = "Subject;AssignedTo",
        Criteria = "Status = 'InProgress'", BackColor = "LemonChiffon")]
    [Appearance("Deferred", TargetItems = "Subject",
        Criteria = "Status = 'Deferred'", BackColor = "MistyRose")]
    [RuleCriteria("EndDate >= StartDate")]
    //
    public class WoTask : BaseObject
    {
        public WoTask(Session session) : base(session)
        {
        }

        string subject;

        [Size(255)]
        public string Subject
        {
            get { return subject; }
            set { SetPropertyValue(nameof(Subject), ref subject, value); }
        }

        WoTaskStatus status;

        public WoTaskStatus Status
        {
            get { return status; }
            set
            {
                bool modified = SetPropertyValue(nameof(Status), ref status, value);
                if (!IsLoading && !IsSaving && Work_Order != null && modified)
                {
                    Work_Order.UpdateTasksStatus(true);
                }
            }
        }

        DateTime plannedstartDate;

        public DateTime PlannedStartDate
        {
            get { return plannedstartDate; }
            set
            {
                bool modified = SetPropertyValue(nameof(PlannedStartDate), ref plannedstartDate, value);
                if (!IsLoading && !IsSaving && Work_Order != null && modified)
                {
                    Work_Order.UpdatePlannedStartDate(true);
                }

            }
        }

        DateTime startDate;

        public DateTime StartDate
        {
            get { return startDate; }
            set { SetPropertyValue(nameof(StartDate), ref startDate, value); }
        }

        DateTime plannedendDate;

        public DateTime PlannedEndDate
        {
            get { return plannedendDate; }
            set
            {
                bool modified = SetPropertyValue(nameof(PlannedEndDate), ref plannedendDate, value);
                if (!IsLoading && !IsSaving && Work_Order != null && modified)
                {
                    Work_Order.UpdatePlannedEndDate(true);
                }
            }
        }

        DateTime endDate;

        public DateTime EndDate
        {
            get { return endDate; }
            set { SetPropertyValue(nameof(EndDate), ref endDate, value); }
        }

        string taskDescription;

        [Size(SizeAttribute.Unlimited)]
        public string TaskDescription
        {
            get { return taskDescription; }
            set { SetPropertyValue(nameof(TaskDescription), ref taskDescription, value); }
        }

        Work_Order work_Order;

        [Association]
        public Work_Order Work_Order
        {
            get { return work_Order; }
            set
            {
                SetPropertyValue(nameof(Work_Order), ref work_Order, value);

                Work_Order oldWorkOrder = work_Order;
                bool modified = SetPropertyValue(nameof(Work_Order), ref work_Order, value);
                if (!IsLoading && !IsSaving && !ReferenceEquals(oldWorkOrder, work_Order) && modified)
                {
                    oldWorkOrder = oldWorkOrder ?? work_Order;
                    oldWorkOrder.UpdateTasksCount(true);
                    // oldWorkOrder.UpdateTasksStatus(true);
                }
            }
        }

        Work_Unit assignedTo;

        [DataSourceProperty("Work_Order.AssignedToDepartment.WorkUnits", DataSourcePropertyIsNullMode.SelectAll)]
        public Work_Unit AssignedTo
        {
            get { return assignedTo; }
            set { SetPropertyValue(nameof(AssignedTo), ref assignedTo, value); }
        }

        Work_Type workType;

        [ImmediatePostData(true)]
        public Work_Type WorkType
        {
            get { return workType; }
            set { SetPropertyValue(nameof(WorkType), ref workType, value); }
        }

        Work_Cause workCause;

        [DataSourceProperty("WorkType.WorkCauses", DataSourcePropertyIsNullMode.SelectAll)]
        public Work_Cause WorkCause
        {
            get { return workCause; }
            set { SetPropertyValue(nameof(WorkCause), ref workCause, value); }
        }

        private decimal workHours;

        public decimal WorkHours
        {
            get { return workHours; }
            set { SetPropertyValue(nameof(WorkHours), ref workHours, value); }
        }

        string taskReport;

        [Size(SizeAttribute.Unlimited)]
        public string TaskReport
        {
            get { return taskReport; }
            set { SetPropertyValue(nameof(TaskReport), ref taskReport, value); }
        }

        Equipment equipmentTask;

        public Equipment EquipmentTask
        {
            get { return equipmentTask; }
            set { SetPropertyValue(nameof(EquipmentTask), ref equipmentTask, value); }
        }

        private int _taskNumber;

        public int TaskNumber
        {
            get { return _taskNumber; }
            set { SetPropertyValue("TaskNumber", ref _taskNumber, value); }
        }

        protected override void OnSaving()
        {
            base.OnSaving();
            if (Session.IsNewObject(this) && TaskNumber == 0)
            {
                var tasks = Session.Query<WoTask>().ToList()
                    .Where(t => ReferenceEquals(t.Work_Order, work_Order)).ToList();
                if (tasks.Any())
                {
                    var max = tasks.Max(t => t.TaskNumber);
                    TaskNumber = Convert.ToInt32(max) + 1;
                }
                else
                {
                    TaskNumber = 1;
                }
            }
        }
    }
}
