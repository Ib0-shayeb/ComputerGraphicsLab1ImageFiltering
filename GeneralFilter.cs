using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Interactivity;
using ReactiveUI;

namespace ComputerGraphicsLab1_ImageFiltering
{

    public class GeneralFilter :  ReactiveObject
    {
        public  string Name {get;}
        public  Action? AddFilter { get; set; }

        public GeneralFilter(){
            Name = "General Filter";
        }
        public  byte[] ApplyFilter(byte[] input){
            return [];
        }
        public void ExecuteAddFilter()
        {
            AddFilter?.Invoke();
        }
    }
}