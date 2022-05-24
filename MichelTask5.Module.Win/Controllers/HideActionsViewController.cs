using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;

namespace MichelTask5.Module.Win.Controllers
{
    public class HideActionsViewController : ViewController
    {
        public HideActionsViewController()
        {
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            
            if (View.Id == "M_Plan_Equipments_ListView")
            {
                Frame.GetController<NewObjectViewController>().NewObjectAction.Active.SetItemValue("hidenewaction", false);
                Frame.GetController<DeleteObjectsViewController>().DeleteAction.Active.SetItemValue("hidedeleteaction", false);
            }

            if (View.Id == "Equipment_LookupListView")
            {
                Frame.GetController<NewObjectViewController>().NewObjectAction.Active.SetItemValue("hidenewaction", false);
            }

        }

        protected override void OnDeactivated()
        {
            base.OnDeactivated();
        }
    }
}
