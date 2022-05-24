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
    public class Work_Unit : BaseObject
    {
        public Work_Unit(Session session) : base(session)
        { }
        int work_Unit_ID;
        [RuleUniqueValue("", DefaultContexts.Save, CriteriaEvaluationBehavior = CriteriaEvaluationBehavior.BeforeTransaction), NonCloneable, ModelDefault("AllowEdit", "False")]
        [Size(3)]
        public int Work_Unit_ID
        {
            get { return work_Unit_ID; }
            set { SetPropertyValue(nameof(Work_Unit_ID), ref work_Unit_ID, value); }
        }
        string work_Unit_Code;
        [Size(3)]
        public string Work_Unit_Code
        {
            get { return work_Unit_Code; }
            set { SetPropertyValue(nameof(Work_Unit_Code), ref work_Unit_Code, value); }
        }
        string work_Unit_Name;
        [Size(100)]
        public string Work_Unit_Name
        {
            get { return work_Unit_Name; }
            set { SetPropertyValue(nameof(Work_Unit_Name), ref work_Unit_Name, value); }
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
                Work_Unit_ID = Sequence;
            }
        }
        private WorkDepartment workDepartment;
        [Association("WorkDepartment_Work_Unit")]
        public WorkDepartment WorkDepartment
        {
            get { return workDepartment; }
            set { SetPropertyValue(nameof(WorkDepartment), ref workDepartment, value); }
        }
    }
}


