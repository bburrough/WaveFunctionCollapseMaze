using System;
using System.Collections;
using System.Collections.Generic;


/* A PointsTileSet is a specialization of a TileSet which contains PointsTileSlots instead of ordinary TileSlots. */
class PointsTileSet : TileSet
{
    protected PointsTileSet()
    { }

    public PointsTileSet(int rows, int columns, int wildcard = 1) : base()
    {
        if (columns <= 0 || rows <= 0)
            throw new System.Exception("Attempted to create a tile set with an invalid number of columns or rows.  Expected a positive, non-zero number of rows of columns.  Instead, (" + columns + ", " + rows + ") was provided.");

        this._rows = rows;
        this._columns = columns;

        slots = new PointsTileSlot[rows, columns];
        slotReferences = new List<TileSlot>(rows * columns);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                slots[i, j] = new PointsTileSlot(this, i, j, wildcard);
                slotReferences.Add(slots[i, j]);
            }
        }
    }


    public override TileSet CreateCopy()
    {
        PointsTileSet other = new PointsTileSet();
        other._rows = rows;
        other._columns = columns;
        other.enumeratorPosition = enumeratorPosition;
        other.enumeratorInitialized = enumeratorInitialized;
        other.slots = new PointsTileSlot[rows, columns];
        other.slotReferences = new List<TileSlot>(rows * columns);
        for (int i = 0; i < slotReferences.Count; i++)
        {
            System.Diagnostics.Debug.Assert(slotReferences[i] is PointsTileSlot);
            PointsTileSlot tempSlot = (PointsTileSlot)((PointsTileSlot)slotReferences[i]).CreateCopy(other);
            other.slots[tempSlot.row, tempSlot.column] = tempSlot;
            other.slotReferences.Add(tempSlot);
        }

        return other;
    }


    public static new PointsTileSet CreateCollapsed(int rows, int columns)
    {
        PointsTileSet previousTileSet = new PointsTileSet(rows, columns);
        int i = 0;
        do
        {
            i++;
            Console.WriteLine(i);
            PointsTileSet currentTileSet = (PointsTileSet)previousTileSet.CreateCopy();

            PointsTileSlot nextTileSlot = (PointsTileSlot)currentTileSet.GetNextTileSlot();
            if (nextTileSlot == null) // There are no more valid tile slots.
                break;

            try
            {
                nextTileSlot.Collapse(); // Attempt the collapse.
                currentTileSet.ResetEnumerator();
                previousTileSet = currentTileSet;
            }
            catch (System.Exception e)
            {
                // If the collapse fails, do nothing.  This will cause the loop to come around and try again.
                // If we do any tracking to prevent repeats, that would occur here.
                //Console.WriteLine(e);
            }

        } while (true);

        return previousTileSet;
    }


    class PointsTileSlotKey : IEquatable<PointsTileSlotKey>, IComparable<PointsTileSlotKey>
    {
        public float entropy;
        public int states;

        public PointsTileSlotKey(float entropy, int states)
        {
            this.entropy = entropy;
            this.states = states;
        }

        public int CompareTo(PointsTileSlotKey other)
        {
            if (entropy == other.entropy)
                return states.CompareTo(other.states);
            else
                return entropy.CompareTo(other.entropy);
        }

        public bool Equals(PointsTileSlotKey other)
        {
            return entropy == other.entropy && states == other.states;
        }

        public override bool Equals(object obj)
        {
            if (obj is PointsTileSlotKey)
                return Equals((PointsTileSlotKey)obj);
            else
                return false;
        }

        public override int GetHashCode()
        {
            return entropy.GetHashCode() ^ states;
        }

        public static bool operator ==(PointsTileSlotKey x, PointsTileSlotKey y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(PointsTileSlotKey x, PointsTileSlotKey y)
        {
            return !(x.Equals(y));
        }

        //public override int CompareTo(PointsTileSlotKey other)
        //{

        //}
    }


    public void AnalyzeStates()
    {
        Dictionary<PointsTileSlotKey, List<PointsTileSlot>> buckets = new Dictionary<PointsTileSlotKey, List<PointsTileSlot>>();

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                PointsTileSlotKey ptsk = new PointsTileSlotKey(slots[i, j].entropy, slots[i, j].stateCount);
                List<PointsTileSlot> members;
                if (!buckets.TryGetValue(ptsk, out members))
                {
                    members = new List<PointsTileSlot>();
                    buckets.Add(ptsk, members);
                }
                members.Add((PointsTileSlot)slots[i, j]);
            }
        }

        List<PointsTileSlotKey> keys = new List<PointsTileSlotKey>(buckets.Keys);
        keys.Sort();
        Console.WriteLine("entp\tcount\tmembers");
        for (int i = 0; i < keys.Count; i++)
        {
            List<PointsTileSlot> members;
            buckets.TryGetValue(keys[i], out members);
            Console.WriteLine(keys[i].entropy + "\t" + keys[i].states + "\t" + members.Count);
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