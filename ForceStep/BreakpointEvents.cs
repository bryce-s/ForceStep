using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;
using System.Windows.Forms;
using ForceStepConstants;
using System.ComponentModel.Design;

namespace ForceStep
{
    class BreakpointEvents
    {

        public static EnvDTE.DTE m_Dte = null;
        public static DebuggerEvents m_debuggerEvents;
        public static SolutionEvents m_solutionEvents;
        private static AsyncPackage m_package;

        public List<CommandID> m_commandIDs;


        public static BreakpointEvents BreakpointEventsState
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

        public static void InitalizeBreakpointEvents(AsyncPackage package)
        {
            m_Instance = new BreakpointEvents(dte: UtilityMethods.GetDTE(package));
            m_package = package;
        }

        private static BreakpointEvents m_Instance = null;

        BreakpointEvents(EnvDTE.DTE dte)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            m_Dte = dte;

            // seems like this reallocates it. if we modify just the DTE, event listeners
            // won't persist once this is out of scope.
            m_debuggerEvents = dte.Events.DebuggerEvents;
            m_debuggerEvents.OnEnterBreakMode += this.OnEnterBreakMode;
            m_debuggerEvents.OnEnterDesignMode += this.OnEnterDesignMode;

            m_solutionEvents = dte.Events.SolutionEvents;
            m_solutionEvents.BeforeClosing += OnBeforeClosing;
            

            SolutionOpenedImpl();
        }

        public static void StartEvents(DTE dte)
        {
            System.Windows.Forms.MessageBox.Show("Events are attached.");
        }

        private void OnEnterBreakMode(dbgEventReason reason, ref dbgExecutionAction executionAction)
        {
            if (reason == dbgEventReason.dbgEventReasonStep)
            {
                foreach (Breakpoint bp in m_Dte.Debugger.Breakpoints)
                {
                    if (BreakpointStatusCodes.SuspendedFromOperation().Contains(bp.Tag))
                    {
                        bp.Enabled = true;
                        bp.Tag = "";
                    }
                }
            }
        }

        private void EnableMenuCommand(MenuCommand menuCommand)
        {
            menuCommand.Enabled = true;
            menuCommand.Visible = true;
        }

        private void OnSolutionOpened()
        {
            SolutionOpenedImpl();
        }

        private void SolutionOpenedImpl()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            BreakpointManager bpm = new BreakpointManager(package: m_package);
            bpm.DisableSuspendedFromOperationBreakpoints();
        }

        private void OnEnterDesignMode(dbgEventReason reason)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            BreakpointManager bpm = new BreakpointManager(package: m_package);
            bpm.DisableSuspendedFromOperationBreakpoints();
        }

        private void OnBeforeClosing()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            BreakpointManager bpm = new BreakpointManager(package: m_package);
            bpm.RestoreSavedBreakpoints(SaveBreakpointReason.Manual);
        }
        

        #region Breakpoints

        private void DisableSuspendedFromOperationBreakpoints()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            foreach (Breakpoint bp in m_Dte.Debugger.Breakpoints)
            {
                if (BreakpointStatusCodes.SuspendedFromOperation().Contains(bp.Tag))
                {
                    bp.Enabled = true;
                    bp.Tag = "";
                }
            }
        }

        #endregion
    }
}
