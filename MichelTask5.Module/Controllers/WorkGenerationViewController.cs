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
            generateWorkOrder = new SimpleAction(this, "GenerateWorkOrder", PredefinedCategory.Edit)
            {
                SelectionDependencyType = SelectionDependencyType.Independent,
                Caption = "Generate Work Order",
                TargetViewId = "WorkGeneration_Items_ListView",
            };

            generateWorkOrder.Execute += GenerateWorkOrder_Execute;


            generateWorkLoad = new SimpleAction(this, "GenerateWorkLoadInWorkGeneration", PredefinedCategory.Edit)
            {
                SelectionDependencyType = SelectionDependencyType.Independent,
                Caption = "Generate Work Load",
                TargetViewId = "WorkGeneration_DetailView"
            };

            generateWorkLoad.Execute += GenerateWorkLoad_Execute;
        }

        private void GenerateWorkOrder_Execute(object sender, SimpleActionExecuteEventArgs e)
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
                    Web = { Position = InformationPosition.Top },
                    Win =
                    {
                        Caption = "Warning",
                        Type = WinMessageType.Flyout
                    }
                };

                Application.ShowViewStrategy.ShowMessage(options);
                return;
            }

            if (!(View is ListView listView))
                return;

            if (!(listView.CollectionSource is PropertyCollectionSource collectionSource))
                return;

            if (!(collectionSource.MasterObject is WorkGeneration workGeneration))
                return;

            var objectSpace = listView.ObjectSpace;
            workGeneration.Items.Clear();
            DeleteAllWorkGenerationItems(objectSpace);

            //var generatedPlans = new List<Guid>();

            var result = CheckSequences(objectsToProcess);
            if (!result.Successful)
            {
                options = new MessageOptions
                {
                    Duration = 2000,
                    Message = result.Information,
                    Type = InformationType.Warning,
                    Web = { Position = InformationPosition.Top },
                    Win =
                    {
                        Caption = "Warning",
                        Type = WinMessageType.Flyout
                    }
                };

                Application.ShowViewStrategy.ShowMessage(options);
                DeleteAllWorkGenerationItems(objectSpace);
                return;
            }

            var groupedList = objectsToProcess.Cast<WorkLoadItem>()
                .GroupBy
                (x => new
                    {
                        x.PlanId, x.DueDate
                    }
                )
                .Select(y => new
                    {
                        y.Key.PlanId, y.Key.DueDate
                    }
                );

            foreach (var obj in groupedList)
            {
                switch (workGeneration.Plan)
                {
                    case Enums.PlanType.M:
                    {
                        var plan = objectSpace.GetObjectByKey<M_Plan>(obj.PlanId);

                        //if (plan.FrequencyType == FrequencyType.Regular && generatedPlans.Contains(plan.Oid))
                        //{
                        //    continue;
                        //}

                        if (plan.SeparateWorkOrderPerEquipment && plan.Equipments.Any())
                        {
                            var taskNumber = 1;

                            foreach (var equipment in plan.Equipments)
                            {
                                var workOrder = CreateWorkOrder(objectSpace, plan, obj.DueDate, out var operation);
                                var woTask = CreateWoTask(objectSpace, operation, equipment, obj.DueDate, taskNumber);

                                workOrder.Tasks.Add(woTask);

                                taskNumber++;

                                workOrder.Save();
                                var workGenerationItem = workGeneration.CreateWorkGenerationItem(plan,
                                    SecuritySystem.CurrentUserName,
                                    SecuritySystem.CurrentUserId, workOrder);
                                workGeneration.ListOfGeneratedWorkOrders.Add(workGenerationItem);
                                objectSpace.CommitChanges();
                            }
                        }
                        else
                        {
                            var workOrder = CreateWorkOrder(objectSpace, plan, obj.DueDate, out var operation);

                            var taskNumber = 1;
                            foreach (var equipment in plan.Equipments)
                            {
                                var woTask = CreateWoTask(objectSpace, operation, equipment, obj.DueDate, taskNumber);

                                workOrder.Tasks.Add(woTask);
                                taskNumber++;
                            }

                            workOrder.Save();
                            var workGenerationItem = workGeneration.CreateWorkGenerationItem(plan,
                                SecuritySystem.CurrentUserName,
                                SecuritySystem.CurrentUserId, workOrder);
                            workGeneration.ListOfGeneratedWorkOrders.Add(workGenerationItem);
                            objectSpace.CommitChanges();
                        }

                        //generatedPlans.Add(obj.PlanId);

                        plan.Plan_Status = 2;
                        if (plan.FrequencyType == Enums.FrequencyType.Regular)
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
                        break;
                    }
                    case Enums.PlanType.C:
                    {
                        var plan = objectSpace.GetObjectByKey<C_Plan>(obj.PlanId);
                        if (plan.SeparateWorkOrderPerEquipment && plan.Equipments.Any())
                        {
                            var taskNumber = 1;

                            foreach (var equipment in plan.Equipments)
                            {
                                var workOrder = CreateWorkOrder(objectSpace, plan, obj.DueDate, out var operation);
                                var woTask = CreateWoTask(objectSpace, operation, equipment, obj.DueDate,
                                    taskNumber);

                                workOrder.Tasks.Add(woTask);

                                taskNumber++;

                                workOrder.Save();
                                var workGenerationItem = workGeneration.CreateWorkGenerationItem(plan,
                                    SecuritySystem.CurrentUserName,
                                    SecuritySystem.CurrentUserId, workOrder);
                                workGeneration.ListOfGeneratedWorkOrders.Add(workGenerationItem);
                                if (plan.Usage == UsageType.Periodic)
                                {
                                    var nextValue = plan.NextValue;
                                    var counterValue = equipment.Counter.CounterValues
                                        .FirstOrDefault(v => v.Counter_Value >= nextValue)?.Counter_Value;

                                    plan.BaseValue = counterValue.Value;
                                    plan.NextValue = plan.BaseValue + plan.PeriodValue;
                                }

                                objectSpace.CommitChanges();
                            }
                        }
                        else
                        {
                            var workOrder = CreateWorkOrder(objectSpace, plan, obj.DueDate, out var operation);

                            var taskNumber = 1;
                            foreach (var equipment in plan.Equipments)
                            {
                                var woTask = CreateWoTask(objectSpace, operation, equipment, obj.DueDate,
                                    taskNumber);

                                workOrder.Tasks.Add(woTask);
                                taskNumber++;

                                if (plan.Usage == UsageType.Periodic)
                                {
                                    var nextValue = plan.NextValue;
                                    var counterValue = equipment.Counter.CounterValues
                                        .FirstOrDefault(v => v.Counter_Value >= nextValue)?.Counter_Value;

                                    plan.BaseValue = counterValue.Value;
                                    plan.NextValue = plan.BaseValue + plan.PeriodValue;
                                }
                            }

                            workOrder.Save();
                            var workGenerationItem = workGeneration.CreateWorkGenerationItem(plan,
                                SecuritySystem.CurrentUserName,
                                SecuritySystem.CurrentUserId, workOrder);
                            workGeneration.ListOfGeneratedWorkOrders.Add(workGenerationItem);
                            objectSpace.CommitChanges();
                        }

                        if (plan.Usage == UsageType.Periodic)
                        {
                            plan.Plan_Status = 2;
                        }
                        else if (plan.Usage == UsageType.Threshold)
                        {
                            plan.Plan_Status = 3;
                        }

                        plan.LastDate = DateTime.Today;

                        break;
                    }
                }

                objectSpace.CommitChanges();
            }

            options = new MessageOptions
            {
                Duration = 2000,
                Message = "Work Order has been created",
                Type = InformationType.Success,
                Web = { Position = InformationPosition.Top },
                Win =
                {
                    Caption = "Success",
                    Type = WinMessageType.Flyout
                }
            };

            Application.ShowViewStrategy.ShowMessage(options);
        }

        private OperationResult CheckSequences(ArrayList objectsToProcess)
        {
            var result = new OperationResult()
            {
                Successful = true
            };

            var info = new StringBuilder();

            var dictionary = objectsToProcess.Cast<WorkLoadItem>().Where(o => o.Sequential).OrderBy(o => o.PlanNumber)
                .ThenBy(o => o.Sequence)
                .ToList().GroupBy(x => x.PlanNumber)
                .ToDictionary(g => g.Key, g => g.Select(x => x.Sequence).ToList());

            foreach (var item in dictionary)
            {
                for (var index = 0; index < item.Value.Count; index++)
                {
                    if (item.Value[index] != index + 1)
                    {
                        info.AppendLine($"Select precedent sequence for the plan '{item.Key}'");
                        result.Successful = false;
                        break;
                    }
                }
            }

            if (!result.Successful)
            {
                result.Information = info.ToString();
            }

            return result;
        }

        private void GenerateWorkLoad_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (View.CurrentObject is WorkGeneration workGeneration)
            {
                var objectSpace = View.ObjectSpace;
                switch (workGeneration.Plan)
                {
                    case Enums.PlanType.M:
                    {
                        var fromDate = workGeneration.FromDate;
                        var toDate = workGeneration.ToDate;
                        if (fromDate <= DateTime.MinValue || toDate <= DateTime.MinValue)
                        {
                            return;
                        }

                        workGeneration.Items.Clear();
                        DeleteAllWorkLoadItems(objectSpace);

                        var collection = objectSpace.GetObjects<PlanEquipmentLink>(CriteriaOperator.Parse(
                            "LinkPlan.Active_Plan == 'true' and LinkPlan.Freez_Plan == 'false' and LinkPlan.Plan_Status == 1 and LinkPlan.NextDate >= ? and LinkPlan.NextDate <= ?",
                            fromDate, toDate));

                        var superposedPlans = new List<Guid>();
                        foreach (PlanEquipmentLink link in collection)
                        {
                            var plan = link.LinkPlan;
                            if (plan.Superpose_Plan && plan.SuperposedPlan != null &&
                                plan.NextDate.Date == plan.SuperposedPlan.NextDate.Date)
                            {
                                superposedPlans.Add(plan.SuperposedPlan.Oid);
                            }
                        }

                        foreach (PlanEquipmentLink link in collection)
                        {
                            var linkPlan = link.LinkPlan;
                            if (!superposedPlans.Contains(linkPlan.Oid))
                            {
                                if (linkPlan.FrequencyType == Enums.FrequencyType.Rolling ||
                                    linkPlan.FrequencyType == Enums.FrequencyType.Sequential)
                                {
                                    var linkPlanNextDate = linkPlan.NextDate;
                                    var sequence = 1;
                                    while (linkPlanNextDate <= toDate)
                                    {
                                        var days = workGeneration.GetPeriodInDays(linkPlan);
                                        var workLoadItem = workGeneration.CreateWorkLoadItem(link,
                                            SecuritySystem.CurrentUserName,
                                            SecuritySystem.CurrentUserId, sequence, linkPlanNextDate);
                                        workGeneration.Items.Add(workLoadItem);

                                        linkPlanNextDate = linkPlanNextDate.AddDays(days);
                                        sequence++;
                                    }
                                }
                                else
                                {
                                    var workLoadItem = workGeneration.CreateWorkLoadItem(link,
                                        SecuritySystem.CurrentUserName,
                                        SecuritySystem.CurrentUserId, null, null);
                                    workGeneration.Items.Add(workLoadItem);
                                }
                            }
                        }

                        break;
                    }
                    case Enums.PlanType.C:
                    {
                        workGeneration.Items.Clear();
                        DeleteAllWorkLoadItems(objectSpace);

                        var collection = objectSpace.GetObjects<CPlanEquipmentLink>(CriteriaOperator.Parse(
                            "LinkPlan.Active_Plan == 'true' and LinkPlan.Freez_Plan == 'false' and LinkPlan.Plan_Status == 1"));
                        foreach (CPlanEquipmentLink link in collection)
                        {
                            if (link.LinkPlan.Usage == UsageType.Periodic)
                            {
                                var linkPlanNextValue = link.LinkPlan.NextValue;
                                var equipment = link.LinkEquipment;
                                if (equipment.Counter.CounterValues.Any(v => v.Counter_Value >= linkPlanNextValue))
                                {
                                    var counterValue = equipment.Counter.CounterValues
                                        .FirstOrDefault(v => v.Counter_Value >= linkPlanNextValue)?.Counter_Value;
                                        var workLoadItem = workGeneration.CreateWorkLoadItemFromCPlan(link,
                                        SecuritySystem.CurrentUserName,
                                        SecuritySystem.CurrentUserId, counterValue);
                                    workGeneration.Items.Add(workLoadItem);
                                }

                            }
                            else if (link.LinkPlan.Usage == UsageType.Threshold)
                            {
                                var linkPlanNextValue = link.LinkPlan.NextValue;
                                var linkPlanBaseValue = link.LinkPlan.BaseValue;
                                var equipment = link.LinkEquipment;
                                if (equipment.Counter.CounterValues.Any(v =>
                                        v.Counter_Value <= linkPlanNextValue && v.Counter_Value >= linkPlanBaseValue))
                                {
                                    var counterValue = equipment.Counter.CounterValues
                                        .FirstOrDefault((v =>
                                            v.Counter_Value <= linkPlanNextValue &&
                                            v.Counter_Value >= linkPlanBaseValue))?.Counter_Value;
                                        var workLoadItem = workGeneration.CreateWorkLoadItemFromCPlan(link,
                                        SecuritySystem.CurrentUserName,
                                        SecuritySystem.CurrentUserId, counterValue);
                                    workGeneration.Items.Add(workLoadItem);
                                }
                            }
                        }

                        break;
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

        private static WoTask CreateWoTask(IObjectSpace os, M_Operation operation, Equipment equipment, DateTime date,
            int taskNumber)
        {
            var woTask = os.CreateObject<WoTask>();
            if (operation != null)
            {
                woTask.WorkHours = operation.OperationDuration;
                woTask.WorkType = operation.WorkType;
                woTask.AssignedTo = operation.Operation_unit;
                woTask.TaskDescription = operation.Operation_Description;

                if (operation.Operation_unit != null)
                {
                    woTask.Subject = operation.Operation_Name + " " + operation.Operation_unit.Work_Unit_Name;
                }
            }

            woTask.EquipmentTask = equipment;
            woTask.PlannedEndDate = date;
            woTask.PlannedStartDate = date;
            woTask.TaskNumber = taskNumber;

            return woTask;
        }

        private static Work_Order CreateWorkOrder(IObjectSpace os, M_Plan plan, DateTime date, out M_Operation operation)
        {
            var workOrder = os.CreateObject<Work_Order>();
            operation = plan.Plan_Operation;

            workOrder.Operation = operation;
            workOrder.Work_Request = operation != null ? operation.Operation_Name : String.Empty;
            workOrder.Prefix = operation?.Prefix;
            workOrder.Site = plan.Site;
            workOrder.Plan = plan;
            workOrder.PlannedEndDate = date;
            workOrder.PlannedStartDate = date;
            workOrder.DueDate = date;
            workOrder.Request_Description = operation?.Operation_Description;
            workOrder.Work_orderDate = DateTime.Today;
            
            return workOrder;
        }
        
        private static Work_Order CreateWorkOrder(IObjectSpace os, C_Plan plan, DateTime date, out M_Operation operation)
        {
            var workOrder = os.CreateObject<Work_Order>();
            operation = plan.Plan_Operation;

            workOrder.Operation = operation;
            workOrder.Work_Request = operation != null ? operation.Operation_Name : String.Empty;
            workOrder.Prefix = operation?.Prefix;
            workOrder.Site = plan.Site;
            workOrder.CPlan = plan;
            workOrder.PlannedEndDate = date;
            workOrder.PlannedStartDate = date;
            workOrder.DueDate = date;
            workOrder.Request_Description = operation?.Operation_Description;
            workOrder.Work_orderDate = DateTime.Today;
            
            return workOrder;
        }

        private DateTime GetNextDate(M_Plan plan)
        {
            var nextDate = DateTime.Today;
            switch (plan.Period)
            {
                case Enums.PeriodType.Days:
                    nextDate = plan.BaseDate.AddDays(plan.Number);
                    break;
                case Enums.PeriodType.Weeks:
                    nextDate = plan.BaseDate.AddDays(plan.Number * 7);
                    break;
                case Enums.PeriodType.Months:
                    nextDate = plan.BaseDate.AddMonths(plan.Number);
                    break;
            }

            return nextDate;
        }

        private static void DeleteAllWorkGenerationItems(IObjectSpace os)
        {
            var colDelete = os.GetObjects<WorkGenerationItem>(CriteriaOperator.Parse("UserId = ?",
                SecuritySystem.CurrentUserId));
            foreach (var item in colDelete.ToList())
            {
                item.Delete();
            }

            os.CommitChanges();
        }
    }
}
