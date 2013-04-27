﻿using System;
using System.Collections.Generic;
using System.IO;
using Antlr.Runtime.Tree;
using ELTE.AEGIS.Core;
using ELTE.AEGIS.IO;
using ELTE.AEGIS.IO.GeoTiff;

namespace AEGIScript.Lang.Evaluation
{
    internal class TermNode : ASTNode
    {
        public TermNode(CommonTree tree, String content) : base(tree)
        {
            Funs = new Dictionary<string, FunCallNode.FunCallAttributes>();
            SupportedFuns = new HashSet<string>();
        }

        public TermNode(CommonTree tree)
        {
            Funs = new Dictionary<string, FunCallNode.FunCallAttributes>();
            SupportedFuns = new HashSet<string>();
        }

        public TermNode()
        {
            Funs = new Dictionary<string, FunCallNode.FunCallAttributes>();
            SupportedFuns = new HashSet<string>();
        }

        public Dictionary<String, FunCallNode.FunCallAttributes> Funs { get; set; }
        public HashSet<String> SupportedFuns { get; set; }

        public virtual TermNode CallFun(FunCallNode func)
        {
            switch (func.FunName)
            {
                default:
                    throw new Exception(func.BadCallMessage());
            }
        }
    }

    // ReSharper disable InconsistentNaming
    internal class IGeometryNode : TermNode
        // ReSharper restore InconsistentNaming
    {
        public IGeometryNode(CommonTree tree)
            : base(tree)
        {
            ActualType = Type.Geometry;
        }

        public IGeometryNode(IGeometry geom)
        {
            Value = geom;
            Value.GeometryChanged += Value_GeometryChanged;
            ActualType = Type.Geometry;
        }

        public IGeometry Value { get; set; }

        private void Value_GeometryChanged(object sender, EventArgs e)
        {
            // todo?
        }

        public override TermNode CallFun(FunCallNode func)
        {
            switch (func.FunName)
            {
                case "Boundary":
                    return Boundary();
                case "ConvexHull":
                    return ConvexHull();
                case "Envelope":
                    return Envelope();
                case "Centroid":
                    return Centroid();
                case "Clone":
                    return Clone();
                case "Dimension":
                    return Dimension();
                case "DimensionType":
                    return DimensionType();
                case "Name":
                    return Name();
                case "ReferenceSystem":
                    return ReferenceSystem();
                case "IsValid":
                    return IsValid();
                case "IsSimple":
                    return IsSimple();
                case "IsEmpty":
                    return IsEmpty();
                default:
                    throw new Exception(func.BadCallMessage());
            }
        }

        private IGeometryNode Boundary()
        {
            return new IGeometryNode(Value.Boundary);
        }

        private IGeometryNode ConvexHull()
        {
            return new IGeometryNode(Value.ConvexHull);
        }

        private EnvelopeNode Envelope()
        {
            return new EnvelopeNode(Value.Envelope);
        }

        private CoordinateNode Centroid()
        {
            return new CoordinateNode(Value.Centroid);
        }

        private IGeometryNode Clone()
        {
            return new IGeometryNode(Value.Clone());
        }

        private IntNode Dimension()
        {
            return new IntNode(Value.Dimension);
        }

        private GeometryDimNode DimensionType()
        {
            return new GeometryDimNode(Value.DimensionType);
        }

        private StringNode Name()
        {
            return new StringNode(Value.Name);
        }

        private BooleanNode IsValid()
        {
            return new BooleanNode(Value.IsValid);
        }

        private BooleanNode IsSimple()
        {
            return new BooleanNode(Value.IsSimple);
        }

        private BooleanNode IsEmpty()
        {
            return new BooleanNode(Value.IsEmpty);
        }

        private ReferenceSystemNode ReferenceSystem()
        {
            return new ReferenceSystemNode(Value.ReferenceSystem);
        }


        public override string ToString()
        {
            return Value.ToString();
        }
    }

    internal class EnvelopeNode : TermNode
    {
        public EnvelopeNode(Envelope envelope)
        {
            Value = envelope;
            ActualType = Type.Envelope;
        }

        public Envelope Value { get; set; }

        public override TermNode CallFun(FunCallNode func)
        {
            switch (func.FunName)
            {
                case "Center":
                    return Center();
                case "Contains":
                    if (func.ResolvedArgs.Count == 1)
                    {
                        if (func.ResolvedArgs[0].ActualType == Type.Coordinate)
                        {
                            return Contains(func.ResolvedArgs[0] as CoordinateNode);
                        }
                        if (func.ResolvedArgs[0].ActualType == Type.Envelope)
                        {
                            return Contains(func.ResolvedArgs[0] as EnvelopeNode);
                        }
                    }
                    throw new Exception();
                case "Crosses":
                    if (func.ResolvedArgs.Count == 1 && func.ResolvedArgs[0].ActualType == Type.Envelope)
                    {
                        return Crosses(func.ResolvedArgs[0] as EnvelopeNode);
                    }
                    throw new Exception(func.BadCallMessage());
                case "Disjoint":
                    if (func.ResolvedArgs.Count == 1 && func.ResolvedArgs[0].ActualType == Type.Envelope)
                    {
                        return Disjoint(func.ResolvedArgs[0] as EnvelopeNode);
                    }
                    throw new Exception(func.BadCallMessage());
                case "Expand":
                    if (func.ResolvedArgs.Count == 1 && func.ResolvedArgs[0].ActualType == Type.Coordinate)
                    {
                        return Expand(func.ResolvedArgs[0] as CoordinateNode);
                    }
                    throw new Exception(func.BadCallMessage());
                case "Distance":
                    if (func.ResolvedArgs.Count == 1 && func.ResolvedArgs[0].ActualType == Type.Envelope)
                    {
                        return Distance(func.ResolvedArgs[0] as EnvelopeNode);
                    }
                    throw new Exception(func.BadCallMessage());
                case "Overlaps":
                    if (func.ResolvedArgs.Count == 1 && func.ResolvedArgs[0].ActualType == Type.Envelope)
                    {
                        return Overlaps(func.ResolvedArgs[0] as EnvelopeNode);
                    }
                    throw new Exception(func.BadCallMessage());
                case "Touches":
                    if (func.ResolvedArgs.Count == 1 && func.ResolvedArgs[0].ActualType == Type.Envelope)
                    {
                        return Touches(func.ResolvedArgs[0] as EnvelopeNode);
                    }
                    throw new Exception(func.BadCallMessage());
                case "Within":
                    if (func.ResolvedArgs.Count == 1 && func.ResolvedArgs[0].ActualType == Type.Envelope)
                    {
                        return Within(func.ResolvedArgs[0] as EnvelopeNode);
                    }
                    throw new Exception(func.BadCallMessage());
                case "Intersects":
                    if (func.ResolvedArgs.Count == 1 && func.ResolvedArgs[0].ActualType == Type.Envelope)
                    {
                        return Intersects(func.ResolvedArgs[0] as EnvelopeNode);
                    }
                    throw new Exception(func.BadCallMessage());
                case "Maximum":
                    return Maximum();
                case "MaxZ":
                    return MaxZ();
                case "MaxX":
                    return MaxX();
                case "MaxY":
                    return MaxY();
                case "Minimum":
                    return Minimum();
                case "MinX":
                    return MinX();
                case "MinY":
                    return MinY();
                case "MinZ":
                    return MinZ();
                case "IsValid":
                    return IsValid();
                case "IsPlanar":
                    return IsPlanar();
                case "IsEmpty":
                    return IsEmpty();
                default:
                    throw new Exception(func.BadCallMessage());
            }
        }


        private CoordinateNode Center()
        {
            return new CoordinateNode(Value.Center);
        }

        private BooleanNode Contains(CoordinateNode other)
        {
            return new BooleanNode(Value.Contains(other.Value));
        }

        private BooleanNode Contains(EnvelopeNode other)
        {
            return new BooleanNode(Value.Contains(other.Value));
        }

        private BooleanNode Crosses(EnvelopeNode other)
        {
            return new BooleanNode(Value.Crosses(other.Value));
        }

        private BooleanNode Disjoint(EnvelopeNode other)
        {
            return new BooleanNode(Value.Disjoint(other.Value));
        }

        private DoubleNode Distance(EnvelopeNode other)
        {
            return new DoubleNode(Value.Distance(other.Value));
        }

        private EnvelopeNode Expand(CoordinateNode other)
        {
            Value.Expand(other.Value);
            return this;
        }

        private BooleanNode Intersects(EnvelopeNode other)
        {
            return new BooleanNode(Value.Intersects(other.Value));
        }

        private BooleanNode Overlaps(EnvelopeNode other)
        {
            return new BooleanNode(Value.Overlaps(other.Value));
        }

        private BooleanNode Touches(EnvelopeNode other)
        {
            return new BooleanNode(Value.Touches(other.Value));
        }

        private BooleanNode Within(EnvelopeNode other)
        {
            return new BooleanNode(Value.Within(other.Value));
        }

        private BooleanNode IsValid()
        {
            return new BooleanNode(Value.IsValid);
        }

        private BooleanNode IsEmpty()
        {
            return new BooleanNode(Value.IsEmpty);
        }

        private BooleanNode IsPlanar()
        {
            return new BooleanNode(Value.IsPlanar);
        }

        private CoordinateNode Maximum()
        {
            return new CoordinateNode(Value.Maximum);
        }

        private CoordinateNode Minimum()
        {
            return new CoordinateNode(Value.Minimum);
        }

        private DoubleNode MaxX()
        {
            return new DoubleNode(Value.MaxX);
        }

        private DoubleNode MaxY()
        {
            return new DoubleNode(Value.MaxY);
        }

        private DoubleNode MaxZ()
        {
            return new DoubleNode(Value.MaxZ);
        }

        private DoubleNode MinX()
        {
            return new DoubleNode(Value.MinX);
        }

        private DoubleNode MinY()
        {
            return new DoubleNode(Value.MinY);
        }

        private DoubleNode MinZ()
        {
            return new DoubleNode(Value.MinZ);
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    internal class CoordinateNode : TermNode
    {
        public CoordinateNode(Coordinate coord)
        {
            Value = coord;
            ActualType = Type.Coordinate;
        }

        public Coordinate Value { get; set; }

        public override TermNode CallFun(FunCallNode func)
        {
            switch (func.FunName)
            {
                case "IsValid":
                    return IsValid();
                case "IsEmpty":
                    return IsEmpty();
                case "X":
                    return X();
                case "Y":
                    return Y();
                case "Z":
                    return Z();
                default:
                    throw new Exception(func.BadCallMessage());
            }
        }

        private DoubleNode X()
        {
            return new DoubleNode(Value.X);
        }

        private DoubleNode Y()
        {
            return new DoubleNode(Value.Y);
        }

        private DoubleNode Z()
        {
            return new DoubleNode(Value.Z);
        }

        private BooleanNode IsValid()
        {
            return new BooleanNode(Value.IsValid);
        }

        private BooleanNode IsEmpty()
        {
            return new BooleanNode(Value.IsEmpty);
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    internal class GeometryDimNode : TermNode
    {
        public GeometryDimNode(GeometryDimension dim)
        {
            Value = dim;
            ActualType = Type.Geometrydim;
        }

        public GeometryDimension Value { get; set; }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    internal class MetadataNode : TermNode
    {
        public MetadataNode(IDictionary<string, object> val)
        {
            Value = val;
            ActualType = Type.Metadata;
        }

        public IDictionary<string, object> Value { get; set; }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    internal class ReferenceSystemNode : TermNode
    {
        public ReferenceSystemNode(IReferenceSystem referenceSystem)
        {
            Value = referenceSystem;
            ActualType = Type.ReferenceSys;
        }

        public IReferenceSystem Value { get; set; }

        public override TermNode CallFun(FunCallNode func)
        {
            switch (func.FunName)
            {
                case "Dimension":
                    return Dimension();
                case "Name":
                    return Name();
                case "Identifier":
                    return Identifier();
                default:
                    throw new Exception(func.BadCallMessage());
            }
        }

        private IntNode Dimension()
        {
            return new IntNode(Value.Dimension);
        }

        private StringNode Name()
        {
            return new StringNode(Value.Name);
        }

        private StringNode Identifier()
        {
            return new StringNode(Value.Identifier);
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

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
                            resArr.Elements.Add(new IGeometryNode(geometry));
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

    internal class ShapeFileReaderNode : GeometryStreamReaderNode
    {
        public ShapeFileReaderNode(String path) : base(new ShapefileReader(path))
        {
        }
    }

    internal class TiffReaderNode : GeometryStreamReaderNode
    {
        public TiffReaderNode(String path) : base(new TiffReader(path))
        {
        }
    }

    /*
    class GeoTiffReaderNode : TiffReaderNode
    {
        public GeoTiffReaderNode()
        {
            
        }
    }*/

    internal class AEGISReaderNode : FunCallNode
    {
        public AEGISReaderNode(CommonTree tree, String path)
            : base(tree)
        {
            Attributes = new FunCallAttributes(Type.Geometry);
            ReadAttribs = new FunCallAttributes(Type.Geometry, new List<Type> {Type.String});
        }

        public AEGISReaderNode(CommonTree tree, Stream stream)
            : base(tree)
        {
            Attributes = new FunCallAttributes(Type.Geometry);
        }

        public AEGISReaderNode(CommonTree tree)
            : base(tree)
        {
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
                ReturnValue = new IGeometryNode(Reader.Read());
            }
        }
    }
}