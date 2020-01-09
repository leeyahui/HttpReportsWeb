﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HttpReports.Web.Implements
{
    public class Utils
    { 
        public static bool ObjToBool(object expression, bool defValue = false)
        {
            if (expression != null)
            {
                bool result = false;
                bool.TryParse(expression.ToString(), out result);
                return result;
            }
            return defValue;
        }

    }
}
