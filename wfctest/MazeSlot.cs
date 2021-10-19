using System;
using System.Collections;
using System.Collections.Generic;


/* A MazeSlot is a special type of PointsTileSlot used for maze generation. Point values may be derived from bitmap images, thereby
   controlling the distribution of tiles in the maze (e.g. making a pattern similar to the inputted image). */
class MazeSlot : PointsTileSlot
{
    private int pathId = -1;
    public bool outputPathColors = false;

    public MazeSlot(TileSet tileSet, int row, int column) : base(tileSet, row, column)
    { }


    public override void PopulateTileSet()
    {
        if (states.Count != 0)
            throw new System.ArgumentException("Attempted to populate a tile slot that already contains tiles.  Expected to only populate empty tile slots.");

        //// elbows
        //states.Add(new PointsTile(Tile.EdgeType.trail, Tile.EdgeType.trail, Tile.EdgeType.empty, Tile.EdgeType.empty, '╰', 1 * 5));
        //states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.trail, Tile.EdgeType.trail, Tile.EdgeType.empty, '╭', 1 * 5));
        //states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.empty, Tile.EdgeType.trail, Tile.EdgeType.trail, '╮', 1 * 5));
        //states.Add(new PointsTile(Tile.EdgeType.trail, Tile.EdgeType.empty, Tile.EdgeType.empty, Tile.EdgeType.trail, '╯', 1 * 5));

        ////// straight throughs
        //states.Add(new PointsTile(Tile.EdgeType.trail, Tile.EdgeType.empty, Tile.EdgeType.trail, Tile.EdgeType.empty, '│', 20 * 10));
        //states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.trail, Tile.EdgeType.empty, Tile.EdgeType.trail, '─', 20 * 10));

        //// 3-way stops
        //states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.trail, Tile.EdgeType.trail, Tile.EdgeType.trail, '┬', 1 * 10));
        //states.Add(new PointsTile(Tile.EdgeType.trail, Tile.EdgeType.empty, Tile.EdgeType.trail, Tile.EdgeType.trail, '┤', 1 * 10));
        //states.Add(new PointsTile(Tile.EdgeType.trail, Tile.EdgeType.trail, Tile.EdgeType.empty, Tile.EdgeType.trail, '┴', 1 * 10));
        //states.Add(new PointsTile(Tile.EdgeType.trail, Tile.EdgeType.trail, Tile.EdgeType.trail, Tile.EdgeType.empty, '├', 1 * 10));        

        // elbows
        states.Add(new PointsTile(Tile.EdgeType.trail, Tile.EdgeType.trail, Tile.EdgeType.empty, Tile.EdgeType.empty, '╰', 1));
        states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.trail, Tile.EdgeType.trail, Tile.EdgeType.empty, '╭', 1));
        states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.empty, Tile.EdgeType.trail, Tile.EdgeType.trail, '╮', 1));
        states.Add(new PointsTile(Tile.EdgeType.trail, Tile.EdgeType.empty, Tile.EdgeType.empty, Tile.EdgeType.trail, '╯', 1));

        //// straight throughs
        states.Add(new PointsTile(Tile.EdgeType.trail, Tile.EdgeType.empty, Tile.EdgeType.trail, Tile.EdgeType.empty, '│', 10));
        states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.trail, Tile.EdgeType.empty, Tile.EdgeType.trail, '─', 10));

        // 3-way stops
        states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.trail, Tile.EdgeType.trail, Tile.EdgeType.trail, '┬', 1));
        states.Add(new PointsTile(Tile.EdgeType.trail, Tile.EdgeType.empty, Tile.EdgeType.trail, Tile.EdgeType.trail, '┤', 1));
        states.Add(new PointsTile(Tile.EdgeType.trail, Tile.EdgeType.trail, Tile.EdgeType.empty, Tile.EdgeType.trail, '┴', 1));
        states.Add(new PointsTile(Tile.EdgeType.trail, Tile.EdgeType.trail, Tile.EdgeType.trail, Tile.EdgeType.empty, '├', 1));

        // end points
        states.Add(new PointsTile(Tile.EdgeType.trail, Tile.EdgeType.empty, Tile.EdgeType.empty, Tile.EdgeType.empty, '╵', 1));
        states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.trail, Tile.EdgeType.empty, Tile.EdgeType.empty, '╶', 1));
        states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.empty, Tile.EdgeType.trail, Tile.EdgeType.empty, '╷', 1));
        states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.empty, Tile.EdgeType.empty, Tile.EdgeType.trail, '╴', 1));
    }


    public void PopulateTileSet3()
    {
        System.IO.FileStream fs = new System.IO.FileStream("input.bmp", System.IO.FileMode.Open, System.IO.FileAccess.Read);
        //System.Graphics.Bitmap bmp = new System.Graphics.Bitmap()
        System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(fs);

        System.Drawing.Color c = bmp.GetPixel(column, row);
        const int range = 1500;

        if (states.Count != 0)
            throw new System.ArgumentException("Attempted to populate a tile slot that already contains tiles.  Expected to only populate empty tile slots.");

        // elbows
        //states.Add(new PointsTile(Tile.EdgeType.trail, Tile.EdgeType.trail, Tile.EdgeType.empty, Tile.EdgeType.empty, '╰', 1));
        //states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.trail, Tile.EdgeType.trail, Tile.EdgeType.empty, '╭', 1));
        //states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.empty, Tile.EdgeType.trail, Tile.EdgeType.trail, '╮', 1));
        //states.Add(new PointsTile(Tile.EdgeType.trail, Tile.EdgeType.empty, Tile.EdgeType.empty, Tile.EdgeType.trail, '╯', 1));
        states.Add(new PointsTile(Tile.EdgeType.trail, Tile.EdgeType.trail, Tile.EdgeType.empty, Tile.EdgeType.empty, '└', 1));
        states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.trail, Tile.EdgeType.trail, Tile.EdgeType.empty, '┌', 1));
        states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.empty, Tile.EdgeType.trail, Tile.EdgeType.trail, '┐', 1));
        states.Add(new PointsTile(Tile.EdgeType.trail, Tile.EdgeType.empty, Tile.EdgeType.empty, Tile.EdgeType.trail, '┘', 1));

        //// straight throughs
        states.Add(new PointsTile(Tile.EdgeType.trail, Tile.EdgeType.empty, Tile.EdgeType.trail, Tile.EdgeType.empty, '│', 1 + (int)(c.GetBrightness() * range)));
        states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.trail, Tile.EdgeType.empty, Tile.EdgeType.trail, '─', 1 + (int)((1f - c.GetBrightness()) * range)));

        // 3-way stops
        states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.trail, Tile.EdgeType.trail, Tile.EdgeType.trail, '┬', 1));
        states.Add(new PointsTile(Tile.EdgeType.trail, Tile.EdgeType.empty, Tile.EdgeType.trail, Tile.EdgeType.trail, '┤', 1));
        states.Add(new PointsTile(Tile.EdgeType.trail, Tile.EdgeType.trail, Tile.EdgeType.empty, Tile.EdgeType.trail, '┴', 1));
        states.Add(new PointsTile(Tile.EdgeType.trail, Tile.EdgeType.trail, Tile.EdgeType.trail, Tile.EdgeType.empty, '├', 1));

        // end points
        states.Add(new PointsTile(Tile.EdgeType.trail, Tile.EdgeType.empty, Tile.EdgeType.empty, Tile.EdgeType.empty, '╵', 1 + (int)(c.GetBrightness() * 100)));
        states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.trail, Tile.EdgeType.empty, Tile.EdgeType.empty, '╶', 1 + (int)((1f - c.GetBrightness()) * 100)));
        states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.empty, Tile.EdgeType.trail, Tile.EdgeType.empty, '╷', 1 + (int)(c.GetBrightness() * 100)));
        states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.empty, Tile.EdgeType.empty, Tile.EdgeType.trail, '╴', 1 + (int)((1f - c.GetBrightness()) * 100)));
    }


    public void PopulateTileSet2()
    {
        if (states.Count != 0)
            throw new System.ArgumentException("Attempted to populate a tile slot that already contains tiles.  Expected to only populate empty tile slots.");

        // elbows
        states.Add(new PointsTile(Tile.EdgeType.trail, Tile.EdgeType.trail, Tile.EdgeType.empty, Tile.EdgeType.empty, '└', 1));
        states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.trail, Tile.EdgeType.trail, Tile.EdgeType.empty, '┌', 1));
        states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.empty, Tile.EdgeType.trail, Tile.EdgeType.trail, '┐', 1));
        states.Add(new PointsTile(Tile.EdgeType.trail, Tile.EdgeType.empty, Tile.EdgeType.empty, Tile.EdgeType.trail, '┘', 1));

        //// straight throughs
        states.Add(new PointsTile(Tile.EdgeType.trail, Tile.EdgeType.empty, Tile.EdgeType.trail, Tile.EdgeType.empty, '│', 1));
        states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.trail, Tile.EdgeType.empty, Tile.EdgeType.trail, '─', 1));

        // 3-way stops
        states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.trail, Tile.EdgeType.trail, Tile.EdgeType.trail, '┬', 1));
        states.Add(new PointsTile(Tile.EdgeType.trail, Tile.EdgeType.empty, Tile.EdgeType.trail, Tile.EdgeType.trail, '┤', 1));
        states.Add(new PointsTile(Tile.EdgeType.trail, Tile.EdgeType.trail, Tile.EdgeType.empty, Tile.EdgeType.trail, '┴', 1));
        states.Add(new PointsTile(Tile.EdgeType.trail, Tile.EdgeType.trail, Tile.EdgeType.trail, Tile.EdgeType.empty, '├', 1));

        // end points
        states.Add(new PointsTile(Tile.EdgeType.trail, Tile.EdgeType.empty, Tile.EdgeType.empty, Tile.EdgeType.empty, '╵', 1));
        states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.trail, Tile.EdgeType.empty, Tile.EdgeType.empty, '╶', 1));
        states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.empty, Tile.EdgeType.trail, Tile.EdgeType.empty, '╷', 1));
        states.Add(new PointsTile(Tile.EdgeType.empty, Tile.EdgeType.empty, Tile.EdgeType.empty, Tile.EdgeType.trail, '╴', 1));
    }


    public bool CheckWhetherPathEnters(int pathIdArg, MazeSlot excludingSlot)
    {
        if (pathIdArg < 0)
            throw new System.Exception("Invalid path id provided. Provided " + pathIdArg + " when an integer >= 0 was expected.");

        if (collapsed)
            throw new System.ArgumentException("Called CheckWhetherPathEnters() on an already-collapsed slot. CheckWhetherPathEnters() should only be called on non-collapsed slots.");

        // path definitely enters
        //if (this.pathId == pathIdArg)
        //    return true;
        else if (upSlotValid && upSlot.collapsed && upSlot.constraints.down == Tile.EdgeType.trail && upSlot.pathId == pathIdArg && upSlot != excludingSlot)
            return true;
        else if (rightSlotValid && rightSlot.collapsed && rightSlot.constraints.left == Tile.EdgeType.trail && rightSlot.pathId == pathIdArg && rightSlot != excludingSlot)
            return true;
        else if (downSlotValid && downSlot.collapsed && downSlot.constraints.up == Tile.EdgeType.trail && downSlot.pathId == pathIdArg && downSlot != excludingSlot)
            return true;
        else if (leftSlotValid && leftSlot.collapsed && leftSlot.constraints.right == Tile.EdgeType.trail && leftSlot.pathId == pathIdArg && leftSlot != excludingSlot)
            return true;

        // path is capable of entering
        else if (upSlotValid && !upSlot.collapsed && upSlot.ContainsEdge(Tile.EdgeLocation.down, Tile.EdgeType.trail) && upSlot.pathId == pathIdArg && upSlot != excludingSlot)
            return true;
        else if (rightSlotValid && !rightSlot.collapsed && rightSlot.ContainsEdge(Tile.EdgeLocation.left, Tile.EdgeType.trail) && rightSlot.pathId == pathIdArg && rightSlot != excludingSlot)
            return true;
        else if (downSlotValid && !downSlot.collapsed && downSlot.ContainsEdge(Tile.EdgeLocation.up, Tile.EdgeType.trail) && downSlot.pathId == pathIdArg && downSlot != excludingSlot)
            return true;
        else if (leftSlotValid && !leftSlot.collapsed && leftSlot.ContainsEdge(Tile.EdgeLocation.right, Tile.EdgeType.trail) && leftSlot.pathId == pathIdArg && leftSlot != excludingSlot)
            return true;

        return false;
    }


    public override void Collapse()
    {
        base.Collapse();

        RecursivelyAddressPathConstraints(this);
    }


    public void ApplyPathConstraints()
    {
        if (pathId == -1)
            throw new System.ArgumentException("Attempted to apply path constraints to a slot without a pathId.");

        if (!collapsed)
            return;

        /*
            When a path runs off into an uncollapsed slot, from that slot, we need to check whether the
            current path enters its neighbors.  If the current path does enter a neighbor, then the current
            slot is prohibited from also entering that square.
        */
        if (upSlotValid && constraints.up == Tile.EdgeType.trail)
        {
            // check upSlot's neighbors
            if (!upSlot.collapsed && !upSlot.upSlot.collapsed && upSlot.upSlot.CheckWhetherPathEnters(pathId, upSlot))
            {
                // upSlot cannot go up
                upSlot.RemoveStatesWithEdgeTypesNotMatching(Tile.EdgeLocation.up, Tile.EdgeType.empty);
            }
            if (!upSlot.collapsed && !upSlot.rightSlot.collapsed && upSlot.rightSlot.CheckWhetherPathEnters(pathId, upSlot))
            {
                // upSlot cannot go right
                upSlot.RemoveStatesWithEdgeTypesNotMatching(Tile.EdgeLocation.right, Tile.EdgeType.empty);
            }
            // skip
            //if (!upSlot.collapsed && !upSlot.downSlot.collapsed && upSlot.downSlot.CheckWhetherPathEnters(pathId, upSlot))
            //{
            //    // upSlot cannot go down
            //      upSlot.RemoveStatesWithEdgeTypesNotMatching(Tile.EdgeLocation.down, Tile.EdgeType.empty);
            //}
            if (!upSlot.collapsed && !upSlot.leftSlot.collapsed && upSlot.leftSlot.CheckWhetherPathEnters(pathId, upSlot))
            {
                // upSlot cannot go left
                upSlot.RemoveStatesWithEdgeTypesNotMatching(Tile.EdgeLocation.left, Tile.EdgeType.empty);
            }
        }
        if (rightSlotValid && constraints.right == Tile.EdgeType.trail)
        {
            // check rightSlot's neighbors
            if (!rightSlot.collapsed && !rightSlot.upSlot.collapsed && rightSlot.upSlot.CheckWhetherPathEnters(pathId, rightSlot))
            {
                // rightSlot cannot go up
                rightSlot.RemoveStatesWithEdgeTypesNotMatching(Tile.EdgeLocation.up, Tile.EdgeType.empty);
            }
            if (!rightSlot.collapsed && !rightSlot.rightSlot.collapsed && rightSlot.rightSlot.CheckWhetherPathEnters(pathId, rightSlot))
            {
                // rightSlot cannot go right
                rightSlot.RemoveStatesWithEdgeTypesNotMatching(Tile.EdgeLocation.right, Tile.EdgeType.empty);
            }
            if (!rightSlot.collapsed && !rightSlot.downSlot.collapsed && rightSlot.downSlot.CheckWhetherPathEnters(pathId, rightSlot))
            {
                // rightSlot cannot go down
                rightSlot.RemoveStatesWithEdgeTypesNotMatching(Tile.EdgeLocation.down, Tile.EdgeType.empty);
            }
            // skip
            //if (!rightSlot.collapsed && !rightSlot.leftSlot.collapsed && rightSlot.leftSlot.CheckWhetherPathEnters(pathId, rightSlot))
            //{
            //    // rightSlot cannot go left
            //    rightSlot.RemoveStatesWithEdgeTypesNotMatching(Tile.EdgeLocation.left, Tile.EdgeType.empty);
            //}
        }
        if (downSlotValid && constraints.down == Tile.EdgeType.trail)
        {
            // check downSlot's neighbors
            // skip
            //if (!downSlot.collapsed && !downSlot.upSlot.collapsed && downSlot.upSlot.CheckWhetherPathEnters(pathId, downSlot))
            //{
            //    // downSlot cannot go up
            //    downSlot.RemoveStatesWithEdgeTypesNotMatching(Tile.EdgeLocation.up, Tile.EdgeType.empty);
            //}
            if (!downSlot.collapsed && !downSlot.rightSlot.collapsed && downSlot.rightSlot.CheckWhetherPathEnters(pathId, downSlot))
            {
                // downSlot cannot go right
                downSlot.RemoveStatesWithEdgeTypesNotMatching(Tile.EdgeLocation.right, Tile.EdgeType.empty);
            }
            if (!downSlot.collapsed && !downSlot.downSlot.collapsed && downSlot.downSlot.CheckWhetherPathEnters(pathId, downSlot))
            {
                // downSlot cannot go down
                downSlot.RemoveStatesWithEdgeTypesNotMatching(Tile.EdgeLocation.down, Tile.EdgeType.empty);
            }
            if (!downSlot.collapsed && !downSlot.leftSlot.collapsed && downSlot.leftSlot.CheckWhetherPathEnters(pathId, downSlot))
            {
                // downSlot cannot go left
                downSlot.RemoveStatesWithEdgeTypesNotMatching(Tile.EdgeLocation.left, Tile.EdgeType.empty);
            }
        }
        if (leftSlotValid && constraints.left == Tile.EdgeType.trail)
        {
            // check leftSlot's neighbors
            if (!leftSlot.collapsed && !leftSlot.upSlot.collapsed && leftSlot.upSlot.CheckWhetherPathEnters(pathId, leftSlot))
            {
                // leftSlot cannot go up
                leftSlot.RemoveStatesWithEdgeTypesNotMatching(Tile.EdgeLocation.up, Tile.EdgeType.empty);
            }
            // skip
            //if (!leftSlot.collapsed && !leftSlot.rightSlot.collapsed && leftSlot.rightSlot.CheckWhetherPathEnters(pathId, leftSlot))
            //{
            //    // leftSlot cannot go right
            //    leftSlot.RemoveStatesWithEdgeTypesNotMatching(Tile.EdgeLocation.right, Tile.EdgeType.empty);
            //}
            if (!leftSlot.collapsed && !leftSlot.downSlot.collapsed && leftSlot.downSlot.CheckWhetherPathEnters(pathId, leftSlot))
            {
                // leftSlot cannot go down
                leftSlot.RemoveStatesWithEdgeTypesNotMatching(Tile.EdgeLocation.down, Tile.EdgeType.empty);
            }
            if (!leftSlot.collapsed && !leftSlot.leftSlot.collapsed && leftSlot.leftSlot.CheckWhetherPathEnters(pathId, leftSlot))
            {
                // leftSlot cannot go left
                leftSlot.RemoveStatesWithEdgeTypesNotMatching(Tile.EdgeLocation.left, Tile.EdgeType.empty);
            }
        }

        // alternative implementation
        //foreach(Tile.EdgeLocation location in Enum.GetValues(typeof(Tile.EdgeLocation)))
        //{
        //    if(currentState.GetEdgeType(location) == Tile.EdgeType.trail)
        //    {
        //        MazeSlot targetSlot = (MazeSlot)GetSlotByLocation(location); // Need a better class layout to eliminate these darned typecasts.

        //        Tile.EdgeLocation relativeLocation = Tile.GetLocationByLocationOffset(location, -1);
        //        MazeSlot relativeSlot = (MazeSlot)GetSlotByLocation(relativeLocation);
        //        if (relativeSlot.pathId == pathId && relativeSlot.IsCollapsedTo(location, Tile.EdgeType.trail))
        //            targetSlot.RemoveStatesWithEdgeTypesMatching(relativeLocation, Tile.EdgeType.trail);

        //        relativeLocation = Tile.GetLocationByLocationOffset(location, 1);
        //        relativeSlot = (MazeSlot)GetSlotByLocation(relativeLocation);
        //        if (relativeSlot.pathId == pathId && relativeSlot.IsCollapsedTo(location, Tile.EdgeType.trail))
        //            targetSlot.RemoveStatesWithEdgeTypesMatching(relativeLocation, Tile.EdgeType.trail);
        //    }
        //}
    }


    public void RecursivelyAddressPathConstraints(MazeSlot caller)
    {
        if (upSlotValid && upSlot != caller)
        {
            if ((collapsed && constraints.up == Tile.EdgeType.trail) ||
                (upSlot.collapsed && upSlot.constraints.down == Tile.EdgeType.trail))
            {
                if (upSlot.pathId != pathId)
                    throw new System.Exception();

                upSlot.RecursivelyAddressPathConstraints(this);
            }
        }
        if (rightSlotValid && rightSlot != caller)
        {
            if ((collapsed && constraints.right == Tile.EdgeType.trail) ||
                (rightSlot.collapsed && rightSlot.constraints.left == Tile.EdgeType.trail))
            {
                if (rightSlot.pathId != pathId)
                    throw new System.Exception();

                rightSlot.RecursivelyAddressPathConstraints(this);
            }
        }
        if (downSlotValid && downSlot != caller)
        {
            if ((collapsed && constraints.down == Tile.EdgeType.trail) ||
                (downSlot.collapsed && downSlot.constraints.up == Tile.EdgeType.trail))
            {
                if (downSlot.pathId != pathId)
                    throw new System.Exception();

                downSlot.RecursivelyAddressPathConstraints(this);
            }
        }
        if (leftSlotValid && leftSlot != caller)
        {
            if ((collapsed && constraints.left == Tile.EdgeType.trail) ||
                (leftSlot.collapsed && leftSlot.constraints.right == Tile.EdgeType.trail))
            {
                if (leftSlot.pathId != pathId)
                    throw new System.Exception();

                leftSlot.RecursivelyAddressPathConstraints(this);
            }
        }

        ApplyPathConstraints();
    }


    // TODO: I really don't think this should be here.  MazeSlot's base should be templated such that it provides the appropriate return types automatically.  i.e. MazeSlot : TileSlot<MazeSlot>
    public new MazeSlot upSlot
    {
        get
        {
            return (MazeSlot)tileSet[row - 1, column];
        }
    }


    public new MazeSlot rightSlot
    {
        get
        {
            return (MazeSlot)tileSet[row, column + 1];
        }
    }


    public new MazeSlot downSlot
    {
        get
        {
            return (MazeSlot)tileSet[row + 1, column];
        }
    }


    public new MazeSlot leftSlot
    {
        get
        {
            return (MazeSlot)tileSet[row, column - 1];
        }
    }


    public override void RemoveStatesWithEdgeTypesNotMatching(Tile.EdgeLocation location, Tile.EdgeType constraint)
    {
        List<Tile> removedStates = new List<Tile>();

        // TODO: This should use GetEdgeType() instead of a switch statement.
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

        if (removedStates.Count > 0 && states.Count == 1)
        {
            // The slot just collapsed.
            int newPathId = int.MaxValue;
            foreach (Tile.EdgeLocation pathloc in Enum.GetValues(typeof(Tile.EdgeLocation)))
            {
                MazeSlot targetSlot = (MazeSlot)GetSlotByLocation(pathloc);
                if (targetSlot != null && targetSlot.pathId != -1 && constraints.GetEdgeType(pathloc) == Tile.EdgeType.trail && targetSlot.pathId < newPathId)
                {
                    newPathId = targetSlot.pathId;
                    //break;
                }
            }
            if (newPathId == int.MaxValue)
                newPathId = ((MazeTileSet)tileSet).GeneratePathId(this);

            RecursivelySetPathId(newPathId);
        }

        if (tileSet.realtimeConsoleOutput)
        {
            Console.SetCursorPosition(column, row);
            DebugOut();
        }

        if (states.Count == 0)
            throw new System.Exception("Contradiction occurred while applying constraints.");

        // TODO: The RemovedStates* methods are duplicating code.
        for (int i = 0; i < removedStates.Count; i++)
        {
            if (!ContainsEdge(Tile.EdgeLocation.up, removedStates[i].up) && upSlotValid) // This means states of a particular type on an edge were all removed, so that fact may be propagated to the next tile.
                upSlot.RemoveStatesWithEdgeTypesMatching(Tile.EdgeLocation.down, removedStates[i].up);
            if (!ContainsEdge(Tile.EdgeLocation.right, removedStates[i].right) && rightSlotValid)
                rightSlot.RemoveStatesWithEdgeTypesMatching(Tile.EdgeLocation.left, removedStates[i].right);
            if (!ContainsEdge(Tile.EdgeLocation.down, removedStates[i].down) && downSlotValid)
                downSlot.RemoveStatesWithEdgeTypesMatching(Tile.EdgeLocation.up, removedStates[i].down);
            if (!ContainsEdge(Tile.EdgeLocation.left, removedStates[i].left) && leftSlotValid)
                leftSlot.RemoveStatesWithEdgeTypesMatching(Tile.EdgeLocation.right, removedStates[i].left);
        }
    }


    public override void RemoveStatesWithEdgeTypesMatching(Tile.EdgeLocation location, Tile.EdgeType constraint)
    {
        List<Tile> removedStates = new List<Tile>();
        // TODO: This should use GetEdgeType() instead of a switch statement.
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

        if (removedStates.Count > 0 && states.Count == 1)
        {
            // Slot just collapsed.
            int newPathId = int.MaxValue;
            foreach (Tile.EdgeLocation pathloc in Enum.GetValues(typeof(Tile.EdgeLocation)))
            {
                MazeSlot targetSlot = (MazeSlot)GetSlotByLocation(pathloc);
                if (targetSlot != null && targetSlot.pathId != -1 && constraints.GetEdgeType(pathloc) == Tile.EdgeType.trail && targetSlot.pathId < newPathId)
                {
                    newPathId = targetSlot.pathId;
                    //break;
                }
            }
            if (newPathId == -1)
                newPathId = ((MazeTileSet)tileSet).GeneratePathId(this);

            RecursivelySetPathId(newPathId);
        }

        if (tileSet.realtimeConsoleOutput)
        {
            Console.SetCursorPosition(column, row);
            DebugOut();
        }

        if (states.Count == 0)
            throw new System.Exception("Contradiction occurred while applying constraints.");

        // TODO: The RemovedStates* methods are duplicating code.
        for (int i = 0; i < removedStates.Count; i++)
        {
            if (!ContainsEdge(Tile.EdgeLocation.up, removedStates[i].up) && upSlotValid) // This means states of a particular type on an edge were all removed, so that fact may be propagated to the next tile.
                upSlot.RemoveStatesWithEdgeTypesMatching(Tile.EdgeLocation.down, removedStates[i].up);
            if (!ContainsEdge(Tile.EdgeLocation.right, removedStates[i].right) && rightSlotValid)
                rightSlot.RemoveStatesWithEdgeTypesMatching(Tile.EdgeLocation.left, removedStates[i].right);
            if (!ContainsEdge(Tile.EdgeLocation.down, removedStates[i].down) && downSlotValid)
                downSlot.RemoveStatesWithEdgeTypesMatching(Tile.EdgeLocation.up, removedStates[i].down);
            if (!ContainsEdge(Tile.EdgeLocation.left, removedStates[i].left) && leftSlotValid)
                leftSlot.RemoveStatesWithEdgeTypesMatching(Tile.EdgeLocation.right, removedStates[i].left);
        }
    }


    private void RecursivelySetPathId(int newPathId)
    {
        pathId = newPathId;

        if (tileSet.realtimeConsoleOutput && outputPathColors)
        {
            Console.SetCursorPosition(column, row);
            DebugOut();
        }

        foreach (Tile.EdgeLocation location in Enum.GetValues(typeof(Tile.EdgeLocation)))
        {
            MazeSlot targetSlot = (MazeSlot)GetSlotByLocation(location);
            if (targetSlot == null || targetSlot.pathId == newPathId)
                continue;
            if (targetSlot.collapsed && targetSlot.constraints.GetEdgeTypeOpposite(location) == Tile.EdgeType.trail)
                targetSlot.RecursivelySetPathId(newPathId);
            else if (collapsed && constraints.GetEdgeType(location) == Tile.EdgeType.trail)
                targetSlot.RecursivelySetPathId(newPathId);
        }
    }





    private static ConsoleColor[] colors = { ConsoleColor.Blue, ConsoleColor.Cyan, ConsoleColor.DarkBlue, ConsoleColor.DarkCyan,
        ConsoleColor.DarkGray, ConsoleColor.DarkGreen, ConsoleColor.DarkMagenta, ConsoleColor.DarkRed, ConsoleColor.DarkYellow,
        ConsoleColor.Green, ConsoleColor.Magenta, ConsoleColor.Red, ConsoleColor.White, ConsoleColor.Yellow };


    public override void DebugOut()
    {
        ConsoleColor color = Console.BackgroundColor;
        if (outputPathColors)
        {
            if (pathId == -1)
                Console.BackgroundColor = ConsoleColor.Black;
            else
                Console.BackgroundColor = colors[pathId % colors.Length];
        }
        else
            color = ConsoleColor.Black;
        base.DebugOut();
        Console.BackgroundColor = color;
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