﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MainViewModel : INotifyPropertyChanged
{
    public ObservableCollection<string> MapList { get; set; }

    public string ItemToAdd { get; set; }

    private string selectedItem;

    public string SelectedItem
    {
        get { return selectedItem; }
        set
        {
            selectedItem = value;
            OnPropertyChanged("SelectedItem");
        }
    }

    public void AddNewItem()
    {
        this.MapList.Add(this.ItemToAdd);
        this.SelectedItem = this.ItemToAdd;
    }


    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        if (this.PropertyChanged != null)
        {
            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
