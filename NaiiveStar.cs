using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Priority_Queue;

namespace Gridworld_Heuristics
{

    class Naiive
    {
        SimplePriorityQueue<int[], float> fringe;
        int[,] closedList;
        List<int[]> parents;//[x,y,f,g,h]

        // Returns true if goal node is found. Returns false if no goal node is found.
        bool hSearch(int heuristic, int startx, int starty,
            int endx, int endy)
        {
            int currentx = startx;
            int currenty = starty;
            int g = 0;
            int h = 0; //Place heuristic in here.
            int[] fordq;

            fringe = new SimplePriorityQueue<int[], float>();
            closedList = new int[120, 160];
            parents = new List<int[]>();

            fringe.Enqueue(new int[2] { currentx, currenty }, g + h);

            while (fringe.Any())
            {
                fordq = fringe.Dequeue();
                currentx = fordq[0];
                currenty = fordq[1];
                if (currentx == endx && currenty == endy)
                {
                    return true;
                }
                closedList[currentx, currenty] = 1;
                for (int i = -1; i < 1; i++)
                {
                    for (int j = -1; j < 1; j++)
                        //We don't necessarily want to check when i=0 and j=0, but it won't affect anything.
                        //This is because the value in the closed list will be 1.
                        if (closedList[currentx + i, currenty + j] != 1)
                        {
                            //This line is questionable. If I call 'contains' on an integer array, while passing it a new integer array, will it make the right comparison?
                            if (!fringe.Contains(new int[] { currentx + i, currenty + j }))
                            {

                            }
                        }
                }
            }
            return false;
        }
        void UpdateVertex(int x, int y, int nx, int ny)
        {

        }

    }
}