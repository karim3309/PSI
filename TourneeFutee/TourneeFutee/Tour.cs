using System;
using System.Collections.Generic;

namespace TourneeFutee
{
    public class Tour
    {
        private readonly List<string> _vertices;
        private readonly float _totalCost;

        public Tour(List<string> vertices, float totalCost)
        {
            if (vertices == null)
                throw new ArgumentNullException(nameof(vertices));

            _vertices = new List<string>(vertices);
            _totalCost = totalCost;
        }

        public IList<string> Vertices => _vertices.AsReadOnly();

        public float TotalCost => _totalCost;
    }
}