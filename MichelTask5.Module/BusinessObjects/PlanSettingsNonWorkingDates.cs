using System;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace MichelTask5.Module.BusinessObjects
{
    public class PlanSettingsNonWorkingDates : BaseObject
    {
        public PlanSettingsNonWorkingDates(Session session) : base(session) { }

        DateTime _NonWorkingDate;
        public DateTime NonWorkingDate
        {
            get => _NonWorkingDate;
            set => SetPropertyValue(nameof(NonWorkingDate), ref _NonWorkingDate, value);
        }

        PlanSettings _PlanSettings;
        [Association]
        public PlanSettings PlanSettings
        {
            get => _PlanSettings;
            set => SetPropertyValue(nameof(PlanSettings), ref _PlanSettings, value);
        }
    }
}
