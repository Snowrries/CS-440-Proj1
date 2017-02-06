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
    //following the provided img(1) on resoreces, where p1=p0 in this case and that its a y,x system
    private static readonly GridPOS[] P = new[]
       {
            new GridPOS( -1, 1),
            new GridPOS( 1, 0),
            new GridPOS(1, 1),
            new GridPOS(0, 1),
            new GridPOS( -1, 1),
            new GridPOS( -1, 0),
            new GridPOS(-1, -1),
            new GridPOS(0, -1)
        };
    private int numCol; //only currently the grid is set like this.
    private int numRow;
    private int[,] grid;
    public TwoDGrid(int[,] grid, int rows, int cols)
    {
        this.grid = grid;
        this.numCol=cols;
        this.numRow=rows;
    }
    public bool InBounds(GridPOS id)
    {
        return 0 <= id.row && id.row < numRow
            && 0 <= id.col && id.col < numCol;
    }
    public bool Passable(GridPOS id, GridPOS next)
    {
        //assumes that you can't travel diagonally on another highway from a highway

        int idGridType = grid[id.row, id.col];
        int nextGridType = grid[next.row, next.col];
        if (nextGridType == 0)
        {
            return false;
        }
        if (isDiag(id, next) && (idGridType == 3 || idGridType == 4) && (nextGridType == 3 || nextGridType == 4))
        {
            return false;
        }
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
        //assumes that getting on the highway have no modifiers and while on a highway, you can't travel diagonally on another highway from a highway
        //nomral cells
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
                return (float)(Math.Sqrt((double)2) + Math.Sqrt((double)8)) / 2;
            }
            return (float)1.5;
        }

        //highway
        if (aGridType == 3 && bGridType == 3)
        {
            return (float)0.25;
        }
        if (aGridType == 4 && bGridType == 4)
        {
            return (float)0.5;
        }
        if ((aGridType == 3 && bGridType == 4) || (aGridType == 4 && bGridType == 3))
        {
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
    public Dictionary<GridPOS, GridPOS> cameFrom
       = new Dictionary<GridPOS, GridPOS>();
    public Dictionary<GridPOS, float[]> g
        = new Dictionary<GridPOS, float[]>(); //also works as the closed set.
    private Stopwatch sw;
    WeightedGraph<GridPOS> graph;
    GridPOS start, goal;
    int hCase;
    int wValue = 1;
    private SimplePriorityQueue<GridPOS> openSet;

    public AStarSearch(int[,] grid, int[] startPOS, int[] GoalPOS, int heruisticCase)
    {
        graph = new TwoDGrid(grid, grid.Rank, grid.GetLength(0));
        start = new GridPOS(startPOS[0], startPOS[1]);
        goal = new GridPOS(GoalPOS[0], GoalPOS[1]);
        openSet = new SimplePriorityQueue<GridPOS>();
        openSet.Enqueue(start, 0);
        hCase = heruisticCase;
        g[start] = new float[]{0,0};
        cameFrom[start] = start;
    }
    public AStarSearch(int[,] grid, int[] startPOS, int[] GoalPOS, int heruisticCase, int Weight)
    {
        graph = new TwoDGrid(grid, grid.Rank, grid.GetLength(0));
        start = new GridPOS(startPOS[0], startPOS[1]);
        goal = new GridPOS(GoalPOS[0], GoalPOS[1]);
        openSet = new SimplePriorityQueue<GridPOS>();
        openSet.Enqueue(start, 0);
        hCase = heruisticCase;
        wValue = Weight;
        g[start] = new float[] { 0, 0 };
        cameFrom[start] = start;
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
            Debug.WriteLine("dequeu" + current.ToString());
            if (current.Equals(goal))
            {
                sw.Stop();
                return true;
            }
            foreach (var next in graph.Neighbors(current))
            {
                float newG = g[current][0]
                    + graph.Cost(current, next);
                if (!g.ContainsKey(next)
                    || newG < g[next][0])
                {
                    if (!g.ContainsKey(next))
                    {
                        g[next] = new float[2];
                    }
                    g[next][0] = newG;
                    g[next][1] = wValue*Heuristic(next, goal);
                    float f = newG + wValue* g[next][1];
                    openSet.Enqueue(next, f);
                    cameFrom[next] = current;
                }
            }
        }
        sw.Stop();
        return false;
    }
}
public class test
{
    static void Main()
    {
        Debug.WriteLine("hello");
        int[,] grid = new int[5, 5] { {1,1,1,1,1},
                                    { 1, 1, 0, 1, 1 },
                                    { 1,1,0,1,1},
                                    { 1,1,0,1,1},
                                    {1,1,1,1,1 }};
        int[] start = new int[] { 2, 0 };
        int[] end = new int[] { 2, 4 };
        AStarSearch aSearch = new AStarSearch(grid, start, end, 0);
        bool result = aSearch.AStarSearchEx();
        Debug.WriteLine(result);
        
        foreach (var entry in aSearch.cameFrom)
        {
            
            Debug.WriteLine(entry.Key.ToString()+ entry.Value.ToString());
        }
    }

}
