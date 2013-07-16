using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace AEGIScript.IO
{
    public static class SourceIO
    {

        public static List<String> GetFileContent(string path)
        {
            if (IsValidPath(path))
            {
                return ReadFromFile(path);
            }
            throw new ScriptIOException(path);
        }


        public static void SaveToFile(String text, String path) 
        {
            File.WriteAllText(path, text);
        }

        /// <summary>
        /// Reads file line-by-line
        /// </summary>
        /// <param name="path">Need valis file path</param>
        /// <returns>List of the lines</returns>
        public static List<String> ReadFromFile(string path)
        {
            var lines = new List<string>();

            using (var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var reader = new StreamReader(stream))
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

        // 1 sor beolvasása
        public static String ReadWKT(string path)
        {
            var builder = new StringBuilder();
            using (var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var reader = new StreamReader(stream))
                {
                    while (!reader.EndOfStream)
                    {
                        builder.Append(reader.ReadLine());
                    }
                    reader.Close();
                }
                stream.Close();
            }
            return builder.ToString();
        }

        public static bool IsValidPath(string path)
        {
            return path.Contains(".aes") && File.Exists(path);
        }
    }
}
