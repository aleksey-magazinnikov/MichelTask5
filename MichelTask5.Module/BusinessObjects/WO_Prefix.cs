using System;
using System.ComponentModel;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace MichelTask5.Module.BusinessObjects
{
    [NavigationItem("Maintenance")]
    public class WO_Prefix : BaseObject
    {
        public WO_Prefix(Session session) : base(session)
        { }
        int work_Prefix_ID;
        [RuleUniqueValue("", DefaultContexts.Save, CriteriaEvaluationBehavior = CriteriaEvaluationBehavior.BeforeTransaction), NonCloneable, ModelDefault("AllowEdit", "False")]
        [Size(3)]
        public int Work_Prefix_ID
        {
            get { return work_Prefix_ID; }
            set { SetPropertyValue(nameof(Work_Prefix_ID), ref work_Prefix_ID, value); }
        }
        string work_Prefix_Code;
        [Size(3)]
        public string Work_Prefix_Code
        {
            get { return work_Prefix_Code; }
            set { SetPropertyValue(nameof(Work_Prefix_Code), ref work_Prefix_Code, value); }
        }
        string work_Prefix_Name;
        [Size(100)]
        public string Work_Prefix_Name
        {
            get { return work_Prefix_Name; }
            set { SetPropertyValue(nameof(Work_Prefix_Name), ref work_Prefix_Name, value); }
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
        }
        private bool for_WO_Generation;
        public bool For_WO_Generation
        {
            get { return for_WO_Generation; }
            set { SetPropertyValue(nameof(For_WO_Generation), ref for_WO_Generation, value); }
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
                Work_Prefix_ID = Sequence;
            }
        }

    }
}

