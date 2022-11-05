using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace MichelTask5.Module.BusinessObjects
{

    [NavigationItem("Maintenance")]
    [RuleCriteria("RuleForDays", DefaultContexts.Save,
        "Monday = false or Tuesday = false or Wednesday = false or Thursday = false or Friday = false or Saturday = false or Sunday = false", "At least one day must not be ticked", SkipNullOrEmptyValues = true)]
    public class PlanSettings : BaseObject
    {
        public PlanSettings(Session session) : base(session)
        {
        }

        int _PlanSettingID;

        [Size(3)]
        public int PlanSettingID
        {
            get => _PlanSettingID;
            set => SetPropertyValue(nameof(PlanSettingID), ref _PlanSettingID, value);
        }

        string _Name;
        
        [Size(100)]
        public string Name
        {
            get => _Name;
            set => SetPropertyValue(nameof(Name), ref _Name, value);
        }

        private bool _Monday;
        public bool Monday
        {
            get => _Monday;
            set => SetPropertyValue(nameof(Monday), ref _Monday, value);
        }

        private bool _Tuesday;
        public bool Tuesday
        {
            get => _Tuesday;
            set => SetPropertyValue(nameof(Tuesday), ref _Tuesday, value);
        }

        private bool _Wednesday;
        public bool Wednesday
        {
            get => _Wednesday;
            set => SetPropertyValue(nameof(Wednesday), ref _Wednesday, value);
        }

        private bool _Thursday;
        public bool Thursday
        {
            get => _Thursday;
            set => SetPropertyValue(nameof(Thursday), ref _Thursday, value);
        }

        private bool _Friday;
        public bool Friday
        {
            get => _Friday;
            set => SetPropertyValue(nameof(Friday), ref _Friday, value);
        }

        private bool _Saturday;
        public bool Saturday
        {
            get => _Saturday;
            set => SetPropertyValue(nameof(Saturday), ref _Saturday, value);
        }

        private bool _Sunday;
        public bool Sunday
        {
            get => _Sunday;
            set => SetPropertyValue(nameof(Sunday), ref _Sunday, value);
        }

        [Association]
        public XPCollection<PlanSettingsNonWorkingDates> PlanSettingsNonWorkingDates => GetCollection<PlanSettingsNonWorkingDates>(nameof(PlanSettingsNonWorkingDates));
    }
}