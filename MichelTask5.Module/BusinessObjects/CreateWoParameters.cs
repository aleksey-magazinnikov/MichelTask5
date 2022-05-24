using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;

namespace MichelTask5.Module.BusinessObjects
{
    [DomainComponent]
    public class CreateWoParameters
    {
        public CreateWoParameters(Session session)
        {
        }

        [LookupEditorMode(LookupEditorMode.AllItems)]
        public M_Operation Operation { get; set; }
    }
}
