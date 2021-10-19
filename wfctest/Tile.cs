using System;
using System.Collections;
using System.Collections.Generic;

/* Tile is the base type for tile-based wave function collapse implementations. A tile is an object/cell/unit that
   has four sides and can be placed in a set of other tiles with adjacent sides. */
class Tile
{
    public enum EdgeType
    {
        empty,
        road,
        trail,
        alpha, // EdgeTypes are arbitrary symbols representing compatible tile sides. A particular tile may have alpha on its right edge, while another may have alpha on its left, indicating that these tiles may associate with left-right adjacency.
        bravo,
        delta,
        //delta1,
        //delta2,
        //delta3,
        //delta4,
        echo,
        //echo1,
        //echo2,
        //echo3,
        //echo4,
        foxtrot,
        //foxtrot1,
        //foxtrot2,
        //foxtrot3,
        //foxtrot4,
        golf,
        //golf1,
        //golf2,
        //golf3,
        //golf4,
        //hotel,
        //hotel1,
        //hotel2,
        //hotel3,
        //hotel4,
        //india,
        //india1,
        //india2,
        //india3,
        //india4,
        //juliet,
        //juliet1,
        //juliet2,
        //juliet3,
        //juliet4,
        //kilo,
        //kilo1,
        //kilo2,
        //kilo3,
        //kilo4
    }

    public enum EdgeLocation
    {
        up,
        right,
        down,
        left
    }

    private const int edgeCount = 4;
    private EdgeType[] edges = new EdgeType[edgeCount];
    protected char symbol;

    public EdgeType up
    {
        get
        {
            return edges[0];
        }
    }

    public EdgeType right
    {
        get
        {
            return edges[1];
        }
    }

    public EdgeType down
    {
        get
        {
            return edges[2];
        }
    }

    public EdgeType left
    {
        get
        {
            return edges[3];
        }
    }


    public Tile(EdgeType up, EdgeType right, EdgeType down, EdgeType left, char symbol)
    {
        edges[0] = up;
        edges[1] = right;
        edges[2] = down;
        edges[3] = left;
        this.symbol = symbol;
    }


    public virtual Tile CreateCopy()
    {
        return new Tile(edges[0], edges[1], edges[2], edges[3], symbol);
    }


    public override string ToString()
    {
        return symbol.ToString();
    }


    public EdgeType GetEdgeType(EdgeLocation location)
    {
        switch (location)
        {
            case EdgeLocation.up:
                return edges[0];
            case EdgeLocation.right:
                return edges[1];
            case EdgeLocation.down:
                return edges[2];
            case EdgeLocation.left:
                return edges[3];
            default:
                throw new System.ArgumentException("unknown type");
        }
    }


    public EdgeType GetEdgeTypeOpposite(EdgeLocation location)
    {
        switch (location)
        {
            case EdgeLocation.up:
                return edges[2];
            case EdgeLocation.right:
                return edges[3];
            case EdgeLocation.down:
                return edges[0];
            case EdgeLocation.left:
                return edges[1];
            default:
                throw new System.ArgumentException("unknown type");
        }
    }



    public static int WrapLocationIndex(int index)
    {
        if (index < 0)
        {
            index = -index;
            index = index % edgeCount;
            index = edgeCount - index;
        }
        else
        {
            index = index % edgeCount;
        }

        return index;
    }


    public static int GetIndexByLocation(EdgeLocation location)
    {
        switch (location)
        {
            case EdgeLocation.up:
                return 0;
            case EdgeLocation.right:
                return 1;
            case EdgeLocation.down:
                return 2;
            case EdgeLocation.left:
                return 3;
            default:
                throw new System.ArgumentException("invalid location");
        }
    }


    public static EdgeLocation GetLocationByLocationOffset(EdgeLocation location, int offset)
    {
        return GetLocationByIndex(GetIndexByLocation(location) + offset);
    }


    public static EdgeLocation GetLocationByIndex(int index)
    {
        int actualIndex = WrapLocationIndex(index);
        switch (actualIndex)
        {
            case 0:
                return EdgeLocation.up;
            case 1:
                return EdgeLocation.right;
            case 2:
                return EdgeLocation.down;
            case 3:
                return EdgeLocation.left;
            default:
                throw new System.ArgumentException("invalid index");
        }
    }


    public void DebugOut()
    {
        Console.Write(this);
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