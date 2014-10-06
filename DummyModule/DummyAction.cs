﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperbEdit.Base;

namespace DummyModule
{
    [Export(typeof(IActionItem))]
    [ExportActionMetadata(Menu = "", Order = 0, Owner = "Dummy", RegisterInCommandWindow = true)]
    public class DummyAction : ActionItem
    {
        public DummyAction() : base("Dummy", "Just a developer helper.")
        {
            
        }
  
        public override void Execute()
        {
            
        }
    }
}