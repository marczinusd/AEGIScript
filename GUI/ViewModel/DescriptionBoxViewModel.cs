namespace AEGIScript.GUI.ViewModel
{
    class DescriptionBoxViewModel : ViewModelBase
    {
        public DescriptionBoxViewModel(FunctionDescription desc)
        {
            Description = desc;
        }

        public void SetDescription(FunctionDescription desc)
        {
            Description = desc;
            OnPropertyChanged("Description");
        }

        public FunctionDescription Description { get; set; }
    }
}
