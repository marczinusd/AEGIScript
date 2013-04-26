using System;
using ELTE.AEGIS.IO;
using ELTE.AEGIS.IO.GeoTiff;
using ELTE.AEGIS.IO.Shapefile;

namespace AEGIScript.Lang.FunCalls
{
    /// <summary>
    /// Unimplemented class to help function calls
    /// </summary>
    class FunCallHelper
    {


        public bool Contains(String FunSym)
        {
            return false;
        }

        public void ReadGeoTiff()
        {
            reader = new ShapefileReader("be.sf");
            reader.Read();
            TiffReader treader = new TiffReader("be.tiff");
            treader.Read();
        }

        public ShapefileReader reader { get; set; }
    }
}
