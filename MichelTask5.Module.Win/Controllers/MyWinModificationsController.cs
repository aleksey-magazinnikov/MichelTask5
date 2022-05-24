using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.SystemModule;

namespace MichelTask5.Module.Win.Controllers
{
    public class MyWinModificationsController : WinModificationsController
    {
        public MyWinModificationsController()
        {
            TargetViewNesting = Nesting.Any;
        }

        protected override void UpdateActionState()
        {
            base.UpdateActionState();
            this.SaveAction.Active["IsRoot"] = !(this.Frame is NestedFrame);
        }
    }
}
