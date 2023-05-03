using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using static NoteWPF.MainWindow;

namespace NoteWPF.ViewModel
{
    public class NoteViewModel : INotifyPropertyChanged
    {
        private string _name = "Note";

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                Data.Data.Note.Name = value;
                OnPropertyChanged("Name");
            }
        }
        
        


        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        
        
        
    }

    public class FontBackColorComboBox : ObservableCollection<string>
    {
        public FontBackColorComboBox()
        {
            Add("White");
            Add("LightGray");
            Add("DarkGray");
            Add("Black");
            Add("Red");
            Add("Green");
            Add("Blue");
        }
    }
    
    public class FontHeightList : ObservableCollection<string>
    {
        public FontHeightList()
        {
            Add("8");
            Add("9");
            Add("10");
            Add("11");
            Add("12");
            Add("14");
            Add("16");
            Add("18");
            Add("20");
            Add("22");
            Add("24");
            Add("26");
            Add("28");
            Add("36");
            Add("48");
            Add("72");
        }
    }
    
}
