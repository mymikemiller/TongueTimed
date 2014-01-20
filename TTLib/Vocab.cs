using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TTLib
{
    public class Vocab
    {
        public string Source { get; private set; }
        public string Target { get; private set; }

        protected Vocab(string source, string target)
        {
            Source = source;
            Target = target;
        }

        public override string ToString()
        {
            return Source + " -> " + Target;
        }
    }
}
