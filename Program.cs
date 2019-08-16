using System;
using System.Linq;

namespace MapGen
{
    public class Program
    {
        public static readonly Random Random = new Random();

        public const int MAP_WIDTH = 10;
        public const int MAP_HEIGHT = 5;

        static void Main(string[] args)
        {
            while (true) 
            {
                Tile[,] map;
                do
                {
                    map = InitializeMap();
                    WaveFunctionCollapse(map);
                }
                while (!ClearLoopsAndCheck(map));

                DrawMap(map);

                Console.WriteLine("Press enter to regenerate.");
                Console.ReadLine();
            }
        }

        static Tile[,] InitializeMap()
        {
            var map = new Tile[MAP_WIDTH, MAP_HEIGHT];
            for (var x = 0; x < MAP_WIDTH; x++)
            {
                for (var y = 0; y < MAP_HEIGHT; y++)
                {
                    var tile = new Tile(x, y);
                    map[x, y] = tile;

                    if (x == 0) tile.West = false;
                    if (y == 0) tile.North = false;
                    if (x == MAP_WIDTH - 1) tile.East = false;
                    if (y == MAP_HEIGHT - 1) tile.South = false;
                }
            }

            SetStartAndHome(map);

            return map;
        }

        static void SetStartAndHome(Tile[,] map) 
        {
            int startX = 0;
            int startY = Random.Next(MAP_HEIGHT);
            Tile start = map[startX, startY];
            start.Type = TileType.Start;
            start.West = true;

            int homeX = MAP_WIDTH - 1;
            int homeY = Random.Next(MAP_HEIGHT);
            Tile home = map[homeX, homeY];
            home.Type = TileType.Home;
            home.North = true;
            home.South = true;
            home.East = true;
            home.West = true;
            Propogate(map, homeX, homeY);
        }

        static void WaveFunctionCollapse(Tile[,] map)
        {
            while (true)
            {
                (int lowestEntropy, int lowestEntropyX, int lowestEntropyY) = FindLowestEntropy(map);

                if (lowestEntropy == 5) break;

                var lowestEntropyTile = map[lowestEntropyX, lowestEntropyY];
                lowestEntropyTile.Collapse();

                // Propogate collapsed information
                Propogate(map, lowestEntropyX, lowestEntropyY);
            }
        }

        static void Propogate(Tile[,] map, int fromX, int fromY)
        {
            var tile = map[fromX, fromY];

            foreach (Direction dir in DirectionUtils.Directions())
            {
                bool? valueInDir = tile.Get(dir);
                if (valueInDir == null) continue;

                (Tile neighbor, int neighborX, int neighborY) = map.ValueInDirection(fromX, fromY, dir);

                if (neighbor != null)
                {
                    if (neighbor.TryResolve(dir.Opposite(), valueInDir.Value))
                    {
                        Propogate(map, neighborX, neighborY);
                    }
                }
            }
        }

        static bool ClearLoopsAndCheck(Tile[,] map)
        {
            int startX = 0;
            int startY;
            for (startY = 0; startY < MAP_HEIGHT; startY++)
            {
                Tile tile = map[0, startY];
                if (tile.Type == TileType.Start)
                {
                    break;
                }
            }

            bool homeOnPath = false;

            void Search(int fromX, int fromY)
            {
                Tile tile = map.MaybeGet(fromX, fromY);
                if (tile == null || tile.OnPath) return;

                tile.OnPath = true;
                homeOnPath = homeOnPath || tile.Type == TileType.Home;

                foreach (Direction dir in DirectionUtils.Directions())
                {
                    if (tile.Get(dir) == true)
                    {
                        Search(fromX + dir.X(), fromY + dir.Y());
                    }
                }
            }

            Search(startX, startY);

            if (!homeOnPath) return false;

            for (int x = 0; x < MAP_WIDTH; x++)
            {
                for (int y = 0; y < MAP_HEIGHT; y++)
                {
                    var tile = map[x, y];
                    if (!tile.OnPath) tile.Clear();
                }
            }

            return true;
        }

        static (int lowestEntropy, int lowestEntropyX, int lowestEntropyY) FindLowestEntropy(Tile[,] map)
        {
            int lowestEntropy = 5;
            int lowestEntropyX = -1;
            int lowestEntropyY = -1;
            for (var x = 0; x < MAP_WIDTH; x++)
            {
                for (var y = 0; y < MAP_HEIGHT; y++)
                {
                    var tile = map[x, y];
                    var entropy = tile.Entropy();

                    if (entropy < lowestEntropy && entropy > 0)
                    {
                        lowestEntropy = entropy;
                        lowestEntropyX = x;
                        lowestEntropyY = y;
                    }
                }
            }

            return (lowestEntropy, lowestEntropyX, lowestEntropyY);
        }

        static void DrawMap(Tile[,] map)
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);

            for (int y = 0; y < height; y++)
            {
                for (int row = 0; row < 3; row++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        Tile tile = map[x, y];
                        string[] tileRows = tile.Render();
                        Console.Write(tileRows[row]);
                        if (x < width - 1)
                        {
                            Console.Write('│');
                        }
                    }
                    Console.WriteLine();
                }
                if (y < height - 1)
                {
                    Console.WriteLine(String.Join("┼", Enumerable.Repeat("─────", width)));
                }
            }
        }
    }

}
