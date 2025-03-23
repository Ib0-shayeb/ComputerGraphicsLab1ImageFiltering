using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive;
using Avalonia.Controls;
using ReactiveUI;
using System.Linq;
using System.Drawing;

namespace ComputerGraphicsLab1_ImageFiltering.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public string Greeting { get; } = "Welcome to Avalonia!";
    public ObservableCollection<GeneralFilter> Filters { get; set; }
    public ObservableCollection<GeneralFilterListItem> SelectedFilters { get; set; }

    public ReactiveCommand<Unit, Unit> ExampleCommand { get; }
  
    private GeneralFilterListItem? _selectedItem;
    public GeneralFilterListItem? SelectedItem
    {
        get => _selectedItem;
        set {
            if (_selectedItem != value)
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }
    }

    public void AddInversionFilter()
    {
        SelectedFilters.Add(new GeneralFilterListItem(new GeneralFilter(FilterType.Inversion), SelectedFilters.Count));
    }
    public void AddBrightnessCorrectionFilter()
    {
        SelectedFilters.Add(new GeneralFilterListItem(new GeneralFilter(FilterType.BrightnessCorrection), SelectedFilters.Count));
    }
    public void AddContrastEnhancementFilter()
    {
        SelectedFilters.Add(new GeneralFilterListItem(new GeneralFilter(FilterType.ContrastEnhancement), SelectedFilters.Count));
    }
    public void AddGammaCorrectionFilter()
    {
        SelectedFilters.Add(new GeneralFilterListItem(new GeneralFilter(FilterType.GammaCorrection), SelectedFilters.Count));
    }
    public void AddBlurFilter()
    {
        SelectedFilters.Add(new GeneralFilterListItem(new GeneralFilter(FilterType.Blur), SelectedFilters.Count));
    }
    public void AddGausianBlurFilter()
    {
        SelectedFilters.Add(new GeneralFilterListItem(new GeneralFilter(FilterType.GausianBlur), SelectedFilters.Count));
    }
    public void AddSharpenFilter()
    {
        SelectedFilters.Add(new GeneralFilterListItem(new GeneralFilter(FilterType.Sharpen), SelectedFilters.Count));
    }
    public void AddEdgeDetectionFilter()
    {
        SelectedFilters.Add(new GeneralFilterListItem(new GeneralFilter(FilterType.EdgeDetection), SelectedFilters.Count));
    }
    public void AddEmbossFilter()
    {
        SelectedFilters.Add(new GeneralFilterListItem(new GeneralFilter(FilterType.Emboss), SelectedFilters.Count));
    }
    public void AddErrosionFilter()
    {
        SelectedFilters.Add(new GeneralFilterListItem(new GeneralFilter(FilterType.Errosion), SelectedFilters.Count));
    }
    public void AddDilationFilter()
    {
        SelectedFilters.Add(new GeneralFilterListItem(new GeneralFilter(FilterType.Dilation), SelectedFilters.Count));
    }
    public void AddGrayScaleFilter()
    {
        SelectedFilters.Add(new GeneralFilterListItem(new GeneralFilter(FilterType.GrayScale), SelectedFilters.Count));
    }
    public void AddRandomDitheringFilter()
    {
        SelectedFilters.Add(new GeneralFilterListItem(new GeneralFilter(FilterType.RandomDithering), SelectedFilters.Count));
    }
    public void AddKMeansFilter()
    {
        SelectedFilters.Add(new GeneralFilterListItem(new GeneralFilter(FilterType.KMeans), SelectedFilters.Count));
    }
    public void ClearSelectedFilters(){
        SelectedFilters.Clear();
    }
    public void SelectCanvasFilterPolyline(GeneralFilterListItem canvas){
        foreach (var item in SelectedFilters)
        {
            item.selected = false;
        }
        canvas.selected = true;
        
        var f = SelectedFilters.FirstOrDefault(c => c.selected);
        if(f == null) return;
        SelectedItem = f;       
    }

    public MainWindowViewModel()
        {
            SelectedFilters = new ObservableCollection<GeneralFilterListItem>();

            var InversionFilter = new GeneralFilter(FilterType.Inversion);
            InversionFilter.AddFilter = AddInversionFilter;
            var BrightnessCorrectionFilter = new GeneralFilter(FilterType.BrightnessCorrection);
            BrightnessCorrectionFilter.AddFilter = AddBrightnessCorrectionFilter;
            var ContrastEnhancementFilter = new GeneralFilter(FilterType.ContrastEnhancement);
            ContrastEnhancementFilter.AddFilter = AddContrastEnhancementFilter;
            var GammaCorrectionFilter = new GeneralFilter(FilterType.GammaCorrection);
            GammaCorrectionFilter.AddFilter = AddGammaCorrectionFilter;
            var BlurFilter = new GeneralFilter(FilterType.Blur);
            BlurFilter.AddFilter = AddBlurFilter;
            var GausianBlurFilter = new GeneralFilter(FilterType.GausianBlur);
            GausianBlurFilter.AddFilter = AddGausianBlurFilter;
            var SharpenFilter = new GeneralFilter(FilterType.Sharpen);
            SharpenFilter.AddFilter = AddSharpenFilter;
            var EdgeDetectionFilter = new GeneralFilter(FilterType.EdgeDetection);
            SharpenFilter.AddFilter = AddEdgeDetectionFilter;
            var EmbossFilter = new GeneralFilter(FilterType.Emboss);
            SharpenFilter.AddFilter = AddEmbossFilter;
            var ErrosionFilter = new GeneralFilter(FilterType.Errosion);
            ErrosionFilter.AddFilter = AddErrosionFilter;
            var DilationFilter = new GeneralFilter(FilterType.Dilation);
            DilationFilter.AddFilter = AddDilationFilter;
            var GrayScaleFilter = new GeneralFilter(FilterType.GrayScale);
            GrayScaleFilter.AddFilter = AddGrayScaleFilter;
            var RandomDithering = new GeneralFilter(FilterType.RandomDithering);
            RandomDithering.AddFilter = AddRandomDitheringFilter;
            var KMeans = new GeneralFilter(FilterType.KMeans);
            KMeans.AddFilter = AddKMeansFilter;

            Filters = new ObservableCollection<GeneralFilter>(new List<GeneralFilter>
            {
                InversionFilter,
                BrightnessCorrectionFilter,
                ContrastEnhancementFilter,
                GammaCorrectionFilter,
                
                BlurFilter,
                GausianBlurFilter,
                SharpenFilter,
                EmbossFilter,
                EdgeDetectionFilter,

                ErrosionFilter,
                DilationFilter,

                GrayScaleFilter,
                RandomDithering,
                KMeans
            });
            
        }
    
}
