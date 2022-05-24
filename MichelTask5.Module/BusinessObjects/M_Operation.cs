using System;
using System.ComponentModel;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace MichelTask5.Module.BusinessObjects
{
    [NavigationItem("Maintenance")]
    [XafDefaultProperty(nameof(M_Operation_Num))]
    public class M_Operation : BaseObject
    {
        public M_Operation(Session session) : base(session)
        {
        }

        int m_Operation_ID;

        [RuleUniqueValue("", DefaultContexts.Save,
             CriteriaEvaluationBehavior = CriteriaEvaluationBehavior.BeforeTransaction), NonCloneable,
         ModelDefault("AllowEdit", "False")]
        [Size(3)]
        public int M_Operation_ID
        {
            get { return m_Operation_ID; }
            set { SetPropertyValue(nameof(M_Operation_ID), ref m_Operation_ID, value); }
        }

        private int _Sequence;

        [Browsable(false)]
        public int Sequence
        {
            get { return _Sequence; }
            set { SetPropertyValue("Sequence", ref _Sequence, value); }
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            if (Session.IsNewObject(this))
            {
                UpdateSequence();
            }
        }

        protected override void OnSaving()
        {
            base.OnSaving();
            UpdateSequence();
        }

        private void UpdateSequence()
        {
            if (Session.IsNewObject(this))
            {
                var max = Session.Evaluate(Session.GetClassInfo(GetType()), CriteriaOperator.Parse("Max(Sequence)"),
                    null);
                if (max == null)
                {
                    Sequence = 1;
                }
                else
                {
                    Sequence = Convert.ToInt32(max) + 1;
                }

                M_Operation_ID = Sequence;
                M_Operation_Num = $"{Sequence:0000}";
            }
        }

        string m_Operation_Num;

        [Size(4), ModelDefault("AllowEdit", "False")]
        [VisibleInLookupListView(true)]
        public string M_Operation_Num
        {
            get { return m_Operation_Num; }
            set { SetPropertyValue(nameof(M_Operation_Num), ref m_Operation_Num, value); }
        }

        string operation_Name;

        [Size(255)]
        [VisibleInLookupListView(true)]
        public string Operation_Name
        {
            get { return operation_Name; }
            set { SetPropertyValue(nameof(Operation_Name), ref operation_Name, value); }
        }

        string operation_Description;

        [Size(SizeAttribute.Unlimited)]
        public string Operation_Description
        {
            get { return operation_Description; }
            set { SetPropertyValue(nameof(Operation_Description), ref operation_Description, value); }
        }

        Work_Type workType;

        public Work_Type WorkType
        {
            get { return workType; }
            set { SetPropertyValue(nameof(WorkType), ref workType, value); }
        }

        WO_Prefix prefix;

        public WO_Prefix Prefix
        {
            get { return prefix; }
            set { SetPropertyValue(nameof(Prefix), ref prefix, value); }
        }

        Work_Unit operation_unit;

        public Work_Unit Operation_unit
        {
            get { return operation_unit; }
            set { SetPropertyValue(nameof(Operation_unit), ref operation_unit, value); }
        }

        private decimal operationDuration;

        public decimal OperationDuration
        {
            get { return operationDuration; }
            set { SetPropertyValue(nameof(OperationDuration), ref operationDuration, value); }
        }

        private bool active_Operation;

        [ImagesForBoolValues("Check", "del")]
        [CaptionsForBoolValues("TRUE", "FALSE")]
        public bool Active_Operation
        {
            get { return active_Operation; }
            set { SetPropertyValue(nameof(Active_Operation), ref active_Operation, value); }
        }
    }
}
