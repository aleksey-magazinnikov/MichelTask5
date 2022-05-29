using System;
using System.ComponentModel;
using DevExpress.Xpo;

namespace MichelTask5.Module.BusinessObjects
{
    [DeferredDeletion(false)]
    public class WorkGenerationItem : XPLiteObject
    {
        public WorkGenerationItem(Session session) : base(session) { }

        [Key(AutoGenerate = true), Browsable(false)]
        public int Oid { get; set; }

        Guid _planId;
        [Browsable(false)]
        public Guid PlanId
        {
            get { return _planId; }
            set { SetPropertyValue(nameof(PlanId), ref _planId, value); }
        }
        
        Guid _workGenerationId;
        [Browsable(false)]
        public Guid WorkGenerationId
        {
            get { return _workGenerationId; }
            set { SetPropertyValue(nameof(WorkGenerationId), ref _workGenerationId, value); }
        }
        
        Guid _workOrderId;
        [Browsable(false)]
        public Guid WorkOrderId
        {
            get { return _workOrderId; }
            set { SetPropertyValue(nameof(WorkOrderId), ref _workOrderId, value); }
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
    }
}
