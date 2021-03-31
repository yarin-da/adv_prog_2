using System;
using System.Collections.Generic;
using System.Text;

namespace Adv_Prog_2
{
    interface IFileIterator
    {
        public bool HasNext { get; }
        public string CurrentLine { get; }
        public int LineNumber { get; set; }
        public int LineCount { get; }
        public void Next();
        public void LoadFile(string filePath);
    }
}
