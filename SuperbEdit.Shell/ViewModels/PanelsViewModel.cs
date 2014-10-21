﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using SuperbEdit.Base;

namespace SuperbEdit.Shell.ViewModels
{
    [Export]
    internal class PanelsViewModel : Conductor<IPanel>.Collection.OneActive
    {
        [ImportingConstructor]
        public PanelsViewModel([ImportMany] IEnumerable<Lazy<IPanel, IPanelMetadata>> lazyPanels)
        {
            
        }
    }
}
