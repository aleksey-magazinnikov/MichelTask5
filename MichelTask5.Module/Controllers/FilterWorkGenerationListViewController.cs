using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using MichelTask5.Module.BusinessObjects;

namespace MichelTask5.Module.Controllers
{
    public class FilterWorkGenerationListViewController : ObjectViewController<ListView, WorkGeneration>
    {
        protected override void OnActivated()
        {
            base.OnActivated();
            View.CollectionSource.Criteria["Filter2"] = CriteriaOperator.Parse("User.Oid = CurrentUserId()");
        }
    }
}
