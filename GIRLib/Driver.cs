using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GIRLib
{
    class Driver<T>
    {
        RecallableBag<T> mBag;

        public static int NumberAllowedInUse { get { return 1; } }
        public static int MinimumAppearances { get { return 10; } } // The minimum number of appearances each Recallable must make

        public Driver(List<T> objects)
        {
            mBag = new RecallableBag<T>(objects);
        }

        // Returns recallables not InUse in the order they can be used if extra recallables are needed.
        public List<Recallable<T>> GetAdditionalUsableLetters(bool includeVowels)
        {
            List<Recallable<T>> recallables = new List<Recallable<T>>();

            foreach (Recallable<T> recallable in mBag.Recallables)
            {
                if (recallable.State == Recallable<T>.States.Off && recallable.AppearanceCount > 0)
                {
                    recallables.Add(recallable);
                }
            }

            return recallables.OrderBy(l => l.TicksInCurrentState).ToList();
        }

        public T GetNextRequiredObject()
        {
            List<Recallable<T>> recallables = GetNext();
            if (recallables.Count == 0)
                return default(T);

            return recallables.First(o => o.State == Recallable<T>.States.Require).Object;
        }
        

        public List<Recallable<T>> GetNext()
        {
            bool done = true;

            // We're done (return an empty list) when there are no letters that haven't been used
            foreach (Recallable<T> recallable in mBag.Recallables)
            {
                if (recallable.AppearanceCount < MinimumAppearances)
                {
                    done = false;
                }
            }

            if (done)
            {
                return new List<Recallable<T>>();
            }

            mBag.Tick(NumberAllowedInUse, MinimumAppearances);
            return mBag.InUse;
        }

    }
}
