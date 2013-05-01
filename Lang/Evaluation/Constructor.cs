using System;

namespace AEGIScript.Lang.Evaluation
{
    static class Constructor
    {
        public static TermNode Construct(FunCallNode fun)
        {
            switch (fun.ResolvedArgs.Count)
            {
                case 0:
                    switch (fun.FunName)
                    {
                        case "Array":
                            return new ArrayNode();
                        default:
                            throw new Exception("RUNTIME ERROR! \n Array has no constructors that take " + fun.ResolvedArgs.Count +
                                                " arguments. Line " + fun.Line);
                    }
                case 1:
                    switch (fun.FunName)
                    {
                        case "TiffReader":
                            if (fun.ResolvedArgs[0].ActualType == ASTNode.Type.String)
                            {
                                try
                                {
                                    return new TiffReaderNode(((StringNode) fun.ResolvedArgs[0]).Value);
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception("RUNTIME ERROR! \nTiffReader error: " + ex.Message + " on line " + fun.Line);
                                }

                            }
                            throw new Exception("RUNTIME ERROR! \n TiffReader has no constructor that takes type: "
                                                     + fun.ResolvedArgs[0].ActualType + " as a parameter. Line " + fun.Line);
                        case "ShapeFileReader":
                            if (fun.ResolvedArgs[0].ActualType == ASTNode.Type.String)
                            {
                                try
                                {
                                    return new ShapeFileReaderNode(((StringNode) fun.ResolvedArgs[0]).Value);
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception("RUNTIME ERROR! \nShapeFileReader error: " + ex.Message + " on line " + fun.Line);
                                }
                            }
                            throw new Exception("RUNTIME ERROR! \n ShapeFileReader has no constructor that takes type: "
                                                     + fun.ResolvedArgs[0].ActualType + " as a parameter. Line " + fun.Line);
                        default:
                            throw new Exception("RUNTIME ERROR! \n Unknown error in construct");

                    }
                default:
                    throw new Exception("RUNTIME ERROR! \n" + fun.FunName + " has no constructor that takes " + fun.ResolvedArgs.Count + " arguments!");

            }
        }
    }
}
