using System;
using System.Collections.Generic;

public interface WeightedGraph<L>
{
    double Cost(GridPOS a, GridPOS b);
    IEnumerable<GridPOS> Neighbors(GridPOS id);
}

    
public struct GridPOS
{
    public readonly int row, col;
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
    private int numCol = 160; //only currently the grid is set like this.
    private int numRow= 120;
    private int[,] grid;
    public TwoDGrid(int [,] grid)
    {
        this.grid = grid;
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
        if(isDiag(id,next)&&(idGridType==3||idGridType==4) && (nextGridType == 3 || nextGridType == 4))
        {
            return false;
        }
        return true;
    }
    private bool isDiag(GridPOS a, GridPOS b)
    {
        GridPOS result = a - b;
        if(result == P[0] || result == P[2] || result == P[4] || result == P[6])
        {
            return true;
        }
        return false;
    }
    public double Cost(GridPOS a, GridPOS b)
    {
        int aGridType = grid[a.row, a.col];
        int bGridType = grid[b.row, b.col];
        bool diag = isDiag(a, b);
        //assumes that getting on the highway have no modifiers and while on a highway, you can't travel diagonally on another highway from a highway
        //nomral cells
        if (aGridType==1 && (bGridType == 1|| bGridType == 3))
        {
            if (diag)
            {
                return Math.Sqrt((double)2);
            }
            return 1;
        }
        //both hard
        if (aGridType == 2 && (bGridType == 2|| bGridType == 4))
        {
            if (diag)
            {
                return Math.Sqrt((double)8);
            }
            return 2;
        }
        //one hard one normal
        if ((aGridType == 1 && (bGridType == 2 || bGridType == 4))|| (aGridType == 2 && (bGridType == 1 || bGridType == 3)))
        {
            if (diag)
            {
                return (Math.Sqrt((double)2)+ Math.Sqrt((double)8))/2;
            }
            return 1.5;
        }
       
        //highway
        if(aGridType == 3 && bGridType == 3)
        {
            return 0.25;
        }
        if (aGridType == 4 && bGridType == 4)
        {
            return 0.5;
        }
        if( (aGridType == 3&&bGridType==4)|| (aGridType == 4 && bGridType == 3))
        {
            return 0.375;
        }
        return Double.PositiveInfinity; //some error, just ignore
    }
    public IEnumerable<GridPOS> Neighbors(GridPOS id)
    {
        foreach (var dir in DIRS)
        {
            GridPOS next = id+dir;
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
    public Dictionary<GridPOS, double> g
        = new Dictionary<GridPOS, double>(); //also works as the closed set.
    private Stopwatch sw;
    WeightedGraph<Location> graph;
    GridPOS start, goal;
    int hCase;
    int wValue = 1;
    private PrioirtyQueue openSet;

    public AStarSearch(int[,] grid, int[] startPOS, int[] GoalPOS, int heruisticCase)
	{
        graph = new TwoDGrid(grid);
        start = new GridPOS(startPOS[0], startPOS[1]);
        goal = new GridPOS(GoalPOS[0], GoalPOS[1]);
        openSet = new PrioirtyQueue<GridPOS>(start, 0);
        g[start] = 0;
        cameFrom[start] = start;
    }
    public AStarSearch(int[,] grid, int[] startPOS, int[] GoalPOS, int heruisticCase, int Weight)
	{
        graph = new TwoDGrid(grid);
        start = new GridPOS(startPOS[0], startPOS[1]);
        goal = new GridPOS(GoalPOS[0], GoalPOS[1]);
        openSet = new PrioirtyQueue<GridPOS>(start, 0);
        wValue = Weight;
        g[start] = 0;
        cameFrom[start] = start;
    }
    public double Heuristic(GridPOS next, GridPOS goal)
    {
        //add case stuff here
        if(hCase == 1)
        {
            return Math.Sqrt(Math.Pow(next.col - goal.col, 2.0) + Math.Pow(next.row - goal.row, 2.0));
        }
        return 0;
    }
    public bool AStarSearchEx()
    {
        
        sw = Stopwatch.StartNew();
        while (openSet.Count > 0)
        {
            var current = openSet.pop();
            if (current.Equals(goal))
            {
                sw.Stop();
                return true;
            }
            foreach (var next in graph.Neighbors(current))
            {
                double newCost = g[current]
                    + graph.Cost(current, next);
                if (!g.ContainsKey(next)
                    || newCost < g[next])
                {
                    g[next] = newG;
                    double f = newG + Heuristic(next, goal);
                    openSet.Enqueue(next, f);
                    cameFrom[next] = current;
                }
            }
        }
        return false;
    }
}
