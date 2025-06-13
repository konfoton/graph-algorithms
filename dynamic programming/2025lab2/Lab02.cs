    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;


    // Programowanie dynamiczne Autor: Konrad Burdach
    using ASD;
    namespace ASD
    {
        public class Lab02 : MarshalByRefObject
        {
            /// <summary>
            /// Etap 1 - Wyznaczenie ścieżki (seam) o minimalnym sumarycznym score.
            /// Ścieżka przebiega od górnego do dolnego wiersza obrazu.
            /// </summary>
            /// <param name="S">macierz score o wymiarach H x W, gdzie S[i, j] reprezentuje "ważność" piksela w wierszu i i kolumnie j</param>
            /// <returns>
            /// (int cost, (int, int)[] seam) - 
            /// cost: minimalny łączny koszt ścieżki (suma wartości pikseli);
            /// seam: tablica pozycji pikseli (włącznie z pikselem z pierwszego i ostatniego wiersza) tworzących ścieżkę.
            /// </returns>
            public (int cost, (int i, int j)[] seam) Stage1(int[,] S)
            {
                int H = S.GetLength(0);
                int W = S.GetLength(1);
                
                
                int[,] table = new int[H, W];
                int[,] table2 = new int[H, W];
                

                (int i, int j)[] seam1 = new (int i, int j)[H];
                
                for (int i = 0; i < W; i++)
                {
                    table[0, i] = S[0, i];
                }

                for (int i = 1; i < H; i++)
                {
                    for (int j = 0; j < W; j++)
                    {
                        int min = int.MaxValue;
                        int move = -2;
                        if (i - 1 >= 0)
                        {
                            if (table[i - 1, j] + S[i, j] < min)
                            {
                                min = table[i - 1, j] + S[i, j];
                                move = 0;
                            }
                        }

                        if (i - 1 >= 0 && j - 1 >= 0)
                        {
                            if (table[i - 1, j - 1] + S[i, j] < min)
                            {
                                min = table[i - 1, j - 1] + S[i, j];
                                move = -1;
                            }
                        }

                        if (i - 1 >= 0 && j + 1 < W)
                        {
                            if (table[i - 1, j + 1] + S[i, j] < min)
                            {
                                min = table[i - 1, j + 1] + S[i, j];
                                move = 1;
                            }
                        }
                        
                        table[i, j] = min;
                        table2[i, j] = move;
                            
                            
                    }
                }
                int result = int.MaxValue;
                int index = -1;
                for (int i = 0; i < W; i++)
                {
                    if (table[H - 1, i] < result)
                    {
                        result = table[H - 1, i];
                        index = i;
                    }
                }
                
                int a = H - 1;
                int b = index;
                while (a != 0)
                {
                    seam1[a] = (a, b);
                    if (table2[a, b] == -1)
                    {
                        a--;
                        b--;
                    } else if (table2[a, b] == 0)
                    {
                        a--;
                    }
                    else if (table2[a, b] == 1)
                    {
                        a--;
                        b++;
                    }
                }
                seam1[a] = (a, b); 
                
                return (result, seam1);
            }

            /// <summary>
            /// Etap 2 - Wyznaczenie ścieżki (seam) o minimalnym sumarycznym score z uwzględnieniem kary za zmianę kierunku.
            /// Przy każdym przejściu, gdy kierunek ruchu różni się od poprzedniego, do łącznego kosztu dodawana jest kara K.
            /// Pierwszy krok (z pierwszego wiersza) nie podlega karze.
            /// </summary>
            /// <param name="S">macierz score o wymiarach H x W</param>
            /// <param name="K">kara za zmianę kierunku (K >= 1)</param>
            /// <returns>
            /// (int cost, (int, int)[] seam) - 
            /// cost: minimalny łączny koszt ścieżki (suma wartości pikseli oraz naliczonych kar);
            /// seam: tablica pozycji pikseli tworzących ścieżkę.
            /// </returns>
            public (int cost, (int i, int j)[] seam) Stage2(int[,] S, int K)
            {
                int H = S.GetLength(0);
                int W = S.GetLength(1);
                int[,,] table = new int[H, W, 3];
                (int i, int j)[] seam1 = new (int i, int j)[H];
                for (int i = 0; i < W; i++)
                {
                    table[0, i, 0] = S[0, i];
                    table[0, i, 1] = S[0, i];
                    table[0, i, 2] = S[0, i];
                }

                for (int i = 1; i < H; i++)
                {
                    for (int j = 0; j < W; j++)
                    {
                        table[i, j, 1] = int.MaxValue/2;
                        if (i - 1 >= 0)
                        {
                            if (table[i - 1, j, 0] + S[i, j]  + K< table[i, j, 1])
                            {
                                table[i, j, 1] = table[i - 1, j, 0] + S[i, j] + K;
                                
                            }
                            if (table[i - 1, j, 1] + S[i, j] < table[i, j, 1])
                            {
                                table[i, j, 1] = table[i - 1, j, 1] + S[i, j];
                                
                            }
                            if (table[i - 1, j, 2] + S[i, j]  + K < table[i, j, 1])
                            {
                                table[i, j, 1] = table[i - 1, j, 2] + S[i, j] + K;
                                
                            }
                        }
                        table[i, j, 0] = int.MaxValue/2;
                        if (i - 1 >= 0 && j - 1 >= 0)
                        {
                            if (table[i - 1, j - 1, 0] + S[i, j] < table[i, j , 0])
                            {
                                table[i, j, 0] = table[i - 1, j - 1, 0] + S[i, j];
                            }
                            if (table[i - 1, j - 1, 1] + S[i, j] + K< table[i, j , 0])
                            {
                                table[i, j, 0] = table[i - 1, j - 1, 1] + S[i, j] + K;
                            }
                            if (table[i - 1, j - 1, 2] + S[i, j] + K< table[i, j , 0])
                            {
                                table[i, j, 0] = table[i - 1, j - 1, 2] + S[i, j] + K;
                            }
                        }
                        table[i, j, 2] = int.MaxValue/2;
                        if (i - 1 >= 0 && j + 1 < W)
                        {
                            if (table[i - 1, j + 1, 2] + S[i, j] < table[i, j, 2])
                            {
                                table[i, j, 2] = table[i - 1, j + 1, 2] + S[i, j];
                            }
                            if (table[i - 1, j + 1, 1] + S[i, j] + K< table[i, j, 2])
                            {
                                table[i, j, 2] = table[i - 1, j + 1, 1] + S[i, j] + K;
                            }
                            if (table[i - 1, j + 1, 0] + S[i, j] + K< table[i, j, 2])
                            {
                                table[i, j, 2] = table[i - 1, j + 1, 0] + S[i, j] + K;
                            }
                        }

                    }
                }

                int min_cost = int.MaxValue;
                int index = -1;
                int direction = -1;
                for (int i = 0; i < W; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (table[H - 1, i, j] < min_cost)
                        {
                            min_cost = table[H - 1, i, j];
                            index = i;
                            direction = j;
                        }
                    }
                }

                int min_cost_f = min_cost;
                int a = H - 1;
                int b = index;
                seam1[a] = (a, b);
                while (a != 0)
                {
                    min_cost -= S[a, b];
                    if (direction == 0)
                    {
                        a--;
                        b--;
                    } else if (direction == 1)
                    {
                        a--;
                    } else if (direction == 2)
                    {
                        a--;
                        b++;
                    }
                    seam1[a] = (a, b);
                    if (table[a, b, direction] == min_cost)
                    {
                        min_cost = table[a, b, direction];
                    }

                    for (int i = 0; i < 3; i++)
                    {
                        if (i == direction)
                        {
                            continue;
                        }
                        if (table[a, b, i] == min_cost - K)
                        {
                            direction = i;
                            min_cost = table[a, b, direction];
                        }
                    }
                    
                }
                return (min_cost_f, seam1);
            }
        }
    }