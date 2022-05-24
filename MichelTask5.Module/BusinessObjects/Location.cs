using System;
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
    [DefaultProperty(nameof(LocationLongCode))]
    public class Location : BaseObject, IMapsMarker, ITreeNode, IBaseMapsMarker
    {
        #region Constructor
        public Location(Session session) : base(session) { }
        #endregion

        #region Properties
        private int _LocationID;
        [VisibleInListView(false)]
        public int LocationID
        {
            get { return _LocationID; }
            set { SetPropertyValue<int>(nameof(LocationID), ref _LocationID, value); }
        }

        private string _LocationCode;
        [ImmediatePostData(true)]
        [RuleRequiredField(DefaultContexts.Save)]
        public string LocationCode
        {
            get { return _LocationCode; }
            set
            {
                SetPropertyValue<string>(nameof(LocationCode), ref _LocationCode, value);
                OnChanged(LocationLongCode);
            }
        }

        private string _LocationName;
        [RuleRequiredField(DefaultContexts.Save)]
        public string LocationName
        {
            get { return _LocationName; }
            set { SetPropertyValue<string>(nameof(LocationName), ref _LocationName, value); }
        }

        [NonPersistent]
        public string LocationLongCode
        {
            get { return GenerateLongCode(this); }
        }

        private Location _ParentLocation;
        [ImmediatePostData(true)]
        [Association("Location-Parent-Child")]
        public Location ParentLocation
        {
            get { return _ParentLocation; }
            set
            {
                SetPropertyValue<Location>(nameof(ParentLocation), ref _ParentLocation, value);
                if (!IsLoading && !IsSaving)
                    OnChanged(LocationLongCode);
            }
        }

        [Association("Location-Parent-Child")]
        public XPCollection<Location> Locations
        {
            get { return GetCollection<Location>(nameof(Locations)); }
        }

        [Association("Location-Equipment")]
        public XPCollection<Equipment> Equipments
        {
            get { return GetCollection<Equipment>(nameof(Equipments)); }
        }




        #endregion

        #region IMapsMarker
        [VisibleInListView(false)]
        public string Title { get => LocationName; }

        //public double Latitude { get; set; }
        //public double Longitude { get; set; }

        private double _Latitude;
        public double Latitude
        {
            get { return _Latitude; }
            set { SetPropertyValue<double>(nameof(Latitude), ref _Latitude, value); }
        }

        private double _Longitude;
        public double Longitude
        {
            get { return _Longitude; }
            set { SetPropertyValue<double>(nameof(Longitude), ref _Longitude, value); }
        }
        #endregion

        #region ITreeNode
        [VisibleInListView(false)]
        public string Name { get => LocationLongCode; }
        [VisibleInListView(false)]
        public ITreeNode Parent { get => ParentLocation; }
        [VisibleInListView(false)]
        public IBindingList Children { get => this.Locations; }
        #endregion

        #region Functions
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            LocationID = Convert.ToInt32(Session.Evaluate<Location>(CriteriaOperator.Parse("Max(LocationID)"), null)) + 1;
        }
        private string GenerateLongCode(Location location)
        {
            try
            {
                if (location.ParentLocation != null)
                {
                    return GenerateLongCode(location.ParentLocation) + '-' + location.LocationCode;
                }

                return location.LocationCode;
            }
            catch (Exception exception)
            {
                Tracing.Tracer.LogError(exception);
                return location.LocationCode;
            }
        }
        #endregion

    }
}
