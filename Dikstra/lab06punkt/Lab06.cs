using ASD.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASD
{
    public class Lab06 : MarshalByRefObject
    {
        /// <summary>Etap I</summary>
        /// <param name="G">Graf opisujący połączenia szlakami turystycznymi z podanym czasem przejścia krawędzi w wadze.</param>
        /// <param name="waitTime">Czas oczekiwania Studenta-Podróżnika w danym wierzchołku.</param>
        /// <param name="s">Wierzchołek startowy (początek trasy).</param>
        /// <returns>Pierwszy element krotki to wierzchołek końcowy szukanej trasy. Drugi element to długość trasy w minutach. Trzeci element to droga będąca rozwiązaniem: sekwencja odwiedzanych wierzchołków (zawierająca zarówno wierzchołek początkowy, jak i końcowy).</returns>
        public (int t, int l, int[] path) Stage1(DiGraph<int> G, int[] waitTime, int s)
        {
            DiGraph<int> G2 = (DiGraph<int>)G.Clone();
            for (int i = 0; i < G2.VertexCount; i++)
            {
                if (i != s)
                {
                    foreach (int vertex in G.OutNeighbors(i))
                    {
                        G2.SetEdgeWeight(i, vertex, G2.GetEdgeWeight(i, vertex) + waitTime[i]);
                    }
                }
            }

            int[] distance = new int[G2.VertexCount];
            int[] prev = new int[G2.VertexCount];
            for (int i = 0; i < G2.VertexCount; i++)
            {
                prev[i] = -1;
            }

            int[] visited = new int[G2.VertexCount];
            for (int i = 0; i < G2.VertexCount; i++)
            {
                distance[i] = int.MaxValue;
                visited[i] = 0;
            }

            distance[s] = 0;
            PriorityQueue<int, int> pq = new PriorityQueue<int, int>();
            pq.Insert(s, 0);
            while (pq.Count > 0)
            {
                int k = pq.Extract();
                if (visited[k] == 1) continue;
                visited[k] = 1;
                foreach (int vertex in G2.OutNeighbors(k))
                {
                    if (distance[vertex] > distance[k] + G2.GetEdgeWeight(k, vertex))
                    {
                        prev[vertex] = k;
                        distance[vertex] = distance[k] + G2.GetEdgeWeight(k, vertex);
                        pq.Insert(vertex, distance[vertex]);
                    }
                }
            }

            int longest_dinstce = 0;
            int longest_vertex = 0;
            for (int i = 0; i < G2.VertexCount; i++)
            {
                if (distance[i] > longest_dinstce && distance[i] != int.MaxValue)
                {
                    longest_dinstce = distance[i];
                    longest_vertex = i;
                }
            }

            List<int> path = new List<int>();
            if (longest_dinstce == int.MaxValue)
            {
                longest_dinstce = 0;
                longest_vertex = 0;
                return (longest_vertex, longest_dinstce, new int[0]);
            }
            else
            {
                int current = longest_vertex;
                while (current != s)
                {
                    path.Insert(0, current);
                    current = prev[current];
                }

                path.Insert(0, s);
            }

            return (longest_vertex, longest_dinstce, path.ToArray());
        }

        /// <summary>Etap II</summary>
        /// <param name="G">Graf opisujący połączenia szlakami turystycznymi z podanym czasem przejścia krawędzi w wadze.</param>
        /// <param name="C">Graf opisujący koszty przejścia krawędziami w grafie G.</param>
        /// <param name="waitTime">Czas oczekiwania Studenta-Podróżnika w danym wierzchołku.</param>
        /// <param name="s">Wierzchołek startowy (początek trasy).</param>
        /// <param name="t">Wierzchołek końcowy (koniec trasy).</param>
        /// <returns>Pierwszy element krotki to długość trasy w minutach. Drugi element to koszt przebycia trasy w złotych. Trzeci element to droga będąca rozwiązaniem: sekwencja odwiedzanych wierzchołków (zawierająca zarówno wierzchołek początkowy, jak i końcowy). Jeśli szukana trasa nie istnieje, funkcja zwraca `null`.</returns>
        public (int l, int c, int[] path)? Stage2(DiGraph<int> G, Graph<int> C, int[] waitTime, int s, int t)
        {
            DiGraph<int> G2 = (DiGraph<int>)G.Clone();
            for (int i = 0; i < G2.VertexCount; i++)
            {
                if (i != s)
                {
                    foreach (int vertex in G.OutNeighbors(i))
                    {
                        G2.SetEdgeWeight(i, vertex, G2.GetEdgeWeight(i, vertex) + waitTime[i]);
                    }
                }
            }

            int[] distance = new int[C.VertexCount];
            int[] distance2 = new int[C.VertexCount];
            int[] prev = new int[C.VertexCount];
            int[] prev2 = new int[C.VertexCount];
            for (int i = 0; i < C.VertexCount; i++)
            {
                prev[i] = -1;
                prev2[i] = -1;
            }

            int[] visited = new int[C.VertexCount];
            for (int i = 0; i < C.VertexCount; i++)
            {
                distance[i] = int.MaxValue;
                distance2[i] = int.MaxValue;
                visited[i] = 0;
            }

            distance[s] = 0;
            distance2[s] = 0;
            PriorityQueue<int, int> pq = new PriorityQueue<int, int>();
            pq.Insert(s, 0);
            while (pq.Count > 0)
            {
                int k = pq.Extract();
                if (visited[k] == 1) continue;
                visited[k] = 1;
                foreach (int vertex in C.OutNeighbors(k))
                {
                    if (distance[vertex] >= distance[k] + C.GetEdgeWeight(k, vertex) && G.HasEdge(k, vertex))
                    {
                        if (distance[vertex] > distance[k] + C.GetEdgeWeight(k, vertex))
                        {
                            distance2[vertex] = distance2[k] + G2.GetEdgeWeight(k, vertex);
                            prev2[vertex] = k;
                        }
                        if (distance2[vertex] > distance2[k] + G2.GetEdgeWeight(k, vertex))
                        {
                            distance2[vertex] = distance2[k] + G2.GetEdgeWeight(k, vertex);
                            prev2[vertex] = k;
                        }
                        distance[vertex] = distance[k] + C.GetEdgeWeight(k, vertex);
                        pq.Insert(vertex, distance[vertex]);
                    }
                }
            }

            int shortest = distance[t];
            List<int> path = new List<int>();
            if (shortest == int.MaxValue)
            {
                return null;
            }
            else
            {
                int current = t;
                while (current != s)
                {
                    path.Insert(0, current);
                    current = prev2[current];
                }

                path.Insert(0, s);
            }

            return (distance2[t], shortest, path.ToArray());


        }
    }
}
