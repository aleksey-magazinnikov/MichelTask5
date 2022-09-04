using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace MichelTask5.Module.BusinessObjects
{
    [DefaultClassOptions]
    [DefaultProperty(nameof(EquipmentName))]
    public class Equipment : BaseObject, ITreeNode, IMapsMarker
    {
        #region Constructor
        public Equipment(Session session) : base(session) { }
        #endregion

        #region Properties
        private int _EquipmentId;
        [Size(5)]
        [VisibleInListView(false)]
        public int EquipmentId
        {
            get { return _EquipmentId; }
            set { SetPropertyValue<int>(nameof(EquipmentId), ref _EquipmentId, value); }
        }


        private string _EquipmentName;
        [RuleRequiredField(DefaultContexts.Save)]
        public string EquipmentName
        {
            get { return _EquipmentName; }
            set { SetPropertyValue<string>(nameof(EquipmentName), ref _EquipmentName, value); }
        }

        private string _EquipmentCode;
        [RuleRequiredField(DefaultContexts.Save)]
        [ImmediatePostData(true)]
        [Size(10)]
        public string EquipmentCode
        {
            get { return _EquipmentCode; }
            set
            {
                SetPropertyValue<string>(nameof(EquipmentCode), ref _EquipmentCode, value);
                OnChanged(EquipmentLongCode);
            }
        }

        private EquipmentType _EquipmentType;
        public EquipmentType EquipmentType
        {
            get { return _EquipmentType; }
            set { SetPropertyValue<EquipmentType>(nameof(EquipmentType), ref _EquipmentType, value); }
        }

        [NonPersistent]
        public string EquipmentLongCode
        {
            // get { return EquipmentLocation is null? GenerateLongCode(this) : EquipmentLocation.LocationLongCode + '-' + GenerateLongCode(this); }
            get { return GenerateLongCode(this); }
        }

        private Location _EquipmentLocation;
        [ImmediatePostData(true), Association("Location-Equipment")]
        public Location EquipmentLocation
        {
            get { return _EquipmentLocation; }
            set
            {
                SetPropertyValue<Location>(nameof(EquipmentLocation), ref _EquipmentLocation, value);
                OnChanged(EquipmentLongCode);
            }
        }

        private Equipment _ParentEquipment;
        [Association("Equipment-Parent-Child")]
        [ImmediatePostData(true)]
        [VisibleInListView(false)]
        public Equipment ParentEquipment
        {
            get { return _ParentEquipment; }
            set
            {
                SetPropertyValue<Equipment>(nameof(ParentEquipment), ref _ParentEquipment, value);
                OnChanged(EquipmentLongCode);
            }
        }

        [Association("Equipment-Parent-Child")]
        public XPCollection<Equipment> Equipments
        {
            get { return GetCollection<Equipment>(nameof(Equipments)); }
        }
        #endregion

        #region ITreeNode
        [VisibleInListView(false)]
        public string Name { get => EquipmentName; }
        [VisibleInListView(false)]
        public ITreeNode Parent { get => ParentEquipment; }
        [VisibleInListView(false)]
        public IBindingList Children { get => Equipments; }

        #endregion

        #region IMapsMarker
        [VisibleInListView(false)]
        public string Title { get => EquipmentLongCode; }

        //private double _Latitude;
        [VisibleInListView(false)]
        public double Latitude
        {
            get { return EquipmentLocation is null ? 0 : EquipmentLocation.Latitude; }
            //get { return _Latitude; }
            //set { SetPropertyValue<double>(nameof(Latitude), ref _Latitude, value); }
        }

        // private double _Longitude;
        [VisibleInListView(false)]
        public double Longitude
        {
            get { return EquipmentLocation is null ? 0 : EquipmentLocation.Longitude; }
            //get { return _Longitude; }
            //set { SetPropertyValue<double>(nameof(Longitude), ref _Longitude, value); }
        }

        #endregion

        #region Functions
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            EquipmentId = Convert.ToInt32(Session.Evaluate<Equipment>(CriteriaOperator.Parse("Max(EquipmentId)"), null)) + 1;
        }

        private string GenerateLongCode(Equipment equipment)
        {
            try
            {
                if (equipment.ParentEquipment != null)
                {
                    return GenerateLongCode(equipment.ParentEquipment) + '-' + equipment.EquipmentCode;
                }

                return equipment.EquipmentCode;
            }
            catch (Exception exception)
            {
                Tracing.Tracer.LogError(exception);
                return equipment.EquipmentCode;
            }
        }
        #endregion
        private bool active_Equipment;
        [ImagesForBoolValues("Check", "del")]
        [CaptionsForBoolValues("TRUE", "FALSE")]
        public bool Active_Equipment
        {
            get { return active_Equipment; }
            set { SetPropertyValue(nameof(Active_Equipment), ref active_Equipment, value); }
        }

        private Counter counter;
        [Association("Equip_Counter")]
        public Counter Counter
        {
            get { return counter; }
            set { SetPropertyValue(nameof(Counter), ref counter, value); }
        }

        [Association, Browsable(false)]
        public IList<PlanEquipmentLink> PlanEquipmentLinks => GetList<PlanEquipmentLink>(nameof(PlanEquipmentLinks));

        [ManyToManyAlias(nameof(PlanEquipmentLinks), nameof(PlanEquipmentLink.LinkPlan))]
        public IList<M_Plan> Plans => GetList<M_Plan>(nameof(Plans));

        [Association, Browsable(false)]
        public IList<CPlanEquipmentLink> СPlanEquipmentLinks => GetList<CPlanEquipmentLink>(nameof(СPlanEquipmentLinks));

        [ManyToManyAlias(nameof(СPlanEquipmentLinks), nameof(CPlanEquipmentLink.LinkPlan))]
        public IList<C_Plan> СPlans => GetList<C_Plan>(nameof(СPlans));
    }
}
