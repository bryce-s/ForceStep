using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace ForceStep
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(ForceStepPackage.PackageGuidString)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionOpening_string, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    public sealed class ForceStepPackage : AsyncPackage
    {
        public const string PackageGuidString = "d8a1c04d-42ae-4bb7-b053-aae99c2720a4";

        #region Package Members

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            await ForceStepCommand.InitializeAsync(this);
            await ForceContinue.InitializeAsync(this);
            await ForceStepOut.InitializeAsync(this);
            await ForceStepInto.InitializeAsync(this);
            await ManuallyDisable.InitializeAsync(this);
            await ManuallyEnable.InitializeAsync(this);

            BreakpointEvents.InitalizeBreakpointEvents(this);
        }

        #endregion
    }
}
