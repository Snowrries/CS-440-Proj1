﻿using System;
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
        Button[,] buttonlist;
        Rectangle[,] backgroundList;
        //AStarSearch aSearch;
        Naiive search;
        public MainWindow()
        {
            this.DataContext = mvm;
            // Create the Grid
            myGrid = new Grid();
            myGrid.Width = 1280;
            myGrid.Height = 960;
            myGrid.HorizontalAlignment = HorizontalAlignment.Left;
            myGrid.VerticalAlignment = VerticalAlignment.Top;
            myGrid.ShowGridLines = false;

            InitializeComponent();

            // Define the Columns
            for (int i = 0; i < 160; i++)
            {
                ColumnDefinition col = new ColumnDefinition();
                col.Width = new GridLength((int)1280 / 160);
                myGrid.ColumnDefinitions.Add(col);
            }

            // Define the Rows
            for (int i = 0; i < 120; i++)
            {
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength((int)1280 / 160);
                myGrid.RowDefinitions.Add(row);
            }
            
            // Add the Grid as the Content of the Parent Window Object
            this.Map.Content= myGrid;

        }

        private void Visualize()
        {
            //buttonlist = new Button[120, 160];
            backgroundList = new Rectangle[120, 160];
            SolidColorBrush whiteBrush = new SolidColorBrush();
            whiteBrush.Color = Colors.White;
            SolidColorBrush blackBrush = new SolidColorBrush();
            blackBrush.Color = Colors.Black;
            SolidColorBrush blueBrush = new SolidColorBrush();
            blueBrush.Color = Colors.Aqua;
            SolidColorBrush grayBrush = new SolidColorBrush();
            grayBrush.Color = Colors.Gray;
            
            for ( int i = 0; i < 120; i++)
            {
                for(int j = 0; j < 160; j++)
                {
                    Rectangle chunk = new Rectangle();
                    backgroundList[i, j] = chunk;
                    switch (mvm.world[i, j])
                    {
                        case 0://Black
                            chunk.Fill = blackBrush;
                            chunk.StrokeThickness = 0;
                            break;
                        case 1://White
                            chunk.Fill = whiteBrush;
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
            float wght;
            if (!float.TryParse(Weight.Text, out wght))
            {
                wght = 1;
            }
            search = new Naiive(mvm.world);
            bool result = search.hSearch(Heuristic.SelectedIndex, Algo.SelectedIndex, wght, mvm.startPairs[0, 0], mvm.startPairs[0, 1], mvm.endPairs[0, 0], mvm.endPairs[0, 1]);

        }

        private void StartEndPairs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Update Map display path highlights
            //Update Map displayed Start/goal pair
            if (search == null) return;
            recalculateAlgorithm();
            mvm.Runtime = search.sw.ElapsedTicks;
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
            pathBrush.Color = Colors.BlueViolet;
            SolidColorBrush startBrush = new SolidColorBrush();
            startBrush.Color = Colors.Green;
            SolidColorBrush endBrush = new SolidColorBrush();
            endBrush.Color = Colors.Red;
            Visualize();
            buttonlist = new Button[120, 160];
            //Rerun the algorithm, update runtime
            int pairIdx = StartEndPairs.SelectedIndex;
            if (pairIdx < 0) pairIdx = 0;
            float wght;
            if (!float.TryParse(Weight.Text, out wght)) {
                wght = 1;
            }
            search.hSearch(Heuristic.SelectedIndex, Algo.SelectedIndex, wght, mvm.startPairs[pairIdx, 0], mvm.startPairs[pairIdx, 1],
                mvm.endPairs[pairIdx, 0], mvm.endPairs[pairIdx, 1]);

            //Interpret search data.
            worldNode a = search.end;
            while (a.parent != null)
            {
                a = a.parent;
                Button chunk = new Button();
                myGrid.Children.Remove(backgroundList[a.x, a.y]);
                buttonlist[a.x, a.y] = chunk;
                switch (mvm.world[a.x, a.y])
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
                chunk.Content = new int[] { a.x, a.y } ;
                chunk.Click += chunkClick;
                Grid.SetColumn(chunk, a.y);
                Grid.SetRow(chunk, a.x);

                myGrid.Children.Add(chunk);
            }
            //Update Start/end pair appearance
            int spi = StartEndPairs.SelectedIndex;
            if (spi < 0) spi = 0;
            int startx = mvm.startPairs[spi, 0];
            int starty = mvm.startPairs[spi, 1];
            int endx = mvm.endPairs[spi, 0];
            int endy = mvm.endPairs[spi, 1];

            List<int> sep = new List<int> { mvm.world[startx, starty], mvm.world[endx, endy] };
            myGrid.Children.Remove(buttonlist[startx, starty]);
            myGrid.Children.Remove(buttonlist[endx, endy]);
            int i = 0;
            foreach (int ba in sep)
            {
                Button chunk = new Button();
                switch (ba)
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
                if (i == 0)
                {
                    chunk.BorderBrush = startBrush;
                    chunk.BorderThickness = new Thickness(1);
                    chunk.Content = new int[] { startx, starty };
                    buttonlist[startx, starty] = chunk;
                    Grid.SetColumn(chunk, starty);
                    Grid.SetRow(chunk, startx);

                }
                else
                {
                    chunk.BorderBrush = endBrush;
                    chunk.BorderThickness = new Thickness(1);
                    chunk.Content = new int[] { endx, endy};
                    buttonlist[endx, endy] = chunk;
                    Grid.SetColumn(chunk, endy );
                    Grid.SetRow(chunk, endx);

                }
                chunk.Click += chunkClick;
                myGrid.Children.Add(chunk);
                i++;
            }
        }
            
    }
}
