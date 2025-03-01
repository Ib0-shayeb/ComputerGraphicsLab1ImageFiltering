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

    public void PerformAction()
        {
            SelectedFilters.Add(new GeneralFilter());
        }
    public void ClearSelectedFilters(){
        SelectedFilters.Clear();
    }

    public MainWindowViewModel()
        {
            ExampleCommand = ReactiveCommand.Create(PerformAction);

            SelectedFilters = new ObservableCollection<GeneralFilter>();

            var GeneralFilter = new GeneralFilter();
            GeneralFilter.AddFilter = PerformAction;

            Filters = new ObservableCollection<GeneralFilter>(new List<GeneralFilter>
            {
                GeneralFilter,

            });
            
        }
}
