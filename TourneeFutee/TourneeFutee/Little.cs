namespace TourneeFutee
{
    // Résout le problème de voyageur de commerce défini par le graphe `graph`
    // en utilisant l'algorithme de Little
    public class Little
    {
        private Graph graph;
        private int nbCities;
        private List<string> cities;
        private float bestCost;
        // Avant : private List<(string, string)> bestTour;
        private List<(string source, string destination)> bestTour;
    

        // Instancie le planificateur en spécifiant le graphe modélisant un problème de voyageur de commerce
        public Little(Graph graph)
        {
            this.graph = graph;
            this.nbCities = graph.Nodes.Count;
            this.cities = graph.Nodes.ToList();
            this.bestCost = float.MaxValue;
            this.bestTour = null;
        }

        // Trouve la tournée optimale dans le graphe `this.graph`
        // (c'est à dire le cycle hamiltonien de plus faible coût)
        public Tour ComputeOptimalTour()
        {
            // Initialiser la matrice des coûts
            Matrix costMatrix = InitializeCostMatrix();

            // Lancer l'algorithme de branch and bound
            BranchAndBound(costMatrix, new List<(string, string)>(), 0);

            // Construire la tournée à partir des segments trouvés
            if (bestTour != null)
            {
                Tour tour = new Tour();

                // AJOUT INDISPENSABLE POUR LES TESTS : 
                // On assigne le coût optimal calculé par le Branch & Bound
                tour.Cost = bestCost;

                foreach (var segment in bestTour)
                {
                    tour.AddSegment(segment.source, segment.destination);
                }
                return tour;
            }

            return new Tour();
        }

        // --- Méthodes utilitaires réalisant des étapes de l'algorithme de Little

        // Réduit la matrice `m` et revoie la valeur totale de la réduction
        // Après appel à cette méthode, la matrice `m` est *modifiée*.
        public static float ReduceMatrix(Matrix m)
        {
            float totalReduction = 0;
            int rows = m.NbRows;
            int cols = m.NbColumns;

            // Réduction des lignes
            for (int i = 0; i < rows; i++)
            {
                float min = float.MaxValue;
                for (int j = 0; j < cols; j++)
                {
                    float val = m.GetValue(i, j);
                    if (val < min)
                        min = val;
                }

                if (min > 0 && min < float.MaxValue)
                {
                    totalReduction += min;
                    for (int j = 0; j < cols; j++)
                    {
                        float val = m.GetValue(i, j);
                        if (val < float.MaxValue)
                            m.SetValue(i, j, val - min);
                    }
                }
            }

            // Réduction des colonnes
            for (int j = 0; j < cols; j++)
            {
                float min = float.MaxValue;
                for (int i = 0; i < rows; i++)
                {
                    float val = m.GetValue(i, j);
                    if (val < min)
                        min = val;
                }

                if (min > 0 && min < float.MaxValue)
                {
                    totalReduction += min;
                    for (int i = 0; i < rows; i++)
                    {
                        float val = m.GetValue(i, j);
                        if (val < float.MaxValue)
                            m.SetValue(i, j, val - min);
                    }
                }
            }

            return totalReduction;
        }

        // Renvoie le regret de valeur maximale dans la matrice de coûts `m` sous la forme d'un tuple `(int i, int j, float value)`
        // où `i`, `j`, et `value` contiennent respectivement la ligne, la colonne et la valeur du regret maximale
        public static (int i, int j, float value) GetMaxRegret(Matrix m)
        {
            int rows = m.NbRows;
            int cols = m.NbColumns;
            float maxRegret = -1;
            int maxI = -1, maxJ = -1;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (m.GetValue(i, j) == 0)
                    {
                        // Calculer le minimum sur la ligne (hors case courante)
                        float minRow = float.MaxValue;
                        for (int k = 0; k < cols; k++)
                        {
                            if (k != j)
                            {
                                float val = m.GetValue(i, k);
                                if (val < minRow)
                                    minRow = val;
                            }
                        }

                        // Calculer le minimum sur la colonne (hors case courante)
                        float minCol = float.MaxValue;
                        for (int k = 0; k < rows; k++)
                        {
                            if (k != i)
                            {
                                float val = m.GetValue(k, j);
                                if (val < minCol)
                                    minCol = val;
                            }
                        }

                        float regret = (minRow < float.MaxValue ? minRow : 0) +
                                      (minCol < float.MaxValue ? minCol : 0);

                        if (regret > maxRegret)
                        {
                            maxRegret = regret;
                            maxI = i;
                            maxJ = j;
                        }
                    }
                }
            }

            return (maxI, maxJ, maxRegret);
        }

        /* Renvoie vrai si le segment `segment` est un trajet parasite, c'est-à-dire s'il ferme prématurément la tournée incluant les trajets contenus dans `includedSegments`
         * Une tournée est incomplète si elle visite un nombre de villes inférieur à `nbCities`
         */
        public static bool IsForbiddenSegment((string source, string destination) segment, List<(string source, string destination)> includedSegments, int nbCities)
        {
            // Vérifier si c'est le segment inverse d'un segment déjà inclus (cas simple)
            foreach (var incSegment in includedSegments)
            {
                if (incSegment.source == segment.destination && incSegment.destination == segment.source)
                    return true;
            }

            // Vérifier si l'ajout de ce segment crée un cycle prématuré
            // Construire le graphe des segments inclus
            Dictionary<string, string> next = new Dictionary<string, string>();
            foreach (var incSegment in includedSegments)
            {
                next[incSegment.source] = incSegment.destination;
            }

            // Ajouter le segment candidat temporairement
            string current = segment.source;
            int length = 1;

            while (next.ContainsKey(current))
            {
                current = next[current];
                length++;
                if (current == segment.source) // On a trouvé un cycle
                {
                    // Si le cycle inclut toutes les villes, c'est permis
                    // Sinon, c'est un cycle prématuré (parasite)
                    return length < nbCities;
                }
            }

            return false;
        }

        // Méthode privée pour initialiser la matrice des coûts
        private Matrix InitializeCostMatrix()
        {
            Matrix matrix = new Matrix(nbCities, nbCities);

            for (int i = 0; i < nbCities; i++)
            {
                for (int j = 0; j < nbCities; j++)
                {
                    if (i == j)
                    {
                        matrix.SetValue(i, j, float.PositiveInfinity);
                    }
                    else
                    {
                        string source = cities[i];
                        string dest = cities[j];
                        matrix.SetValue(i, j, graph.GetEdgeWeight(source, dest));
                    }
                }
            }

            return matrix;
        }

        // Méthode récursive de branch and bound
        private void BranchAndBound(Matrix matrix, List<(string, string)> includedSegments, float currentCost)
        {
            // Réduire la matrice
            float reduction = ReduceMatrix(matrix);
            currentCost += reduction;

            // Vérifier si on a une solution complète
            if (includedSegments.Count == nbCities)
            {
                if (currentCost < bestCost)
                {
                    bestCost = currentCost;
                    bestTour = new List<(string, string)>(includedSegments);
                }
                return;
            }

            // Élagage
            if (currentCost >= bestCost)
                return;

            // Trouver le segment avec le regret maximal
            (int i, int j, float regret) = GetMaxRegret(matrix);
            if (i == -1 || j == -1)
                return;

            string source = cities[i];
            string dest = cities[j];
            var segment = (source, dest);

            // Branche 1: Inclure le segment
            if (!IsForbiddenSegment(segment, includedSegments, nbCities))
            {
                Matrix newMatrix = CloneMatrix(matrix);
                List<(string, string)> newIncluded = new List<(string, string)>(includedSegments);
                newIncluded.Add(segment);

                // Supprimer la ligne i et la colonne j
                for (int k = 0; k < nbCities; k++)
                {
                    newMatrix.SetValue(i, k, float.PositiveInfinity);
                    newMatrix.SetValue(k, j, float.PositiveInfinity);
                }

                // Interdire le segment qui créerait un sous-cycle
                // Trouver la prochaine ville après dest
                string next = dest;
                while (newIncluded.Any(s => s.source == next))
                {
                    next = newIncluded.Find(s => s.source == next).destination;
                }

                int nextIndex = cities.IndexOf(next);
                if (nextIndex >= 0)
                    newMatrix.SetValue(nextIndex, i, float.PositiveInfinity);

                BranchAndBound(newMatrix, newIncluded, currentCost);
            }

            // Branche 2: Exclure le segment
            if (currentCost + regret < bestCost)
            {
                Matrix newMatrix = CloneMatrix(matrix);
                newMatrix.SetValue(i, j, float.PositiveInfinity);
                BranchAndBound(newMatrix, includedSegments, currentCost);
            }
        }

        // Méthode utilitaire pour cloner une matrice
        private Matrix CloneMatrix(Matrix source)
        {
            Matrix clone = new Matrix(source.NbRows, source.NbColumns);

            for (int i = 0; i < source.NbRows; i++)
            {
                for (int j = 0; j < source.NbColumns; j++)
                {
                    clone.SetValue(i, j, source.GetValue(i, j));
                }
            }

            return clone;
        }
    }
}