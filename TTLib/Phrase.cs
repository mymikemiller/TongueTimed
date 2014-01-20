using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TTLib
{
    class Phrase : Vocab
    {
        public List<Word> ComponentWords { get; private set; }

        public Phrase(string source, string target, List<Word> componentWords)
            : base(source, target)
        {
            ComponentWords = componentWords;
        }
    }
}
