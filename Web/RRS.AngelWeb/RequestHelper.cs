using System;

namespace RRS.AngelWeb
{
    public class RequestHelper
    {
        public static string NullString(object input)
        {
            if (input == DBNull.Value || input == null)
            {
                return string.Empty;
            }
            return input.ToString().Trim();
        }

        public static int NullInt(object input)
        {
            int result;

            try
            {
                result = int.Parse(NullString(input));
            }
            catch (Exception)
            {
                result = 0;
            }

            return result;
        }

        public static bool NullBool(object input)
        {
            bool result;

            try
            {
                result = bool.Parse(NullString(input));
            }
            catch (Exception)
            {
                result = false;
            }

            return result;
        }

        public static decimal NullDecimal(object input)
        {
            decimal result;

            try
            {
                result = decimal.Parse(NullString(input));
            }
            catch (Exception)
            {
                result = 0m;
            }

            return result;
        }

        public static DateTime NullDateTime(object input)
        {
            DateTime result;

            try
            {
                result = DateTime.Parse(NullString(input));
            }
            catch (Exception)
            {
                result = new DateTime(1900, 1, 1);
            }

            return result;
        }

        public static string GetSqlDateFormat(DateTime dateVal)
        {
            var y = dateVal.Year;
            var m = dateVal.Month;
            var d = dateVal.Day;

            return NullString(y) + "-" + NullString(m) + "-" + NullString(d);
        }

        public static string GetSqlDateAndTimeFormat(DateTime dateVal)
        {
            var y = dateVal.Year;
            var m = dateVal.Month;
            var d = dateVal.Day;
            var h = dateVal.Hour;
            var min = dateVal.Minute;
            var s = dateVal.Second;
            var ms = dateVal.Millisecond;

            return NullString(y) + "-" + NullString(m) + "-" + NullString(d) + " " +
                NullString(h) + ":" + NullString(min) + ":" + NullString(s) + "." + NullString(ms);
        }

        public static string CorrectSubscriberId(string id)
        {
            string result = string.Empty;

            result = id.Substring(0, id.IndexOf(','));
            return result;
        }
    }
}