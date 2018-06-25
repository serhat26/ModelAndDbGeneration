using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbConnection
{
    public class Security
    {

        #region encode-decode
        #region encode

        public static string Encrypt(string data)
        {
            string lcRet, lcAsc, lcAdd1, lcData1, lcAdd2, lcData2;
            lcAsc = GetAsc(data);
            lcAdd1 = GetRand(3);
            lcData1 = GetSum(lcAsc, lcAdd1, 1);
            lcAdd2 = GetRand(3);
            lcData2 = GetSum(lcAsc, lcAdd2, 1);
            lcRet = lcAdd1 + lcData1 + lcAdd2 + lcData2;
            return lcRet;
        }
        public static string GetRand(int lnLength)
        {
            string lcRet;
            lcRet = "";
            Random rnd = new Random();
            for (int k = 0; k < lnLength; k++)
            {
                lcRet = lcRet + rnd.Next(1, 100).ToString().Substring(0, 1);
            }
            return lcRet;
        }
        public static string GetAsc(string lcStr)
        {
            string lcRet;
            lcRet = "";
            for (int k = 0; k < lcStr.Length; k++)
            {
                lcRet = lcRet + ((int)(lcStr.Substring(k, 1)[0])).ToString().PadLeft(3, '0');
            }
            return lcRet;
        }

        #endregion

        #region decode
        public static string GetSum(string lcStr, string lcAdd, int lnFactor)
        {
            string lcRet;
            int i, c; //, m;
            lcRet = "";
            for (int k = 0; k < lcStr.Length; k++)
            {
                i = (k + 1) % 3;
                i = (i == 0 ? 3 : i);
                c = ((Convert.ToInt32(lcStr.Substring(k, 1)) + lnFactor * Convert.ToInt32(lcAdd.Substring(i - 1, 1))) % 10);
                c = (c < 0 ? 10 + c : c);
                lcRet = lcRet + c.ToString();
            }
            return lcRet;
        }
        public static string GetChr(string lcStr)
        {
            string lcRet;
            int c;
            lcRet = "";
            for (int k = 0; k < lcStr.Length; k = k + 3)
            {
                c = Convert.ToInt32(lcStr.Substring(k, 3));
                c = (c > 255 ? 255 : c);
                c = (c < 0 ? 0 : c);
                lcRet = lcRet + Convert.ToChar(c);
            }
            return lcRet;
        }
        public static string Decrypt(string data)
        {
            string lcRet, lcAdd1, lcData1, lcAdd2, lcData2;
            int lnDataLen;
            string lcResult1, lcResult2;
            lnDataLen = (data.Length - 6) / 2;
            lcAdd1 = data.Substring(0, 3);
            lcData1 = data.Substring(4 - 1, lnDataLen);
            lcAdd2 = data.Substring(3 + lnDataLen + 1 - 1, 3);
            lcData2 = data.Substring(3 + lnDataLen + 3 + 1 - 1, lnDataLen);
            lcResult1 = GetChr(GetSum(lcData1, lcAdd1, -1));
            lcResult2 = GetChr(GetSum(lcData2, lcAdd2, -1));
            if (lcResult1 == lcResult2)
            {
                lcRet = lcResult1;
            }
            else
            {
                //if (!llNoMess) MessageBox.Show("Kriptolu bilgi formatı hatalı.", "decode"); //Console.WriteLine("Kriptolu bilgi formatı hatalı.");
                lcRet = "CRYPT_ERROR";
            }
            return lcRet;
        }

        #endregion
        #endregion

        #region SHA1 Hash
        public static string GetSHA1Hash(string data)
        {
            System.Security.Cryptography.SHA1 sha1 = System.Security.Cryptography.SHA1.Create();
            byte[] hashData = sha1.ComputeHash(Encoding.Default.GetBytes(data));

            StringBuilder returnValue = new StringBuilder();

            for (int i = 0; i < hashData.Length; i++)
            {
                returnValue.Append(hashData[i].ToString("X").PadLeft(2, '0'));
            }

            return returnValue.ToString();
        }

        #endregion SHA1 Hash
    }
}
