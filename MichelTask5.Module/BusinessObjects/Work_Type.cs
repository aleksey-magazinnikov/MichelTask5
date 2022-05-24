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
    public class Work_Type : BaseObject
    {
        public Work_Type(Session session) : base(session)
        { }
        int work_Type_ID;
        [RuleUniqueValue("", DefaultContexts.Save, CriteriaEvaluationBehavior = CriteriaEvaluationBehavior.BeforeTransaction), NonCloneable, ModelDefault("AllowEdit", "False")]
        [Size(3)]
        public int Work_Type_ID
        {
            get { return work_Type_ID; }
            set { SetPropertyValue(nameof(Work_Type_ID), ref work_Type_ID, value); }
        }
        string work_Type_Code;
        [Size(3)]
        public string Work_Type_Code
        {
            get { return work_Type_Code; }
            set { SetPropertyValue(nameof(Work_Type_Code), ref work_Type_Code, value); }
        }
        string work_Type_Name;
        [Size(100)]
        public string Work_Type_Name
        {
            get { return work_Type_Name; }
            set { SetPropertyValue(nameof(Work_Type_Name), ref work_Type_Name, value); }
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
                Work_Type_ID = Sequence;
            }
        }
        [Association("Work_Type_Work_Cause")]
        public XPCollection<Work_Cause> WorkCauses
        {
            get { return GetCollection<Work_Cause>(nameof(WorkCauses)); }
        }
    }
}
