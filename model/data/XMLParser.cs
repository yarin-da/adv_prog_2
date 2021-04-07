using System.Collections.Generic;
using System.Xml.Linq;

namespace Adv_Prog_2
{
    class XMLParser
    {
        private List<string> titles = null;

        public bool HasData { get { return titles != null; } }

        public List<string> GetTitles()
        {
            return titles;
        }

        public void LoadFile(string filePath)
        {
            titles = ParseXML(filePath);
        }

        private int Count(List<string> list, string toCount)
        {
            int counter = 0;
            foreach (string str in list)
            {
                if (str == toCount) { counter++; }
            }
            return counter;
        }

        private List<string> HandleDuplicates(List<string> titles)
        {
            HashSet<string> duplicates = new HashSet<string>();
            foreach (string title in titles) {
                if (Count(titles, title) > 1)
                {
                    duplicates.Add(title);
                }
            }

            foreach (string dup in duplicates)
            {
                int counter = 1;
                for (int i = 0; i < titles.Count; i++)
                {
                    if (titles[i] == dup)
                    {
                        titles[i] = dup + counter;
                        counter++;
                    }
                }
            }

            return titles;
        }

        public List<string> ParseXML(string filePath)
        {
            // read XML file into data
            string data = System.IO.File.ReadAllText(filePath);
            // get to the 'input' element (because we want to ignore 'output')
            // and get all of chunk elements' names
            IEnumerable<XElement> chunkNames =
                XDocument.Parse(data)
                .Element("PropertyList")
                .Element("generic")
                .Element("input")
                .Descendants("name");
            // put all chunk names in a list
            List<string> titles = new List<string>();
            foreach (XElement name in chunkNames)
            {
                titles.Add(name.Value);
            }

            return HandleDuplicates(titles);
        }
    }
}
