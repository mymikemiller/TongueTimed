using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TTLib
{
    public class SayablePair
    {
        public Sayable SourceSayable { get; private set; }
        public Sayable TargetSayable { get; private set; }

        public SayablePair(Sayable sourceSayable, Sayable targetSayable)
        {
            SourceSayable = sourceSayable;
            TargetSayable = targetSayable;
        }
    }
}
