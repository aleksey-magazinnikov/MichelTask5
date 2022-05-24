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
    public class Work_Cause : BaseObject
    {
        public Work_Cause(Session session) : base(session)
        { }
        int work_Cause_ID;
        [RuleUniqueValue("", DefaultContexts.Save, CriteriaEvaluationBehavior = CriteriaEvaluationBehavior.BeforeTransaction), NonCloneable, ModelDefault("AllowEdit", "False")]
        [Size(3)]
        public int Work_Cause_ID
        {
            get { return work_Cause_ID; }
            set { SetPropertyValue(nameof(Work_Cause_ID), ref work_Cause_ID, value); }
        }
        string work_Cause_Code;
        [Size(3)]
        public string Work_Cause_Code
        {
            get { return work_Cause_Code; }
            set { SetPropertyValue(nameof(Work_Cause_Code), ref work_Cause_Code, value); }
        }
        string work_Cause_Name;
        [Size(100)]
        public string Work_Cause_Name
        {
            get { return work_Cause_Name; }
            set { SetPropertyValue(nameof(Work_Cause_Name), ref work_Cause_Name, value); }
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
                Work_Cause_ID = Sequence;
            }
        }
        private Work_Type work_Type;
        [Association("Work_Type_Work_Cause")]
        public Work_Type Work_Type
        {
            get { return work_Type; }
            set { SetPropertyValue(nameof(Work_Type), ref work_Type, value); }
        }
    }
}

