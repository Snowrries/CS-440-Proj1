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
        private Window mainWindow;

        public MainWindow()
        {

            int[,] world = new int[120, 160];
            int[,] startPairs = new int[10, 2];
            int[,] endPairs = new int[10, 2];
            int[,] hardPairs = new int[8, 2];

            InitializeComponent();

            // Create the Grid
            Grid myGrid = new Grid();
            myGrid.Width = 1600;
            myGrid.Height = 1200;
            myGrid.HorizontalAlignment = HorizontalAlignment.Left;
            myGrid.VerticalAlignment = VerticalAlignment.Top;
            myGrid.ShowGridLines = true;

            // Define the Columns
            for(int i = 0; i < 160; i++)
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
            //Read file input
            for (int i = 0; i < 5; i++)
            {
                string filename = $"C:\\Users\\Public\\Gridworld_Heuristics\\world_{i}";
                string[] lines = System.IO.File.ReadAllLines(filename);
                string[] xy;

                string[] coordinatePairs = lines[0].Split('|');
                for (int j = 0; j < 10; j++)
                {
                    xy = coordinatePairs[j].Split(',');
                    startPairs[j, 0] = Convert.ToInt32(xy[0]);
                    startPairs[j, 1] = Convert.ToInt32(xy[1]);
                }
                coordinatePairs = lines[1].Split('|');
                for (int j = 0; j < 10; j++)
                {
                    xy = coordinatePairs[j].Split(',');
                    endPairs[j, 0] = Convert.ToInt32(xy[0]);
                    endPairs[j, 1] = Convert.ToInt32(xy[1]);
                }

                for (int k = 2; k < 10; k++)
                {
                    xy = lines[k].Split(',');
                    endPairs[k-2, 0] = Convert.ToInt32(xy[0]);
                    endPairs[k-2, 1] = Convert.ToInt32(xy[1]);
                }

                for (int j = 0; j < 120; j++)
                {
                    xy = lines[j + 10].Split(',');
                    for (int k = 0; k < 160; k++)
                    {
                        switch (xy[k])
                        {
                            case "0":
                                world[j, k] = 0;
                                break;
                            case "1":
                                world[j, k] = 1;
                                break;
                            case "2":
                                world[j, k] = 2;
                                break;
                            case "a":
                                world[j, k] = 3;
                                break;
                            case "b":
                                world[j, k] = 4;
                                break;
                        }
                    }
                }
                
            }

            //To Do: Interpret the loaded data values as input, and visualize it.
            //Consider loading only one map, then reperforming all of these calculations whenever a new map is selected.
            //Add colored blocks to the map based on the input, and make a trigger on click to display heuristic information.
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
            this.Content = myGrid;
            //this.Show();

        }
    }
}
