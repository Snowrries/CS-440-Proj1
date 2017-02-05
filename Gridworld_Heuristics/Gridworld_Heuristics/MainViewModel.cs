using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MainViewModel : INotifyPropertyChanged
{

    private ObservableCollection<string> mapList = new ObservableCollection<string> { "World_0", "World_1", "World_2", "World_3", "World_4", "World_5",
                "World_6", "World_7", "World_8", "World_9", "World_10" };
    public ObservableCollection<string> MapList {
        get
        {
            return mapList;
        }
        set { }
    }
    public ObservableCollection<string> PairList { get; set; }

    public string ItemToAdd { get; set; }
    
    public int f { get; set; }
    public int g { get; set; }
    public int h { get; set; }

    public int[,] startPairs { get; set; }
    public int[,] endPairs { get; set; }
    public int[,] hardPairs = new int[8, 2];
    public int[,] world = new int[120, 160];

    public int tilex { get; set; } // Holds the column index of the clicked button

    public int tiley { get; set; }// Row index of clicked button
    
    

    public void RefreshPairs()
    {
        this.PairList.Clear();
        for (int i = 0; i < 10; i++)
            this.PairList.Add($"{startPairs[i, 0]},{startPairs[i, 1]} | {endPairs[i, 0]},{endPairs[i, 1]}");
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
