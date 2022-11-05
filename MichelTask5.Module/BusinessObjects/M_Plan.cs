using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace MichelTask5.Module.BusinessObjects
{
    [NavigationItem("Maintenance")]
    public class M_Plan : BaseObject
    {
        public M_Plan(Session session) : base(session)
        {
            
        }
        int m_Plan_ID;
        [RuleUniqueValue("", DefaultContexts.Save, CriteriaEvaluationBehavior = CriteriaEvaluationBehavior.BeforeTransaction), NonCloneable, ModelDefault("AllowEdit", "False")]
        [Size(3)]
        public int M_Plan_ID
        {
            get { return m_Plan_ID; }
            set { SetPropertyValue(nameof(M_Plan_ID), ref m_Plan_ID, value); }
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

                M_Plan_ID = Sequence;
                M_Plan_Num = $"{Sequence:0000}";
                Plan_Status = 1;
                BaseDate = DateTime.Today;
                NextDate = PlanSettings != null ? Utils.GetNextDateBasedOnPlanSettings(DateTime.Today, PlanSettings) : DateTime.Today;
            }
        }

        string m_Plan_Num;
        [Size(4), ModelDefault("AllowEdit", "False")]
        public string M_Plan_Num
        {
            get { return m_Plan_Num; }
            set { SetPropertyValue(nameof(M_Plan_Num), ref m_Plan_Num, value); }
        }
        string plan_Name;
        [Size(255)]
        public string Plan_Name
        {
            get { return plan_Name; }
            set { SetPropertyValue(nameof(Plan_Name), ref plan_Name, value); }
        }
        private bool active_Plan;
        [ImagesForBoolValues("Check", "del")]
        [CaptionsForBoolValues("TRUE", "FALSE")]
        public bool Active_Plan
        {
            get { return active_Plan; }
            set { SetPropertyValue(nameof(Active_Plan), ref active_Plan, value); }
        }

        private bool freeze_Plan;
        public bool Freez_Plan
        {
            get { return freeze_Plan; }
            set { SetPropertyValue(nameof(Freez_Plan), ref freeze_Plan, value); }
        }

        private bool superpose_Plan;
        [ImmediatePostData(true)]
        public bool Superpose_Plan
        {
            get { return superpose_Plan; }
            set
            {
                bool modified = SetPropertyValue(nameof(Superpose_Plan), ref superpose_Plan, value);
                if (!IsLoading && !IsSaving && modified && !value)
                {
                    SuperposedPlan = null;
                    SuperposeThreshold = 0;
                }
            }
        }

        private int superposeThreshold;

        [Size(3)]
        [Appearance("SuperposeThresholdHiden", Visibility = ViewItemVisibility.ShowEmptySpace, Criteria = "Superpose_Plan == false", Context = "DetailView")]
        public int SuperposeThreshold
        {
            get => superposeThreshold;
            set => SetPropertyValue(nameof(SuperposeThreshold), ref superposeThreshold, value);
        }

        //private bool superposedPlanFlag;

        //[Browsable(false)]
        //public bool SuperposedPlanFlag
        //{
        //    get { return superposedPlanFlag; }
        //    set { SetPropertyValue(nameof(SuperposedPlanFlag), ref superposedPlanFlag, value); }
        //}

        private M_Plan superposedPlan;
        [Appearance("SuperposedPlanHiden", Visibility = ViewItemVisibility.ShowEmptySpace, Criteria = "Superpose_Plan == false", Context = "DetailView")]
        [DataSourceProperty(nameof(AvailablePlans), DataSourcePropertyIsNullMode.SelectNothing)]
        public M_Plan SuperposedPlan
        {
            get { return superposedPlan; }
            set
            {
                bool modified = SetPropertyValue(nameof(SuperposedPlan), ref superposedPlan, value);
                if (!IsLoading && !IsSaving && modified && value!= null)
                {
                    //value.superposedPlanFlag = true;
                    //value.Save();
                }
            }
        }

        private XPCollection<M_Plan> fAvailablePlans;
        [Browsable(false)]
        public XPCollection<M_Plan> AvailablePlans
        {
            get
            {
                if (fAvailablePlans == null)
                {
                    fAvailablePlans = new XPCollection<M_Plan>(Session);
                    RefreshAvailablePlans();
                }
                return fAvailablePlans;
            }
        }

        private void RefreshAvailablePlans()
        {
            if (fAvailablePlans == null)
                return;
            if (Equipments.Count > 0)
            {
                var equipmentOids = Equipments.Select(c => c.Oid).ToList();
                fAvailablePlans.Criteria = new GroupOperator(GroupOperatorType.And,
                    new ContainsOperator("Equipments", new InOperator("Oid", equipmentOids)),
                    CriteriaOperator.Parse("Oid != ?", this.Oid));
            }
            else
            {
                fAvailablePlans.Criteria = null;
            }
        }

        private DateTime baseDate;
        public DateTime BaseDate
        {
            get { return baseDate; }
            set
            {
                bool modified = SetPropertyValue(nameof(BaseDate), ref baseDate, value);
                if (!IsLoading && !IsSaving && modified && period != Enums.PeriodType.NotSelected)
                {
                    RecalculateDatesForPeriod(period);
                    OnChanged(nameof(NextDate));
                }
            }
        }

        private DateTime nextDate;
        public DateTime NextDate
        {
            get { return nextDate; }
            set { SetPropertyValue(nameof(NextDate), ref nextDate, value); }
        }

        private DateTime lastDate;
        public DateTime LastDate
        {
            get { return lastDate; }
            set { SetPropertyValue(nameof(LastDate), ref lastDate, value); }
        }

        private int plan_Status;
        [Size(1), ModelDefault("AllowEdit", "False")]
        public int Plan_Status
        {
            get { return plan_Status; }
            set { SetPropertyValue(nameof(Plan_Status), ref plan_Status, value); }
        }

        private Enums.FrequencyType frequencyType;
        public Enums.FrequencyType FrequencyType
        {
            get { return frequencyType; }
            set { SetPropertyValue(nameof(FrequencyType), ref frequencyType, value); }
        }

        private M_Operation plan_Operation;
        [ImmediatePostData]
        public M_Operation Plan_Operation
        {
            get { return plan_Operation; }
            set { SetPropertyValue(nameof(Plan_Operation), ref plan_Operation, value); }
        }
        
        private PlanGroup planGroup;
        [ImmediatePostData]
        public PlanGroup PlanGroup
        {
            get { return planGroup; }
            set
            {
                bool modified = SetPropertyValue(nameof(PlanGroup), ref planGroup, value);
                if (!IsLoading && !IsSaving && modified && value.Equipment != null)
                {
                    if (!Equipments.Contains(value.Equipment))
                    {
                        Equipments.Add(value.Equipment);
                    }
                }
            }
        }

        private WorkSite site;
        public WorkSite Site
        {
            get { return site; }
            set { SetPropertyValue(nameof(Site), ref site, value); }
        }

        private Enums.PeriodType period;
        [ImmediatePostData]
        public Enums.PeriodType Period
        {
            get { return period; }
            set
            {
                bool modified = SetPropertyValue(nameof(Enums.PeriodType), ref period, value);
                if (!IsLoading && !IsSaving && modified)
                {
                    if (value == Enums.PeriodType.NotSelected)
                    {
                        Number = 0;
                        BaseDate = DateTime.Today;
                        NextDate = PlanSettings != null ? Utils.GetNextDateBasedOnPlanSettings(DateTime.Today, PlanSettings) : DateTime.Today;
                    }
                    else
                    {
                        RecalculateDatesForPeriod(value);
                    }

                    OnChanged(nameof(NextDate));
                }
            }
        }

        private int number;
        [Size(3), Custom("Caption", " ")]
        [Appearance("NumberDisabled", Enabled = false, Criteria = "Period == 0", Context = "DetailView")]
        public int Number
        {
            get { return number; }
            set
            {
                bool modified = SetPropertyValue(nameof(Number), ref number, value);
                if (!IsLoading && !IsSaving && modified)
                {
                    RecalculateDatesForPeriod(period);
                    OnChanged(nameof(NextDate));
                }
            }
        }

        private bool separateWorkOrderPerEquipment;
        public bool SeparateWorkOrderPerEquipment
        {
            get { return separateWorkOrderPerEquipment; }
            set { SetPropertyValue(nameof(SeparateWorkOrderPerEquipment), ref separateWorkOrderPerEquipment, value); }
        }

        private PlanSettings _PlanSettings;
        
        [RuleRequiredField(DefaultContexts.Save), ImmediatePostData]
        public PlanSettings PlanSettings
        {
            get => _PlanSettings;
            set => SetPropertyValue(nameof(PlanSettings), ref _PlanSettings, value);
        }

        [Association, Browsable(false)] 
        public IList<PlanEquipmentLink> PlanEquipmentsLinks => GetList<PlanEquipmentLink>(nameof(PlanEquipmentsLinks));

        [ManyToManyAlias(nameof(PlanEquipmentsLinks), nameof(PlanEquipmentLink.LinkEquipment))]
        public IList<Equipment> Equipments => GetList<Equipment>(nameof(Equipments));

        private void RecalculateDatesForPeriod(Enums.PeriodType value)
        {
            switch (value)
            {
                case Enums.PeriodType.Days:
                    NextDate = PlanSettings != null ? Utils.GetNextDateBasedOnPlanSettings(BaseDate.AddDays(number), PlanSettings) : BaseDate.AddDays(number);
                    break;
                case Enums.PeriodType.Weeks:
                    NextDate = PlanSettings != null ? Utils.GetNextDateBasedOnPlanSettings(BaseDate.AddDays(number * 7), PlanSettings) : BaseDate.AddDays(number * 7);
                    break;
                case Enums.PeriodType.Months:
                    NextDate = PlanSettings != null ? Utils.GetNextDateBasedOnPlanSettings(BaseDate.AddMonths(number), PlanSettings) : BaseDate.AddMonths(number);
                    break;
            }
        }
    }
}
