using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MichelTask5.Module.BusinessObjects
{
    public static class Utils
    {
        public static DateTime GetNextDateBasedOnPlanSettings(DateTime dt, PlanSettings planSettings)
        {
            var nonWorkingDates = new List<DateTime>();

            var startDate = dt;
            var endDate = dt.AddDays(7);
            
            while (startDate < endDate)
            {
                var dw = startDate.DayOfWeek;
                if ((dw == DayOfWeek.Monday && planSettings.Monday) ||
                    (dw == DayOfWeek.Tuesday && planSettings.Tuesday) ||
                    (dw == DayOfWeek.Wednesday && planSettings.Wednesday) ||
                    (dw == DayOfWeek.Thursday && planSettings.Thursday) ||
                    (dw == DayOfWeek.Friday && planSettings.Friday) ||
                    (dw == DayOfWeek.Saturday && planSettings.Saturday) ||
                    (dw == DayOfWeek.Sunday && planSettings.Sunday))
                {
                    nonWorkingDates.Add(startDate);
                }

                startDate = startDate.AddDays(1);
            }

            var dateTimes = planSettings.PlanSettingsNonWorkingDates.Select(p => p.NonWorkingDate).ToList();
            nonWorkingDates.AddRange(dateTimes);

            if (nonWorkingDates.Any())
            {
                startDate = dt;
                endDate = dt.AddDays(7);
                while (startDate < endDate)
                {
                    if (!nonWorkingDates.Contains(startDate))
                    {
                        return startDate;
                    }

                    startDate = startDate.AddDays(1);
                }
            }

            return dt;
        }
    }
}
