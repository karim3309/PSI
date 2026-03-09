namespace TourneeFutee
{
    public class Graph
    {
        private Dictionary<string, int> vertexIndex;
        private Dictionary<string, float> vertexValues;
        private Matrix matriceAdj;
        private bool directed;
        private float noEdgeValue;

        public Graph(bool directed, float noEdgeValue = 0)
        {
            this.directed = directed;
            this.noEdgeValue = noEdgeValue;
            vertexIndex = new Dictionary<string, int>();
            vertexValues = new Dictionary<string, float>();
            matriceAdj = new Matrix(0, 0, noEdgeValue);
        }
        // L'ordre, c'est juste le nombre de sommets qu'on a en stock
        public int Order => vertexIndex.Count;

        public bool Directed => directed;

        public void AddVertex(string name, float value = 0)
        {
            if (vertexIndex.ContainsKey(name))
                throw new ArgumentException();

            // L'index du nouveau sommet sera le nombre actuel de sommets
            int newIndex = vertexIndex.Count;
            vertexIndex.Add(name, newIndex);
            vertexValues.Add(name, value);

            // On agrandit la matrice d'adjacence
            matriceAdj.AddRow(newIndex);
            matriceAdj.AddColumn(newIndex);
        }

        public void RemoveVertex(string name)
        {
            if (!vertexIndex.ContainsKey(name))
                throw new ArgumentException();

            int indexToRemove = vertexIndex[name];
            // On nettoie les dicos
            vertexIndex.Remove(name);
            vertexValues.Remove(name);
            // On retire la ligne et la colonne dans la matrice
            matriceAdj.RemoveRow(indexToRemove);
            matriceAdj.RemoveColumn(indexToRemove);

            // Mise à jour des index des sommets restants (car la matrice a glissé)
            List<string> keys = new List<string>(vertexIndex.Keys);
            foreach (var k in keys)
            {
                if (vertexIndex[k] > indexToRemove)
                {
                    vertexIndex[k]--;
                }
            }
        }

        public float GetVertexValue(string name)
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
            // On parcourt la ligne du sommet dans la matrice pour voir avec qui il est lié
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

            // Vérification si l'arc existe déjà
            if (matriceAdj.GetValue(source, dest) != noEdgeValue)
                throw new ArgumentException();

            // Cas particulier du graphe non-orienté : l'arc (B,A) est le même que (A,B) 
            if (!directed && source != dest && matriceAdj.GetValue(dest, source) != noEdgeValue)
                throw new ArgumentException();

            matriceAdj.SetValue(source, dest, weight);
            if (!directed)
                matriceAdj.SetValue(dest, source, weight);
        }

        public void RemoveEdge(string sourceName, string destinationName)
        {
            if (!vertexIndex.ContainsKey(sourceName) || !vertexIndex.ContainsKey(destinationName))
                throw new ArgumentException();

            int source = vertexIndex[sourceName];
            int dest = vertexIndex[destinationName];
            // On ne veut pas écraser un arc existant par erreur
            if (matriceAdj.GetValue(source, dest) == noEdgeValue)
                throw new ArgumentException();
            // Si c'est un graphe non-orienté, on fait l'aller-retour automatiquement
            matriceAdj.SetValue(source, dest, noEdgeValue);
            if (!directed)
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
            if (!directed)
            {
                matriceAdj.SetValue(dest, source, weight);
            }
        }
    }
}
