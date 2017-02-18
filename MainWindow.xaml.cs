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
using System.Windows.Threading;

namespace Gridworld_Heuristics
{

    public partial class MainWindow : Window
    {

        private delegate void generate_delegate();
        private delegate void calculate_delegate();
        private delegate void map_delegate();

        MainViewModel mvm = new MainViewModel();
        Grid myGrid;
        Grid myPath;
        private bool generating = false;
        private bool calculating = false;
        Naiive search;

        public MainWindow()
        {
            this.DataContext = mvm;
            myGrid = grid_creation();
            myPath = grid_creation();
            // Add the Grid as the Content of the Parent Window Object
            this.Map.Content = myGrid;
            this.Path.Content = myPath;
        }

        private Grid grid_creation()
        {
            Grid x;
            // Create the Grid
            x = new Grid();
            x.Width = 1280;
            x.Height = 960;
            x.HorizontalAlignment = HorizontalAlignment.Left;
            x.VerticalAlignment = VerticalAlignment.Top;
            x.ShowGridLines = false;

            InitializeComponent();

            // Define the Columns
            for (int i = 0; i < 160; i++)
            {
                ColumnDefinition col = new ColumnDefinition();
                col.Width = new GridLength((int)1280 / 160);
                x.ColumnDefinitions.Add(col);
            }

            // Define the Rows
            for (int i = 0; i < 120; i++)
            {
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength((int)1280 / 160);
                x.RowDefinitions.Add(row);
            }
            return x;
        }

        private void Visualize()
        {
            myGrid.Children.Clear();
            SolidColorBrush blackBrush = new SolidColorBrush();
            blackBrush.Color = Colors.Black;
            SolidColorBrush whiteBrush = new SolidColorBrush();
            whiteBrush.Color = Colors.White;
            SolidColorBrush blueBrush = new SolidColorBrush();
            blueBrush.Color = Colors.Aqua;
            SolidColorBrush grayBrush = new SolidColorBrush();
            grayBrush.Color = Colors.Gray;

            for (int i = 0; i < 120; i++)
            {
                for (int j = 0; j < 160; j++)
                {
                    if (mvm.world[i, j] == 1) continue;
                    Rectangle chunk = new Rectangle();
                    switch (mvm.world[i, j])
                    {
                        case 0://Black
                            chunk.Fill = blackBrush;
                            chunk.StrokeThickness = 0;
                            break;
                        case 2://Grey
                            chunk.Fill = grayBrush;
                            chunk.StrokeThickness = 0;
                            break;
                        case 3://White with blue stripe
                            chunk.Fill = whiteBrush;
                            chunk.Stroke = blueBrush;
                            chunk.StrokeThickness = 1;
                            break;
                        case 4://Grey with blue stripe
                            chunk.Fill = grayBrush;
                            chunk.Stroke = blueBrush;
                            chunk.StrokeThickness = 1;
                            break;

                        default://??
                            break;
                    }
                    Grid.SetColumn(chunk, j);
                    Grid.SetRow(chunk, i);
                    myGrid.Children.Add(chunk);
                }
            }
        }

        private void chunkClick(object sender, RoutedEventArgs e)
        {
            Button clicked = (Button)sender;
            int[] coordinates = (int[])clicked.Content;

            if (search.worldNodes[coordinates[0], coordinates[1]] != null)
            {
                worldNode chunky = search.worldNodes[coordinates[0], coordinates[1]];
                if (chunky != null)
                {
                    mvm.f = chunky.f;
                    mvm.g = chunky.g;
                    mvm.h = chunky.h;
                }
            }

        }
        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            if (!generating)
            {
                generating = true;
                Generate.Dispatcher.BeginInvoke(
                        DispatcherPriority.Normal,
                        new generate_delegate(gen));
            }
        }
        private void gen()
        {
            //Create the world.
            createWorld.generateWorld();
            //First load world 0.
            GridHelper.readInputs(0, mvm.world, mvm.startPairs, mvm.endPairs, mvm.hardPairs);
            //Visualize the world.
            Visualize();
            mvm.RefreshPairs();
            generating = false;
        }

        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            if (!calculating)
            {
                calculating = true;
                Generate.Dispatcher.BeginInvoke(
                        DispatcherPriority.Normal,
                        new calculate_delegate(calc));
            }
        }
        private void calc()
        {
            //Check what the algorithms dropdown selection is, and the weight for the heuristic.
            //Perform algorithm on the loaded world and one particular start/end pair.
            //Update Runtime
            recalculateAlgorithm();
            mvm.Runtime = search.sw.ElapsedTicks;
            calculating = false;
        }

        private void MapSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MapSelect.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new map_delegate(select));

        }
        private void select()
        {
            //Reload the world. Update StartEndPairs
            int selection = MapSelect.SelectedIndex;
            GridHelper.readInputs(selection, mvm.world, mvm.startPairs, mvm.endPairs, mvm.hardPairs);
            //Visualize the world.
            Visualize();
            mvm.RefreshPairs();
            //Recalculate Algorithm
            float wght;
            if (!float.TryParse(Weight.Text, out wght))
            {
                wght = 1;
            }
            search = new Naiive(mvm.world);
            //bool result = search.hSearch(Heuristic.SelectedIndex, Algo.SelectedIndex, wght, mvm.startPairs[0, 0], mvm.startPairs[0, 1], mvm.endPairs[0, 0], mvm.endPairs[0, 1]);

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
            pathBrush.Color = Colors.BlueViolet;
            SolidColorBrush startBrush = new SolidColorBrush();
            startBrush.Color = Colors.Green;
            SolidColorBrush endBrush = new SolidColorBrush();
            endBrush.Color = Colors.Red;

            SolidColorBrush transparent = new SolidColorBrush();
            transparent.Color = Colors.Transparent;


            int spi = StartEndPairs.SelectedIndex;
            if (spi < 0) spi = 0;
            int startx = mvm.startPairs[spi, 0];
            int starty = mvm.startPairs[spi, 1];
            int endx = mvm.endPairs[spi, 0];
            int endy = mvm.endPairs[spi, 1];

            //Rerun the algorithm, update runtime
            int pairIdx = StartEndPairs.SelectedIndex;
            if (pairIdx < 0) pairIdx = 0;
            float wght;
            if (!float.TryParse(Weight.Text, out wght))
            {
                wght = 1;
            }
            search.initAttr(Heuristic.SelectedIndex, Algo.SelectedIndex, wght, mvm.startPairs[pairIdx, 0], mvm.startPairs[pairIdx, 1],
                mvm.endPairs[pairIdx, 0], mvm.endPairs[pairIdx, 1]);
            search.hSearch();
            mvm.Expanded = search.expanded;
            myPath.Children.Clear();
            //Interpret search data.
            Button chunk;
            worldNode a = search.end;
            int path = 1;
            while (a.parent != null)
            {
                path++;
                chunk = new Button();
                if (a.x == endx && a.y == endy)
                    chunk.BorderBrush = endBrush;
                else
                    chunk.BorderBrush = pathBrush;
                chunk.Background = transparent;
                chunk.BorderThickness = new Thickness(1);
                chunk.Content = new int[] { a.x, a.y };
                chunk.Click += chunkClick;
                Grid.SetColumn(chunk, a.y);
                Grid.SetRow(chunk, a.x);

                a = a.parent;
                myPath.Children.Add(chunk);
            }
            mvm.PathLen = path;
            chunk = new Button();
            chunk.Background = transparent;
            chunk.BorderBrush = startBrush;
            chunk.BorderThickness = new Thickness(1);
            chunk.Content = new int[] { a.x, a.y };
            chunk.Click += chunkClick;
            Grid.SetColumn(chunk, a.y);
            Grid.SetRow(chunk, a.x);
            myPath.Children.Add(chunk);
            //Update Start/end pair appearance

        }

    }
}
