using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia;
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
        Errosion,
        Dilation,
        GrayScale,
    }

    public class GeneralFilterListItem{
        public GeneralFilter filter {get;}
        public int index {get;}
        public bool selected {get; set;}
        public GeneralFilterListItem(GeneralFilter _filter, int _index){
            filter = _filter;
            index = _index;
            selected = false;
        }

        internal static GeneralFilterListItem? FirstOrDefault(Func<object, object> value)
        {
            throw new NotImplementedException();
        }
    }

    public class GeneralFilter
    {
        public EditablePolyline editablePolyline {get;}
        public double constant;
        public FilterType FType {get;}
        public  string Name {get;}
        public  Action? AddFilter { get; set; }

        public GeneralFilter(FilterType ftype){
            FType = ftype;
            Points points;
            switch (FType)
            {
                case FilterType.Inversion:
                    Name = "Inversion";
                    points = new Points{new Point(0, 255), new Point(255, 0)};
                    editablePolyline = new EditablePolyline(points);
                    break;
                case FilterType.BrightnessCorrection:
                    Name = "BrightnessCorrection";
                    constant = 50;
                    points = new Points{new Point(0, constant), new Point(255 - constant, 255), new Point(255, 255)};
                    editablePolyline = new EditablePolyline(points);
                    break;
                case FilterType.ContrastEnhancement:
                    Name = "ContrastEnhancement";
                    constant = 20;
                    points = new Points{new Point(0, 0), new Point(constant, 0), new Point(255 - constant, 255), new Point(255, 255)};
                    editablePolyline = new EditablePolyline(points);
                    break;
                case FilterType.GammaCorrection:
                    Name = "GammaCorrection";
                    constant = 0.25; //Gamma
                    var gammaCorrection = 1/constant;
                    points = new Points();
                    for(int i = 0; i < 256; i++){
                        points.Add(new Point(i, TrimValue((int)(Math.Pow((i/255.0), gammaCorrection) * 255))));
                    }
                    editablePolyline = new EditablePolyline(points);
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
                case FilterType.Errosion:
                    Name = "Errosion";
                    break;
                case FilterType.Dilation:
                    Name = "Dilation";
                    break;
                case FilterType.GrayScale:
                    Name = "GrayScale";
                    break;
            }
        }
        public  byte[] ApplyFilter(byte[] pixelArray, int pixelWidth, int pixelHeight, int stride){
            
            switch (FType)
            {
                case FilterType.Inversion:
                    GeneralFunctionFilterApply(pixelArray, pixelWidth, pixelHeight, stride);
                    break;
                case FilterType.BrightnessCorrection:
                    GeneralFunctionFilterApply(pixelArray, pixelWidth, pixelHeight, stride);
                    // BrightnessCorrectionFilterApply(pixelArray, pixelWidth, pixelHeight, stride);
                    break;
                case FilterType.ContrastEnhancement:
                    GeneralFunctionFilterApply(pixelArray, pixelWidth, pixelHeight, stride);
                    //ContrastEnhancementFilterApply(pixelArray, pixelWidth, pixelHeight, stride);
                    break;
                case FilterType.GammaCorrection:
                    //GeneralFunctionFilterApply(pixelArray, pixelWidth, pixelHeight, stride);
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
                case FilterType.Errosion:
                    GausianBlurFilterApply(pixelArray, pixelWidth, pixelHeight, stride);
                    break;
                case FilterType.Dilation:
                    GausianBlurFilterApply(pixelArray, pixelWidth, pixelHeight, stride);
                    break;
                case FilterType.GrayScale:
                    GrayScaleFilterApply(pixelArray, pixelWidth, pixelHeight, stride);
                    break;
            }
            
            return pixelArray;
        }
        public byte findOutputFromPolyline(byte color, IList<Point> points){
            int i;
            for(i =0; i<points.Count; i++){
                if(points[i].X >= color) break;
            }
            if(i == 0) return (byte)points[0].Y;
            var M = (points[i].Y - points[i-1].Y)/(points[i].X - points[i-1].X);
            var C = points[i].Y  - M * points[i].X;
            byte Y = (byte)(M * color + C);
            return Y;
        }
        public byte[] GeneralFunctionFilterApply(byte[] pixelArray, int pixelWidth, int pixelHeight, int stride)
        {
            var points = editablePolyline._polyline.Points;

            for (int y = 0; y < pixelHeight; y++)
            {
                for (int x = 0; x < pixelWidth; x++)
                {
                    int index = (y * stride) + (x * 4); // Assuming 32-bit RGBA format

                    pixelArray[index] = findOutputFromPolyline(pixelArray[index], points);      // Blue
                    pixelArray[index + 1] = findOutputFromPolyline(pixelArray[index + 1], points);  // Green
                    pixelArray[index + 2] = findOutputFromPolyline(pixelArray[index + 2], points); // Red

                }
            }

            return pixelArray;

        }
        public static byte[] ErrosionFilterApply(byte[] pixelArray, int pixelWidth, int pixelHeight, int stride)
        {
                
            for (int y = 0; y < pixelHeight; y++)
            {
                for (int x = 0; x < pixelWidth; x++)
                {
                    int index = (y * stride) + (x * 4); // Assuming 32-bit RGBA format

                    pixelArray[index] = (byte)(ConvolutionalFilterMin(pixelArray, x, y, pixelWidth, pixelHeight, stride, 3, 3, 1, 1, 0));
                    pixelArray[index + 1] = (byte)(ConvolutionalFilterMin(pixelArray, x, y, pixelWidth, pixelHeight, stride, 3, 3, 1, 1, 1));
                    pixelArray[index + 2] = (byte)(ConvolutionalFilterMin(pixelArray, x, y, pixelWidth, pixelHeight, stride, 3, 3, 1, 1, 2));
                }
            }

            return pixelArray;

        }
        public static byte[] DilationFilterApply(byte[] pixelArray, int pixelWidth, int pixelHeight, int stride)
        {
            
            for (int y = 0; y < pixelHeight; y++)
            {
                for (int x = 0; x < pixelWidth; x++)
                {
                    int index = (y * stride) + (x * 4); // Assuming 32-bit RGBA format

                    pixelArray[index] = (byte)(ConvolutionalFilterMax(pixelArray, x, y, pixelWidth, pixelHeight, stride, 3, 3, 1, 1, 0));
                    pixelArray[index + 1] = (byte)(ConvolutionalFilterMax(pixelArray, x, y, pixelWidth, pixelHeight, stride, 3, 3, 1, 1, 1));
                    pixelArray[index + 2] = (byte)(ConvolutionalFilterMax(pixelArray, x, y, pixelWidth, pixelHeight, stride, 3, 3, 1, 1, 2));
                }
            }

            return pixelArray;

        }
        public static int ConvolutionalFilterMax(byte[] pixelArray, int x, int y, int pixelWidth, int pixelHeight, int stride, int mheight, int mwidth, int mx, int my, int clr)
        {
            int max = 0;
            for (int i = 0; i < mheight; i++)
            {
                for (int j = 0; j < mwidth; j++)
                {
                    int actualx = Math.Max(Math.Min(x + j - mx, pixelWidth - 1), 0);
                    int actualy = Math.Max(Math.Min(y + i - my, pixelHeight -1), 0);//replace oob with edge pixels
                    if(actualx == mx && actualy == my) continue;

                    int index = (actualy * stride) + (actualx * 4);
                    
                    
                    max = (pixelArray[index + clr] > max) ? pixelArray[index + clr] : max;
                }
            }

            return max;
        }
        public static int ConvolutionalFilterMin(byte[] pixelArray, int x, int y, int pixelWidth, int pixelHeight, int stride, int mheight, int mwidth, int mx, int my, int clr)
        {
            int min = 255;
            for (int i = 0; i < mheight; i++)
            {
                for (int j = 0; j < mwidth; j++)
                {
                    int actualx = Math.Max(Math.Min(x + j - mx, pixelWidth - 1), 0);
                    int actualy = Math.Max(Math.Min(y + i - my, pixelHeight -1), 0);//replace oob with edge pixels
                    if(actualx == mx && actualy == my) continue;
                    
                    int index = (actualy * stride) + (actualx * 4);
                    
                    min = (pixelArray[index + clr] < min) ? pixelArray[index + clr] : min;
                }
            }

            return min;
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
        public static byte TrimValue(int value){
            return (byte)Math.Max(0, Math.Min(255, value));
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
        public static byte[] GrayScaleFilterApply(byte[] pixelArray, int pixelWidth, int pixelHeight, int stride)
        {
            for (int y = 0; y < pixelHeight; y++)
            {
                for (int x = 0; x < pixelWidth; x++)
                {
                    int index = (y * stride) + (x * 4); // Assuming 32-bit RGBA format

                    int B = pixelArray[index];
                    int G = pixelArray[index + 1];
                    int R = pixelArray[index + 2];

                    byte Gray = (byte)TrimValue((int)(0.299*R+0.587*G+0.114*B));

                    pixelArray[index] = Gray;
                    pixelArray[index + 1] = Gray;
                    pixelArray[index + 2] = Gray;
                }
            }

            return pixelArray;

        }
    }
}