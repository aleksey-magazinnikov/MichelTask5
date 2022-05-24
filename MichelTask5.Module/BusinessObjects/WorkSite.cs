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
    public class WorkSite : BaseObject
    {
        public WorkSite(Session session) : base(session)
        { }
        int workSite_ID;
        [RuleUniqueValue("", DefaultContexts.Save, CriteriaEvaluationBehavior = CriteriaEvaluationBehavior.BeforeTransaction), NonCloneable, ModelDefault("AllowEdit", "False")]
        [Size(3)]
        public int WorkSite_ID
        {
            get { return workSite_ID; }
            set { SetPropertyValue(nameof(WorkSite_ID), ref workSite_ID, value); }
        }
        string workSite_Code;
        [Size(3)]
        public string WorkSite_Code
        {
            get { return workSite_Code; }
            set { SetPropertyValue(nameof(WorkSite_Code), ref workSite_Code, value); }
        }
        string workSite_Name;
        [Size(100)]
        public string WorkSite_Name
        {
            get { return workSite_Name; }
            set { SetPropertyValue(nameof(WorkSite_Name), ref workSite_Name, value); }
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
                WorkSite_ID = Sequence;
            }
        }
    }
}

