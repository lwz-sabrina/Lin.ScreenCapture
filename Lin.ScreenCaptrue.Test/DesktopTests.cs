using Lin.ScreenCapture;

namespace Lin.ScreenCaptrue.Test
{
    [TestClass]
    public sealed class DesktopTests
    {
        [TestMethod]
        public void Constructor_ShouldInitializeCorrectly()
        {
            var desktop = new Desktop();
            Assert.IsTrue(desktop.Width > 0);
            Assert.IsTrue(desktop.Height > 0);
            Assert.AreEqual(1, desktop.Scale);
        }

        [TestMethod]
        public void Dispose_ShouldReleaseResourcesCorrectly()
        {
            var desktop = new Desktop();
            desktop.Dispose();
            // No exception should be thrown during disposal
        }

        [TestMethod]
        public void GetSKBitmap_ShouldReturnResizedBitmap()
        {
            var desktop = new Desktop();
            desktop.ScaleSize(0.5);
            var bitmap = desktop.GetSKBitmap();

            Assert.IsNotNull(bitmap);
            Assert.AreEqual((int)(desktop.Width * 0.5), bitmap.Width);
            Assert.AreEqual((int)(desktop.Height * 0.5), bitmap.Height);
        }

        [TestMethod]
        public void ScaleSize_ShouldUpdateScaleCorrectly()
        {
            var desktop = new Desktop();
            desktop.ScaleSize(2);
            Assert.AreEqual(2, desktop.Scale);
        }
    }
}
