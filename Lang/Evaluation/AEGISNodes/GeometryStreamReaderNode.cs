using System;
using AEGIScript.Lang.Evaluation.ExpressionNodes;
using AEGIScript.Lang.Evaluation.PrimitiveNodes;
using ELTE.AEGIS.Core;
using ELTE.AEGIS.IO;

namespace AEGIScript.Lang.Evaluation.AEGISNodes
{
    internal class GeometryStreamReaderNode : TermNode
    {
        public GeometryStreamReaderNode(GeometryStreamReader reader)
        {
            Reader = reader;
        }

        public GeometryStreamReader Reader { get; set; }

        public override string ToString()
        {
            return Reader.ToString();
        }

        public override TermNode CallFun(FunCallNode func)
        {
            switch (func.FunName)
            {
                case "Read":
                    if (func.ResolvedArgs.Count == 0)
                    {
                        IGeometry[] geoms = Reader.ReadToEnd();
                        Reader.Close();
                        var resArr = new ArrayNode();
                        foreach (IGeometry geometry in geoms)
                        {
                            resArr.Elements.Add(new GeometryNode(geometry));
                        }

                        return resArr;
                    }
                    throw new Exception(func.BadCallMessage());
                case "GeometryCount":
                    if (func.ResolvedArgs.Count == 0)
                    {
                        return GeometryCount();
                    }
                    throw new Exception(func.BadCallMessage());
                case "ReadMetadata":
                    if (func.ResolvedArgs.Count == 0)
                    {
                        return ReadMetadata();
                    }
                    throw new Exception(func.BadCallMessage());
            }
            throw new Exception(func.BadCallMessage());
        }

        protected virtual IntNode GeometryCount()
        {
            return new IntNode(Reader.GeometryCount);
        }

        protected virtual MetadataNode ReadMetadata()
        {
            return new MetadataNode(Reader.ReadMetadata());
        }
    }
}