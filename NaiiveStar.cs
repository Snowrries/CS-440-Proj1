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
        MainViewModel mvm;
        SimplePriorityQueue<int[], float> fringe;
        int[,] closedList;
        float[,] g;
        List<float[]> parents;//[x,y,f,g,h]

        Naiive(MainViewModel pmvm)
        {
            mvm = pmvm;
        }


        // Returns true if goal node is found. Returns false if no goal node is found.
        bool hSearch(int heuristic, int startx, int starty,
            int endx, int endy)
        {
            int cx = startx;
            int cy = starty;
            int nx, ny;
            g[startx, starty] = 0;
            float h = computeHeuristic(heuristic, cx, cy, endx, endy); //Place heuristic in here.
            int[] fordq;

            fringe = new SimplePriorityQueue<int[], float>();
            closedList = new int[120, 160];
            parents = new List<float[]>();

            fringe.Enqueue(new int[2] { cx, cy }, g[startx,starty] + h);

            while (fringe.Any())
            {
                fordq = fringe.Dequeue();
                cx = fordq[0];
                cy = fordq[1];
                if (cx == endx && cy == endy)
                {
                    return true;
                }
                closedList[cx, cy] = 1;
                for (int i = -1; i < 1; i++)
                {
                    for (int j = -1; j < 1; j++)
                    {
                        nx = cx + i;
                        ny = cy + j;
                        //We don't necessarily want to check when i=0 and j=0, but it won't affect anything.
                        //This is because the value in the closed list will be 1.
                        if (closedList[nx,ny] != 1)
                        {
                            if (!fringe.Any(p => p[0] == nx || p[1] == ny))
                            {
                                g[nx, ny] = 30000; // use 30,000 to refer to infinity
                                //Parent of s' = NULL; everyone is default set to null
                                //s' never has a parent set, so it's implicitly null
                            }
                            UpdateVertex(cx, cy, nx, ny,heuristic,endx,endy);
                        }
                    }
                }
            }
            return false;
        }

        public float computeHeuristic(int heuristic, int cx, int cy, int endx, int endy)
        {
            return 0;
        }

        void UpdateVertex(int x, int y, int nx, int ny, int heuristic, int endx, int endy)
        {
            float css = cost(x, y, nx, ny);
            float h = computeHeuristic(heuristic, x, y, endx, endy);
            float cg = g[x, y];
            if (cg + css  < g[nx, ny])
            {
                g[nx, ny] = cg + css;
                parents.Add(new float[] { x, y, cg+h, cg, h });//x,y,f,g,h
            }
        }
        float cost(int x, int y, int nx, int ny)
        {
            int start = mvm.world[x, y];
            int next = mvm.world[nx, ny];
            float cost = 0;
            float multiplier = 1;
            //Assume start and next are both valid (not impassable)
            if(start == 0 || next == 0)
            {
                return 0;
            }
            //Check if both are highways
            if (start == 3 || start == 4)
                if (next == 3 || next == 4)
                    multiplier = .25f;
            //Normalize start and next;
            if (start == 3)
                start = 1;
            else if (start == 4)
                start = 2;
            if (next == 3)
                next = 1;
            else if (next == 4)
                next = 2;
            
            //Check if we're moving diagonally or horizontall/vertically
            int check = Math.Abs(x - nx) + Math.Abs(y - ny);
            if (check > 1)
            {
                if (start == 1)
                {
                    if (next == 1)
                        cost = 1.41421356237f; //sqrt(2)
                    else if (next == 2)
                        cost = 2.12132034356f; //[sqrt(2) + sqrt(8)]/2
                }
                else if (start == 2)
                {
                    if (next == 1)
                        cost = 2.12132034356f; //[sqrt(2) + sqrt(8)]/2
                    else if (next == 2)
                        cost = 2.82842712475f; //sqrt(8)
                }
            }
            else
            {
                if (start == 1)
                {
                    if (next == 1)
                        cost = 1f;
                    else if (next == 2)
                        cost = 1.5f;
                }
                else if (start == 2)
                {
                    if (next == 1)
                        cost = 1.5f;
                    else if (next == 2)
                        cost = 2f;
                }
            }
            return cost * multiplier;
        }

    }
}