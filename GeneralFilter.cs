using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using ReactiveUI;

namespace ComputerGraphicsLab1_ImageFiltering
{
    public enum FilterType {
                Inversion,
                BrightnessCorrection,
                ContrastEnhancement,
                GammaCorrection,
                Blur,
                Emboss,
                EdgeDetection,
                Sharpen,
                GausianBlur,
            }
    public class GeneralFilter
    {
        public int constant;
        public FilterType FType {get;}
        public  string Name {get;}
        public  Action? AddFilter { get; set; }

        public GeneralFilter(FilterType ftype){
            FType = ftype;
            switch (FType)
            {
                case FilterType.Inversion:
                    Name = "Inversion";
                    break;
                case FilterType.BrightnessCorrection:
                    Name = "BrightnessCorrection";
                    break;
                case FilterType.ContrastEnhancement:
                    Name = "ContrastEnhancement";
                    break;
                case FilterType.GammaCorrection:
                    Name = "GammaCorrection";
                    break;
                case FilterType.Blur:
                    Name = "BlurFilter";
                    break;
                case FilterType.Emboss:
                    Name = "Emboss";
                    break;
                case FilterType.EdgeDetection:
                    Name = "EdgeDetection";
                    break;
                case FilterType.Sharpen:
                    Name = "Sharpen";
                    break;
                case FilterType.GausianBlur:
                    Name = "GausianBlur";
                    break;
            }
        }
        public  byte[] ApplyFilter(byte[] pixelArray, int pixelWidth, int pixelHeight, int stride){
            
            switch (FType)
            {
                case FilterType.Inversion:
                    InversionFilterApply(pixelArray, pixelWidth, pixelHeight, stride);
                    break;
                case FilterType.BrightnessCorrection:
                    BrightnessCorrectionFilterApply(pixelArray, pixelWidth, pixelHeight, stride);
                    break;
                case FilterType.ContrastEnhancement:
                    ContrastEnhancementFilterApply(pixelArray, pixelWidth, pixelHeight, stride);
                    break;
                case FilterType.GammaCorrection:
                    GammaCorrectionFilterApply(pixelArray, pixelWidth, pixelHeight, stride);
                    break;
                case FilterType.Blur:
                    BlurFilterApply(pixelArray, pixelWidth, pixelHeight, stride);
                    break;
                case FilterType.Emboss:
                    EmbossFilterApply(pixelArray, pixelWidth, pixelHeight, stride);
                    break;
                case FilterType.EdgeDetection:
                    EdgeDetectionFilterApply(pixelArray, pixelWidth, pixelHeight, stride);
                    break;
                case FilterType.Sharpen:
                    SharpenFilterApply(pixelArray, pixelWidth, pixelHeight, stride);
                    break;
                case FilterType.GausianBlur:
                    GausianBlurFilterApply(pixelArray, pixelWidth, pixelHeight, stride);
                    break;
            }
            
            return pixelArray;
        }
        //this function only sums, you have to divide the output
        public static int ConvolutionalFilterSum(byte[] pixelArray, int x, int y, int pixelWidth, int pixelHeight, int stride, int[,] matrix, int mheight, int mwidth, int mx, int my, int clr)
        {
            int sum = 0;
            for (int i = 0; i < mheight; i++)
            {
                for (int j = 0; j < mwidth; j++)
                {
                    int actualx = Math.Max(Math.Min(x + j - mx, pixelWidth - 1), 0);
                    int actualy = Math.Max(Math.Min(y + i - my, pixelHeight -1), 0);//replace oob with edge pixels
                    int index = (actualy * stride) + (actualx * 4);
                    
                    sum += pixelArray[index + clr] * matrix[i,j];//index  out of bound??
                }
            }

            return sum;
        }
        public static byte[] EmbossFilterApply(byte[] pixelArray, int pixelWidth, int pixelHeight, int stride)
        {
            int[,] matrix = {
                {-1, 0, 1},
                {-1, 1, 1},
                {-1, 0, 1}};
                
            for (int y = 0; y < pixelHeight; y++)
            {
                for (int x = 0; x < pixelWidth; x++)
                {
                    int index = (y * stride) + (x * 4); // Assuming 32-bit RGBA format

                    pixelArray[index] = (byte)(ConvolutionalFilterSum(pixelArray, x, y, pixelWidth, pixelHeight, stride, matrix, 3, 3, 1, 1, 0)/9);
                    pixelArray[index + 1] = (byte)(ConvolutionalFilterSum(pixelArray, x, y, pixelWidth, pixelHeight, stride, matrix, 3, 3, 1, 1, 1)/9);
                    pixelArray[index + 2] = (byte)(ConvolutionalFilterSum(pixelArray, x, y, pixelWidth, pixelHeight, stride, matrix, 3, 3, 1, 1, 2)/9);
                }
            }

            return pixelArray;

        }
        public static byte[] EdgeDetectionFilterApply(byte[] pixelArray, int pixelWidth, int pixelHeight, int stride)
        {
            int[,] matrix = {
                {0, -1, 0},
                {0, 1, 0},
                {0, 0, 0}};
                
            for (int y = 0; y < pixelHeight; y++)
            {
                for (int x = 0; x < pixelWidth; x++)
                {
                    int index = (y * stride) + (x * 4); // Assuming 32-bit RGBA format

                    pixelArray[index] = (byte)(ConvolutionalFilterSum(pixelArray, x, y, pixelWidth, pixelHeight, stride, matrix, 3, 3, 1, 1, 0)/9);
                    pixelArray[index + 1] = (byte)(ConvolutionalFilterSum(pixelArray, x, y, pixelWidth, pixelHeight, stride, matrix, 3, 3, 1, 1, 1)/9);
                    pixelArray[index + 2] = (byte)(ConvolutionalFilterSum(pixelArray, x, y, pixelWidth, pixelHeight, stride, matrix, 3, 3, 1, 1, 2)/9);
                }
            }

            return pixelArray;

        }
        public static byte[] SharpenFilterApply(byte[] pixelArray, int pixelWidth, int pixelHeight, int stride)
        {
            int[,] matrix = {
                {0,-1, 0},
                {-1, 5, -1},
                {0, -1, 0}};
                
            for (int y = 0; y < pixelHeight; y++)
            {
                for (int x = 0; x < pixelWidth; x++)
                {
                    int index = (y * stride) + (x * 4); // Assuming 32-bit RGBA format

                    pixelArray[index] = (byte)(ConvolutionalFilterSum(pixelArray, x, y, pixelWidth, pixelHeight, stride, matrix, 3, 3, 1, 1, 0)/9);
                    pixelArray[index + 1] = (byte)(ConvolutionalFilterSum(pixelArray, x, y, pixelWidth, pixelHeight, stride, matrix, 3, 3, 1, 1, 1)/9);
                    pixelArray[index + 2] = (byte)(ConvolutionalFilterSum(pixelArray, x, y, pixelWidth, pixelHeight, stride, matrix, 3, 3, 1, 1, 2)/9);
                }
            }

            return pixelArray;

        }
        public static byte[] GausianBlurFilterApply(byte[] pixelArray, int pixelWidth, int pixelHeight, int stride)
        {
            int[,] matrix = {
                {0, 1, 0},
                {1, 4, 1},
                {0, 1, 0}};
                
            for (int y = 0; y < pixelHeight; y++)
            {
                for (int x = 0; x < pixelWidth; x++)
                {
                    int index = (y * stride) + (x * 4); // Assuming 32-bit RGBA format

                    pixelArray[index] = (byte)(ConvolutionalFilterSum(pixelArray, x, y, pixelWidth, pixelHeight, stride, matrix, 3, 3, 1, 1, 0)/9);
                    pixelArray[index + 1] = (byte)(ConvolutionalFilterSum(pixelArray, x, y, pixelWidth, pixelHeight, stride, matrix, 3, 3, 1, 1, 1)/9);
                    pixelArray[index + 2] = (byte)(ConvolutionalFilterSum(pixelArray, x, y, pixelWidth, pixelHeight, stride, matrix, 3, 3, 1, 1, 2)/9);
                }
            }

            return pixelArray;

        }
        public static byte[] BlurFilterApply(byte[] pixelArray, int pixelWidth, int pixelHeight, int stride)
        {
            int[,] matrix = {
                {1, 1, 1},
                {1, 1, 1},
                {1, 1, 1}};
                
            for (int y = 0; y < pixelHeight; y++)
            {
                for (int x = 0; x < pixelWidth; x++)
                {
                    int index = (y * stride) + (x * 4); // Assuming 32-bit RGBA format

                    pixelArray[index] = (byte)(ConvolutionalFilterSum(pixelArray, x, y, pixelWidth, pixelHeight, stride, matrix, 3, 3, 1, 1, 0)/9);
                    pixelArray[index + 1] = (byte)(ConvolutionalFilterSum(pixelArray, x, y, pixelWidth, pixelHeight, stride, matrix, 3, 3, 1, 1, 1)/9);
                    pixelArray[index + 2] = (byte)(ConvolutionalFilterSum(pixelArray, x, y, pixelWidth, pixelHeight, stride, matrix, 3, 3, 1, 1, 2)/9);
                }
            }

            return pixelArray;

        }
        public void ExecuteAddFilter()
        {
            AddFilter?.Invoke();
        }
        public static int TrimValue(int value){
            return Math.Max(0, Math.Min(255, value));
        }
        public static byte[] BrightnessCorrectionFilterApply(byte[] pixelArray, int pixelWidth, int pixelHeight, int stride)
        {
            int CorrectionConstant = 30;
            for (int y = 0; y < pixelHeight; y++)
            {
                for (int x = 0; x < pixelWidth; x++)
                {
                    int index = (y * stride) + (x * 4); // Assuming 32-bit RGBA format

                    pixelArray[index] = (byte)TrimValue(pixelArray[index] + CorrectionConstant);      // Blue
                    pixelArray[index + 1] = (byte)TrimValue(pixelArray[index + 1] + CorrectionConstant);  // Green
                    pixelArray[index + 2] = (byte)TrimValue(pixelArray[index + 2] + CorrectionConstant); // Red

                }
            }

            return pixelArray;

        }
        public static byte[] ContrastEnhancementFilterApply(byte[] pixelArray, int pixelWidth, int pixelHeight, int stride)
        {
            int enhancmentReductionConst = 10;
            for (int y = 0; y < pixelHeight; y++)
            {
                for (int x = 0; x < pixelWidth; x++)
                {
                    int index = (y * stride) + (x * 4); // Assuming 32-bit RGBA format

                    pixelArray[index] = (byte)TrimValue(pixelArray[index] + (pixelArray[index] - 255/2)/enhancmentReductionConst);      // Blue
                    pixelArray[index + 1] = (byte)TrimValue(pixelArray[index + 1] + (pixelArray[index + 1] - 255/2)/enhancmentReductionConst);  // Green
                    pixelArray[index + 2] = (byte)TrimValue(pixelArray[index + 2] + (pixelArray[index + 2] - 255/2)/enhancmentReductionConst); // Red

                }
            }

            return pixelArray;

        }
        public static byte[] GammaCorrectionFilterApply(byte[] pixelArray, int pixelWidth, int pixelHeight, int stride)
        {
            var gamma = 0.25;
            var gammaCorrection = 1/gamma;
            //per pixel gama corection
            Func<int, byte> GC = (i) => (byte)TrimValue((int)(Math.Pow((i/255.0), gammaCorrection) * 255));
            for (int y = 0; y < pixelHeight; y++)
            {
                for (int x = 0; x < pixelWidth; x++)
                {
                    int index = (y * stride) + (x * 4); // Assuming 32-bit RGBA format

                    pixelArray[index] = GC(pixelArray[index]);          // Blue
                    pixelArray[index + 1] = GC(pixelArray[index + 1]);    // Green
                    pixelArray[index + 2] = GC(pixelArray[index + 2]);   // Red
                }
            }

            return pixelArray;

        }
        public static byte[] InversionFilterApply(byte[] pixelArray, int pixelWidth, int pixelHeight, int stride)
        {
            for (int y = 0; y < pixelHeight; y++)
            {
                for (int x = 0; x < pixelWidth; x++)
                {
                    int index = (y * stride) + (x * 4); // Assuming 32-bit RGBA format

                    pixelArray[index] = (byte)TrimValue(255 - pixelArray[index]);      // Blue
                    pixelArray[index + 1] = (byte)TrimValue(255 - pixelArray[index + 1]);  // Green
                    pixelArray[index + 2] = (byte)TrimValue(255 - pixelArray[index + 2]); // Red

                }
            }

            return pixelArray;

        }
    }
}