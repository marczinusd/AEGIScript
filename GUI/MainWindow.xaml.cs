using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;
using AEGIScript.GUI.ViewModel;
using AEGIScript.IO;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using MahApps.Metro.Controls;
using Microsoft.Win32;

namespace AEGIScript.GUI
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private readonly List<string> _keywords = new List<string>();
        private readonly EditorViewModel _viewModel;
        private CompletionWindow _completionWindow;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new EditorViewModel();
            DataContext = _viewModel;
            mainGrid.DataContext = _viewModel;
            textEditor.TextArea.TextEntering += textEditor_TextArea_TextEntering;
            textEditor.TextArea.TextEntered += textEditor_TextArea_TextEntered;

            // highlighting based on Ruby .xshd file from
            // AvalonEdit github repo
            textEditor.SyntaxHighlighting = ResourceLoader.LoadHighlightingDefinition("AEGIScript.xshd");

            _viewModel.OnOpenFile += viewModel_OnOpenFile;
            _viewModel.OnSaveFile += viewModel_OnSaveFile;
            _viewModel.OnSaveAsFile += viewModel_OnSaveAsFile;
            _viewModel.OnFileUpToDate += viewModel_OnFileUpToDate;
            _viewModel.OnNewFile += viewModel_OnNewFile;
            _viewModel.OnClose += viewModel_OnClose;
            _viewModel.OnFinished += _viewModel_OnFinished;
            _viewModel.OnRunning += _viewModel_OnRunning;
            CancelButton.IsEnabled = false;
            InitKeywords();
            FileChangedButton.Background = Brushes.Transparent;
        }

        void _viewModel_OnRunning(object sender, EventArgs e)
        {
            RunButton.IsEnabled = false;
            DebugButton.IsEnabled = false;
            ImmediateButton.IsEnabled = false;
            CancelButton.IsEnabled = true;
        }

        void _viewModel_OnFinished(object sender, EventArgs e)
        {
            RunButton.IsEnabled = true;
            DebugButton.IsEnabled = true;
            ImmediateButton.IsEnabled = true;
            CancelButton.IsEnabled = false;
        }

        private void viewModel_OnClose(object sender, EventArgs e)
        {
            Close();
        }

        private void viewModel_OnNewFile(object sender, EventArgs e)
        {
            FileChangedButton.Background = Brushes.Transparent;
        }

        private void viewModel_OnFileUpToDate(object sender, EventArgs e)
        {
            FileChangedButton.Background = Brushes.Transparent;
        }

        private void viewModel_OnSaveAsFile(object sender, SaveFileEventArgs e)
        {
            var saveDial = new SaveFileDialog
                {
                    Title = "Save current file as...",
                    Filter = "AEGIScript source files (*.aes) | *.aes"
                };
            saveDial.ShowDialog();
            if (saveDial.FileName != "")
            {
                SourceIO.SaveToFile(e.Doc.Text, saveDial.FileName);
                FileChangedButton.Background = Brushes.Transparent;
                _viewModel.UpdateSession(saveDial.FileName);
            } // check for file saved and signal VM
            else
            {
                FileChangedButton.Background = new SolidColorBrush(Color.FromRgb(65, 177, 225));
            }
        }

        private void viewModel_OnSaveFile(object sender, EventArgs e)
        {
            FileChangedButton.Background = Brushes.Transparent;
        }

        private void viewModel_OnOpenFile(object sender, EventArgs e)
        {
            var openDial = new OpenFileDialog
                {
                    Filter = "AEGIScript source files|*.aes",
                    Title = "Open existing AEGIScript source file"
                };
            if (openDial.ShowDialog().Value)
            {
                _viewModel.OnFileToOpenSelected(openDial.FileName);
            }
        }

        /*
         *  AVALONEDIT boilerplate begin
         * */

        private void InitKeywords()
        {
            _keywords.Add("for");
            _keywords.Add("new");
            _keywords.Add("while");
            _keywords.Add("begin");
            _keywords.Add("end");
            _keywords.Add("true");
            _keywords.Add("false");
            _keywords.Add("endif");
            _keywords.Add("print");
            _keywords.Add("len");
            _keywords.Add("append");
        }

        private void textEditor_TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == " ")
            {
                _completionWindow = new CompletionWindow(textEditor.TextArea);
                IList<ICompletionData> data = _completionWindow.CompletionList.CompletionData;
                foreach (string k in _keywords)
                {
                    data.Add(new CompData(k));
                }
                _completionWindow.Show();
                _completionWindow.Closed += delegate { _completionWindow = null; };
            }
        }

        private void textEditor_TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && _completionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    _completionWindow.CompletionList.RequestInsertion(e);
                }
            }
        }

        private void textEditor_DocumentChanged(object sender, EventArgs e)
        {
        }

        private void textEditor_TextChanged(object sender, EventArgs e)
        {
            //FileChangedButton.Background = Brushes.DodgerBlue;
            FileChangedButton.Background = new SolidColorBrush(Color.FromRgb(65, 177, 225));
        }
    }

    public class CompData : ICompletionData
    {
        public CompData(string text)
        {
            Text = text;
        }

        public double Priority { get; set; }

        public ImageSource Image
        {
            get { return null; }
        }

        public string Text { get; private set; }

        // Use this property if you want to show a fancy UIElement in the list.
        public object Content
        {
            get { return Text; }
        }

        public object Description
        {
            get { return "Description for " + Text; }
        }

        public void Complete(TextArea textArea, ISegment completionSegment,
                             EventArgs insertionRequestEventArgs)
        {
            textArea.Document.Replace(completionSegment, Text);
        }
    }
}