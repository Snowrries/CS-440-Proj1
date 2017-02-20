using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gridworld_Heuristics
{

    class createWorld
    {

        private int[,] world;
        private int[,] startPairs;
        private int[,] endPairs;
        private int[,] hardPairs;
        private Random dice;

        createWorld(int i)
        {
            dice = new Random((int)i * DateTime.Now.Millisecond);

        }

        int randomBorder(int[] points)
        {
            int direction = dice.Next(4);
            int x = dice.Next(120);
            int y = dice.Next(160);
            if (direction == 0) y = 159;
            else if (direction == 1) x = 119;
            else if (direction == 2) y = 0;
            else x = 0;
            points[0] = x;
            points[1] = y;
            return direction;
        }

        int determineHeading(int[] incPair, int direction, bool first)
        {
            int incx;
            int incy;
            int heading;
            int orientation;

            if (first)
            {
                //Choose orientation to head in. 
                orientation = dice.Next(3);
                //each number will add 90 degrees clockwise to the border orientation;
                // If we're at the north border, 1 will mean we head east. 2 is south. 3 is west. 
                // If we're at the east border, 1 will mean we head south. Etc.


                heading = (direction + orientation + 1) % 4;
                //If we add the orientation to the direction and 1, then mod by 4, 
                //we will obtain the direction that is not going straight out of bounds.
            }
            else
            {
                int roll = dice.Next(5);
                // if we roll 0, 1, 2 then we go in the same direction. 
                // if we roll 3, we go left.
                // if we roll 4, we go right.
                heading = direction;
                if (roll == 3)
                {
                    heading = (direction + 1) % 4;
                }
                else if (roll == 4)
                {
                    heading = (direction - 1) % 4;
                }
            }

            if (heading == 0)
            { incx = 0; incy = 1; }
            else if (heading == 1)
            { incx = 1; incy = 0; }
            else if (heading == 2)
            { incx = 0; incy = -1; }
            else { incx = -1; incy = 0; }
            incPair[0] = incx;
            incPair[1] = incy;

            return heading;
        }


        void create()
        {
            world = new int[120, 160];
            startPairs = new int[10, 2];
            endPairs = new int[10, 2];
            hardPairs = new int[8, 2];
            int xpair, ypair;
            //Obstacle cost = 0
            //white cost = 1
            //Dark cost = 2
            //Highway numbers 3 or 4
            for (int i = 0; i < 120; i++)
            {
                for (int j = 0; j < 160; j++)
                {
                    world[i, j] = 1;
                }
            }
            //Create 8 centers of difficult to traverse patches.
            for (int i = 0; i < 8; i++)
            {
                xpair = dice.Next(120);
                ypair = dice.Next(160);
                hardPairs[i, 0] = xpair;
                hardPairs[i, 1] = ypair;
                for (int j = -15; j < 15; j++)
                {
                    for (int k = -15; k < 15; k++)
                    {
                        if (xpair + j > 0 && ypair + k > 0 && xpair + j < 120 && ypair + k < 160)
                            world[xpair + j, ypair + k] = dice.Next(2) + 1;
                    }
                }
            }
            //Start creating 4 highways.
            bool highwayConstruction;
            int direction;
            int[] border = new int[2];
            int[] incPair = new int[2];
            bool hit;
            int failsafe = 0;
            int counter, heading, incx, incy;
            int[,] tempworld =(int[,])world.Clone();
            int xpos, ypos;

            for (int i = 0; i < 4; i++)
            {
                highwayConstruction = true;
                while (highwayConstruction)
                {//Continue until we hit a boundary, then check how far we've gone.
                    //Copy the world so we may revert to a previous state if the river construction fails.
                    int[,] tempworld2 = (int[,])tempworld.Clone();
                    //East is 0, South is 1, West is 2, North is 3.
                    //Origin is at top left. Y grows horizontally, x grows vertically.
                    // randomBorder produces a random border point. Return value will be the cardinal direction of the border.
                    direction = randomBorder(border);
                    xpos = border[0];
                    ypos = border[1];
                    hit = false;
                    //When the third parameter of this method is true, 
                    //we roll evenly between the three directions not specified by 'direction'
                    //Then we store the results in incPair.
                    heading = determineHeading(incPair, direction, true);
                    incx = incPair[0];
                    incy = incPair[1];
                    counter = 0;
                    while (!hit) //Continue in a striaght line for 20 steps, and repeat until we hit a boundary
                    {
                        for (int w = 0; w < 20; w++)
                        {
                            if (xpos > -1 && xpos < 120 && ypos > -1 && ypos < 160)
                            {// Checking to see if we're in bounds. Possibly inefficient, may be optimized.
                                if (tempworld2[xpos, ypos] > 2)
                                {   //Restart process. We hit a highway. 
                                    hit = true;
                                    counter = 0;
                                    failsafe++; //We're bound to hit highways because of RNG at some point, 
                                                //but if it's excessive, we'll just start over.
                                    break;
                                }
                                else
                                {
                                    tempworld2[xpos, ypos] += 2;
                                    // Normal highway will be 3, Hard to traverse highway will be 4;
                                }
                                xpos += incx;
                                ypos += incy;
                                counter++;
                            }
                            else
                            {
                                hit = true;
                                break;
                            }
                        }
                        //Calculate the next heading

                        heading = determineHeading(incPair, heading, false);
                        incx = incPair[0];
                        incy = incPair[1];

                    }

                    if (counter > 99)
                    {
                        highwayConstruction = false;
                        // Highway built successfully!
                        failsafe = 0;
                        //If we made a highway successfully, let's be more lenient.
                        //Also, save progress.
                        tempworld = (int[,])tempworld2.Clone();
                    }
                    if(failsafe > 1000)
                    {
                        //If we fail more than 500 times, then just reset.
                        failsafe = 0;
                        tempworld = (int[,])world.Clone();
                        i = 0;
                        //This is also the reason why we need to keep two temp copies. 
                        //If we keep running into highways, we will want to reset the failcount, and start the process anew
                    }
                }
            }
            world = (int[,])tempworld.Clone();
            //Highway is constructed.
            //Generate impassable terrain
            bool repeat;
            int x;
            int y;
            for (int i = 0; i < 3840; i++)
            {// Increment for 20% of all blocks
                repeat = true;
                while (repeat)
                {
                    x = dice.Next(120);
                    y = dice.Next(160);

                    if (world[x, y] < 3)
                    {
                        world[x, y] = 0; //Impassable terrain.
                        repeat = false;
                    }
                }
            }
        }

        void generatePairs()
        {
            bool incomplete;
            //Start and goal pairs.
            int[] sp = new int[2];
            int[] gp = new int[2];

            for (int i = 0; i < 10; i++)
            {
                incomplete = true;
                while (incomplete)
                {
                    createPair(sp);
                    for (int j = 0; j < i; j++)
                    {
                        if (startPairs[j, 0] == sp[0] && startPairs[j, 1] == sp[1])
                        {
                            j = 0; createPair(sp);
                        }
                    }
                    createPair(gp);
                    if ((Math.Abs(sp[0] - gp[0]) + Math.Abs(sp[1] - gp[1])) < 100)
                        incomplete = true;
                    else
                    {
                        Naiive search = new Naiive(world);
                        search.initAttr(2, 0, 1, sp[0], sp[1], gp[0], gp[1]);
                        incomplete = !search.hSearch();
                    }
                }
                startPairs[i, 0] = sp[0];
                startPairs[i, 1] = sp[1];
                endPairs[i, 0] = gp[0];
                endPairs[i, 1] = gp[1];

            }
        }

        void createPair(int[] pair)
        {
            int x, y;
            // Randomly create a start and end pair.
            int a = dice.Next(4);
            x = dice.Next(0, 120);
            y = dice.Next(0, 160);
            //Roll to see if we're generating a pair in the top, right, left, or bottom boundaries.
            if (a == 0) y = dice.Next(0, 20);
            else if (a == 1) y = dice.Next(140, 160);
            else if (a == 2) x = dice.Next(0, 20);
            else x = dice.Next(100, 120);
            pair[0] = x;
            pair[1] = y;
        }
        public static void generateWorld()
        {
            Thread[] container = new Thread[5];
            for (int i = 0; i < 5; i++)
            {
                var th = new Thread(gen_thread);
                container[i] = th;
                th.Start(i);
            }
            for(int i = 0; i < 5; i++)
            {
                container[i].Join();
            }

        }

        private static void gen_thread(Object i)
        {
            createWorld newgame = new createWorld((int) i);
            newgame.create();
            newgame.generatePairs();
            StringBuilder buffer = new StringBuilder();

            for (int j = 0; j < 10; j++)
                buffer.Append($"{newgame.startPairs[j, 0]},{newgame.startPairs[j, 1]}|");
            buffer.AppendLine();

            for (int j = 0; j < 10; j++)
                buffer.Append($"{newgame.endPairs[j, 0]},{newgame.endPairs[j, 1]}|");
            buffer.AppendLine();

            for (int j = 0; j < 8; j++)
            {
                buffer.Append($"{newgame.hardPairs[j, 0]},{newgame.hardPairs[j, 1]}");
                buffer.AppendLine();
            }

            for (int j = 0; j < 120; j++)
            {
                for (int k = 0; k < 160; k++)
                {
                    //Process each character individually
                    //Can Optimize by using a lookup table.
                    switch (newgame.world[j, k])
                    {
                        case 0:
                            buffer.Append("0,");
                            break;
                        case 1:
                            buffer.Append("1,");
                            break;
                        case 2:
                            buffer.Append("2,");
                            break;
                        case 3:
                            buffer.Append("a,");
                            break;
                        case 4:
                            buffer.Append("b,");
                            break;
                    }
                }
                buffer.AppendLine();
            }

            string filename = $"C:\\Users\\Public\\Gridworld_Heuristics\\world_{i}";
            System.IO.Directory.CreateDirectory($"C:\\Users\\Public\\Gridworld_Heuristics");
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(filename, false))
            {//Overwrites existing files
                file.WriteLine(buffer.ToString());
            }
        }


    }
}
