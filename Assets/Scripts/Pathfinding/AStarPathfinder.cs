using System;
using System.Collections.Generic;
using Roller.Core;
using Roller.Grid;

namespace Roller.Pathfinding
{
    public static class AStarPathfinder
    {
        const int CardinalCost = 10;
        const int DiagonalCost = 14;

        /// <summary>
        /// 그리드에서 start → goal 최단 경로 탐색. 경로 없으면 null.
        /// 결과 경로는 start 포함, goal 포함.
        /// </summary>
        public static List<Coord> FindPath(GridMap map, Coord start, Coord goal)
        {
            if (map == null) return null;
            if (!map.InBounds(start) || !map.InBounds(goal)) return null;
            if (start == goal) return new List<Coord> { start };

            var goalCell = map.Get(goal);
            if (goalCell == null || !goalCell.IsPassable) return null;

            var open = new SimplePriorityQueue<Coord>();
            var cameFrom = new Dictionary<Coord, Coord>();
            var gScore = new Dictionary<Coord, int>();

            gScore[start] = 0;
            open.Enqueue(start, Heuristic(start, goal));

            int safetyCounter = 0;
            int maxIters = map.Config.Width * map.Config.Height * 8;

            while (open.Count > 0)
            {
                if (++safetyCounter > maxIters)
                {
                    UnityEngine.Debug.LogWarning("[AStar] Safety counter triggered. Path search aborted.");
                    return null;
                }

                Coord current = open.Dequeue();
                if (current == goal) return Reconstruct(cameFrom, current);

                foreach (Coord neighbor in Directions.Neighbors(current))
                {
                    if (!map.InBounds(neighbor)) continue;
                    var cell = map.Get(neighbor);
                    if (cell == null) continue;
                    // 도착 칸이 점유되었어도 경로 자체는 허용 (start와 같은 칸이 아니면)
                    if (!cell.Terrain.IsPassable()) continue;
                    if (cell.Occupant != null && neighbor != goal) continue;

                    bool isDiagonal = (neighbor.X != current.X) && (neighbor.Y != current.Y);
                    int baseStep = isDiagonal ? DiagonalCost : CardinalCost;
                    int terrainMult = cell.MoveCost / 10; // 1 또는 1.5 효과
                    int stepCost = baseStep * Math.Max(1, terrainMult);

                    int tentativeG = gScore[current] + stepCost;
                    if (!gScore.TryGetValue(neighbor, out int existingG) || tentativeG < existingG)
                    {
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentativeG;
                        int f = tentativeG + Heuristic(neighbor, goal);
                        open.Enqueue(neighbor, f);
                    }
                }
            }

            return null;
        }

        static int Heuristic(Coord a, Coord b)
        {
            int dx = Math.Abs(a.X - b.X);
            int dy = Math.Abs(a.Y - b.Y);
            // 옥타일 거리 (대각선 비용 반영)
            return CardinalCost * (dx + dy) + (DiagonalCost - 2 * CardinalCost) * Math.Min(dx, dy);
        }

        static List<Coord> Reconstruct(Dictionary<Coord, Coord> cameFrom, Coord current)
        {
            var path = new List<Coord> { current };
            while (cameFrom.TryGetValue(current, out var prev))
            {
                current = prev;
                path.Add(current);
            }
            path.Reverse();
            return path;
        }
    }
}
