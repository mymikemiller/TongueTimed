using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TTLib
{
    public class Translator
    {

        public Translator()
        {
        }

        public VocabList Translate(string text)
        {
            List<string> words = SplitUnique(text);
            return YandexTranslate(words);
        }

        private static List<string> SplitUnique(string text)
        {
            string[] splitSentences = text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var unique = new HashSet<string>();
            foreach (string sentence in splitSentences)
            {
                // Remove punctuation except for single quotes (to allow contractions) and dashes for hyphenated words
                string textSansPunc = sentence.Where(c => !char.IsPunctuation(c) || c == '\'' || c == '-').Aggregate("", (current, c) => current + c);
                string[] splitAllWords = textSansPunc.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
                foreach (string word in splitAllWords)
                {
                    unique.Add(word);
                }
                unique.Add(sentence);
            }

            
            
            return unique.ToList();
        }

        #region Yandex Translate
        

        private static VocabList YandexTranslate(List<string> words)
        {
            VocabList vocabList = new VocabList();

            // Yandex doesn't seem to like translating too many words at once so split into groups of 1000 (I haven't figured out if it's a character limit, word limit, or phrase limit. Results are insonsistent)
            int wordsAllowed = 1000;
            int passes = (-1 + words.Count + wordsAllowed) / wordsAllowed;
            for (int pass = 0; pass < passes; pass++ )
            {
                string allText = string.Empty;

                for (int i = 0; i < wordsAllowed; i++)
                {
                    int wordIndex = i + (wordsAllowed * pass);
                    if (wordIndex >= words.Count)
                    {
                        break;
                    }

                    string word = words[wordIndex];
                    // Don't print the &text= part for the first one of each pass because that's added on the QueryString.Add call
                    if (i > 0)
                    {
                        allText += "&text=";
                    }
                    allText += word;
                }

                WebClient webClient = new WebClient();
                webClient.QueryString.Add("key", "trnsl.1.1.20140114T110901Z.06c89613f8b9bdf9.ec1414fa46f30fe0bcdaa6985c3e7b2660ffd9e7");
                webClient.QueryString.Add("lang", "de-en");
                webClient.QueryString.Add("text", allText);
                webClient.Encoding = Encoding.UTF8;
                string result = webClient.DownloadString("https://translate.yandex.net/api/v1.5/tr/translate");
                List<string> translatedList = YandexResponseToList(result);

                for (int i = 0; i < translatedList.Count; i++)
                {
                    int wordIndex = i + (wordsAllowed * pass);
                    vocabList.Add(new Word(words[wordIndex], translatedList[i]));
                }

            }

            return vocabList;
        }

        private static Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }


        private static List<string> YandexResponseToList(string response)
        {
            List<string> translated = new List<string>();

            using (Stream stream = GenerateStreamFromString(response))
            {
                XDocument document = XDocument.Load(stream);
                foreach (XElement element in document.Descendants("text"))
                {
                    translated.Add(element.Value);
                }
            }

            return translated;
        }

        #endregion
    }
}