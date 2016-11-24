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
        const string punctuation = "([\\s,.;:!?()\\-+])";
        //With no ()
        const string punctuation2 = "[\\s,.;:!?()\\-]+";
        static void Main(string[] args)
        {
            string[] books = ReadFile("Knyga.txt");
            WriteAnalysis("Analysis.txt", books);
            WriteFormattedBook("Rez.txt", books);
        }
        public static string[] ReadFile(string fName)
        {
            string[] books = File.ReadAllLines(@fName, Encoding.UTF8);
            return books;
        }

        public static bool EndMatchesBeginning(string one, string two)
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

        public static string LongestEndMatchesBeginnings(string[] books, string wordPunctuation)
        {
            List<string> fragments = new List<string>();
            List<int> actualWordLines = new List<int>();
            List<int> actualWords = new List<int>();

            string allBooks = string.Join("\n", books);
            string[] parts = Regex.Split(allBooks, wordPunctuation);
           
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
            //foreach (int str in actualWords)
               //Console.WriteLine(parts[str]);
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
                bool endBeginningMatch = EndMatchesBeginning(parts[actualWords[j]], parts[actualWords[j + 1]]);
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
                        if (lineCount == 1 && EndMatchesBeginning(parts[actualWords[j]], parts[actualWords[j + 1]]))
                            last = actualWords[j + 1];
                        else
                            last = actualWords[j];
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

        public static int CountNumberWords(string[] books, string wordPunctuation, out double total)
        {
            string allBooks = string.Join("\n", books);
            string[] parts = Regex.Split(allBooks, wordPunctuation);
            //all tryParse methods require results
            int result = 0;
            double doubleResult = 0;
            total = 0;
            int count = 0;
            foreach (string word in parts)
            {
                if (int.TryParse(word, out result))
                {
                    total += result;
                    count++;
                }
                else
                if (double.TryParse(word, out doubleResult))
                {
                    total += doubleResult;
                    count++;
                }
            }
            return count;
        }
        
        public static void WriteFormattedBook(string fName, string[] books)
        {
            int[] wordFormat = MaxWordSizes(books);
            using (StreamWriter writer = new StreamWriter(fName))
            {
                for (int i = 0; i < books.Length; i++)
                {
                    string[] parts = Regex.Split(books[i], " ");
                    for (int j = 0; j < parts.Length; j++)
                    {
                        if (parts[j].Length > wordFormat[j] +1)
                            parts[j] = parts[j].Remove(wordFormat[j] + 1, parts[j].Length - (wordFormat[j] +1) );
                        writer.Write(parts[j].PadRight(wordFormat[j] + 2, ' '));
                    }
                    writer.WriteLine();
                }
            }
        }

        public static void WriteAnalysis(string fName, string[] books)
        {
            using (StreamWriter writer = new StreamWriter(@fName))
            {
                writer.WriteLine("Longest fragment in which one word ending matches nexts words beginning (lines = first char):\r\n");
                writer.WriteLine(LongestEndMatchesBeginnings(books, punctuation).Replace("\n","\r\n"));
                double total = 0;
                writer.WriteLine();
                writer.WriteLine("Number of words that are numbers: ");
                writer.WriteLine(CountNumberWords(books, punctuation, out total));
                writer.WriteLine();
                writer.WriteLine("Their total:");
                writer.WriteLine(total);
            }
        }

        public static int MaxLineWords(string[] books)
        {
            int maxLines = 0;
            for (int i = 0; i < books.Length; i++)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                string[] parts = Regex.Split(books[i], punctuation2);
                parts = parts.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                foreach (string part in parts)
                    Console.WriteLine(part);
                if (parts.Length > maxLines)
                    maxLines = parts.Length;
                Console.WriteLine();
            }
            return maxLines;
        }

        public static int[] MaxWordSizes(string[] books)
        {
            int maxLines = MaxLineWords(books);
            int[] lines = new int[maxLines];
            for (int i = 0; i < books.Length; i++)
            {
                string[] parts = Regex.Split(books[i], punctuation2);
                parts = parts.Where(x => !string.IsNullOrEmpty(x)).ToArray();

                for (int j = 0; j < parts.Length; j++)
                    if (parts[j].Length > lines[j])
                        lines[j] = parts[j].Length;
            }
            return lines;
        }
    }
}
/*
* Ilgiausią teksto fragmentą, sudarytą iš žodžių, kur žodžio paskutinė raidė sutampa su kito žodžio
pirmąja raide(tarp didžiųjų ir mažųjų raidžių skirtumo nedaryti) ir juos skiriančių skyriklių, bei jo
eilutės numerius;
*/
