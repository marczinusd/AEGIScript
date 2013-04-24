using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using AEGIScript.GUI.Model;
using AEGIScript.IO;
using ICSharpCode.AvalonEdit.Document;

namespace AEGIScript.GUI.ViewModel
{
    internal class EditorViewModel : ViewModelBase
    {
        public EditorViewModel()
        {
            SetCommands();
            AesInterpreter = new Interpreter();
            AesInterpreter.Print += Aes_Interpreter_Print;
            InputDoc = new TextDocument();
            OutputDoc = new TextDocument();
            ImmediateDoc = new TextDocument();
            _timer = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(400)};
            _timer.Tick += _timer_Tick;
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            if (!TaskRunning)
            {
                return;
            }

            if (OutputDoc.Text == "Working....")
            {
                OutputDoc.Text = "Working";
            }
            else
            {
                OutputDoc.Text = OutputDoc.Text + ".";
            }
            OnPropertyChanged("OutputDoc");
        }

        public TextDocument OutputDoc { get; set; }
        public TextDocument InputDoc { get; set; }
        public TextDocument ImmediateDoc { get; set; }

        private String CurrentFilePath { get; set; }
        private Boolean HasOpenFile { get; set; }
        private CancellationToken CToken { get; set; }
        private CancellationTokenSource CTokenS { get; set; }
        private Boolean TaskRunning { get; set; }
        private DispatcherTimer _timer { get; set; }

        public DelegateCommand BuildCommand { get; private set; }
        public DelegateCommand OpenCommand { get; private set; }
        public DelegateCommand SaveCommand { get; private set; }
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

        private Interpreter AesInterpreter { get; set; }

        public event EventHandler OnOpenFile;
        public event EventHandler<SaveFileEventArgs> OnSaveFile;
        public event EventHandler<EventArgs> OnNewFile;
        public event EventHandler<SaveFileEventArgs> OnSaveAsFile;
        public event EventHandler<EventArgs> OnFileUpToDate;
        public event EventHandler OnClose;
        public event EventHandler OnRunning;
        public event EventHandler OnFinished;


        private void Aes_Interpreter_Print(object sender, PrintEventArgs e)
        {
            OutputDoc.Text = OutputDoc.Text + e + "\n";
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
            if (ImmediateDoc.Text == "clear()" || ImmediateDoc.Text == "clear" ||
                ImmediateDoc.Text == "clear();")
            {
                Clear();
                ImmediateDoc.Text = "";
                OnPropertyChanged("ImmediateDoc");
                return;
            }

            string Pattern = @"begin [.\n]* end;";
            var reg = new Regex(Pattern);
            if (reg.IsMatch(ImmediateDoc.Text))
            {
                AesInterpreter.Walk(ImmediateDoc.Text, true);
            }
            else
            {
                var newSource = "begin\n" + ImmediateDoc.Text + "\nend;";
                AesInterpreter.Walk(newSource, true);
            }
            OnPropertyChanged("OutputDoc");
        }

        private void Build()
        {
            RunBuildOnSource();
        }

        private void Clear()
        {
            OutputDoc.Text = "";
            OnPropertyChanged("OutputDoc");
        }

        private void Debug()
        {
            if (!TaskRunning)
            {
                Clear();
                CTokenS = new CancellationTokenSource();
                CToken = CTokenS.Token;
                AesInterpreter = new Interpreter();
                String source = InputDoc.Text;
                OutputDoc.Text = "Working";
                OnPropertyChanged("OutputDoc");
                TaskRunning = true;
                _timer.Start();
                OnRunning(this, new EventArgs());
                Task.Factory.StartNew(() => Run(source), CToken)
                            .ContinueWith(q => Update(), TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private void Update()
        {
            Clear();
            TaskRunning = false;
            _timer.Stop();
            OnFinished(this, new EventArgs());
            OutputDoc.Text = AesInterpreter.Output.ToString();
            OnPropertyChanged("OutputDoc");
        }

        private void Run()
        {
            if (!TaskRunning)
            {
                Clear();
                CTokenS = new CancellationTokenSource();
                CToken = CTokenS.Token;
                // causes task to be stuck -- why?
                //AesInterpreter = new Interpreter();
                String source = InputDoc.Text;
                OutputDoc.Text = "Working";
                OnPropertyChanged("OutputDoc");
                TaskRunning = true;
                _timer.Start();
                OnRunning(this, new EventArgs());
                var T = Task.Factory.StartNew(() => Run(source), CToken)
                            .ContinueWith(q => Update(), TaskScheduler.FromCurrentSynchronizationContext());
            }
        }


        private void Run(String source)
        {
            AesInterpreter.Walk(source);
        }


        private void New()
        {
            InputDoc.Text = "";
            HasOpenFile = false;
            OnNewFile(this, new EventArgs());
            OnPropertyChanged("TDoc");
        }

        private void Open()
        {
            OnOpenFile(this, new EventArgs());
        }

        private void Save()
        {
            if (HasOpenFile)
            {
                SourceIO.SaveToFile(InputDoc.Text, CurrentFilePath);
            }
            else
            {
                OnSaveAsFile(this, new SaveFileEventArgs(InputDoc, ""));
            }
            OnFileUpToDate(this, new EventArgs());
        }

        private void SaveAs()
        {
            OnSaveAsFile(this, new SaveFileEventArgs(InputDoc, ""));
        }


        /// <summary>
        ///     Runs the ANTLR grammar on the source file -- output is an AST at the moment
        /// </summary>
        /// TODO: Move to model
        private void RunBuildOnSource()
        {
            OutputDoc.Text = AesInterpreter.Interpret(InputDoc.Text);
            OnPropertyChanged("outputDoc");
        }

        /*
        private void Interpret()
        {
            CTokenS = new CancellationTokenSource();
            CToken = CTokenS.Token;
            string Source = InputDoc.Text;
            Task InterpretTask = Task.Factory.StartNew(() => Run(Source), CToken);
        }*/


        private void Cancel()
        {
            if (TaskRunning)
            {
                CTokenS.Cancel();
                TaskRunning = false;
                _timer.Stop();
                OnFinished(this, new EventArgs());
                OutputDoc.Text = "Canceled";
                OnPropertyChanged("OutputDoc");
            }
        }

        /// <summary>
        ///     Method to respond to a file opening
        /// </summary>
        /// <param name="path">Path to source file</param>
        public void OnFileToOpenSelected(string path)
        {
            List<string> res = SourceIO.ReadFromFile(path);
            CurrentFilePath = path;
            HasOpenFile = true;
            var builder = new StringBuilder();
            foreach (string s in res)
            {
                builder.Append(s + "\n");
            }
            InputDoc.Text = builder.ToString();
            OnFileUpToDate(this, new EventArgs());
        }

        public void PrintAST()
        {
            OutputDoc.Text = AesInterpreter.GetAstAsString(InputDoc.Text);
            OnPropertyChanged("outputDoc");
        }

        private void PrintASTTokens()
        {
            OutputDoc.Text = AesInterpreter.GetASTTokensAsString(InputDoc.Text);
            OnPropertyChanged("outputDoc");
        }

        private void PrintAST_DFS()
        {
            OutputDoc.Text = AesInterpreter.PrintAST_DFS(InputDoc.Text);
            OnPropertyChanged("outputDoc");
        }

        private void PrintASTObjects()
        {
            OutputDoc.Text = AesInterpreter.GetASTObjectsAsString(InputDoc.Text);
            OnPropertyChanged("outputDoc");
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
        }
    }
}