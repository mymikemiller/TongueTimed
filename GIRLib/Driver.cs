using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GIRLib
{
    class Driver
    {
        RecallableBag mBag;

        public static int NumberAllowedInUse { get { return 1; } }
        public static int MinimumAppearances { get { return 10; } } // The minimum number of appearances each Recallable must make

        public Driver(int numIndices)
        {
            mBag = new RecallableBag(numIndices);
        }

        // Returns recallables not InUse in the order they can be used if extra recallables are needed.
        public List<Recallable> GetAdditionalUsableLetters(bool includeVowels)
        {
            List<Recallable> recallables = new List<Recallable>();

            foreach (Recallable recallable in mBag.Recallables)
            {
                if (recallable.State == Recallable.States.Off && recallable.AppearanceCount > 0)
                {
                    recallables.Add(recallable);
                }
            }

            return recallables.OrderBy(l => l.TicksInCurrentState).ToList();
        }

        public int GetNextRequiredIndex()
        {
            List<Recallable> recallables = GetNext();
            if (recallables.Count == 0)
                return -1;

            return recallables.First(o => o.State == Recallable.States.Require).Index;
        }
        

        public List<Recallable> GetNext()
        {
            bool done = true;

            // We're done (return an empty list) when there are no letters that haven't been used
            foreach (Recallable recallable in mBag.Recallables)
            {
                if (recallable.AppearanceCount < MinimumAppearances)
                {
                    done = false;
                }
            }

            if (done)
            {
                return new List<Recallable>();
            }

            mBag.Tick(NumberAllowedInUse, MinimumAppearances);
            return mBag.InUse;
        }

    }
}
