using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive;
using Avalonia.Controls;
using ReactiveUI;

namespace ComputerGraphicsLab1_ImageFiltering.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public string Greeting { get; } = "Welcome to Avalonia!";
    public ObservableCollection<GeneralFilter> Filters { get; set; }
    public ObservableCollection<GeneralFilter> SelectedFilters { get; set; }

    public ReactiveCommand<Unit, Unit> ExampleCommand { get; }

    public void AddInversionFilter()
    {
        SelectedFilters.Add(new GeneralFilter(FilterType.Inversion));
    }
    public void AddBrightnessCorrectionFilter()
    {
        SelectedFilters.Add(new GeneralFilter(FilterType.BrightnessCorrection));
    }
    public void AddContrastEnhancementFilter()
    {
        SelectedFilters.Add(new GeneralFilter(FilterType.ContrastEnhancement));
    }
    public void AddGammaCorrectionFilter()
    {
        SelectedFilters.Add(new GeneralFilter(FilterType.GammaCorrection));
    }
    public void AddBlurFilter()
    {
        SelectedFilters.Add(new GeneralFilter(FilterType.Blur));
    }
    public void AddGausianBlurFilter()
    {
        SelectedFilters.Add(new GeneralFilter(FilterType.GausianBlur));
    }
    public void AddSharpenFilter()
    {
        SelectedFilters.Add(new GeneralFilter(FilterType.Sharpen));
    }
    public void AddEdgeDetectionFilter()
    {
        SelectedFilters.Add(new GeneralFilter(FilterType.EdgeDetection));
    }
    public void AddEmbossFilter()
    {
        SelectedFilters.Add(new GeneralFilter(FilterType.Emboss));
    }
    public void ClearSelectedFilters(){
        SelectedFilters.Clear();
    }

    public MainWindowViewModel()
        {
            SelectedFilters = new ObservableCollection<GeneralFilter>();

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
            });
            
        }
    
}
