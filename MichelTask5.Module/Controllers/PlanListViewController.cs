using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using MichelTask5.Module.BusinessObjects;

namespace MichelTask5.Module.Controllers
{
    public class PlanListViewController: ObjectViewController<ListView, M_Plan>
    {
        private PopupWindowShowAction shiftPlanAction;

        public PlanListViewController()
        {
            shiftPlanAction = new PopupWindowShowAction(this, "ShiftPlan", PredefinedCategory.Edit)
            {
                SelectionDependencyType = SelectionDependencyType.RequireSingleObject,
                Caption = "Shift Plan",
                TargetObjectsCriteria = "PlanSequence != null"
            };

            shiftPlanAction.CustomizePopupWindowParams += ShiftPlanAction_CustomizePopupWindowParams;
            shiftPlanAction.Execute += ShiftPlanAction_Execute; ;
        }

        private void ShiftPlanAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var parameters = e.PopupWindow.View.CurrentObject as ShiftPlanParameters;
            if (parameters == null || parameters.Days == 0)
            {
                return;
            }

            if (e.CurrentObject is M_Plan mPlan)
            {
                var os = Application.CreateObjectSpace(typeof(M_Plan));
                var objectsToPrecess = os.GetObjectsQuery<M_Plan>().Where(p => p.PlanSequence != null && p.PlanSequence.SequenceId >= mPlan.PlanSequence.SequenceId);
                foreach (var plan in objectsToPrecess)
                {
                    plan.NextDate += TimeSpan.FromDays(parameters.Days);
                    if (plan.PlanSettings != null)
                    {
                        plan.NextDate = Utils.GetNextDateBasedOnPlanSettings(plan.NextDate, plan.PlanSettings);
                    }
                }
                
                os.CommitChanges();
                View.Refresh(true);

                if (objectsToPrecess.Any())
                {
                    var options = new MessageOptions
                    {
                        Duration = 2000,
                        Message = $"{objectsToPrecess.Count()} plans have been shifted",
                        Type = InformationType.Success,
                        Web = { Position = InformationPosition.Top },
                        Win =
                        {
                            Caption = "Info",
                            Type = WinMessageType.Flyout
                        }
                    };

                    Application.ShowViewStrategy.ShowMessage(options);
                }

            }
        }

        private void ShiftPlanAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var os = Application.CreateObjectSpace(typeof(M_Plan));
            e.Context = TemplateContext.PopupWindow;
            e.View = Application.CreateDetailView(os, new ShiftPlanParameters(((DevExpress.ExpressApp.Xpo.XPObjectSpace)os).Session));
            ((DetailView)e.View).ViewEditMode = ViewEditMode.Edit;
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            foreach (IModelColumn column in View.Model.Columns)
            {
                if (column.PropertyName == "PlanSequence")
                {
                    column.SortIndex = 0;
                    column.SortOrder = DevExpress.Data.ColumnSortOrder.Ascending;
                }
                else
                {
                    column.SortIndex = -1;
                    column.SortOrder = DevExpress.Data.ColumnSortOrder.None;
                }
            }
        }
    }
}
