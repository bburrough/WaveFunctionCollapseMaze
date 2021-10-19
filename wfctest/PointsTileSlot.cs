

/* A PointsTileSlot is a special type of TileSlot which may be assigned tiles that have a number
   of points that represent their likelihood of being selected for removal during collapse.  The
   number of points for each tile are arbitrary and relative only to other tiles in the slot. */
class PointsTileSlot : TileSlot
{
    private int _totalPoints = 0;
    private int numStatesWhenTotalPointsCached = -1;
    public int totalPoints
    {
        get
        {
            if (states.Count == numStatesWhenTotalPointsCached)
                return _totalPoints;

            int accum = 0;
            for (int i = 0; i < states.Count; i++)
            {
                accum += ((PointsTile)states[i]).points;
            }
            _totalPoints = accum;
            numStatesWhenTotalPointsCached = states.Count;
            return _totalPoints;
        }
    }


    protected PointsTileSlot()
    { }

    private int wildcard;
    public PointsTileSlot(TileSet tileSet, int row, int column, int wildcard = 1)
    {
        this.tileSet = tileSet;
        _row = row;
        _column = column;
        this.wildcard = wildcard;

        PopulateTileSet();
    }


    public override TileSlot CreateCopy(TileSet newOwner)
    {
        PointsTileSlot other = new PointsTileSlot();

        other.tileSet = newOwner;
        other._row = _row;
        other._column = _column;
        other.wildcard = wildcard;
        other._totalPoints = _totalPoints;
        other.numStatesWhenTotalPointsCached = numStatesWhenTotalPointsCached;
        other._entropy = _entropy;
        other.numStatesWhenEntropyCached = numStatesWhenEntropyCached;

        for (int i = 0; i < states.Count; i++)
        {
            System.Diagnostics.Debug.Assert(states[i] is PointsTile);
            other.states.Add(states[i].CreateCopy());
        }

        return other;
    }


    // Proper Shannon entropy
    private float _entropy = float.NaN;
    private int numStatesWhenEntropyCached = -1;

    //public override float entropy
    //{
    //    get
    //    {
    //        if (states.Count == numStatesWhenEntropyCached)
    //            return _entropy;

    //        int _totalPoints = totalPoints; // Cached here so we don't incessantly recalculate it.
    //        float accumTerms = 0f;
    //        for (int i = 0; i < states.Count; i++)
    //        {
    //            float stateProbability = TileAt(i).points / (float)_totalPoints;
    //            float term = stateProbability * (float)System.Math.Log((double)stateProbability); // Is there a real, high performance log2f?
    //            accumTerms += term;
    //        }
    //        _entropy = -accumTerms;
    //        numStatesWhenEntropyCached = states.Count;
    //        return _entropy;
    //    }
    //}


    public float regionEntropy
    {
        get
        {
            float retEntropy = 0f;
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (tileSet.CheckCoords(row + i, column + j))
                        retEntropy += tileSet.GetSlotAt(row + i, column + j).entropy;
                }
            }
            //if (tileSet.CheckCoords(row - 1, column))
            //    retEntropy += upSlot.entropy;
            //if (tileSet.CheckCoords(row, column + 1))
            //    retEntropy += rightSlot.entropy;
            //if (tileSet.CheckCoords(row + 1, column))
            //    retEntropy += downSlot.entropy;
            //if (tileSet.CheckCoords(row, column - 1))
            //    retEntropy += leftSlot.entropy;
            return retEntropy;
        }
    }


    public PointsTile TileAt(int i)
    {
        return (PointsTile)states[i];
    }


    public override void PopulateTileSet()
    {
        if (states.Count != 0)
            throw new System.ArgumentException("Attempted to populate a tile slot that already contains tiles.  Expected to only populate empty tile slots.");

        // ╦ ╣ ╩ ╠ ╚ ╔ ╗ ╝ ║ ═ ╦ ╣ ╩ ╠

        int amplifcationFactor = 1000;
        states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.empty, Tile.EdgeType.empty, Tile.EdgeType.empty, ' ', wildcard * amplifcationFactor));

        // 4-way stop
        //states.Add(new PointsTile(Tile.EdgeType.road, Tile.EdgeType.road, Tile.EdgeType.road, Tile.EdgeType.road, '╬', 1 * amplifcationFactor));

        // elbows
        states.Add(new PointsTile(Tile.EdgeType.road, Tile.EdgeType.road, Tile.EdgeType.empty, Tile.EdgeType.empty, '╚', 1 * amplifcationFactor));
        states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.road, Tile.EdgeType.road, Tile.EdgeType.empty, '╔', 1 * amplifcationFactor));
        states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.empty, Tile.EdgeType.road, Tile.EdgeType.road, '╗', 1 * amplifcationFactor));
        states.Add(new PointsTile(Tile.EdgeType.road, Tile.EdgeType.empty, Tile.EdgeType.empty, Tile.EdgeType.road, '╝', 1 * amplifcationFactor));

        // straight throughs
        states.Add(new PointsTile(Tile.EdgeType.road, Tile.EdgeType.empty, Tile.EdgeType.road, Tile.EdgeType.empty, '║', 1 * amplifcationFactor));
        states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.road, Tile.EdgeType.empty, Tile.EdgeType.road, '═', 1 * amplifcationFactor));

        // 3-way stops
        states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.road, Tile.EdgeType.road, Tile.EdgeType.road, '╦', 1 * amplifcationFactor));
        states.Add(new PointsTile(Tile.EdgeType.road, Tile.EdgeType.empty, Tile.EdgeType.road, Tile.EdgeType.road, '╣', 1 * amplifcationFactor));
        states.Add(new PointsTile(Tile.EdgeType.road, Tile.EdgeType.road, Tile.EdgeType.empty, Tile.EdgeType.road, '╩', 1 * amplifcationFactor));
        states.Add(new PointsTile(Tile.EdgeType.road, Tile.EdgeType.road, Tile.EdgeType.road, Tile.EdgeType.empty, '╠', 1 * amplifcationFactor));

        //end points
        states.Add(new PointsTile(Tile.EdgeType.road, Tile.EdgeType.empty, Tile.EdgeType.empty, Tile.EdgeType.empty, '╜', 1));
        states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.road, Tile.EdgeType.empty, Tile.EdgeType.empty, '╘', 1));
        states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.empty, Tile.EdgeType.road, Tile.EdgeType.empty, '╓', 1));
        states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.empty, Tile.EdgeType.empty, Tile.EdgeType.road, '╕', 1));

        //// 4-way stop
        //states.Add(new PointsTile(Tile.EdgeType.trail, Tile.EdgeType.trail, Tile.EdgeType.trail, Tile.EdgeType.trail, '┼', 1 * amplifcationFactor));

        // elbows
        states.Add(new PointsTile(Tile.EdgeType.trail, Tile.EdgeType.trail, Tile.EdgeType.empty, Tile.EdgeType.empty, '└', 1 * amplifcationFactor));
        states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.trail, Tile.EdgeType.trail, Tile.EdgeType.empty, '┌', 1 * amplifcationFactor));
        states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.empty, Tile.EdgeType.trail, Tile.EdgeType.trail, '┐', 1 * amplifcationFactor));
        states.Add(new PointsTile(Tile.EdgeType.trail, Tile.EdgeType.empty, Tile.EdgeType.empty, Tile.EdgeType.trail, '┘', 1 * amplifcationFactor));

        //// straight throughs
        states.Add(new PointsTile(Tile.EdgeType.trail, Tile.EdgeType.empty, Tile.EdgeType.trail, Tile.EdgeType.empty, '│', 1 * amplifcationFactor));
        states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.trail, Tile.EdgeType.empty, Tile.EdgeType.trail, '─', 1 * amplifcationFactor));

        // 3-way stops
        states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.trail, Tile.EdgeType.trail, Tile.EdgeType.trail, '┬', 1 * amplifcationFactor));
        states.Add(new PointsTile(Tile.EdgeType.trail, Tile.EdgeType.empty, Tile.EdgeType.trail, Tile.EdgeType.trail, '┤', 1 * amplifcationFactor));
        states.Add(new PointsTile(Tile.EdgeType.trail, Tile.EdgeType.trail, Tile.EdgeType.empty, Tile.EdgeType.trail, '┴', 1 * amplifcationFactor));
        states.Add(new PointsTile(Tile.EdgeType.trail, Tile.EdgeType.trail, Tile.EdgeType.trail, Tile.EdgeType.empty, '├', 1 * amplifcationFactor));

        // end points
        states.Add(new PointsTile(Tile.EdgeType.trail, Tile.EdgeType.empty, Tile.EdgeType.empty, Tile.EdgeType.empty, '╵', 1));
        states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.trail, Tile.EdgeType.empty, Tile.EdgeType.empty, '╶', 1));
        states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.empty, Tile.EdgeType.trail, Tile.EdgeType.empty, '╷', 1));
        states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.empty, Tile.EdgeType.empty, Tile.EdgeType.trail, '╴', 1));

        //// passes
        //states.Add(new PointsTile(Tile.EdgeType.road, Tile.EdgeType.trail, Tile.EdgeType.road, Tile.EdgeType.trail, '╫', 1));
        //states.Add(new PointsTile(Tile.EdgeType.trail, Tile.EdgeType.road, Tile.EdgeType.trail, Tile.EdgeType.road, '╪', 1));


        // Bob
        states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.alpha, Tile.EdgeType.empty, Tile.EdgeType.empty, 'B', 1 * amplifcationFactor));
        states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.bravo, Tile.EdgeType.empty, Tile.EdgeType.alpha, 'o', 1 * amplifcationFactor));
        states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.empty, Tile.EdgeType.empty, Tile.EdgeType.bravo, 'b', 1 * amplifcationFactor));

        // end loops
        // ╦ ╣ ╩ ╠ ╚ ╔ ╗ ╝
        //
        // ╠ ╗
        // ╚ ╝
        //
        // ╔ ╣
        // ╚ ╝
        //
        // ╔ ╦
        // ╚ ╝
        //
        // ╔ ╗
        // ╚ ╩
        //
        // ╔ ╗
        // ╚ ╣
        //
        // ╔ ╗
        // ╠ ╝
        //
        // ╔ ╗
        // ╩ ╝
        //
        // ╦ ╗
        // ╚ ╝


        //// ╠1╗
        //// 4 2
        //// ╚3╝
        //states.Add(new PointsTile(Tile.EdgeType.road, Tile.EdgeType.delta1, Tile.EdgeType.delta4, Tile.EdgeType.empty, '╠', 1 * amplifcationFactor));
        //states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.empty, Tile.EdgeType.delta2, Tile.EdgeType.delta1, '╗', 1 * amplifcationFactor));
        //states.Add(new PointsTile(Tile.EdgeType.delta2, Tile.EdgeType.empty, Tile.EdgeType.empty, Tile.EdgeType.delta3, '╝', 1 * amplifcationFactor));
        //states.Add(new PointsTile(Tile.EdgeType.delta4, Tile.EdgeType.delta3, Tile.EdgeType.empty, Tile.EdgeType.empty, '╚', 1 * amplifcationFactor));

        //// ╔1╣
        //// 4 2
        //// ╚3╝
        //states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.echo1, Tile.EdgeType.echo4, Tile.EdgeType.empty, '╔', 1 * amplifcationFactor));
        //states.Add(new PointsTile(Tile.EdgeType.road, Tile.EdgeType.empty, Tile.EdgeType.echo2, Tile.EdgeType.echo1, '╣', 1 * amplifcationFactor));
        //states.Add(new PointsTile(Tile.EdgeType.echo2, Tile.EdgeType.empty, Tile.EdgeType.empty, Tile.EdgeType.echo3, '╝', 1 * amplifcationFactor));
        //states.Add(new PointsTile(Tile.EdgeType.echo4, Tile.EdgeType.echo3, Tile.EdgeType.empty, Tile.EdgeType.empty, '╚', 1 * amplifcationFactor));

        //// ╔1╦
        //// 4 2
        //// ╚3╝
        //states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.foxtrot1, Tile.EdgeType.foxtrot4, Tile.EdgeType.empty, '╔', 1 * amplifcationFactor));
        //states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.road, Tile.EdgeType.foxtrot2, Tile.EdgeType.foxtrot1, '╦', 1 * amplifcationFactor));
        //states.Add(new PointsTile(Tile.EdgeType.foxtrot2, Tile.EdgeType.empty, Tile.EdgeType.empty, Tile.EdgeType.foxtrot3, '╝', 1 * amplifcationFactor));
        //states.Add(new PointsTile(Tile.EdgeType.foxtrot4, Tile.EdgeType.foxtrot3, Tile.EdgeType.empty, Tile.EdgeType.empty, '╚', 1 * amplifcationFactor));

        //// ╔1╗
        //// 4 2
        //// ╚3╩
        //states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.golf1, Tile.EdgeType.golf4, Tile.EdgeType.empty, '╔', 1 * amplifcationFactor));
        //states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.empty, Tile.EdgeType.golf2, Tile.EdgeType.golf1, '╗', 1 * amplifcationFactor));
        //states.Add(new PointsTile(Tile.EdgeType.golf2, Tile.EdgeType.road, Tile.EdgeType.empty, Tile.EdgeType.golf3, '╩', 1 * amplifcationFactor));
        //states.Add(new PointsTile(Tile.EdgeType.golf4, Tile.EdgeType.golf3, Tile.EdgeType.empty, Tile.EdgeType.empty, '╚', 1 * amplifcationFactor));

        //// ╔1╗
        //// 4 2
        //// ╚3╣
        //states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.hotel1, Tile.EdgeType.hotel4, Tile.EdgeType.empty, '╔', 1 * amplifcationFactor));
        //states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.empty, Tile.EdgeType.hotel2, Tile.EdgeType.hotel1, '╗', 1 * amplifcationFactor));
        //states.Add(new PointsTile(Tile.EdgeType.hotel2, Tile.EdgeType.empty, Tile.EdgeType.road, Tile.EdgeType.hotel3, '╣', 1 * amplifcationFactor));
        //states.Add(new PointsTile(Tile.EdgeType.hotel4, Tile.EdgeType.hotel3, Tile.EdgeType.empty, Tile.EdgeType.empty, '╚', 1 * amplifcationFactor));

        //// ╔1╗
        //// 4 2
        //// ╠3╝
        //states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.india1, Tile.EdgeType.india4, Tile.EdgeType.empty, '╔', 1 * amplifcationFactor));
        //states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.empty, Tile.EdgeType.india2, Tile.EdgeType.india1, '╗', 1 * amplifcationFactor));
        //states.Add(new PointsTile(Tile.EdgeType.india2, Tile.EdgeType.empty, Tile.EdgeType.empty, Tile.EdgeType.india3, '╝', 1 * amplifcationFactor));
        //states.Add(new PointsTile(Tile.EdgeType.india4, Tile.EdgeType.india3, Tile.EdgeType.road, Tile.EdgeType.empty, '╠', 1 * amplifcationFactor));

        //// ╔1╗
        //// 4 2
        //// ╩3╝
        //states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.juliet1, Tile.EdgeType.juliet4, Tile.EdgeType.empty, '╔', 1 * amplifcationFactor));
        //states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.empty, Tile.EdgeType.juliet2, Tile.EdgeType.juliet1, '╗', 1 * amplifcationFactor));
        //states.Add(new PointsTile(Tile.EdgeType.juliet2, Tile.EdgeType.empty, Tile.EdgeType.empty, Tile.EdgeType.juliet3, '╝', 1 * amplifcationFactor));
        //states.Add(new PointsTile(Tile.EdgeType.juliet4, Tile.EdgeType.juliet3, Tile.EdgeType.empty, Tile.EdgeType.road, '╩', 1 * amplifcationFactor));

        //// ╦1╗
        //// 4 2
        //// ╚3╝
        //states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.kilo1, Tile.EdgeType.kilo4, Tile.EdgeType.road, '╦', 1 * amplifcationFactor));
        //states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.empty, Tile.EdgeType.kilo2, Tile.EdgeType.kilo1, '╗', 1 * amplifcationFactor));
        //states.Add(new PointsTile(Tile.EdgeType.kilo2, Tile.EdgeType.empty, Tile.EdgeType.empty, Tile.EdgeType.kilo3, '╝', 1 * amplifcationFactor));
        //states.Add(new PointsTile(Tile.EdgeType.kilo4, Tile.EdgeType.kilo3, Tile.EdgeType.empty, Tile.EdgeType.empty, '╚', 1 * amplifcationFactor));


        //// condensed end loops

        //// ╔D╗
        //// G E
        //// ╚F╝

        //states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.delta, Tile.EdgeType.golf, Tile.EdgeType.empty, '╔', 1 * amplifcationFactor));
        //states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.empty, Tile.EdgeType.echo, Tile.EdgeType.delta, '╗', 1 * amplifcationFactor));
        //states.Add(new PointsTile(Tile.EdgeType.echo, Tile.EdgeType.empty, Tile.EdgeType.empty, Tile.EdgeType.foxtrot, '╝', 1 * amplifcationFactor));
        //states.Add(new PointsTile(Tile.EdgeType.golf, Tile.EdgeType.foxtrot, Tile.EdgeType.empty, Tile.EdgeType.empty, '╚', 1 * amplifcationFactor));

        //// ╠ ╣ ╦ ╩
        //states.Add(new PointsTile(Tile.EdgeType.road, Tile.EdgeType.delta, Tile.EdgeType.golf, Tile.EdgeType.empty, '╠', 1 * amplifcationFactor));

        //states.Add(new PointsTile(Tile.EdgeType.road, Tile.EdgeType.empty, Tile.EdgeType.echo, Tile.EdgeType.delta, '╣', 1 * amplifcationFactor));
        //states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.road, Tile.EdgeType.echo, Tile.EdgeType.delta, '╦', 1 * amplifcationFactor));

        //states.Add(new PointsTile(Tile.EdgeType.echo, Tile.EdgeType.road, Tile.EdgeType.empty, Tile.EdgeType.foxtrot, '╩', 1 * amplifcationFactor));
        //states.Add(new PointsTile(Tile.EdgeType.echo, Tile.EdgeType.empty, Tile.EdgeType.road, Tile.EdgeType.foxtrot, '╣', 1 * amplifcationFactor));

        //states.Add(new PointsTile(Tile.EdgeType.golf, Tile.EdgeType.foxtrot, Tile.EdgeType.road, Tile.EdgeType.empty, '╠', 1 * amplifcationFactor));
        //states.Add(new PointsTile(Tile.EdgeType.golf, Tile.EdgeType.foxtrot, Tile.EdgeType.empty, Tile.EdgeType.road, '╩', 1 * amplifcationFactor));

        //states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.delta, Tile.EdgeType.golf, Tile.EdgeType.road, '╦', 1 * amplifcationFactor));
    }


    public override void Collapse()
    {
        int beginning_states = states.Count;
        if (states.Count == 0)
            throw new System.Exception("Wave function collapse failed.");
        if (states.Count == 1)
            throw new System.Exception("Attempted to collapse an already-collapsed slot.");

        int value = Utes.rnd.Next() % totalPoints;
        int currentvalue = 0;
        int stateindex = -1;
        for (int i = 0; i < states.Count; i++)
        {
            currentvalue += TileAt(i).points;
            if (currentvalue > value)
            {
                stateindex = i;
                break;
            }
        }
        if (stateindex == -1 || currentvalue < 0 || currentvalue > totalPoints)
            throw new System.Exception("algo failure");

        // pick one of our states at random
        // apply each of its edges as contraints.

        Tile constraint = states[stateindex];

        RemoveStatesWithEdgeTypesNotMatching(Tile.EdgeLocation.up, constraint.up);
        RemoveStatesWithEdgeTypesNotMatching(Tile.EdgeLocation.right, constraint.right);
        RemoveStatesWithEdgeTypesNotMatching(Tile.EdgeLocation.down, constraint.down);
        RemoveStatesWithEdgeTypesNotMatching(Tile.EdgeLocation.left, constraint.left);
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