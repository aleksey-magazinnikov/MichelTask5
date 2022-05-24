using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace MichelTask5.Module.BusinessObjects
{
    [NavigationItem("Maintenance")]
    public class Ope_Equ : BaseObject
    {
        public Ope_Equ(Session session) : base(session) { }

        M_Plan m_Plan;
        public M_Plan M_Plan
        {
            get { return m_Plan; }
            set { SetPropertyValue(nameof(M_Plan), ref m_Plan, value); }
        }
        M_Operation m_Operation;
        public M_Operation M_Operation
        {
            get { return m_Operation; }
            set { SetPropertyValue(nameof(M_Operation), ref m_Operation, value); }
        }
       Equipment equipment;
        public Equipment Equipment
        {
            get { return equipment; }
            set { SetPropertyValue(nameof(Equipment), ref equipment, value); }
        }
    }
}
