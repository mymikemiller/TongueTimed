using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GIRLib
{
    class RecallableBag<T>
    {
        //private static List<object> sInput = "eatslridnocpumhgyfbvwkxqjz".ToList<object>();

        public List<Recallable<T>> Recallables { get; private set; }

        public RecallableBag(List<T> objects)
        {
            Recallables = new List<Recallable<T>>();
            for (int i = 0; i < objects.Count; i++)
            {
                Recallables.Add(new Recallable<T>(objects[i], i));
            }
        }

        public Recallable<T> GetNewRecallable()
        {
            // Return the Recallable with the lowest Order with an Appearance of 0. If no Recallables have an appearance of 0, return null.
            Recallable<T> lowest = null;
            foreach (Recallable<T> recallable in Recallables)
            {
                if (recallable.AppearanceCount == 0 && (lowest == null || recallable.Order < lowest.Order))
                {
                    lowest = recallable;
                }
            }
            return lowest;
        }

        // Tick the clock once, making the next Recallable available or resurfacing an old Recallable for recall
        public void Tick(int numberAllowedInUse, int minimumAppearances)
        {
            // Increase each Recallable's tick count and set each Recallable to non-required
            foreach (Recallable<T> recallable in Recallables)
            {
                recallable.TicksInCurrentState++;
                recallable.State = recallable.State == Recallable<T>.States.Require ? Recallable<T>.States.On : recallable.State;
            }

            int numRecallablesInUse = InUse.Count;

            if (numRecallablesInUse < numberAllowedInUse)
            {
                // Simply add a new Recallable if we haven't yet reached our number of Recallables allowed
                Recallable<T> newRecallable = GetNewRecallable();
                newRecallable.State = Recallable<T>.States.Require;
                newRecallable.AppearanceCount++;
            }
            else
            {
                // Otherwise, replace an existing Recallable with a new one or one for recall

                // Remove the Recallable with the largest TicksInCurrentState
                Recallable<T> RecallableToRemove = InUse[0];
                foreach (Recallable<T> recallable in InUse)
                {
                    if (recallable.TicksInCurrentState > RecallableToRemove.TicksInCurrentState)
                    {
                        RecallableToRemove = recallable;
                    }
                }
                RecallableToRemove.State = Recallable<T>.States.Off;
                RecallableToRemove.TicksInCurrentState = 0;

                // The Recallable we'll replace it with is either a new one (Off and AppearanceCount = 0) or one that's off but has been used (off and AppearanceCount > 0)

                // If there are no new Recallables available, make sure we find one below by incrementing TicksInCurrentState for all Recallables until at least one is equal to or less than Recallable's TicksInCurrentState

                if (GetNewRecallable() == null)
                {
                    while(!Recallables.Exists(r => r.TicksInCurrentState >= r.ReturnAtTick))
                    {
                        foreach (Recallable<T> recallable in Recallables)
                        {
                            recallable.TicksInCurrentState++;
                        }
                    }
                }

                // Recall a Recallable that's been unseen for too many ticks (based on its number of appearances).
                Recallable<T> mostTicksPastReturnRecallable = null;
                foreach (Recallable<T> recallable in Recallables)
                {
                    if (recallable.State == Recallable<T>.States.Off && recallable.AppearanceCount > 0 && recallable.TicksInCurrentState >= recallable.ReturnAtTick)
                    {
                        if (mostTicksPastReturnRecallable == null)
                        {
                            mostTicksPastReturnRecallable = recallable;
                        }
                        else
                        {
                            int mostTicksRecallable = mostTicksPastReturnRecallable.TicksInCurrentState - mostTicksPastReturnRecallable.ReturnAtTick;
                            int thisRecallable = recallable.TicksInCurrentState - recallable.ReturnAtTick;

                            if (thisRecallable > mostTicksRecallable)
                            {
                                mostTicksPastReturnRecallable = recallable;
                            }
                        }
                    }
                }

                // Only get a completely new Recallable if there are no Recallables needing recall
                Recallable<T> nextRecallable = mostTicksPastReturnRecallable != null ? mostTicksPastReturnRecallable : GetNewRecallable();

                nextRecallable.State = Recallable<T>.States.Require;
                nextRecallable.AppearanceCount++;
            }
        }

        public List<Recallable<T>> InUse
        {
            get
            {
                List<Recallable<T>> recallablesThatCanBeUsed = new List<Recallable<T>>();

                foreach (Recallable<T> Recallable in Recallables)
                {
                    if (Recallable.State != Recallable<T>.States.Off)
                    {
                        recallablesThatCanBeUsed.Add(Recallable);
                    }
                }

                return recallablesThatCanBeUsed;
            }
        }
    }
}
