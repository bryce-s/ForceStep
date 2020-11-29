using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForceStepConstants;

namespace ForceStep
{
    class BreakpointEvents
    {

        EnvDTE.DTE m_Dte;
        
        BreakpointEvents(EnvDTE.DTE dte)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            m_Dte = dte;
            var debuggerEvents = dte.Events.DebuggerEvents;
            debuggerEvents.OnEnterBreakMode += OnEnterBreakMode;
        }

        private void OnEnterBreakMode(dbgEventReason reason, ref dbgExecutionAction executionAction)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            if (reason == dbgEventReason.dbgEventReasonStep)
            {
               foreach (Breakpoint bp in m_Dte.Debugger.Breakpoints)
               {
                    if (bp.Tag == BreakpointStatusCodes.SuspendedFromStep)
                    {
                        bp.Enabled = true;
                        bp.Tag = "";
                    }
               }
            }
        }

        


    }
}

