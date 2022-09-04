using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MichelTask5.Module.BusinessObjects
{
    public class Enums
    {
        public enum PeriodType
        {
            NotSelected = 0,
            Days = 1,
            Weeks = 2,
            Months = 3
        }

        public enum FrequencyType
        {
            Regular = 0,
            Rolling = 1,
            Sequential = 2
        }

        public enum PlanType
        {
            M = 0,
            C
        }
    }
}
