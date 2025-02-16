using Sdcb.FFmpeg.Raw;
using Sdcb.FFmpeg.Utils;
using SkiaSharp;

namespace Lin.ScreenCaptrue.Console
{
    internal static class BgraFrameExtensions
    {
        public static IEnumerable<Frame> ToBgraFrame(this IEnumerable<SKBitmap> bgras)
        {
            using Frame frame = new Frame();
            foreach (SKBitmap bgra in bgras)
            {
                frame.Width = bgra.Width;
                frame.Height = bgra.Height;
                frame.Format = (int)AVPixelFormat.Bgr0;
                frame.Data[0] = bgra.GetPixels();
                frame.Linesize[0] = bgra.RowBytes;
                yield return frame;
            }
        }
    }
}
