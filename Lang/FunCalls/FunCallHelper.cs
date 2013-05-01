using System;
using ELTE.AEGIS.IO;

namespace AEGIScript.Lang.FunCalls
{
    /// <summary>
    /// Unimplemented class to help function calls
    /// </summary>
    class FunCallHelper
    {


        public bool Contains(String funSym)
        {
            return false;
        }

        public void ReadGeoTiff()
        {
            Reader = new ShapefileReader("be.sf");
            Reader.Read();
            var treader = new TiffReader("be.tiff");
            treader.Read();
        }

        public ShapefileReader Reader { get; set; }
    }
}
