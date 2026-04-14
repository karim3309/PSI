namespace TourneeFutee
{
    public class Graph
    {
        private Dictionary<string, int> vertexIndex;
        private Dictionary<string, float> vertexValues;
        private Matrix matriceAdj;
        private bool _directed;
        private float noEdgeValue;
        public List<string> Nodes => new List<string>(vertexIndex.Keys);

        public Graph(bool directed, float noEdgeValue = 0)
        {
            this._directed = directed;
            this.noEdgeValue = noEdgeValue;
            vertexIndex = new Dictionary<string, int>();
            vertexValues = new Dictionary<string, float>();
            matriceAdj = new Matrix(0, 0, noEdgeValue);
        }

        public int Order => vertexIndex.Count;
        public int VertexCount => vertexIndex.Count;

        public bool directed
        {
            get { return _directed; }
        }

        public bool Directed => _directed;

        public bool ContainsVertex(string name)
        {
            return vertexIndex.ContainsKey(name);
        }

        // Méthode originale qui lève une exception si le sommet n'existe pas
        // (comportement attendu par GraphTests)
        public float GetVertexValue(string name)
        {
            if (!vertexValues.ContainsKey(name))
                throw new ArgumentException($"Le sommet '{name}' n'existe pas.", nameof(name));
            return vertexValues[name];
        }

        public void AddVertex(string name, float value = 0)
        {
            if (vertexIndex.ContainsKey(name))
                throw new ArgumentException($"Le sommet '{name}' existe déjà.", nameof(name));

            int newIndex = vertexIndex.Count;
            vertexIndex.Add(name, newIndex);
            vertexValues.Add(name, value);

            matriceAdj.AddRow(newIndex);
            matriceAdj.AddColumn(newIndex);
        }

        public void RemoveVertex(string name)
        {
            if (!vertexIndex.ContainsKey(name))
                throw new ArgumentException($"Le sommet '{name}' n'existe pas.", nameof(name));

            int indexToRemove = vertexIndex[name];
            vertexIndex.Remove(name);
            vertexValues.Remove(name);
            matriceAdj.RemoveRow(indexToRemove);
            matriceAdj.RemoveColumn(indexToRemove);

            List<string> keys = new List<string>(vertexIndex.Keys);
            foreach (var k in keys)
            {
                if (vertexIndex[k] > indexToRemove)
                {
                    vertexIndex[k]--;
                }
            }
        }

        public void SetVertexValue(string name, float value)
        {
            if (!vertexValues.ContainsKey(name))
                throw new ArgumentException($"Le sommet '{name}' n'existe pas.", nameof(name));
            vertexValues[name] = value;
        }

        public List<string> GetNeighbors(string vertexName)
        {
            if (!vertexIndex.ContainsKey(vertexName))
                throw new ArgumentException($"Le sommet '{vertexName}' n'existe pas.", nameof(vertexName));

            List<string> neighborNames = new List<string>();
            int i = vertexIndex[vertexName];
            foreach (var kvp in vertexIndex)
            {
                int j = kvp.Value;
                if (matriceAdj.GetValue(i, j) != noEdgeValue)
                {
                    neighborNames.Add(kvp.Key);
                }
            }
            return neighborNames;
        }

        public void AddEdge(string sourceName, string destinationName, float weight = 1)
        {
            if (!vertexIndex.ContainsKey(sourceName))
                throw new ArgumentException($"Le sommet source '{sourceName}' n'existe pas.", nameof(sourceName));
            if (!vertexIndex.ContainsKey(destinationName))
                throw new ArgumentException($"Le sommet destination '{destinationName}' n'existe pas.", nameof(destinationName));

            int source = vertexIndex[sourceName];
            int dest = vertexIndex[destinationName];

            if (matriceAdj.GetValue(source, dest) != noEdgeValue)
                throw new ArgumentException($"L'arc de '{sourceName}' vers '{destinationName}' existe déjà.");

            if (!_directed && source != dest && matriceAdj.GetValue(dest, source) != noEdgeValue)
                throw new ArgumentException($"L'arc entre '{sourceName}' et '{destinationName}' existe déjà.");

            matriceAdj.SetValue(source, dest, weight);
            if (!_directed)
                matriceAdj.SetValue(dest, source, weight);
        }

        public void RemoveEdge(string sourceName, string destinationName)
        {
            if (!vertexIndex.ContainsKey(sourceName))
                throw new ArgumentException($"Le sommet source '{sourceName}' n'existe pas.", nameof(sourceName));
            if (!vertexIndex.ContainsKey(destinationName))
                throw new ArgumentException($"Le sommet destination '{destinationName}' n'existe pas.", nameof(destinationName));

            int source = vertexIndex[sourceName];
            int dest = vertexIndex[destinationName];

            if (matriceAdj.GetValue(source, dest) == noEdgeValue)
                throw new ArgumentException($"L'arc de '{sourceName}' vers '{destinationName}' n'existe pas.");

            matriceAdj.SetValue(source, dest, noEdgeValue);
            if (!_directed)
                matriceAdj.SetValue(dest, source, noEdgeValue);
        }

        public float GetEdgeWeight(string sourceName, string destinationName)
        {
            if (!vertexIndex.ContainsKey(sourceName))
                throw new ArgumentException($"Le sommet source '{sourceName}' n'existe pas.", nameof(sourceName));
            if (!vertexIndex.ContainsKey(destinationName))
                throw new ArgumentException($"Le sommet destination '{destinationName}' n'existe pas.", nameof(destinationName));

            int source = vertexIndex[sourceName];
            int dest = vertexIndex[destinationName];

            float weight = matriceAdj.GetValue(source, dest);
            if (weight == noEdgeValue)
                throw new ArgumentException($"Aucun arc n'existe entre '{sourceName}' et '{destinationName}'.");

            return weight;
        }

        public void SetEdgeWeight(string sourceName, string destinationName, float weight)
        {
            if (!vertexIndex.ContainsKey(sourceName))
                throw new ArgumentException("Sommet source introuvable.", nameof(sourceName));
            if (!vertexIndex.ContainsKey(destinationName))
                throw new ArgumentException("Sommet destination introuvable.", nameof(destinationName));

            int source = vertexIndex[sourceName];
            int dest = vertexIndex[destinationName];

            if (matriceAdj.GetValue(source, dest) == noEdgeValue)
                throw new ArgumentException($"L'arc de '{sourceName}' vers '{destinationName}' n'existe pas.");

            matriceAdj.SetValue(source, dest, weight);
            if (!_directed)
            {
                matriceAdj.SetValue(dest, source, weight);
            }
        }
    }
}