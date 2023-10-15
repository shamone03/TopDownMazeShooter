using System;
using System.Collections.Generic;

// this is just a regular c# class no monobehaviour NO TOUCH!!!
public static class StaggeredGenerator {
    [Flags]
    public enum NodeState : int {
        // negate and & to add, | to add 
        Left      = 1 << 0, // 000000001
        UpLeft    = 1 << 1, // 000000010
        UpRight   = 1 << 2, // 000000100
        Right     = 1 << 3, // 000001000
        DownLeft  = 1 << 4, // 000010000
        DownRight = 1 << 5, // 000100000
        None      = 1 << 6, // 001000000
        Visited   = 1 << 7, // 010000000
        Current   = 1 << 8, // 010000000
        Neighbour = 1 << 9  // 010000000
    }

    private struct Position {
        public int X;
        public int Y;
    }

    private struct Neighbour {
        public Position Position;
        public NodeState SharedWall;
    }

    private static NodeState GetOppositeWall(NodeState node) {
        switch (node) {
            case NodeState.Right: return NodeState.Left;     
            case NodeState.Left: return NodeState.Right;
            
            case NodeState.UpLeft: return NodeState.DownRight;         
            case NodeState.DownRight: return NodeState.UpLeft;
            
            case NodeState.DownLeft: return NodeState.UpRight;
            case NodeState.UpRight: return NodeState.DownLeft;
            
            
            default: return NodeState.None; // shouldn't happend
        }
    }

    private static IEnumerable<NodeState[,]> RecursiveBacktracker(NodeState[,] maze, int width, int height) {

        Random rng = new Random();
        Stack<Position> positions = new Stack<Position>();
        
        // random start
        // Position pos = new Position { x = rng.Next(0, width), y = rng.Next(0, height) };
        Position pos = new Position { X = 0, Y = 0 };
        maze[pos.X, pos.Y] |= NodeState.Visited;
        positions.Push(pos);

        while (positions.Count > 0) {
            Position current = positions.Pop();
            maze[current.X, current.Y] |= NodeState.Current;
            List<Neighbour> neighbours = GetUnvisitedNeighbours(current, maze, width, height);

            if (neighbours.Count > 0) {
                // push because this node still has unvisited neighbours
                positions.Push(current);
                Neighbour randNeighbour = neighbours[rng.Next(0, neighbours.Count)];
                maze[randNeighbour.Position.X, randNeighbour.Position.Y] |= NodeState.Neighbour;

                // remove walls
                maze[current.X, current.Y] &= ~randNeighbour.SharedWall;
                maze[randNeighbour.Position.X, randNeighbour.Position.Y] &= ~GetOppositeWall(randNeighbour.SharedWall);

                maze[randNeighbour.Position.X, randNeighbour.Position.Y] |= NodeState.Visited;
                
                positions.Push(randNeighbour.Position);
                yield return maze;
                maze[current.X, current.Y] &= ~NodeState.Current;
                maze[randNeighbour.Position.X, randNeighbour.Position.Y] &= ~NodeState.Neighbour;
            }
        }
        yield return maze;
    }
    private static List<Neighbour> GetUnvisitedNeighbours(Position p, NodeState[,] maze, int width, int height) {
        List<Neighbour> list = new List<Neighbour>();

        // DO NOT CHANGE ORDER OF ARRAYS
        int[] dx = { -1, 1,  0, 0, 1,  1, -1, -1 };
        int[] dy = {  0, 0, -1, 1, 1, -1,  1, -1 };
        // neighbourPos = (x + dx[i], y + dy[i]), sharedWall = states[i][p.y % 2]
        // assumes even rows shifted left
        NodeState[,] states = { 
            // when y even    when y odd
            { NodeState.Left, NodeState.Left },          // x - 1, y 
            { NodeState.Right, NodeState.Right },        // x + 1, y
            { NodeState.DownRight, NodeState.DownLeft }, // x, y - 1
            { NodeState.UpRight, NodeState.UpLeft },     // x, y + 1
            // not a neighbour
            { NodeState.None, NodeState.UpRight},        // x + 1, y + 1
            { NodeState.None, NodeState.DownRight},      // x + 1, y - 1
            //                  not a neighbour
            { NodeState.UpLeft, NodeState.None},         // x - 1, y + 1
            { NodeState.DownLeft, NodeState.None}        // x - 1, y - 1
        };

        for (int i = 0; i < dx.Length; i++) {
            int x = p.X + dx[i];
            int y = p.Y + dy[i];

            if (x >= 0 && y >= 0 && x < width && y < height) {
                if (!maze[x, y].HasFlag(NodeState.Visited)) {
                    NodeState sharedWall = states[i, p.Y % 2];
                    if (!sharedWall.HasFlag(NodeState.None)) {
                        list.Add(new Neighbour {
                            Position = new Position { X = x, Y = y },
                            SharedWall = sharedWall
                        });
                    }
                    
                }
            }
        }

        return list;
    }
    
    
    public static IEnumerable<NodeState[,]> Generate(int width, int height) {
        NodeState[,] maze = new NodeState[width, height];
        NodeState start = NodeState.DownLeft | NodeState.Left | NodeState.Right | NodeState.UpLeft | NodeState.DownRight | NodeState.UpRight;

        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                maze[i, j] = start;
            }
        }

        foreach (NodeState[,] mazeStep in RecursiveBacktracker(maze, width, height)) {
            yield return mazeStep;
        }
    }

    
}