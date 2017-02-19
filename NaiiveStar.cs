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
    class Sequential
    {
        Naiive[] searches;
        Stopwatch sw;
        int startx, starty, endx, endy;
        float weight;
        float weight2;
        bool initialized = false;

        public Sequential(int[,] world)
        {
            searches = new Naiive[5];
            for(int i = 0; i < 5; i++)
            {
                searches[i] = new Naiive(world);
            }
        }
        public void initAttr(float weight, float weight2, int startx, int starty, int endx, int endy)
        {
            for(int i = 0; i < 5; i++)
            {
                searches[i].initAttr(i, 0, weight, startx, starty, endx, endy);
                searches[i].setup();
            }
            this.weight = weight;
            this.weight2 = weight2;
            this.startx = startx;
            this.starty = starty;
            this.endx = endx;
            this.endy = endy;
            initialized = true;
        }

        public Naiive seqSearch()
        {
            sw = Stopwatch.StartNew();
            if (!initialized)
            {
                return null;
            }
            int consistent = 2;
            while(searches[consistent].minkey() < 30000)
            {
                for (int i = 0; i < 5; i++)
                {
                    if (i == consistent) continue;
                    Naiive a = searches[i];
                    if(a.minkey() <= weight2 * searches[consistent].minkey())
                    {
                        if(a.worldNodes[endx,endy].g <= a.minkey())
                        {
                            if(a.worldNodes[endx,endy].g < 30000)
                            {
                                sw.Stop();
                                return a;
                            }

                        }
                        else
                        {
                            a.expandNode(a.fringe.Dequeue());
                        }
                    }
                    else
                    {
                        if(searches[consistent].worldNodes[endx,endy].g <= searches[consistent].minkey())
                        {
                            if(searches[consistent].worldNodes[endx,endy].g < 30000)
                            {
                                return searches[consistent];
                            }
                        }
                        else
                        {
                            searches[consistent].expandNode(searches[consistent].fringe.Dequeue());
                        }
                    }
                }
            }
            return null;
        }
    }

    class Integrated
    {
        Stopwatch sw;
        Naiive anchor;
        Naiive inad;
        float weight,weight2;
        int consistent = 2;
        int startx, starty, endx, endy;
        bool initialized = false;

        public Integrated(int[,] world)
        {
            anchor = new Naiive(world);
            inad = new Naiive(world);
        }

        public void initAttr(float weight, float weight2, int startx, int starty, int endx, int endy)
        {
            anchor.initAttr(consistent, 0, weight, startx, starty, endx, endy);
            inad.initAttr(0, 0, weight, startx, starty, endx, endy);
            anchor.setup();
            inad.setup();
            this.weight = weight;
            this.weight2 = weight2;
            this.startx = startx;
            this.starty = starty;
            this.endx = endx;
            this.endy = endy;
            initialized = true;
        }
        private void expandState(worldNode s)
        {
            if(anchor.fringe.Contains(s))
                anchor.fringe.Remove(s);
            if(inad.fringe.Contains(s))
                inad.fringe.Remove(s);
            worldNode sp;
            int nx, ny;
            int cx = s.x;
            int cy = s.y;
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    nx = cx + i;
                    ny = cy + j;
                    if (nx < 0 || nx > 119 || ny < 0 || ny > 159)
                    {
                        continue;
                    }
                    if (anchor.worldNodes[nx, ny] == null)
                    {
                        //Was never generated.
                        anchor.worldNodes[nx, ny] = new worldNode(nx, ny);
                    }
                    float cg = anchor.worldNodes[cx, cy].g;
                    float css = anchor.cost(cx, cy, nx, ny);
                    sp = anchor.worldNodes[nx, ny];
                    if (css == 0)
                    {
                        anchor.closedList.Add(sp);
                        inad.closedList.Add(sp);
                    }

                    if ( sp.g > cg + css)
                    {
                        sp.g = cg + css;
                        sp.parent = s;

                        if (!anchor.closedList.Contains(sp))
                        {
                            sp.h = anchor.computeHeuristic(nx, ny);
                            sp.f = sp.h + sp.g;
                            anchor.fringe.Enqueue(sp, anchor.keyMe(sp));
                            if (!inad.closedList.Contains(sp))
                            {
                                for ( int k = 0; k < 5; k++)
                                {
                                    if (k == consistent) continue;
                                    if(inad.keyMe(sp) < weight2*anchor.keyMe(sp))
                                    {
                                        sp.h = inad.computeHeuristic(nx, ny);
                                        sp.f = sp.h + sp.g;
                                        if (inad.fringe.Contains(sp))
                                            inad.fringe.UpdatePriority(sp, inad.keyMe(sp));
                                        else
                                            inad.fringe.Enqueue(sp, inad.keyMe(sp));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        public Naiive intSearch()
        {
            sw = Stopwatch.StartNew();
            if (!initialized)
            {
                return null;
            }
            while(anchor.minkey() < 30000)
            {
                for(int i = 0; i < 5; i++)
                {
                    if (i == consistent) continue;
                    inad.heuristic = i;
                    if(inad.minkey() < weight2 * anchor.minkey())
                    {
                        if(anchor.worldNodes[endx,endy].g <= inad.minkey())
                        {
                            if(anchor.worldNodes[endx,endy].g < 30000)
                            {
                                sw.Stop();
                                return anchor;
                            }
                        }
                        else
                        {
                            expandState(inad.fringe.Dequeue());
                        }
                    }
                    else
                    {
                        if(anchor.worldNodes[endx,endy].g <= anchor.minkey())
                        {
                            if(anchor.worldNodes[endx,endy].g < 30000)
                            {
                                sw.Stop();
                                return anchor;
                            }
                        }
                        else
                        {
                            expandState(anchor.fringe.Dequeue());
                        }
                    }
                }
            }
            return null;
        }
    }

    class Naiive
    {
        int[,] world;
        public worldNode[,] worldNodes;
        public SimplePriorityQueue<worldNode, float> fringe;
        public List<worldNode> closedList;
        public worldNode end;
        public Stopwatch sw;
        public int expanded;
        public int heuristic;
        int algo;
        float weight;
        int startx, starty, endx, endy;

        public Naiive(int[,] world)
        {
            this.world = world;
        }
        public void initAttr(int heuristic, int algo, float weight, int startx, int starty,
            int endx, int endy)
        {
            this.heuristic = heuristic;
            this.algo = algo;
            this.weight = weight;
            this.startx = startx;
            this.starty = starty;
            this.endx = endx;
            this.endy = endy;

        }
        public void setup()
        {
            worldNodes = new worldNode[120, 160];
            sw = Stopwatch.StartNew();
            fringe = new SimplePriorityQueue<worldNode, float>();
            closedList = new List<worldNode>();

            worldNode currentNode = new worldNode(startx, starty);
            worldNodes[startx, starty] = currentNode;
            end = new worldNode(endx, endy);
            worldNodes[endx, endy] = end;
            currentNode.g = 0;
            float h = computeHeuristic(startx, starty); //Place heuristic in here.

            currentNode.f = weight * h;
            fringe.Enqueue(currentNode, currentNode.f);//currentNode.g + h - 200*currentNode.g
            expanded = 0;
        }
        // Returns true if goal node is found. Returns false if no goal node is found.
        public bool hSearch()
        {
            worldNode currentNode;
            setup();
            while (fringe.Any())
            {
                currentNode = fringe.Dequeue();
                if (currentNode == worldNodes[endx, endy])
                {
                    sw.Stop();
                    return true;
                }
                expandNode(currentNode);
            }
            sw.Stop();
            return false;
        }

        public float keyMe(worldNode s)
        {
            if(s!= null)
            {
                return s.g + weight * computeHeuristic(s.x,s.y);
            }
            return 30000;
        }
        public float minkey()
        {
            if (fringe.Any())
            {
                worldNode a = fringe.Dequeue();
                fringe.Enqueue(a, a.f);
                return a.f;
            }
            return 30000;
        }
        
        public void expandNode(worldNode currentNode)
        {
            expanded++;
            int cx = currentNode.x;
            int cy = currentNode.y;
            worldNode nextNode;
            int nx, ny;
            closedList.Add(currentNode);
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    nx = cx + i;
                    ny = cy + j;
                    if (nx < 0 || nx > 119 || ny < 0 || ny > 159)
                    {
                        continue;
                    }
                    if (worldNodes[nx, ny] == null)
                    {
                        //Was never generated.
                        nextNode = new worldNode(nx, ny);
                        worldNodes[nx, ny] = nextNode;
                        UpdateVertex(cx, cy, nx, ny);
                    }
                    else
                    {
                        nextNode = worldNodes[nx, ny];
                        if (!closedList.Contains(worldNodes[nx, ny]))
                        {
                            /*if (!fringe.Contains(worldNodes[nx, ny]))
                            {
                                UpdateVertex(cx, cy, nx, ny, heuristic, algo, weight, endx, endy);
                            }*/
                            UpdateVertex(cx, cy, nx, ny);
                        }
                    }
                }
            }
        }
        public float computeHeuristic(int cx, int cy)
        {
            float lweight = weight;
            if (algo == 2)
            {//Uniform cost search
                return 0;
            }
            else if(algo == 0)
            {//Plain A*
                lweight = 1;
            }
            float ret;
            //Can be slightly optimized with a switch statement
            if (heuristic == 1)
            {
                //Manhattan (all 1)
                ret = (float)(Math.Abs(cx - endx) + Math.Abs(cy - endy));
            }
            else if (heuristic == 2)
            {
                //Manhattan (all .25)
                ret = ((float)((Math.Abs(cx - endx) + Math.Abs(cy - endy))))/4;
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
                ret = cost;

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
                ret = cost;

            }
            else// (heuristic == 0)
            {
                //Euclidian
                ret = (float)Math.Sqrt(Math.Pow(cx - endx, 2) + Math.Pow(cy - endy, 2));
            }
            return ret * lweight;
        }

        public void UpdateVertex(int x, int y, int nx, int ny)
        {
            worldNode current = worldNodes[x, y];
            worldNode next = worldNodes[nx, ny];
            
            float css = cost(x, y, nx, ny);
            if(css == 0)
            {
                closedList.Add(next);
                return;
            }
            
            if (current.g + css  < next.g)
            {
                next.g = current.g + css;
                next.h = computeHeuristic(nx, ny);
                next.f = next.g + next.h;
                next.parent = current;
                if (fringe.Contains(next))
                    fringe.UpdatePriority(next, next.f);
                else
                    fringe.Enqueue(next, next.f); // f- c*g where c = 200
                //next.g + h - 200*next.g
            }
        }
        public float cost(int x, int y, int nx, int ny)
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
            if ((start == 3 || start == 4) && (next == 3 || next == 4))
            {
                //Normalize start and next;
                multiplier = .25f;
                start -= 2;
                next -= 2;
            }

            if (start == 3 || start == 4)
                start -= 2;
            if (next == 3 || next == 4)
                next -= 2;

            
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