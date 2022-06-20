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
                var fromDate = workLoad.FromDate;
                var toDate = workLoad.ToDate;
                if (fromDate <= DateTime.MinValue || toDate <= DateTime.MinValue)
                {
                    return;
                }

                var objectSpace = View.ObjectSpace;
                workLoad.Items.Clear();
                DeleteAllWorkLoadItems(objectSpace);

                var collection = objectSpace.GetObjects<PlanEquipmentLink>(CriteriaOperator.Parse(
                    "LinkPlan.Active_Plan == 'true' and LinkPlan.Freez_Plan == 'false' and LinkPlan.Plan_Status == 1 and LinkPlan.NextDate >= ? and LinkPlan.NextDate <= ?",
                    fromDate, toDate));
                foreach (PlanEquipmentLink link in collection)
                {
                    if (link.LinkPlan.FrequencyType == FrequencyType.Rolling || link.LinkPlan.FrequencyType == FrequencyType.Sequential)
                    {
                        var linkPlanNextDate = link.LinkPlan.NextDate;

                        while (linkPlanNextDate <= toDate)
                        {
                            var days = workLoad.GetPeriodInDays(link);
                            var workLoadItem = workLoad.CreateWorkLoadItem(link, SecuritySystem.CurrentUserName, SecuritySystem.CurrentUserId, linkPlanNextDate);
                            workLoad.Items.Add(workLoadItem);

                            linkPlanNextDate = linkPlanNextDate.AddDays(days);
                        }
                    }
                    else
                    {
                        var workLoadItem = workLoad.CreateWorkLoadItem(link, SecuritySystem.CurrentUserName, SecuritySystem.CurrentUserId, null);
                        workLoad.Items.Add(workLoadItem);
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
