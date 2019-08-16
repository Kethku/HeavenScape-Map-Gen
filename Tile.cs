using System;
using System.Linq;
using System.Text;

namespace MapGen
{
    public enum TileType
    {
        Start,
        Normal,
        Home
    }

    public class Tile
    {
        public bool? North { get; set; }
        public bool? East { get; set; }
        public bool? South { get; set; }
        public bool? West { get; set; }

        public int X { get; }
        public int Y { get; }

        public TileType Type { get; set; }
        public bool OnPath { get; set; }

        public Tile(int x, int y)
        {
            X = x;
            Y = y;
            Type = TileType.Normal;
        }

        public bool? Get(Direction dir)
        {
            switch (dir)
            {
                case Direction.North: return North;
                case Direction.South: return South;
                case Direction.East: return East;
                case Direction.West: return West;
                default: throw new ArgumentException("No such direction");
            }
        }

        private void UnconstrainedSet(Direction dir, bool value)
        {
            switch (dir)
            {
                case Direction.North:
                    North = value;
                    break;
                case Direction.South:
                    South = value;
                    break;
                case Direction.East:
                    East = value;
                    break;
                case Direction.West:
                    West = value;
                    break;
                default: throw new ArgumentException("No such direction");
            }
        }

        public void Set(Direction direction, bool value)
        {
            UnconstrainedSet(direction, value);
            if (Empty() == 2 && InCenter())
            {
                foreach (Direction dir in UnresolvedDirections())
                {
                    UnconstrainedSet(dir, true);
                }
            }
        }

        public bool TryResolve(Direction dir, bool value)
        {
            if (Get(dir) == null)
            {
                Set(dir, value);
                return true;
            }
            return false;
        }

        public void Collapse()
        {
            Direction[] unresolved = UnresolvedDirections();
            while (unresolved.Length > 0)
            {
                Set(unresolved.Random(), Program.Random.NextDouble() < 0.5);
                unresolved = UnresolvedDirections();
            }
        }

        public void Clear()
        {
            foreach (Direction dir in DirectionUtils.Directions())
            {
                Set(dir, false);
            }
        }

        public bool InCenter() =>
            X != 0 && X != Program.MAP_WIDTH - 1 &&
            Y != 0 && Y != Program.MAP_HEIGHT - 1;

        public int Count(Func<bool?, bool> predicate) => DirectionUtils
            .Directions()
            .Select(dir => predicate(Get(dir)))
            .Count(hasValue => hasValue);

        public int Entropy() => Count(value => value == null);
        public int Empty() => Count(value => value.HasValue && value != true);

        private Direction[] UnresolvedDirections() =>
            DirectionUtils.Directions()
                .Where(dir => Get(dir) == null)
                .ToArray();

        public string[] Render()
        {
            string[] render = RenderBase();
            if (Type == TileType.Normal) return render;

            char center = Type == TileType.Home ? 'H' : 'S';
            var sb = new StringBuilder(render[1]);
            sb[2] = center;
            render[1] = sb.ToString();

            return render;
        }

        private string[] RenderBase()
        {
            if (North == null || East == null || South == null || West == null)
            {
                char RenderMaybe(bool? option, char success)
                {
                    char result = '?';
                    if (option != null)
                    {
                        result = option.Value ? success : ' ';
                    }
                    return result;
                }

                char up = RenderMaybe(North, '║');
                char right = RenderMaybe(East, '═');
                char down = RenderMaybe(South, '║');
                char left = RenderMaybe(West, '═');

                return new[] {
                    $"??{up}??",
                    $"{left}{left}?{right}{right}",
                    $"??{down}??"
                };
            }

            if (North.Value)
            {
                if (East.Value)
                {
                    if (South.Value)
                    {
                        if (West.Value)
                        {
                            return new[] {
                                "  ║  ",
                                "══╬══",
                                "  ║  "
                            };
                        }
                        else // !StreetLeft
                        {
                            return new[] {
                                "  ║  ",
                                "  ╠══",
                                "  ║  "
                            };
                        }
                    }
                    else // !StreetDown
                    {
                        if (West.Value)
                        {
                            return new[] {
                                "  ║  ",
                                "══╩══",
                                "     "
                            };
                        }
                        else // !StreetLeft
                        {
                            return new[] {
                                "  ║  ",
                                "  ╚══",
                                "     "
                            };
                        }
                    }
                }
                else // !StreetRight
                {
                    if (South.Value)
                    {
                        if (West.Value)
                        {
                            return new[] {
                                "  ║  ",
                                "══╣  ",
                                "  ║  "
                            };
                        }
                        else // !StreetLeft
                        {
                            return new[] {
                                "  ║  ",
                                "  ║  ",
                                "  ║  "
                            };
                        }
                    }
                    else // !StreetDown
                    {
                        if (West.Value)
                        {
                            return new[] {
                                "  ║  ",
                                "══╝  ",
                                "     "
                            };
                        }
                        else // !StreetLeft
                        {
                            return new[] {
                                "  ║  ",
                                "     ",
                                "     "
                            };
                        }
                    }
                }
            }
            else // !StreetUp
            {
                if (East.Value)
                {
                    if (South.Value)
                    {
                        if (West.Value)
                        {
                            return new[] {
                                "     ",
                                "══╦══",
                                "  ║  "
                            };
                        }
                        else // !StreetLeft
                        {
                            return new[] {
                                "     ",
                                "  ╔══",
                                "  ║  "
                            };
                        }
                    }
                    else // !StreetDown
                    {
                        if (West.Value)
                        {
                            return new[] {
                                "     ",
                                "═════",
                                "     "
                            };
                        }
                        else // !StreetLeft
                        {
                            return new[] {
                                "     ",
                                "   ══",
                                "     "
                            };
                        }
                    }
                }
                else // !StreetRight
                {
                    if (South.Value)
                    {
                        if (West.Value)
                        {
                            return new[] {
                                "     ",
                                "══╗  ",
                                "  ║  "
                            };
                        }
                        else // !StreetLeft
                        {
                            return new[] {
                                "     ",
                                "     ",
                                "  ║  "
                            };
                        }
                    }
                    else // !StreetDown
                    {
                        if (West.Value)
                        {
                            return new[] {
                                "     ",
                                "══   ",
                                "     "
                            };
                        }
                        else // !StreetLeft
                        {
                            return new[] {
                                "     ",
                                "     ",
                                "     "
                            };
                        }
                    }
                }
            }
        }
    }
}