using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForceStepConstants;

namespace ForceStep
{
    class BreakpointManager
    {

        private readonly AsyncPackage m_package;
        public BreakpointManager(AsyncPackage package)
        {
            m_package = package;
        }

        /// <summary>
        /// clears and re-enables all breakpoints set by set or continue operations.
        /// need to run on mode change.
        /// </summary>
        public void DisableSuspendedFromOperationBreakpoints()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var dte = UtilityMethods.GetDTE(m_package);

            foreach (Breakpoint bp in dte.Debugger.Breakpoints)
            {
                if (BreakpointStatusCodes.SuspendedFromOperation().Contains(bp.Tag))
                {
                    bp.Enabled = true;
                    bp.Tag = "";
                }
            }
        }

        private void SuspendBreakpoints(string statusCode)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            DTE dte = UtilityMethods.GetDTE(m_package);

            foreach (EnvDTE.Breakpoint bp in dte.Debugger.Breakpoints)
            {
                if (bp.Enabled && ((EnvDTE80.Breakpoint2)bp).BreakWhenHit)
                {
                    bp.Tag = statusCode;
                    bp.Enabled = false;
                }
            }
        }
        

        private void RestoreBreakpointsFromSuspension(string type)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            DTE dte = UtilityMethods.GetDTE(m_package);

            foreach (EnvDTE.Breakpoint bp in dte.Debugger.Breakpoints)
            {

                if (bp.Tag == type || (type == BreakpointStatusCodes.SuspendedManually && 
                    BreakpointStatusCodes.SuspendedFromOperation().Contains(bp.Tag)))
                {
                    bp.Tag = BreakpointStatusCodes.Active;
                    bp.Enabled = true;
                } 
            }
        }


        public bool HasSuspendedBreakpoints()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            DTE dte = UtilityMethods.GetDTE(m_package);

            foreach (EnvDTE.Breakpoint bp in dte.Debugger.Breakpoints)
            {
                if (bp.Tag != ForceStepConstants.BreakpointStatusCodes.Active)
                {
                    return false;
                }
            }
            return true;
        }


        public void SaveAndSuspendActiveBreakpoints(SaveBreakpointReason reason)
        {
            if (reason == SaveBreakpointReason.ForceStep)
            {
                SuspendBreakpoints(BreakpointStatusCodes.SuspendedFromStep);
            }
            if (reason == SaveBreakpointReason.ForceStepOut)
            {
                SuspendBreakpoints(BreakpointStatusCodes.SuspendedFromStepOut);
            }
            if (reason == SaveBreakpointReason.ForceStepInto)
            {
                SuspendBreakpoints(BreakpointStatusCodes.SuspendedFromStepInto);
            }
            if (reason == SaveBreakpointReason.ForceContinue)
            {
                SuspendBreakpoints(BreakpointStatusCodes.SuspendedFromContinue);
            }
            if (reason == SaveBreakpointReason.Manual)
            {
                SuspendBreakpoints(BreakpointStatusCodes.SuspendedManually);
            }
        }

        public void RestoreSavedBreakpoints(SaveBreakpointReason reason)
        {
            if (reason == SaveBreakpointReason.Manual)
            {
                RestoreBreakpointsFromSuspension(BreakpointStatusCodes.SuspendedManually);
            }
            if (reason == SaveBreakpointReason.ForceStep)
            {
                RestoreBreakpointsFromSuspension(BreakpointStatusCodes.SuspendedFromStep);
            }
            if (reason == SaveBreakpointReason.ForceStepOut)
            {
                RestoreBreakpointsFromSuspension(BreakpointStatusCodes.SuspendedFromStepOut);
            }
            if (reason == SaveBreakpointReason.ForceStepInto)
            {
                RestoreBreakpointsFromSuspension(BreakpointStatusCodes.SuspendedFromStepInto);
            }
            if (reason == SaveBreakpointReason.ForceContinue)
            {
                RestoreBreakpointsFromSuspension(BreakpointStatusCodes.SuspendedFromContinue);
            }
        }

    }
}

