using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;



namespace Xnb.Exporter
{
    public class Exporter
    {

        public delegate void Completed();
        public event Completed OnCompleted;

        public ContentManager contentManager;
        public GraphicsDevice graphicsDevice;

        private readonly PictureBox pictureBox;
        private readonly string[] files;
        public readonly string outPath;
        private byte[] buffer;

        public Texture2D CurrentTexture { get; set; }
        public SpriteFont CurrentFont { get; set; }
        public Model CurrentModel { get; set; }
        public AnimationClip CurrentAnimation { get; set; }
        public int CurrentFrame { get; set; }
        public string CurrentModelName { get; set; }
        public string ContentRootDirectory { get; private set; }

        /**
        * @brief Constructor initializes the Exporter with given files, PictureBox, and output path.
        *
        * @param files The array of file paths to be exported.
        * @param pictureBox The PictureBox control to display images.
        * @param outPath The output path for exporting files. Defaults to an empty string.
        */
        public Exporter(string[] files, PictureBox pictureBox, string outPath = "")
        {
            this.files = files;
            this.pictureBox = pictureBox;
            this.outPath = outPath;
            InitializeContentManager();
        }


        /**
        * @brief Sets the content root directory for the ContentManager.
        *
        * @param directory The directory path to set as the content root.
        */
        public void SetContentRootDirectory(string directory)
        {
            try
            {
                ContentRootDirectory = directory;
                contentManager.RootDirectory = directory;
                Debug.LogMessage($"Content root directory set to: {directory}");
            }
            catch (Exception ex)
            {
                Debug.LogException($"Error setting content root directory: {ex.Message}");
                MessageBox.Show($"An error occurred while setting content root directory: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /**
        * @brief Initializes the ContentManager and GraphicsDevice.
        */
        public void InitializeContentManager()
        {
            var services = new GameServiceContainer();
            var graphicsDeviceService = GraphicsDeviceService.AddRef(IntPtr.Zero, 800, 600);
            services.AddService<IGraphicsDeviceService>(graphicsDeviceService);

            this.graphicsDevice = graphicsDeviceService.GraphicsDevice;



            string contentRootDirectory = Path.Combine(Application.StartupPath, "content");
            if (!Directory.Exists(contentRootDirectory))
            {
                Directory.CreateDirectory(contentRootDirectory);
            }
            contentManager = new ContentManager(services, contentRootDirectory);
        }

        /**
        * @brief Loads the XNB files and returns a list of XnbItems.
        *
        * @return A list of XnbItems representing the loaded files.
        */
        public List<XnbItem> Load()
        {
            List<XnbItem> items = new List<XnbItem>();

            try
            {
                foreach (var file in files)
                {
                    try
                    {
                        // Get the relative path from the ContentRootDirectory to the file
                        string relativePath = Path.GetRelativePath(ContentRootDirectory, file);

                        // Remove the file extension
                        string itemName = Path.ChangeExtension(relativePath, null);

                        // Create a new XnbItem and add it to the list
                        items.Add(new XnbItem { Name = itemName, Path = file });

                        Debug.LogMessage($"Added item: {itemName}");
                    }
                    catch (ArgumentException ex)
                    {
                        Debug.LogException($"ArgumentException while processing file '{file}': {ex.Message}");
                        // Optionally, display or log more specific details about the ArgumentException
                    }
                    catch (Exception ex)
                    {
                        Debug.LogException($"Error processing file '{file}': {ex.Message}");
                        // Optionally, handle or log other types of exceptions specific to file processing
                    }
                }

                OnCompleted?.Invoke();
            }
            catch (IOException ex)
            {
                Debug.LogException($"IOException during file loading: {ex.Message}");
                // Handle the exception as needed
            }
            catch (UnauthorizedAccessException ex)
            {
                Debug.LogException($"UnauthorizedAccessException during file loading: {ex.Message}");
                // Handle the exception as needed
            }
            catch (Exception ex)
            {
                Debug.LogException($"Unexpected error during file loading: {ex.Message}");
                // Handle the exception as needed
            }

            return items;
        }

        /**
        * @brief Displays a skinned model in the PictureBox.
        *
        * @param model The Model object to be displayed.
        * @param modelName The name of the model.
        */
        public void DisplaySkinnedModel(Model model, string modelName)
        {
            // Assuming you have a valid GraphicsDevice instance available
            if (graphicsDevice == null)
            {
                // Handle error or return
                return;
            }

            try
            {
                // Set any necessary graphics device states
                graphicsDevice.BlendState = BlendState.Opaque;
                graphicsDevice.DepthStencilState = DepthStencilState.Default;

                // Iterate over each ModelMesh in the Model
                foreach (ModelMesh mesh in model.Meshes)
                {
                    // Iterate over each ModelMeshPart in the ModelMesh
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        // Set the effect used for rendering (if needed)
                        SharpDX.Direct3D9.Effect effect = null;


                        mesh.Draw();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogException($"Failed to display skinned model '{modelName}': {ex.Message}");
                MessageBox.Show($"Failed to display skinned model '{modelName}': {ex.Message}", "Rendering Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /**
        * @brief Displays the font atlas in the PictureBox.
        *
        * @param font The SpriteFont object containing the font atlas.
        * @param fileName The file name of the font.
        */
        public void DisplayFontAtlas(SpriteFont font, string fileName)
        {
            //Displaying the texture of the font
            using (var stream = new MemoryStream())
            {
                font.Texture.SaveAsPng(stream, font.Texture.Width, font.Texture.Height);
                stream.Seek(0, SeekOrigin.Begin);
                pictureBox.Image = System.Drawing.Image.FromStream(stream);
            }

        }

        /**
        * @brief Displays an image in the PictureBox.
        *
        * @param texture The Texture2D object to be displayed.
        */
        public void DisplayImage(Texture2D texture)
        {
            try
            {
                texture = ConvertToSupportedFormat(texture);

                // Retrieve texture data into an array of Color
                Microsoft.Xna.Framework.Color[] textureData = new Microsoft.Xna.Framework.Color[texture.Width * texture.Height];
                texture.GetData(0, null, textureData, 0, texture.Width * texture.Height);

                // Save the texture as PNG to a MemoryStream
                using (var stream = new MemoryStream())
                {
                    // Assuming texture is already converted to supported format
                    texture.SaveAsPng(stream, texture.Width, texture.Height);
                    stream.Seek(0, SeekOrigin.Begin);

                    // Display the image in pictureBox
                    pictureBox.Image = System.Drawing.Image.FromStream(stream);
                }

            }
            catch (OutOfMemoryException ex)
            {
                Debug.LogException(ex.Message); // Log the exception message
            }
            catch (Exception ex)
            {
                Debug.LogException(ex.Message); // Log the exception message
            }
        }

        /**
        * @brief Applies an animation to the current model.
        *
        * @param animation The AnimationClip object containing the animation data.
        */
        public void ApplyAnimation(AnimationClip animation)
        {
            // Check if there is a current model to apply the animation to
            if (CurrentModel == null)
            {
                Debug.LogMessage("No current model to apply animation to.");
                return;
            }

            // Check if the animation data is valid
            if (animation == null || animation.FrameCount == 0)
            {
                Debug.LogMessage("Invalid animation data.");
                return;
            }

            // Apply the animation to each bone in the model
            for (int i = 0; i < CurrentModel.Bones.Count; i++)
            {
                // Get the transform from the animation data for this bone
                Matrix transform = animation.GetTransform(i, CurrentFrame);

                // Apply the transform to the bone
                CurrentModel.Bones[i].Transform = transform;
            }

            // Update the current frame (this could be more complex depending on your needs)
            CurrentFrame = (CurrentFrame + 1) % animation.FrameCount;
        }

        /**
        * @brief Converts a texture to a supported format if needed.
        *
        * @param texture The Texture2D object to be converted.
        * @return The converted Texture2D object.
        */
        public Texture2D ConvertToSupportedFormat(Texture2D texture)
        {
            // Check if the format is DXT and decompress if needed
            if (IsDXTFormat(texture.Format))
            {
                var data = DecompressDXT(texture);
                var convertedTexture = new Texture2D(texture.GraphicsDevice, texture.Width, texture.Height, false, SurfaceFormat.Color);
                convertedTexture.SetData(data);
                return convertedTexture;
            }

            return texture;
        }

        /**
        * @brief Checks if a texture format is a DXT format.
        *
        * @param format The SurfaceFormat to be checked.
        * @return True if the format is DXT, false otherwise.
        */
        public bool IsDXTFormat(SurfaceFormat format)
        {
            return format == SurfaceFormat.Dxt1 || format == SurfaceFormat.Dxt3 || format == SurfaceFormat.Dxt5;
        }

        /**
        * @brief Decompresses a DXT texture to an array of colors.
        *
        * @param texture The Texture2D object to be decompressed.
        * @return An array of decompressed colors.
        */
        public Microsoft.Xna.Framework.Color[] DecompressDXT(Texture2D texture)
        {
            int width = texture.Width;
            int height = texture.Height;
            byte[] compressedData;

            switch (texture.Format)
            {
                case SurfaceFormat.Color:
                    Microsoft.Xna.Framework.Color[] colorData = new Microsoft.Xna.Framework.Color[width * height];
                    texture.GetData(colorData);
                    return colorData;

                case SurfaceFormat.Dxt1:
                    compressedData = new byte[width * height / 2]; // 8:1 compression ratio
                    texture.GetData(compressedData);
                    byte[] dxt1Data = DxtUtil.DecompressDxt1(compressedData, width, height);
                    return ConvertToColorArray(dxt1Data, width, height);

                case SurfaceFormat.Dxt3:
                    compressedData = new byte[width * height]; // 4:1 compression ratio
                    texture.GetData(compressedData);
                    byte[] dxt3Data = DxtUtil.DecompressDxt3(compressedData, width, height);
                    return ConvertToColorArray(dxt3Data, width, height);

                case SurfaceFormat.Dxt5:
                    compressedData = new byte[width * height]; // 4:1 compression ratio
                    texture.GetData(compressedData);
                    byte[] dxt5Data = DxtUtil.DecompressDxt5(compressedData, width, height);
                    return ConvertToColorArray(dxt5Data, width, height);

                default:
                    throw new NotSupportedException("Unsupported texture format");
            }
        }

        /**
        * @brief Converts a byte array to an array of colors.
        *
        * @param byteArray The byte array to be converted.
        * @param width The width of the texture.
        * @param height The height of the texture.
        * @return An array of colors.
        */
        private Microsoft.Xna.Framework.Color[] ConvertToColorArray(byte[] data, int width, int height)
        {
            var colorData = new Microsoft.Xna.Framework.Color[width * height];
            for (int i = 0; i < colorData.Length; i++)
            {
                colorData[i] = new Microsoft.Xna.Framework.Color(data[i * 4], data[i * 4 + 1], data[i * 4 + 2], data[i * 4 + 3]);
            }
            return colorData;
        }

        /**
        * @brief Generates a preview of a model as a Bitmap.
        *
        * @param model The model to generate the preview for.
        * @param graphicsDevice The graphics device used for rendering.
        * @param meshIndex The index of the mesh to render.
        * @return The generated Bitmap preview of the model.
        */
        public Bitmap GenerateModelPreview(Model model, GraphicsDevice graphicsDevice, int meshIndex)
        {
            if (graphicsDevice == null)
            {
                throw new Exception("GraphicsDevice is null");
            }

            if (model == null)
            {
                throw new Exception("Model is null");
            }

            // Create a new RenderTarget2D to render the model to
            RenderTarget2D renderTarget = new RenderTarget2D(graphicsDevice, pictureBox.Width, pictureBox.Height);
            graphicsDevice.SetRenderTarget(renderTarget);

            // Clear the render target
            graphicsDevice.Clear(Microsoft.Xna.Framework.Color.CornflowerBlue);

            // Create a view and projection matrix
            Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, -5), Vector3.Zero, Vector3.Up);
            Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), pictureBox.Width / (float)pictureBox.Height, 0.1f, 100f);

            // Get the specific mesh
            ModelMesh mesh = model.Meshes[meshIndex];

            // Check that the mesh contains at least one vertex and one triangle
            bool hasValidMeshPart = false;
            foreach (var part in mesh.MeshParts)
            {
                if (part.VertexBuffer.VertexCount >= 1 && part.IndexBuffer.IndexCount >= 3)
                {
                    hasValidMeshPart = true;
                    break;
                }
            }
            if (!hasValidMeshPart)
            {
                throw new Exception("The mesh does not contain enough data to form at least one triangle.");
            }

            // Draw the specific mesh
            foreach (BasicEffect effect in mesh.Effects)
            {
                effect.View = view;
                effect.Projection = projection;
                effect.World = Matrix.Identity;
                effect.EnableDefaultLighting();
            }
            mesh.Draw();

            // Reset the render target
            graphicsDevice.SetRenderTarget(null);

            // Get the data from the render target
            Microsoft.Xna.Framework.Color[] data = new Microsoft.Xna.Framework.Color[pictureBox.Width * pictureBox.Height];
            renderTarget.GetData(data);

            // Create a new Bitmap and set its pixels to the data from the render target
            Bitmap bitmap = new Bitmap(pictureBox.Width, pictureBox.Height);
            for (int x = 0; x < pictureBox.Width; x++)
            {
                for (int y = 0; y < pictureBox.Height; y++)
                {
                    Microsoft.Xna.Framework.Color color = data[y * pictureBox.Width + x];
                    bitmap.SetPixel(x, y, System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B));
                }
            }

            // Dispose of the render target to free resources
            renderTarget.Dispose();

            // Return the Bitmap
            return bitmap;
        }

    }

    public static class Extensions
    {
        /**
        * @brief Reads a specified number of bytes from a FileStream starting at a given index.
        *
        * This extension method allows for reading a sequence of bytes from a FileStream, beginning at the specified index.
        * If the length is not provided or is set to 0, the method will read all bytes from the specified index to the end of the stream.
        *
        * @param stream The FileStream from which bytes will be read.
        * @param index The starting position in the stream from which to begin reading bytes.
        * @param length The number of bytes to read. If set to 0, reads from the index to the end of the stream.
        * @return A byte array containing the bytes read from the stream.
        */
        public static byte[] ReadBytes(this FileStream stream, int index, int length = 0)
        {
            stream.Seek(index, SeekOrigin.Begin);
            var bytes = new byte[length == 0 ? stream.Length - index : Math.Min(stream.Length - index, length)];
            for (var i = 0; i < bytes.Length; i++)
                bytes[i] = (byte)stream.ReadByte();
            return bytes;
        }
    }
}
