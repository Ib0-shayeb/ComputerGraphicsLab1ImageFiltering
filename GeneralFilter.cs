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
using Avalonia.Styling;
using DynamicData;
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
        RandomDithering,
        KMeans,
    }
    public class ImageData{
        public byte[] pixelArray {get; set;}
        public int pixelWidth, pixelHeight, stride;
        public bool IsGrayScale { get; set;}
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
        public double constant {get; set;}
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
                case FilterType.RandomDithering:
                    Name = "RandomDithering";
                    break;
                case FilterType.KMeans:
                    Name = "KMeans";
                    constant = 8;
                    break;
            }
        }
        public byte[] ApplyFilter(ImageData imageData){
            
            switch (FType)
            {
                case FilterType.Inversion:
                    GeneralFunctionFilterApply(imageData);
                    break;
                case FilterType.BrightnessCorrection:
                    GeneralFunctionFilterApply(imageData);
                    // BrightnessCorrectionFilterApply(pixelArray, pixelWidth, pixelHeight, stride);
                    break;
                case FilterType.ContrastEnhancement:
                    GeneralFunctionFilterApply(imageData);
                    //ContrastEnhancementFilterApply(pixelArray, pixelWidth, pixelHeight, stride);
                    break;
                case FilterType.GammaCorrection:
                    //GeneralFunctionFilterApply(pixelArray, pixelWidth, pixelHeight, stride);
                    GammaCorrectionFilterApply(imageData);
                    break;
                case FilterType.Blur:
                    BlurFilterApply(imageData);
                    break;
                case FilterType.Emboss:
                    EmbossFilterApply(imageData);
                    break;
                case FilterType.EdgeDetection:
                    EdgeDetectionFilterApply(imageData);
                    break;
                case FilterType.Sharpen:
                    SharpenFilterApply(imageData);
                    break;
                case FilterType.GausianBlur:
                    GausianBlurFilterApply(imageData);
                    break;
                case FilterType.Errosion:
                    GausianBlurFilterApply(imageData);
                    break;
                case FilterType.Dilation:
                    GausianBlurFilterApply(imageData);
                    break;
                case FilterType.GrayScale:
                    GrayScaleFilterApply(imageData);
                    break;
                case FilterType.RandomDithering:
                    RandomDitheringFilterApply(imageData);
                    break;
                case FilterType.KMeans:
                    KMeansFilterApply(imageData);
                    break;
            }
            
            return imageData.pixelArray;
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
        public byte[] GeneralFunctionFilterApply(ImageData imageData)
        {
            int pixelHeight = imageData.pixelHeight;
            int pixelWidth = imageData.pixelWidth;
            int stride = imageData.stride;
            byte[] pixelArray = imageData.pixelArray;

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
        public static byte[] ErrosionFilterApply(ImageData imageData)
        {
            int pixelHeight = imageData.pixelHeight;
            int pixelWidth = imageData.pixelWidth;
            int stride = imageData.stride;
            byte[] pixelArray = imageData.pixelArray;

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
        public static byte[] DilationFilterApply(ImageData imageData)
        {
            int pixelHeight = imageData.pixelHeight;
            int pixelWidth = imageData.pixelWidth;
            int stride = imageData.stride;
            byte[] pixelArray = imageData.pixelArray;
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
        public static byte[] EmbossFilterApply( ImageData imageData)
        {
            int pixelHeight = imageData.pixelHeight;
            int pixelWidth = imageData.pixelWidth;
            int stride = imageData.stride;
            byte[] pixelArray = imageData.pixelArray;

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
        public static byte[] EdgeDetectionFilterApply(ImageData imageData)
        {
            int pixelHeight = imageData.pixelHeight;
            int pixelWidth = imageData.pixelWidth;
            int stride = imageData.stride;
            byte[] pixelArray = imageData.pixelArray;

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
        public static byte[] SharpenFilterApply(ImageData imageData)
        {
            int pixelHeight = imageData.pixelHeight;
            int pixelWidth = imageData.pixelWidth;
            int stride = imageData.stride;
            byte[] pixelArray = imageData.pixelArray;

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
        public static byte[] GausianBlurFilterApply(ImageData imageData)
        {
            int pixelHeight = imageData.pixelHeight;
            int pixelWidth = imageData.pixelWidth;
            int stride = imageData.stride;
            byte[] pixelArray = imageData.pixelArray;

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
        public static byte[] BlurFilterApply(ImageData imageData)
        {
            int pixelHeight = imageData.pixelHeight;
            int pixelWidth = imageData.pixelWidth;
            int stride = imageData.stride;
            byte[] pixelArray = imageData.pixelArray;

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
        public static byte[] BrightnessCorrectionFilterApply(ImageData imageData)
        {
            int pixelHeight = imageData.pixelHeight;
            int pixelWidth = imageData.pixelWidth;
            int stride = imageData.stride;
            byte[] pixelArray = imageData.pixelArray;

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
        public static byte[] ContrastEnhancementFilterApply(ImageData imageData)
        {
            int pixelHeight = imageData.pixelHeight;
            int pixelWidth = imageData.pixelWidth;
            int stride = imageData.stride;
            byte[] pixelArray = imageData.pixelArray;

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
        public static byte[] GammaCorrectionFilterApply(ImageData imageData)
        {
            int pixelHeight = imageData.pixelHeight;
            int pixelWidth = imageData.pixelWidth;
            int stride = imageData.stride;
            byte[] pixelArray = imageData.pixelArray;

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
        public static byte[] InversionFilterApply(ImageData imageData)
        {
            int pixelHeight = imageData.pixelHeight;
            int pixelWidth = imageData.pixelWidth;
            int stride = imageData.stride;
            byte[] pixelArray = imageData.pixelArray;

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
        public int CompareColors(byte ar, byte ag, byte ab, byte br, byte bg, byte bb){
            return (int)Math.Sqrt(Math.Pow(ar - br, 2) + Math.Pow(ag - bg, 2) + Math.Pow(ab - bb, 2));
        }
        public bool differentCollorPallete(byte[] oldPalette, byte[] palette, int K){
            //returns true if palette is different from oldPalette
            bool same = true;
            for(int i = 0; i < K; i++){
                if(oldPalette[i*4] != palette[i*4]) same = false;
                if(oldPalette[(i*4)+1] != palette[(i*4)+1]) same = false;
                if(oldPalette[(i*4)+2] != palette[(i*4)+2]) same = false;
            }
            return !same;
        }
        public byte[] KMeansFilterApply(ImageData imageData)
        {
            int pixelHeight = imageData.pixelHeight;
            int pixelWidth = imageData.pixelWidth;
            int stride = imageData.stride;
            byte[] pixelArray = imageData.pixelArray;

            byte[] CentroidAssignment = new byte[pixelHeight * pixelWidth];//every pixel wil be assigned to a centroid
                                                                        //order = pixelArray order, data = centroidIndex
            int K = (int)constant;//palette size
            byte[] oldPalette = new byte[K * 4];//K collors
            byte[] palette = new byte[K * 4];//K collors
            //innit palette with K rand collors
            for(int i = 0; i< K; i++){
                palette[i*4] = (byte)(i* (255/K));
                palette[i*4 + 1] = (byte)(i* (255/K));
                palette[i*4 + 2] = (byte)(i* (255/K));
            }

            while(differentCollorPallete(oldPalette, palette, K)){
                for (int y = 0; y < pixelHeight; y++)
                {
                    for (int x = 0; x < pixelWidth; x++)
                    {
                        int index = (y * stride) + (x * 4); // Assuming 32-bit RGBA format

                        //assign each pixel to its closest centroid
                        byte selectedCentroidIndex = 0;
                        byte closestCentroidDistance = Byte.MaxValue;
                        for(int i = 0; i < K; i++){
                            //if (compare: color(palette[K], [K+1], [K+2]) to color(pixelArray[index], [index+1], [index+2])
                            int dist = CompareColors(palette[4*i], palette[4*i+1], palette[4*i+2], pixelArray[index], pixelArray[index+1], pixelArray[index+2]);
                            if(dist < closestCentroidDistance){
                                selectedCentroidIndex = (byte)i;
                                closestCentroidDistance = (byte)dist;
                            }
                        }
                        CentroidAssignment[(y * pixelWidth) + x] = selectedCentroidIndex; //at (x, y) set centroid to selectedCentroidIndex 
                    }
                }
                //for every centroid calculate new palette value by averaging the pixel collors for all pixels under that centroid
                Int128[] CentroidSums = new Int128[K * 4];
                int[] CentroidCounts = new int[K];
                for (int y = 0; y < pixelHeight; y++)
                {
                    for (int x = 0; x < pixelWidth; x++)
                    {
                        int index = (y * stride) + (x * 4); // Assuming 32-bit RGBA format

                        int centroidIndex = CentroidAssignment[(y * pixelWidth) + x];
                        CentroidCounts[centroidIndex]++;
                        CentroidSums[centroidIndex * 4] += pixelArray[index];
                        CentroidSums[centroidIndex * 4 + 1] += pixelArray[index + 1];
                        CentroidSums[centroidIndex * 4 + 2] += pixelArray[index + 2];
                    }
                }
                //save old & set new palette
                for(int i = 0; i< K; i++){
                    oldPalette[i*4] = palette[i*4];
                    oldPalette[i*4 + 1] = palette[i*4 + 1];
                    oldPalette[i*4 + 2] = palette[i*4 + 2];
                    palette[i*4] = (byte)(CentroidSums[i*4] / CentroidCounts[i]);
                    palette[i*4 + 1] = (byte)(CentroidSums[i*4 + 1] / CentroidCounts[i]);
                    palette[i*4 + 2] = (byte)(CentroidSums[i*4 + 2] / CentroidCounts[i]);
                }
            }
            //now actually modify the pixel array values by replacing them with its closest centroid
            for (int y = 0; y < pixelHeight; y++)
            {
                for (int x = 0; x < pixelWidth; x++)
                {
                    int index = (y * stride) + (x * 4); // Assuming 32-bit RGBA format

                    pixelArray[index] = palette[CentroidAssignment[(y * pixelWidth) + x]*4];
                    pixelArray[index + 1] = palette[CentroidAssignment[(y * pixelWidth) + x]*4 + 1];
                    pixelArray[index + 2] = palette[CentroidAssignment[(y * pixelWidth) + x]*4 + 2];
                }
            }

            return pixelArray;

        }
        public byte[] RandomDitheringFilterApply(ImageData imageData)
        {
            int pixelHeight = imageData.pixelHeight;
            int pixelWidth = imageData.pixelWidth;
            int stride = imageData.stride;
            byte[] pixelArray = imageData.pixelArray;

            Random rand = new Random();
            int k = 8;//dithering const/ palete size
            byte[] thresholds = new byte[k];
            byte[] palete = new byte[k];// preparing palete
            var step = 255.0 / (k - 1);
            for(int i = 0; i < k - 1; i++){
                palete[i] = (byte)(step * i);
                thresholds[i] = (byte)(step * i);
            }
            palete[k - 1] = 255;
            thresholds[k - 1] = 255;

            if(imageData.IsGrayScale){
                for (int y = 0; y < pixelHeight; y++)
                {
                    for (int x = 0; x < pixelWidth; x++)
                    {
                        int index = (y * stride) + (x * 4); // Assuming 32-bit RGBA format

                        for(int i = 1; i < k; i++){
                            byte randomNumber = (byte)rand.Next(0, (int)(step));
                            thresholds[i] -=  randomNumber;
                        }
                        
                        byte Gray = pixelArray[index];
                        for(int i = 0;i < k; i++){
                            if(Gray < thresholds[i]) {
                                Gray = palete[i];
                                break;
                            }
                        }

                        pixelArray[index] = (byte)TrimValue(Gray);      // Blue
                        pixelArray[index + 1] = (byte)TrimValue(Gray);  // Green
                        pixelArray[index + 2] = (byte)TrimValue(Gray);  // Red

                    }
                }
            }
            else{
                for (int y = 0; y < pixelHeight; y++)
                {
                    for (int x = 0; x < pixelWidth; x++)
                    {
                        int index = (y * stride) + (x * 4); // Assuming 32-bit RGBA format

                        for(int i = 0; i < k-1; i++){
                            byte randomNumber = (byte)rand.Next(0, (int)(step));
                            thresholds[i] +=  randomNumber;
                        }
                        
                        byte Color = pixelArray[index];
                        for(int i = 0;i < k; i++){
                            if(Color < thresholds[i]) {
                                Color = palete[i];
                                break;
                            }
                        }

                        pixelArray[index] = (byte)TrimValue(Color);      // Blue

                        for(int i = 0; i < k-1; i++){
                            byte randomNumber = (byte)rand.Next(0, (int)(step));
                            thresholds[i] +=  randomNumber;
                        }
                        
                        Color = pixelArray[index + 1];
                        for(int i = 0;i < k; i++){
                            if(Color < thresholds[i]) {
                                Color = palete[i];
                                break;
                            }
                        }
                        pixelArray[index + 1] = (byte)TrimValue(Color);  // Green

                        for(int i = 0; i < k-1; i++){
                            byte randomNumber = (byte)rand.Next(0, (int)(step));
                            thresholds[i] +=  randomNumber;
                        }
                        
                        Color = pixelArray[index + 2];
                        for(int i = 0;i < k; i++){
                            if(Color < thresholds[i]) {
                                Color = palete[i];
                                break;
                            }
                        }
                        pixelArray[index + 2] = (byte)TrimValue(Color);  // Red

                    }
                }
            }

            return pixelArray;

        }
        public byte[] GrayScaleFilterApply(ImageData imageData)
        {
            int pixelHeight = imageData.pixelHeight;
            int pixelWidth = imageData.pixelWidth;
            int stride = imageData.stride;
            byte[] pixelArray = imageData.pixelArray;

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
            imageData.IsGrayScale = true;

            return pixelArray;

        }
    }
}