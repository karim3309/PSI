using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace TourneeFutee
{

    public class ServicePersistance
    {
        private readonly string _connectionString;
        private const string DB_SERVER = "127.0.0.1";
        private const string DB_NAME = "tourneefutee_test";
        private const string DB_USER = "root";
        private const string DB_PWD = "root";

        public ServicePersistance(string serverIp = DB_SERVER, string dbname = DB_NAME, string user = DB_USER, string pwd = DB_PWD)
        {
            _connectionString = $"server={serverIp};database={dbname};uid={user};pwd={pwd};";
            using (var conn = OpenConnection()) { }
        }

        public uint SaveGraph(Graph g)
        {
            using (var conn = OpenConnection())
            {
                List<string> nodes = g.Nodes;

                uint graphId;
                string sqlGraphe = "INSERT INTO Graphe(nb_sommets, est_oriente) VALUES (@nb, @oriente); SELECT LAST_INSERT_ID();";
                using (var cmd = new MySqlCommand(sqlGraphe, conn))
                {
                    cmd.Parameters.AddWithValue("@nb", nodes.Count);
                    cmd.Parameters.AddWithValue("@oriente", g.Directed ? 1 : 0);
                    graphId = Convert.ToUInt32(cmd.ExecuteScalar());
                }

                var sommetDbIds = new Dictionary<string, uint>();
                string sqlSommet = "INSERT INTO Sommet(nom, valeur, graphe_id) VALUES (@nom, @v, @gid); SELECT LAST_INSERT_ID();";
                foreach (string nom in nodes)
                {
                    using (var cmd = new MySqlCommand(sqlSommet, conn))
                    {
                        cmd.Parameters.AddWithValue("@nom", nom);
                        cmd.Parameters.AddWithValue("@v", g.GetVertexValue(nom));
                        cmd.Parameters.AddWithValue("@gid", graphId);
                        sommetDbIds[nom] = Convert.ToUInt32(cmd.ExecuteScalar());
                    }
                }

                string sqlArc = "INSERT INTO Arc(sommet_source_id, sommet_dest_id, poids, graphe_id) VALUES (@src, @dst, @poids, @gid);";
                foreach (string source in nodes)
                {
                    foreach (string dest in g.GetNeighbors(source))
                    {
                        if (!g.Directed && string.Compare(source, dest, StringComparison.Ordinal) > 0)
                            continue;
                        using (var cmd = new MySqlCommand(sqlArc, conn))
                        {
                            cmd.Parameters.AddWithValue("@src", sommetDbIds[source]);
                            cmd.Parameters.AddWithValue("@dst", sommetDbIds[dest]);
                            cmd.Parameters.AddWithValue("@poids", g.GetEdgeWeight(source, dest));
                            cmd.Parameters.AddWithValue("@gid", graphId);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                return graphId;
            }
        }
        public Graph LoadGraph(uint id)
        {
            using (var conn = OpenConnection())
            {
                bool estOriente;
                using (var cmd = new MySqlCommand("SELECT est_oriente FROM Graphe WHERE id = @id;", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read()) throw new Exception($"Graphe {id} introuvable.");
                        estOriente = reader.GetBoolean("est_oriente");
                    }
                }

                var sommetIdToNom = new Dictionary<uint, string>();
                var sommetNomToValeur = new Dictionary<string, float>();
                using (var cmd = new MySqlCommand("SELECT id, nom, valeur FROM Sommet WHERE graphe_id = @gid ORDER BY id ASC;", conn))
                {
                    cmd.Parameters.AddWithValue("@gid", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            uint sid = Convert.ToUInt32(reader["id"]);
                            string nom = reader["nom"].ToString();
                            sommetIdToNom[sid] = nom;
                            sommetNomToValeur[nom] = reader.GetFloat("valeur");
                        }
                    }
                }

                var graph = new Graph(estOriente, noEdgeValue: 0);
                foreach (string nom in sommetIdToNom.Values)
                    graph.AddVertex(nom, sommetNomToValeur[nom]);

                using (var cmd = new MySqlCommand("SELECT sommet_source_id, sommet_dest_id, poids FROM Arc WHERE graphe_id = @gid;", conn))
                {
                    cmd.Parameters.AddWithValue("@gid", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string srcNom = sommetIdToNom[Convert.ToUInt32(reader["sommet_source_id"])];
                            string dstNom = sommetIdToNom[Convert.ToUInt32(reader["sommet_dest_id"])];
                            graph.AddEdge(srcNom, dstNom, reader.GetFloat("poids"));
                        }
                    }
                }
                return graph;
            }
        }

        public uint SaveTour(uint graphId, Tour t)
        {
            using (var conn = OpenConnection())
            {
                uint tourId;
                using (var cmd = new MySqlCommand("INSERT INTO Tournee(cout_total, graphe_id) VALUES (@cout, @gid); SELECT LAST_INSERT_ID();", conn))
                {
                    cmd.Parameters.AddWithValue("@cout", t.Cost);
                    cmd.Parameters.AddWithValue("@gid", graphId);
                    tourId = Convert.ToUInt32(cmd.ExecuteScalar());
                }

                var nomToDbId = new Dictionary<string, uint>();
                using (var cmd = new MySqlCommand("SELECT id, nom FROM Sommet WHERE graphe_id = @gid ORDER BY id ASC;", conn))
                {
                    cmd.Parameters.AddWithValue("@gid", graphId);
                    using (var reader = cmd.ExecuteReader())
                        while (reader.Read())
                            nomToDbId[reader["nom"].ToString()] = Convert.ToUInt32(reader["id"]);
                }

                string sqlEtape = "INSERT INTO EtapeTournee(tournee_id, numero_ordre, sommet_id) VALUES (@tid, @ordre, @sid);";
                for (int ordre = 0; ordre < t.Vertices.Count; ordre++)
                {
                    using (var cmd = new MySqlCommand(sqlEtape, conn))
                    {
                        cmd.Parameters.AddWithValue("@tid", tourId);
                        cmd.Parameters.AddWithValue("@ordre", ordre);
                        cmd.Parameters.AddWithValue("@sid", nomToDbId[t.Vertices[ordre]]);
                        cmd.ExecuteNonQuery();
                    }
                }
                return tourId;
            }
        }

        public Tour LoadTour(uint id)
        {
            using (var conn = OpenConnection())
            {
                float coutTotal;
                uint graphId;
                using (var cmd = new MySqlCommand("SELECT cout_total, graphe_id FROM Tournee WHERE id = @id;", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read()) throw new Exception($"Tournée {id} introuvable.");
                        coutTotal = reader.GetFloat("cout_total");
                        graphId = Convert.ToUInt32(reader["graphe_id"]);
                    }
                }

                var sommetIdToNom = new Dictionary<uint, string>();
                using (var cmd = new MySqlCommand("SELECT id, nom FROM Sommet WHERE graphe_id = @gid ORDER BY id ASC;", conn))
                {
                    cmd.Parameters.AddWithValue("@gid", graphId);
                    using (var reader = cmd.ExecuteReader())
                        while (reader.Read())
                            sommetIdToNom[Convert.ToUInt32(reader["id"])] = reader["nom"].ToString();
                }

                var sequence = new List<string>();
                using (var cmd = new MySqlCommand("SELECT sommet_id FROM EtapeTournee WHERE tournee_id = @tid ORDER BY numero_ordre ASC;", conn))
                {
                    cmd.Parameters.AddWithValue("@tid", id);
                    using (var reader = cmd.ExecuteReader())
                        while (reader.Read())
                            sequence.Add(sommetIdToNom[Convert.ToUInt32(reader["sommet_id"])]);
                }

                return new Tour(sequence, coutTotal);
            }
        }


        private MySqlConnection OpenConnection()
        {
            var conn = new MySqlConnection(_connectionString);
            conn.Open();
            return conn;
        }
    }
}