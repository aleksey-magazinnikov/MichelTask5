using System;
using System.ComponentModel;
using DevExpress.Xpo;

namespace MichelTask5.Module.BusinessObjects
{
    [DeferredDeletion(false)]
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

        Guid _operationId;
        [Browsable(false)]
        public Guid OperationId
        {
            get { return _operationId; }
            set { SetPropertyValue(nameof(OperationId), ref _operationId, value); }
        }

        string _operationNumber;
        public string OperationNumber
        {
            get { return _operationNumber; }
            set { SetPropertyValue(nameof(OperationNumber), ref _operationNumber, value); }
        }
        
        Guid _userId;
        [Browsable(false)]
        public Guid UserId
        {
            get { return _userId; }
            set { SetPropertyValue(nameof(UserId), ref _userId, value); }
        }

        string _userName;
        public string UserName
        {
            get { return _userName; }
            set { SetPropertyValue(nameof(UserName), ref _userName, value); }
        }

        Guid? _equipmentId;
        [Browsable(false)]
        public Guid? EquipmentId
        {
            get { return _equipmentId; }
            set { SetPropertyValue(nameof(EquipmentId), ref _equipmentId, value); }
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
        
        WorkLoad _workLoad;
        public WorkLoad WorkLoad
        {
            get { return _workLoad; }
            set { SetPropertyValue(nameof(WorkLoad), ref _workLoad, value); }
        }
        
        WorkGeneration _workGeneration;
        public WorkGeneration WorkGeneration
        {
            get { return _workGeneration; }
            set { SetPropertyValue(nameof(WorkGeneration), ref _workGeneration, value); }
        }

        private bool separateWorkOrderPerEquipment;
        public bool SeparateWorkOrderPerEquipment
        {
            get { return separateWorkOrderPerEquipment; }
            set { SetPropertyValue(nameof(SeparateWorkOrderPerEquipment), ref separateWorkOrderPerEquipment, value); }
        }
        
        private bool sequential;
        public bool Sequential
        {
            get { return sequential; }
            set { SetPropertyValue(nameof(Sequential), ref sequential, value); }
        }

        private int sequence;
        [Browsable(false)]
        public int Sequence
        {
            get { return sequence; }
            set { SetPropertyValue(nameof(Sequence), ref sequence, value); }
        }
    }
}
