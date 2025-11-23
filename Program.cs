using System;
using System.IO;

namespace TokenCreator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("-----Text tokenization-----");

            string changedTextPath = "C:\\VScode_projects\\Lab3\\text.txt";
            string inputPath = "C:\\VScode_projects\\Lab3\\original_text.txt";
            string ruPath = "C:\\VScode_projects\\Lab3\\stopwords_ru.txt";
            string enPath = "C:\\VScode_projects\\Lab3\\stopwords_en.txt";
            string xmlPath = "C:\\VScode_projects\\Lab3\\text.xml";

            string inputText = string.Empty;

            if (File.Exists(inputPath))
                try
                {
                    inputText = File.ReadAllText(inputPath);
                    Console.WriteLine($"\nText was successfully loaded from file: {inputPath}");
                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine($"\nError: File not found at path {inputPath}");
                    return;
                }
                Text.LoadStopWords(ruPath, enPath);


            Text text = Text.Parse(inputText);
   

            Console.WriteLine("\n-----Sorted by amount-----");
            foreach (var s in text.OrderByWordCount())
                Console.WriteLine(s);

            Console.WriteLine("\n-----3 symbol words in ? senteces was found-----");
            foreach (var w in text.FindWordsInQuestions(3))
                Console.WriteLine(w);

            text.RemoveWordsByLengthAndConsonant(5);
            File.WriteAllText(changedTextPath, text.ToString());
            Console.WriteLine("\n-----Words removed-----");

            text.ReplaceWordsInSentence(0, 4, "!!!Replaced!!!");
            File.WriteAllText(inputPath, text.ToString());
            Console.WriteLine("\n-----Word replaced-----");


            text.RemoveStopWords();
            File.WriteAllText(changedTextPath, text.ToString());
            Console.WriteLine("\n-----Stop words removed-----");
            

            //XML
            text.SaveAsXml(xmlPath);
            Console.WriteLine($"\nXML saved to: {xmlPath}");

            
            Text loaded = Text.LoadFromXml(xmlPath);
            Console.WriteLine("\n-----From XML to TXT-----");
            Console.WriteLine(loaded);
        
        }
    }
}