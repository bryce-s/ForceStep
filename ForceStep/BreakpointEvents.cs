using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForceStepConstants;
using Microsoft.VisualStudio.Shell;

namespace ForceStep
{
    class BreakpointEvents
    {

        static EnvDTE.DTE m_Dte;

        /// <summary>
        /// get the singleton instance of the breakpoint events object
        /// </summary>
        public BreakpointEvents BreakpointEventsState
        {
            get
            {
                if (m_Instance == null)
                {
                    throw new Exception("BreakpointEvents cannot be accessed before it's initialized.");
                }
                return m_Instance;
            }
        }

        /// <summary>
        /// workaround to ensure we start with a package
        /// </summary>
        /// <param name="package"></param>
        public static void InitalizeBreakpointEvents(AsyncPackage package) {
            m_Instance = new BreakpointEvents(UtilityMethods.GetDTE(package));
        }

        private static BreakpointEvents m_Instance = null;

        BreakpointEvents(EnvDTE.DTE dte)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            m_Dte = dte;
            var debuggerEvents = dte.Events.DebuggerEvents;
            debuggerEvents.OnEnterBreakMode += OnEnterBreakMode;
        }

        /// <summary>
        /// consumer for Dte.Events.OnEnterBreakMode
        /// </summary>
        /// <param name="reason"></param>
        /// <param name="executionAction"></param>
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

