using System;
using ASD.Graphs;
using ASD;
using System.Collections.Generic;
using System.Linq;

namespace ASD
{

    public class Lab15 : System.MarshalByRefObject
    {
        /// <summary>
        /// Etap 1: Rozmiar najliczniejszej krainy w zadanym grafie (2.5p)
        /// 
        /// Przez krainę rozumiemy maksymalny zbiór wierzchołków, z których
        /// każde dwa należą do jakiegoś cyklu (równoważnie: najliczniejszy
        /// zbiór wierzchołków G indukujący podgraf 2-spójny wierzchołkowo).
        /// 
        /// Uwaga: Z powyższej definicji wynika, że zbiór pusty jest krainą, 
        /// a zbiór jednoelementowy nie.
        /// </summary>
        /// <param name="G">Graf prosty</param>
        /// <returns>Rozmiar największej bańki</returns>
        public int MaxProvinceSize(Graph G)
        {
            var visited = new bool[G.VertexCount];
            for (int i = 0; i < G.VertexCount; i++) visited[i] = false;
            var disc = new int[G.VertexCount];
            for (int i = 0; i < G.VertexCount; i++) disc[i] = 0;
            var low = new int[G.VertexCount];
            for (int i = 0; i < G.VertexCount; i++) low[i] = 0;
            var parent = new int[G.VertexCount];
            for (int i = 0; i < G.VertexCount; i++) parent[i] = -1;
            var stack = new Stack<(int, int)>();
            int maxSize = 0;
            for (var i = 0; i < G.VertexCount; i++)
            {
                if (!visited[i])
                {
                    DFSBiconnectd(i, 0);
                }
            }
            void DFSBiconnectd(int u, int time)
            {
                visited[u] = true;
                disc[u] = low[u] = time;
                time++;
                foreach (int v in G.OutNeighbors(u))
                {
                    if (!visited[v])
                    {
                        parent[v] = u;
                        stack.Push((u, v));
                        DFSBiconnectd(v, time);
                        low[u] = Math.Min(low[u], low[v]);
                        if (low[v] >= disc[u])
                        {
                            var component = new HashSet<int>();
                            while (stack.Count > 0)
                            {
                                var (x, y) = stack.Pop();
                                component.Add(x);
                                component.Add(y);
                                
                                if (x == u && y == v) 
                                    break;
                            }
                    
                            maxSize = Math.Max(maxSize, component.Count);
                        }
                    } else if (v != parent[u] && disc[v] < disc[u])
                    {
                        low[u] = Math.Min(low[u], disc[v]);
                        stack.Push((u, v));
                    }
                }
            }

            if (maxSize > 2)
            {
                return maxSize;
            }
            else
            {
                return 0;
            }
            
        }
        /// <summary>
        /// Etap 2: Wierzchołek znajdujący się w największej liczbie krain (2.5p)
        /// 
        /// Funcja zwraca wierzchołek znajdujący się w największej liczbie krain.
        /// 
        /// W przypadku remisu należy zwrócić wierzchołek o mniejszym numerze.
        /// </summary>
        /// <param name="G"></param>
        /// <returns></returns>
        public int VertexInMostProvinces(Graph G)
        {
            var visited = new bool[G.VertexCount];
            for (int i = 0; i < G.VertexCount; i++) visited[i] = false;
            var disc = new int[G.VertexCount];
            for (int i = 0; i < G.VertexCount; i++) disc[i] = 0;
            var low = new int[G.VertexCount];
            for (int i = 0; i < G.VertexCount; i++) low[i] = 0;
            var parent = new int[G.VertexCount];
            for (int i = 0; i < G.VertexCount; i++) parent[i] = -1;
            var stack = new Stack<(int, int)>();
            var how_many_articulation = new int[G.VertexCount];
            for (int i = 0; i < G.VertexCount; i++) how_many_articulation[i] = 0;
            int maxSize = 0;
            for (var i = 0; i < G.VertexCount; i++)
            {
                if (!visited[i])
                {
                    DFSBiconnectd(i, 0);
                }
            }
            void DFSBiconnectd(int u, int time)
            {
                visited[u] = true;
                disc[u] = low[u] = time;
                time++;
                foreach (int v in G.OutNeighbors(u))
                {
                    if (!visited[v])
                    {
                        parent[v] = u;
                        stack.Push((u, v));
                        DFSBiconnectd(v, time);
                        low[u] = Math.Min(low[u], low[v]);
                        if (low[v] >= disc[u])
                        {
                            var currentBCCVertices = new HashSet<int>();
                            while (stack.Count > 0)
                            {
                                var (edgeX, edgeY) = stack.Pop();
                                currentBCCVertices.Add(edgeX);
                                currentBCCVertices.Add(edgeY);
                                if (edgeX == u && edgeY == v)
                                {
                                    break;
                                }
                            }
          
                            if (currentBCCVertices.Count >= 3)
                            {
                                foreach (int vertexInKraina in currentBCCVertices)
                                {
                                    how_many_articulation[vertexInKraina]++;
                                }
                            }
                        }
                    } else if (v != parent[u] && disc[v] < disc[u])
                    {
                        low[u] = Math.Min(low[u], disc[v]);
                    }
                }
            }

            int max_index = 0;
            int value = -1;
            for (int i = 0; i < G.VertexCount; i++)
            {
                if (how_many_articulation[i] > value)
                {
                    max_index = i;
                    value = how_many_articulation[i];
                }
            }
            return max_index;
        }
    }
}