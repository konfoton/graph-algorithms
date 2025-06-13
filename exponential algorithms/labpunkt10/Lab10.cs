using ASD.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;

namespace ASD
{
    public class Lab10 : MarshalByRefObject
    {
        /// <summary>
        /// Szukanie najdłuższego powtórzenia w zadanym kolorowaniu grafu.
        /// </summary>
        /// <param name="G">Graf prosty</param>
        /// <param name="color">Kolorowanie wierzchołków G (color[v] to kolor wierzchołka v)</param>
        /// <returns>Ścieżka, na której występuje powtórzenie (numery kolejnych wierzchołków)</returns>
        /// <remarks>W przypadku braku powtórzeń należy zwrócić null lub tablicę o długości 0</remarks>
        public int[] FindLongestRepetition(Graph G, int[] color)
        {

            Dictionary<int, int> counts_all = new Dictionary<int, int>();
            Dictionary<int, int> counts_temp = new Dictionary<int, int>();
            for (int i = 0; i < G.VertexCount; i++)
            {
                if (!counts_all.ContainsKey(color[i]))
                {
                    counts_all[color[i]] = 1;
                    counts_temp[color[i]] = 0;
                }
                else
                {
                    counts_all[color[i]]++;
                }
            }
            
            
            int length_longest = 0;
            int[] longestarray = new int [G.VertexCount];
            int length = 0;
            int[] currentarray= new int[G.VertexCount];
            
            
            
            int []visited = new int [G.VertexCount];
            
            for (int i = 0; i < G.VertexCount; i++)
            {
                visited[i] = -1;
            }
            void sub_func(int vertex)
            {
                if (length % 2 == 1)
                {
                }
                else
                {
                    if (length > G.VertexCount / 2 + 1)
                    {
                        int number_to_scan = length - G.VertexCount / 2;
                        bool exists = false;
                        bool temp = true;
                        for (int i = length / 2; i < length- number_to_scan; i++)
                        {
                            for (int k = 0; k < number_to_scan; k++)
                            {
                                if (color[currentarray[i + k]] != color[currentarray[k]])
                                {
                                    temp = false;
                                    break;
                                }
                            }
                        
                            if (temp)
                            {
                                exists = true;
                                break;
                            }
                            if (!temp)
                            {
                                temp = true;
                            }
                        }
                        if (!exists)
                        {
                            return;
                        }
                    }
                    bool found = true;
                    for (int i = 0; i < length / 2; i++)
                    {
                        if (color[currentarray[i]] != color[currentarray[length/2 + i]])
                        {
                            found = false;
                            break;
                        }
                        
                    }
                    if (found)
                    {
                        if (length_longest < length)
                        {
                            length_longest = length;
                            for (int i = 0; i < length; i++)
                            {
                                longestarray[i] = currentarray[i];
                            }
                        }
                    }
                }
                
                foreach(int v in G.OutNeighbors(vertex))
                {
                    if (visited[v] != -1)
                    {
                        continue;
                    }

                    if (counts_temp[color[v]] + 1 == counts_all[color[v]] && counts_all[color[v]] % 2 == 1)
                    {
                        continue;
                    }

                    if (length == length_longest / 2)
                    {
                        if (counts_temp[color[v]]>= counts_all[color[v]]/2)
                        {
                            continue;
                        }
                    }
                    currentarray[length] = v;
                    length++;
                    counts_temp[color[v]]++;
                    visited[v] = 1;
                    sub_func(v);
                    counts_temp[color[v]]--;
                    length--;
                    visited[v] = -1;
                    
                }
            }

            for (int i = 0; i < G.VertexCount; i++)
            {
                currentarray[length] = i;
                counts_temp[color[i]]++;
                length++;
                visited[i] = 1;
                sub_func(i);
                counts_temp[color[i]]--;
                length--;
                visited[i] = -1;
            }
            int[] result = new int[length_longest];
            for (int i = 0; i < length_longest; i++)
            {
                result[i] = longestarray[i];
            }
            return result;
        }
    }

}