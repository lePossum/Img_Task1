using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace ImageReadCS
{
    class Program
    {
        /*static void FlipImage(GrayscaleFloatImage image)
        {
            for (int y = 0; y < image.Height; y++)
                for (int x = 0; x < image.Width / 2; x++)
                {
                    float p = image[x, y];
                    image[x, y] = image[image.Width - 1 - x, y];
                    image[image.Width - 1 - x, y] = p;
                }
        }*/

        static ColorFloatImage mirror(ColorFloatImage image, char axis)
        {
            if (axis == 'x')
            {
                ColorFloatImage out_img = image;
                for (int y = 0; y < out_img.Height; y++)
                    for (int x = 0; x < out_img.Width / 2; x++)
                    {
                        ColorFloatPixel p = out_img[x,y];
                        out_img[x, y] = out_img[out_img.Width - 1 - x, y];
                        out_img[out_img.Width - 1 - x, y] = p;
                    }
                return out_img;
            } else if (axis == 'y')
                {
                    ColorFloatImage out_img = image;
                    for (int y = 0; y < out_img.Height / 2; y++)
                        for (int x = 0; x < out_img.Width; x++)
                        {
                            ColorFloatPixel p = out_img[x, y];
                            out_img[x, y] = out_img[x, out_img.Height - 1 - y];
                            out_img[x, out_img.Height - 1 - y] = p;
                        }
                return out_img;
                }
            else
            {
                Console.WriteLine("Wrong axis chosen");
                return image;
            }
        }

        static ColorFloatImage rotate(ColorFloatImage image, String dir, int angle)
        {
            int local_angle;
            if (dir == "cw")
            {
                local_angle = angle % 360;
            }
            else if (dir == "ccw")
            {
                local_angle = 360 - (angle % 360);
            }
            else
                return image;

            switch (local_angle)
                {
                    case 0:
                        Console.WriteLine("Image rotated in 0 degrees");
                        return image;
                    case 90:
                    {
                        ColorFloatImage out_img = new ColorFloatImage(image.Height, image.Width);
                        for (int y = 0; y < image.Height; y++)
                            for (int x = 0; x < image.Width; x++)
                            {
                                ColorFloatPixel p = image[x, y];
                                out_img[out_img.Width - y - 1, x] = image[x, y];
                            }
                        return out_img;
                    }
                    case 180:
                    {
                        ColorFloatImage out_img = new ColorFloatImage(image.Width, image.Height);
                        for (int y = 0; y < image.Height; y++)
                            for (int x = 0; x < image.Width; x++)
                            {
                                ColorFloatPixel p = image[x, y];
                                out_img[out_img.Width - x - 1, out_img.Height - y -1] = image[x, y];
                            }
                        return out_img;
                    }
                    case 270:
                    {
                        ColorFloatImage out_img = new ColorFloatImage(image.Height, image.Width);
                        for (int y = 0; y < image.Height; y++)
                            for (int x = 0; x < image.Width; x++)
                            {
                                ColorFloatPixel p = image[x, y];
                                out_img[y, out_img.Height - 1 - x] = image[x, y];
                            }
                        return out_img;
                    }
                    default:
                        Console.WriteLine("Wrong angle (0, 90, 180, 270 possible)");
                        return image;
            }

        }

        /*ColorFloatPixel sobel_edge(ColorFloatImage image, int x_flag, int y_flag, String mode)
        {
            if (mode == "rep")
            {

            } else if (mode == "odd")
            {

            } else if (mode == "even")
            {

            }

            
        }*/

        static ColorFloatImage sobel(ColorFloatImage image, String mode, char axis)
        {
            int x_flag = 0, y_flag = 0;
            if (axis == 'x')
            {
                x_flag = 1;
            } else if (axis == 'y')
            {
                y_flag = 1;
            } else
            {
                Console.WriteLine("Wrong axis in Sobel filter");
                return image;
            }

            if (mode != "rep" && mode != "odd" && mode != "even")
            {
                Console.WriteLine("Wrong edge mode");
                return image;
            }

            ColorFloatImage out_img = new ColorFloatImage(image.Width, image.Height);
            for (int y = 0; y < image.Height; y++)
                for (int x = 0; x < image.Width; x++)
                {
                    if (x == 0 || y == 0 || x == image.Width - 1 || y == image.Height - 1)
                    {

                    } else
                    {
                        out_img[x, y] = x_flag * ((-1) * (image[x - 1, y - 1] + image[x + 1, y - 1]) + (-2) * image[x, y - 1] +
                            image[x - 1, y + 1] + image[x + 1, y + 1] + 2 * image[x, y + 1]) +
                            y_flag * ((-1) * (image[x - 1, y - 1] + image[x - 1, y + 1]) + (-2) * image[x - 1, y] +
                            image[x + 1, y - 1] + image[x + 1, y + 1] + 2 * image[x + 1, y]) + 128;
                    }
                }

            return out_img;
            /*out_img[0, 0] = image[0, 0];
            Console.WriteLine("Something is here: {0}, {1}, {2}, {3}", out_img[0, 0].r, out_img[0, 0].g, out_img[0, 0].b, out_img[0, 0].a);
            out_img[0, 0] = image[0, 0] * 2;
            Console.WriteLine("Something is here: {0}, {1}, {2}, {3}", out_img[0, 0].r, out_img[0, 0].g, out_img[0, 0].b, out_img[0, 0].a);
            return out_img;*/
        }

        static void Main(string[] args)
        {
            if (args.Length < 2) {
                Console.WriteLine("No input files12345");
                return;
            }
            string InputFileName = args[0], OutputFileName = args[1];
            if (!File.Exists(InputFileName))
            {
                Console.WriteLine("No Input file");
                return;
            }
            ColorFloatImage image = ImageIO.FileToColorFloatImage(InputFileName);
            ColorFloatImage output_image = sobel(image, "rep", 'x');

            ImageIO.ImageToFile(output_image, OutputFileName);
            Console.WriteLine("Image trasformed successfully");
            Console.ReadKey();
        }
    }
}
