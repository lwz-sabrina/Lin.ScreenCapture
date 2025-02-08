using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using Lin.ScreenCapture;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using SkiaSharp;
using X11;

//Desktop desktop = new Desktop();
//Console.WriteLine($"Width:{desktop.Width}");
//Console.WriteLine($"Height:{desktop.Height}");
//using var bitmap = desktop.GetSKBitmap();
//using SKImage skImage = SKImage.FromBitmap(bitmap);
//using SKData data = skImage.Encode(SKEncodedImageFormat.Png, 100);
//using var stream = System.IO.File.OpenWrite("screenshot.png");
//data.SaveTo(stream);
//Console.WriteLine("截图已保存为 screenshot.png");

internal class Program
{
    private static void Main(string[] args)
    {
        //Desktop desktop = new Desktop();
        //Console.WriteLine($"Width:{desktop.Width}");
        //Console.WriteLine($"Height:{desktop.Height}");
        //using var bitmap = desktop.GetSKBitmap();
        //using SKImage skImage = SKImage.FromBitmap(bitmap);
        ////using SKData data = skImage.Encode(SKEncodedImageFormat.Png, 100);
        ////using var stream = System.IO.File.OpenWrite("screenshot.png");
        ////data.SaveTo(stream);
        //var info = bitmap.Info;
        //using var tempBitmap = new Bitmap(
        //    info.Width,
        //    info.Height,
        //    info.RowBytes,
        //    PixelFormat.Format32bppPArgb,
        //    bitmap.GetPixels()
        //);
        //tempBitmap.Save("screenshot.png");
        //Console.WriteLine("截图已保存为 screenshot.png");

        //DesktopVideo desktopVideo = new DesktopVideo();
        //int frameRate = 60;
        //string outputFilePath = "screencapture.mp4";
        //// 初始化视频写入器
        //var fourcc = VideoWriter.FourCC('M', 'P', 'G', '4');
        //using var videoWriter = new VideoWriter(
        //    outputFilePath,
        //    fourcc,
        //    frameRate,
        //    new OpenCvSharp.Size(1920, 1080)
        //);
        //int index = 0;
        //int time = 0;
        //desktopVideo.OnFarme += bitmap =>
        //{
        //    var info = bitmap.Info;
        //    using var tempBitmap = new Bitmap(
        //        info.Width,
        //        info.Height,
        //        info.RowBytes,
        //        PixelFormat.Format32bppPArgb,
        //        bitmap.GetPixels()
        //    );
        //    var mat = tempBitmap.ToMat();
        //    videoWriter.Write(mat);
        //    index++;
        //    if (index == frameRate)
        //    {
        //        index = 0;
        //        time++;
        //        Console.WriteLine(time);
        //    }
        //};
        //desktopVideo.Start();
        //Console.WriteLine("按下回车键停止录制...");
        //Console.ReadLine();
        //desktopVideo.Stop();
        //desktopVideo.Dispose();
        //videoWriter.Release();
        //Console.WriteLine($"视频已保存为 {outputFilePath}");

        DesktopVideo desktopVideo = new DesktopVideo();
        int frameRate = 30;
        int index = 0;
        int time = 0;
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        desktopVideo.OnFarme += bitmap =>
        {
            index++;
            if (index == frameRate)
            {
                index = 0;
                time++;
                stopwatch.Stop();
                Console.WriteLine($"耗时 {stopwatch.ElapsedMilliseconds}");
                stopwatch.Restart();
            }
        };
        desktopVideo.Start(frameRate);
        Console.ReadLine();
        desktopVideo.Stop();
        desktopVideo.Dispose();
    }
}