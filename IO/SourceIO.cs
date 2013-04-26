using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace AEGIScript.IO
{
    public class ScriptIOException : Exception
    {
        public ScriptIOException(string path)
        {
            ThrownOnPath = path;
        }
        public override string Message
        {
            get
            {
                return base.Message + " \n File doesn't exist at : " + ThrownOnPath;
            }
        }
        public String ThrownOnPath { get; private set; }
    }


    public static class SourceIO
    {

        public static List<String> GetFileContent(string path)
        {
            if (isValidPath(path))
            {
                return ReadFromFile(path);
            }
            else
            {
                throw new ScriptIOException(path);
            }

        }


        public static void SaveToFile(String Text, String Path)
        
        {
            /*
            if (File.Exists(Path))
            {
                File.Delete(Path);
            }
            File.Create(Path);
            */
            File.WriteAllText(Path, Text);
            //using (FileStream stream = File.Open(Path, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
            //{
            //    using (StreamWriter writer = new StreamWriter(stream))
            //    {
            //        writer.Write(Text);
            //    }
            //}
        }

        /// <summary>
        /// Reads file line-by-line
        /// </summary>
        /// <param name="Path">Need valis file path</param>
        /// <returns>List of the lines</returns>
        public static List<String> ReadFromFile(string Path)
        {
            List<String> lines = new List<string>();

            using (FileStream stream = File.Open(Path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    while (!reader.EndOfStream)
                    {
                        lines.Add(reader.ReadLine());
                    }
                    reader.Close();
                }
                stream.Close();
            }

            return lines;
        }

        public static String ReadWKT(string path)
        {
            StringBuilder builder = new StringBuilder();
            using (FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    while (!reader.EndOfStream)
                    {
                        builder.AppendLine(reader.ReadLine());
                    }
                    reader.Close();
                }
                stream.Close();
            }
            return builder.ToString();
        }

        public static bool isValidPath(string path)
        {
            return path.Contains(".aes") && File.Exists(path);
        }
    }
}
