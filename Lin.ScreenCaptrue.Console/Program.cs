using System.Buffers.Binary;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Lin.ScreenCaptrue.Console;
using Lin.ScreenCapture;
using Sdcb.FFmpeg.Codecs;
using Sdcb.FFmpeg.Formats;
using Sdcb.FFmpeg.Raw;
using Sdcb.FFmpeg.Toolboxs.Extensions;
using Sdcb.FFmpeg.Utils;

#pragma warning disable CA1416 // Validate platform compatibility
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
        //StartService(CancellationToken.None);
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

        //int frameRate = 30;
        //string outputFilePath = Path.Combine(Directory.GetCurrentDirectory(), "screencapture.mp4");
        //// 初始化视频写入器
        //DesktopVideo desktopVideo = new DesktopVideo();
        //desktopVideo.ScaleSize(0.5);
        //using var videoWriter = new VideoWriter(
        //    outputFilePath,
        //    FourCC.Prompt,
        //    frameRate,
        //    new OpenCvSharp.Size(desktopVideo.Width, desktopVideo.Height)
        //);
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
        //};
        //desktopVideo.Start(frameRate);
        //Console.WriteLine("按下回车键停止录制...");
        //Console.ReadLine();
        //videoWriter.Release();
        //desktopVideo.Stop();
        //desktopVideo.Dispose();
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
        Console.ReadLine();
    }

    static void StartService(CancellationToken cancellationToken = default)
    {
        var tcpListener = new TcpListener(IPAddress.Any, 5555);
        cancellationToken.Register(() => tcpListener.Stop());
        tcpListener.Start();
        while (!cancellationToken.IsCancellationRequested)
        {
            TcpClient client = tcpListener.AcceptTcpClient();
            Task.Run(() => ServeClient(client, cancellationToken));
        }
    }

    static void ServeClient(TcpClient tcpClient, CancellationToken cancellationToken = default)
    {
        try
        {
            using var _ = tcpClient;
            using NetworkStream stream = tcpClient.GetStream();
            using DesktopVideo desktopVideo = new DesktopVideo();
            using BinaryWriter writer = new(stream);
            RdpCodecParameter rcp =
                new(AVCodecID.H264, desktopVideo.Width, desktopVideo.Height, AVPixelFormat.Bgr0);

            using CodecContext cc =
                new(Codec.CommonEncoders.Libx264RGB)
                {
                    Width = rcp.Width,
                    Height = rcp.Height,
                    PixelFormat = rcp.PixelFormat,
                    TimeBase = new AVRational(1, 20),
                };
            cc.Open(
                null,
                new MediaDictionary
                {
                    ["crf"] = "30",
                    ["tune"] = "zerolatency",
                    ["preset"] = "veryfast"
                }
            );

            writer.Write(rcp.ToArray());
            using Frame source = new();
            foreach (
                Packet packet in desktopVideo
                    .GetBitmaps(30)
                    .ToBgraFrame()
                    .ConvertFrames(cc)
                    .EncodeFrames(cc)
            )
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                //writer.Write(packet.Data.Length);
                writer.Write(packet.Data.AsSpan());
            }
        }
        catch (IOException ex)
        {
            // Unable to write data to the transport connection: 远程主机强迫关闭了一个现有的连接。.
            // Unable to write data to the transport connection: 你的主机中的软件中止了一个已建立的连接。
            Console.WriteLine(ex.ToString());
        }
    }

    record RdpCodecParameter(AVCodecID CodecId, int Width, int Height, AVPixelFormat PixelFormat)
    {
        public byte[] ToArray()
        {
            byte[] data = new byte[16];
            Span<byte> span = data.AsSpan();
            BinaryPrimitives.WriteInt32LittleEndian(span, (int)CodecId);
            BinaryPrimitives.WriteInt32LittleEndian(span[4..], Width);
            BinaryPrimitives.WriteInt32LittleEndian(span[8..], Height);
            BinaryPrimitives.WriteInt32LittleEndian(span[12..], (int)PixelFormat);
            return data;
        }
    }
}


//using System.Drawing;
//using Sdcb.FFmpeg.Raw;
//using Sdcb.FFmpeg.Toolboxs.Extensions;
//using Sdcb.FFmpeg.Toolboxs.Generators;
//using Sdcb.FFmpeg.Utils;

//using Frame frame = Frame.CreateVideo(800, 600, AVPixelFormat.Yuv420p);
//VideoFrameGenerator.FillYuv420p(frame, 0);
//byte[] pngData = frame.EncodeToBytes(formatName: "apng");
//using (MemoryStream memoryStream = new MemoryStream(pngData))
//using (Bitmap bitmap = new Bitmap(memoryStream))
//{
//    bitmap.Save("screenshot.png");
//}
//Console.WriteLine(111);
