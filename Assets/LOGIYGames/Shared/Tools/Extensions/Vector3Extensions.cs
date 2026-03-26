using UnityEngine;
namespace LOGIYGames
{
    public static class Vector3Extensions
    {
        /// <summary>
        /// Вычисляет квадрат расстояния между двумя точками.
        /// Это более эффективно, чем Distance, т.к. не требует вычисления квадратного корня.
        /// </summary>
        /// <param name="a">Первая точка</param>
        /// <param name="b">Вторая точка</param>
        /// <returns>Квадрат расстояния между точками</returns>
        public static float SquaredDistance(this Vector3 a, Vector3 b)
        {
            float diffX = a.x - b.x;
            float diffY = a.y - b.y;
            float diffZ = a.z - b.z;

            return diffX * diffX + diffY * diffY + diffZ * diffZ;
        }
    }
}