using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Gridworld_Heuristics
{

    public partial class MainWindow : Window
    {
        
        MainViewModel mvm = new MainViewModel();
        Grid myGrid;
        //AStarSearch aSearch;
        Naiive search;
        public MainWindow()
        {
            this.DataContext = mvm;
            InitializeComponent();
            // Create the Grid
            myGrid = new Grid();
            myGrid.Width = 1280;
            myGrid.Height = 960;
            myGrid.HorizontalAlignment = HorizontalAlignment.Left;
            myGrid.VerticalAlignment = VerticalAlignment.Top;
            myGrid.ShowGridLines = false;


            // Define the Columns
            for (int i = 0; i < 160; i++)
            {
                ColumnDefinition col = new ColumnDefinition();
                myGrid.ColumnDefinitions.Add(col);
            }

            // Define the Rows
            for (int i = 0; i < 120; i++)
            {
                RowDefinition row = new RowDefinition();
                myGrid.RowDefinitions.Add(row);
            }
            
            //To Do: create a button to calculate the algorithm and check a dropdown box for what kind of algorithm to perform
            
            // Add the Grid as the Content of the Parent Window Object
            this.Map.Content= myGrid;
            GridHelper.initData(mvm);

        }

        private void Visualize()
        {
            SolidColorBrush whiteBrush = new SolidColorBrush();
            whiteBrush.Color = Colors.White;
            SolidColorBrush blackBrush = new SolidColorBrush();
            blackBrush.Color = Colors.Black;
            SolidColorBrush blueBrush = new SolidColorBrush();
            blueBrush.Color = Colors.Aqua;
            SolidColorBrush grayBrush = new SolidColorBrush();
            grayBrush.Color = Colors.Gray;
            Thickness noBorder = new Thickness(0);
            Thickness thinBorder = new Thickness(1);
            
            for ( int i = 0; i < 120; i++)
            {
                for(int j = 0; j < 160; j++)
                {
                    Button chunk = new Button();
                    switch (mvm.world[i, j])
                    {
                        case 0://Black
                            chunk.Background= blackBrush;
                            chunk.BorderThickness = noBorder;
                            break;
                        case 1://White
                            chunk.Background = whiteBrush;
                            chunk.BorderThickness = noBorder;
                            break;
                        case 2://Grey
                            chunk.Background = grayBrush;
                            chunk.BorderThickness = noBorder;
                            break;
                        case 3://White with blue stripe
                            chunk.Background = whiteBrush;
                            chunk.BorderBrush= blueBrush;
                            chunk.BorderThickness = thinBorder;
                            break;
                        case 4://Grey with blue stripe
                            chunk.Background = grayBrush;
                            chunk.BorderBrush = blueBrush;
                            chunk.BorderThickness = thinBorder;
                            break;

                        default://??
                            break;
                    }
                    chunk.Content = new int[] { i, j };
                    chunk.Click += chunkClick;
                    Grid.SetColumn(chunk, j);
                    Grid.SetRow(chunk, i);
                    myGrid.Children.Add(chunk);
                }
            }
        }

        private void chunkClick(object sender, RoutedEventArgs e)
        {
            Button clicked = (Button)sender;
            int[] coordinates = (int[]) clicked.Content;

            if (search != null)
            {
                float[] result = search.parents.First(p => p[0] == coordinates[0] && p[1] == coordinates[1]);
                if(result != null)
                {
                    mvm.f = result[2];
                    mvm.g = result[3];
                    mvm.h = result[4];
                }
            }
            
            //Grab button coordinates
            //Look up coordinates in the algorithm results
            //Update fgh 
        }
        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            //Create the world.
            createWorld.generateWorld();
            //First load world 0.
            GridHelper.readInputs(0, mvm.world, mvm.startPairs, mvm.endPairs, mvm.hardPairs);
            //Visualize the world.
            Visualize();

            mvm.RefreshPairs();
        }

        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            //Check what the algorithms dropdown selection is, and the weight for the heuristic.
            //Perform algorithm on the loaded world and one particular start/end pair.
            //Update Runtime
            recalculateAlgorithm();
            mvm.Runtime = search.sw.ElapsedTicks;

        }

        private void MapSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Reload the world. Update StartEndPairs
            int selection = MapSelect.SelectedIndex;
            GridHelper.readInputs(selection, mvm.world, mvm.startPairs, mvm.endPairs, mvm.hardPairs);
            //Visualize the world.
            Visualize();
            mvm.RefreshPairs();
            //Recalculate Algorithm
            
            search = new Naiive(mvm.world);
            bool result = search.hSearch(Heuristic.SelectedIndex, mvm.startPairs[0, 0], mvm.startPairs[0, 1], mvm.endPairs[0, 0], mvm.endPairs[0, 1]);

        }

        private void StartEndPairs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Update Map display path highlights
            //Update Map displayed Start/goal pair
            recalculateAlgorithm();
            
        }

        //May not need this function.
        private void StartEndPairs_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }
        private void recalculateAlgorithm()
        {
            SolidColorBrush whiteBrush = new SolidColorBrush();
            whiteBrush.Color = Colors.White;
            SolidColorBrush blackBrush = new SolidColorBrush();
            blackBrush.Color = Colors.Black;
            SolidColorBrush chartreuseBrush = new SolidColorBrush();
            chartreuseBrush.Color = Colors.Chartreuse;
            SolidColorBrush beigeBrush = new SolidColorBrush();
            beigeBrush.Color = Colors.Beige;
            SolidColorBrush grayBrush = new SolidColorBrush();
            grayBrush.Color = Colors.Gray;
            SolidColorBrush pathBrush = new SolidColorBrush();
            grayBrush.Color = Colors.BlueViolet;
            SolidColorBrush startBrush = new SolidColorBrush();
            startBrush.Color = Colors.Green;
            SolidColorBrush endBrush = new SolidColorBrush();
            endBrush.Color = Colors.Red;

            //Rerun the algorithm, update runtime
            int pairIdx = StartEndPairs.SelectedIndex;

            search.hSearch(Heuristic.SelectedIndex, mvm.startPairs[pairIdx, 0], mvm.startPairs[pairIdx, 1],
                mvm.endPairs[pairIdx, 0], mvm.endPairs[pairIdx, 1]);

            //Interpret search data.
            foreach (float[] a in search.parents)//[x,y,f,g,h]
            {
                Button chunk = new Button();
                switch (mvm.world[(int)a[0], (int)a[1]])
                {
                    case 0://Black
                        //Should never get here.
                        break;
                    case 1://White
                        chunk.Background = whiteBrush;
                        break;
                    case 2://Grey
                        chunk.Background = grayBrush;
                        break;
                    case 3://white river
                        chunk.Background = chartreuseBrush;
                        break;
                    case 4://Grey river
                        chunk.Background = beigeBrush;
                        break;

                    default://??
                        break;
                }
                chunk.BorderBrush = pathBrush;
                chunk.BorderThickness = new Thickness(1);
                chunk.Content = new int[] { (int)a[0], (int)a[1] };
                chunk.Click += chunkClick;
                Grid.SetColumn(chunk, (int)a[0]);
                Grid.SetRow(chunk, (int)a[1]);
                myGrid.Children.Add(chunk);
            }
            //Update Start/end pair appearance
            int spi = StartEndPairs.SelectedIndex;
            int startx = mvm.startPairs[spi, 0];
            int starty = mvm.startPairs[spi, 1];
            int endx = mvm.endPairs[spi, 0];
            int endy = mvm.endPairs[spi, 1];

            List<int> sep = new List<int> { mvm.world[startx, starty], mvm.world[endx, endy] };
            int i = 0;
            foreach (int a in sep)
            {
                Button chunk = new Button();
                switch (a)
                {
                    case 0://Black
                           //Should never get here.
                        break;
                    case 1://White
                        chunk.Background = whiteBrush;
                        break;
                    case 2://Grey
                        chunk.Background = grayBrush;
                        break;
                    case 3://white river
                        chunk.Background = chartreuseBrush;
                        break;
                    case 4://Grey river
                        chunk.Background = beigeBrush;
                        break;

                    default://??
                        break;
                }
                chunk.BorderBrush = startBrush;
                chunk.BorderThickness = new Thickness(1);
                if (i == 0)
                {
                    chunk.Content = new int[] { starty, startx };
                    Grid.SetColumn(chunk, starty);
                    Grid.SetRow(chunk, startx);

                }
                else
                {
                    chunk.Content = new int[] { endy, endx};
                    Grid.SetColumn(chunk, endy);
                    Grid.SetRow(chunk, endx);

                }
                chunk.Click += chunkClick;
                myGrid.Children.Add(chunk);
                i++;
            }
        }
            
    }
}
