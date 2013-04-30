using System;
using System.Collections.Generic;
using System.Linq;
using AEGIScript.Lang.Exceptions;
using Antlr.Runtime.Tree;
using ELTE.AEGIS.Core;
using ELTE.AEGIS.Core.Geometry;
using ELTE.AEGIS.Core.Geometry.Raster;
using ELTE.AEGIS.IO;
using ELTE.AEGIS.IO.GeoTiff;

namespace AEGIScript.Lang.Evaluation
{
    internal class TermNode : ASTNode
    {
        public TermNode(CommonTree tree, String content) : base(tree)
        {
        }

        public TermNode(CommonTree tree)
        {
        }

        public TermNode()
        {
        }


        /// <summary>
        ///     Provides an interface for the interpreter to call functions defined by the nodes
        /// </summary>
        /// <param name="func">Function node</param>
        /// <returns>Result of the function call</returns>
        public virtual TermNode CallFun(FunCallNode func)
        {
            switch (func.FunName)
            {
                default:
                    throw ExceptionGenerator.UndefinedFunction(func, ActualType);
            }
        }

        protected TermNode Call(FunCallNode func, Func<TermNode> funToCall)
        {
            if (func.ResolvedArgs.Count == 0)
            {
                return funToCall.Invoke();
            }
            throw ExceptionGenerator.UndefinedFunction(func, ActualType);
        }

        protected TermNode Call<T>(Type[] funSig, Type[] actualSig, FunCallNode fnode, Func<T, TermNode> func)
            where T : class
        {
            if (funSig.Length != actualSig.Length)
                throw ExceptionGenerator.BadArity(fnode);

            if (!MatchesSignature(funSig, actualSig))
                throw ExceptionGenerator.BadArguments(fnode, funSig);

            var arg = fnode.ResolvedArgs[0] as T;
            return func.Invoke(arg);
        }

        protected TermNode Call<T1, T2>(Type[] funSig, Type[] actualSig, FunCallNode fnode,
                                        Func<T1, T2, TermNode> func)
            where T1 : class
            where T2 : class
        {
            if (funSig.Length != actualSig.Length)
                throw ExceptionGenerator.BadArity(fnode);

            if (!MatchesSignature(funSig, actualSig))
                throw ExceptionGenerator.BadArguments(fnode, funSig);

            var arg1 = fnode.ResolvedArgs[0] as T1;
            var arg2 = fnode.ResolvedArgs[1] as T2;
            return func.Invoke(arg1, arg2);
        }

        protected TermNode Call<T1, T2, T3>(Type[] funSig, Type[] actualSig, FunCallNode fnode,
                                            Func<T1, T2, T3, TermNode> func)
            where T1 : class
            where T2 : class
            where T3 : class
        {
            if (funSig.Length != actualSig.Length)
                throw ExceptionGenerator.BadArity(fnode);

            if (!MatchesSignature(funSig, actualSig))
                throw ExceptionGenerator.BadArguments(fnode, funSig);

            var arg1 = fnode.ResolvedArgs[0] as T1;
            var arg2 = fnode.ResolvedArgs[0] as T2;
            var arg3 = fnode.ResolvedArgs[0] as T3;
            return func.Invoke(arg1, arg2, arg3);
        }

        protected bool MatchesSignature(Type[] funSig, Type[] argSig)
        {
            return !argSig.Where((t, i) => funSig[i] != t && funSig[i] != Type.Any).Any();
        }
    }

    internal class GeometryNode : TermNode
    {
        public GeometryNode(CommonTree tree)
            : base(tree)
        {
            ActualType = Type.Geometry;
        }

        public GeometryNode(IGeometry geom)
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
                    return Call(func, Boundary);
                case "ConvexHull":
                    return Call(func, ConvexHull);
                case "Envelope":
                    return Call(func, Envelope);
                case "Centroid":
                    return Call(func, Centroid);
                case "Clone":
                    return Call(func, Clone);
                case "Dimension":
                    return Call(func, Dimension);
                case "DimensionType":
                    return Call(func, DimensionType);
                case "Name":
                    return Call(func, Name);
                case "ReferenceSystem":
                    return Call(func, ReferenceSystem);
                case "IsValid":
                    return Call(func, IsValid);
                case "IsSimple":
                    return Call(func, IsSimple);
                case "IsEmpty":
                    return Call(func, IsEmpty);
                default:
                    return base.CallFun(func);
            }
        }

        private GeometryNode Boundary()
        {
            return new GeometryNode(Value.Boundary);
        }

        private GeometryNode ConvexHull()
        {
            return new GeometryNode(Value.ConvexHull);
        }

        private EnvelopeNode Envelope()
        {
            return new EnvelopeNode(Value.Envelope);
        }

        private CoordinateNode Centroid()
        {
            return new CoordinateNode(Value.Centroid);
        }

        private GeometryNode Clone()
        {
            return new GeometryNode(Value.Clone());
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

    internal class HistogramNode : TermNode
    {
        public HistogramNode(Histogram value)
        {
            ActualType = Type.Histogram;
            Value = value;
        }

        public UInt32Node MinimalIntensity()
        {
            return new UInt32Node(Value.MinimalIntensity);
        }

        public UInt32Node MaximalIntensity()
        {
            return new UInt32Node(Value.MaximalIntensity);
        }

        public UInt32Node MaximalOccurence()
        {
            return new UInt32Node(Value.MaximalOccurrance);
        }

        public IntNode PixelCount()
        {
            return new IntNode(Value.PixelCount);
        }

        public DoubleNode MeanValue()
        {
            return new DoubleNode(Value.MeanValue);
        }

        public DoubleNode DeviationValue()
        {
            return new DoubleNode(Value.DeviationValue);
        }

        public IntNode RadiometricResolution()
        {
            return new IntNode(Value.RadiometricResolution);
        }

        public ArrayNode Values()
        {
            var nodeVals = Value.Values
                                .Select(val => new UInt32Node(val)).Cast<TermNode>().ToList();
            return new ArrayNode(nodeVals);
        }

        public ArrayNode CulcumativeDistributionValues()
        {
            var nodeVals = Value.CulcumativeDistibutionValues
                                .Select(val => new UInt32Node(val)).Cast<TermNode>().ToList();
            return new ArrayNode(nodeVals);
        }

        public DoubleNode OtsuThreshold()
        {
            return new DoubleNode(Value.OtsuThreshold);
        }

        public Histogram Value { get; private set; }
    }

    internal class RectangleNode : PolygonNode
    {
        public RectangleNode(Rectangle rect) : base(rect)
        {
            ActualType = Type.Rectangle;
        }

        public override TermNode CallFun(FunCallNode func)
        {
            switch (func.FunName)
            {
                default:
                    return base.CallFun(func);
            }
        }
    }

    internal class RasterBandNode : SurfaceNode
    {
        public RasterBandNode(RasterBand rband)
            : base(rband)
        {
            ActualType = Type.RasterBand;
        }
    }     

    internal class RasterNode : RectangleNode
    {
        public RasterNode(Raster raster) : base(raster)
        {
            ActualType = Type.Raster;
        }

        public override TermNode CallFun(FunCallNode func)
        {
            switch (func.FunName)
            {
                default:
                    return base.CallFun(func);
            }
        }

        protected IntNode PixelWidth()
        {
            return new IntNode(((Raster) Value).PixelWidth);
        }

        protected IntNode PixelHeight()
        {
            return new IntNode(((Raster) Value).PixelWidth);
        }

        protected IntNode SpectralResolution()
        {
            return new IntNode(((Raster)Value).SpectralResolution);
        }

        protected ArrayNode Bands()
        {/*
            var rasterBands = ((Raster) Value).Bands;
            List<RasterNode> rasterNodes = new List<RasterNode>();
            foreach (var raster in rasterBands)
            {
                rasterNodes.Add(new RasterNode(raster));
            }*/
            return new ArrayNode();
        }

    }

    internal class LineStringNode : CurveNode
    {
        public LineStringNode(LineString ls) : base(ls)
        {
            ActualType = Type.LineString;
        }

        public override TermNode CallFun(FunCallNode func)
        {
            Type[] actualTypes = func.ResolvedArgs.Select(x => x.ActualType).ToArray();
            switch (func.FunName)
            {
                case "GetCoordinate":
                    return Call<IntNode>(new[] {Type.Int}, actualTypes, func, GetCoordinate);
                case "SetCoordinate":
                    return Call<IntNode, CoordinateNode>(new[] {Type.Int, Type.Coordinate}, actualTypes,
                                                         func, SetCoordinate);
                case "Includes":
                    return Call<CoordinateNode>(new[] {Type.Coordinate}, actualTypes, func, Includes);
                case "Add":
                    return Call<CoordinateNode>(new[] {Type.Coordinate}, actualTypes, func, Add);
                case "Insert":
                    return Call<IntNode, CoordinateNode>(new[] {Type.Int, Type.Coordinate}, actualTypes,
                                                         func, Insert);
                case "Remove":
                    return Call<CoordinateNode>(new[] {Type.Coordinate}, actualTypes, func, Remove);
                case "RemoveAt":
                    return Call<IntNode>(new[] {Type.Int}, actualTypes, func, RemoveAt);
                case "Clear":
                    return Call(func, Clear);

                default:
                    return base.CallFun(func);
            }
        }

        private LineStringNode SetCoordinate(IntNode ind, CoordinateNode coord)
        {
            ((LineString) Value).SetCoordinate(ind.Value, coord.Value);
            return this;
        }

        private BooleanNode Includes(CoordinateNode coord)
        {
            return new BooleanNode(((LineString) Value).Includes(coord.Value));
        }

        private LineStringNode Add(CoordinateNode coord)
        {
            ((LineString) Value).Add(coord.Value);
            return this;
        }

        private LineStringNode Insert(IntNode ind, CoordinateNode coord)
        {
            ((LineString) Value).Insert(ind.Value, coord.Value);
            return this;
        }

        private LineStringNode Remove(CoordinateNode node)
        {
            ((LineString) Value).Remove(node.Value);
            return this;
        }

        private LineStringNode RemoveAt(IntNode ind)
        {
            ((LineString) Value).RemoveAt(ind.Value);
            return this;
        }

        private LineStringNode Clear()
        {
            ((LineString) Value).Clear();
            return this;
        }

        private CoordinateNode GetCoordinate(IntNode ind)
        {
            return new CoordinateNode(((LineString) Value).GetCoordinate(ind.Value));
        }
    }

    internal class CurveNode : GeometryNode
    {
        public CurveNode(Curve curve) : base(curve)
        {
            ActualType = Type.Curve;
        }

        public override TermNode CallFun(FunCallNode func)
        {
            switch (func.FunName)
            {
                case "IsClosed":
                    return Call(func, IsClosed);
                case "IsRing":
                    return Call(func, IsRing);
                case "Count":
                    return Call(func, Count);
                case "Length":
                    return Call(func, Length);
                case "Coordinates":
                    return Call(func, Coordinates);
                case "StartCoordinate":
                    return Call(func, StartCoordinate);
                case "EndCoordinate":
                    return Call(func, EndCoordinate);

                default:
                    return base.CallFun(func);
            }
        }

        private BooleanNode IsClosed()
        {
            return new BooleanNode(((Curve) Value).IsClosed);
        }

        private BooleanNode IsRing()
        {
            return new BooleanNode(((Curve) Value).IsRing);
        }

        private IntNode Count()
        {
            return new IntNode(((Curve) Value).Count);
        }

        private DoubleNode Length()
        {
            return new DoubleNode(((Curve) Value).Length);
        }

        private ArrayNode Coordinates()
        {
            var curve = Value as Curve;
            var coords = curve.Coordinates.Select(t => new CoordinateNode(t)).Cast<TermNode>().ToList();
            return new ArrayNode(coords);
        }

        private CoordinateNode StartCoordinate()
        {
            return new CoordinateNode(((Curve) Value).StartCoordinate);
        }

        private CoordinateNode EndCoordinate()
        {
            return new CoordinateNode(((Curve) Value).EndCoordinate);
        }
    }

    internal class PolygonNode : SurfaceNode
    {
        public PolygonNode(Polygon poly) : base(poly)
        {
            ActualType = Type.Polygon;
        }

        public override TermNode CallFun(FunCallNode func)
        {
            switch (func.FunName)
            {
                default:
                    return base.CallFun(func);
            }
        }
    }

    internal class SurfaceNode : GeometryNode
    {
        public SurfaceNode(Surface surf) : base(surf)
        {
            ActualType = Type.Surface;
        }

        public override TermNode CallFun(FunCallNode func)
        {
            switch (func.FunName)
            {
                case "IsConvex":
                    return Call(func, IsConvex);
                case "IsDivided":
                    return Call(func, IsDivided);
                case "IsWhole":
                    return Call(func, IsWhole);
                case "Area":
                    return Call(func, Area);
                case "Perimeter":
                    return Call(func, Perimeter);
                default:
                    return base.CallFun(func);
            }
        }

        public BooleanNode IsConvex()
        {
            return new BooleanNode(((Surface) Value).IsConvex);
        }

        public BooleanNode IsDivided()
        {
            return new BooleanNode(((Surface) Value).IsDivided);
        }

        public BooleanNode IsWhole()
        {
            return new BooleanNode(((Surface) Value).IsWhole);
        }

        public DoubleNode Area()
        {
            return new DoubleNode(((Surface) Value).Area);
        }

        public DoubleNode Perimeter()
        {
            return new DoubleNode(((Surface) Value).Perimeter);
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
            Type[] actualTypes = func.ResolvedArgs.Select(x => x.ActualType).ToArray();
            switch (func.FunName)
            {
                case "Center":
                    return Call(func, Center);
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
                    return Call<EnvelopeNode>(new[] {Type.Envelope}, actualTypes, func, Crosses);
                case "Disjoint":
                    return Call<EnvelopeNode>(new[] {Type.Envelope}, actualTypes, func, Disjoint);
                case "Expand":
                    return Call<CoordinateNode>(new[] {Type.Coordinate}, actualTypes, func, Expand);
                case "Distance":
                    return Call<EnvelopeNode>(new[] {Type.Envelope}, actualTypes, func, Distance);
                case "Overlaps":
                    return Call<EnvelopeNode>(new[] {Type.Envelope}, actualTypes, func, Overlaps);
                case "Touches":
                    return Call<EnvelopeNode>(new[] {Type.Envelope}, actualTypes, func, Touches);
                case "Within":
                    return Call<EnvelopeNode>(new[] {Type.Envelope}, actualTypes, func, Within);
                case "Intersects":
                    return Call<EnvelopeNode>(new[] {Type.Envelope}, actualTypes, func, Intersects);
                case "Maximum":
                    return Call(func, Maximum);
                case "MaxZ":
                    return Call(func, MaxZ);
                case "MaxX":
                    return Call(func, MaxX);
                case "MaxY":
                    return Call(func, MaxY);
                case "Minimum":
                    return Call(func, Minimum);
                case "MinX":
                    return Call(func, MinX);
                case "MinY":
                    return Call(func, MinY);
                case "MinZ":
                    return Call(func, MinZ);
                case "IsValid":
                    return Call(func, IsValid);
                case "IsPlanar":
                    return Call(func, IsPlanar);
                case "IsEmpty":
                    return Call(func, IsEmpty);
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
                    return Call(func, IsValid);
                case "IsEmpty":
                    return Call(func, IsEmpty);
                case "X":
                    return Call(func, X);
                case "Y":
                    return Call(func, Y);
                case "Z":
                    return Call(func, Z);
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

        public ReferenceSystemNode()
        {
            Value = GeometryFactory.ReferenceSystem;
        }

        public IReferenceSystem Value { get; set; }

        public override TermNode CallFun(FunCallNode func)
        {
            switch (func.FunName)
            {
                case "Dimension":
                    return Call(func, Dimension);
                case "Name":
                    return Call(func, Dimension);
                case "Identifier":
                    return Call(func, Identifier);
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

    internal class GeoTiffReaderNode : TiffReaderNode
    {
        /// <summary>
        ///     todo
        /// </summary>
        public GeoTiffReaderNode() : base("")
        {
        }
    }

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