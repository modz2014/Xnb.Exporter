using Microsoft.Xna.Framework.Graphics;
using Xnb.Exporter;


namespace WindowsApplication1
{
    public class Extract
    {
        private string outPath;
        private Exporter _exporter;
        public Extract(Exporter exporter)
        {
            _exporter = exporter;
        }

        public void ExtractTexture(Texture2D texture, string fileName)
        {
            // Convert the texture to a supported format if necessary
            texture = _exporter.ConvertToSupportedFormat(texture);

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG Image|*.png";
            saveFileDialog.Title = "Save an Image File";
            saveFileDialog.FileName = fileName;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (var outStream = (FileStream)saveFileDialog.OpenFile())
                {
                    texture.SaveAsPng(outStream, texture.Width, texture.Height);
                }


            }
        }

        public void ExtractModelData(Model model, int meshIndex, string fileName)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "OBJ File|*.obj";
            saveFileDialog.Title = "Save an OBJ File";
            saveFileDialog.FileName = fileName;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Create the file name for the MTL file
                string mtlFileName = Path.ChangeExtension(saveFileDialog.FileName, ".mtl");
                string textureFileName = Path.ChangeExtension(saveFileDialog.FileName, ".png");

                using (StreamWriter writer = new StreamWriter(saveFileDialog.OpenFile()))
                {
                    // Write the reference to the material file
                    writer.WriteLine($"mtllib {Path.GetFileName(mtlFileName)}");

                    // Get the specific mesh
                    ModelMesh mesh = model.Meshes[meshIndex];

                    int vertexOffset = 1; // OBJ format uses 1-based indexing for vertices

                    // Loop through each mesh part
                    foreach (ModelMeshPart meshPart in mesh.MeshParts)
                    {
                        // Extract vertices
                        VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[meshPart.NumVertices];
                        meshPart.VertexBuffer.GetData(vertices);

                        // Extract indices
                        short[] indices = new short[meshPart.PrimitiveCount * 3]; // Assuming triangles and using short for indices
                        meshPart.IndexBuffer.GetData(indices);

                        // Write vertex positions, normals, and texture coordinates
                        foreach (var vertex in vertices)
                        {
                            writer.WriteLine($"v {vertex.Position.X} {vertex.Position.Y} {vertex.Position.Z}");
                            writer.WriteLine($"vn {vertex.Normal.X} {vertex.Normal.Y} {vertex.Normal.Z}");
                            writer.WriteLine($"vt {vertex.TextureCoordinate.X} {vertex.TextureCoordinate.Y}");
                        }

                        // Write faces (triangles) with material reference
                        writer.WriteLine($"usemtl Material_{meshIndex}");
                        for (int i = 0; i < indices.Length; i += 3)
                        {
                            // OBJ file format faces are 1-based indices
                            int index1 = indices[i] + vertexOffset;
                            int index2 = indices[i + 1] + vertexOffset;
                            int index3 = indices[i + 2] + vertexOffset;
                            writer.WriteLine($"f {index1}/{index1}/{index1} {index2}/{index2}/{index2} {index3}/{index3}/{index3}");
                        }

                        vertexOffset += vertices.Length;
                    }
                }

                // Create the MTL file
                using (StreamWriter mtlWriter = new StreamWriter(mtlFileName))
                {
                    mtlWriter.WriteLine($"newmtl Material_{meshIndex}");
                    mtlWriter.WriteLine($"Ka 1.000 1.000 1.000");
                    mtlWriter.WriteLine($"Kd 1.000 1.000 1.000");
                    mtlWriter.WriteLine($"Ks 0.000 0.000 0.000");
                    mtlWriter.WriteLine($"d 1.0");
                    mtlWriter.WriteLine($"illum 2");
                    mtlWriter.WriteLine($"map_Kd {Path.GetFileName(textureFileName)}");
                }

                // Save the texture
                SaveTexture(model, meshIndex, textureFileName);
            }
        }
        private void SaveTexture(Model model, int meshIndex, string textureFileName)
        {
            ModelMesh mesh = model.Meshes[meshIndex];
            foreach (BasicEffect effect in mesh.Effects)
            {
                if (effect.Texture != null)
                {
                    Texture2D texture = effect.Texture;

                    // Convert the texture to a supported format
                    RenderTarget2D renderTarget = new RenderTarget2D(texture.GraphicsDevice, texture.Width, texture.Height, false, SurfaceFormat.Color, DepthFormat.None);
                    texture.GraphicsDevice.SetRenderTarget(renderTarget);
                    texture.GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Transparent);

                    // Draw the original texture onto the new render target
                    SpriteBatch spriteBatch = new SpriteBatch(texture.GraphicsDevice);
                    spriteBatch.Begin();
                    spriteBatch.Draw(texture, new Microsoft.Xna.Framework.Rectangle(0, 0, texture.Width, texture.Height), Microsoft.Xna.Framework.Color.White);
                    spriteBatch.End();

                    texture.GraphicsDevice.SetRenderTarget(null);

                    // Save the render target as a PNG
                    using (Stream stream = File.Create(textureFileName))
                    {
                        renderTarget.SaveAsPng(stream, renderTarget.Width, renderTarget.Height);
                    }

                    renderTarget.Dispose();
                    spriteBatch.Dispose();
                }
            }
        }
        public void ExtractFontAtlas(SpriteFont font, string fileName)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG Image|*.png";
            saveFileDialog.Title = "Save an Image File";
            saveFileDialog.FileName = fileName;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (var outStream = (FileStream)saveFileDialog.OpenFile())
                {
                    font.Texture.SaveAsPng(outStream, font.Texture.Width, font.Texture.Height);
                }


            }
        }


    }
}
