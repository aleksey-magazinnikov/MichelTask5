using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
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
                NextDate = DateTime.Today;
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
        DateTime baseDate;
        public DateTime BaseDate
        {
            get { return baseDate; }
            set
            {
                bool modified = SetPropertyValue(nameof(baseDate), ref baseDate, value);
                if (!IsLoading && !IsSaving && modified && period != PeriodType.NotSelected)
                {
                    RecalculateDatesForPeriod(period);
                    OnChanged(nameof(NextDate));
                }
            }
        }
        DateTime nextDate;
        public DateTime NextDate
        {
            get { return nextDate; }
            set { SetPropertyValue(nameof(nextDate), ref nextDate, value); }
        }
        DateTime lastDate;
        public DateTime LastDate
        {
            get { return lastDate; }
            set { SetPropertyValue(nameof(lastDate), ref lastDate, value); }
        }
        int plan_Status;
      
        [Size(1), ModelDefault("AllowEdit", "False")]
        public int Plan_Status
        {
            get { return plan_Status; }
            set { SetPropertyValue(nameof(Plan_Status), ref plan_Status, value); }
        }

        private bool rolling_Plan;
        public bool Rolling_Plan
        {
            get { return rolling_Plan; }
            set { SetPropertyValue(nameof(Rolling_Plan), ref rolling_Plan, value); }
        }
        M_Operation plan_Operation;
        [ImmediatePostData]
        public M_Operation Plan_Operation
        {
            get { return plan_Operation; }
            set { SetPropertyValue(nameof(Plan_Operation), ref plan_Operation, value); }
        }
        WorkSite site;
        public WorkSite Site
        {
            get { return site; }
            set { SetPropertyValue(nameof(Site), ref site, value); }
        }

        PeriodType period;
        [ImmediatePostData]
        public PeriodType Period
        {
            get { return period; }
            set
            {
                bool modified = SetPropertyValue(nameof(PeriodType), ref period, value);
                if (!IsLoading && !IsSaving && modified)
                {
                    if (value == PeriodType.NotSelected)
                    {
                        Number = 0;
                        BaseDate = DateTime.Today;
                        NextDate = DateTime.Today;
                    }
                    else
                    {
                        RecalculateDatesForPeriod(value);
                    }

                    OnChanged(nameof(NextDate));
                }
            }
        }

        int number;
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

        [Association, Browsable(false)] 
        public IList<PlanEquipmentLink> PlanEquipmentsLinks => GetList<PlanEquipmentLink>(nameof(PlanEquipmentsLinks));

        [ManyToManyAlias(nameof(PlanEquipmentsLinks), nameof(PlanEquipmentLink.LinkEquipment))]
        public IList<Equipment> Equipments => GetList<Equipment>(nameof(Equipments));

        private void RecalculateDatesForPeriod(PeriodType value)
        {
            switch (value)
            {
                case PeriodType.Days:
                    NextDate = BaseDate.AddDays(number);
                    break;
                case PeriodType.Weeks:
                    NextDate = BaseDate.AddDays(number * 7);
                    break;
                case PeriodType.Months:
                    NextDate = BaseDate.AddMonths(number);
                    break;
            }
        }
    }

    public enum PeriodType
    {
        NotSelected = 0,
        Days = 1,
        Weeks = 2,
        Months = 3
    }
}
