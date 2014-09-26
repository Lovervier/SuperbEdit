﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using System.Reflection;
using SuperbEdit.Base;

namespace SuperbEdit.ViewModels
{
    public sealed class AboutViewModel : Screen
    {
        public string Version
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                return fvi.FileVersion;
            }
        }

        public string License
        {
            get { return File.ReadAllText(Path.Combine(Folders.DocumentationFolder, "LICENSE.md")); }
        }

        public AboutViewModel()
        {
            DisplayName = "About SuperbEdit";
        }
    }
}
