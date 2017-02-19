using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gridworld_Heuristics
{
    class GridHelper
    {


        public static void readInputs(int worldNumber, int[,] world, int[,] startPairs, int[,] endPairs, int[,] hardPairs)
        {
            //Read file input

            string filename = $"C:\\Users\\Public\\Gridworld_Heuristics\\world_{worldNumber}";
            if (!File.Exists(filename))
            {
                System.Windows.MessageBox.Show("Please generate worlds before selection.");
                return;
            }
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

