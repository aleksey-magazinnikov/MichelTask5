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
        {
        }

        int _counter_ID;

        [RuleUniqueValue("", DefaultContexts.Save,
             CriteriaEvaluationBehavior = CriteriaEvaluationBehavior.BeforeTransaction), NonCloneable,
         ModelDefault("AllowEdit", "False")]
        [Size(3)]
        public int Counter_ID
        {
            get { return _counter_ID; }
            set { SetPropertyValue(nameof(Counter_ID), ref _counter_ID, value); }
        }

        string _counter_Code;

        [Size(3)]
        public string Counter_Code
        {
            get { return _counter_Code; }
            set { SetPropertyValue(nameof(Counter_Code), ref _counter_Code, value); }
        }

        string _counter_Name;

        [Size(100)]
        public string Counter_Name
        {
            get { return _counter_Name; }
            set { SetPropertyValue(nameof(Counter_Name), ref _counter_Name, value); }
        }

        private int _sequence;

        [Browsable(false)]
        public int Sequence
        {
            get { return _sequence; }
            set { SetPropertyValue(nameof(Sequence), ref _sequence, value); }
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
