using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;
using AEGIScript.GUI.ViewModel;
using AEGIScript.IO;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using Microsoft.Win32;

namespace AEGIScript.GUI.View
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly List<string> _keywords = new List<string>();
        private readonly Dictionary<string, string> _descriptions = new Dictionary<string, string>(); 
        private readonly Dictionary<string, FunctionDescription> _functionDescriptions = new Dictionary<string, FunctionDescription>(); 
        private readonly EditorViewModel _viewModel;
        private DescriptionBoxViewModel _descViewModel;
        private DescriptionBox _descBox;
        private CompletionWindow _completionWindow;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new EditorViewModel();
            DataContext = _viewModel;
            _descViewModel = new DescriptionBoxViewModel(new FunctionDescription("Append", "Array", "Object", "Array", "Append the object to the array", FunctionDescription.FunctionType.Call));
            _descBox = new DescriptionBox();
            _descBox.DataContext = _descViewModel;
            mainGrid.DataContext = _viewModel;
            textEditor.TextArea.TextEntering += textEditor_TextArea_TextEntering;
            textEditor.TextArea.TextEntered += textEditor_TextArea_TextEntered;

            // highlighting based on Ruby .xshd file from
            // AvalonEdit github repo
            textEditor.SyntaxHighlighting = ResourceLoader.LoadHighlightingDefinition("AEGIScript.xshd");

            _viewModel.OnOpenFile += viewModel_OnOpenFile;
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
        #region Init keywords
        private void InitKeywords()
        {

            _keywords.Add("Append");
            _descriptions.Add("Append", "Append(object), Append(Array, object) \t returns: Array \n\t Appends the object at the end of the array.");
            _functionDescriptions["Append"] = new FunctionDescription("Append", "Array", "Object", "Array", "Appends the object at the end of the array.", FunctionDescription.FunctionType.Call);
            _keywords.Add("Array");
            _functionDescriptions["Array"] = new FunctionDescription(funName: "Array",
                                                              calledOn: "void",
                                                              parameters: "void",
                                                              returns: "Array",
                                                              description: "Returns an empty array.",
                                                              type: FunctionDescription.FunctionType.Ctor);

            _keywords.Add("Count");
            _descriptions.Add("Count", "Count(Array|String) \t returns: Integer \n\t Returns the length of an array or a string.");
            _functionDescriptions["Count"] = new FunctionDescription("Count", "Array|String", "void", "Integer", "Returns the length of the array or string.", FunctionDescription.FunctionType.Call);

            _keywords.Add("Read");
            _descriptions.Add("Read", "Read(GeometryStreamReader) \t returns: Array<Geometry> \n\t Reads until end of stream, then returns a list of Geometries.");
            _functionDescriptions["Read"] = new FunctionDescription("Read", "GeometryStream", "void", "Array<Geometry>", "Returns the length of the array or string.", FunctionDescription.FunctionType.Call);

            _keywords.Add("Boundary");
            _descriptions.Add("Boundary", "Boundary(Geometry) \t returns: Geometry \n\t Returns the boundary of the geometry object.");
            _functionDescriptions["Boundary"] = new FunctionDescription("Boundary", "Geometry", "void", "Geometry", "Returns the boundary of the geometry object.", FunctionDescription.FunctionType.Call);

            _keywords.Add("ConvexHull");
            _functionDescriptions["ConvexHull"] = new FunctionDescription(funName: "ConvexHull", 
                                                                          calledOn: "Geometry", 
                                                                          parameters: "void", 
                                                                          returns: "Geometry", 
                                                                          description: "Returns the boundary of the geometry object.", 
                                                                          type: FunctionDescription.FunctionType.Call);

            _keywords.Add("Envelope");
            _functionDescriptions["Envelope"] = new FunctionDescription(funName: "Envelope",
                                                              calledOn: "Geometry",
                                                              parameters: "void",
                                                              returns: "Envelope",
                                                              description: "Returns the envelope of the geometry object.",
                                                              type: FunctionDescription.FunctionType.Call);


            _keywords.Add("Centroid");
            _functionDescriptions["Centroid"] = new FunctionDescription(funName: "Centroid",
                                                              calledOn: "Geometry",
                                                              parameters: "void",
                                                              returns: "Coordinate",
                                                              description: "Returns the centroid of the geometry object.",
                                                              type: FunctionDescription.FunctionType.Call);

            _keywords.Add("Clone");
            _functionDescriptions["Clone"] = new FunctionDescription(funName: "Clone",
                                                  calledOn: "Geometry",
                                                  parameters: "void",
                                                  returns: "Geometry",
                                                  description: "Returns a new geometry object identical to the original.",
                                                  type: FunctionDescription.FunctionType.Call);

            _keywords.Add("Dimension");
            _functionDescriptions["Dimension"] = new FunctionDescription(funName: "Dimension",
                                                  calledOn: "Geometry",
                                                  parameters: "void",
                                                  returns: "Integer",
                                                  description: "Returns the dimension of the geometry.",
                                                  type: FunctionDescription.FunctionType.Call);

            _keywords.Add("DimensionType");
            _functionDescriptions["DimensionType"] = new FunctionDescription(funName: "DimensionType",
                                      calledOn: "Geometry",
                                      parameters: "void",
                                      returns: "DimensionType",
                                      description: "Returns the dimension type of the geometry.",
                                      type: FunctionDescription.FunctionType.Call);

            _keywords.Add("Name");
            _functionDescriptions["Name"] = new FunctionDescription(funName: "DimensionType",
                          calledOn: "Geometry",
                          parameters: "void",
                          returns: "String",
                          description: "Returns the named type of the geometry.",
                          type: FunctionDescription.FunctionType.Call);


            _keywords.Add("ReferenceSystem");
            _functionDescriptions["ReferenceSystem"] = new FunctionDescription
             (funName: "ReferenceSystem",
              calledOn: "Geometry",
              parameters: "void",
              returns: "ReferenceSystem",
              description: "Returns the reference system used by the geometry.",
              type: FunctionDescription.FunctionType.Call);

            _keywords.Add("IsValid");
            _functionDescriptions["IsValid"] = new FunctionDescription
             (funName: "IsValid",
              calledOn: "Geometry",
              parameters: "void",
              returns: "Boolean",
              description: "Returns whether the geometry is valid or not.",
              type: FunctionDescription.FunctionType.Call);
            _keywords.Add("IsSimple");
            _functionDescriptions["IsSimple"] = new FunctionDescription
             (funName: "IsSimple",
              calledOn: "Geometry",
              parameters: "void",
              returns: "Boolean",
              description: "Returns whether the geometry is simple or not.",
              type: FunctionDescription.FunctionType.Call);

            _keywords.Add("IsEmpty");
            _functionDescriptions["IsEmpty"] = new FunctionDescription
             (funName: "IsEmpty",
              calledOn: "Geometry",
              parameters: "void",
              returns: "Boolean",
              description: "Returns whether the geometry is empty or not.",
              type: FunctionDescription.FunctionType.Call);

            _keywords.Add("Center");
            _functionDescriptions["Center"] = new FunctionDescription
             (funName: "Center",
              calledOn: "Envelope",
              parameters: "void",
              returns: "Coordinate",
              description: "Returns the center of the envelope.",
              type: FunctionDescription.FunctionType.Call);
            _keywords.Add("Crosses");
            _functionDescriptions["Crosses"] = new FunctionDescription
             (funName: "Crosses",
              calledOn: "Envelope",
              parameters: "Envelope",
              returns: "Boolean",
              description: "Returns whether the caller crosses the parameter.",
              type: FunctionDescription.FunctionType.Call);
            _keywords.Add("Disjoint");
            _descriptions.Add("Disjoint", "Disjoint(Envelope) \t returns Boolean \n\t Returns if the two objects are disjoint.");
            _functionDescriptions["Disjoint"] = new FunctionDescription
             (funName: "Disjoint",
              calledOn: "Envelope",
              parameters: "Envelope",
              returns: "Boolean",
              description: "Returns whether the caller is disjoint with the parameter.",
              type: FunctionDescription.FunctionType.Call);
            _keywords.Add("Expand");
            _functionDescriptions["Expand"] = new FunctionDescription
             (funName: "Expand",
              calledOn: "Envelope",
              parameters: "Envelope",
              returns: "Envelope",
              description: "Expands an envelope with another envelope.",
              type: FunctionDescription.FunctionType.Call);

            _keywords.Add("Distance");
            _functionDescriptions["Distance"] = new FunctionDescription
             (funName: "Distance",
              calledOn: "Envelope",
              parameters: "Envelope",
              returns: "Double",
              description: "Returns the distance of the parameter and the caller.",
              type: FunctionDescription.FunctionType.Call);

            _keywords.Add("Overlaps");
            _descriptions.Add("Overlaps", "Overlaps(Envelope) \t returns Boolean \n\t Returns whether the two objects overlap.");
            _functionDescriptions["Overlaps"] = new FunctionDescription
             (funName: "Overlaps",
              calledOn: "Envelope",
              parameters: "Envelope",
              returns: "Boolean",
              description: "Returns whether caller overlaps with the parameter.",
              type: FunctionDescription.FunctionType.Call);

            _keywords.Add("Touches");
            _functionDescriptions["Touches"] = new FunctionDescription
                         (funName: "Touches",
                          calledOn: "Envelope",
                          parameters: "Envelope",
                          returns: "Boolean",
                          description: "Returns whether the parameter touches the caller envelope or not.",
                          type: FunctionDescription.FunctionType.Call);

            _keywords.Add("Within");
            _functionDescriptions["Within"] = new FunctionDescription
             (funName: "Within",
              calledOn: "Envelope",
              parameters: "Envelope",
              returns: "Boolean",
              description: "Returns if the parameter is within the caller envelope.",
              type: FunctionDescription.FunctionType.Call);
            
            _keywords.Add("Intersects");
            _descriptions.Add("Intersects", "Intersects(Envelope) \t returns Coordinate \n\t Returns the center of an envelope.");
            _functionDescriptions["Intersects"] = new FunctionDescription
             (funName: "Intersects",
              calledOn: "Envelope",
              parameters: "Envelope",
              returns: "Boolean",
              description: "Returns whether the caller intersects with the parameter or not.",
              type: FunctionDescription.FunctionType.Call);
            _keywords.Add("Maximum");
            _descriptions.Add("Maximum", "Maximum(Envelope) \t returns Coordinate \n\t Returns the center of an envelope.");
            _functionDescriptions["Maximum"] = new FunctionDescription
             (funName: "Maximum",
              calledOn: "Envelope",
              parameters: "void",
              returns: "Double",
              description: "Returns whether the caller intersects with the parameter or not.",
              type: FunctionDescription.FunctionType.Call);
            
            _keywords.Add("Minimum");
            _functionDescriptions["Minimum"] = new FunctionDescription
             (funName: "Minimum",
              calledOn: "Envelope",
              parameters: "void",
              returns: "Double",
              description: "Returns whether the caller intersects with the parameter or not.",
              type: FunctionDescription.FunctionType.Call);

            _keywords.Add("IsPlanar");
            _functionDescriptions["IsPlanar"] = new FunctionDescription
             (funName: "IsPlanar",
              calledOn: "Envelope",
              parameters: "void",
              returns: "Boolean",
              description: "Returns whether the envelope is planar or not.",
              type: FunctionDescription.FunctionType.Call);
            
            _keywords.Add("Coordinate");
            _functionDescriptions["Coordinate"] = new FunctionDescription
             (funName: "Coordinate",
              calledOn: "void",
              parameters: "double,double(,double)",
              returns: "Coordinate",
              description: "Returns a new coordinate",
              type: FunctionDescription.FunctionType.Ctor);
            
            _keywords.Add("Point");
            _functionDescriptions["Point"] = new FunctionDescription
                (funName: "Point",
                 calledOn: "void",
                 parameters: "double,double(,double)",
                 returns: "Point",
                 description: "Returns a new Point",
                 type: FunctionDescription.FunctionType.Ctor);

            _keywords.Add("LinearRing");
            _functionDescriptions["LinearRing"] = new FunctionDescription
             (funName: "LinearRing",
              calledOn: "void",
              parameters: "Array<Point|Coordinate>",
              returns: "LinearRing",
              description: "Returns a new LinearRing",
              type: FunctionDescription.FunctionType.Ctor);

            _keywords.Add("Line");
            _functionDescriptions["Line"] = new FunctionDescription
             (funName: "Line",
              calledOn: "void",
              parameters: "Array<Point|Coordinate>",
              returns: "Coordinate",
              description: "Returns a new Line",
              type: FunctionDescription.FunctionType.Ctor);

            _keywords.Add("LineString");
            _functionDescriptions["LineString"] = new FunctionDescription
             (funName: "LineString",
              calledOn: "void",
              parameters: "Array<Point|Coordinate>",
              returns: "Coordinate",
              description: "Returns a new LineString",
              type: FunctionDescription.FunctionType.Ctor);

            _keywords.Add("Type");
            _descriptions.Add("Type", "Type(Object) \t returns: String \n\t Returns the string representation of the object's known type.");
            _functionDescriptions["Type"] = new FunctionDescription
             (funName: "Type",
              calledOn: "Object",
              parameters: "void",
              returns: "Type",
              description: "Gets the known type of the object.",
              type: FunctionDescription.FunctionType.Call);

            _keywords.Sort();
        }
        #endregion Init function helpers

        private void textEditor_TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == ".")
            {
                _completionWindow = new CompletionWindow(textEditor.TextArea);
                _completionWindow.Width = _completionWindow.Width + 60;
                _completionWindow.Height = 150;
                IList<ICompletionData> data = _completionWindow.CompletionList.CompletionData;
                foreach (string k in _keywords)
                {
                    data.Add(new CompData(k, _functionDescriptions));
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
        public CompData(string text, Dictionary<String, FunctionDescription> descriptions)
        {
            Text = text;
            Descriptions = descriptions;
        }

        public double Priority { get; set; }

        public ImageSource Image
        {
            get { return null; }
        }

        public string Text { get; private set; }
        public Dictionary<string, FunctionDescription> Descriptions { get; set; }

        // Use this property if you want to show a fancy UIElement in the list.
        public object Content
        {
            get
            {
                var vm = new DescriptionBoxViewModel(Descriptions[Text]);
                var item = new CompletionListItem();
                item.DataContext = vm;
                return item;
            }
        }

        public object Description
        {
            get
            {
                var vm = new DescriptionBoxViewModel(Descriptions[Text]);
                var box = new DescriptionBox();
                box.DataContext = vm;
                return box;
            }
        }

        public void Complete(TextArea textArea, ISegment completionSegment,
                             EventArgs insertionRequestEventArgs)
        {
            textArea.Document.Replace(completionSegment, Text);
        }
    }
}