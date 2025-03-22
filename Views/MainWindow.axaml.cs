using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Rendering;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using System.Threading.Tasks;
using System;
using Avalonia.Platform;
using System.Runtime.InteropServices;
using ComputerGraphicsLab1_ImageFiltering.ViewModels;
using System.IO;

namespace ComputerGraphicsLab1_ImageFiltering.Views;

public partial class MainWindow : Window
{
    public byte[] OriginalPixelData;
    public byte[] pixelArray;
    public int pixelWidth;
    public int pixelHeight;
    public int stride;
    public Image imageControl2;
    public WriteableBitmap bitmap2;
    public MainWindow()
    {
        DataContext = new MainWindowViewModel(); 
        InitializeComponent();

    }
    private async void Open_File_Button_Clicked(object sender, RoutedEventArgs e)
    {
       // Get top level from the current control. Alternatively, you can use Window reference instead.
        var topLevel = TopLevel.GetTopLevel(this);

        // Start async operation to open the dialog.
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open Text File",
            AllowMultiple = false
        });

        if (files.Count >= 1)
        {
            var imageControl = this.FindControl<Image>("ImageControll1");
            if(imageControl==null)
                return;
            var sourceBitmap = new Bitmap(files[0].Path.AbsolutePath);
            imageControl.Source = sourceBitmap;

            var size = sourceBitmap.Size;
            pixelWidth = (int)size.Width;
            pixelHeight = (int)size.Height;
            stride = pixelWidth * 4; // Assuming 32bpp (RGBA)

            OriginalPixelData = new byte[pixelHeight * stride];

            unsafe
            {
                fixed (byte* p = OriginalPixelData)
                {
                    sourceBitmap.CopyPixels(new PixelRect(0, 0, pixelWidth, pixelHeight), (nint)p, OriginalPixelData.Length, stride);
                }
            }
        }
    }
    
    private async void ApplyFilters(object sender, RoutedEventArgs e)
    {
        //create a copy to work with
        //pixelArray = new byte[OriginalPixelData.Length];
        var pixelData = new ImageData{ pixelArray = new byte[OriginalPixelData.Length],
         pixelHeight = pixelHeight, pixelWidth = pixelWidth, stride = stride, IsGrayScale = false};

        Array.Copy(OriginalPixelData, pixelData.pixelArray, OriginalPixelData.Length);

        var viewModel = this.DataContext as MainWindowViewModel;

        foreach (var filterItem in viewModel.SelectedFilters)
        {
            filterItem.filter.ApplyFilter(pixelData);
        }

        imageControl2 = this.FindControl<Image>("ImageControll2");
        Vector dpi = new Vector(96, 96);
        bitmap2 = new WriteableBitmap(
            new PixelSize(pixelWidth, pixelHeight),
            dpi,
            PixelFormat.Bgra8888,
            AlphaFormat.Premul);

        using (var frameBuffer = bitmap2.Lock())
        {
            Marshal.Copy(pixelData.pixelArray, 0, frameBuffer.Address, pixelData.pixelArray.Length);
        }
        imageControl2.Source = bitmap2;
    }
    private async void Save_Click(object sender, RoutedEventArgs e)
    {
        if (bitmap2==null) return;

        var dialog = new SaveFileDialog
        {
            Title = "Save Image",
            InitialFileName = "output.png",
            Filters = new()
            {
                new FileDialogFilter { Name = "PNG Image", Extensions = { "png" } },
                new FileDialogFilter { Name = "JPEG Image", Extensions = { "jpg", "jpeg" } }
            }
        };

        string? filePath = await dialog.ShowAsync(this);

        using (FileStream fs = new FileStream(filePath, FileMode.Create))
        {
            // Encode the bitmap as PNG (or use JpegEncoder for JPEG)
            bitmap2.Save(fs);
        }
   
    }

    private void Exit_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}