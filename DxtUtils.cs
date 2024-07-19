namespace Xnb.Exporter
{

    internal static class DxtUtil
    {
        internal static byte[] DecompressDxt1(byte[] imageData, int width, int height)
        {
            using MemoryStream imageStream = new MemoryStream(imageData);
            return DecompressDxt1(imageStream, width, height);
        }

        internal static byte[] DecompressDxt1(Stream imageStream, int width, int height)
        {
            byte[] array = new byte[width * height * 4];
            using BinaryReader imageReader = new BinaryReader(imageStream);
            int num = (width + 3) / 4;
            int num2 = (height + 3) / 4;
            for (int i = 0; i < num2; i++)
            {
                for (int j = 0; j < num; j++)
                {
                    DecompressDxt1Block(imageReader, j, i, num, width, height, array);
                }
            }

            return array;
        }

        private static void DecompressDxt1Block(BinaryReader imageReader, int x, int y, int blockCountX, int width, int height, byte[] imageData)
        {
            ushort num = imageReader.ReadUInt16();
            ushort num2 = imageReader.ReadUInt16();
            ConvertRgb565ToRgb888(num, out var r, out var g, out var b);
            ConvertRgb565ToRgb888(num2, out var r2, out var g2, out var b2);
            uint num3 = imageReader.ReadUInt32();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    byte b3 = 0;
                    byte b4 = 0;
                    byte b5 = 0;
                    byte b6 = byte.MaxValue;
                    uint num4 = (num3 >> 2 * (4 * i + j)) & 3u;
                    if (num > num2)
                    {
                        switch (num4)
                        {
                            case 0u:
                                b3 = r;
                                b4 = g;
                                b5 = b;
                                break;
                            case 1u:
                                b3 = r2;
                                b4 = g2;
                                b5 = b2;
                                break;
                            case 2u:
                                b3 = (byte)((2 * r + r2) / 3);
                                b4 = (byte)((2 * g + g2) / 3);
                                b5 = (byte)((2 * b + b2) / 3);
                                break;
                            case 3u:
                                b3 = (byte)((r + 2 * r2) / 3);
                                b4 = (byte)((g + 2 * g2) / 3);
                                b5 = (byte)((b + 2 * b2) / 3);
                                break;
                        }
                    }
                    else
                    {
                        switch (num4)
                        {
                            case 0u:
                                b3 = r;
                                b4 = g;
                                b5 = b;
                                break;
                            case 1u:
                                b3 = r2;
                                b4 = g2;
                                b5 = b2;
                                break;
                            case 2u:
                                b3 = (byte)((r + r2) / 2);
                                b4 = (byte)((g + g2) / 2);
                                b5 = (byte)((b + b2) / 2);
                                break;
                            case 3u:
                                b3 = 0;
                                b4 = 0;
                                b5 = 0;
                                b6 = 0;
                                break;
                        }
                    }

                    int num5 = (x << 2) + j;
                    int num6 = (y << 2) + i;
                    if (num5 < width && num6 < height)
                    {
                        int num7 = num6 * width + num5 << 2;
                        imageData[num7] = b3;
                        imageData[num7 + 1] = b4;
                        imageData[num7 + 2] = b5;
                        imageData[num7 + 3] = b6;
                    }
                }
            }
        }

        internal static byte[] DecompressDxt3(byte[] imageData, int width, int height)
        {
            using MemoryStream imageStream = new MemoryStream(imageData);
            return DecompressDxt3(imageStream, width, height);
        }

        internal static byte[] DecompressDxt3(Stream imageStream, int width, int height)
        {
            byte[] array = new byte[width * height * 4];
            using BinaryReader imageReader = new BinaryReader(imageStream);
            int num = (width + 3) / 4;
            int num2 = (height + 3) / 4;
            for (int i = 0; i < num2; i++)
            {
                for (int j = 0; j < num; j++)
                {
                    DecompressDxt3Block(imageReader, j, i, num, width, height, array);
                }
            }

            return array;
        }

        private static void DecompressDxt3Block(BinaryReader imageReader, int x, int y, int blockCountX, int width, int height, byte[] imageData)
        {
            byte b = imageReader.ReadByte();
            byte b2 = imageReader.ReadByte();
            byte b3 = imageReader.ReadByte();
            byte b4 = imageReader.ReadByte();
            byte b5 = imageReader.ReadByte();
            byte b6 = imageReader.ReadByte();
            byte b7 = imageReader.ReadByte();
            byte b8 = imageReader.ReadByte();
            ushort color = imageReader.ReadUInt16();
            ushort color2 = imageReader.ReadUInt16();
            ConvertRgb565ToRgb888(color, out var r, out var g, out var b9);
            ConvertRgb565ToRgb888(color2, out var r2, out var g2, out var b10);
            uint num = imageReader.ReadUInt32();
            int num2 = 0;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    byte b11 = 0;
                    byte b12 = 0;
                    byte b13 = 0;
                    byte b14 = 0;
                    uint num3 = (num >> 2 * (4 * i + j)) & 3u;
                    switch (num2)
                    {
                        case 0:
                            b14 = (byte)((b & 0xFu) | (uint)((b & 0xF) << 4));
                            break;
                        case 1:
                            b14 = (byte)((b & 0xF0u) | (uint)((b & 0xF0) >> 4));
                            break;
                        case 2:
                            b14 = (byte)((b2 & 0xFu) | (uint)((b2 & 0xF) << 4));
                            break;
                        case 3:
                            b14 = (byte)((b2 & 0xF0u) | (uint)((b2 & 0xF0) >> 4));
                            break;
                        case 4:
                            b14 = (byte)((b3 & 0xFu) | (uint)((b3 & 0xF) << 4));
                            break;
                        case 5:
                            b14 = (byte)((b3 & 0xF0u) | (uint)((b3 & 0xF0) >> 4));
                            break;
                        case 6:
                            b14 = (byte)((b4 & 0xFu) | (uint)((b4 & 0xF) << 4));
                            break;
                        case 7:
                            b14 = (byte)((b4 & 0xF0u) | (uint)((b4 & 0xF0) >> 4));
                            break;
                        case 8:
                            b14 = (byte)((b5 & 0xFu) | (uint)((b5 & 0xF) << 4));
                            break;
                        case 9:
                            b14 = (byte)((b5 & 0xF0u) | (uint)((b5 & 0xF0) >> 4));
                            break;
                        case 10:
                            b14 = (byte)((b6 & 0xFu) | (uint)((b6 & 0xF) << 4));
                            break;
                        case 11:
                            b14 = (byte)((b6 & 0xF0u) | (uint)((b6 & 0xF0) >> 4));
                            break;
                        case 12:
                            b14 = (byte)((b7 & 0xFu) | (uint)((b7 & 0xF) << 4));
                            break;
                        case 13:
                            b14 = (byte)((b7 & 0xF0u) | (uint)((b7 & 0xF0) >> 4));
                            break;
                        case 14:
                            b14 = (byte)((b8 & 0xFu) | (uint)((b8 & 0xF) << 4));
                            break;
                        case 15:
                            b14 = (byte)((b8 & 0xF0u) | (uint)((b8 & 0xF0) >> 4));
                            break;
                    }

                    num2++;
                    switch (num3)
                    {
                        case 0u:
                            b11 = r;
                            b12 = g;
                            b13 = b9;
                            break;
                        case 1u:
                            b11 = r2;
                            b12 = g2;
                            b13 = b10;
                            break;
                        case 2u:
                            b11 = (byte)((2 * r + r2) / 3);
                            b12 = (byte)((2 * g + g2) / 3);
                            b13 = (byte)((2 * b9 + b10) / 3);
                            break;
                        case 3u:
                            b11 = (byte)((r + 2 * r2) / 3);
                            b12 = (byte)((g + 2 * g2) / 3);
                            b13 = (byte)((b9 + 2 * b10) / 3);
                            break;
                    }

                    int num4 = (x << 2) + j;
                    int num5 = (y << 2) + i;
                    if (num4 < width && num5 < height)
                    {
                        int num6 = num5 * width + num4 << 2;
                        imageData[num6] = b11;
                        imageData[num6 + 1] = b12;
                        imageData[num6 + 2] = b13;
                        imageData[num6 + 3] = b14;
                    }
                }
            }
        }

        internal static byte[] DecompressDxt5(byte[] imageData, int width, int height)
        {
            using MemoryStream imageStream = new MemoryStream(imageData);
            return DecompressDxt5(imageStream, width, height);
        }

        internal static byte[] DecompressDxt5(Stream imageStream, int width, int height)
        {
            byte[] array = new byte[width * height * 4];
            using BinaryReader imageReader = new BinaryReader(imageStream);
            int num = (width + 3) / 4;
            int num2 = (height + 3) / 4;
            for (int i = 0; i < num2; i++)
            {
                for (int j = 0; j < num; j++)
                {
                    DecompressDxt5Block(imageReader, j, i, num, width, height, array);
                }
            }

            return array;
        }

        private static void DecompressDxt5Block(BinaryReader imageReader, int x, int y, int blockCountX, int width, int height, byte[] imageData)
        {
            byte b = imageReader.ReadByte();
            byte b2 = imageReader.ReadByte();
            ulong num = imageReader.ReadByte();
            num += (ulong)imageReader.ReadByte() << 8;
            num += (ulong)imageReader.ReadByte() << 16;
            num += (ulong)imageReader.ReadByte() << 24;
            num += (ulong)imageReader.ReadByte() << 32;
            num += (ulong)imageReader.ReadByte() << 40;
            ushort color = imageReader.ReadUInt16();
            ushort color2 = imageReader.ReadUInt16();
            ConvertRgb565ToRgb888(color, out var r, out var g, out var b3);
            ConvertRgb565ToRgb888(color2, out var r2, out var g2, out var b4);
            uint num2 = imageReader.ReadUInt32();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    byte b5 = 0;
                    byte b6 = 0;
                    byte b7 = 0;
                    byte b8 = byte.MaxValue;
                    uint num3 = (num2 >> 2 * (4 * i + j)) & 3u;
                    uint num4 = (uint)((num >> 3 * (4 * i + j)) & 7);
                    b8 = num4 switch
                    {
                        0u => b,
                        1u => b2,
                        _ => (b <= b2) ? (num4 switch
                        {
                            6u => (byte)0,
                            7u => byte.MaxValue,
                            _ => (byte)(((6 - num4) * b + (num4 - 1) * b2) / 5),
                        }) : (byte)(((8 - num4) * b + (num4 - 1) * b2) / 7),
                    };
                    switch (num3)
                    {
                        case 0u:
                            b5 = r;
                            b6 = g;
                            b7 = b3;
                            break;
                        case 1u:
                            b5 = r2;
                            b6 = g2;
                            b7 = b4;
                            break;
                        case 2u:
                            b5 = (byte)((2 * r + r2) / 3);
                            b6 = (byte)((2 * g + g2) / 3);
                            b7 = (byte)((2 * b3 + b4) / 3);
                            break;
                        case 3u:
                            b5 = (byte)((r + 2 * r2) / 3);
                            b6 = (byte)((g + 2 * g2) / 3);
                            b7 = (byte)((b3 + 2 * b4) / 3);
                            break;
                    }

                    int num5 = (x << 2) + j;
                    int num6 = (y << 2) + i;
                    if (num5 < width && num6 < height)
                    {
                        int num7 = num6 * width + num5 << 2;
                        imageData[num7] = b5;
                        imageData[num7 + 1] = b6;
                        imageData[num7 + 2] = b7;
                        imageData[num7 + 3] = b8;
                    }
                }
            }
        }

        private static void ConvertRgb565ToRgb888(ushort color, out byte r, out byte g, out byte b)
        {
            int num = (color >> 11) * 255 + 16;
            r = (byte)((num / 32 + num) / 32);
            num = ((color & 0x7E0) >> 5) * 255 + 32;
            g = (byte)((num / 64 + num) / 64);
            num = (color & 0x1F) * 255 + 16;
            b = (byte)((num / 32 + num) / 32);
        }
    }
}

