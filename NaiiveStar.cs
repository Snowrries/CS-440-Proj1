﻿using System;
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
        public Stopwatch sw;

        public Naiive(int[,] world)
        {
            this.world = world;
        }
        
        // Returns true if goal node is found. Returns false if no goal node is found.
        // To do: Add options to select algorithms. Track runtime.
        public bool hSearch(int heuristic, int algo, float weight, int startx, int starty,
            int endx, int endy)
        {
            worldNodes = new worldNode[120,160];
            sw = Stopwatch.StartNew();
            worldNode currentNode = new worldNode(startx,starty);
            worldNodes[startx, starty] = currentNode;
            end = new worldNode(endx, endy);
            worldNodes[endx, endy] = end;

            worldNode nextNode;
            currentNode.g = 0;

            int cx = startx;
            int cy = starty;
            int nx, ny;
            float h = computeHeuristic(heuristic, algo, weight, cx, cy, endx, endy); //Place heuristic in here.
            
            fringe = new SimplePriorityQueue<worldNode, float>();
            closedList = new List<worldNode>();

            fringe.Enqueue(currentNode, currentNode.f );//currentNode.g + h - 200*currentNode.g

            while (fringe.Any())
            {
                currentNode = fringe.Dequeue();
                cx = currentNode.x;
                cy = currentNode.y;
                if (currentNode == worldNodes[endx, endy])
                {
                    sw.Stop();
                    return true;
                }
                closedList.Add(currentNode);
                for (int i = -1; i < 2; i++)
                {
                    for (int j = -1; j < 2; j++)
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
                            UpdateVertex(cx, cy, nx, ny, heuristic, algo, weight, endx, endy);
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
                                UpdateVertex(cx, cy, nx, ny, heuristic, algo, weight, endx, endy);
                            }
                        }
                    }
                }
            }
            sw.Stop();
            return false;
        }

        public float computeHeuristic(int heuristic, int algo, float w, int cx, int cy, int endx, int endy)
        {
            float weight = w;
            if (algo == 2)
            {//Uniform cost search
                return 0;
            }
            else if(algo == 0)
            {//Plain A*
                weight = 1;
            }
            float ret;
            w = .25f;
            //Can be slightly optimized with a switch statement
            if (heuristic == 1)
            {
                //Manhattan (all 1)
                ret = (float)(Math.Abs(cx - endx) + Math.Abs(cy - endy));
            }
            else if (heuristic == 2)
            {
                //Manhattan (all .25)
                ret = (float)(Math.Abs(cx - endx) + Math.Abs(cy - endy)/4);
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
            return ret * weight;
        }

        void UpdateVertex(int x, int y, int nx, int ny, int heuristic, int algo, float weight, int endx, int endy)
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
                next.h = computeHeuristic(heuristic, algo, weight, nx, ny, endx, endy);
                next.f = next.g + next.h;
                next.parent = current;
                if (fringe.Contains(next))
                    fringe.UpdatePriority(next, next.f);
                else
                    fringe.Enqueue(next, next.f); // f- c*g where c = 200
                //next.g + h - 200*next.g
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
            if ((start == 3 || start == 4) && (next == 3 || next == 4))
            {
                //Normalize start and next;
                multiplier = .25f;
                start -= 2;
                next -= 2;
            }
            
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