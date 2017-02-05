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

        public MainWindow()
        {
            this.DataContext = mvm;
            InitializeComponent();
            // Create the Grid
            Grid myGrid = new Grid();
            myGrid.Width = 1600;
            myGrid.Height = 1200;
            myGrid.HorizontalAlignment = HorizontalAlignment.Left;
            myGrid.VerticalAlignment = VerticalAlignment.Top;
            myGrid.ShowGridLines = true;


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

            //below is example code on how to add items to a grid from Microsoft

            // Add the final text cell to the Grid
            TextBlock txt7 = new TextBlock();
            Double db3 = new Double();
            db3 = 150000;
            txt7.Text = db3.ToString();
            Grid.SetRow(txt7, 2);
            Grid.SetColumn(txt7, 2);
            

            // Add the TextBlock elements to the Grid Children collection
            myGrid.Children.Add(txt7);

            // Add the Grid as the Content of the Parent Window Object
            //this.Content = myGrid;
            this.Map = myGrid;
            //this.Show();
            GridHelper.initData(mvm);

        }

        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            //Create the world.
            Gridworld_Heuristics.createWorld.generateWorld();
            //First load world 0.
            GridHelper.readInputs(0, mvm.world, mvm.startPairs, mvm.endPairs, mvm.hardPairs);
            mvm.RefreshPairs();
        }

        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            //Check what the algorithms dropdown selection is, and the weight for the heuristic.
            //Perform algorithm on the loaded world and one particular start/end pair.
            //Update f, g, h, and Runtime.
            //For testing, manually update f g h, and try to refresh in window
            switch (Algo.SelectedIndex)
            {
                case 0:
                    //normal A*
                    mvm.f = 1;
                    mvm.g = 1;
                    mvm.h = 1;
                    break;
                case 1:
                    //Weighted A*
                    mvm.f = 2;
                    mvm.g = 2;
                    mvm.h = 2;
                    break;
                case 2:
                    //Uniform Cost Search
                    mvm.f = 3;
                    mvm.g = 3;
                    mvm.h = 3;
                    break;

                default:
                    mvm.f = 4;
                    mvm.g = 4;
                    mvm.h = 4;
                    break;
            }

        }

        private void MapSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Reload the world. Update StartEndPairs
            int selection = MapSelect.SelectedIndex;
            GridHelper.readInputs(selection, mvm.world, mvm.startPairs, mvm.endPairs, mvm.hardPairs);
            mvm.RefreshPairs();
            //Recalculate Algorithm
        }

        private void StartEndPairs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Rerun the algorithm, update fgh+runtime
            //Update Map display path highlights
            //Update Map displayed Start/goal pair
        }

        private void StartEndPairs_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }
    }
}
