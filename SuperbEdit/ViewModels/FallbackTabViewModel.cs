﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperbEdit.Base.ViewModels
{
    /// <summary>
    /// Tab used when no tabs are available
    /// </summary>
    [ExportTab(Name = "FallbackTab")]
    public class FallbackTabViewModel : Tab
    {
        public FallbackTabViewModel()
        {
            DisplayName = "Error";
        }


        public override bool Save()
        {
            return true;
        }

        public override bool SaveAs()
        {
            return true;
        }

        public override void Undo()
        {

        }

        public override void Redo()
        {

        }

        public override void Cut()
        {

        }

        public override void Copy()
        {

        }

        public override void Paste()
        {

        }

        public override void SetFile(string filePath)
        {

        }

        public override void RegisterCommands()
        {

        }

        public override string FileContent
        {
            get
            {
                return "";
            }
            set
            {
                //Does nothing
            }
        }
    }
}
