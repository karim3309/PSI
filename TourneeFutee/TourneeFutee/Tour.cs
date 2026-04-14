using System;
using System.Collections.Generic;

namespace TourneeFutee
{
    public class Tour
    {
        private List<(string source, string destination, float cost)> _segments = new();

        // AJOUT : Constructeur pour les tests
        public Tour(List<string> vertices, float cost)
        {
            // Construit les segments à partir de la liste de sommets
            for (int i = 0; i < vertices.Count - 1; i++)
            {
                _segments.Add((vertices[i], vertices[i + 1], 0f));
            }

            // Le coût total est stocké séparément ou réparti sur les segments
            // Pour simplifier, on met tout le coût sur le premier segment
            if (_segments.Count > 0)
            {
                var first = _segments[0];
                _segments[0] = (first.source, first.destination, cost);
            }
        }

        // AJOUT : Constructeur par défaut
        public Tour()
        {
        }

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

        // AJOUT : Propriété Vertices pour les tests
        public IList<string> Vertices
        {
            get
            {
                var vertices = new List<string>();
                if (_segments.Count == 0)
                    return vertices;

                vertices.Add(_segments[0].source);
                foreach (var seg in _segments)
                {
                    vertices.Add(seg.destination);
                }
                return vertices;
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