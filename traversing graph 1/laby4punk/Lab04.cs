using ASD.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASD
{
    public class Lab04 : MarshalByRefObject
    {
        /// <summary>
        /// Etap 1 - Wyznaczenie liczby oraz listy zainfekowanych serwisów po upływie K dni.
        /// Algorytm analizuje propagację infekcji w grafie i zwraca wszystkie dotknięte nią serwisy.
        /// </summary>
        /// <param name="G">Graf reprezentujący infrastrukturę serwisów.</param>
        /// <param name="K">Liczba dni propagacji infekcji.</param>
        /// <param name="s">Indeks początkowo zainfekowanego serwisu.</param>
        /// <returns>
        /// (int numberOfInfectedServices, int[] listOfInfectedServices) - 
        /// numberOfInfectedServices: liczba zainfekowanych serwisów,
        /// listOfInfectedServices: tablica zawierająca numery zainfekowanych serwisów w kolejności rosnącej.
        /// </returns>
        public (int numberOfInfectedServices, int[] listOfInfectedServices) Stage1(Graph G, int K, int s)
        {
            int []visited = new int[G.VertexCount];
            for (int i = 0; i < G.VertexCount; i++) visited[i] = 0;
            int []day = new int[G.VertexCount];
            List<int> listOfInfectedServices = new List<int>();
            Queue<int> q = new Queue<int>();
            q.Enqueue(s);
            visited[s] = 1;
            day[s] = 0;
            while (q.Count > 0)
            {
                int v = q.Dequeue();
                if (day[v] == K)
                {
                    break;
                }
                else
                {
                    listOfInfectedServices.Add(v);
                }
                foreach (int j in G.OutNeighbors(v))
                {
                    if (visited[j] == 0)
                    {
                        q.Enqueue(j);
                        visited[j] = 1;
                        day[j] = day[v] + 1;
                    }
                }
            }
            return (listOfInfectedServices.Count, listOfInfectedServices.ToArray());
            
            
            
        }

        /// <summary>
        /// Etap 2 - Wyznaczenie liczby oraz listy zainfekowanych serwisów przy uwzględnieniu wyłączeń.
        /// Algorytm analizuje propagację infekcji z możliwością wcześniejszego wyłączania serwisów.
        /// </summary>
        /// <param name="G">Graf reprezentujący infrastrukturę serwisów.</param>
        /// <param name="K">Liczba dni propagacji infekcji.</param>
        /// <param name="s">Tablica początkowo zainfekowanych serwisów.</param>
        /// <param name="serviceTurnoffDay">Tablica zawierająca dzień, w którym dany serwis został wyłączony (K + 1 oznacza brak wyłączenia).</param>
        /// <returns>
        /// (int numberOfInfectedServices, int[] listOfInfectedServices) - 
        /// numberOfInfectedServices: liczba zainfekowanych serwisów,
        /// listOfInfectedServices: tablica zawierająca numery zainfekowanych serwisów w kolejności rosnącej.
        /// </returns>
        public (int numberOfInfectedServices, int[] listOfInfectedServices) Stage2(Graph G, int K, int[] s, int[] serviceTurnoffDay)
        {
           
            int []visited = new int[G.VertexCount];
            for (int i = 0; i < G.VertexCount; i++) visited[i] = 0;
            int []day = new int[G.VertexCount];
            List<int> listOfInfectedServices = new List<int>();
            Queue<int> q = new Queue<int>();
            foreach (int vertex in s)
            {
                q.Enqueue(vertex);
                visited[vertex] = 1;
                day[vertex] = 1;
            }
            while (q.Count > 0)
            {
                int v = q.Dequeue();
                if (day[v] == K + 1)
                {
                    break;
                }
                else
                {
                        listOfInfectedServices.Add(v);
                        
                }
                foreach (int j in G.OutNeighbors(v))
                {
                    if (visited[j] == 0)
                    {
                        if (day[v] == 1 && (serviceTurnoffDay[v] <= 2))
                        {
                            continue;
                        } else if (day[v] + 2 == serviceTurnoffDay[j] && (day[v] + 1 <= K))
                        {
                            listOfInfectedServices.Add(j);
                            visited[j] = 1;
                        }
                        else if (day[v] + 1 < serviceTurnoffDay[j])
                        {
                            q.Enqueue(j);
                            visited[j] = 1;
                            day[j] = day[v] + 1;
                        } 
                        else 
                        {
                            visited[j] = 1;
                        }
                    }
                }
            }
            // int []visited = new int[G.VertexCount];
            // for (int i = 0; i < G.VertexCount; i++) visited[i] = 0;
            // int []day = new int[G.VertexCount];
            // List<int> listOfInfectedServices = new List<int>();
            // Queue<int> q = new Queue<int>();
            // foreach (int vertex in s)
            // {
            //     q.Enqueue(vertex);
            //     visited[vertex] = 1;
            //     day[vertex] = 1;
            // }
            // for (int i = 2; i <= K +1; i++)
            // {
            //     int length = q.Count;
            //     for (int j = 0; j < length; j++)
            //     {
            //         int proccesing = q.Dequeue();
            //         listOfInfectedServices.Add(proccesing);
            //         if(serviceTurnoffDay[proccesing] <= i){
            //             continue;
            //         }
            //         else
            //         {
            //                 foreach (int k in G.OutNeighbors(proccesing))
            //                 {
            //                     if (visited[k] == 0)
            //                     {
            //                         if (serviceTurnoffDay[k] > i)
            //                         {
            //                             q.Enqueue(k);
            //                             visited[k] = 1;
            //                         }
            //                         else
            //                         {
            //                             visited[k] = 1;
            //                         }
            //                     }
            //                 }
            //             }
            //     }
            // }
            return (listOfInfectedServices.Count, listOfInfectedServices.ToArray());
        }

        /// <summary>
        /// Etap 3 - Wyznaczenie liczby oraz listy zainfekowanych serwisów z możliwością ponownego włączenia wyłączonych serwisów.
        /// Algorytm analizuje propagację infekcji uwzględniając serwisy, które mogą być ponownie uruchamiane po określonym czasie.
        /// </summary>
        /// <param name="G">Graf reprezentujący infrastrukturę serwisów.</param>
        /// <param name="K">Liczba dni propagacji infekcji.</param>
        /// <param name="s">Tablica początkowo zainfekowanych serwisów.</param>
        /// <param name="serviceTurnoffDay">Tablica zawierająca dzień, w którym dany serwis został wyłączony (K + 1 oznacza brak wyłączenia).</param>
        /// <param name="serviceTurnonDay">Tablica zawierająca dzień, w którym dany serwis został ponownie włączony.</param>
        /// <returns>
        /// (int numberOfInfectedServices, int[] listOfInfectedServices) - 
        /// numberOfInfectedServices: liczba zainfekowanych serwisów,
        /// listOfInfectedServices: tablica zawierająca numery zainfekowanych serwisów w kolejności rosnącej.
        /// </returns>
        public (int numberOfInfectedServices, int[] listOfInfectedServices) Stage3(Graph G, int K, int[] s, int[] serviceTurnoffDay, int[] serviceTurnonDay)
        {
            int []visited = new int[G.VertexCount];
            int []visitedsecond = new int[G.VertexCount];
            int []infected = new int[G.VertexCount];
            
            Queue<int>[] next_days = new Queue<int>[K];
            for (int i = 0; i < K; i++)
            {
                next_days[i] = new Queue<int>();
            }
            for (int i = 0; i < G.VertexCount; i++)
            {
                visited[i] = 0;
                visitedsecond[i] = 0;
            }

            foreach (int k in s)
            {
                next_days[0].Enqueue(k);
                visited[k] = 1;
            }
            for (int i = 2; i <= K + 1; i++)
            {
                int length = next_days[i-2].Count;
                for (int j = 0; j < length; j++)
                {
                    int proccesing = next_days[i-2].Dequeue();
                    if (visitedsecond[proccesing] == 0)
                    {
                        infected[proccesing] = 1;
                        visitedsecond[proccesing] = 1;
                    }
                    if (serviceTurnoffDay[proccesing] == serviceTurnonDay[proccesing] &&
                        serviceTurnoffDay[proccesing] == i - 1)
                    {
                        if (i - 1 < K)
                        {
                            next_days[i - 1].Enqueue(proccesing);
                            if (i == 2)
                            {
                                foreach (int k in G.OutNeighbors(proccesing))
                                {
                                    if (visited[k] == 0)
                                    {
                                        if (serviceTurnoffDay[k] > i || serviceTurnonDay[k] <= i )
                                        {
                                            if (i <= K)
                                            {
                                                next_days[i - 1].Enqueue(k);
                                            }

                                            visited[k] = 1;
                                        }
                                    }
                                }
                            }
                            continue;
                        }
                    }
                    if(serviceTurnoffDay[proccesing] <= i && !(serviceTurnoffDay[proccesing] == serviceTurnonDay[proccesing] && serviceTurnoffDay[proccesing] == i- 1)
                                                          && serviceTurnonDay[proccesing] >= i - 1){
                        if (serviceTurnonDay[proccesing] < K)
                        {
                            next_days[serviceTurnonDay[proccesing]].Enqueue(proccesing);
                        }
                    }
                    else
                    {
                            
                            foreach (int k in G.OutNeighbors(proccesing))
                            {
                                if (visited[k] == 0)
                                {
                                    if (serviceTurnoffDay[k] > i || serviceTurnonDay[k] <= i)
                                    {
                                        if (i <= K)
                                        {
                                            next_days[i - 1].Enqueue(k);
                                        }

                                        visited[k] = 1;
                                    }
                                    else
                                    {
                                        if (serviceTurnonDay[proccesing] <= serviceTurnonDay[k] || serviceTurnoffDay[proccesing] > serviceTurnonDay[k])
                                        {
                                            if (serviceTurnonDay[k] < K)
                                            {
                                                next_days[serviceTurnonDay[k]].Enqueue(k);
                                            }
                                        }
                                    }
                                    // else
                                    // {
                                    //     visited[k] = 1;
                                    // }
                                }
                            }
                        }
                }
            }
            List<int> InfectedServices = new List<int>();
            for (int i = 0; i < G.VertexCount; i++)
            {
                if (infected[i] == 1)
                {
                    InfectedServices.Add(i);
                }
            }
            
            
        return (InfectedServices.Count, InfectedServices.ToArray());
        }
 
    }
}
