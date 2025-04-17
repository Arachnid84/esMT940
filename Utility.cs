using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace esMT940
{
    internal class Utility
    {
        public static CultureInfo CInfo = new CultureInfo("en-US");

        public static DateTime ParseDate(string year, string month, string day, CultureInfo cultureInfo)
        {
            if (string.IsNullOrWhiteSpace(year))
            {
                throw new ArgumentException("year can not be empty", nameof(year));
            }

            if (string.IsNullOrWhiteSpace(month))
            {
                throw new ArgumentException("month can not be empty", nameof(month));
            }

            if (string.IsNullOrWhiteSpace(day))
            {
                throw new ArgumentException("day can not be empty", nameof(day));
            }

            if (cultureInfo == null)
            {
                throw new ArgumentNullException(nameof(cultureInfo));
            }

            var parsedFourDigitYear = cultureInfo.Calendar.ToFourDigitYear(ParseInteger(year, cultureInfo));
            var parsedMonth = ParseInteger(month, cultureInfo);
            var parsedDay = ParseInteger(day, cultureInfo);

            return new DateTime(parsedFourDigitYear, parsedMonth, parsedDay);
        }

        public static int ParseInteger(string value, IFormatProvider cultureInfo)
        {
            if (TryParseInteger(value, cultureInfo, out int result))
                return result;

            throw new InvalidCastException();
        }

        public static bool TryParseInteger(string integer, IFormatProvider cultureInfo, out int result)
        {
            return int.TryParse
                (
                    integer,
                    NumberStyles.Any,
                    cultureInfo,
                    out result
                );
        }
    }
}
