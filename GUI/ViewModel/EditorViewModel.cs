using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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
            CancelTokens = new List<CancellationToken>();
            AesInterpreter.ProgressChanged += AesInterpreter_ProgressChanged;
            InputDoc = new TextDocument();
            OutputDoc = new TextDocument();
            ImmediateDoc = new TextDocument();
            Timer = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(750)};
            Timer.Tick += _timer_Tick;
            ProgressPercentage = "IDLE";
        }

        void AesInterpreter_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            CurrentProgress = e.ProgressPercentage;
            ProgressPercentage = CurrentProgress.ToString(CultureInfo.InvariantCulture) + "%";
            OnPropertyChanged("ProgressPercentage");
            var args = e as InterpreterProgressChangedArgs;
            if (args != null)
                OutputDoc.Text = args.CurrentOutput;
            OnPropertyChanged("OutputDoc");
            OnPropertyChanged("CurrentProgress");
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            /*
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
             * */
        }

        public TextDocument OutputDoc { get; set; }
        public TextDocument InputDoc { get; set; }
        public TextDocument ImmediateDoc { get; set; }
        public int CurrentProgress { get; set; }

        private String CurrentFilePath { get; set; }
        private Boolean HasOpenFile { get; set; }
        private CancellationToken CToken { get; set; }
        private List<CancellationToken> CancelTokens { get; set; } 
        private CancellationTokenSource CTokenS { get; set; }
        private DispatcherTimer Timer { get; set; }

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
        public Boolean TaskRunning { get; set; }
        public String ProgressPercentage { get; set; }

        private Interpreter AesInterpreter { get; set; }

        public event EventHandler OnOpenFile;
        public event EventHandler<EventArgs> OnNewFile;
        public event EventHandler<SaveFileEventArgs> OnSaveAsFile;
        public event EventHandler<EventArgs> OnFileUpToDate;
        public event EventHandler OnClose;
        public event EventHandler OnRunning;
        public event EventHandler OnFinished;

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

            const string pattern = @"begin [.\n]* end;";
            var reg = new Regex(pattern);
            if (reg.IsMatch(ImmediateDoc.Text))
            {
                AesInterpreter.Walk(ImmediateDoc.Text, true);
            }
            else
            {
                var newSource = "begin\n" + ImmediateDoc.Text + "\nend;";
                AesInterpreter.Walk(newSource, true);
            }
            OutputDoc.Text = AesInterpreter.Output.ToString();
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


        /// <summary>
        ///     Deprecated function, since massive string building has been eliminated from the interpreter
        ///     because of performance issues.
        /// </summary>
        private void Debug()
        {
            if (!TaskRunning)
            {
                Clear();
                CTokenS = new CancellationTokenSource();
                CToken = CTokenS.Token;
                CancelTokens.Add(CTokenS.Token);
                String source = InputDoc.Text;
                OutputDoc.Text = "Working";
                OnPropertyChanged("OutputDoc");
                TaskRunning = true;
                OnPropertyChanged("TaskRunning");
                Timer.Start();
                OnRunning(this, new EventArgs());
                // wat
                var TaskId = 12345;
                var op = AsyncOperationManager.CreateOperation(TaskId);
                Task.Factory.StartNew(() => RunParallel(source, CToken, op), CToken)
                            .ContinueWith(q => Update(), TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private void Update()
        {
            Clear();
            TaskRunning = false;
            OnPropertyChanged("TaskRunning");
            ProgressPercentage = "IDLE";
            OnPropertyChanged("ProgressPercentage");
            Timer.Stop();
            OnFinished(this, new EventArgs());
            OutputDoc.Text = AesInterpreter.Output.ToString();
            OnPropertyChanged("OutputDoc");
            CurrentProgress = 0;
            OnPropertyChanged("CurrentProgress");
        }

        private void Run()
        {
            if (!TaskRunning)
            {
                Clear();
                CTokenS = new CancellationTokenSource();
                CToken = CTokenS.Token;
                CancelTokens.Add(CTokenS.Token);
                String source = InputDoc.Text;
                OutputDoc.Text = "Working";
                OnPropertyChanged("OutputDoc");
                TaskRunning = true;
                OnPropertyChanged("TaskRunning");
                Timer.Start();
                OnRunning(this, new EventArgs());
                AsyncOperation op = AsyncOperationManager.CreateOperation(null);
                Task.Factory.StartNew(() => RunParallel(source, CToken, op), CToken)
                            .ContinueWith(q => Update(), TaskScheduler.FromCurrentSynchronizationContext());
                
            }
        }

        private void RunParallel(String source, CancellationToken token, AsyncOperation operation)
        {
            AesInterpreter.WalkParallel(source, token, operation);
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

        public void UpdateSession(String path)
        {
            CurrentFilePath = path;
            HasOpenFile = true;
        }


        /// <summary>
        ///     Runs the ANTLR grammar on the source file -- output is an AST at the moment
        /// </summary>
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
                OnPropertyChanged("TaskRunning");
                ProgressPercentage = "IDLE";
                OnPropertyChanged("ProgressPercentage");
                Timer.Stop();
                OnFinished(this, new EventArgs());
                OutputDoc.Text = "Canceled";
                OnPropertyChanged("OutputDoc");
                CurrentProgress = 0;
                OnPropertyChanged("CurrentProgress");
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
            OnPropertyChanged("OutputDoc");
        }

        private void PrintASTTokens()
        {
            OutputDoc.Text = AesInterpreter.GetASTTokensAsString(InputDoc.Text);
            OnPropertyChanged("OutputDoc");
        }

        private void PrintAST_DFS()
        {
            OutputDoc.Text = AesInterpreter.PrintAST_DFS(InputDoc.Text);
            OnPropertyChanged("OutputDoc");
        }

        private void PrintASTObjects()
        {
            OutputDoc.Text = AesInterpreter.GetASTObjectsAsString(InputDoc.Text);
            OnPropertyChanged("OutputDoc");
        }

// ReSharper disable RedundantOverridenMember
        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
        }
// ReSharper restore RedundantOverridenMember
    }
}