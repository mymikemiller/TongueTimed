using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TTLib
{
    public class Sayable
    {
        public string Language { get; private set; }
        public string Line { get; private set; }

        public Sayable(string language, string line)
        {
            Language = language;
            Line = line;
        }
    }
}
