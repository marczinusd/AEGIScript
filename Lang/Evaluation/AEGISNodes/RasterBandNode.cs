using ELTE.AEGIS.Core.Geometry.Raster;

namespace AEGIScript.Lang.Evaluation.AEGISNodes
{
    internal class RasterBandNode : SurfaceNode
    {
        public RasterBandNode(RasterBand rband)
            : base(rband)
        {
            ActualType = Type.RasterBand;
        }
    }
}