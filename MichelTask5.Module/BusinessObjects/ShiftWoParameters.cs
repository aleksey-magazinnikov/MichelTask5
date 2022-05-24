using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;

namespace MichelTask5.Module.BusinessObjects
{
    [DomainComponent]
    public class ShiftWoParameters
    {
        public ShiftWoParameters(Session session)
        {
        }

        public DateTime DateTime { get; set; }
    }
}
