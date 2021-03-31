using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Adv_Prog_2
{
    class XMLParser
    {
        private List<string> titles = null;

        public void LoadFile(string filePath)
        {
            titles = ParseXML(filePath);
        }

        private int getIndex(string title)
        {
            if (titles != null)
            {
                for (int i = 0; i < titles.Count; i++)
                {
                    if (titles[i].CompareTo(title) == 0)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public float getValue(string line, string title)
        {
            int index = getIndex(title);
            if (index == -1)
            {
                return 0;
            }
            string data = line.Split(",")[index];
            float ret = float.Parse(data);
            return ret;
        }

        public List<string> getTitles()
        {
            return titles;
        }

        public List<string> ParseXML(string filePath)
        {
            string data = System.IO.File.ReadAllText(filePath);
            IEnumerable<XElement> chunkNames =
                XDocument.Parse(data)
                .Element("PropertyList")
                .Element("generic")
                .Element("input")
                .Descendants("name");
            List<string> titles = new List<string>();
            int index = 0;
            foreach (XElement name in chunkNames)
            {
                titles.Add(name.Value);
                index++;
            }
            return titles;
        }

    }
}
