using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.Persistent.Base;
using MichelTask5.Module.BusinessObjects;

namespace MichelTask5.Module.Controllers
{
    public class CreateWorkOrderFromOperationController: ViewController
    {
        private PopupWindowShowAction createWorkOrder;

        public CreateWorkOrderFromOperationController()
        {
            TargetObjectType = typeof(Work_Order);
            TargetViewType = ViewType.ListView;

            createWorkOrder = new PopupWindowShowAction(this, "CreateWorkOrder", PredefinedCategory.ObjectsCreation)
            {
                SelectionDependencyType = SelectionDependencyType.Independent,
                Caption = "WO From Operation",
            };

            createWorkOrder.CustomizePopupWindowParams += CreateWorkOrder_CustomizePopupWindowParams;
            createWorkOrder.Execute += CreateWorkOrder_Execute;
        }

        private void CreateWorkOrder_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var parameters = e.PopupWindow.View.CurrentObject as CreateWoParameters;
            if (parameters?.Operation == null)
            {
                return;
            }

            var os = Application.CreateObjectSpace(typeof(Work_Order));
            var operation = os.GetObject<M_Operation>(parameters.Operation);
            
            var workOrder = os.CreateObject<Work_Order>();
            workOrder.Operation = operation;
            workOrder.Work_Request = operation.Operation_Name;
            workOrder.Prefix = operation.Prefix;
            workOrder.PlannedEndDate = DateTime.Today;
            workOrder.PlannedStartDate = DateTime.Today;
            workOrder.Request_Description = operation.Operation_Description;

            var woTask = os.CreateObject<WoTask>();
            woTask.WorkHours = operation.OperationDuration;
            woTask.WorkType = operation.WorkType;
            woTask.AssignedTo = operation.Operation_unit;
            woTask.TaskDescription = operation.Operation_Description;

            if (operation.Operation_unit != null)
            {
                woTask.Subject = operation.Operation_Name + " " + operation.Operation_unit.Work_Unit_Name;
            }

            workOrder.Tasks.Add(woTask);
            workOrder.Save();

            os.CommitChanges();

            e.ShowViewParameters.CreatedView = Application.CreateDetailView(os, workOrder, true);
            e.ShowViewParameters.Context = TemplateContext.View;
            e.ShowViewParameters.CreatedView.Closed += CreatedView_Closed;
        }

        private void CreatedView_Closed(object sender, EventArgs e)
        {
            View.Refresh(true);
        }

        private void CreateWorkOrder_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            IObjectSpace os = Application.CreateObjectSpace(typeof(Work_Order));
            e.Context = TemplateContext.PopupWindow;
            e.View = Application.CreateDetailView(os, new CreateWoParameters(((DevExpress.ExpressApp.Xpo.XPObjectSpace)os).Session));
            ((DetailView)e.View).ViewEditMode = ViewEditMode.Edit;
        }
    }
}
