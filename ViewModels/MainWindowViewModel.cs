using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ComputerGraphicsLab1_ImageFiltering.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public string Greeting { get; } = "Welcome to Avalonia!";
    public ObservableCollection<GeneralFilter> Filters { get; set; }
    public ObservableCollection<GeneralFilter> SelectedFilters { get; set; }

    public MainWindowViewModel()
        {
            Filters = new ObservableCollection<GeneralFilter>(new List<GeneralFilter>
            {
                new InversionFilter(),
                new InversionFilter(),
            });
            SelectedFilters = new ObservableCollection<GeneralFilter>();
        }
}
