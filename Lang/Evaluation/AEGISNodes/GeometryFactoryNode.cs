using System;
using System.Collections.Generic;
using System.Linq;
using AEGIScript.Lang.Evaluation.ExpressionNodes;
using AEGIScript.Lang.Evaluation.PrimitiveNodes;
using AEGIScript.Lang.Exceptions;
using ELTE.AEGIS.Core;
using ELTE.AEGIS.Core.Geometry;

namespace AEGIScript.Lang.Evaluation.AEGISNodes
{
    internal class GeometryFactoryNode : TermNode
    {
        public override TermNode CallFun(FunCallNode func)
        {
            try
            {
                switch (func.FunName)
                {
                    case "Point":
                        switch (func.ResolvedArgs.Count)
                        {
                            case 1:
                                return CreatePoint(func.ResolvedArgs[0]);
                            case 2:
                                return CreatePoint(func.ResolvedArgs[0], func.ResolvedArgs[1]);
                            case 3:
                                return CreatePoint(func.ResolvedArgs[0], func.ResolvedArgs[1], func.ResolvedArgs[2]);
                            default:
                                throw ExceptionGenerator.BadArity(func);
                        }

                    case "Coordinate":
                        switch (func.ResolvedArgs.Count)
                        {
                            case 2:
                                return CreateCoordinate(func.ResolvedArgs[0], func.ResolvedArgs[1]);
                            case 3:
                                return CreateCoordinate(func.ResolvedArgs[0], func.ResolvedArgs[1], func.ResolvedArgs[2]);
                            default:
                                throw ExceptionGenerator.BadArity(func);
                        }

                    case "Line":
                        switch (func.ResolvedArgs.Count)
                        {
                            case 1:
                                return CreateLine(func.ResolvedArgs[0]);
                            case 2:
                                return CreateLine(func.ResolvedArgs[0], func.ResolvedArgs[1]);
                            default:
                                throw ExceptionGenerator.BadArity(func);

                        }
                    case "LineString":
                        switch (func.ResolvedArgs.Count)
                        {
                            case 0:
                                return CreateLineString();
                            case 1:
                                return CreateLineString(func.ResolvedArgs[0]);
                            default:
                                throw ExceptionGenerator.BadArity(func);
                        }
                    case "LinearRing":
                        switch (func.ResolvedArgs.Count)
                        {
                            case 0:
                                return CreateLinearRing();
                            case 1:
                                return CreateLinearRing(func.ResolvedArgs[0]);
                            default:
                                throw ExceptionGenerator.BadArity(func);
                        }

                    default:
                        return base.CallFun(func);
                }
            }
            catch (ArgumentException argex)
            {
                throw new Exception("RUNTIME ERROR!\n Reason:" + argex.Message);
            }
            catch (Exception)
            {
                throw ExceptionGenerator.BadArguments(func);
            }
            
        }

        public PointNode CreatePoint(TermNode node)
        {
            switch (node.ActualType)
            {
                case Type.Point:
                    return new PointNode(GeometryFactory.CreatePoint((node as PointNode).Value as Point));
                case Type.Coordinate:
                    return new PointNode(GeometryFactory.CreatePoint((node as CoordinateNode).Value));
                default:
                    throw new Exception("Bad funcall");
            }
        }

        public CoordinateNode CreateCoordinate(TermNode first, TermNode second)
        {
            if (first.ActualType == second.ActualType && second.ActualType == Type.Double)
            {
                return new CoordinateNode(new Coordinate((first as DoubleNode).Value, (second as DoubleNode).Value));
            } throw new Exception();
        }

        public CoordinateNode CreateCoordinate(TermNode first, TermNode second, TermNode third)
        {
            if (first.ActualType == second.ActualType && second.ActualType == third.ActualType && third.ActualType == Type.Double)
            {
                return new CoordinateNode(new Coordinate((first as DoubleNode).Value,(second as DoubleNode).Value,(third as DoubleNode).Value));
            } throw new Exception();
        }

        public PointNode CreatePoint(TermNode first, TermNode second)
        {
            switch (first.ActualType)
            {
                case Type.Double:
                    if (second.ActualType == Type.Double)
                    {
                        return new PointNode(GeometryFactory.CreatePoint((first as DoubleNode).Value, (second as DoubleNode).Value));
                    } throw new Exception("bad funcall");

                default:
                    throw new Exception("bad funcall");
            }   
        }

        public PointNode CreatePoint(TermNode first, TermNode second, TermNode third)
        {
            if (first.ActualType == second.ActualType && second.ActualType == third.ActualType && third.ActualType == Type.Double)
            {
                return new PointNode(GeometryFactory.CreatePoint((first as DoubleNode).Value,(second as DoubleNode).Value, (third as DoubleNode).Value));
            } throw new Exception();
        }

        public LineStringNode CreateLineString()
        {
            return new LineStringNode(GeometryFactory.CreateLineString());
        }

        public LineStringNode CreateLineString(TermNode node)
        {
            switch (node.ActualType)
            {
                case Type.LineString:
                    return new LineStringNode((node as LineStringNode).Value as LineString);
                case Type.Array:
                    var asArray = node as ArrayNode;
                    if (asArray.Elements.Count == 0)
                    {
                        return new LineStringNode(GeometryFactory.CreateLineString(new Point[]{ }));
                    }
                    switch (asArray.Elements[0].ActualType)
                    {
                        case Type.Point:
                            if (IsUniformArray(asArray))
                            {
                                List<Point> points = asArray.Elements.Select(elem => ((PointNode) elem).Value as Point).ToList();
                                return new LineStringNode(GeometryFactory.CreateLineString(points.ToArray()));
                            } throw new Exception("RUNTIME ERROR! \n Bad funcall");
                        case Type.Coordinate:
                            if (IsUniformArray(asArray))
                            {
                                List<Coordinate> coords = asArray.Elements.Select(elem => ((CoordinateNode) elem).Value).ToList();
                                return new LineStringNode(GeometryFactory.CreateLineString(coords.ToArray()));
                            } throw new Exception("RUNTIME ERROR! \n Bad funcall");
                        default:
                            throw new Exception("RUNTIME ERROR! \n");
                    }
                default:
                    throw new Exception("RUNTIME ERROR! \n Bad funcall");
            }
        }

        private Boolean IsUniformArray(ArrayNode array)
        {
            Boolean isUniform = true;
            Type first = array.Elements[0].ActualType;
            for (int i = 0; i < array.Elements.Count && isUniform; i++)
            {
                isUniform = isUniform && (first == array.Elements[i].ActualType);
            }
            return isUniform;
        }

        public LineNode CreateLine(TermNode node)
        {
            if (node.ActualType == Type.Line)
            {
                return new LineNode(GeometryFactory.CreateLine(((LineNode) node).Value as Line));
            }
            var asArray = node as ArrayNode;
            if (asArray.Elements.Count == 0)
            {
                return new LineNode(GeometryFactory.CreateLine(new Point[] { }));
            }
            switch (asArray.Elements[0].ActualType)
            {
                case Type.Point:
                    if (IsUniformArray(asArray))
                    {
                        List<Point> points = asArray.Elements.Select(elem => ((PointNode)elem).Value as Point).ToList();
                        return new LineNode(GeometryFactory.CreateLine(points.ToArray()));
                    } throw new Exception("RUNTIME ERROR! \n Bad funcall");
                case Type.Coordinate:
                    if (IsUniformArray(asArray))
                    {
                        List<Coordinate> coords = asArray.Elements.Select(elem => ((CoordinateNode)elem).Value).ToList();
                        return new LineNode(GeometryFactory.CreateLine(coords.ToArray()));
                    } throw new Exception("RUNTIME ERROR! \n Bad funcall");
                default:
                    throw new Exception("RUNTIME ERROR! \n");
            }

        }

        public LineNode CreateLine(TermNode first, TermNode second)
        {
            switch (first.ActualType)
            {
                case Type.Point:
                    if (second.ActualType == Type.Point)
                    {
                        return new LineNode(GeometryFactory.CreateLine(((PointNode) first).Value as Point, ((PointNode) second).Value as Point));
                    } throw new Exception("RUNTIME ERROR! \n Bad funcall");
                case Type.Coordinate:
                    if (second.ActualType == Type.Point)
                    {
                        return new LineNode(GeometryFactory.CreateLine(((CoordinateNode) first).Value, ((CoordinateNode) second).Value));
                    } throw new Exception("RUNTIME ERROR! \n Bad funcall");
                default:
                    throw new Exception("RUNTIME ERROR!\n Bad funcall");
            }
        }

        public LinearRingNode CreateLinearRing()
        {
            return new LinearRingNode(GeometryFactory.CreateLinearRing());
        }

        public LinearRingNode CreateLinearRing(TermNode node)
        {
            if (node.ActualType == Type.LinearRing)
            {
                return new LinearRingNode(GeometryFactory.CreateLinearRing((node as LineStringNode).Value as LinearRing));
            }
            var asArray = node as ArrayNode;
            if (asArray.Elements.Count == 0)
            {
                return new LinearRingNode(GeometryFactory.CreateLinearRing(new Point[] { }));
            }
            switch (asArray.Elements[0].ActualType)
            {
                case Type.Point:
                    if (IsUniformArray(asArray))
                    {
                        List<Point> points = asArray.Elements.Select(elem => ((PointNode)elem).Value as Point).ToList();
                        return new LinearRingNode(GeometryFactory.CreateLinearRing(points.ToArray()));
                    } throw new Exception("RUNTIME ERROR! \n Bad funcall");
                case Type.Coordinate:
                    if (IsUniformArray(asArray))
                    {
                        List<Coordinate> coords = asArray.Elements.Select(elem => ((CoordinateNode)elem).Value).ToList();
                        return new LinearRingNode(GeometryFactory.CreateLinearRing(coords.ToArray()));
                    } throw new Exception("RUNTIME ERROR! \n Bad funcall");
                default:
                    throw new Exception("RUNTIME ERROR! \n");
            }
        }
    }
}