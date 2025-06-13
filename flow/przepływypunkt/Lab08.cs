using ASD.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace ASD
{
    public class Lab08 : MarshalByRefObject
    {
        /// <summary>Etap I</summary>
        /// <param name="P">Tablica która dla każdego pola zawiera informacje, ile maszyn moze lacznie wyjechac z tego pola</param>
        /// <param name="MachinePos">Tablica zawierajaca informacje o poczatkowym polozeniu maszyn</param>
        /// <returns>Pierwszy element kroki to liczba uratowanych maszyn, drugi to tablica indeksów tych maszyn</returns>
        public (int savedNum, int[] Saved) Stage1(int[,] P, (int row, int col)[] MachinePos)
        {
            int h = P.GetLength(0);
            int w = P.GetLength(1);
            foreach (var pos in MachinePos)
            {
                if (pos.row < 0 || pos.row >= h || pos.col < 0 || pos.col >= w)
                    throw new ArgumentException("Invalid machine position.");
            }
            DiGraph<int> graph = new DiGraph<int>(h * w + 2 + h * w);
            for (int i = 0; i < MachinePos.Length; i++)
            {
                graph.AddEdge(2 * h * w, (MachinePos[i].row)* w + (MachinePos[i].col) + w * h, 1);
            }

            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    if (j + 1 < w)
                    {
                            graph.AddEdge(i * w + j, i * w + j + 1 + h * w, int.MaxValue);
                    }

                    if (j - 1 >= 0)
                    {
                            graph.AddEdge(i * w + j, i * w + j - 1 + h * w, int.MaxValue);
                    }

                    if (i - 1 >= 0)
                    {
                        graph.AddEdge(i * w + j, (i -1)* w + j + h * w, int.MaxValue);
                    }
                   
                    if (i + 1 < h)
                    {
                        graph.AddEdge(i * w + j, (i+1) * w + j + h * w, int.MaxValue);
                    }
                    graph.AddEdge(i * w + j + h * w, i * w + j, P[i, j]);
                    
                }
            }

            List<int> saved = new List<int>();
            for (int i = 0; i < w; i++)
            {
                graph.AddEdge(i, 2 * h * w + 1, int.MaxValue);
            }
            var (flowValue, f) = Flows.FordFulkerson(graph, 2 * h * w, 2 * h * w + 1);
            foreach(int v in f.OutNeighbors(2 * h * w))
            {
                    int column = (v - h * w) % w;
                    int row = (v - h * w - column) / w;
                    for (int i = 0; i < MachinePos.Length; i++)
                    {
                        if (row == MachinePos[i].row && column == MachinePos[i].col)
                        {
                            saved.Add(i);
                        }
                    }
                    
            }
            return (flowValue, saved.ToArray());
        }

        /// <summary>Etap II</summary>
        /// <param name="P">Tablica która dla każdego pola zawiera informacje, ile maszyn moze lacznie wyjechac z tego pola</param>
        /// <param name="MachinePos">Tablica zawierajaca informacje o poczatkowym polozeniu maszyn</param>
        /// <param name="MachineValue">Tablica zawierajaca informacje o wartosci maszyn</param>
        /// <param name="moveCost">Koszt jednego ruchu</param>
        /// <returns>Pierwszy element kroki to najwiekszy mozliwy zysk, drugi to tablica indeksow maszyn, ktorych wyprowadzenie maksymalizuje zysk</returns>
        public (int bestProfit, int[] Saved) Stage2(int[,] P, (int row, int col)[] MachinePos, int[] MachineValue, int moveCost)
        {
            int h = P.GetLength(0);
            int w = P.GetLength(1);
            foreach (var pos in MachinePos)
            {
                if (pos.row < 0 || pos.row >= h || pos.col < 0 || pos.col >= w)
                    throw new ArgumentException("Invalid machine position.");
            }
            NetworkWithCosts<int, int> graph = new 	NetworkWithCosts<int, int>(h * w + 2 + h * w + 1);
            graph.AddEdge(h * w * 2, h * w + 2 + h * w, MachinePos.GetLength(0), 0);
            graph.AddEdge(h * w + 2 + h * w,  2 * h * w + 1, MachinePos.GetLength(0), 0);
            Dictionary<int, int> dictionary = new Dictionary<int, int>();
            for (int i = 0; i < MachinePos.Length; i++)
            {
                graph.AddEdge(h * w + 2 + h * w, (MachinePos[i].row)* w + (MachinePos[i].col) + w * h, 1, -MachineValue[i]);
                dictionary.Add((MachinePos[i].row)* w + (MachinePos[i].col) + w * h, i);
            }

            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    if (j + 1 < w)
                    {
                            graph.AddEdge(i * w + j, i * w + j + 1 + h * w, P[i, j], moveCost);
                    }

                    if (j - 1 >= 0)
                    {
                            graph.AddEdge(i * w + j, i * w + j - 1 + h * w, P[i, j], moveCost);
                            
                    }

                    if (i - 1 >= 0)
                    {
                        graph.AddEdge(i * w + j, (i -1)* w + j + h * w, P[i, j], moveCost);
                    }
                   
                    if (i + 1 < h)
                    {
                        graph.AddEdge(i * w + j, (i+1) * w + j + h * w, P[i, j], moveCost);
                    }
                    graph.AddEdge(i * w + j + h * w, i * w + j, P[i, j], 0);
                    
                }
            }

            List<int> saved = new List<int>();
            for (int i = 0; i < w; i++)
            {
                graph.AddEdge(i, 2 * h * w + 1, P[0, i], moveCost);
            }
            var (flowValue, lowestCost,g) = Flows.MinCostMaxFlow(graph, 2 * h * w, 2 * h * w + 1);
            foreach(int v in g.OutNeighbors(h * w + 2 + h * w ))
            {
                    if (g.GetEdgeWeight(h * w + 2 + h * w, v) <= 0)
                    {
                        continue; 
                    }
                    if (v == 2 * h * w + 1)
                    {
                        continue;
                    }

                    if (dictionary.ContainsKey(v))
                    {
                        saved.Add(dictionary[v]);
                    }
                    
            }
            return (-lowestCost, saved.ToArray());
        }
    }
}