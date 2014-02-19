using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GIRLib
{
    class Recallable
    {
        // Off = Not to be recalled
        // On = Can be recalled
        // Required = The newest word to be learned
        public enum States { Off, On, Require }

        // This letter should be returned for recall when State is Off and TicksInCurrentState == ReturnAtTick.
        public int ReturnAtTick
        {
            get
            {
                if (AppearanceCount == 0)
                {
                    return -1;
                } 
                else
                {
                    return (AppearanceCount) * AppearanceCount;
                }
            }
        }

        public int Index { get; private set; }
        public int AppearanceCount { get; set; }

        // For this purpose, On and Required are considered one state
        public int TicksInCurrentState { get; set; }
        public States State { get; set; }

        // The order the indices should be recalled, lowest first
        public int Order { get; private set; }

        public Recallable(int index, int order)
        {
            Index = index;
            Order = order;
            AppearanceCount = 0;
            State = States.Off;
        }

        public override string ToString()
        {
            return Index.ToString();
        }
    }
}
