using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Priority_Queue;
using System.Diagnostics;

namespace Gridworld_Heuristics
{
    class worldNode
    {
        public worldNode parent;
        public int x, y;
        public float f, g, h;

        public worldNode(int x_, int y_)
        {
            x = x_;
            y = y_;
            f = 0;
            g = 30000;
            h = 0;
        }
    }


    class Naiive
    {
        
        int[,] world;
        public worldNode[,] worldNodes;
        SimplePriorityQueue<worldNode, float> fringe;
        List<worldNode> closedList;
        public worldNode end;
//        public LinkedList<worldNode> parents;//[x,y,f,g,h]
        public Stopwatch sw;

        public Naiive(int[,] world)
        {
            this.world = world;
            worldNodes = new worldNode[120,160];
        }
        
        // Returns true if goal node is found. Returns false if no goal node is found.
        // To do: Add options to select algorithms. Track runtime.
        public bool hSearch(int heuristic, int startx, int starty,
            int endx, int endy)
        {
            sw = Stopwatch.StartNew();
            /*for (int i = 0; i < 120; i++)
            {
                for (int j = 0; j < 160; j++)
                {
                    worldNodes[i, j] = new worldNode(i, j);
                }
            }*/
            worldNode currentNode = new worldNode(startx,starty);
            worldNodes[startx, starty] = currentNode;
            worldNodes[endx, endy] = new worldNode(endx, endy);
            end = worldNodes[endx, endy];

            worldNode nextNode;
            currentNode.g = 0;

            int cx = startx;
            int cy = starty;
            int nx, ny;
            float h = computeHeuristic(heuristic, cx, cy, endx, endy); //Place heuristic in here.
            
            fringe = new SimplePriorityQueue<worldNode, float>();
            closedList = new List<worldNode>();

            fringe.Enqueue(currentNode, currentNode.g + h - 200*currentNode.g);

            while (fringe.Any())
            {
                currentNode = fringe.Dequeue();
                if (currentNode == worldNodes[endx, endy])
                {
                    sw.Stop();
                    return true;
                }
                closedList.Add(currentNode);
                for (int i = -1; i < 1; i++)
                {
                    for (int j = -1; j < 1; j++)
                    {
                        nx = cx + i;
                        ny = cy + j;
                        if(nx < 0 || nx > 119 || ny < 0 || ny > 159)
                        {
                            continue;
                        }
                        if (worldNodes[nx, ny] == null)
                        {
                            //Was never expanded.
                            nextNode = new worldNode(nx, ny);
                            worldNodes[nx, ny] = nextNode;
                            UpdateVertex(cx, cy, nx, ny, heuristic, endx, endy);
                        }
                        else
                        {
                            nextNode = worldNodes[nx, ny];
                            if (!closedList.Contains(worldNodes[nx, ny]))
                            {
                                if (!fringe.Contains(worldNodes[nx, ny]))
                                {//It exists, but was not generated, and is not in the fringe?
                                    //Strange... Very strange... reset it.
                                    nextNode.g = 30000; // use 30,000 to refer to infinity
                                    nextNode.parent = null;//Parent of s' = NULL
                                }
                                UpdateVertex(cx, cy, nx, ny, heuristic, endx, endy);
                            }
                        }
                    }
                }
            }
            sw.Stop();
            return false;
        }

        public float computeHeuristic(int heuristic, int cx, int cy, int endx, int endy)
        {
            //Can be slightly optimized with a switch statement
            if (heuristic == 1)
            {
                //Manhattan (all 1)
                return (float)(Math.Abs(cx - endx) + Math.Abs(cy - endy));
            }
            else if (heuristic == 2)
            {
                //Manhattan (all .25)
                return (float)(Math.Abs(cx - endx) + Math.Abs(cy - endy)/4);
            }
            else if (heuristic == 3)
            {
                //Octal, assume all highway.
                float a = Math.Abs(cx - endx);
                float b = Math.Abs(cy - endy);
                float cost;
                if (a < b)
                {
                    cost = a * 1.41421356237f;
                    cost += (b - a) * .25f;
                }
                else
                {
                    cost = b * 1.41421356237f;
                    cost += (a - b) * .25f;
                }
                return cost;

            }
            else if (heuristic == 4)
            {
                //Chebyshev, assume all highway.
                float a = Math.Abs(cx - endx);
                float b = Math.Abs(cy - endy);
                float cost;
                if (a < b)
                {
                    cost = a * .25f;
                    cost += (b - a) * .25f;
                }
                else
                {
                    cost = b * .25f;
                    cost += (a - b) * .25f;
                }
                return cost;

            }
            else// (heuristic == 0)
            {
                //Euclidian
                return (float)Math.Sqrt(Math.Pow(cx - endx, 2) + Math.Pow(cy - endy, 2));
            }
        }

        void UpdateVertex(int x, int y, int nx, int ny, int heuristic, int endx, int endy)
        {
            worldNode current = worldNodes[x, y];
            worldNode next = worldNodes[nx, ny];
            
            float css = cost(x, y, nx, ny);
            float h = computeHeuristic(heuristic, nx, ny, endx, endy);
            
            if (current.g + css  < next.g)
            {
                next.g = current.g + css;
                next.parent = current;
                if (fringe.Contains(next))
                {
                    fringe.Remove(next);
                }
                fringe.Enqueue(next, next.g + h - 200*next.g); // f- c*g where c = 200

            }
        }
        float cost(int x, int y, int nx, int ny)
        {
            int start = world[x, y];
            int next = world[nx, ny];
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