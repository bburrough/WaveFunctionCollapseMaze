using System;
using System.Collections;
using System.Collections.Generic;


/* A TileSlot is a position within a TileSet that represents an arbitrary number of possible tiles. This is
   essence of the initial state of a wave function collapse algorithm. A grid of TileSlots are all initialized
   to have several potential states. As the collapse procedure takes place, each TileSlot is "collapsed" by
   removing potential states. Once the TileSlot has only one remaining state, it is fully collapsed. */
class TileSlot : IComparable
{
    protected List<Tile> states = new List<Tile>();
    protected TileSet tileSet;

    protected int _row;
    public int row
    {
        get { return _row; }
    }

    protected int _column;
    public int column
    {
        get { return _column; }
    }


    public int stateCount
    {
        get
        {
            return states.Count;
        }
    }


    public virtual float entropy
    {
        get
        {
            return states.Count;
        }
    }


    public bool collapsed
    {
        get
        {
            if (states.Count == 1)
                return true;
            else
                return false;
        }
    }


    public Tile constraints
    {
        get
        {
            if (states.Count != 1)
                throw new System.Exception("Attempted to retrieve constraints from a slot with " + states.Count + " states.  Constraints are only valid for slots with one state.");

            return states[0];
        }
    }


    public int CompareTo(object y)
    {
        if (!(y is TileSlot))
            throw new System.ArgumentException("Invalid comparison of " + this.GetType().Name + " with " + y.GetType().Name + ".");

        return entropy.CompareTo(((TileSlot)y).entropy);
    }


    protected TileSlot()
    { }


    public virtual void PopulateTileSet()
    {
        if (states.Count != 0)
            throw new System.ArgumentException("Attempted to populate a tile slot that already contains tiles.  Expected to only populate empty tile slots.");

        // ╦ ╣ ╩ ╠ ╚ ╔ ╗ ╝ ║ ═ ╦ ╣ ╩ ╠

        states.Add(new Tile(Tile.EdgeType.empty, Tile.EdgeType.empty, Tile.EdgeType.empty, Tile.EdgeType.empty, ' '));

        // elbows
        states.Add(new Tile(Tile.EdgeType.road, Tile.EdgeType.road, Tile.EdgeType.empty, Tile.EdgeType.empty, '╚'));//
        states.Add(new Tile(Tile.EdgeType.empty, Tile.EdgeType.road, Tile.EdgeType.road, Tile.EdgeType.empty, '╔'));
        states.Add(new Tile(Tile.EdgeType.empty, Tile.EdgeType.empty, Tile.EdgeType.road, Tile.EdgeType.road, '╗'));//
        states.Add(new Tile(Tile.EdgeType.road, Tile.EdgeType.empty, Tile.EdgeType.empty, Tile.EdgeType.road, '╝'));//

        // straight throughs
        states.Add(new Tile(Tile.EdgeType.road, Tile.EdgeType.empty, Tile.EdgeType.road, Tile.EdgeType.empty, '║'));//
        states.Add(new Tile(Tile.EdgeType.empty, Tile.EdgeType.road, Tile.EdgeType.empty, Tile.EdgeType.road, '═'));//

        // 3-way stops
        states.Add(new Tile(Tile.EdgeType.empty, Tile.EdgeType.road, Tile.EdgeType.road, Tile.EdgeType.road, '╦'));//
        states.Add(new Tile(Tile.EdgeType.road, Tile.EdgeType.empty, Tile.EdgeType.road, Tile.EdgeType.road, '╣'));//
        states.Add(new Tile(Tile.EdgeType.road, Tile.EdgeType.road, Tile.EdgeType.empty, Tile.EdgeType.road, '╩'));//
        states.Add(new Tile(Tile.EdgeType.road, Tile.EdgeType.road, Tile.EdgeType.road, Tile.EdgeType.empty, '╠'));//
    }


    public TileSlot(TileSet tileSet, int row, int column)
    {
        this.tileSet = tileSet;
        _row = row;
        _column = column;

        PopulateTileSet();
    }


    public virtual TileSlot CreateCopy(TileSet newOwner)
    {
        TileSlot other = new TileSlot();

        other.tileSet = newOwner;
        other._row = _row;
        other._column = _column;

        for (int i = 0; i < states.Count; i++)
            other.states.Add(states[i].CreateCopy()); // Add a copy of our states to the other's states.

        return other;
    }


    //class TraversalRecord : Tuple<int, int, float, Tile.EdgeLocation, Tile.EdgeType>, IComparable<TraversalRecord>
    //{
    //    public TraversalRecord(int row, int col, float entropy, Tile.EdgeLocation location, Tile.EdgeType constraint) : base(row, col, entropy, location, constraint)
    //    { }

    //    public int CompareTo(TraversalRecord y)
    //    {
    //        return this.Item3.CompareTo(y.Item3);
    //    }
    //}


    public virtual void Collapse()
    {
        if (states.Count == 0)
            throw new System.Exception("Wave function collapse failed.");
        if (states.Count == 1)
            throw new System.Exception("Attempted to collapse an already-collapsed slot.");

        // pick one of our states at random
        // apply each of its edges as contraints.

        Tile constraint = states[Utes.rnd.Next() % states.Count];

        RemoveStatesWithEdgeTypesNotMatching(Tile.EdgeLocation.up, constraint.up);
        RemoveStatesWithEdgeTypesNotMatching(Tile.EdgeLocation.right, constraint.right);
        RemoveStatesWithEdgeTypesNotMatching(Tile.EdgeLocation.down, constraint.down);
        RemoveStatesWithEdgeTypesNotMatching(Tile.EdgeLocation.left, constraint.left);
    }



    public virtual void RemoveStatesWithEdgeTypesNotMatching(Tile.EdgeLocation location, Tile.EdgeType constraint)
    {
        List<Tile> removedStates = new List<Tile>();

        // Apply explicit constraint, keeping track of removed states as I go.
        for (int i = 0; i < states.Count; i++)
        {
            switch (location)
            {
                case Tile.EdgeLocation.up:
                    if (states[i].up != constraint)
                    {
                        removedStates.Add(states[i]);
                        states.RemoveAt(i);
                        i--;
                    }
                    break;
                case Tile.EdgeLocation.right:
                    if (states[i].right != constraint)
                    {
                        removedStates.Add(states[i]);
                        states.RemoveAt(i);
                        i--;
                    }
                    break;
                case Tile.EdgeLocation.down:
                    if (states[i].down != constraint)
                    {
                        removedStates.Add(states[i]);
                        states.RemoveAt(i);
                        i--;
                    }
                    break;
                case Tile.EdgeLocation.left:
                    if (states[i].left != constraint)
                    {
                        removedStates.Add(states[i]);
                        states.RemoveAt(i);
                        i--;
                    }
                    break;
            }
        }

        if (removedStates.Count == 0)
            return; // bail early

        if (tileSet.realtimeConsoleOutput)
        {
            Console.SetCursorPosition(column, row);
            DebugOut();
        }

        if (states.Count == 0)
            throw new System.Exception("Contradiction occurred while applying constraints.");

        for (int i = 0; i < removedStates.Count; i++)
        {
            // up
            if (!ContainsEdge(Tile.EdgeLocation.up, removedStates[i].up))
            {
                if (tileSet.CheckCoords(row - 1, column))
                    upSlot.RemoveStatesWithEdgeTypesMatching(Tile.EdgeLocation.down, removedStates[i].up); // We need to be very careful that this doesn't (harmfully) change any of our underlying data underneath us.
            }

            // right
            if (!ContainsEdge(Tile.EdgeLocation.right, removedStates[i].right))
            {
                if (tileSet.CheckCoords(row, column + 1)) // prevent out of bounds
                    rightSlot.RemoveStatesWithEdgeTypesMatching(Tile.EdgeLocation.left, removedStates[i].right);
            }

            // down 
            if (!ContainsEdge(Tile.EdgeLocation.down, removedStates[i].down))
            {
                if (tileSet.CheckCoords(row + 1, column)) // prevent out of bounds
                    downSlot.RemoveStatesWithEdgeTypesMatching(Tile.EdgeLocation.up, removedStates[i].down);
            }

            // left
            if (!ContainsEdge(Tile.EdgeLocation.left, removedStates[i].left))
            {
                if (tileSet.CheckCoords(row, column - 1)) // prevent out of bounds
                    leftSlot.RemoveStatesWithEdgeTypesMatching(Tile.EdgeLocation.right, removedStates[i].left);
            }
        }
    }


    public bool upSlotValid
    {
        get
        {
            return tileSet.CheckCoords(row - 1, column);
        }
    }


    public bool rightSlotValid
    {
        get
        {
            return tileSet.CheckCoords(row, column + 1);
        }
    }


    public bool downSlotValid
    {
        get
        {
            return tileSet.CheckCoords(row + 1, column);
        }
    }


    public bool leftSlotValid
    {
        get
        {
            return tileSet.CheckCoords(row, column - 1);
        }
    }


    public TileSlot upSlot
    {
        get
        {
            return tileSet[row - 1, column];
        }
    }


    public TileSlot rightSlot
    {
        get
        {
            return tileSet[row, column + 1];
        }
    }


    public TileSlot downSlot
    {
        get
        {
            return tileSet[row + 1, column];
        }
    }


    public TileSlot leftSlot
    {
        get
        {
            return tileSet[row, column - 1];
        }
    }


    public bool ContainsEdge(Tile.EdgeLocation location, Tile.EdgeType type)
    {
        for (int i = 0; i < states.Count; i++)
        {
            switch (location)
            {
                case Tile.EdgeLocation.up:
                    if (states[i].up == type)
                        return true;
                    break;
                case Tile.EdgeLocation.right:
                    if (states[i].right == type)
                        return true;
                    break;
                case Tile.EdgeLocation.down:
                    if (states[i].down == type)
                        return true;
                    break;
                case Tile.EdgeLocation.left:
                    if (states[i].left == type)
                        return true;
                    break;
            }
        }
        return false;
    }


    public virtual void RemoveStatesWithEdgeTypesMatching(Tile.EdgeLocation location, Tile.EdgeType constraint)
    {
        List<Tile> removedStates = new List<Tile>();

        for (int i = 0; i < states.Count; i++)
        {
            switch (location)
            {
                case Tile.EdgeLocation.up:
                    if (states[i].up == constraint)
                    {
                        removedStates.Add(states[i]);
                        states.RemoveAt(i);
                        i--;
                    }
                    break;
                case Tile.EdgeLocation.right:
                    if (states[i].right == constraint)
                    {
                        removedStates.Add(states[i]);
                        states.RemoveAt(i);
                        i--;
                    }
                    break;
                case Tile.EdgeLocation.down:
                    if (states[i].down == constraint)
                    {
                        removedStates.Add(states[i]);
                        states.RemoveAt(i);
                        i--;
                    }
                    break;
                case Tile.EdgeLocation.left:
                    if (states[i].left == constraint)
                    {
                        removedStates.Add(states[i]);
                        states.RemoveAt(i);
                        i--;
                    }
                    break;
            }
        }

        if (removedStates.Count == 0)
            return; // bail early

        if (tileSet.realtimeConsoleOutput)
        {
            Console.SetCursorPosition(column, row);
            DebugOut();
        }

        if (states.Count == 0)
            throw new System.Exception("Contradiction occurred while applying constraints.");

        for (int i = 0; i < removedStates.Count; i++)
        {
            // up
            if (!ContainsEdge(Tile.EdgeLocation.up, removedStates[i].up))
            {
                if (tileSet.CheckCoords(row - 1, column))
                    upSlot.RemoveStatesWithEdgeTypesMatching(Tile.EdgeLocation.down, removedStates[i].up); // We need to be very careful that this doesn't (harmfully) change any of our underlying data underneath us.
            }

            // right
            if (!ContainsEdge(Tile.EdgeLocation.right, removedStates[i].right))
            {
                if (tileSet.CheckCoords(row, column + 1)) // prevent out of bounds
                    rightSlot.RemoveStatesWithEdgeTypesMatching(Tile.EdgeLocation.left, removedStates[i].right);
            }

            // down 
            if (!ContainsEdge(Tile.EdgeLocation.down, removedStates[i].down))
            {
                if (tileSet.CheckCoords(row + 1, column)) // prevent out of bounds
                    downSlot.RemoveStatesWithEdgeTypesMatching(Tile.EdgeLocation.up, removedStates[i].down);
            }

            // left
            if (!ContainsEdge(Tile.EdgeLocation.left, removedStates[i].left))
            {
                if (tileSet.CheckCoords(row, column - 1)) // prevent out of bounds
                    leftSlot.RemoveStatesWithEdgeTypesMatching(Tile.EdgeLocation.right, removedStates[i].left);
            }
        }
    }


    public bool IsCollapsedTo(Tile.EdgeLocation location, Tile.EdgeType type)
    {
        if (states.Count == 0) // This should never happen, but let's be defensive.
            return false;

        for (int i = 0; i < states.Count; i++)
        {
            if (states[i].GetEdgeType(location) != type)
                return false;
        }

        return true;
    }


    public virtual void DebugOut()
    {
        if (states.Count == 1)
            states[0].DebugOut();
        else if (states.Count > 9)
            Console.Write(">");
        else if (states.Count == 0)
        {
            ConsoleColor color = Console.BackgroundColor;
            Console.BackgroundColor = ConsoleColor.Red;
            Console.Write("0");
            Console.BackgroundColor = color;
        }
        else
            Console.Write(states.Count);
    }


    public TileSlot GetSlotByLocation(Tile.EdgeLocation location, int offset)
    {
        return GetSlotByIndex(Tile.GetIndexByLocation(location) + offset);
    }


    public TileSlot GetSlotByLocation(Tile.EdgeLocation location)
    {
        return GetSlotByIndex(Tile.GetIndexByLocation(location));
    }


    public TileSlot GetSlotByIndex(int index)
    {
        int actualIndex = Tile.WrapLocationIndex(index);
        switch (actualIndex)
        {
            case 0:
                if (!upSlotValid)
                    return null;
                return upSlot;
            case 1:
                if (!rightSlotValid)
                    return null;
                return rightSlot;
            case 2:
                if (!downSlotValid)
                    return null;
                return downSlot;
            case 3:
                if (!leftSlotValid)
                    return null;
                return leftSlot;
            default:
                throw new System.ArgumentException("invalid index");
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