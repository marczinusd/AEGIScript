using AEGIScript.GUI.ViewModel;
using AEGIScript.IO;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace AEGIScript
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private EditorViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();
            viewModel = new EditorViewModel();
            this.DataContext = viewModel;
            this.mainGrid.DataContext = viewModel;
            textEditor.TextArea.TextEntering += textEditor_TextArea_TextEntering;
            textEditor.TextArea.TextEntered += textEditor_TextArea_TextEntered;

            // highlighting based on Ruby .xshd file from
            // AvalonEdit github repo
            textEditor.SyntaxHighlighting = ResourceLoader.LoadHighlightingDefinition("AEGIScript.xshd");
            
            viewModel.OnOpenFile += viewModel_OnOpenFile;
            viewModel.OnSaveFile += viewModel_OnSaveFile;
            viewModel.OnSaveAsFile += viewModel_OnSaveAsFile;
            viewModel.OnFileUpToDate += viewModel_OnFileUpToDate;
            viewModel.OnNewFile += viewModel_OnNewFile;
            viewModel.OnClose += viewModel_OnClose;
            initKeywords();
            FileChangedButton.Background = System.Windows.Media.Brushes.Transparent;
        }

        void viewModel_OnClose(object sender, EventArgs e)
        {
            this.Close();
        }

        void viewModel_OnNewFile(object sender, EventArgs e)
        {
            FileChangedButton.Background = System.Windows.Media.Brushes.Transparent;
        }

        void viewModel_OnFileUpToDate(object sender, EventArgs e)
        {
            FileChangedButton.Background = System.Windows.Media.Brushes.Transparent;
        }

        private void viewModel_OnSaveAsFile(object sender, SaveFileEventArgs e)
        {
            SaveFileDialog saveDial = new SaveFileDialog();
            saveDial.Title = "Save current file as...";
            saveDial.Filter = "AEGIScript source files (*.aes) | *.aes";
            saveDial.ShowDialog();
            if (saveDial.FileName != "")
            {
                SourceIO.SaveToFile(e.Doc.Text, saveDial.FileName);
                FileChangedButton.Background = System.Windows.Media.Brushes.Transparent;
            } // check for file saved and signal VM
            else
            {
                FileChangedButton.Background = System.Windows.Media.Brushes.Red;
            }
        }

        private void viewModel_OnSaveFile(object sender, EventArgs e)
        {
            FileChangedButton.Background = System.Windows.Media.Brushes.Transparent;
        }

        private void viewModel_OnOpenFile(object sender, EventArgs e)
        {
            OpenFileDialog openDial = new OpenFileDialog();
            openDial.Filter = "AEGIScript source files|*.aes";
            openDial.Title = "Open existing AEGIScript source file";
            if (openDial.ShowDialog().Value) 
            {
                viewModel.OnFileToOpenSelected(openDial.FileName);
            }
        }

        /*
         *  AVALONEDIT boilerplate begin
         * */
        private CompletionWindow completionWindow;

        private List<string> Keywords = new List<string>();

        private void initKeywords()
        {
            Keywords.Add("for");
            Keywords.Add("new");
            Keywords.Add("while");
            Keywords.Add("begin");
            Keywords.Add("end");
            Keywords.Add("var");
            Keywords.Add("true");
            Keywords.Add("false");
        }

        private void textEditor_TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == " ")
            {
                completionWindow = new CompletionWindow(textEditor.TextArea);
                IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                foreach (var k in Keywords)
                {
                    data.Add(new CompData(k));
                }
                completionWindow.Show();
                completionWindow.Closed += delegate
                {
                    completionWindow = null;
                };
            }
        }

        private void textEditor_TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && completionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    completionWindow.CompletionList.RequestInsertion(e);
                }
            }
        }

        private void textEditor_DocumentChanged(object sender, EventArgs e)
        {
        }

        private void textEditor_TextChanged(object sender, EventArgs e)
        {
            FileChangedButton.Background = System.Windows.Media.Brushes.Red;
        }
    }

    public class CompData : ICompletionData
    {
        double priority;

        public double Priority
        {
            get
            {
                return priority;
            }

            set
            {
                priority = value;
            }
        }

        public CompData(string text)
        {
            this.Text = text;
        }

        public System.Windows.Media.ImageSource Image
        {
            get { return null; }
        }

        public string Text { get; private set; }

        // Use this property if you want to show a fancy UIElement in the list.
        public object Content
        {
            get { return this.Text; }
        }

        public object Description
        {
            get { return "Description for " + this.Text; }
        }

        public void Complete(TextArea textArea, ISegment completionSegment,
            EventArgs insertionRequestEventArgs)
        {
            textArea.Document.Replace(completionSegment, this.Text);
        }
    }
}
