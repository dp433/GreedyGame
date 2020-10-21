using System;
using System.Collections.Generic;
using System.Linq;
using Greedy.Architecture;
using System.Drawing;

namespace Greedy
{
	class DijkstraData
	{
		public Point Previous { get; set; }
		public int Price { get; set; }
	}

	public class DijkstraPathFinder
	{
		public IEnumerable<PathWithCost> GetPathsByDijkstra(State state, Point start,
			IEnumerable<Point> targets)
		{
			var notVisitedTargets = targets.ToList();
			var notVisitedPoints = new List<Point>();
			var track = new Dictionary<Point, DijkstraData>();
			track[start] = new DijkstraData { Previous = new Point(-1, -1), Price = 0 };
			notVisitedPoints.Add(start);

			while (notVisitedTargets.Any())
			{
				var path = GetPathByDijkstra(state, notVisitedPoints, track, notVisitedTargets);
				if (path == null) yield break;
				yield return path;
				notVisitedTargets.Remove(path.End);
			}
		}

		private PathWithCost GetPathByDijkstra(State state, List<Point> notVisitedPoints,
			Dictionary<Point, DijkstraData> track, IEnumerable<Point> targets)
		{
			Point end;
			while (true)
			{
				var toOpen = GetPointToOpen(notVisitedPoints, track);
				if (toOpen == new Point(-1, -1)) return null;
				if (targets.Contains(toOpen))
				{
					end = toOpen;
					break;
				}
				OpenPoint(toOpen, state, notVisitedPoints, track);
			}
			return GetPath(end, track);
		}

		private Point GetPointToOpen(List<Point> notVisitedPoints, Dictionary<Point, DijkstraData> track)
		{
			var toOpen = new Point(-1, -1);
			int bestPrice = int.MaxValue;

			foreach (var point in notVisitedPoints)
			{
				if (track.ContainsKey(point) && track[point].Price < bestPrice)
				{
					bestPrice = track[point].Price;
					toOpen = point;
				}
			}

			return toOpen;
		}

		private PathWithCost GetPath(Point end, Dictionary<Point, DijkstraData> track)
		{
			var result = new PathWithCost(track[end].Price, end);
			end = track[end].Previous;
			while (end != new Point(-1, -1))
			{
				result.Path.Add(end);
				end = track[end].Previous;
			}
			result.Path.Reverse();
			return result;
		}

		private void OpenPoint(Point toOpen, State state, List<Point> notVisitedPoints, Dictionary<Point, DijkstraData> track)
		{
			for (var dy = -1; dy <= 1; dy++)
				for (var dx = -1; dx <= 1; dx++)
					if (dx == 0 || dy == 0)
					{
						var nextPoint = new Point(toOpen.X + dx, toOpen.Y + dy);
						if (track.ContainsKey(nextPoint) || !state.InsideMap(nextPoint) || state.IsWallAt(nextPoint)) continue;
						notVisitedPoints.Add(nextPoint);
						var currentPrice = track[toOpen].Price + state.CellCost[nextPoint.X, nextPoint.Y];
						if (!track.ContainsKey(nextPoint) || track[nextPoint].Price > currentPrice)
							track[nextPoint] = new DijkstraData { Previous = toOpen, Price = currentPrice };
					}
			notVisitedPoints.Remove(toOpen);
		}
	}
}
