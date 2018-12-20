using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace PromotionCMS.Library
{
    public class Utils
    {
        public static string GetAlias(string strInputHtml)
        {

            StringBuilder sb = new StringBuilder();
            string sTemp = StandardWord(strInputHtml);
            // sTemp = HttpContext.Current.Server.HtmlDecode(HTML);
            sTemp = sTemp.Replace("&", " ");
            sTemp = Regex.Replace(sTemp, "Đ|&#273;", "D");
            sTemp = Regex.Replace(sTemp, "đ|&#272;", "d");
            sTemp = Regex.Replace(sTemp, "-", " ");
            sTemp = sTemp.Trim();
            // normalize the Unicode
            sTemp = sTemp.Normalize(NormalizationForm.FormD);

            for (int i = 0; i < sTemp.Length; i++)
            {
                char currentChar = sTemp[i];

                if (char.IsWhiteSpace(currentChar) || currentChar == '.')
                {
                    sb.Append('-');
                }
                else if (char.GetUnicodeCategory(currentChar) != UnicodeCategory.NonSpacingMark && !char.IsPunctuation(currentChar) && !char.IsSymbol(currentChar) &&
                         currentChar < 128)
                {
                    sb.Append(currentChar);
                }
            }

            return sb.ToString();

        }
        private static readonly string[] VietNamChar = new string[] 
        { 
            "aAeEoOuUiIdDyY", 
            "áàạảãâấầậẩẫăắằặẳẵ", 
            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ", 
            "éèẹẻẽêếềệểễ", 
            "ÉÈẸẺẼÊẾỀỆỂỄ", 
            "óòọỏõôốồộổỗơớờợởỡ", 
            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ", 
            "úùụủũưứừựửữ", 
            "ÚÙỤỦŨƯỨỪỰỬỮ", 
            "íìịỉĩ", 
            "ÍÌỊỈĨ", 
            "đ", 
            "Đ", 
            "ýỳỵỷỹ", 
            "ÝỲỴỶỸ" 
        };
        public static string LocDau(string str)
        {
            //Thay thế và lọc dấu từng char      
            for (int i = 1; i < VietNamChar.Length; i++)
            {
                for (int j = 0; j < VietNamChar[i].Length; j++)
                    str = str.Replace(VietNamChar[i][j], VietNamChar[0][i - 1]);
            }
            return str;
        }
        private static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

        public static double CalculateDistance(double latitude1, double longitude1, double latitude2, double longitude2)
        {
            const double circumference = 40075.6; // Earth's circumference at the equator in km
            var distance = 0.0;

            //Calculate radians
            var latitude1Rad = DegreesToRadians(latitude1);
            var longitude1Rad = DegreesToRadians(longitude1);
            var latititude2Rad = DegreesToRadians(latitude2);
            var longitude2Rad = DegreesToRadians(longitude2);

            var logitudeDiff = Math.Abs(longitude1Rad - longitude2Rad);

            if (logitudeDiff > Math.PI)
            {
                logitudeDiff = 2.0 * Math.PI - logitudeDiff;
            }

            var angleCalculation =
                Math.Acos(
                    Math.Sin(latititude2Rad) * Math.Sin(latitude1Rad) +
                    Math.Cos(latititude2Rad) * Math.Cos(latitude1Rad) * Math.Cos(logitudeDiff));

            distance = circumference * angleCalculation / (2.0 * Math.PI);

            return distance;
        }

        public static string StandardWord(string strInput)
        {

            var strTemp = strInput.Trim().Replace("-", " ");

            if (strTemp.Length > 0)
            {
                while (strTemp.IndexOf("  ") > 0)
                {
                    strTemp = strTemp.Replace("  ", " ");
                    strTemp = strTemp.Trim();
                }
            }
            return strTemp;

        }
    }
}
