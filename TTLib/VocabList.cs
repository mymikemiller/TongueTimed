using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TTLib
{
    public class VocabList : IList<Vocab>
    {
        private List<Vocab> mVocabList;

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

        public VocabList(string fileName, int startIndex = 0, int count = -1)
        {
            using (StreamReader file = new StreamReader(fileName)) {
                Init(file, startIndex, count);
            }
        }

        public VocabList(StreamReader file, int startIndex = 0, int count = -1)
        {
            Init(file, startIndex, count);
        }

        public void Init(StreamReader file, int startIndex = 0, int count = -1)
        {
            mVocabList = new List<Vocab>();

            string line;
            int lineNum = 0;
            while ((line = file.ReadLine()) != null)
            {
                if (count != -1 && lineNum >= count) break;

                if (lineNum >= startIndex)
                {
                    string[] parts = line.Split(new string[] { " = " }, StringSplitOptions.RemoveEmptyEntries);
                    mVocabList.Add(new Word(parts[1], parts[0]));
                }

                lineNum++;
            }
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

        public int IndexOf(Vocab item)
        {
            return mVocabList.IndexOf(item);
        }

        public void Insert(int index, Vocab item)
        {
            mVocabList.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            mVocabList.RemoveAt(index);
        }

        public Vocab this[int index]
        {
            get
            {
                return mVocabList[index];
            }
            set
            {
                mVocabList[index] = value;
            }
        }


        public void Clear()
        {
            mVocabList.Clear();
        }

        public bool Contains(Vocab item)
        {
            return mVocabList.Contains(item);
        }

        public void CopyTo(Vocab[] array, int arrayIndex)
        {
            mVocabList.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return mVocabList.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(Vocab item)
        {
            return mVocabList.Remove(item);
        }

        public void Add(Vocab item)
        {
            mVocabList.Add(item);
        }
    }
}
