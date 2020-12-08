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

        private static List<string> StepOperations = null;

        public static List<string> SuspendedFromOperation()
        {
            if (StepOperations == null)
            {
                StepOperations = new List<string>();
                StepOperations.Add(SuspendedFromStep);
                StepOperations.Add(SuspendedFromStepOut);
                StepOperations.Add(SuspendedFromContinue);
            }
            return StepOperations;
        } 
    }

    public enum SaveBreakpointReason
    {
        Manual = 0,
        ForceStep = 1,
        ForceStepOut = 2,
        ForceContinue = 3,
    }

}

