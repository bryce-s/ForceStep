using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForceStepConstants
{
    public static class BreakpointStatusCodes
    {
        public static string SuspendedManually = "[manual]";
        public static string SuspendedFromStep = "[step]";
        public static string SuspendedFromStepOut = "[sout]";
        public static string SuspendedFromContinue = "[continue]";
        public static string Active = "";
    }

    public enum SaveBreakpointReason
    {
        Manual = 0,
        ForceStep = 1,
        ForceStepOut = 2
    }

}

