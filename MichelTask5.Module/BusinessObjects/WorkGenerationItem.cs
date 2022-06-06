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

        WorkGeneration _workGeneration;
        public WorkGeneration WorkGeneration
        {
            get { return _workGeneration; }
            set { SetPropertyValue(nameof(WorkGeneration), ref _workGeneration, value); }
        }

        Work_Order _workOrder;
        public Work_Order WorkOrder
        {
            get { return _workOrder; }
            set { SetPropertyValue(nameof(WorkOrder), ref _workOrder, value); }
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
