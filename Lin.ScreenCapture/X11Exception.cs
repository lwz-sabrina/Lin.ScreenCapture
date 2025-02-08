namespace Lin.ScreenCapture
{
    internal class X11Exception : Exception
    {
        public X11Exception(string? message)
            : base(message) { }
    }
}