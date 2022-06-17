using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.Persistent.Base;
using MichelTask5.Module.BusinessObjects;

namespace MichelTask5.Module.Controllers
{
    public class ShiftWorkOrderViewController : ViewController
    {
        private PopupWindowShowAction shiftWorkOrder;

        public ShiftWorkOrderViewController()
        {
            TargetObjectType = typeof(Work_Order);
            TargetViewType = ViewType.ListView;

            shiftWorkOrder = new PopupWindowShowAction(this, "ShiftWorkOrder", PredefinedCategory.Edit)
            {
                SelectionDependencyType = SelectionDependencyType.Independent,
                Caption = "Shift Work Order",
            };

            shiftWorkOrder.CustomizePopupWindowParams += ShiftWorkOrder_CustomizePopupWindowParams;
            shiftWorkOrder.Execute += ShiftWorkOrder_Execute;
        }

        private void ShiftWorkOrder_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var parameters = e.PopupWindow.View.CurrentObject as ShiftWoParameters;
            if (parameters?.DateTime == null)
            {
                return;
            }

            var objectsToProcess = new ArrayList(e.SelectedObjects);
            if (objectsToProcess.Count == 0)
            {
                var options = new MessageOptions
                {
                    Duration = 2000,
                    Message = "Select at least on record",
                    Type = InformationType.Warning,
                    Web = {Position = InformationPosition.Top},
                    Win =
                    {
                        Caption = "Warning",
                        Type = WinMessageType.Flyout
                    }
                };

                Application.ShowViewStrategy.ShowMessage(options);
            }
            else
            {
                foreach (Work_Order workOrder in objectsToProcess)
                {
                    if (workOrder.Status == WoTaskStatus.Completed)
                    {
                        continue;
                    }

                    workOrder.PlannedEndDate = parameters.DateTime;
                    workOrder.PlannedStartDate = parameters.DateTime;
                    workOrder.DueDate = parameters.DateTime;

                    foreach (var task in workOrder.Tasks)
                    {
                        if (task.Status == WoTaskStatus.Completed)
                        {
                            continue;
                        }

                        task.PlannedStartDate = parameters.DateTime;
                        task.PlannedEndDate = parameters.DateTime;
                    }
                }

                View.ObjectSpace.CommitChanges();
            }
        }

        private void ShiftWorkOrder_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var os = Application.CreateObjectSpace(typeof(Work_Order));
            e.Context = TemplateContext.PopupWindow;
            e.View = Application.CreateDetailView(os, new ShiftWoParameters(((DevExpress.ExpressApp.Xpo.XPObjectSpace)os).Session));
            ((DetailView)e.View).ViewEditMode = ViewEditMode.Edit;
        }
    }
}
