using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.ExpressApp.DC;
using DevExpress.Xpo;

namespace MichelTask5.Module.Controllers
{
    [DomainComponent]
    public class ShiftPlanParameters
    {
        public ShiftPlanParameters(Session session)
        {
        }

        public int Days { get; set; }
    }
}
