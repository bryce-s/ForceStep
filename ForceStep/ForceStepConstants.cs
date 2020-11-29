using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForceStepConstants
{
        public static class BreakpointStatusCodes
        {
            public static string SuspendedManually = "[s]";
            public static string SuspendedFromStep = "[step]";
            public static string Active = "";
       }

        public enum SaveBreakpointReason
    {
        ForceStep = 0,
        Manual = 1
    }

}

