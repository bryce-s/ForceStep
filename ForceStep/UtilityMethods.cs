﻿using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForceStep
{
    class UtilityMethods
    {

        /// <summary>
        /// root automation model
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public static DTE GetDTE(AsyncPackage package)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (BreakpointEvents.m_Dte != null)
            {
                return BreakpointEvents.m_Dte;
            }

            System.IServiceProvider serviceProvider = package as System.IServiceProvider;
            return (DTE)serviceProvider.GetService(typeof(DTE));
        }


        public static MenuCommandService GetOleMenuCommandService(AsyncPackage package)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            System.IServiceProvider serviceProvider = package as System.IServiceProvider;
            return (MenuCommandService) serviceProvider.GetService(typeof(OleMenuCommandService));
        }


        /// <summary>
        /// more involved window functionality than provided by the DTE
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public static IVsUIShell GetIVsUIShell(AsyncPackage package)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            System.IServiceProvider serviceProvider = package as System.IServiceProvider;
            return (IVsUIShell)serviceProvider.GetService(typeof(SVsUIShell));
        }


    }
}
