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
    public class WorkDepartment : BaseObject
    {
        public WorkDepartment(Session session) : base(session)
        { }
        int workDepartment_ID;
        [RuleUniqueValue("", DefaultContexts.Save, CriteriaEvaluationBehavior = CriteriaEvaluationBehavior.BeforeTransaction), NonCloneable, ModelDefault("AllowEdit", "False")]
        [Size(3)]
        public int WorkDepartment_ID
        {
            get { return workDepartment_ID; }
            set { SetPropertyValue(nameof(WorkDepartment_ID), ref workDepartment_ID, value); }
        }
        string workDepartment_Code;
        [Size(3)]
        public string WorkDepartment_Code
        {
            get { return workDepartment_Code; }
            set { SetPropertyValue(nameof(WorkDepartment_Code), ref workDepartment_Code, value); }
        }
        string workDepartment_Name;
        [Size(100)]
        public string WorkDepartment_Name
        {
            get { return workDepartment_Name; }
            set { SetPropertyValue(nameof(WorkDepartment_Name), ref workDepartment_Name, value); }
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
                WorkDepartment_ID = Sequence;
            }
        }
        [Association("WorkDepartment_Work_Unit")]
        public XPCollection<Work_Unit> WorkUnits
        {
            get { return GetCollection<Work_Unit>(nameof(WorkUnits)); }
        }
    }
}


