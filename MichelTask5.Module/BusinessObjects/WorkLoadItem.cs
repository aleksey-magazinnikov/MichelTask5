using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Xpo;

namespace MichelTask5.Module.BusinessObjects
{
    [DeferredDeletion(true)]
    public class WorkLoadItem : XPLiteObject
    {
        public WorkLoadItem(Session session) : base(session) { }
        
        [Key(AutoGenerate = true), Browsable(false)]
        public int Oid { get; set; }
        
        Guid _planId;
        [Browsable(false)]
        public Guid PlanId
        {
            get { return _planId; }
            set { SetPropertyValue(nameof(PlanId), ref _planId, value); }
        }
        
        string _planNumber;
        public string PlanNumber
        {
            get { return _planNumber; }
            set { SetPropertyValue(nameof(PlanNumber), ref _planNumber, value); }
        }
        
        string _operationNumber;
        public string OperationNumber
        {
            get { return _operationNumber; }
            set { SetPropertyValue(nameof(OperationNumber), ref _operationNumber, value); }
        }
        
        string _equipment;
        public string Equipment
        {
            get { return _equipment; }
            set { SetPropertyValue(nameof(Equipment), ref _equipment, value); }
        }
        
        DateTime _dueDate;
        public DateTime DueDate
        {
            get { return _dueDate; }
            set { SetPropertyValue(nameof(DueDate), ref _dueDate, value); }
        }

        private bool separateWorkOrderPerEquipment;
        public bool SeparateWorkOrderPerEquipment
        {
            get { return separateWorkOrderPerEquipment; }
            set { SetPropertyValue(nameof(SeparateWorkOrderPerEquipment), ref separateWorkOrderPerEquipment, value); }
        }
    }
}
