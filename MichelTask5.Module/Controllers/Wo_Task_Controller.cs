using System;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using MichelTask5.Module.BusinessObjects;

namespace MichelTask5.Module.Controllers
{
    public class WoTaskController : ViewController
    {
        public WoTaskController()
        {
            TargetObjectType = typeof(WoTask);
            TargetViewType = ViewType.Any;
            SimpleAction markCompletedAction = new SimpleAction(
                this, "MarkCompleted",
                DevExpress.Persistent.Base.PredefinedCategory.RecordEdit)
            {
                TargetObjectsCriteria =
                    (CriteriaOperator.Parse("Status != ?", WoTaskStatus.Completed)).ToString(),
                ConfirmationMessage =
                    "Are you sure you want to mark the selected task(s) as 'Completed'?",
                ImageName = "State_Task_Completed"
            };
            markCompletedAction.Execute += OnMarkCompletedActionOnExecute;
        }

        private void OnMarkCompletedActionOnExecute(object s, SimpleActionExecuteEventArgs e)
        {
            var objectSpace = Application.CreateObjectSpace(typeof(WoTask));
            foreach (WoTask task in e.SelectedObjects)
            {
                var woTask = objectSpace.GetObject(task);
                woTask.EndDate = DateTime.Now;
                woTask.Status = WoTaskStatus.Completed;
            }

            objectSpace.CommitChanges();
            View.Refresh(true);
        }
    }
}