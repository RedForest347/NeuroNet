using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    class ImageWork
    {
        //public static Bitmap BedPicture;
        //public static Bitmap IdealPictures;
        //public static Bitmap ReCreatePicture;

        public static void ChengePixels(Bitmap BM)
        {
            int Width = BM.Width;
            int Height = BM.Height;
            byte[,,] image_array = new byte[Width, Height, 3];

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    image_array[x, y, 0] = BM.GetPixel(x, y).R;
                    image_array[x, y, 1] = BM.GetPixel(x, y).G;
                    image_array[x, y, 2] = BM.GetPixel(x, y).B;
                }
            }

            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    BM.SetPixel(x, y, GetColor(x, y, ref image_array));

            //ImageBox.Image = BM;
        }

        static Color GetColor(int x, int y, ref byte[,,] image_array)
        {
            return Color.FromArgb(100, image_array[x, y, 1], image_array[x, y, 1], image_array[x, y, 2]);
        }

        public static float[] GetInputPixels(ref int x, ref int y, ref int x_after, ref int y_after, int offset, Bitmap ImageFrom)
        {
            float[] pixels = new float[(offset + offset + 1) * (offset + offset + 1) * 3];
            int current_pixel = 0;

            if (x < offset)
                x = offset;
            if (y < offset)
                y = offset;

            if (x > ImageFrom.Width - offset - 1)
            {
                x = offset;
                y++;
            }
            if (y > ImageFrom.Height - offset - 1)
            {
                y = offset;
            }

            for (int i = -offset + y; i <= offset + y; i++)
            {
                for (int j = -offset + x; j <= offset + x; j++)
                {
                    pixels[current_pixel] = ImageFrom.GetPixel(j, i).R / 255f;
                    current_pixel++;
                    pixels[current_pixel] = ImageFrom.GetPixel(j, i).G / 255f;
                    current_pixel++;
                    pixels[current_pixel] = ImageFrom.GetPixel(j, i).B / 255f;
                    current_pixel++;
                }
            }
            x_after = x;
            y_after = y;
            x++;
            return pixels;
        }

        public static float[] GetIdealPixel(int x, int y, Bitmap ImageFrom)
        {
            float[] pixels = new float[3];

            pixels[0] = ImageFrom.GetPixel(x, y).R / 255f;
            pixels[1] = ImageFrom.GetPixel(x, y).G / 255f;
            pixels[2] = ImageFrom.GetPixel(x, y).B / 255f;

            return pixels;
        }
    }
}
