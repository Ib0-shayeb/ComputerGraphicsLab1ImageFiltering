using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;

namespace ComputerGraphicsLab1_ImageFiltering.Views;

public partial class MainWindow : Window
{
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
            imageControl.Source = new Bitmap(files[0].Path.AbsolutePath);
        }
    }
    

    private void Settings_Click(object sender, RoutedEventArgs e)
    {
        
    }

    private void Exit_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}