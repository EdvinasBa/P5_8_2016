using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace P5_8_2016
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] books = readFile("Knyga.txt");
            string punctuation = "([\\s,.;:!?()\\-+])";
            Console.WriteLine(longestEndMatchesBeginningV2(books, punctuation));
        }
        public static string[] readFile(string fName)
        {
            string[] books = File.ReadAllLines(@fName, Encoding.UTF8);
            return books;
        }

        public static bool endMatchesBeginning(string one, string two)
        {
            one = one.ToLower();
            two = two.ToLower();
            one = new string(one.Where(c => !char.IsPunctuation(c)).ToArray());
            two = new string(two.Where(c => !char.IsPunctuation(c)).ToArray());
            if (string.IsNullOrWhiteSpace(one) || string.IsNullOrWhiteSpace(two))
                return false;
            //Console.WriteLine("{0} {1}",one,two);
            if (one[one.Length - 1] == two[0])
                return true;
            return false;
        }

        public static string longestEndMatchesBeginningV2(string[] books, string skyrikliai)
        {
            List<string> fragments = new List<string>();
            List<int> actualWordLines = new List<int>();
            List<int> actualWords = new List<int>();

            string allBooks = string.Join("\n", books);
            string[] parts = Regex.Split(allBooks, skyrikliai);
           
            int line = 1;

            for (int j = 0; j < parts.Length; j++)
            {
                if (!string.IsNullOrWhiteSpace(new string(parts[j].Where(c => char.IsLetter(c)).ToArray())))
                {
                    actualWords.Add(j);
                    actualWordLines.Add(line);
                }

                if (parts[j] == "\n")
                    ++line;
            }

            // foreach (int str in actualWords)
            //   Console.WriteLine(parts[str]);

            string temp = "";
            bool inMatching = false;
            bool firstSet = false;
            int first = 0;
            int last = 0;
            int lineCount = line;
            for (int j = 0; j < actualWords.Count - 1; j++)
            {
                // Console.WriteLine(parts[actualWords[j]]);
                // Console.WriteLine(parts[actualWords[j+1]]);
                bool endBeginningMatch = endMatchesBeginning(parts[actualWords[j]], parts[actualWords[j + 1]]);
                if (endBeginningMatch)
                {
                    /*
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Match");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine("{0} | {1}",parts[actualWords[j]], parts[actualWords[j + 1]]);*/
                    if (!firstSet)
                    {
                        firstSet = true;
                        line = actualWordLines[j];
                        first = actualWords[j];
                    }
                    inMatching = true;
                }
                if (!endBeginningMatch || j == actualWords.Count - 2)
                {
                    if (inMatching)
                    {
                        if (lineCount == 1 && endMatchesBeginning(parts[actualWords[j]], parts[actualWords[j + 1]]))
                            last = actualWords[j + 1];
                        else
                            last = actualWords[j];

                        // Console.Clear();
                        // Console.WriteLine("First: {0} \nLast: {1}", first, last);
                        temp += line + " ";
                        for (int k = first; k <= last; k++)
                        {
                            temp += parts[k];
                            if (parts[k] == "\n")
                                temp += ++line + " ";
                        }
                        // Console.WriteLine(temp);
                        fragments.Add(temp);
                        inMatching = false;
                        firstSet = false;
                    }
                    temp = "";
                }
            }

            /*
            Console.WriteLine();
            foreach(string text in fragments)
            {
                Console.WriteLine(text);
                Console.WriteLine();
            }*/

            fragments = fragments.OrderBy(str => str.Length).ToList();
            fragments.Reverse();

            if (fragments.Count > 0)
                return fragments[0];

            return null;
        }
        public static int countNumberWords(string[] books, char[] skyrikliai)
        {
            string allBooks = string.Join(" ", books);
            string[] words = allBooks.Split(skyrikliai, StringSplitOptions.RemoveEmptyEntries);

            //all tryParse methods require results
            int result = 0;
            double doubleResult = 0;

            int count = 0;
            foreach (string word in words)
            {
                if (int.TryParse(word, out result))
                    count++;
                else
                    if (double.TryParse(word, out doubleResult))
                    count++;
            }
            return count;
        }
    }
}
/*
* Ilgiausią teksto fragmentą, sudarytą iš žodžių, kur žodžio paskutinė raidė sutampa su kito žodžio
pirmąja raide(tarp didžiųjų ir mažųjų raidžių skirtumo nedaryti) ir juos skiriančių skyriklių, bei jo
eilutės numerius;
*/
