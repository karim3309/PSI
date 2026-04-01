namespace TourneeFutee
{
    // Modélise une tournée dans le cadre du problème du voyageur de commerce
    public class Tour
    {
        // TODO : ajouter tous les attributs que vous jugerez pertinents 
        private List<(string source, string destination, float cost)> _segments = new();
        // propriétés

        // Coût total de la tournée
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

        // Nombre de trajets dans la tournée
        public int NbSegments
        {
            get { return _segments.Count; }    // TODO : implémenter
        }


        // Renvoie vrai si la tournée contient le trajet `source`->`destination`
        public bool ContainsSegment((string source, string destination) segment)
        {
            foreach (var seg in _segments)
                if (seg.source == segment.source && seg.destination == segment.destination)
                { 
                    return true;
                }
            return false;   
        }


        // Affiche les informations sur la tournée : coût total et trajets
        public void Print()
        {
            Console.WriteLine("Coût total : " + Cost);
            foreach (var seg in _segments)
            {
                Console.WriteLine("  " + seg.source + " -> " + seg.destination + " (coût : " + seg.cost + ")");
            }
        }
        

        // TODO : ajouter toutes les méthodes que vous jugerez pertinentes 

    }
}
