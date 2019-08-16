namespace MapGen
{
    public static class Extensions
    {
        public static T Random<T>(this T[] array) =>
            array[Program.Random.Next(array.Length)];

        public static (T value, int x, int y) ValueInDirection<T>(this T[,] map, int currentX, int currentY, Direction dir) where T : class
        {
            int newX = currentX + dir.X();
            int newY = currentY + dir.Y();
            return (map.MaybeGet(newX, newY), newX, newY);
        }

        public static T MaybeGet<T>(this T[,] map, int x, int y) where T : class
        {
            int mapWidth = map.GetLength(0);
            int mapHeight = map.GetLength(1);

            if (x >= 0 && x < mapWidth && y >= 0 && y < mapHeight)
            {
                return map[x, y];
            }
            return null;
        }
    }
}