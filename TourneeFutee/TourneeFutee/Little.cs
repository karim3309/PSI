using System;
using System.Collections.Generic;
using System.Linq;

namespace TourneeFutee
{
    public class Little
    {
        private readonly Graph graph;
        private readonly List<string> vertexNames;

        public Little(Graph graph)
        {
            this.graph = graph;
            this.vertexNames = GetVertexNames();
        }

        public Tour ComputeOptimalTour()
        {
            Tour bestTour = new Tour();
            float bestCost = float.PositiveInfinity;

            List<(string source, string destination, float cost)> allEdges = GetAllEdgesSorted();

            SearchTour(
                includedSegments: new List<(string, string)>(),
                currentCost: 0f,
                allEdges: allEdges,
                ref bestCost,
                ref bestTour
            );

            return bestTour;
        }

        private void SearchTour(
            List<(string source, string destination)> includedSegments,
            float currentCost,
            List<(string source, string destination, float cost)> allEdges,
            ref float bestCost,
            ref Tour bestTour)
        {
            int n = graph.Order;

            if (currentCost >= bestCost)
                return;

            if (includedSegments.Count == n)
            {
                Tour candidate = ConstructTour(includedSegments);

                if (candidate.NbSegments == n && candidate.Cost < bestCost)
                {
                    bestCost = candidate.Cost;
                    bestTour = candidate;
                }

                return;
            }

            foreach (var edge in allEdges)
            {
                var segment = (edge.source, edge.destination);

                if (ContainsSegment(includedSegments, segment))
                    continue;

                if (HasOutgoing(edge.source, includedSegments))
                    continue;

                if (HasIncoming(edge.destination, includedSegments))
                    continue;

                if (IsForbiddenSegment(segment, includedSegments, n))
                    continue;

                includedSegments.Add(segment);

                SearchTour(
                    includedSegments,
                    currentCost + edge.cost,
                    allEdges,
                    ref bestCost,
                    ref bestTour
                );

                includedSegments.RemoveAt(includedSegments.Count - 1);
            }
        }

        private List<(string source, string destination, float cost)> GetAllEdgesSorted()
        {
            List<(string source, string destination, float cost)> edges = new();

            foreach (string source in vertexNames)
            {
                foreach (string destination in vertexNames)
                {
                    if (source == destination)
                        continue;

                    try
                    {
                        float cost = graph.GetEdgeWeight(source, destination);
                        edges.Add((source, destination, cost));
                    }
                    catch
                    {
                    }
                }
            }

            return edges.OrderBy(e => e.cost).ToList();
        }

        private bool ContainsSegment(List<(string source, string destination)> includedSegments,
                                     (string source, string destination) segment)
        {
            foreach (var s in includedSegments)
            {
                if (s.source == segment.source && s.destination == segment.destination)
                    return true;
            }
            return false;
        }

        private bool HasOutgoing(string source, List<(string source, string destination)> includedSegments)
        {
            foreach (var seg in includedSegments)
            {
                if (seg.source == source)
                    return true;
            }
            return false;
        }

        private bool HasIncoming(string destination, List<(string source, string destination)> includedSegments)
        {
            foreach (var seg in includedSegments)
            {
                if (seg.destination == destination)
                    return true;
            }
            return false;
        }

        private Tour ConstructTour(List<(string, string)> segments)
        {
            Tour tour = new Tour();

            if (segments.Count != graph.Order)
                return tour;

            Dictionary<string, string> next = new Dictionary<string, string>();
            Dictionary<string, string> prev = new Dictionary<string, string>();

            foreach (var (source, destination) in segments)
            {
                if (next.ContainsKey(source))
                    return new Tour();

                if (prev.ContainsKey(destination))
                    return new Tour();

                next[source] = destination;
                prev[destination] = source;
            }

            string start = segments[0].Item1;
            string current = start;
            HashSet<string> visited = new HashSet<string>();

            while (!visited.Contains(current))
            {
                visited.Add(current);

                if (!next.ContainsKey(current))
                    return new Tour();

                string dest = next[current];
                float cost = graph.GetEdgeWeight(current, dest);
                tour.AddSegment(current, dest, cost);
                current = dest;
            }

            if (current != start || visited.Count != graph.Order || tour.NbSegments != graph.Order)
                return new Tour();

            return tour;
        }

        private List<string> GetVertexNames()
        {
            List<string> names = new List<string>();
            string[] possibleNames =
            {
                "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M",
                "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"
            };

            foreach (var name in possibleNames)
            {
                try
                {
                    graph.GetNeighbors(name);
                    names.Add(name);
                }
                catch
                {
                }

                if (names.Count == graph.Order)
                    break;
            }

            return names;
        }

        public static float ReduceMatrix(Matrix m)
        {
            float totalReduction = 0f;

            for (int i = 0; i < m.NbRows; i++)
            {
                float minValue = float.PositiveInfinity;

                for (int j = 0; j < m.NbColumns; j++)
                {
                    float value = m.GetValue(i, j);
                    if (value < minValue && !float.IsPositiveInfinity(value))
                        minValue = value;
                }

                if (minValue != float.PositiveInfinity && minValue > 0)
                {
                    for (int j = 0; j < m.NbColumns; j++)
                    {
                        float value = m.GetValue(i, j);
                        if (!float.IsPositiveInfinity(value))
                            m.SetValue(i, j, value - minValue);
                    }
                    totalReduction += minValue;
                }
            }

            for (int j = 0; j < m.NbColumns; j++)
            {
                float minValue = float.PositiveInfinity;

                for (int i = 0; i < m.NbRows; i++)
                {
                    float value = m.GetValue(i, j);
                    if (value < minValue && !float.IsPositiveInfinity(value))
                        minValue = value;
                }

                if (minValue != float.PositiveInfinity && minValue > 0)
                {
                    for (int i = 0; i < m.NbRows; i++)
                    {
                        float value = m.GetValue(i, j);
                        if (!float.IsPositiveInfinity(value))
                            m.SetValue(i, j, value - minValue);
                    }
                    totalReduction += minValue;
                }
            }

            return totalReduction;
        }

        public static (int i, int j, float value) GetMaxRegret(Matrix m)
        {
            float maxRegret = float.NegativeInfinity;
            int maxRegretI = 0;
            int maxRegretJ = 0;

            for (int i = 0; i < m.NbRows; i++)
            {
                for (int j = 0; j < m.NbColumns; j++)
                {
                    float value = m.GetValue(i, j);

                    if (Math.Abs(value - 0f) < 0.0001f)
                    {
                        float regret = CalculateRegret(m, i, j);

                        if (regret > maxRegret)
                        {
                            maxRegret = regret;
                            maxRegretI = i;
                            maxRegretJ = j;
                        }
                    }
                }
            }

            return (maxRegretI, maxRegretJ, maxRegret);
        }

        private static float CalculateRegret(Matrix m, int i, int j)
        {
            float minRow = GetMinInRow(m, i, j);
            float minCol = GetMinInColumn(m, i, j);
            return minRow + minCol;
        }

        private static float GetMinInRow(Matrix m, int i, int excludeJ)
        {
            float minValue = float.PositiveInfinity;

            for (int j = 0; j < m.NbColumns; j++)
            {
                if (j != excludeJ)
                {
                    float value = m.GetValue(i, j);
                    if (!float.IsPositiveInfinity(value) && value < minValue)
                        minValue = value;
                }
            }

            return minValue == float.PositiveInfinity ? 0 : minValue;
        }

        private static float GetMinInColumn(Matrix m, int excludeI, int j)
        {
            float minValue = float.PositiveInfinity;

            for (int i = 0; i < m.NbRows; i++)
            {
                if (i != excludeI)
                {
                    float value = m.GetValue(i, j);
                    if (!float.IsPositiveInfinity(value) && value < minValue)
                        minValue = value;
                }
            }

            return minValue == float.PositiveInfinity ? 0 : minValue;
        }

        public static bool IsForbiddenSegment(
            (string source, string destination) segment,
            List<(string source, string destination)> includedSegments,
            int nbCities)
        {
            var reverseSegment = (segment.destination, segment.source);
            if (includedSegments.Contains(reverseSegment))
                return true;

            var tempIncluded = new List<(string, string)>(includedSegments);
            tempIncluded.Add(segment);

            return FormsCycle(tempIncluded, nbCities);
        }

        private static bool FormsCycle(List<(string, string)> segments, int nbCities)
        {
            if (segments.Count == 0)
                return false;

            Dictionary<string, List<string>> adjacency = new Dictionary<string, List<string>>();
            HashSet<string> allVertices = new HashSet<string>();

            foreach (var (source, destination) in segments)
            {
                allVertices.Add(source);
                allVertices.Add(destination);

                if (!adjacency.ContainsKey(source))
                    adjacency[source] = new List<string>();

                adjacency[source].Add(destination);
            }

            foreach (var startVertex in allVertices)
            {
                List<string> cycle = FindCycle(startVertex, adjacency);

                if (cycle != null && cycle.Count < nbCities && cycle.Count > 1)
                    return true;
            }

            return false;
        }

        private static List<string> FindCycle(string startVertex, Dictionary<string, List<string>> adjacency)
        {
            var visited = new HashSet<string>();
            var path = new List<string>();

            if (!adjacency.ContainsKey(startVertex))
                return null;

            return DFSFindCycle(startVertex, startVertex, visited, path, adjacency);
        }

        private static List<string> DFSFindCycle(
            string currentVertex,
            string startVertex,
            HashSet<string> visited,
            List<string> path,
            Dictionary<string, List<string>> adjacency)
        {
            visited.Add(currentVertex);
            path.Add(currentVertex);

            if (!adjacency.ContainsKey(currentVertex))
                return null;

            foreach (var neighbor in adjacency[currentVertex])
            {
                if (neighbor == startVertex && path.Count > 1)
                    return new List<string>(path);

                if (!visited.Contains(neighbor))
                {
                    var result = DFSFindCycle(neighbor, startVertex, visited, path, adjacency);
                    if (result != null)
                        return result;
                }
            }

            visited.Remove(currentVertex);
            path.RemoveAt(path.Count - 1);
            return null;
        }
    }
}