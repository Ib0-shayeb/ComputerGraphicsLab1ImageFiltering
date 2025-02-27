using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Interactivity;

namespace ComputerGraphicsLab1_ImageFiltering
{
    public abstract class GeneralFilter
    {
        public abstract string Name { get;}
        public abstract void AddFilter(object? sender, RoutedEventArgs e);
        public abstract byte[] ApplyFilter(byte[] input);
    }
    public class InversionFilter : GeneralFilter
    {
        public override string Name {get;}


        public InversionFilter(){
            Name = "Inversion Filter";
        }
        public override void AddFilter(object? sender, RoutedEventArgs e)
        {
            
        }
        public override byte[] ApplyFilter(byte[] input){
            return [];
        }
    }
}