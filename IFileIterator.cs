namespace Adv_Prog_2
{
    interface IFileIterator
    {
        public delegate void CallbackFunc();
        public event CallbackFunc OnLineChanged;
        public bool HasNext { get; }
        public string CurrentLine { get; }
        public int LineNumber { get; set; }
        public int LineCount { get; }
        public void Next();
        public void LoadFile(string filePath);
    }
}
