using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Diagnostics;

namespace Loreganizer
{
    internal class LrgXmlReader
    {
        XmlTextReader _reader;

        public LrgXmlReader(String filename)
        {
            _reader = new XmlTextReader(filename);
        }

        public void Read()
        {
            String curTag = "tagless";
            while (_reader.Read())
            {
                switch (_reader.NodeType)
                {
                    case XmlNodeType.Element:
                        curTag = _reader.Name;
                        DetermineObject(curTag);
                        break;


                }
            }
        }

        public void TempRead()
        {
            string cur = "";
            while (_reader.Read())
            {
                switch (_reader.NodeType)
                {
                    case XmlNodeType.Element:
                        cur = _reader.Name;
                        break;

                    case XmlNodeType.Text:
                        cur = _reader.Value;
                        break;

                    case XmlNodeType.EndElement:
                        cur = _reader.Name;
                        break;
                }
                Debug.WriteLine("Read: " + cur);
            }
        }

        private void DetermineObject(String tag)
        {
            if (tag.Equals("tb"))
            {
                Read_tb();
            }
            else
            {
                _reader.MoveToNextAttribute();
            }
        }

        private void Read_tb()
        {
            string[] tbData = new string[5];
            string tag = "tagless";
            int found = 0;
            while(_reader.Read() && found<5)
            {
                switch (_reader.NodeType)
                {
                    case XmlNodeType.Element:
                        tag = _reader.Name;
                        break;
                }

                switch (tag)
                {
                    case "height":
                        _reader.Read();
                        tbData[0] = _reader.Value;
                        found++;
                        tag = "tagless";
                        break;

                    case "width":
                        _reader.Read();
                        tbData[1] = _reader.Value;
                        found++;
                        tag = "tagless";
                        break;

                    case "x":
                        _reader.Read();
                        tbData[2] = _reader.Value;
                        found++;
                        tag = "tagless";
                        break;

                    case "y":
                        _reader.Read();
                        tbData[3] = _reader.Value;
                        found++;
                        tag = "tagless";
                        break;

                    case "content":
                        _reader.Read();
                        tbData[4] = _reader.Value;
                        found++;
                        tag = "tagless";
                        break;

                }
            }
            ((MainWindow)System.Windows.Application.Current.MainWindow).DrawTextBoxFromData(tbData);
        }
    }
}
