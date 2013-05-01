using System;
using ELTE.AEGIS.IO;

namespace AEGIScript.Lang.Evaluation.AEGISNodes
{
    internal class TiffReaderNode : GeometryStreamReaderNode
    {
        public TiffReaderNode(String path) : base(new TiffReader(path))
        {
        }
    }
}