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
    [NavigationItem("Default")]
    public class Counter : BaseObject
    {
        public Counter(Session session) : base(session)
        { }
        int counter_ID;
        [RuleUniqueValue("", DefaultContexts.Save, CriteriaEvaluationBehavior = CriteriaEvaluationBehavior.BeforeTransaction), NonCloneable, ModelDefault("AllowEdit", "False")]
        [Size(3)]
        public int Counter_ID
        {
            get { return counter_ID; }
            set { SetPropertyValue(nameof(Counter_ID), ref counter_ID, value); }
        }
        string counter_Code;
        [Size(3)]
        public string Counter_Code
        {
            get { return counter_Code; }
            set { SetPropertyValue(nameof(Counter_Code), ref counter_Code, value); }
        }
        string counter_Name;
        [Size(100)]
        public string Counter_Name
        {
            get { return counter_Name; }
            set { SetPropertyValue(nameof(Counter_Name), ref counter_Name, value); }
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
                Counter_ID = Sequence;
            }
        }
        private bool active_Counter;

        [ImagesForBoolValues("Check", "del")]
        [CaptionsForBoolValues("TRUE", "FALSE")]
        public bool Active_Counter
        {
            get { return active_Counter; }
            set { SetPropertyValue(nameof(Active_Counter), ref active_Counter, value); }
        }

        [Association("Values_Of_Counter")]
        public XPCollection<Counter_Values> CounterValues
        {
            get { return GetCollection<Counter_Values>(nameof(CounterValues)); }
        }
        [Association("Equip_Counter")]
        public XPCollection<Equipment> EquipCounters
        {
            get { return GetCollection<Equipment>(nameof(EquipCounters)); }
        }
    }
}
