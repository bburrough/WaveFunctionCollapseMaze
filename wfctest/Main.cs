using System;
using System.Diagnostics;

static class Program
{

    static void Main()
    {
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        const int rows = 60;
        const int columns = 100;
        bool realtimeConsoleOutput = true;
        Console.OutputEncoding = System.Text.Encoding.UTF8;
#if true
        //TileSet tileSet = new TileSet(columns, rows);
        MazeTileSet tileSet = new MazeTileSet(rows, columns);
        tileSet.periodic = true;
        tileSet.realtimeConsoleOutput = realtimeConsoleOutput;

        const int entranceExitOffset = 2;
        for (int i = 0; i < rows; i++)
        {
            TileSlot leftMostColumn = tileSet.GetSlotAt(i, 0);
            TileSlot rightMostColumn = tileSet.GetSlotAt(i, columns - 1);

            leftMostColumn.RemoveStatesWithEdgeTypesNotMatching(Tile.EdgeLocation.left, Tile.EdgeType.empty);
            if (i == entranceExitOffset)
            {
                leftMostColumn.RemoveStatesWithEdgeTypesNotMatching(Tile.EdgeLocation.down, Tile.EdgeType.empty);
            }
            else if (i > 0 && i < rows - 1)
            {
                //leftMostColumn.RemoveStatesWithEdgeTypesNotMatching(Tile.EdgeLocation.right, Tile.EdgeType.empty);

                if (i != entranceExitOffset + 1)
                    leftMostColumn.RemoveStatesWithEdgeTypesMatching(Tile.EdgeLocation.up, Tile.EdgeType.empty);
                leftMostColumn.RemoveStatesWithEdgeTypesMatching(Tile.EdgeLocation.down, Tile.EdgeType.empty);
            }

            rightMostColumn.RemoveStatesWithEdgeTypesNotMatching(Tile.EdgeLocation.right, Tile.EdgeType.empty);
            if (i == (rows - 1) - entranceExitOffset - 1)
            {
                rightMostColumn.RemoveStatesWithEdgeTypesNotMatching(Tile.EdgeLocation.down, Tile.EdgeType.empty);
            }
            else if (i > 0 && i < rows - 1)
            {
                //rightMostColumn.RemoveStatesWithEdgeTypesNotMatching(Tile.EdgeLocation.left, Tile.EdgeType.empty);

                if (i != (rows - 1) - entranceExitOffset)
                    rightMostColumn.RemoveStatesWithEdgeTypesMatching(Tile.EdgeLocation.up, Tile.EdgeType.empty);
                rightMostColumn.RemoveStatesWithEdgeTypesMatching(Tile.EdgeLocation.down, Tile.EdgeType.empty);
            }

        }

        for (int i = 0; i < columns; i++)
        {
            TileSlot topMostRow = tileSet.GetSlotAt(0, i);
            TileSlot bottomMostRow = tileSet.GetSlotAt(rows - 1, i);

            if(i != entranceExitOffset)
                topMostRow.RemoveStatesWithEdgeTypesNotMatching(Tile.EdgeLocation.up, Tile.EdgeType.empty);
            else
                topMostRow.RemoveStatesWithEdgeTypesMatching(Tile.EdgeLocation.up, Tile.EdgeType.empty);

            if (i > 0 && i < columns - 1)
            {
                topMostRow.RemoveStatesWithEdgeTypesMatching(Tile.EdgeLocation.left, Tile.EdgeType.empty);
                topMostRow.RemoveStatesWithEdgeTypesMatching(Tile.EdgeLocation.right, Tile.EdgeType.empty);
            }

            if (i != entranceExitOffset)
                bottomMostRow.RemoveStatesWithEdgeTypesNotMatching(Tile.EdgeLocation.down, Tile.EdgeType.empty);
            else
                bottomMostRow.RemoveStatesWithEdgeTypesMatching(Tile.EdgeLocation.down, Tile.EdgeType.empty);

            if (i > 0 && i < columns - 1)
            {
                bottomMostRow.RemoveStatesWithEdgeTypesMatching(Tile.EdgeLocation.left, Tile.EdgeType.empty);
                bottomMostRow.RemoveStatesWithEdgeTypesMatching(Tile.EdgeLocation.right, Tile.EdgeType.empty);
            }            
        }

        sw.Start();
        try
        {
            tileSet.Collapse();
            sw.Stop();
            if (realtimeConsoleOutput)
            {
                //tileSet.DebugOut(columns, 0);
                Console.SetCursorPosition(0, rows + 2);
            }
            else
            {
                tileSet.DebugOut();
            }
        }
        catch (System.Exception e)
        {
            sw.Stop();
            if (realtimeConsoleOutput)
                Console.SetCursorPosition(0, rows + 2);
            else
                tileSet.DebugOut();
            Console.WriteLine(e);
        }
        Console.WriteLine("\nseed: " + Utes.seed + "  elapsed: " + sw.ElapsedMilliseconds + " ms.");
        tileSet.Analyze();
#elif false
        //TileSet tileSet = new TileSet(columns, rows);
        PointsTileSet tileSet = new PointsTileSet(rows, columns, 40);
        tileSet.realtimeConsoleOutput = realtimeConsoleOutput;
        sw.Start();
        try
        {
            tileSet.Collapse();
            if (realtimeConsoleOutput)
            {
                tileSet.DebugOut(columns, 0);
                Console.SetCursorPosition(0, rows + 2);
            }
            else
            {
                tileSet.DebugOut(true);
            }
        }
        catch (System.Exception e)
        {
            if (realtimeConsoleOutput)
                Console.SetCursorPosition(0, rows + 2);
            else
                tileSet.DebugOut();
            Console.WriteLine(e);
        }
        sw.Stop();
        Console.WriteLine("\nelapsed: " + sw.ElapsedMilliseconds + " ms.");
#elif false
        //TileSet tileSet = new TileSet(100, 60);
        sw.Start();
        PointsTileSet pts = PointsTileSet.CreateCollapsed(rows, columns);
        sw.Stop();
        pts.DebugOut();
        Console.WriteLine("elapsed: " + sw.ElapsedMilliseconds + " ms.");
#else
        for (int i = 0; i < 100; i++)
        {
            //int accum = 0;
            Console.Write((i + 1) + " " + ((i+1)/(float)(i+21)*100f).ToString("F1"));
            for (int j = 0; j < 20; j++)
            {
                int attemptCount = 0;
                while (true)
                {
                    sw.Start();
                    attemptCount++;
                    PointsTileSet tileSet = new PointsTileSet(rows, columns, (i + 1));
                    try
                    {
                        tileSet.Collapse();
                        //Console.WriteLine("iterations: " + (i + 1));                    
                        sw.Stop();
                        //Console.Write(".");
                        Console.Write(" " + attemptCount);
                        //tileSet.DebugOut();
                        break;
                    }
                    catch //(System.Exception e)
                    {
                        //Console.Write(".");
                        //Console.WriteLine(e);
                    }
                    sw.Stop();
                }
            }
            Console.WriteLine();
        }
        Console.WriteLine("elapsed: " + sw.ElapsedMilliseconds + " ms.");
#endif
    }
}


/*
------------------------------------------------------------------------------
This software is available under 2 licenses -- choose whichever you prefer.
------------------------------------------------------------------------------
ALTERNATIVE A - MIT License
Copyright (c) 2019-2021 Bobby G. Burrough
Permission is hereby granted, free of charge, to any person obtaining a copy of 
this software and associated documentation files (the "Software"), to deal in 
the Software without restriction, including without limitation the rights to 
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
of the Software, and to permit persons to whom the Software is furnished to do 
so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all 
copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
SOFTWARE.
------------------------------------------------------------------------------
ALTERNATIVE B - Public Domain (www.unlicense.org)
This is free and unencumbered software released into the public domain.
Anyone is free to copy, modify, publish, use, compile, sell, or distribute this 
software, either in source code form or as a compiled binary, for any purpose, 
commercial or non-commercial, and by any means.
In jurisdictions that recognize copyright laws, the author or authors of this 
software dedicate any and all copyright interest in the software to the public 
domain. We make this dedication for the benefit of the public at large and to 
the detriment of our heirs and successors. We intend this dedication to be an 
overt act of relinquishment in perpetuity of all present and future rights to 
this software under copyright law.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN 
ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION 
WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
------------------------------------------------------------------------------
*/