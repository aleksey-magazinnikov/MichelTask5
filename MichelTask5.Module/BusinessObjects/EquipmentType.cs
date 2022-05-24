using System;
using System.ComponentModel;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace MichelTask5.Module.BusinessObjects
{
    [DefaultClassOptions]
    [DefaultProperty(nameof(EquipmentTypeName))]
    public class EquipmentType : BaseObject
    {
        #region Constructor
        public EquipmentType(Session session) : base(session) { }
        #endregion

        #region Functions
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            EquipmentTypeId = Convert.ToInt32(Session.Evaluate<EquipmentType>(CriteriaOperator.Parse("Max(EquipmentTypeId)"), null)) + 1;
        }
        #endregion

        #region Properties
        private int _EquipmentTypeId;
        [Size(5)]
        [VisibleInListView(false)]
        public int EquipmentTypeId
        {
            get { return _EquipmentTypeId; }
            set { SetPropertyValue<int>(nameof(EquipmentTypeId), ref _EquipmentTypeId, value); }
        }

        private string _EquipmentTypeName;
        [RuleRequiredField(DefaultContexts.Save)]
        public string EquipmentTypeName
        {
            get { return _EquipmentTypeName; }
            set { SetPropertyValue<string>(nameof(EquipmentTypeName), ref _EquipmentTypeName, value); }
        }

        private string _EquipmentTypeCode;
        [Size(10)]
        [RuleRequiredField(DefaultContexts.Save)]
        public string EquipmentTypeCode
        {
            get { return _EquipmentTypeCode; }
            set { SetPropertyValue<string>(nameof(EquipmentTypeCode), ref _EquipmentTypeCode, value); }
        }

        private string _EquipmentTypeDescription;
        public string EquipmentTypeDescription
        {
            get { return _EquipmentTypeDescription; }
            set { SetPropertyValue<string>(nameof(EquipmentTypeDescription), ref _EquipmentTypeDescription, value); }
        }

        private MediaDataObject _ET_Image;
        [DevExpress.Xpo.DisplayName("Image")]
        [ImageEditor(ListViewImageEditorMode = ImageEditorMode.PictureEdit, ListViewImageEditorCustomHeight = 20)]
        public MediaDataObject ET_Image
        {
            get { return _ET_Image; }
            set { SetPropertyValue<MediaDataObject>(nameof(ET_Image), ref _ET_Image, value); }
        }
        #endregion
    }
}
