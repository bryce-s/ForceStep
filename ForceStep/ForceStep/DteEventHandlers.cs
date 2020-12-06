using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ForceStep
{
    static class DteEventHandlers
    {
        /// <summary>
        /// Consumes SolutionOpened, no unsubscribe
        /// </summary>
        public static void OnSolutionOpened()
        {
            Console.WriteLine("brr");
        }

        /// <summary>
        /// Consumes EnterBreakMode. Do we want to register this event listener every time then
        /// unsub, or just leave it on?
        /// </summary>
        /// <param name="reason"></param>
        /// <param name="executionAction"></param>
        public static void OnEnterBreakMode(dbgEventReason reason, ref dbgExecutionAction executionAction)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            //ForceStepPackage.m_Dte.Events.DebuggerEvents.OnEnterBreakMode -= OnEnterBreakMode;
            MessageBox.Show("hey");

        }

    }
}
