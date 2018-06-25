using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace DbConnection
{
    public static class CommonHelper
    {
        public static string Nvl(object value, string defaulvalue)
        {
            string retval = defaulvalue;
            if (value == null) return retval;
            if (value == DBNull.Value) return retval;
            if (Convert.ToString(value) == "") return retval;
            return value.ToString().Trim(); //.Trim(); eklendi. 30.09.2015 ersinkecis
        }

        public static string Nvl(object value)
        {
            return Nvl(value, "");
        }

        public static decimal ToDecimal(object value, int ondalik)
        {
            decimal sayi = Math.Round(System.Convert.ToDecimal(Nvl(value, "0")), ondalik, MidpointRounding.AwayFromZero);
            return sayi;
        }

        public static decimal ToDecimal(object value)
        {
            decimal sayi = ToDecimal(value, 0);
            return sayi;
        }

        public static Int64 ToInt64(object value)
        {
            Int64 sayi = System.Convert.ToInt64(Nvl(value, "0"));
            return sayi;
        }

        public static double ToDouble(object value)
        {
            double sayi = System.Convert.ToDouble(Nvl(value, "0"));
            return sayi;
        }

        public static long ToLong(object value)
        {
            Int64 sayi = System.Convert.ToInt64(Nvl(value, "0"));
            return sayi;
        }

        public static Int32 ToInt32(object value)
        {
            Int32 sayi = 0;
            sayi = Int32.Parse(ToDecimal(value, 0).ToString());
            return sayi;
        }

        public static Int16 ToInt16(object value)
        {
            Int16 sayi = 0;
            if (value is bool)
                sayi = System.Convert.ToInt16(value);
            else
                sayi = System.Convert.ToInt16(Nvl(value, "0"));
            return sayi;
        }

        public static DateTime ToDateTime(object value)
        {
            DateTime tarih = System.Convert.ToDateTime(Nvl(value, "01.01.1900"));
            return tarih;
        }

        public static byte[] ToByteArray(object value)
        {
            byte[] byteArray = null;
            if (value != null && value != DBNull.Value)
            {
                byteArray = (byte[])(value);
            }
            return byteArray;
        }

        public static bool IsValidTime(string thetime)
        {
            Regex checktime = new Regex(@"([01][0-9]|2[0-3]):[0-5][0-9]");

            return checktime.IsMatch(thetime);
        }

        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static T ParseEnum<T>(Int32 value)
        {
            return (T)(object)value;
        }

        public static DataTable ListToDataTable<T>(IEnumerable<T> varlist)
        {
            DataTable dtReturn = new DataTable();


            // column names
            PropertyInfo[] oProps = null;


            if (varlist == null) return dtReturn;


            foreach (T rec in varlist)
            {
                // Use reflection to get property names, to create table, Only first time, others will follow
                if (oProps == null)
                {
                    oProps = ((Type)rec.GetType()).GetProperties();
                    foreach (PropertyInfo pi in oProps)
                    {
                        Type colType = pi.PropertyType;


                        if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                        {
                            colType = colType.GetGenericArguments()[0];
                        }


                        dtReturn.Columns.Add(new DataColumn(pi.Name, colType));
                    }
                }


                DataRow dr = dtReturn.NewRow();


                foreach (PropertyInfo pi in oProps)
                {
                    dr[pi.Name] = pi.GetValue(rec, null) == null ? DBNull.Value : pi.GetValue
                    (rec, null);
                }


                dtReturn.Rows.Add(dr);
            }
            return dtReturn;
        }

        public static bool AreStringsEqual(string a, string b)
        {
            if (string.IsNullOrEmpty(a))
            {
                return string.IsNullOrEmpty(b);
            }
            else
            {
                return string.Equals(a, b);
            }
        }

        public static string ConvertDataTableToString(DataTable dataTable)
        {
            var output = new StringBuilder();

            var columnsWidths = new int[dataTable.Columns.Count];

            // Get column widths
            foreach (DataRow row in dataTable.Rows)
            {
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    var length = row[i].ToString().Length;
                    if (columnsWidths[i] < length)
                        columnsWidths[i] = length;
                }
            }

            // Get Column Titles
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                var length = dataTable.Columns[i].ColumnName.Length;
                if (columnsWidths[i] < length)
                    columnsWidths[i] = length;
            }

            // Write Column titles
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                var text = dataTable.Columns[i].ColumnName;
                output.Append("|" + PadCenter(text, columnsWidths[i] + 2));
            }
            output.Append("|\n" + new string('=', output.Length) + "\n");

            // Write Rows
            foreach (DataRow row in dataTable.Rows)
            {
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    var text = row[i].ToString();
                    output.Append("|" + PadCenter(text, columnsWidths[i] + 2));
                }
                output.Append("|\n");
            }
            return output.ToString();
        }

        private static string PadCenter(string text, int maxLength)
        {
            int diff = maxLength - text.Length;
            return new string(' ', diff / 2) + text + new string(' ', (int)(diff / 2.0 + 0.5));
        }

        public static Dictionary<int, string> AllIndexesOf(this string str, string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("the string to find may not be empty", "value");
            Dictionary<int, string> indexes = new Dictionary<int, string>();
            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index);
                if (index == -1)
                    return indexes;
                indexes.Add(index, value);
            }
        }

        public static void AddRange<T, S>(this Dictionary<T, S> source, Dictionary<T, S> collection)
        {
            foreach (var item in collection)
            {
                if (!source.ContainsKey(item.Key))
                {
                    source.Add(item.Key, item.Value);
                }
                else
                {
                    throw new ArgumentNullException("Aynı index değerinde değer mevcut");
                }
            }
        }

        public static bool isAllUpper(string input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (char.IsLetter(input[i]) && !char.IsUpper(input[i]))
                    return false;
            }
            return true;
        }
    }
}
