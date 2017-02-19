using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gridworld_Heuristics
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            mapList = new ObservableCollection<string> { "World_0", "World_1", "World_2", "World_3", "World_4"};
            algos = new ObservableCollection<string> { "A*", "Weighted A*", "Uniform Cost", "Sequential Search"};
            heuristic = new ObservableCollection<string> { "Euclidian", "Manhattan", "Manhattan/4", "Octal/4", "Chebyshev/4" };
            pairList = new ObservableCollection<string> { "p1", "p2", "p3", "p4", "p5", "p6", "p7", "p8", "p9", "p10" };
            status1 = "...";
            status2 = "...";
            status3 = "...";
        }

        public ObservableCollection<string> heuristic { get; private set; }


        private string s1;
        public string status1 { get { return s1; } set { s1 = value; OnPropertyChanged("status1"); } }
        private string s2;
        public string status2 { get { return s2; } set { s2 = value; OnPropertyChanged("status2"); } }
        private string s3;
        public string status3 { get { return s3; } set { s3 = value; OnPropertyChanged("status3"); } }

        private ObservableCollection<string> mapList;
        public ObservableCollection<string> MapList
        {
            get
            {
                return mapList;
            }
            set
            {
                if (value != mapList)
                {
                    mapList = value;
                    OnPropertyChanged("MapList");
                }

            }
        }
        private ObservableCollection<string> pairList;
        public ObservableCollection<string> PairList
        {
            get
            {
                return pairList;
            }
            set
            {
                if (pairList != value)
                {
                    RefreshPairs();
                    OnPropertyChanged("PairList");
                }
            }
        }
        private ObservableCollection<string> algos;
        public ObservableCollection<string> Algos
        {
            get
            {
                return algos;
            }
            set
            {
                if (value != algos)
                {
                    algos = value;
                    OnPropertyChanged("Algos");
                }
            }
        }
        
        private float f_;
        public float f {
            get { return f_; }
            set
            {
                f_ = value;
                OnPropertyChanged("f");
            }
        }
        private float g_;
        public float g { get { return g_; } set { g_ = value; OnPropertyChanged("g"); } }
        private float h_;
        public float h { get { return h_; } set { h_ = value; OnPropertyChanged("h"); } }
        private float runtime;
        public float Runtime { get { return runtime; } set { runtime = value; OnPropertyChanged("Runtime"); } }
        private float pathlen;
        public float PathLen { get { return pathlen; } set { pathlen = value; OnPropertyChanged("PathLen"); } }
        private float expanded;
        public float Expanded { get { return expanded; } set { expanded = value; OnPropertyChanged("Expanded"); } }

        public int[,] startPairs = new int[10, 2];
        public int[,] endPairs = new int[10, 2];
        public int[,] hardPairs = new int[8, 2];
        public int[,] world = new int[120, 160];
        
        public void RefreshPairs()
        {
            pairList.Clear();
            for (int i = 0; i < 10; i++)
                pairList.Add($"{startPairs[i, 0]},{startPairs[i, 1]} | {endPairs[i, 0]},{endPairs[i, 1]}");

            OnPropertyChanged("PairList");
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
}