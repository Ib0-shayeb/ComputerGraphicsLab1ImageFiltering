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

namespace ComputerGraphicsLab1_ImageFiltering.Views;

public partial class MainWindow : Window
{
    public byte[] OriginalPixelData;
    public byte[] pixelArray;
    public int pixelWidth;
    public int pixelHeight;
    public int stride;
    public MainWindow()
    {
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
        pixelArray = new byte[OriginalPixelData.Length];
        Array.Copy(OriginalPixelData, pixelArray, OriginalPixelData.Length);

        var viewModel = this.DataContext as MainWindowViewModel;

        foreach (var filter in viewModel.SelectedFilters)
        {
            filter.ApplyFilter(pixelArray, pixelWidth, pixelHeight, stride);
        }

        var imageControl2 = this.FindControl<Image>("ImageControll2");
        Vector dpi = new Vector(96, 96);
        var bitmap = new WriteableBitmap(
            new PixelSize(pixelWidth, pixelHeight),
            dpi,
            PixelFormat.Bgra8888,
            AlphaFormat.Premul);

        using (var frameBuffer = bitmap.Lock())
        {
            Marshal.Copy(pixelArray, 0, frameBuffer.Address, pixelArray.Length);
        }
        imageControl2.Source = bitmap;
    }
    private void Settings_Click(object sender, RoutedEventArgs e)
    {
        
    }

    private void Exit_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}