using System;
using System.Collections.Generic;
using AEGIScript.Lang.Evaluation.ExpressionNodes;
using AEGIScript.Lang.Evaluation.PrimitiveNodes;
using Antlr.Runtime.Tree;
using ELTE.AEGIS.IO;
using ELTE.AEGIS.IO.GeoTiff;

namespace AEGIScript.Lang.Evaluation.AEGISNodes
{
    internal class AEGISReaderNode : FunCallNode
    {
        public AEGISReaderNode(CommonTree tree)
            : base(tree)
        {
            Attributes = new FunCallAttributes(Type.Geometry);
            ReadAttribs = new FunCallAttributes(Type.Geometry, new List<Type> {Type.String});
        }

        public FunCallAttributes ReadAttribs { get; set; }
        public GeometryStreamReader Reader { get; set; }

        public override void Call(List<TermNode> args, Type caller)
        {
            if (args[0].ActualType == Type.String && FunName == "read")
            {
                string path = ((StringNode) args[0]).Value;
                switch (caller)
                {
                    case Type.Tiffreader:
                        Reader = new TiffReader(path);
                        break;
                    case Type.Geotiffreader:
                        Reader = new GeoTiffReader();
                        break;
                    case Type.Shapefreader:
                        Reader = new ShapefileReader(path);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("caller");
                }
                ReturnValue = new GeometryNode(Reader.Read());
            }
        }
    }
}