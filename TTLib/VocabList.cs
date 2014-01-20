using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TTLib
{
    public class VocabList : IEnumerable<Vocab>
    {
        private List<Vocab> mVocabList;
        private string p;

        public IEnumerator<Vocab> GetEnumerator()
        {
            return mVocabList.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return mVocabList.GetEnumerator();
        }

        public List<Vocab> GetVocabList() { return mVocabList; }

        public VocabList()
        {
            mVocabList = new List<Vocab>();
        }

        public VocabList(string fileName)
        {
            mVocabList = new List<Vocab>();

            string line;
            System.IO.StreamReader file = new System.IO.StreamReader(fileName);

            while ((line = file.ReadLine()) != null)
            {
                string[] parts = line.Split(new string[]{" = "}, StringSplitOptions.RemoveEmptyEntries);
                mVocabList.Add(new Word(parts[0], parts[1]));
            }

            file.Close();
        }

        public void Add(Vocab vocab)
        {
            mVocabList.Add(vocab);
        }

        public void WriteToFile(string fileName)
        {
            /*
            string text = string.Empty;
            foreach (Vocab v in mVocabList)
            {
                text += v.Source + " = " + v.Target;
            }

            System.IO.File.WriteAllText(fileName, text);//@"C:\Users\Public\TestFolder\WriteText.txt", text);
            */

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileName))
            {
                foreach (Vocab v in mVocabList)
                {
                    file.WriteLine(v.Source + " = " + v.Target);
                }
            }
        }
    }
}
