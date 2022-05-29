using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.XtraPrinting.Native;
using MichelTask5.Module.BusinessObjects;

namespace MichelTask5.Module.Controllers
{
    public class WorkGenerationViewController : ViewController
    {
        private SimpleAction generateWorkOrder;
        private SimpleAction generateWorkLoad;

        public WorkGenerationViewController()
        {
            TargetObjectType = typeof(WorkGeneration);

            generateWorkOrder = new SimpleAction(this, "GenerateWorkOrder", PredefinedCategory.Edit)
            {
                SelectionDependencyType = SelectionDependencyType.Independent,
                Caption = "Generate Work Order",
                TargetViewId = "WorkGeneration_Items_ListView"
            };

            generateWorkOrder.Execute += GenerateWork_Execute;


            generateWorkLoad = new SimpleAction(this, "GenerateWorkLoadInWorkGeneration", PredefinedCategory.Edit)
            {
                SelectionDependencyType = SelectionDependencyType.Independent,
                Caption = "Generate Work Load",
                TargetViewId = "WorkGeneration_DetailView"
            };

            generateWorkLoad.Execute += GenerateWorkLoad_Execute;
        }

        private void GenerateWork_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var objectsToProcess = new ArrayList(e.SelectedObjects);
            MessageOptions options;
            if (objectsToProcess.Count == 0)
            {
                options = new MessageOptions
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
                return;
            }

            foreach (WorkLoadItem obj in objectsToProcess)
            {
                var os = Application.CreateObjectSpace(typeof(Work_Order));
                var plan = os.GetObjectByKey<M_Plan>(obj.PlanId);

                if (plan.SeparateWorkOrderPerEquipment && plan.Equipments.Any())
                {
                    var taskNumber = 1;

                    foreach (var equipment in plan.Equipments)
                    {
                        var workOrder = CreateWorkOrder(os, plan, obj, out var operation);
                        var woTask = CreateWoTask(os, operation, equipment, obj, taskNumber);

                        workOrder.Tasks.Add(woTask);

                        taskNumber++;

                        workOrder.Save();
                    }
                }
                else
                {
                    var workOrder = CreateWorkOrder(os, plan, obj, out var operation);

                    var taskNumber = 1;
                    foreach (var equipment in plan.Equipments)
                    {
                        var woTask = CreateWoTask(os, operation, equipment, obj, taskNumber);

                        workOrder.Tasks.Add(woTask);
                        taskNumber++;
                    }

                    workOrder.Save();
                }

                plan.Plan_Status = 2;
                if (!plan.Rolling_Plan)
                {
                    plan.BaseDate = DateTime.Today;
                }
                else
                {
                    if (plan.BaseDate < obj.DueDate)
                    {
                        plan.BaseDate = obj.DueDate;
                    }
                }

                plan.LastDate = DateTime.Today;
                plan.NextDate = GetNextDate(plan);

                os.CommitChanges();
            }

            options = new MessageOptions
            {
                Duration = 2000,
                Message = "Work Order has been created",
                Type = InformationType.Success,
                Web = {Position = InformationPosition.Top},
                Win =
                {
                    Caption = "Success",
                    Type = WinMessageType.Flyout
                }
            };

            Application.ShowViewStrategy.ShowMessage(options);
        }
        private void GenerateWorkLoad_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (View.CurrentObject is WorkGeneration workGeneration)
            {
                var fromDate = workGeneration.FromDate;
                var toDate = workGeneration.ToDate;
                if (fromDate <= DateTime.MinValue || toDate <= DateTime.MinValue)
                {
                    return;
                }

                var objectSpace = View.ObjectSpace;
                workGeneration.Items.Clear();
                DeleteAllWorkLoadItems(objectSpace);

                var os = Application.CreateObjectSpace(typeof(WorkLoad));
                var collection = os.GetObjects<M_Plan>(CriteriaOperator.Parse(
                    "Active_Plan == 'true' and Plan_Status == 1 and NextDate >= ? and NextDate <= ?",
                    fromDate, toDate));
                foreach (M_Plan plan in collection)
                {
                    if (plan.Rolling_Plan)
                    {
                        var linkPlanNextDate = plan.NextDate;

                        while (linkPlanNextDate <= toDate)
                        {
                            var days = workGeneration.GetPeriodInDays(plan);
                            var workLoadItem = workGeneration.CreateWorkLoadItem(plan, SecuritySystem.CurrentUserName, SecuritySystem.CurrentUserId, linkPlanNextDate);
                            workGeneration.Items.Add(workLoadItem);

                            linkPlanNextDate = linkPlanNextDate.AddDays(days);
                        }
                    }
                    else
                    {
                        var workLoadItem = workGeneration.CreateWorkLoadItem(plan, SecuritySystem.CurrentUserName, SecuritySystem.CurrentUserId, null);
                        workGeneration.Items.Add(workLoadItem);
                    }
                }

                objectSpace.CommitChanges();
            }
        }

        private static void DeleteAllWorkLoadItems(IObjectSpace os)
        {
            var colDelete = os.GetObjects<WorkLoadItem>(CriteriaOperator.Parse("UserId = ?",
                SecuritySystem.CurrentUserId));
            foreach (var item in colDelete.ToList())
            {
                item.Delete();
            }

            os.CommitChanges();
        }

        private static WoTask CreateWoTask(IObjectSpace os, M_Operation operation, Equipment equipment, WorkLoadItem obj,
            int taskNumber)
        {
            var woTask = os.CreateObject<WoTask>();
            woTask.WorkHours = operation.OperationDuration;
            woTask.WorkType = operation.WorkType;
            woTask.AssignedTo = operation.Operation_unit;
            woTask.EquipmentTask = equipment;
            woTask.TaskDescription = operation.Operation_Description;
            woTask.PlannedEndDate = obj.DueDate;
            woTask.PlannedStartDate = obj.DueDate;
            woTask.TaskNumber = taskNumber;

            if (operation.Operation_unit != null)
            {
                woTask.Subject = operation.Operation_Name + " " + operation.Operation_unit.Work_Unit_Name;
            }

            return woTask;
        }

        private static Work_Order CreateWorkOrder(IObjectSpace os, M_Plan plan, WorkLoadItem obj, out M_Operation operation)
        {
            var workOrder = os.CreateObject<Work_Order>();
            operation = plan.Plan_Operation;

            workOrder.Operation = operation;
            workOrder.Work_Request = operation.Operation_Name;
            workOrder.Prefix = operation.Prefix;
            workOrder.Site = plan.Site;
            workOrder.Plan = plan;
            workOrder.PlannedEndDate = obj.DueDate;
            workOrder.PlannedStartDate = obj.DueDate;
            workOrder.DuetDate = obj.DueDate;
            workOrder.Request_Description = operation.Operation_Description;
            workOrder.Work_orderDate = DateTime.Today;
            
            return workOrder;
        }

        private DateTime GetNextDate(M_Plan plan)
        {
            var nextDate = DateTime.Today;
            switch (plan.Period)
            {
                case PeriodType.Days:
                    nextDate = plan.BaseDate.AddDays(plan.Number);
                    break;
                case PeriodType.Weeks:
                    nextDate = plan.BaseDate.AddDays(plan.Number * 7);
                    break;
                case PeriodType.Months:
                    nextDate = plan.BaseDate.AddMonths(plan.Number);
                    break;
            }

            return nextDate;
        }
    }
}
