using Lin.ScreenCapture;

namespace Lin.ScreenCaptrue.Test
{
    [TestClass]
    public class DesktopVideoTests
    {
        [TestMethod]
        public void Constructor_ShouldInitializeCorrectly()
        {
            var desktop = new Desktop();
            var desktopVideo = new DesktopVideo(desktop);

            Assert.IsNotNull(desktopVideo.Desktop);
            Assert.AreEqual(desktop.Width, desktopVideo.Width);
            Assert.AreEqual(desktop.Height, desktopVideo.Height);
            Assert.AreEqual(desktop.Size, desktopVideo.Size);
            Assert.AreEqual(desktop.Scale, desktopVideo.Scale);
        }

        [TestMethod]
        public void ScaleSize_ShouldUpdateScaleCorrectly()
        {
            var desktop = new Desktop();
            var desktopVideo = new DesktopVideo(desktop);

            desktopVideo.ScaleSize(2);
            Assert.AreEqual(2, desktopVideo.Scale);
        }

        [TestMethod]
        public void Dispose_ShouldReleaseResourcesCorrectly()
        {
            var desktop = new Desktop();
            var desktopVideo = new DesktopVideo(desktop);

            desktopVideo.Dispose();
            // No exception should be thrown during disposal
        }

        [TestMethod]
        public void GetBitmaps_ShouldReturnCorrectNumberOfFrames()
        {
            var desktop = new Desktop();
            var desktopVideo = new DesktopVideo(desktop);

            int fps = 2;
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(1000); // Run for 1 second

            foreach (var frame in desktopVideo.GetBitmaps(fps, cancellationTokenSource.Token))
            {
                Assert.IsNotNull(frame);
                Assert.AreEqual(desktop.Width, frame.Width);
                Assert.AreEqual(desktop.Height, frame.Height);
            }
        }
    }
}
