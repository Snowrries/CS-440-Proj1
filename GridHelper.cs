using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gridworld_Heuristics
{
    class GridHelper
    {
        public static void initData(MainViewModel mvm)
        {
            //Call this after data is read in, and properties are populated.
            //mvm.MapList = new ObservableCollection<string> { "World_0", "World_1", "World_2", "World_3", "World_4", "World_5",
            //    "World_6", "World_7", "World_8", "World_9", "World_10" };
            //Pairlist should update automatically once maps are generated or a new map is selected.
            //mvm.Algos = new ObservableCollection<string> { "A*", "Weighted A*", "Uniform Cost" };
        }


        public static void readInputs(int worldNumber, int[,] world, int[,] startPairs, int[,] endPairs, int[,] hardPairs)
        {
            //Read file input

            string filename = $"C:\\Users\\Public\\Gridworld_Heuristics\\world_{worldNumber}";
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
                hardPairs[k - 2, 0] = Convert.ToInt32(xy[0]);
                hardPairs[k - 2, 1] = Convert.ToInt32(xy[1]);
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
    }
}

