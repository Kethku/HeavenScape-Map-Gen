using System;

namespace MapGen
{
    public enum Direction
    {
        North, South, East, West
    }

    public static class DirectionUtils
    {
        public static Direction[] Directions()
        {
            return new[] {
                Direction.North, Direction.South, Direction.East, Direction.West
            };
        }

        public static int X(this Direction dir)
        {
            switch (dir)
            {
                case Direction.East: return 1;
                case Direction.West: return -1;
                default: return 0;
            }
        }

        public static int Y(this Direction dir)
        {
            switch (dir)
            {
                case Direction.North: return -1;
                case Direction.South: return 1;
                default: return 0;
            }
        }

        public static Direction Opposite(this Direction dir)
        {
            switch (dir)
            {
                case Direction.North: return Direction.South;
                case Direction.South: return Direction.North;
                case Direction.East: return Direction.West;
                case Direction.West: return Direction.East;
                default: throw new ArgumentException("No such direction.");
            }
        }
    }
}