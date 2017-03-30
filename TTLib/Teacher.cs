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
        public int CurrentGirIndicesIndex { get; set; }
        List<int> mGirIndices;
        private VocabList mVocabList;

        public Teacher(string vocab)
        {
            String[] vocabArray = vocab.Split('\n');
            Init(vocabArray);
            CurrentGirIndicesIndex = 0;
        }

        public Teacher(StreamReader file)
        {
            Init(file);
            CurrentGirIndicesIndex = 0;
        }

        public void Init(String[] vocabArray)
        {
            mVocabList = new VocabList();

            foreach(String line in vocabArray)
            {
                string[] parts = line.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2)
                {
                    String source = parts[0].Trim();
                    String target = parts[1].Trim();
                    mVocabList.Add(new Word(source, target));
                }
            }

            PerformGIR();
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

            PerformGIR();
        }

        public void Reset()
        {
            CurrentGirIndicesIndex = 0;
        }

        private void PerformGIR()
        {
            Console.WriteLine("Starting GIR");

            DateTime start = DateTime.Now;
            mGirIndices = GIR.GetGIR(mVocabList.GetVocabList().Count);
            DateTime end = DateTime.Now;

            Console.WriteLine("Finished GIR in " + (end - start).ToString());
        }

        public String GetVocab()
        {
            String allVocab = "";
            foreach(Vocab vocab in mVocabList)
            {
                allVocab += vocab.Source + " = " + vocab.Target + "\n";
            }
            return allVocab;
        }

        public SayablePair GetNextSayablePair()
        {
            if (CurrentGirIndicesIndex >= mGirIndices.Count) return null;

            int index = mGirIndices[CurrentGirIndicesIndex];

            if (index >= mVocabList.GetVocabList().Count) return null;

            Vocab vocab = mVocabList[index];

            Sayable sourceSayable = new Sayable("en", vocab.Source);
            Sayable targetSayable = new Sayable("de", vocab.Target);

            SayablePair sayablePair = new SayablePair(sourceSayable, targetSayable);

            CurrentGirIndicesIndex++;
            return sayablePair;
        }

    }
}
