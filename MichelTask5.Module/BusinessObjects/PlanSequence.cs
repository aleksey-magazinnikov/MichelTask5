using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace MichelTask5.Module.BusinessObjects
{
    [XafDefaultProperty("SequenceId"), NavigationItem("Maintenance")]
    [RuleCriteria("SequenceValidRule", DefaultContexts.Save, "SequenceId > 0",
        "SequenceId should be greater than 0", SkipNullOrEmptyValues = false)]
    public class PlanSequence : BaseObject
    {
        public PlanSequence(Session session) : base(session)
        {
        }

        int _sequenceId;

        [RuleUniqueValue]
        public int SequenceId
        {
            get => _sequenceId;
            set => SetPropertyValue(nameof(SequenceId), ref _sequenceId, value);
        }

        string _Name;

        [Size(100)]
        public string Name
        {
            get => _Name;
            set => SetPropertyValue(nameof(Name), ref _Name, value);
        }
    }
}
