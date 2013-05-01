using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AEGIScript.GUI.ViewModel
{
    public class FunctionDescription
    {
        public enum FunctionType
        {
            Ctor,
            Call
        };

        public FunctionType FunType;
        public string Name { get; private set; }
        private readonly string _calledOn;
        private readonly string _returns;

        public static ImageSource Arrow = new BitmapImage(new Uri("../../Res/Icons/arrow.png", UriKind.Relative));
        public static ImageSource Constr = new BitmapImage(new Uri("../../Res/Icons/constructor.png", UriKind.Relative));
        public static ImageSource BlueArrow = new BitmapImage(new Uri("../../Res/Icons/bluebgrarrow.png", UriKind.Relative));
        public static ImageSource BlueCtor = new BitmapImage(new Uri("../../Res/Icons/bluebgrctor.png", UriKind.Relative));

        public ImageSource Image { get; set; }
        public ImageSource BlueImage { get; set; }

        public string FunName
        {
            get { return Name.ToUpper() + "(" + Parameters + ")"; }
        }

        public string CalledOn
        {
            get { return "Callable on: " + _calledOn.ToUpper(); }
        }

        public string Returns
        {
            get { return "Returns: " + _returns.ToUpper(); }
        }

        public string Parameters { get; set; }

        public string Description { get;
            private set;
        }

        public FunctionDescription(String funName, String calledOn, String parameters, String returns, String description, FunctionType type)
        {
            Name = funName;
            _calledOn = calledOn;
            _returns = returns;
            Parameters = parameters;
            Description = description;
            FunType = type;
            switch (FunType)
            {
                case FunctionType.Ctor:
                    Image = Constr;
                    BlueImage = BlueCtor;
                    break;
                case FunctionType.Call:
                    Image = Arrow;
                    BlueImage = BlueArrow;
                    break;
            }
        }

    }
}
