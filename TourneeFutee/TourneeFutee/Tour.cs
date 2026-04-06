using System;
using System.Collections.Generic;

namespace TourneeFutee
{
    public class Tour
    {
        private List<(string source, string destination, float cost)> _segments = new();

        public float Cost
        {
            get
            {
                float total = 0f;
                foreach (var seg in _segments)
                    total += seg.cost;
                return total;
            }
        }

        public int NbSegments
        {
            get { return _segments.Count; }
        }

        public bool ContainsSegment((string source, string destination) segment)
        {
            foreach (var seg in _segments)
                if (seg.source == segment.source && seg.destination == segment.destination)
                    return true;
            return false;
        }

        public void AddSegment(string source, string destination, float cost)
        {
            _segments.Add((source, destination, cost));
        }

        public void Print()
        {
            Console.WriteLine("Coût total : " + Cost);
            foreach (var seg in _segments)
            {
                Console.WriteLine("  " + seg.source + " -> " + seg.destination + " (coût : " + seg.cost + ")");
            }
        }
    }
}