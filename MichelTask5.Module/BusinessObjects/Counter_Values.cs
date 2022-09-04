using System;
using System.ComponentModel;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace MichelTask5.Module.BusinessObjects
{
    [NavigationItem("Default")]
    public class Counter_Values : BaseObject
    {
        public Counter_Values(Session session) : base(session)
        {
        }

        DateTime _date;

        [RuleRequiredField(DefaultContexts.Save)]

        public DateTime Date
        {
            get { return _date; }
            set { SetPropertyValue(nameof(Date), ref _date, value); }
        }

        float _counter_Value;

        public float Counter_Value
        {
            get { return _counter_Value; }
            set { SetPropertyValue(nameof(Counter_Value), ref _counter_Value, value); }
        }

        private Counter _counter;

        [Association("Values_Of_Counter")]
        [RuleRequiredField(DefaultContexts.Save)]
        public Counter Counter
        {
            get { return _counter; }
            set { SetPropertyValue(nameof(Counter), ref _counter, value); }
        }

        [Browsable(false)]
        [RuleFromBoolProperty("CounterValuesRule", DefaultContexts.Save, "Check Counter ({Counter}): values Date: {Date} or Value: {Counter_Value} are not correct.", UsedProperties = "Counter_Value, Date")]
        public bool CounterRule
        {
            get
            {
                using (var counterValues = new XPCollection<Counter_Values>(
                    PersistentCriteriaEvaluationBehavior.InTransaction, Session,
                    new BinaryOperator("Counter", this.Counter, BinaryOperatorType.Equal)))
                {
                    if (counterValues.Any(counterValue =>
                        counterValue.Date >= this.Date && counterValue.Counter_Value <= this.Counter_Value &&
                        counterValue.Oid != this.Oid ||
                        counterValue.Date <= this.Date && counterValue.Counter_Value >= this.Counter_Value &&
                        counterValue.Oid != this.Oid))
                    {
                        return false;
                    }
                }

                return true;
            }
        }
    }
}
