using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace ForceStep
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(ForceStepPackage.PackageGuidString)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionOpening_string, PackageAutoLoadFlags.BackgroundLoad)]
    //[ProvideAutoLoad(UIContextGuids80.SolutionExists, PackageAutoLoadFlags.BackgroundLoad)]
    //[ProvideAutoLoad(UIContextGuids80.NoSolution, PackageAutoLoadFlags.BackgroundLoad)]
    //[ProvideAutoLoad(VSConstants.UICONTEXT.SolutionHasMultipleProjects_string, PackageAutoLoadFlags.BackgroundLoad)]
    //[ProvideAutoLoad(VSConstants.UICONTEXT.SolutionHasSingleProject_string, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class ForceStepPackage : AsyncPackage
    {
        /// <summary>
        /// ForceStepPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "3d735e2d-9fd5-4f6f-91b3-eb8f318b05e1";

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            await ForceStepOut.InitializeAsync(this);

            //m_Dte = (DTE2)await this.GetServiceAsync(typeof(DTE));
            //m_Dte.Events.SolutionEvents.Opened -= new _dispSolutionEvents_OnOpenedHandler(OnSolutionOpened); ;
            //m_Dte.Events.DebuggerEvents.OnEnterBreakMode += new _dispDebuggerEvents_OnEnterBreakModeEventHandler(OnEnterBreakMode); 

        }

        public static DTE2 m_Dte;


        private static void OnEnterBreakMode(dbgEventReason reason, ref dbgExecutionAction executionAction)
        {
            MessageBox.Show("here!");

        }

        //https://github.com/mayerwin/vs-customize-window-title/blob/master/CustomizeVSWindowTitle/CustomizeVSWindowTitlePackage.cs
        //Every 5 seconds, we check the window titles in case we missed an event.
        //this.ResetTitleTimer = new System.Windows.Forms.Timer { Interval = 5000 };
        //this.ResetTitleTimer.Tick += this.UpdateWindowTitleAsync;
        //this.ResetTitleTimer.Start();

        private static async void OnSolutionOpened()
        {
            Console.WriteLine("test");
            MessageBox.Show("we're here");
        }


        

        #endregion
    }
}
