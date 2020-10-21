using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Greedy.Architecture;
using Greedy.Architecture.Drawing;

namespace Greedy
{
	public class GreedyPathFinder : IPathFinder
	{
		public List<Point> FindPathToCompleteGoal(State state)
		{
			var notVisitedChests = state.Chests.ToList();
			var start = state.Position;
			var energy = state.Energy;
			var resultPath = new List<Point>();
			var pathFinder = new DijkstraPathFinder();

			while(state.Chests.Count - notVisitedChests.Count != state.Goal)
			{
				var path = pathFinder.GetPathsByDijkstra(state, start, notVisitedChests);
				var shortestPath = path.FirstOrDefault();
				if (shortestPath == null) return new List<Point>();

				energy -= shortestPath.Cost;
				if (energy < 0) return new List<Point>();

				start = shortestPath.End;
				resultPath.AddRange(shortestPath.Path.Skip(1));
				notVisitedChests.Remove(shortestPath.End);
			}
			return resultPath;
		}
	}
}