using System;
using System.Collections.Generic;

namespace TourneeFutee
{
    public class Graph
    {
        private Dictionary<string, int> vertexIndex;
        private Dictionary<string, float> vertexValues;
        private Matrix matriceAdj;
        private bool directed;
        private float noEdgeValue;

        public List<string> Nodes => new List<string>(vertexIndex.Keys);

        // Compatible avec : new Graph(isOriented: true)
        public Graph(bool isOriented, float noEdgeValue = 0)
        {
            this.directed = isOriented;
            this.noEdgeValue = noEdgeValue;
            vertexIndex = new Dictionary<string, int>();
            vertexValues = new Dictionary<string, float>();
            matriceAdj = new Matrix(0, 0, noEdgeValue);
        }

        // Alias attendu par les tests
        public int VertexCount => vertexIndex.Count;

        // Alias attendu par les tests
        public bool IsOriented => directed;

        // Tu peux garder les anciens noms si d'autres classes les utilisent
        public int Order => vertexIndex.Count;
        public bool Directed => directed;

        public bool ContainsVertex(string name)
        {
            return vertexIndex.ContainsKey(name);
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
                    vertexIndex[k]--;
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

            foreach (var kvp in vertexIndex)
            {
                int j = kvp.Value;
                if (matriceAdj.GetValue(i, j) != noEdgeValue)
                    neighborNames.Add(kvp.Key);
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

            if (matriceAdj.GetValue(source, dest) == noEdgeValue)
                throw new ArgumentException();

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
                matriceAdj.SetValue(dest, source, weight);
        }
    }
}