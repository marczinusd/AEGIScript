using System;
using System.ComponentModel;

namespace AEGIScript.GUI.Model
{
    /// <summary>
    ///     Eventargs for continuous output of the interpreter
    ///     Should consider the performance before actual use
    /// </summary>
    class InterpreterProgressChangedArgs : ProgressChangedEventArgs
    {
        public InterpreterProgressChangedArgs(int progressPercentage, object userState, String currentOutput)
            : base(progressPercentage, userState)
        {
            CurrentOutput = currentOutput;
        }

        public String CurrentOutput { get; set; }
    }
}