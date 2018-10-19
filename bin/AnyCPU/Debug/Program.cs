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
        static ColorFloatImage mirror(ColorFloatImage image, string axis)
        {
            if (axis == "x")
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
            } else if (axis == "y")
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

        static ColorFloatImage img_expansion (ColorFloatImage inp_img, String mode, int radius)
        {
            ColorFloatImage out_img = new ColorFloatImage(inp_img.Width + 2 * radius, inp_img.Height + 2 * radius);
            for (int y = radius; y < out_img.Height - radius; y++) //centre part pf image
                for (int x = 0; x < out_img.Width; x++)
                    if (mode == "rep")//replicate
                        if (x < radius)
                            out_img[x, y] = inp_img[0, y - radius];
                        else if (x >= radius + inp_img.Width)
                            out_img[x, y] = inp_img[inp_img.Width - 1, y - radius];
                        else
                            out_img[x, y] = inp_img[x - radius, y - radius]; 
                    else if (mode == "odd")//odd
                        if (x < radius)
                            out_img[x, y] = 2 * inp_img[0, y - radius] + (-1) * inp_img[radius - x - 1, y - radius];
                        else if (x >= radius + inp_img.Width)
                            out_img[x, y] = 2 * inp_img[inp_img.Width - 1, y - radius] + (-1) * inp_img[radius + 2 * inp_img.Width - x - 1, y - radius];
                        else
                            out_img[x, y] = inp_img[x - radius, y - radius];
                    else if (mode == "even")//even
                        if (x < radius)
                            out_img[x, y] = inp_img[radius - x - 1, y - radius];
                        else if (x >= radius + inp_img.Width)
                            out_img[x, y] = inp_img[radius + 2 * inp_img.Width - x - 1, y - radius];
                        else
                            out_img[x, y] = inp_img[x - radius, y - radius];
            for (int y = 0; y < radius; y++) //upper part of image
                for (int x = 0; x < out_img.Width; x++)
                    if (mode == "rep")//replicate
                        out_img[x, y] = out_img[x, radius]; 
                    else if (mode == "odd") // odd
                        out_img[x, y] = 2 * out_img[x, radius] + (-1) * out_img[x, 2 * radius - y - 1];
                    else if (mode == "even") // even
                        out_img[x,y] = out_img[x, 2 * radius - y - 1];
            for (int y = inp_img.Height + radius; y < out_img.Height; y++) //lower part of image
                for (int x = 0; x < out_img.Width; x++)
                    if (mode == "rep")//replicate
                        out_img[x, y] = out_img[x, out_img.Height - radius - 1];
                    else if (mode == "odd")//odd
                        out_img[x, y] = 2 * out_img[x, out_img.Height - radius - 1] + (-1) * out_img[x, 2 * (out_img.Height - radius) - y - 1];
                    else if (mode == "even")//even
                        out_img[x, y] = out_img[x, 2 * (out_img.Height - radius) - y - 1];
            return out_img;
        }

        static ColorFloatImage sobel(ColorFloatImage image, String mode, String axis)
        {
            int x_flag = 0, y_flag = 0;
            if (axis == "x")
            {
                x_flag = 1;
            } else if (axis == "y")
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

            int rad = 50;
            ColorFloatImage test_image = img_expansion(image, mode, rad);
            ColorFloatImage out_img = new ColorFloatImage(image.Width, image.Height);
            for (int y = rad; y < out_img.Height + rad; y++)
            {
                for (int x = rad; x < out_img.Width + rad; x++)
                {
                    out_img[x - rad, y - rad] = x_flag * ((-1) * (test_image[x - 1, y - 1] + test_image[x + 1, y - 1]) + (-2) * test_image[x, y - 1] +
                        test_image[x - 1, y + 1] + test_image[x + 1, y + 1] + 2 * test_image[x, y + 1]) +
                        y_flag * ((-1) * (test_image[x - 1, y - 1] + test_image[x - 1, y + 1]) + (-2) * test_image[x - 1, y] +
                        test_image[x + 1, y - 1] + test_image[x + 1, y + 1] + 2 * test_image[x + 1, y]) + 128;
                }
            }

            return out_img;
        }

        static ColorFloatImage median(ColorFloatImage image, int rad)
        {
            if (rad <= 0)
            {
                Console.WriteLine("Radius must be positive amount of pixels");
                return image;
            }

            int window_size = 2 * rad + 1;
            ColorFloatImage out_img = new ColorFloatImage(image.Width, image.Height);
            for (int y = 0; y < image.Height - 1; y++)
                for (int x = 0; x < image.Width - 1; x++)
                {
                    ColorFloatPixel[] pixel_array = new ColorFloatPixel[window_size * window_size];
                    int n = 0;
                    for (int j = -(window_size / 2); j <= window_size / 2; j++)
                        for (int i = -(window_size / 2); i <= window_size / 2; i++)
                        {
                            if ((x + i >= 0) && (x + i < image.Width) && (y + j >= 0) && (y + j < image.Height))
                                pixel_array[n++] = image[x + i, y + j];
                        }
                    Array.Sort(pixel_array);
                    out_img[x, y] = pixel_array[n/2];
                }
            return out_img;
        }

        static ColorFloatImage gauss(ColorFloatImage image, String mode, float sigma)
        {
            if (mode != "rep" && mode != "odd" && mode != "even")
            {
                Console.WriteLine("Wrong edge mode");
                return image;
            }
            if (sigma == 0)
            {
                Console.WriteLine("Sigma must differ from 0");
                return image;
            }
            float sigma2 = 2 * sigma * sigma;
            int window_rad = (int)Math.Round(3 * sigma);
            float[,] window = new float[window_rad * 2 + 1, window_rad * 2 + 1];

            //float weight_sum = 0;
            float final_sum = 0;
            for (int i = 0; i <= window_rad; i++)
                for (int j = 0; j <= window_rad; j++)
                {
                    double xyz = Math.Exp((-i * i - j * j) / sigma2);
                    window[i + window_rad, j + window_rad] = (float)xyz;
                    window[window_rad + i, window_rad - j] = window[i + window_rad, j + window_rad];
                    window[window_rad - i, window_rad + j] = window[i + window_rad, j + window_rad];
                    window[window_rad - i, window_rad - j] = window[i + window_rad, j + window_rad];
                    //weight_sum += 4 * window[i + window_rad,j + window_rad];
                }
            for (int i = 0; i <= window_rad * 2; i++)
            {
                for (int j = 0; j <= window_rad * 2; j++)
                {
                    //window[i, j] = window[i, j] / weight_sum;
                    //Console.Write(" {0} ", window[i, j]);
                    final_sum += window[i, j];
                }
                //Console.Write('\n');
            }
            //Console.WriteLine(final_sum);
            //Console.WriteLine(weight_sum);
            ColorFloatImage test_image = img_expansion(image, mode, window_rad);
            ColorFloatImage out_img = new ColorFloatImage(image.Width, image.Height);

            for (int y = window_rad; y < out_img.Height + window_rad; y++)
                for (int x = window_rad; x < out_img.Width + window_rad; x++)
                {

                    for (int k = -window_rad; k <= window_rad; k++)
                        for (int n = -window_rad; n <= window_rad; n++)
                            out_img[x - window_rad, y - window_rad] +=
                                test_image[x + k, y + n] * window[window_rad + k, window_rad + n] / final_sum;


                }
            return out_img;
        }

        static ColorFloatImage gauss1(ColorFloatImage image, String mode, float sigma)
        {
            if (mode != "rep" && mode != "odd" && mode != "even")
            {
                Console.WriteLine("Wrong edge mode");
                return image;
            }
            if (sigma == 0)
            {
                Console.WriteLine("Sigma must differ from 0");
                return image;
            }
            float sigma2 = 2 * sigma * sigma;
            int window_rad = (int)Math.Round(3 * sigma);
            float[] window = new float[window_rad * 2 + 1];

            //float weight_sum = 0;
            float final_sum = 0;
            for (int i = 0; i <= window_rad; i++)
                //for (int j = 0; j <= window_rad; j++)
                {
                    double xyz = Math.Exp((-i * i) / sigma2);
                    window[i + window_rad] = (float)xyz;
                    window[window_rad - i] = window[window_rad + i];
                    //weight_sum += 4 * window[i + window_rad,j + window_rad];
                }
            for (int i = 0; i <= window_rad * 2; i++)
            {
                    final_sum += window[i];
            }
            //Console.WriteLine(final_sum);
            //Console.WriteLine(weight_sum);
            ColorFloatImage test_image = img_expansion(image, mode, window_rad);
            ColorFloatImage out_img = new ColorFloatImage(image.Width, image.Height);

            for (int y = window_rad; y < out_img.Height + window_rad; y++)
                for (int x = window_rad; x < out_img.Width + window_rad; x++)
                {
                    for (int k = -window_rad; k <= window_rad; k++)
                        out_img[x - window_rad, y - window_rad] +=
                            test_image[x + k, y] * window[window_rad + k] / final_sum / 2;
                }

            for (int y = window_rad; y < out_img.Height + window_rad; y++)
                for (int x = window_rad; x < out_img.Width + window_rad; x++)
                {
                    for (int k = -window_rad; k <= window_rad; k++)
                        out_img[x - window_rad, y - window_rad] +=
                            test_image[x, y + k] * window[window_rad + k] / final_sum / 2;
                }
            return out_img;
        }

        static ColorFloatImage gradient(ColorFloatImage image, String mode, float sigma)
        {
            if (mode != "rep" && mode != "odd" && mode != "even")
            {
                Console.WriteLine("Wrong edge mode");
                return image;
            }

            ColorFloatImage temp_image = gauss(image, mode, sigma);
            temp_image = img_expansion(temp_image, mode, 1);

            ColorFloatImage temp_image_x = new ColorFloatImage(image.Width, image.Height);
            ColorFloatImage temp_image_y = new ColorFloatImage(image.Width, image.Height);

            for (int y = 0; y < temp_image_x.Height; y++)
                for (int x = 0; x < temp_image_x.Width; x++)
                    temp_image_x[x, y] = temp_image[x + 1, y] + (-1) * temp_image[x, y];
            for (int y = 0; y < temp_image_y.Height; y++)
                for (int x = 0; x < temp_image_y.Width; x++)
                    temp_image_y[x, y] = temp_image[x, y + 1] + (-1) * temp_image[x, y];

            ColorFloatImage out_img = new ColorFloatImage(image.Width, image.Height);
            
            float max_color_r = 0;
            float max_color_g = 0;
            float max_color_b = 0;

            for (int y = 0; y < image.Height; y++)
                for (int x = 0; x < image.Width; x++)
                {
                    double r_x = temp_image_x[x, y].r, r_y = temp_image_y[x, y].r;
                    double g_x = temp_image_x[x, y].g, g_y = temp_image_y[x, y].g;
                    double b_x = temp_image_x[x, y].b, b_y = temp_image_y[x, y].b;
                    ColorFloatPixel temp_pixel;
                    temp_pixel.r = (float)(Math.Sqrt(r_x * r_x + r_y * r_y));
                    if (temp_pixel.r > max_color_r) max_color_r = temp_pixel.r;
                    temp_pixel.g = (float)(Math.Sqrt(g_x * g_x + g_y * g_y));
                    if (temp_pixel.g > max_color_g) max_color_g = temp_pixel.g;
                    temp_pixel.b = (float)(Math.Sqrt(b_x * b_x + b_y * b_y));
                    if (temp_pixel.r > max_color_b) max_color_b = temp_pixel.b;
                    temp_pixel.a = 0;
                    out_img[x, y] = temp_pixel;
                }
            //contrast increasing block
            double multiplier_r = 255/max_color_r;
            double multiplier_g = 255/max_color_g;
            double multiplier_b = 255/max_color_b;
            float mul = 1;
            if ((multiplier_r <= multiplier_g) && (multiplier_r <= multiplier_b))
                mul = (float)multiplier_r;
            else if (multiplier_g <= multiplier_r && multiplier_g <= multiplier_b)
                mul = (float)multiplier_g;
            else if (multiplier_b <= multiplier_r && multiplier_b <= multiplier_g)
                mul = (float)multiplier_b;
            for (int y = 0; y < image.Height; y++)
                for (int x = 0; x < image.Width; x++)
                    out_img[x, y] = out_img[x, y] * mul;

            return out_img;
        }

        static void Main(string[] args)
        {
            ColorFloatImage output_image = null;
            if (args[0] == "mirror")
            {
                ColorFloatImage image = ImageIO.FileToColorFloatImage(args[2]);
                output_image = mirror(image, args[1]);
                ImageIO.ImageToFile(output_image, args[3]);
            }
            else if (args[0] == "rotate")
            {
                ColorFloatImage image = ImageIO.FileToColorFloatImage(args[3]);
                output_image = rotate(image, args[1], Convert.ToInt32(args[2]));
                ImageIO.ImageToFile(output_image, args[4]);
            }
            else if (args[0] == "sobel")
            {
                ColorFloatImage image = ImageIO.FileToColorFloatImage(args[3]);
                output_image = sobel(image, args[1], args[2]);
                ImageIO.ImageToFile(output_image, args[4]);
            }
            else if (args[0] == "median")
            {
                ColorFloatImage image = ImageIO.FileToColorFloatImage(args[2]);
                output_image = median(image, Convert.ToInt32(args[1]));
                ImageIO.ImageToFile(output_image, args[3]);
            }
            else if (args[0] == "gauss")
            {
                ColorFloatImage image = ImageIO.FileToColorFloatImage(args[3]);
                output_image = gauss(image, args[1], (float)(Convert.ToDouble(args[2])));
                ImageIO.ImageToFile(output_image, args[4]);
            }
            else if (args[0] == "gradient")
            {
                ColorFloatImage image = ImageIO.FileToColorFloatImage(args[3]);
                output_image = gradient(image, args[1], (float)(Convert.ToDouble(args[2])));
                //output_image = img_expansion(image, "odd", 40);
                ImageIO.ImageToFile(output_image, args[4]);
            }
            Console.WriteLine("Image trasformed successfully");
        }
    }
}
