using System;
using System.Collections;
using System.Collections.Generic;


/* TileSet is a 2D arrangement of TileSlots. */
class TileSet
{
    protected TileSlot[,] slots;
    protected List<TileSlot> slotReferences;
    public bool realtimeConsoleOutput = false;
    protected int _columns;
    protected int _rows;
    public bool periodic = false;

    public int columns
    {
        get
        {
            return _columns;
        }
    }

    public int rows
    {
        get
        {
            return _rows;
        }
    }



    public TileSlot GetSlotAt(int row, int column)
    {
        return slots[(row + rows) % rows, (column + columns) % columns];
    }


    protected TileSet()
    { }


    public virtual TileSet CreateCopy()
    {
        TileSet other = new TileSet();

        other._columns = _columns;
        other._rows = _rows;
        other.enumeratorPosition = enumeratorPosition;
        other.enumeratorInitialized = enumeratorInitialized;

        other.slots = new TileSlot[rows, columns];
        other.slotReferences = new List<TileSlot>();
        for (int i = 0; i < slotReferences.Count; i++)
        {
            TileSlot tempSlot = slotReferences[i].CreateCopy(other);
            other.slots[tempSlot.row, tempSlot.column] = tempSlot;
            other.slotReferences.Add(tempSlot);
        }

        return other;
    }


    public TileSet(int rows, int columns)
    {
        if (columns <= 0 || rows <= 0)
            throw new System.Exception("Attempted to create a tile set with an invalid number of columns or rows.  Expected a positive, non-zero number of rows of columns.  Instead, (" + columns + ", " + rows + ") was provided.");

        this._columns = columns;
        this._rows = rows;

        slots = new TileSlot[rows, columns];
        slotReferences = new List<TileSlot>(rows * columns);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                slots[i, j] = new TileSlot(this, i, j);
                slotReferences.Add(slots[i, j]);
            }
        }
    }


    public void ResetEnumerator()
    {
        enumeratorInitialized = false;
        enumeratorPosition = -1;
    }


    protected int enumeratorPosition = -1;
    protected bool enumeratorInitialized = false;
    public virtual TileSlot GetNextTileSlot()
    {
        if (!enumeratorInitialized)
        {
            if (slotReferences == null || slotReferences.Count <= 0)
                throw new System.ArgumentException("Attempt to get the next tile slot when there are no tile slots.");

            slotReferences.Sort();

            enumeratorPosition = 0;
            enumeratorInitialized = true;

            while (slotReferences[enumeratorPosition].collapsed)
            {
                enumeratorPosition++;
                if (enumeratorPosition >= slotReferences.Count)
                    return null;
            }
            return slotReferences[enumeratorPosition];
        }
        else
        {
            enumeratorPosition++;
            if (enumeratorPosition >= slotReferences.Count)
                return null;

            return slotReferences[enumeratorPosition];
        }
    }


    public List<TileSlot>.Enumerator Begin()
    {
        return slotReferences.GetEnumerator();
    }


    public bool CheckCoords(int row, int column)
    {
        if (periodic)
            return true;
        else
        {
            if (row >= 0 && row < rows && column >= 0 && column < columns)
                return true;
            else
                return false;
        }
    }


    public TileSlot this[int row, int column]
    {
        get { return GetSlotAt(row, column); }
    }


    /*
        - create initial tile set and add it to tileSets
        - start loop
        - currentTileSet = copy of the last tile set in the list (on first pass, the initial)
        - get the next slot (on first pass, this initializes the enumerator in the current tile set and sorts the list)
        - attempt to collapse
        - if success, add to list, otherwise do nothing.
        - next loop
        - current tile set = copy of the l
        
     */
    public static TileSet CreateCollapsed(int rows, int columns)
    {
        //List<TileSet> tileSets = new List<TileSet>();
        TileSet previousTileSet = new TileSet(rows, columns);
        //tileSets.Add(new TileSet(columns, rows));
        int i = 0;
        do
        {
            i++;
            Console.WriteLine(i);
            TileSet currentTileSet = previousTileSet.CreateCopy();

            TileSlot nextTileSlot = currentTileSet.GetNextTileSlot();
            if (nextTileSlot == null) // There are no more valid tile slots.
                break;

            try
            {
                nextTileSlot.Collapse(); // Attempt the collapse.
                currentTileSet.ResetEnumerator();
                previousTileSet = currentTileSet;
            }
            catch
            {
                // If the collapse fails, do nothing.  This will cause the loop to come around and try again.
                // If we do any tracking to prevent repeats, that would occur here.
                Console.WriteLine("!");
            }

        } while (true);

        return previousTileSet;
    }

    private static int Comparer(TileSlot x, TileSlot y)
    {
        if (!(x is PointsTileSlot) || !(y is PointsTileSlot))
            throw new System.ArgumentException();

        PointsTileSlot ptx = (PointsTileSlot)x, pty = (PointsTileSlot)y;

        return ptx.regionEntropy.CompareTo(pty.regionEntropy);
    }

    // sweep coordinates
    public static readonly int[] drow = { 0, -1, -1, 0, 1, 1, 1, 0, -1 };
    public static readonly int[] dcol = { 0, 0, 1, 1, 1, 0, -1, -1, -1 };

    public void Collapse(int maxIterations = -1)
    {
        if (realtimeConsoleOutput)
        {
            Console.Clear();
            Console.CursorVisible = false;
            DebugOut();
        }

        if (maxIterations == -1)
            maxIterations = rows * columns;
#if false
        int row = Utes.rnd.Next() % rows;
        int col = Utes.rnd.Next() % columns;

        for (int sweepIndex = 0; sweepIndex < drow.Length; sweepIndex++)
        {
            if (CheckCoords(row + drow[sweepIndex], col + dcol[sweepIndex]))
            {
                TileSlot ts = GetSlotAt(row + drow[sweepIndex], col + dcol[sweepIndex]);
                if (!ts.collapsed)
                    ts.Collapse();
            }
        }


        for (int i = 0; i < 200000; i++)
        {
            int test = 0;
            if (i == 623)
                test++;

            float minimumEntropy = float.PositiveInfinity;
            int minimumEntropyRow = -1, minimumEntropyColumn = -1;
            for (int j = 0; j < rows; j++)
            {
                for (int k = 0; k < columns; k++)
                {
                    if (slots[j, k].collapsed)
                        continue;

                    float currentEntropy;
                    //if (slots[j, k] is PointsTileSlot)
                    //    currentEntropy = ((PointsTileSlot)slots[j, k]).regionEntropy;
                    //else
                    currentEntropy = slots[j, k].entropy;
                    if (currentEntropy < minimumEntropy)
                    {
                        minimumEntropy = currentEntropy;
                        minimumEntropyRow = j;
                        minimumEntropyColumn = k;
                    }
                }
            }
            if (minimumEntropyRow == -1)
                break;
            else
            {
                try
                {
                    for (int sweepIndex = 0; sweepIndex < drow.Length; sweepIndex++)
                    {
                        if (CheckCoords(minimumEntropyRow + drow[sweepIndex], minimumEntropyColumn + dcol[sweepIndex]))
                        {
                            TileSlot ts = GetSlotAt(minimumEntropyRow + drow[sweepIndex], minimumEntropyColumn + dcol[sweepIndex]);
                            if (!ts.collapsed)
                                ts.Collapse();
                        }
                    }
                }
                catch (System.Exception e)
                {
                    throw new System.Exception("seed: " + Utes.seed + " steps: " + i, e);
                }
                //Console.SetCursorPosition(0, 0);
                //DebugOut();
            }
        }
#elif true
        //int row = Utes.rnd.Next() % (rows - 2) + 1;
        //int col = Utes.rnd.Next() % (columns - 2) + 1;

        //slots[row, col].Collapse();

        for (int i = 0; i < maxIterations; i++)
        {
            List<TileSlot> minSet = new List<TileSlot>();
            float minimumEntropy = float.PositiveInfinity;
            //int minimumEntropyRow = -1, minimumEntropyColumn = -1;
            for (int j = 0; j < rows; j++)
            {
                for (int k = 0; k < columns; k++)
                {
                    if (slots[j, k].collapsed)
                        continue;

                    float currentEntropy;
                    //if (slots[j, k] is PointsTileSlot)
                    //    currentEntropy = ((PointsTileSlot)slots[j, k]).regionEntropy;
                    //else
                    currentEntropy = slots[j, k].entropy;
                    if (currentEntropy < minimumEntropy)
                    {
                        minSet.Clear();
                        minSet.Add(slots[j, k]);
                        minimumEntropy = currentEntropy;
                    }
                    else if (currentEntropy <= minimumEntropy) // This is here only for equality's sake. The less than is simply defensive.
                    {
                        minSet.Add(slots[j, k]);
                    }
                }
            }
            if (minSet.Count <= 0)
                break;
            else
            {
                try
                {
                    int test = 0;
                    if (i == 29)
                        test++;
                    //minSet.Sort(Comparer);
                    //minSet[0].Collapse();
                    minSet[Utes.rnd.Next() % minSet.Count].Collapse();
                }
                catch (System.Exception e)
                {
                    throw new System.Exception("seed: " + Utes.seed + " steps: " + i, e);
                }
                //Console.SetCursorPosition(0, 0);
                //DebugOut();
            }
        }
#elif true
        for (int i = 0; i < 200000; i++)
        {
            int row = Utes.rnd.Next() % rows;
            int col = Utes.rnd.Next() % columns;
            slots[row, col].Collapse();
        }
#else
        for (int i = 0; i < rows; i++)
        {
            for(int j = 0; j < columns; j++)
            {
                if (!slots[i, j].collapsed)
                    slots[i, j].Collapse();
            }
        }
#endif

        if (realtimeConsoleOutput)
            Console.CursorVisible = true;
    }


    //class TraversalRecord : Tuple<int, int, float, Tile.EdgeLocation, Tile.EdgeType>, IComparable<TraversalRecord>
    //{
    //    public TraversalRecord(int row, int col, float entropy, Tile.EdgeLocation location, Tile.EdgeType constraint) : base(row, col, entropy, location, constraint)
    //    { }

    //    public int CompareTo(TraversalRecord y)
    //    {
    //        Console.WriteLine("hi");
    //        return this.Item3.CompareTo(y.Item3);
    //    }
    //}


    /*
        So this is having issues running into contradictions.  What
        about a more aggressive constraining procedure whereby, any
        time the possible states of a slot are modified, we interrogate
        the slots to find out whether constraints may be propagated
        to neighbors?  This way, constraints may be propoagated even
        when constraints affect a slot partially.

        The current algorithm assumes there is only one state in the
        current slot.  That is checked when the .constraints field is
        accessed.  If there were more than one states, it would throw
        an exception.

        In the new implementation, this would not occur.  The invocation
        of a recursion would occur on a collapsed slot, but as constraints
        propagate, the recursion would traverse slots which are either
        completely unconstrained, or partially constrained.
    */
    //public void RecursivelyApplyConstraintsAt(int row, int col)
    //{        
    /*
        Impl:

        - Save the number of states.
        - Run the constraint procedure (drop states)
        - If the number of constraints changes,
        - Compare each edge for "constrained" or "unconstrained" condition.
        - If an edge is constrained, propagate its constraint.

        Alt:

        - Determine which edges are unconstrained
        - For each of the unconstrained edges,
        - Apply the filter (if appropriate)
        - Check whether the edge remains unconstrained (i.e. there may be three possible
          states, and only one was dropped)
            - Actually, this doesn't make sense at all.  Constrained / Unconstrained is not
              a good condition to use to drive the algorithm.  If there are three possible
              conditions, the edge remains definitionally unconstrained, but would not cause
              propagation to occur because it hadn't yet transitioned to "constrained."

        So, what needs to occur is any time a particular edge type is removed as
        part of a constraint, that removal must also occur in neighbor cells.
        One weakness of the algorithm currently is the assumption that the cell
        has only one state.

        So...first let's change the ApplyConstraints algo to return true if any
        modifications occur. Then, if ApplyConstraints returns true, we can repropagate
        by calling RecursivelyApplyConstraintsAt() on it.  This also means that
        RecursivelyApplyConstraintsAt() must figure out which constraints to propagate.
        It can't make the assumption that there's only one state at all.

        One way to do it is to simply compare the current states with the possible states
        and apply all of the missing states as constraints...Actually, that doesn't work.
        States being missing does not determine whether a constraint has been applied.
        What does determine whether a constraint has been applied is whether all of the
        existing states

        ---

        Okay, so after thinking about it for a while, it occurs to me that what I'm
        struggling against are implicit constraints.  When we apply a collapsed
        slot as a constraint, it is the most simple, direct, explicit form.  Every
        neighboring cell must have a matching edge type on the adjacent edges.
        However, what's different about applying a slot with a single state as
        an explicit constraint from applying constraints in the neighbors is, 
        we're only applying a single edge as a constraint rather than all four.
        Further, applying a constraint on one edge may implicity cause constraints
        to occur on other edges.

        Hypothetically, if there were a tile that only existed as a left/right
        pass through... let's say a pipe.  Yet, if we apply a constraint to one
        of the sides as a road, then we know by deduction that the tile opposite
        the other side of this one is not allowed to have a pipe.  Basically,
        an implicit constraint against pipeline in that edge has occurred.

        algo:

        Apply the explicit edge constraint, saving which states are removed
        For every other edge, check each removed edge type and see whether
        another example currently exists.  If no other example currently
        exists, then the edge has been implicity constrained and that change
        must be propagated.


     */
    //}


    //public void RecursivelyApplyConstraints2At(int row, int col)
    //{

    //List<TraversalRecord> rowColEntropyRecords = new List<TraversalRecord>();
    //// up
    //int traverseRow = row - 1;
    //int traverseCol = col;
    //if (traverseRow >= 0)
    //    rowColEntropyRecords.Add(new TraversalRecord(traverseRow, traverseCol, slots[traverseRow, traverseRow].entropy, Tile.EdgeLocation.down, slots[row, col].constraints.up));
    //// right
    //traverseRow = row;
    //traverseCol = col + 1;
    //if (traverseCol < columns)
    //    rowColEntropyRecords.Add(new TraversalRecord(traverseRow, traverseCol, slots[traverseRow, traverseRow].entropy, Tile.EdgeLocation.left, slots[row, col].constraints.right));
    //// down
    //traverseRow = row + 1;
    //traverseCol = col;
    //if (traverseRow < rows)
    //    rowColEntropyRecords.Add(new TraversalRecord(traverseRow, traverseCol, slots[traverseRow, traverseRow].entropy, Tile.EdgeLocation.up, slots[row, col].constraints.down));
    //// left
    //traverseRow = row;
    //traverseCol = col - 1;
    //if (traverseCol >= 0)
    //    rowColEntropyRecords.Add(new TraversalRecord(traverseRow, traverseCol, slots[traverseRow, traverseRow].entropy, Tile.EdgeLocation.right, slots[row, col].constraints.left));

    //traverseRow = -1; // prevent these from being used accidentally
    //traverseCol = -1; 
    //rowColEntropyRecords.Sort();

    //for (int i = 0; i < rowColEntropyRecords.Count; i++)
    //    Console.WriteLine(" rowColEntropyRecord " + i + ": row=" + rowColEntropyRecords[i].Item1 + ", col=" + rowColEntropyRecords[i].Item2 + ", entropy=" + rowColEntropyRecords[i].Item3);

    //for (int i = 0; i < rowColEntropyRecords.Count; i++)
    //{

    //    bool collapseOccurred = slots[rowColEntropyRecords[i].Item1, rowColEntropyRecords[i].Item2].ApplyConstraints(rowColEntropyRecords[i].Item4, rowColEntropyRecords[i].Item5);
    //    if (collapseOccurred)
    //        RecursivelyApplyConstraintsAt(rowColEntropyRecords[i].Item1, rowColEntropyRecords[i].Item2); // recursion
    //}


    // up
    //    int traverseRow = row - 1;
    //    int traverseCol = col;
    //    if (traverseRow >= 0)
    //    {
    //        bool collapseOccurred = slots[traverseRow, traverseCol].ApplyConstraints(Tile.EdgeLocation.down, slots[row, col].constraints.up);
    //        if (collapseOccurred)
    //            RecursivelyApplyConstraintsAt(traverseRow, traverseCol); // recursion
    //    }
    //    // right
    //    traverseRow = row;
    //    traverseCol = col + 1;
    //    if (traverseCol < columns)
    //    {
    //        bool collapseOccurred = slots[traverseRow, traverseCol].ApplyConstraints(Tile.EdgeLocation.left, slots[row, col].constraints.right);
    //        if (collapseOccurred)
    //            RecursivelyApplyConstraintsAt(traverseRow, traverseCol); // recursion
    //    }
    //    // down
    //    traverseRow = row + 1;
    //    traverseCol = col;
    //    if (traverseRow < rows)
    //    {
    //        bool collapseOccurred = slots[traverseRow, traverseCol].ApplyConstraints(Tile.EdgeLocation.up, slots[row, col].constraints.down);
    //        if (collapseOccurred)
    //            RecursivelyApplyConstraintsAt(traverseRow, traverseCol); // recursion
    //    }
    //    // left
    //    traverseRow = row;
    //    traverseCol = col - 1;
    //    if (traverseCol >= 0)
    //    {
    //        bool collapseOccurred = slots[traverseRow, traverseCol].ApplyConstraints(Tile.EdgeLocation.right, slots[row, col].constraints.left);
    //        if (collapseOccurred)
    //            RecursivelyApplyConstraintsAt(traverseRow, traverseCol); // recursion
    //    }
    //}


    public void DebugOut(int left, int top)
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                Console.SetCursorPosition(left + j, top + i);
                slots[i, j].DebugOut();
            }
        }
    }

    public void DebugOut(bool periodic = false)
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < (periodic ? columns * 2 : columns); j++)
            {
                GetSlotAt(i, j).DebugOut();
            }
            Console.WriteLine();
        }
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