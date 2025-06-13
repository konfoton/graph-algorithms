using System;
using System.Collections.Generic;

namespace ASD
{
    /// <summary>
    /// Klasa drzewa prefiksowego z możliwością wyszukiwania słów w zadanej odległości edycyjnej
    /// </summary>
    public class Lab14_Trie : System.MarshalByRefObject
    {
        
        // klasy TrieNode NIE WOLNO ZMIENIAĆ!
        private class TrieNode
        {
            public SortedDictionary<char, TrieNode> childs = new SortedDictionary<char, TrieNode>();
            public bool IsWord = false;
            public int WordCount = 0;
        }

        private TrieNode root;

        public Lab14_Trie()
        {
            root = new TrieNode();
        }

        /// <summary>
        /// Zwraca liczbę przechowywanych słów
        /// Ma działać w czasie stałym - O(1)
        /// </summary>
        public int Count { get { return -1; } }

        /// <summary>
        /// Zwraca liczbę przechowywanych słów o zadanym prefiksie
        /// Ma działać w czasie O(len(startWith))
        /// </summary>
        /// <param name="startWith">Prefiks słów do zliczenia</param>
        /// <returns>Liczba słów o zadanym prefiksie</returns>
        public int CountPrefix(string startWith)
        {
            TrieNode current = root;
            foreach (char c in startWith)
            {
                if (!current.childs.ContainsKey(c))
                    return 0;
                current = current.childs[c];
            }
            return current.WordCount;
        }

        /// <summary>
        /// Dodaje słowo do słownika
        /// Ma działać w czasie O(len(newWord))
        /// </summary>
        /// <param name="newWord">Słowo do dodania</param>
        /// <returns>True jeśli słowo udało się dodać, false jeśli słowo już istniało</returns>
        public bool AddWord(string newWord)
        {
        
            TrieNode current = root;
            foreach (char c in newWord)
            {
                if (!current.childs.ContainsKey(c))
                    current.childs[c] = new TrieNode();
                current = current.childs[c];
            }

            if (current.IsWord)
            {
                return false;
            }
            else
            {
                current.IsWord = true;

                current = root;
                root.WordCount++;
                foreach (char c in newWord)
                {
                    current = current.childs[c];
                    current.WordCount++;
                }
                return true;
            }
            
        }

        /// <summary>
        /// Sprawdza czy podane słowo jest przechowywane w słowniku
        /// Ma działać w czasie O(len(word))
        /// </summary>
        /// <param name="word">Słowo do sprawdzenia</param>
        /// <returns>True jeśli słowo znajduje się w słowniku, wpp. false</returns>
        public bool Contains(string word)
        {
            TrieNode current = root;
            foreach (char c in word)
            {
                if (!current.childs.ContainsKey(c))
                    return false;
                current = current.childs[c];
            }
            if (current.IsWord)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Usuwa podane słowo ze słownika
        /// Ma działać w czasie O(len(word))
        /// </summary>
        /// <param name="word">Słowo do usunięcia</param>
        /// <returns>True jeśli udało się słowo usunąć, false jeśli słowa nie było w słowniku</returns>
        public bool Remove(string word)
        {
            if (string.IsNullOrEmpty(word))
                return false;
            
            if (!Contains(word))
                return false;
            
            TrieNode current = root;
            root.WordCount--;
            foreach (char c in word)
            {
                current = current.childs[c];
                current.WordCount--;
            }
            
            current.IsWord = false;
    
            return true;
        }

        /// <summary>
        /// Zwraca wszystkie słowa o podanym prefiksie. 
        /// Dla pustego prefiksu zwraca wszystkie słowa ze słownika.
        /// Wynik jest w porządku alfabetycznym.
        /// Ma działać w czasie O(liczba węzłów w drzewie)
        /// </summary>
        /// <param name="startWith">Prefiks</param>
        /// <returns>Wyliczenie zawierające wszystkie słowa ze słownika o podanym prefiksie</returns>
        ///



        
        public List<string> AllWords(string startWith = "")
        {
            
            List<string> result = new List<string>();
            TrieNode current = root;
            foreach (char c in startWith)
            {
                if (!current.childs.ContainsKey(c))
                    return result;
                current = current.childs[c];
            }
            
            void recursive(TrieNode root, string startWithh)
            {
                if (root.IsWord)
                {
                    result.Add(startWithh);
                }
                foreach (char c in root.childs.Keys)
                {
                    recursive(root.childs[c], startWithh + c);
                }
            }
            recursive(current, startWith);
            
            
            return result;
        }

        /// <summary>
        /// Wyszukuje w słowniku wszystkie słowa w podanej odległości edycyjnej od zadanego słowa
        /// Wynik jest w porządku alfabetycznym ze względu na słowa (a nie na odległość).
        /// Ma działać optymalnie - tj. niedozwolone jest wyszukanie wszystkich słów i sprawdzenie ich odległości
        /// Należy przeszukując drzewo odpowiednio odrzucać niektóre z gałęzi.
        /// Złożoność pesymistyczna (gdy wszystkie słowa w słowniku mieszczą się w zadanej odległości)
        /// O(len(word) * (liczba węzłów w drzewie))
        /// </summary>
        /// <param name="word">Słowo</param>
        /// <param name="distance">Odległość edycyjna</param>
        /// <returns>Lista zawierająca pary (słowo, odległość) spełniające warunek odległości edycyjnej</returns>
        ///
        ///
        
        
        public List<(string, int)> Search(string word, int distance = 1)
        {
            List<(string, int)> result = new List<(string, int)>();
            
            void DfsWithLevenshtein(TrieNode node, string currentWord, int[] prevRow)
            {
                if (node.IsWord)
                {
                    int editDistance = prevRow[word.Length];
                    if (editDistance <= distance)
                    {
                        result.Add((currentWord, editDistance));
                    }
                }

                int min = int.MaxValue;
                foreach (int element in prevRow)
                {
                    if (element < min) min = element;
                }
                if (min > distance)
                    return;
                
                foreach (var pair in node.childs)
                {
                    char letter = pair.Key;
                    TrieNode nextNode = pair.Value;
                    
                    int[] currentRow = new int[word.Length + 1];
                    currentRow[0] = prevRow[0] + 1;
                    
                    for (int j = 1; j <= word.Length; j++)
                    {
                        int cost = (letter == word[j-1]) ? 0 : 1;
                        currentRow[j] = Math.Min(
                            Math.Min(prevRow[j] + 1,              
                                     currentRow[j-1] + 1),        
                            prevRow[j-1] + cost);                 
                    }
                    
                    DfsWithLevenshtein(nextNode, currentWord + letter, currentRow);
                }
            }
            int[] initialRow = new int[word.Length + 1];
            for (int i = 0; i <= word.Length; i++)
                initialRow[i] = i;
            
            DfsWithLevenshtein(root, "", initialRow);
            
            result.Sort((a, b) => string.Compare(a.Item1, b.Item1));
            
            return result;
                }

    }
}