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

        static void mirror(GrayscaleFloatImage image, char axis)
        {
            if (axis == 'x')
            {
                for (int y = 0; y < image.Height; y++)
                    for (int x = 0; x < image.Width / 2; x++)
                    {
                        float p = image[x, y];
                        image[x, y] = image[image.Width - 1 - x, y];
                        image[image.Width - 1 - x, y] = p;
                    }
            } else if (axis == 'y')
                {
                    for (int y = 0; y < image.Height / 2; y++)
                        for (int x = 0; x < image.Width; x++)
                        {
                            float p = image[x, y];
                            image[x, y] = image[x, image.Height - 1 - y];
                            image[x, image.Height - 1 - y] = p;
                        }
                }
            else
            {
                Console.WriteLine("Wrong axis chosen");
                return;
            }
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
            GrayscaleFloatImage image = ImageIO.FileToGrayscaleFloatImage(InputFileName);

            mirror(image, 'y');

            ImageIO.ImageToFile(image, OutputFileName);
            Console.WriteLine("Image trasformed successfully");
            Console.ReadKey();
        }
    }
}
