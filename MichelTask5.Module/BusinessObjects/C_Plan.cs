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
    [Appearance("PeriodValueDisabled", Enabled = false, Criteria = "Usage != 1", Context = "DetailView", TargetItems = "PeriodValue")]
    [Appearance("MaxMinThresholdDisabled", Enabled = false, Criteria = "Usage !=  2", Context = "DetailView", TargetItems = "MaxThreshold,MinThreshold")]
    public class C_Plan : BaseObject
    {
        public C_Plan(Session session) : base(session)
        {

        }

        int c_Plan_ID;

        [RuleUniqueValue("", DefaultContexts.Save,
             CriteriaEvaluationBehavior = CriteriaEvaluationBehavior.BeforeTransaction), NonCloneable,
         ModelDefault("AllowEdit", "False")]
        [Size(3)]
        public int C_Plan_ID
        {
            get { return c_Plan_ID; }
            set { SetPropertyValue(nameof(C_Plan_ID), ref c_Plan_ID, value); }
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

                C_Plan_ID = Sequence;
                C_Plan_Num = $"{Sequence:0000}";
                Plan_Status = 1;

            }
        }

        string c_Plan_Num;

        [Size(4), ModelDefault("AllowEdit", "False")]
        public string C_Plan_Num
        {
            get { return c_Plan_Num; }
            set { SetPropertyValue(nameof(C_Plan_Num), ref c_Plan_Num, value); }
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

        int plan_Status;

        [Size(1), ModelDefault("AllowEdit", "False")]
        public int Plan_Status
        {
            get { return plan_Status; }
            set { SetPropertyValue(nameof(Plan_Status), ref plan_Status, value); }
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

        float baseValue;

        public float BaseValue
        {
            get { return baseValue; }
            set
            {
                bool modified = SetPropertyValue(nameof(BaseValue), ref baseValue, value);
                if (!IsLoading && !IsSaving && modified && usage != UsageType.NotSelected)
                {
                    nextValue = baseValue + periodValue;
                    OnChanged(nameof(NextValue));
                }
            }
        }

        float nextValue;

        public float NextValue
        {
            get { return nextValue; }
            set { SetPropertyValue(nameof(NextValue), ref nextValue, value); }
        }

        DateTime lastDate;

        public DateTime LastDate
        {
            get { return lastDate; }
            set { SetPropertyValue(nameof(LastDate), ref lastDate, value); }
        }

        float periodValue;

        public float PeriodValue
        {
            get { return periodValue; }
            set
            {
                bool modified = SetPropertyValue(nameof(PeriodValue), ref periodValue, value);
                if (!IsLoading && !IsSaving && modified && usage != UsageType.NotSelected)
                {
                    nextValue = baseValue + periodValue;
                    OnChanged(nameof(NextValue));
                }
            }
        }

        float minThreshold;

        public float MinThreshold
        {
            get { return minThreshold; }
            set { SetPropertyValue(nameof(MinThreshold), ref minThreshold, value); }
        }

        float maxThreshold;

        public float MaxThreshold
        {
            get { return maxThreshold; }
            set { SetPropertyValue(nameof(MaxThreshold), ref maxThreshold, value); }
        }
        
        private bool separateWorkOrderPerEquipment;
        public bool SeparateWorkOrderPerEquipment
        {
            get { return separateWorkOrderPerEquipment; }
            set { SetPropertyValue(nameof(SeparateWorkOrderPerEquipment), ref separateWorkOrderPerEquipment, value); }
        }

        private UsageType usage;
        [ImmediatePostData]
        public UsageType Usage
        {
            get { return usage; }
            set
            {
                bool modified = SetPropertyValue(nameof(Usage), ref usage, value);
                if (!IsLoading && !IsSaving && modified)
                {
                    if (value == UsageType.NotSelected)
                    {
                        periodValue = 0;
                        baseValue = 0;
                        nextValue = 0;
                    }
                    else
                    {
                        nextValue = baseValue + periodValue;
                    }

                    OnChanged(nameof(NextValue));
                }
            }
        }

        [Association, Browsable(false)]
        public IList<CPlanEquipmentLink> PlanEquipmentsLinks => GetList<CPlanEquipmentLink>(nameof(PlanEquipmentsLinks));

        [ManyToManyAlias(nameof(PlanEquipmentsLinks), nameof(CPlanEquipmentLink.LinkEquipment))]
        public IList<Equipment> Equipments => GetList<Equipment>(nameof(Equipments));
    }

    public enum UsageType
    {
        NotSelected = 0,
        Periodic = 1,
        Threshold = 2,
    }
}
