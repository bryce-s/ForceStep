﻿using EnvDTE;
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

        private void SuspendBreakpoints(string statusCode)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            DTE dte = UtilityMethods.GetDTE(m_package);

            foreach (EnvDTE.Breakpoint bp in dte.Debugger.Breakpoints)
            {
                if (bp.Enabled)
                {
                    bp.Tag = statusCode;
                    bp.Enabled = false;
                }
            }
        }

        private void RestoreBreakpointsFromSuspension()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            DTE dte = UtilityMethods.GetDTE(m_package);

            foreach (EnvDTE.Breakpoint bp in dte.Debugger.Breakpoints)
            {
                if (bp.Tag == BreakpointStatusCodes.SuspendedManually)
                {
                    bp.Tag = BreakpointStatusCodes.Active;
                    bp.Enabled = true;
                }
            }
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
            if (reason == SaveBreakpointReason.Manual)
            {
                SuspendBreakpoints(BreakpointStatusCodes.SuspendedManually);
            }
        }

        public void RestoreSavedBreakpoints()
        {
            RestoreBreakpointsFromSuspension();
        }

    }
}
