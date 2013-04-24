using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEGIScript.Lang.Scoping
{
    static class ScopeMediator
    {
        public static event EventHandler<EventArgs> newScope;
        public static event EventHandler<EventArgs> removeScope;

        public static void NewScope()
        {
            newScope(null, new EventArgs());
        }

        public static void RemoveScope()
        {
            removeScope(null, new EventArgs());
        }
    }
}
