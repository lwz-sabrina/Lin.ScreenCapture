using System.Drawing;
using System.Drawing.Imaging;
using System.Management;
using SkiaSharp;

#pragma warning disable CA1416

namespace Lin.ScreenCapture
{
    partial class Desktop
    {
        private void Init_Windwos()
        {
            using ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                "select * from Win32_VideoController"
            );
            int width = 0;
            int height = 0;
            // 执行查询并处理结果
            foreach (ManagementObject videoController in searcher.Get())
            {
                foreach (var item in videoController.Properties)
                {
                    var availability = videoController.Properties["Availability"].Value;
                    if (availability.ToString() == "3") // 3 表示可用
                    {
                        // 获取分辨率信息
                        var horizontalResolution = videoController
                            .Properties["CurrentHorizontalResolution"]
                            .Value.ToString();
                        var verticalResolution = videoController
                            .Properties["CurrentVerticalResolution"]
                            .Value.ToString();

                        int.TryParse(horizontalResolution, out width);
                        int.TryParse(verticalResolution, out height);
                        break;
                    }
                }
                videoController.Dispose();
            }
            this.Width = width;
            this.Height = height;
        }

        private SKBitmap GetSKBitmap_Windwos()
        {
            var info = new SKImageInfo(Width, Height);
            var skiaBitmap = new SKBitmap(info);
            using var pixmap = skiaBitmap.PeekPixels();
            var tempBitmap = new Bitmap(
                info.Width,
                info.Height,
                info.RowBytes,
                PixelFormat.Format32bppPArgb,
                pixmap.GetPixels()
            );
            using var g = System.Drawing.Graphics.FromImage(tempBitmap);
            g.CopyFromScreen(0, 0, 0, 0, new System.Drawing.Size(Width, Height));
            return skiaBitmap;
        }
    }
}

#pragma warning restore CA1416