using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace MichelTask5.Module.BusinessObjects
{
    [NavigationItem("Maintenance")]
    public class PlanGroup : BaseObject
    {
        public PlanGroup(Session session) : base(session)
        {
        }

        private string name;
        [Size(100)]
        public string Name
        {
            get { return name; }
            set { SetPropertyValue(nameof(Name), ref name, value); }
        }

        private Equipment equipment;
        public Equipment Equipment
        {
            get { return equipment; }
            set { SetPropertyValue(nameof(Equipment), ref equipment, value); }
        }
    }
}
