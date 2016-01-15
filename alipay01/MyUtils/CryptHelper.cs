using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace alipay01.MyUtils
{
    public class CryptHelper
    {

        static public string EncodeBase64(string s, bool c)
        {
            if (c)
            {
                return System.Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(s));
            }
            else
            {
                return System.Text.Encoding.Default.GetString(System.Convert.FromBase64String(s));
            }
        }

        static public string doEncode(string strIn)
        {
            strIn = EncodeBase64(strIn, true);
            char[] arrcIn = strIn.ToCharArray(0, strIn.Length);
            if (arrcIn.Length > 3)
            {
                arrcIn[0] = (char)(127 - arrcIn[0]);
                arrcIn[strIn.Length / 2] = (char)(127 - arrcIn[strIn.Length / 2]);
                arrcIn[strIn.Length / 2 + 1] = (char)(127 - arrcIn[strIn.Length / 2 + 1]);
            }
            String strOut1 = "";
            String strOut2 = "";
            String strOut3 = "";


            for (int i = 0; i < arrcIn.Length; i++)
            {
                if (i < strIn.Length / 2)
                {
                    strOut1 = strOut1 + arrcIn[i];
                }
                else if (i > strIn.Length / 2)
                {
                    strOut3 = strOut3 + arrcIn[i];
                }
                else
                {
                    strOut2 = arrcIn[i].ToString();
                }
            }
            return  strOut3 + strOut2 + strOut1;
        }

        static public string doDecode(string strIn)
        {
            char[] arrcIn = strIn.ToCharArray(0, strIn.Length);
            if (arrcIn.Length > 3)
            {
                arrcIn[0] = (char)(127 - arrcIn[0]);
                arrcIn[strIn.Length / 2 - 1] = (char)(127 - arrcIn[strIn.Length / 2 - 1]);
                arrcIn[strIn.Length / 2] = (char)(127 - arrcIn[strIn.Length / 2]);
            }
            String strOut1 = "";
            String strOut2 = "";
            String strOut3 = "";


            for (int i = 0; i < arrcIn.Length; i++)
            {
                if (i < strIn.Length / 2 - 1)
                {
                    strOut1 = strOut1 + arrcIn[i];
                }
                else if (i > strIn.Length / 2 - 1)
                {
                    strOut3 = strOut3 + arrcIn[i];
                }
                else
                {
                    strOut2 = arrcIn[i].ToString();
                }
            }
            return EncodeBase64(strOut3 + strOut2 + strOut1, false); 
        }
    }
}