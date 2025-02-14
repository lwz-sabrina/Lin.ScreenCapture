﻿using System.Drawing;
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
            int width = 0;
            int height = 0;
            using ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                "select * from Win32_VideoController"
            );
            // 执行查询并处理结果
            foreach (ManagementObject videoController in searcher.Get())
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
            this.width = width;
            this.height = height;
        }

        private SKBitmap GetSKBitmap_Windwos()
        {
            var info = new SKImageInfo(width, height);
            var skiaBitmap = new SKBitmap(info);
            using (var pixmap = skiaBitmap.PeekPixels())
            using (
                var tempBitmap = new Bitmap(
                    info.Width,
                    info.Height,
                    info.RowBytes,
                    PixelFormat.Format32bppPArgb,
                    pixmap.GetPixels()
                )
            )
            using (var g = System.Drawing.Graphics.FromImage(tempBitmap))
            {
                g.CopyFromScreen(0, 0, 0, 0, new System.Drawing.Size(width, height));
            }
            return skiaBitmap;
        }
    }
}

#pragma warning restore CA1416
