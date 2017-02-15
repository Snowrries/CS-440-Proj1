using System;
using System.Collections.Generic;
using System.Diagnostics;
using Priority_Queue;

public interface WeightedGraph<L>
{
    float Cost(GridPOS a, GridPOS b);
    IEnumerable<GridPOS> Neighbors(GridPOS id);
}


public struct GridPOS
{
    public int row { get; }
    public int col { get; }
    public GridPOS(int row, int col)
    {
        this.row = row;
        this.col = col;
    }
    //TODO override equals and gethashcode
    public static GridPOS operator +(GridPOS a, GridPOS b)
    {
        return new GridPOS(a.row + b.row, a.col + b.col);
    }
    public static GridPOS operator -(GridPOS a, GridPOS b)
    {
        return new GridPOS(a.row - b.row, a.col - b.col);
    }
    public override int GetHashCode()
    {
        unchecked // Overflow is fine, just wrap
        {
            int hash = 17;
            hash = hash * 23 + row;
            hash = hash * 23 + col;
            return hash;
        }
    }
    public override string ToString()
    {
        return "{" + this.row + "," + this.col + "}";
    }
}

public class TwoDGrid : WeightedGraph<GridPOS>
{
    //following the provided img(1) on resoreces, where p1=p0 in this case and that its a row major system.
    private static readonly GridPOS[] P = new[]
       {
            new GridPOS( -1, -1),
            new GridPOS( -1, 0),
            new GridPOS(-1, 1),
            new GridPOS(0, 1),
            new GridPOS( 1, 1),
            new GridPOS( 1, 0),
            new GridPOS(1, -1),
            new GridPOS(0, -1)
        };
    private int numCol; //only currently the grid is set like this.
    private int numRow;
    private int[,] grid;
    public TwoDGrid(int[,] grid, int rows, int cols)
    {
        this.grid = grid;
        this.numCol = cols;
        this.numRow = rows;
    }
    public bool InBounds(GridPOS id)
    {
        return 0 <= id.row && id.row < numRow
            && 0 <= id.col && id.col < numCol;
    }
    public bool Passable(GridPOS id, GridPOS next)
    {

 

        int idGridType = grid[id.row, id.col];
        int nextGridType = grid[next.row, next.col];
        Debug.WriteLine("-------");
        Debug.WriteLine(next.ToString() + " has grid type " + nextGridType);
        if (nextGridType == 0)
        {
            return false;
        }
        
        Debug.WriteLine(id.ToString() + " and " + next.ToString() + " is passable \n ----");

        return true;
    }
    private bool isDiag(GridPOS a, GridPOS b)
    {
        GridPOS result = a - b;
        if (result.Equals(P[0]) || result.Equals(P[2]) || result.Equals(P[4]) || result.Equals(P[6]))
        {
            return true;
        }
        return false;
    }
    public float Cost(GridPOS a, GridPOS b)
    {
        int aGridType = grid[a.row, a.col];
        int bGridType = grid[b.row, b.col];
        bool diag = isDiag(a, b);
      
        //both normal
        if (aGridType == 1 && (bGridType == 1 || bGridType == 3))
        {
            if (diag)
            {
                return (float)Math.Sqrt((double)2);
            }
            return 1;
        }
        //both hard
        if (aGridType == 2 && (bGridType == 2 || bGridType == 4))
        {
            if (diag)
            {
                return (float)Math.Sqrt((double)8);
            }
            return 2;
        }
        //one hard one normal
        if ((aGridType == 1 && (bGridType == 2 || bGridType == 4)) || (aGridType == 2 && (bGridType == 1 || bGridType == 3)))
        {
            if (diag)
            {
                return (float)(Math.Sqrt((double)2) + Math.Sqrt((double)8)) / (float) 2;
            }
            return (float)1.5;
        }

        //highway
        if (aGridType == 3 && bGridType == 3)
        {
            if (diag)
            {
                return (float)Math.Sqrt((double)2)/ (float)4;
            }
            return (float)0.25;
        }
        if (aGridType == 4 && bGridType == 4)
        {
            if (diag)
            {
                return (float)Math.Sqrt((double)8)/ (float) 4;
            }
            return (float)0.5;
        }
        if ((aGridType == 3 && bGridType == 4) || (aGridType == 4 && bGridType == 3))
        {
            if (diag)
            {
                return(float)((Math.Sqrt((double)2) + Math.Sqrt((double)8)) / (float)2)/(float)4;
            }
            return (float)0.375;
        }
        return float.PositiveInfinity; //some error, just ignore
    }
    public IEnumerable<GridPOS> Neighbors(GridPOS id)
    {
        foreach (var dir in P)
        {
            GridPOS next = id + dir;



            if (InBounds(next) && Passable(id, next))
            {
                yield return next;
            }
        }
    }
}
public class AStarSearch
{
    public Dictionary<GridPOS, GridPOS> closedSet = new Dictionary<GridPOS, GridPOS>();
    public Dictionary<GridPOS, GridPOS> cameFrom //path
       = new Dictionary<GridPOS, GridPOS>();
    public Dictionary<GridPOS, float> g
        = new Dictionary<GridPOS, float>();
    public Dictionary<GridPOS, float> h
        = new Dictionary<GridPOS, float>(); //I would like to store h as well. 
    public Stopwatch sw;
    WeightedGraph<GridPOS> graph;
    GridPOS start, goal;
    int hCase;
    int wValue = 1;
    private SimplePriorityQueue<GridPOS> openSet;

    public AStarSearch(int[,] grid, int[] startPOS, int[] GoalPOS, int heruisticCase)
    {
        graph = new TwoDGrid(grid, grid.GetLength(0), grid.GetLength(1));
        start = new GridPOS(startPOS[0], startPOS[1]);
        goal = new GridPOS(GoalPOS[0], GoalPOS[1]);
        openSet = new SimplePriorityQueue<GridPOS>();
        openSet.Enqueue(start, 0);
        hCase = heruisticCase;
        g[start] = 0;
        h[start] = 0;
        //cameFrom[start] = start;
    }
    public AStarSearch(int[,] grid, int[] startPOS, int[] GoalPOS, int heruisticCase, int Weight)
    {
        graph = new TwoDGrid(grid, grid.GetLength(0), grid.GetLength(1));
        start = new GridPOS(startPOS[0], startPOS[1]);
        goal = new GridPOS(GoalPOS[0], GoalPOS[1]);
        openSet = new SimplePriorityQueue<GridPOS>();
        openSet.Enqueue(start, 0);
        hCase = heruisticCase;
        wValue = Weight;
        g[start] = 0;
        h[start] = 0;
        //cameFrom[start] = start;
    }
    public float Heuristic(GridPOS next, GridPOS goal)
    {
        //add case stuff here
        if (hCase == 1)
        {
            return (float)Math.Sqrt(Math.Pow(next.col - goal.col, 2.0) + Math.Pow(next.row - goal.row, 2.0));
        }
        return 0;
    }

    public bool AStarSearchEx()
    {

        sw = Stopwatch.StartNew();
        while (openSet.Count > 0)
        {
            var current = openSet.Dequeue();
            Debug.WriteLine("_____________________________");
            Debug.WriteLine("Dequeueing " + current.ToString());

            if (current.Equals(goal))
            {
                sw.Stop();
                return true;
            }
            closedSet[current] = current;
            foreach (var next in graph.Neighbors(current))
            {
                Debug.WriteLine("neighbor of current " + current.ToString() + " being considered is  " + next.ToString());

                if (closedSet.ContainsKey(next))
                {
                    Debug.WriteLine(next.ToString() + "was already in closed set so skipping");
                    continue;
                }
                if (!g.ContainsKey(next))
                {
                    g[next] = float.PositiveInfinity;
                    h[next] = -1; //value not intilized soo.
                }
                float newG = g[current]
                    + graph.Cost(current, next);
                if (newG < g[next])
                {
                    g[next] = newG;
                    float newH = Heuristic(next, goal);
                    h[next] = newH; //I want to store g and h.
                    float f = newG + (newH * wValue);
                    if (openSet.Contains(next)) //can we optimise it with a hastable? 
                    {
                        Debug.WriteLine(next.ToString() + " was already in the tree, removing");
                        openSet.Remove(next);
                    }
                    Debug.WriteLine(next.ToString() + " added to tree with value " + f);
                    openSet.Enqueue(next, f);
                    cameFrom[next] = current;
                }

            }
        }
        sw.Stop();
        return false;
    }

}
/*public class test
{
    static void Main()
    {
        Debug.WriteLine("hello");
        int[,] grid = new int[5, 5] { {1,1,1,1,1},
                                    { 1, 1, 0, 1, 1 },
                                    { 1,1,0,1,1},
                                    { 1,1,0,1,1},
                                    {1,1,1,1,1 }};
       
        int[] start = new int[] { 0, 0 };
        int[] end = new int[] { 4, 4 };
        GridPOS endPOS = new GridPOS(end[0], end[1]);
       
       
        AStarSearch aSearch = new AStarSearch(grid, start, end, 0);
        bool result = aSearch.AStarSearchEx();
        Debug.WriteLine(result);
        
        String output = endPOS.ToString();
        this prints out the path.
        while (aSearch.cameFrom.ContainsKey(endPOS))
        {
            endPOS = aSearch.cameFrom[endPOS];
            output += endPOS.ToString();
            
        }
        Debug.WriteLine(output);
    }

}*/
