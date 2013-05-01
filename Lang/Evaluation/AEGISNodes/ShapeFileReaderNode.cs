using System;
using ELTE.AEGIS.IO;

namespace AEGIScript.Lang.Evaluation.AEGISNodes
{
    internal class ShapeFileReaderNode : GeometryStreamReaderNode
    {
        public ShapeFileReaderNode(String path) : base(new ShapefileReader(path))
        {
        }
    }
}