using AEGIScript.Lang.Evaluation.ExpressionNodes;
using AEGIScript.Lang.Evaluation.PrimitiveNodes;
using ELTE.AEGIS.Core.Geometry.Raster;

namespace AEGIScript.Lang.Evaluation.AEGISNodes
{
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
}