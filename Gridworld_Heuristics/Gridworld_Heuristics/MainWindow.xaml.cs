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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        MainViewModel mvm = new MainViewModel();
        Grid myGrid;
        AStarSearch aSearch;

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


            //To Do: Interpret the loaded data values of one map as input, and visualize it.
            //Add colored blocks to the map based on the input, and make an onclick trigger to display heuristic information.
            //Create a button to calculate the algorithm and check a dropdown box for what kind of algorithm to perform
            //This button should load heuristic information into a datastructure and display runtimes
            //Create a dropdown to select different start/end pairs
            //Create a dropdown to select different maps
            //Use the Calculate button to reload the map for ease of programming
            
            

            // Add the Grid as the Content of the Parent Window Object
            //this.Content = myGrid;
            this.Map.Content= myGrid;
            //this.Show();
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
            GridPOS clicked2 = new global::GridPOS(coordinates[0], coordinates[1]);
            if (aSearch != null && aSearch.g.ContainsKey(clicked2))
            {
               
                
                mvm.g = aSearch.g[clicked2];
                mvm.h = aSearch.h[clicked2];
                mvm.f = mvm.g + mvm.h;
            }
            
            //Grab button coordinates
            //Look up coordinates in the algorithm results
            //Update fgh 
        }
        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            //Create the world.
            Gridworld_Heuristics.createWorld.generateWorld();
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
            aSearch = recalculateAlgorithm();
            mvm.Runtime = aSearch.sw.ElapsedTicks;

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

            AStarSearch aSearch = new AStarSearch(mvm.world, new int[] { mvm.startPairs[0, 0], mvm.startPairs[0, 1] },
                new int[] { mvm.endPairs[0, 0], mvm.endPairs[0, 1] }, 0);
            bool result = aSearch.AStarSearchEx();
        }

        private void StartEndPairs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            //Update Map display path highlights
            //Update Map displayed Start/goal pair

        }

        private void StartEndPairs_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }
        private AStarSearch recalculateAlgorithm()
        {
            SolidColorBrush whiteBrush = new SolidColorBrush();
            whiteBrush.Color = Colors.White;
            SolidColorBrush blackBrush = new SolidColorBrush();
            blackBrush.Color = Colors.Black;
            SolidColorBrush blueBrush = new SolidColorBrush();
            blueBrush.Color = Colors.Aqua;
            SolidColorBrush grayBrush = new SolidColorBrush();
            grayBrush.Color = Colors.Gray;
            SolidColorBrush pathBrush = new SolidColorBrush();
            grayBrush.Color = Colors.BlueViolet;

            //Rerun the algorithm, update fgh+runtime
            int pairIdx = StartEndPairs.SelectedIndex;

            AStarSearch aSearch = new AStarSearch(mvm.world, new int[] { mvm.startPairs[pairIdx, 0], mvm.startPairs[pairIdx, 1] },
                new int[] { mvm.endPairs[pairIdx, 0], mvm.endPairs[pairIdx, 1] }, Algo.SelectedIndex);
            bool result = aSearch.AStarSearchEx();
            //Interpret aSearch data.
            GridPOS pathPTR = aSearch.goal;
            while (aSearch.cameFrom.ContainsKey(pathPTR))
            {
                
                Button chunk = new Button();
                switch (mvm.world[pathPTR.row, pathPTR.col])
                {
                    case 0://Black
                        //Should never get here.
                        break;
                    case 1://White
                        chunk.Background = whiteBrush;
                        chunk.BorderThickness = new Thickness(0);
                        break;
                    case 2://Grey
                        chunk.Background = grayBrush;
                        chunk.BorderThickness = new Thickness(0);
                        break;
                    case 3://White with blue stripe
                        chunk.Background = whiteBrush;
                        chunk.BorderBrush = blueBrush;
                        chunk.BorderThickness = new Thickness(1);
                        break;
                    case 4://Grey with blue stripe
                        chunk.Background = grayBrush;
                        chunk.BorderBrush = blueBrush;
                        chunk.BorderThickness = new Thickness(1);
                        break;

                    default://??
                        break;
                }
                chunk.BorderBrush = pathBrush;
                chunk.BorderThickness = new Thickness(1);
                chunk.Content = new int[] { mvm.world[pathPTR.row, pathPTR.col] };
                chunk.Click += chunkClick;
                Grid.SetColumn(chunk, pathPTR.col);
                Grid.SetRow(chunk, pathPTR.row);
                myGrid.Children.Add(chunk);
                pathPTR = aSearch.cameFrom[pathPTR];
            }
            return aSearch;
        }


    }
}
