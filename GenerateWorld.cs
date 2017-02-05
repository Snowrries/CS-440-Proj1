using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gridworld_Heuristics
{

    class createWorld
    {

        static int[,] world = new int[120, 160];
        static int[,] startPairs = new int[10, 2];
        static int[,] endPairs = new int[10, 2];
        static int[,] hardPairs = new int[8, 2];


        static int randomBorder(int[] points)
        {
            Random cPair = new Random();
            int direction = cPair.Next(4);
            int x = cPair.Next(120);
            int y = cPair.Next(160);
            if (direction == 0) x = 119;
            else if (direction == 1) y = 0;
            else if (direction == 2) x = 0;
            else y = 159;
            points[0] = x;
            points[1] = y;
            return direction;
        }

        static int determineHeading(int[] incPair, int direction, bool first)
        {
            Random cPair = new Random();
            int incx;
            int incy;
            int heading;
            int orientation;

            if (first)
            {
                //Choose orientation to head in. 
                orientation = cPair.Next(3);
                //each number will add 90 degrees clockwise to the border orientation;
                // If we're at the north border, 1 will mean we head east. 2 is south. 3 is west. 
                // If we're at the east border, 1 will mean we head south. Etc.


                heading = (direction + orientation + 1) % 3;
                //If we add the orientation to the direction and 1, then mod by 3, 
                //we will obtain the direction that is not going straight out of bounds.
            }
            else
            {
                int roll = cPair.Next(5);
                // if we roll 0, 1, 2 then we go in the same direction. 
                // if we roll 3, we go left.
                // if we roll 4, we go right.
                heading = direction;
                if (roll == 3)
                {
                    heading = (direction - 1) % 3;
                }
                else if (roll == 4)
                {
                    heading = (direction + 1) % 3;
                }
            }

            if (heading == 0)
            { incx = 1; incy = 0; }
            else if (heading == 1)
            { incx = 0; incy = -1; }
            else if (heading == 2)
            { incx = -1; incy = 0; }
            else { incx = 0; incy = 1; }
            incPair[0] = incx;
            incPair[1] = incy;

            return heading;
        }


        static void create()
        {
            int xpair, ypair;
            //white cost = 1
            //Obstacle cost = -1
            //Dark cost = 2
            //Highway cost = .25
            for (int i = 0; i < 120; i++)
            {
                for (int j = 0; j < 160; j++)
                {
                    world[i, j] = 1;
                }
            }
            Random cPair = new Random();
            for (int i = 0; i < 8; i++)
            {
                xpair = cPair.Next(120);
                ypair = cPair.Next(160);
                hardPairs[i, 0] = xpair;
                hardPairs[i, 1] = ypair;
                for (int j = -15; j < 15; j++)
                {
                    for (int k = -15; k < 15; k++)
                    {
                        if (xpair + j > 0 && ypair + k > 0 && xpair + j < 120 && ypair + k < 160)
                            world[xpair + j, ypair + k] = cPair.Next(2) + 1;
                    }
                }
            }
            bool highwayConstruction = true;
            int direction;
            int[] border = new int[2];
            int incx, incy;
            bool hit;
            int counter;
            int[] incPair = new int[2];
            int heading;
            int[,] tempworld =(int[,])world.Clone();
            
            while (highwayConstruction)
            {
                tempworld = (int[,])world.Clone();
                //East is 0, South is 1, West is 2, North is 3.
                direction = randomBorder(border);
                // produces a random border point. Return value will be the cardinal direction of the border.
                hit = false;
                heading = determineHeading(incPair, direction, true);
                incx = incPair[0];
                incy = incPair[1];

                counter = 0;
                while (!hit)
                {
                    for (int i = 0; i < 20; i++)
                    {
                        if (border[0] > -1 && border[0] < 120 && border[1] > -1 && border[1] < 160)
                        {// Checking to see if we're in bounds. Possibly inefficient, may be optimized.
                            if (tempworld[border[0], border[1]] > 2)
                            {   //Restart process. We hit a highway. 
                                hit = true;
                                counter = 0;
                                break;
                            }
                            else
                            {
                                tempworld[border[0], border[1]] += 2;
                                // Normal highway will be 3, Hard to traverse highway will be 4;
                            }
                            border[0] += incx;
                            border[1] += incy;
                            counter++;
                        }
                        else hit = true;
                    }
                    //Calculate the next heading

                    heading = determineHeading(incPair, heading, false);
                    incx = incPair[0];
                    incy = incPair[1];

                }

                if (counter > 99) highwayConstruction = false;

            }
            //Highway is constructed.
            world = (int[,])tempworld.Clone();
            //Generate impassable terrain
            bool repeat;
            int x;
            int y;
            for (int i = 0; i < 3840; i++)
            {// Increment for 20% of all blocks
                repeat = true;
                while (repeat)
                {
                    x = cPair.Next(120);
                    y = cPair.Next(160);

                    if (world[x, y] < 3)
                    {
                        world[x, y] = 0; //Impassable terrain.
                        repeat = false;
                    }
                }
            }
        }

        static void generatePairs()
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
                    createPair(gp);
                    //ToDo: Determine if there is a valid path from sp to gp
                    //If valid, set incomplete to false.
                    incomplete = false; // For now, just assume it works, so that we can test output.
                }

            }
        }

        static void createPair(int[] pair)
        {
            Random dice = new Random();
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
            for (int i = 0; i < 5; i++)
            {
                create();
                generatePairs();
                StringBuilder buffer = new StringBuilder();

                for (int j = 0; j < 10; j++)
                    buffer.Append($"{startPairs[j, 0]},{startPairs[j, 1]}|");
                buffer.AppendLine();

                for (int j = 0; j < 10; j++)
                    buffer.Append($"{endPairs[j, 0]},{endPairs[j, 1]}|");
                buffer.AppendLine();

                for (int j = 0; j < 8; j++)
                {
                    buffer.Append($"{hardPairs[j, 0]},{hardPairs[j, 1]}");
                    buffer.AppendLine();
                }

                for (int j = 0; j < 120; j++)
                {
                    for(int k = 0; k < 160; k++)
                    {
                        //Process each character individually
                        //Can Optimize by using a lookup table.
                        switch (world[j, k])
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
                //Create file that has the world map and start/end pairs.
                /*• The first line provides the coordinates of sstart
• The second line provides the coordinates of sgoal
• The next eight lines provide the coordinates of the centers of the hard to traverse regions
(i.e., the centers(rnd, yrnd) from the description above)
• Then, provide 120 rows with 160 characters each that indicate the type of terrain for the map
as follows:
– Use ’0’ to indicate a blocked cell
– Use ’1’ to indicate a regular unblocked cell
– Use ’2’ to indicate a hard to traverse cell
– Use ’a’ to indicate a regular unblocked cell with a highway
– Use ’b’ to indicate a hard to traverse cell with a highway*/
            }

        }



    }
}
