using System;
using System.Collections.Generic;

namespace TourneeFutee
{
    public class Matrix
    {
        // TODO : ajouter tous les attributs que vous jugerez pertinents 
        private List<List<float>> data;
        private float defaultValue;

        /* Crée une matrice de dimensions `nbRows` x `nbColums`.
         * Toutes les cases de cette matrice sont remplies avec `defaultValue`.
         * Lève une ArgumentOutOfRangeException si une des dimensions est négative
         */
        public Matrix(int nbRows = 0, int nbColumns = 0, float defaultValue = 0)
        {
            if (nbRows < 0 || nbColumns < 0)
            {

                throw new ArgumentOutOfRangeException();
            }

            this.defaultValue = defaultValue;
            data = new List<List<float>>();

            for (int i = 0; i < nbRows; i++)
            {
                List<float> row = new List<float>();
                for (int j = 0; j < nbColumns; j++)
                {
                    row.Add(defaultValue);
                }
                data.Add(row);
            }
        }

        // Propriété : valeur par défaut utilisée pour remplir les nouvelles cases
        // Lecture seule
        public float DefaultValue
        {
            get { return defaultValue; }
            // pas de set
        }

        // Propriété : nombre de lignes
        // Lecture seule
        public int NbRows
        {
            get { return data.Count; }
            // pas de set
        }

        // Propriété : nombre de colonnes
        // Lecture seule
        public int NbColumns
        {
            get { return NbRows == 0 ? 0 : data[0].Count; }
            // pas de set
        }

        /* Insère une ligne à l'indice `i`. Décale les lignes suivantes vers le bas.
         * Toutes les cases de la nouvelle ligne contiennent DefaultValue.
         * Si `i` = NbRows, insère une ligne en fin de matrice
         * Lève une ArgumentOutOfRangeException si `i` est en dehors des indices valides
         */
        public void AddRow(int i)
        {
            if (i < 0 || i > NbRows)
                throw new ArgumentOutOfRangeException();

            List<float> newRow = new List<float>();
            for (int j = 0; j < NbColumns; j++)
            {
                newRow.Add(defaultValue);
            }

            data.Insert(i, newRow);
        }

        /* Insère une colonne à l'indice `j`. Décale les colonnes suivantes vers la droite.
         * Toutes les cases de la nouvelle ligne contiennent DefaultValue.
         * Si `j` = NbColums, insère une colonne en fin de matrice
         * Lève une ArgumentOutOfRangeException si `j` est en dehors des indices valides
         */
        public void AddColumn(int j)
        {
            if (j < 0 || j > NbColumns)
                throw new ArgumentOutOfRangeException();

            foreach (var row in data)
            {
                row.Insert(j, defaultValue);
            }
        }

        // Supprime la ligne à l'indice `i`. Décale les lignes suivantes vers le haut.
        // Lève une ArgumentOutOfRangeException si `i` est en dehors des indices valides
        public void RemoveRow(int i)
        {
            if (i < 0 || i >= NbRows)
                throw new ArgumentOutOfRangeException();

            data.RemoveAt(i);
        }

        // Supprime la colonne à l'indice `j`. Décale les colonnes suivantes vers la gauche.
        // Lève une ArgumentOutOfRangeException si `j` est en dehors des indices valides
        public void RemoveColumn(int j)
        {
            if (j < 0 || j >= NbColumns)
                throw new ArgumentOutOfRangeException();

            foreach (var row in data)
            {
                row.RemoveAt(j);
            }
        }

        // Renvoie la valeur à la ligne `i` et colonne `j`
        // Lève une ArgumentOutOfRangeException si `i` ou `j` est en dehors des indices valides
        public float GetValue(int i, int j)
        {
            if (i < 0 || i >= NbRows || j < 0 || j >= NbColumns)
            {

            }
            

            return data[i][j];
        }

        // Affecte la valeur à la ligne `i` et colonne `j` à `v`
        // Lève une ArgumentOutOfRangeException si `i` ou `j` est en dehors des indices valides
        public void SetValue(int i, int j, float v)
        {
            if (i < 0 || i >= NbRows || j < 0 || j >= NbColumns)
            {
                throw new ArgumentOutOfRangeException();
            }
              

            data[i][j] = v;
        }

        // Affiche la matrice
        public void Print()
        {
            for (int i = 0; i < NbRows; i++)
            {
                for (int j = 0; j < NbColumns; j++)
                {
                    Console.Write(data[i][j] + " ");
                }
                Console.WriteLine();
            }
        }

        // TODO : ajouter toutes les méthodes que vous jugerez pertinentes 
    }
}
