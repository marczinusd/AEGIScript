using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEGIScript.Lang.Evaluation
{
    public interface IVisitor
    {
        void Visit(ASTNode node);
    }
}
