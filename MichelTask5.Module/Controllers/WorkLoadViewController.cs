using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using MichelTask5.Module.BusinessObjects;

namespace MichelTask5.Module.Controllers
{
    public class WorkLoadViewController : ViewController
    {
        private SimpleAction generateWorkLoad;

        public WorkLoadViewController()
        {
            TargetViewId = "WorkLoad_DetailView";

            generateWorkLoad = new SimpleAction(this, "GenerateWorkLoad", PredefinedCategory.Edit)
            {
                SelectionDependencyType = SelectionDependencyType.Independent,
                Caption = "Generate Work Load",
            };

            generateWorkLoad.Execute += GenerateWorkLoad_Execute;
        }

        private void GenerateWorkLoad_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (View.CurrentObject is WorkLoad workLoad)
            {
                var objectSpace = View.ObjectSpace;

                switch (workLoad.Plan)
                {
                    case Enums.PlanType.M:
                    {
                        var fromDate = workLoad.FromDate;
                        var toDate = workLoad.ToDate;
                        if (fromDate <= DateTime.MinValue || toDate <= DateTime.MinValue)
                        {
                            return;
                        }

                        workLoad.Items.Clear();
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
                            var plan = link.LinkPlan;
                            if (!superposedPlans.Contains(plan.Oid))
                            {
                                if (plan.FrequencyType == Enums.FrequencyType.Rolling ||
                                    plan.FrequencyType == Enums.FrequencyType.Sequential)
                                {
                                    var linkPlanNextDate = plan.NextDate;
                                    var sequence = 1;
                                    while (linkPlanNextDate <= toDate)
                                    {
                                        var days = workLoad.GetPeriodInDays(plan);
                                        var workLoadItem = workLoad.CreateWorkLoadItem(link,
                                            SecuritySystem.CurrentUserName,
                                            SecuritySystem.CurrentUserId, sequence, linkPlanNextDate);
                                        workLoad.Items.Add(workLoadItem);

                                        linkPlanNextDate = linkPlanNextDate.AddDays(days);
                                        sequence++;
                                    }
                                }
                                else
                                {
                                    var workLoadItem = workLoad.CreateWorkLoadItem(link, SecuritySystem.CurrentUserName,
                                        SecuritySystem.CurrentUserId, null, null);
                                    workLoad.Items.Add(workLoadItem);
                                }
                            }
                        }

                        break;
                    }
                    case Enums.PlanType.C:
                    {
                        workLoad.Items.Clear();
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
                                    var workLoadItem = workLoad.CreateWorkLoadItemFromCPlan(link,
                                        SecuritySystem.CurrentUserName,
                                        SecuritySystem.CurrentUserId, counterValue);
                                    workLoad.Items.Add(workLoadItem);
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
                                    var workLoadItem = workLoad.CreateWorkLoadItemFromCPlan(link,
                                        SecuritySystem.CurrentUserName,
                                        SecuritySystem.CurrentUserId, counterValue);
                                    workLoad.Items.Add(workLoadItem);
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
    }
}
