using GIRLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TTLib
{
    public class Teacher
    {
        private int mCurrentGirIndicesIndex = 0;
        List<int> mGirIndices;
        private VocabList mVocabList;

        public Teacher(string fileName, int startIndex = 0, int count = -1)
        {
            using (StreamReader file = new StreamReader(fileName)) {
                Init(file, startIndex, count);
            }
        }

        public Teacher(StreamReader file)
        {
            Init(file);
        }

        public void Init(StreamReader file, int startIndex = 0, int count = -1)
        {
            mVocabList = new VocabList(file, startIndex, count);

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

            Console.WriteLine("Starting GIR");

            DateTime start = DateTime.Now;
            mGirIndices = GIR.GetGIR(mVocabList.GetVocabList().Count);
            DateTime end = DateTime.Now;

            Console.WriteLine("Finished GIR in " + (end - start).ToString());
        }

        public SayablePair GetNextSayablePair()
        {
            if (mCurrentGirIndicesIndex >= mGirIndices.Count) return null;

            int index = mGirIndices[mCurrentGirIndicesIndex];

            if (index >= mVocabList.GetVocabList().Count) return null;

            Vocab vocab = mVocabList[index];

            Sayable sourceSayable = new Sayable("en", vocab.Source);
            Sayable targetSayable = new Sayable("de", vocab.Target);

            SayablePair sayablePair = new SayablePair(sourceSayable, targetSayable);

            mCurrentGirIndicesIndex++;
            return sayablePair;
        }

    }
}
