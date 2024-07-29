using Microsoft.Xna.Framework.Graphics;


namespace Xnb.Exporter
{
    public class GraphicsDeviceService : IGraphicsDeviceService
    {
        static GraphicsDeviceService singletonInstance;
        static int referenceCount;

        public GraphicsDevice GraphicsDevice { get; private set; }

        public event EventHandler<EventArgs> DeviceCreated;
        public event EventHandler<EventArgs> DeviceDisposing;
        public event EventHandler<EventArgs> DeviceReset;
        public event EventHandler<EventArgs> DeviceResetting;

        /**
        * @brief Initializes a new instance of the `GraphicsDeviceService` class.
        *
        * @param windowHandle The handle of the window associated with the `GraphicsDevice`.
        * @param width The width of the back buffer.
        * @param height The height of the back buffer.
        */
        public GraphicsDeviceService(IntPtr windowHandle, int width, int height)
        {
            var parameters = new PresentationParameters
            {
                BackBufferWidth = Math.Max(width, 1),
                BackBufferHeight = Math.Max(height, 1),
                BackBufferFormat = SurfaceFormat.Color,
                DepthStencilFormat = DepthFormat.Depth24,
                DeviceWindowHandle = windowHandle,
                PresentationInterval = PresentInterval.Immediate,
                IsFullScreen = false
            };

            GraphicsDevice = new GraphicsDevice(GraphicsAdapter.DefaultAdapter, GraphicsProfile.Reach, parameters);
        }

        /**
        * @brief Adds a reference to the singleton instance of the `GraphicsDeviceService`.
        *
        * If the singleton instance does not exist, it is created with the specified window handle,
        * width, and height. The reference count is incremented.
        *
        * @param windowHandle The handle of the window associated with the `GraphicsDevice`.
        * @param width The width of the back buffer.
        * @param height The height of the back buffer.
        * @return The singleton instance of the `GraphicsDeviceService`.
        */
        public static GraphicsDeviceService AddRef(IntPtr windowHandle, int width, int height)
        {
            if (referenceCount == 0)
            {
                singletonInstance = new GraphicsDeviceService(windowHandle, width, height);
            }

            referenceCount++;

            return singletonInstance;
        }

        /**
        * @brief Releases a reference to the singleton instance of the `GraphicsDeviceService`.
        *
        * The reference count is decremented, and if it reaches zero, the `GraphicsDevice` is disposed
        * if disposing is true. The `GraphicsDevice` property is set to null.
        *
        * @param disposing Indicates whether to dispose the `GraphicsDevice`.
        */
        public void Release(bool disposing)
        {
            referenceCount--;

            if (referenceCount == 0)
            {
                if (disposing)
                {
                    if (DeviceDisposing != null)
                        DeviceDisposing(this, EventArgs.Empty);

                    GraphicsDevice.Dispose();
                }

                GraphicsDevice = null;
            }
        }
    }
}
