using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using Caliburn.Micro;
using Microsoft.Win32;
using SuperbEdit.Base;
using SuperbEdit.TextEditor.Views;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;

namespace SuperbEdit.TextEditor.ViewModels
{
    [ExportTab(Name="TextEditor")]
    public sealed class TextEditorViewModel : Tab
    {
        [Import]
        HighlightersLoader _highlightersLoader;

        static FontSizeConverter fontSizeConverter;

        static TextEditorViewModel()
        {
            fontSizeConverter = new FontSizeConverter();
        }

        private string _fileContent;
        private string _filePath;

        private string _originalFileContent = "";

        [ImportingConstructor]
        public TextEditorViewModel(IConfig config) : base(config)
        {
            DisplayName = "New File";
            _originalFileContent = "";
            FileContent = _originalFileContent;
            FilePath = "";
        }

        protected override void OnActivate()
        {
            base.OnActivate();
        }


        protected override void ReloadConfig(IConfig config)
        {
            ShowLineNumbers = config.RetrieveConfigValue<bool>("text_editor.show_line_numbers", true);
            WordWrap = config.RetrieveConfigValue<bool>("text_editor.wrapping", false);
            FontFamily = new FontFamily(config.RetrieveConfigValue<string>("text_editor.font_family", "Consolas"));
            FontSize = (double)fontSizeConverter.ConvertFrom(config.RetrieveConfigValue<string>("text_editor.font_size", "10pt"));
        }


        private bool _showLineNumbers = true;
        public bool ShowLineNumbers
        {
            get
            {
                return _showLineNumbers;
            }
            set
            {
                if(_showLineNumbers != value)
                {
                    _showLineNumbers = value;
                    NotifyOfPropertyChange(() => ShowLineNumbers);
                }
            }
        }

        private bool _wordWrap = true;
        public bool WordWrap
        {
            get
            {
                return _wordWrap;
            }
            set
            {
                if (_wordWrap != value)
                {
                    _wordWrap = value;
                    NotifyOfPropertyChange(() => WordWrap);
                }
            }
        }

        private FontFamily _fontFamily;
        public FontFamily FontFamily
        {
            get
            {
                return _fontFamily;
            }
            set
            {
                if (_fontFamily != value)
                {
                    _fontFamily = value;
                    NotifyOfPropertyChange(() => FontFamily);
                }
            }
        }

        private double _fontSize;
        public double FontSize
        {
            get
            {
                return _fontSize;
            }
            set
            {
                if (_fontSize != value)
                {
                    _fontSize = value;
                    NotifyOfPropertyChange(() => FontSize);
                }
            }
        }

        public string FilePath
        {
            get { return _filePath; }
            set
            {
                if (_filePath != value)
                {
                    _filePath = value;
                    NotifyOfPropertyChange(() => FilePath);
                }
            }
        }

        public override string FileContent
        {
            get { return _fileContent; }
            set
            {
                if (_fileContent != value)
                {
                    _fileContent = value;
                    HasChanges = _originalFileContent != _fileContent;
                    NotifyOfPropertyChange(() => FileContent);
                }
            }
        }

        public override bool Save()
        {
            if (FilePath != "")
            {
                File.WriteAllText(FilePath, FileContent);
                _originalFileContent = FileContent;
                HasChanges = false;
                DisplayName = Path.GetFileName(FilePath);
                return true;
            }
            return SaveAs();
        }

        public override bool SaveAs()
        {
            var dialog = new SaveFileDialog();

            if (dialog.ShowDialog().Value)
            {
                FilePath = dialog.FileName;
                _originalFileContent = FileContent;
                File.WriteAllText(FilePath, FileContent);
                HasChanges = false;
                DisplayName = Path.GetFileName(FilePath);
                return true;
            }
            return false;
        }

        public override void Undo()
        {
            var view = GetView() as TextEditorView;

            view.ModernTextEditor.Undo();
        }

        public override void Redo()
        {
            var view = GetView() as TextEditorView;
            view.ModernTextEditor.Redo();
        }

        public override void Cut()
        {
            var view = GetView() as TextEditorView;
            view.ModernTextEditor.Cut();
        }

        public override void Copy()
        {
            var view = GetView() as TextEditorView;
            view.ModernTextEditor.Copy();
        }

        public override void Paste()
        {
            var view = GetView() as TextEditorView;
            view.ModernTextEditor.Paste();
        }

        public override void SetFile(string filePath)
        {
            if (filePath == "")
            {
                DisplayName = "New File";

                _originalFileContent = "";
                FileContent = _originalFileContent;
                FilePath = "";
            }
            else
            {
                FilePath = filePath;
                DisplayName = Path.GetFileName(filePath);

                if (!File.Exists(FilePath))
                {
                    string directoryName = Path.GetDirectoryName(FilePath);
                    if (!Directory.Exists(directoryName))
                    {
                        Directory.CreateDirectory(directoryName);
                    }

                    File.Create(FilePath).Dispose();
                }
                string fileContents = File.ReadAllText(FilePath);
                _originalFileContent = fileContents;
                FileContent = _originalFileContent;

                FileInfo fileInfo = new FileInfo(FilePath);
                IsReadOnly = fileInfo.IsReadOnly;
                Highlighter = _highlightersLoader.GetForExtension(Path.GetExtension(filePath)); 
            }
        }

        public override void RegisterCommands()
        {
        }

        public override void CanClose(Action<bool> callback)
        {
            if (HasChanges)
            {
                switch (
                    MessageBox.Show("Save Changes to " + DisplayName + "?", "SuperbEdit", MessageBoxButton.YesNoCancel,
                        MessageBoxImage.Question))
                {
                    case MessageBoxResult.Yes:
                        callback(Save());
                        break;
                    case MessageBoxResult.No:
                        callback(true);
                        break;
                    case MessageBoxResult.Cancel:
                        callback(false);
                        break;
                }
            }
            else
            {
                callback(true);
            }
        }

        private bool _isReadOnly = false;
        public override bool IsReadOnly
        {
            get
            {
                return _isReadOnly;
            }
            set
            {
                _isReadOnly = value;
                NotifyOfPropertyChange(() => IsReadOnly);
            }
        }

        private IHighlightingDefinition _highlighter;
        public IHighlightingDefinition Highlighter
        {
            get
            {
                return _highlighter;
            }
            set
            {
                _highlighter = value;
                NotifyOfPropertyChange(() => Highlighter);
            }
        }

        public override bool FindNext(string textToFind, FindReplacOptions options)
        {
            ICSharpCode.AvalonEdit.TextEditor editor;
            editor = ((TextEditorView) this.GetView()).ModernTextEditor;
            Regex regex = options.GetRegEx(textToFind);
            int start = regex.Options.HasFlag(RegexOptions.RightToLeft) ?
            editor.SelectionStart : editor.SelectionStart + editor.SelectionLength;
            Match match = regex.Match(editor.Text, start);

            if (!match.Success)  // start again from beginning or end
            {
                if (regex.Options.HasFlag(RegexOptions.RightToLeft))
                    match = regex.Match(editor.Text, editor.Text.Length);
                else
                    match = regex.Match(editor.Text, 0);
            }

            if (match.Success)
            {
                editor.Select(match.Index, match.Length);
                TextLocation loc = editor.Document.GetLocation(match.Index);
                editor.ScrollTo(loc.Line, loc.Column);
            }

            return match.Success;
        }

        public override void Replace(string textToFind, string replacement, FindReplacOptions options)
        {
            ICSharpCode.AvalonEdit.TextEditor editor;
            editor = ((TextEditorView)this.GetView()).ModernTextEditor;
            Regex regex = options.GetRegEx(textToFind);
            string input = editor.Text.Substring(editor.SelectionStart, editor.SelectionLength);
            Match match = regex.Match(input);
            bool replaced = false;
            if (match.Success && match.Index == 0 && match.Length == input.Length)
            {
                editor.Document.Replace(editor.SelectionStart, editor.SelectionLength, replacement);
                replaced = true;
            }
          
        }

        public override void ReplaceAll(string textToFind, string replacement, FindReplacOptions options)
        {
            ICSharpCode.AvalonEdit.TextEditor editor;
            editor = ((TextEditorView)this.GetView()).ModernTextEditor;
            Regex regex = options.GetRegEx(textToFind, true);
            int offset = 0;
            editor.BeginChange();
            foreach (Match match in regex.Matches(editor.Text))
            {
                editor.Document.Replace(offset + match.Index, match.Length, replacement);
                offset += replacement.Length - match.Length;
            }
            editor.EndChange();
        }
    }
}