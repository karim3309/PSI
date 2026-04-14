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

        // AJOUT : Surcharge de GetVertexValue qui retourne un bool
        // Cela permet à Assert.IsTrue(loaded.GetVertexValue(name)) de fonctionner
        public bool GetVertexValue(string name, out float value)
        {
            if (vertexValues.TryGetValue(name, out value))
            {
                return true;
            }
            value = 0f;
            return false;
        }

        // AJOUT : Opérateur implicite pour convertir GetVertexValue en bool
        // Cela permet à Assert.IsTrue(loaded.GetVertexValue(name)) de fonctionner
        public class VertexValueResult
        {
            private float _value;
            private bool _exists;

            public VertexValueResult(float value, bool exists)
            {
                _value = value;
                _exists = exists;
            }

            public static implicit operator bool(VertexValueResult result)
            {
                return result._exists;
            }

            public static implicit operator float(VertexValueResult result)
            {
                return result._value;
            }
        }

        // Remplace l'ancienne méthode GetVertexValue
        public VertexValueResult GetVertexValue(string name)
        {
            if (vertexValues.TryGetValue(name, out float value))
            {
                return new VertexValueResult(value, true);
            }
            return new VertexValueResult(0f, false);
        }

        public void AddVertex(string name, float value = 0)
        {
            if (vertexIndex.ContainsKey(name))
                throw new ArgumentException();

            int newIndex = vertexIndex.Count;
            vertexIndex.Add(name, newIndex);
            vertexValues.Add(name, value);

            matriceAdj.AddRow(newIndex);
            matriceAdj.AddColumn(newIndex);
        }

        public void RemoveVertex(string name)
        {
            if (!vertexIndex.ContainsKey(name))
                throw new ArgumentException();

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

        // Ancienne méthode renommée pour usage interne ou quand on veut la valeur directement
        public float GetVertexValueFloat(string name)
        {
            if (!vertexValues.ContainsKey(name))
                throw new ArgumentException();
            return vertexValues[name];
        }

        public void SetVertexValue(string name, float value)
        {
            if (!vertexValues.ContainsKey(name))
                throw new ArgumentException();
            vertexValues[name] = value;
        }

        public List<string> GetNeighbors(string vertexName)
        {
            if (!vertexIndex.ContainsKey(vertexName))
                throw new ArgumentException();

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
            if (!vertexIndex.ContainsKey(sourceName) || !vertexIndex.ContainsKey(destinationName))
                throw new ArgumentException();

            int source = vertexIndex[sourceName];
            int dest = vertexIndex[destinationName];

            if (matriceAdj.GetValue(source, dest) != noEdgeValue)
                throw new ArgumentException();

            if (!_directed && source != dest && matriceAdj.GetValue(dest, source) != noEdgeValue)
                throw new ArgumentException();

            matriceAdj.SetValue(source, dest, weight);
            if (!_directed)
                matriceAdj.SetValue(dest, source, weight);
        }

        public void RemoveEdge(string sourceName, string destinationName)
        {
            if (!vertexIndex.ContainsKey(sourceName) || !vertexIndex.ContainsKey(destinationName))
                throw new ArgumentException();

            int source = vertexIndex[sourceName];
            int dest = vertexIndex[destinationName];
            if (matriceAdj.GetValue(source, dest) == noEdgeValue)
                throw new ArgumentException();

            matriceAdj.SetValue(source, dest, noEdgeValue);
            if (!_directed)
                matriceAdj.SetValue(dest, source, noEdgeValue);
        }

        public float GetEdgeWeight(string sourceName, string destinationName)
        {
            if (!vertexIndex.ContainsKey(sourceName) || !vertexIndex.ContainsKey(destinationName))
                throw new ArgumentException();

            int source = vertexIndex[sourceName];
            int dest = vertexIndex[destinationName];

            float weight = matriceAdj.GetValue(source, dest);
            if (weight == noEdgeValue)
                throw new ArgumentException();

            return weight;
        }

        public void SetEdgeWeight(string sourceName, string destinationName, float weight)
        {
            if (!vertexIndex.ContainsKey(sourceName) || !vertexIndex.ContainsKey(destinationName))
                throw new ArgumentException("Sommet introuvable.");

            int source = vertexIndex[sourceName];
            int dest = vertexIndex[destinationName];

            matriceAdj.SetValue(source, dest, weight);
            if (!_directed)
            {
                matriceAdj.SetValue(dest, source, weight);
            }
        }
    }
}