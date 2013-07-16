using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AEGIScript.IO;
using AEGIScript.Lang.Evaluation.AEGISNodes;
using AEGIScript.Lang.Evaluation.ExpressionNodes;
using AEGIScript.Lang.Evaluation.PrimitiveNodes;
using AEGIScript.Lang.Exceptions;
using ELTE.AEGIS.Core;
using ELTE.AEGIS.Core.Geometry;
using ELTE.AEGIS.IO;

namespace AEGIScript.Lang.Evaluation.Helpers
{
    static class Library
    {
        //public static TermNode Call(FunCallNode fun)
        //{

            
        //}

        public static void CallVoid(FunCallNode fun)
        {
            switch (fun.FunName)
            {
                case "WriteToFile":
                    //SaveToFile(fun.ResolvedArgs);
                default:
                    throw ExceptionGenerator.UndefinedFunctionCall(fun);
            }
        }

        private static TermNode ReadFile(StringNode path)
        {
            if (!File.Exists(path.Value))
            {
                return new BooleanNode(false);
            }

            var lines = File.ReadAllLines(path.Value);
            return new ArrayNode(
                new List<TermNode>(
                        lines.Select(s => new StringNode(s)).Cast<TermNode>().ToList()
                    )
                );
        }

        private static void SaveToFile(StringNode path, TermNode toSave)
        {
            File.WriteAllText(path.Value, toSave.ToString());
        }

        private static TermNode Open(StringNode path)
        {
            if (!File.Exists(path.Value))
            {
                
            }
            return new FileNode(File.Open(path.Value, FileMode.Open));
        }

        private static TermNode ReadWKT(StringNode path)
        {
			IReferenceSystem referenceSystem = GeometryFactory.ReferenceSystem;

            //try
            //{
				String read = SourceIO.ReadWKT(path.Value);
				IGeometry res = GeometryConverter.ToGeometry(read, referenceSystem);
				return new GeometryNode(res);
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception("RUNTIME ERROR!\nWell-known text read failed with message: " +
            //                        ex.Message + "\n at line: " + fun.Line);
            //}
        }

        private static ArrayNode Dir()
        {
            List<TermNode> files = Directory
                .GetFiles(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location))
                .Select(s => new StringNode(s))
                .Cast<TermNode>()
                .ToList();
            return new ArrayNode(files);
        }

        private static ArrayNode Dir(StringNode filter)
        {
            List<TermNode> files = Directory
                .GetFiles(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                          filter.Value)
                .Select(s => new StringNode(s))
                .Cast<TermNode>()
                .ToList();
            return new ArrayNode(files);
        }

        private static ArrayNode Dir(StringNode path, StringNode filter)
        {
            var Files = Directory.GetFiles(path.Value, filter.Value);
            List<TermNode> files = Files.Select(s => new StringNode(s)).Cast<TermNode>().ToList();
            return new ArrayNode(files);
        }

        private static IntNode Rand()
        {
            return new IntNode(_rand.Next());
        }

        private static IntNode Rand(IntNode max)
        {
            return new IntNode(_rand.Next(max.Value));
        }

        private static DoubleNode RandDouble()
        {
            return new DoubleNode(_rand.NextDouble());
        }


        static readonly Random _rand = new Random();
    }

    class FileNode : TermNode
    {
        public FileNode(FileStream stream)
        {
            _stream = stream;
            _streamReader = new StreamReader(_stream);
            _eof = _streamReader.EndOfStream;
            ActualType = Type.File;
        }

        public FileNode()
        {
            _eof = true;
            ActualType = Type.File;
        }

        public override TermNode CallFun(FunCallNode func)
        {
            switch (func.FunName)
            {
                case "Read":
                    return Read();
                case "EOF":
                    return End();
                case "ReadToEnd":
                    return ReadToEnd();

                default:
                    return base.CallFun(func);
            }
        }

        public StringNode Read()
        {
            if (_streamReader.EndOfStream)
            {
                throw ExceptionGenerator.EndOfStream();
            }
            return new StringNode(_streamReader.ReadLine());
        }

        public BooleanNode End()
        {
            return new BooleanNode(_streamReader.EndOfStream);
        }

        public void Close()
        {
            _stream.Close();
            _streamReader.Close();
        }

        public ArrayNode ReadToEnd()
        {
            if (_streamReader.EndOfStream)
            {
                throw ExceptionGenerator.EndOfStream();
            }
            return new ArrayNode(_streamReader.ReadToEnd()
                                              .Split(new[]{ '\n' })
                                              .Select(s => new StringNode(s))
                                              .Cast<TermNode>()
                                              .ToList());
        }

        private readonly FileStream _stream;
        private readonly StreamReader _streamReader;
        private bool _eof;

    }


}
