using AEGIScript.GUI.Model;
using AEGIScript.IO;
using ICSharpCode.AvalonEdit.Document;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Threading;

namespace AEGIScript.GUI.ViewModel
{
    class EditorViewModel : ViewModelBase
    {
        public TextDocument outputDoc { get; set; }
        public TextDocument TDoc { get; set; }
        public TextDocument immediateDoc { get; set; }

        private String CurrentFilePath { get; set; }
        private Boolean HasOpenFile { get; set; }
        private CancellationToken CToken { get; set; }
        private CancellationTokenSource CTokenS { get; set; }

        public DelegateCommand BuildCommand { get; private set; }
        public DelegateCommand OpenCommand  { get; private set; }
        public DelegateCommand SaveCommand  { get; private set; }
        public DelegateCommand SaveAsCommand { get; private set; }
        public DelegateCommand NewCommand { get; private set; }
        public DelegateCommand RunCommand { get; private set; }
        public DelegateCommand CancelCommand { get; private set; }
        public DelegateCommand ClearCommand { get; private set; }
        public DelegateCommand RunImmediateCommand { get; private set; }
        public DelegateCommand ExitCommand { get; private set; }
        public DelegateCommand PrintAST_DFSCommand { get; private set; }
        public DelegateCommand PrintASTTokensCommand { get; private set; }
        public DelegateCommand PrintASTObjectsCommand { get; private set; }
        public DelegateCommand WalkCommand { get; private set; }

        private Interpreter Aes_Interpreter { get; set; }

        public event EventHandler OnOpenFile;
        public event EventHandler<SaveFileEventArgs> OnSaveFile;
        public event EventHandler<EventArgs> OnNewFile;
        public event EventHandler<SaveFileEventArgs> OnSaveAsFile;
        public event EventHandler<EventArgs> OnFileUpToDate;
        public event EventHandler OnClose;


        public EditorViewModel()
        {
            SetCommands();
            Aes_Interpreter = new Interpreter();
            Aes_Interpreter.Print += Aes_Interpreter_Print;
            TDoc = new TextDocument();
            outputDoc = new TextDocument();
            immediateDoc = new TextDocument();
        }

        void Aes_Interpreter_Print(object sender, PrintEventArgs e)
        {
            outputDoc.Text = outputDoc.Text + e.ToString() + "\n";
            OnPropertyChanged("outputDoc");
        }

        private void SetCommands()
        {
            BuildCommand = new DelegateCommand(q => Build());
            OpenCommand = new DelegateCommand(q => Open());
            SaveCommand = new DelegateCommand(q => Save());
            SaveAsCommand = new DelegateCommand(q => SaveAs());
            WalkCommand = new DelegateCommand(q => Debug());
            RunCommand = new DelegateCommand(q => Run());
            NewCommand = new DelegateCommand(q => New());
            ClearCommand = new DelegateCommand(q => Clear());
            ExitCommand = new DelegateCommand(q => Close());
            CancelCommand = new DelegateCommand(q => Cancel());
            RunImmediateCommand = new DelegateCommand(q => RunImmediate());
            PrintASTTokensCommand = new DelegateCommand(q => PrintASTTokens());
            PrintASTObjectsCommand = new DelegateCommand(q => PrintASTObjects());
            PrintAST_DFSCommand = new DelegateCommand(q => PrintAST_DFS());
        }

        private void Close()
        {
            OnClose(this, new EventArgs());
        }

        private void RunImmediate()
        {
            if (immediateDoc.Text == "clear()" || immediateDoc.Text == "clear" || 
                immediateDoc.Text == "clear();")
            {
                Clear();
                immediateDoc.Text = "";
                OnPropertyChanged("immediateDoc");
                return;
            }

            String Pattern = @"begin [.\n]* end;";
            Regex reg = new Regex(Pattern);
            if (reg.IsMatch(immediateDoc.Text))
            {
                Aes_Interpreter.Walk(immediateDoc.Text, true);
            }
            else
            {
                String NewSource = "begin\n" + immediateDoc.Text + "\nend;";
                Aes_Interpreter.Walk(NewSource, true);
            }
            OnPropertyChanged("outputDoc");
        }

        private void Build()
        {
            RunBuildOnSource();
        }

        private void Clear()
        {
            outputDoc.Text = "";
            OnPropertyChanged("outputDoc");
        }

        private void Debug()
        {
            outputDoc.Text = Aes_Interpreter.Walk(TDoc.Text);
            OnPropertyChanged("outputDoc");
        }

        private void Run()
        {
            Clear();
            Aes_Interpreter.Walk(TDoc.Text);
        }
        private void Run(String source)
        {
            Aes_Interpreter.Walk(source);
        }

        private void New()
        {
            TDoc.Text = "";
            HasOpenFile = false;
            OnNewFile(this, new EventArgs());
            OnPropertyChanged("TDoc");
        }

        private void Open()
        {
            this.OnOpenFile(this, new EventArgs());
        }

        private void Save()
        {
            if (HasOpenFile)
            {
                SourceIO.SaveToFile(TDoc.Text, CurrentFilePath);
            }
            else
            {
                this.OnSaveAsFile(this, new SaveFileEventArgs(TDoc, ""));
            }
            OnFileUpToDate(this, new EventArgs());
        }

        private void SaveAs()
        {
            OnSaveAsFile(this, new SaveFileEventArgs(TDoc, ""));
        }


        /// <summary>
        /// Runs the ANTLR grammar on the source file -- output is an AST at the moment
        /// </summary>
        /// TODO: Move to model
        private void RunBuildOnSource()
        {
            outputDoc.Text = Aes_Interpreter.Interpret(TDoc.Text);
            OnPropertyChanged("outputDoc");
        }

        private void Interpret()
        {
            CTokenS = new CancellationTokenSource();
            CToken = CTokenS.Token;
            outputDoc.Text = "";
            string Source = TDoc.Text;
            Task InterpretTask = Task.Factory.StartNew(() => Run(Source),CToken);
        }

        private void Cancel()
        {
            CTokenS.Cancel();
        }

        /// <summary>
        /// Method to respond to a file opening
        /// </summary>
        /// <param name="path">Path to source file</param>
        public void OnFileToOpenSelected(string path)
        {
            var res = SourceIO.ReadFromFile(path);
            CurrentFilePath = path;
            HasOpenFile = true;
            StringBuilder builder = new StringBuilder();
            foreach (string s in res)
            {
                builder.Append(s + "\n");
            }
            TDoc.Text = builder.ToString();
            OnFileUpToDate(this, new EventArgs());
        }

        public void PrintAST()
        {
            outputDoc.Text = Aes_Interpreter.GetAstAsString(TDoc.Text);
            OnPropertyChanged("outputDoc");
        }

        private void PrintASTTokens()
        {
            outputDoc.Text = Aes_Interpreter.GetASTTokensAsString(TDoc.Text);
            OnPropertyChanged("outputDoc");
        }

        private void PrintAST_DFS()
        {
            outputDoc.Text = Aes_Interpreter.PrintAST_DFS(TDoc.Text);
            OnPropertyChanged("outputDoc");
        }

        private void PrintASTObjects()
        {
            outputDoc.Text = Aes_Interpreter.GetASTObjectsAsString(TDoc.Text);
            OnPropertyChanged("outputDoc");
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
        }

    }
}
