using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace MichelTask5.Module.BusinessObjects
{
    [Persistent("CPlanEquipmentLink"), CreatableItem(false)]
    public class CPlanEquipmentLink : BaseObject
    {
        public CPlanEquipmentLink(Session session) : base(session) { }

        private C_Plan _LinkPlan;

        [Association]
        public C_Plan LinkPlan
        {
            get => _LinkPlan;
            set
            {
                bool modified = SetPropertyValue(nameof(LinkPlan), ref _LinkPlan, value);
                if (!IsLoading && !IsSaving && modified && !IsDeleted)
                {
                    if (value.Plan_Operation != null)
                    {
                        operation = value.Plan_Operation;
                    }
                }
            }

        }

        private Equipment _LinkEquipment;

        [Association]
        public Equipment LinkEquipment
        {
            get => _LinkEquipment;
            set => SetPropertyValue(nameof(LinkEquipment), ref _LinkEquipment, value);
        }

        [Persistent("Operation")] private M_Operation operation;

        [PersistentAlias("operation"), Browsable(false)]
        public M_Operation Operation => operation;
    }
}
