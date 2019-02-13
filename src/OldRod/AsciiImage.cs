using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace OldRod
{
    public class AsciiImage
    {
        private static readonly IDictionary<Color, ConsoleColor> ColorMapping = new Dictionary<Color, ConsoleColor>
        {
            [Color.Red] = ConsoleColor.Red,
            [Color.DarkRed] = ConsoleColor.DarkRed,
            [Color.Blue] = ConsoleColor.Blue,
            [Color.DarkBlue] = ConsoleColor.DarkBlue,
            [Color.Gray] = ConsoleColor.Gray,
            [Color.DimGray] = ConsoleColor.DarkGray,
            [Color.Cyan] = ConsoleColor.Cyan,
            [Color.DarkCyan] = ConsoleColor.DarkCyan,
            [Color.Green] = ConsoleColor.Green,
            [Color.DarkGreen] = ConsoleColor.DarkGreen,
            [Color.Yellow] = ConsoleColor.Yellow,
            [Color.DarkGoldenrod] = ConsoleColor.DarkYellow,
            [Color.Magenta] = ConsoleColor.Magenta,
            [Color.DarkMagenta] = ConsoleColor.DarkMagenta,
            [Color.Black] = ConsoleColor.DarkGray,
            [Color.White] = ConsoleColor.White,
        };

        public AsciiImage(Bitmap image)
        {
            Image = image ?? throw new ArgumentNullException(nameof(image));
        }

        public Bitmap Image
        {
            get;
        }

        public string CharacterMap => " .:owM";
        public unsafe void PrintAscii(bool colored)
        {
            var info = Image.LockBits(
                new Rectangle(0, 0, Image.Width, Image.Height), 
                ImageLockMode.ReadOnly,
                Image.PixelFormat);

            try
            {
                for (int y = 0; y < info.Height; y++)
                {
                    for (int x = 0; x < info.Width; x++)
                    {
                        int raw = *(int*) (info.Scan0 + y * info.Stride + x * 4);
                        var color = Color.FromArgb(raw);
                        Console.ForegroundColor = GetClosestConsoleColor(color);
                        Console.Write(CharacterMap[(int) (color.A / 255f * (CharacterMap.Length - 1))]);
                    }

                    Console.WriteLine();
                }
            }
            finally
            {
                Image.UnlockBits(info);
            }
            
            Console.ResetColor();
        }

        private ConsoleColor GetClosestConsoleColor(Color color)
        {
            Color best = Color.White;
            ConsoleColor bestMapping = ConsoleColor.White;
            
            foreach (var entry in ColorMapping)
            {
                if (GetDifference(color, entry.Key) < GetDifference(color, best))
                {
                    best = entry.Key;
                    bestMapping = entry.Value;
                }
            }

            return bestMapping;
        }

        private static int GetDifference(Color color, Color other)
        {
            int rdiff = Math.Abs(color.R - other.R);
            int gdiff = Math.Abs(color.G - other.G);
            int bdiff = Math.Abs(color.B - other.B);
            return (rdiff + gdiff + bdiff) / 3;
        }
    }
}