using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace ManufacturingManagement_V2.Models
{
    /// <summary>
    /// General Purpose Class For Validating Input Values
    /// </summary>
    public class clsValidateInput
    {
        //
        // TODO: Add constructor logic here
        //
        public clsValidateInput()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        //
        public static bool IsValidAlfaNumeric(string str)
        {
            Regex rx = new Regex("^[a-zA-Z0-9]{1,255}$");
            if (rx.IsMatch(str))
            {
                return true;
            }
            else
                return false;
        }
        //

        public static bool IsValidIFSCode(string str)
        {
            //Regex rx = new Regex("^[A-Z|a-z]{4}[0][a-zA-Z0-9]{6}$");
            Regex rx = new Regex("^[A-Z]{4}0[A-Z0-9]{6}$");
            return rx.IsMatch(str);
        }
        //
        public static bool IsValidDouble(string str)
        {
            if (str.Length == 0) { return false; }
            double d;
            if (Double.TryParse(str, out d) == true) { return true; };
            return false;
        }
        //

    }
}