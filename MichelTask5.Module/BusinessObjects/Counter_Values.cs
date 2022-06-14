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
    public class Counter_Values : BaseObject
    {
        public Counter_Values(Session session) : base(session)
        { }
        DateTime date;
        [RuleRequiredField(DefaultContexts.Save)]

        public DateTime Date
        {
            get { return date; }
            set { SetPropertyValue(nameof(date), ref date, value); }
        }
        float counter_Value;
        public float Counter_Value
        {
            get { return counter_Value; }
            set { SetPropertyValue(nameof(Counter_Value), ref counter_Value, value); }
        }
        private Counter counter;
        [Association("Values_Of_Counter")]
        public Counter Counter
        {
            get { return counter; }
            set { SetPropertyValue(nameof(Counter), ref counter, value); }
        }
    }
}
