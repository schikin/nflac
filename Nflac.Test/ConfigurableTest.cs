using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Diagnostics;
using Org.Nflac.Tests.Exceptions;
using Org.Nflac.Wave.Util;
using Org.Nflac.Flac.Integration;

namespace Org.Nflac.Tests
{
    //[TestClass]
    public abstract class ConfigurableTest
    {
        private Stream xmlStream;
        private Stream xsdStream;

        private XmlDocument xmlConfig;

        public ConfigurableTest(Stream xsdStream, Stream xmlStream)
        {
            this.xmlStream = xmlStream;
            this.xsdStream = xsdStream;

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.Schemas.Add("https://github.com/schikin/nflac", new XmlTextReader(xsdStream));
            XmlReader reader = XmlReader.Create(xmlStream, settings);

            xmlConfig = new XmlDocument();
            xmlConfig.Load(reader);
        }

        //[TestMethod]
        public void TestConfigConsistency()
        {
            ValidationEventHandler handle = new ValidationEventHandler(ValidationEventHandler);

            xmlConfig.Validate(handle);
        }

        

        //[TestMethod]
        public void TestFromXMLConfig()
        {
            //ok, we're not using linq for platfrom-independence
            //XPath is not suitable here. It's like using a sledge-hammer to crack a nut.
            foreach (XmlNode node in xmlConfig.ChildNodes)
            {
                if (node.Name == "NFLACTestConfiguration")
                {
                    Console.WriteLine("Running total of :" + node.ChildNodes.Count + "tests: ");

                    foreach (XmlNode testCase in node.ChildNodes)
                    {
                        ParseNextToken(testCase);
                    }
                }
            }
            
            //XPathNavigator nav = xmlConfig.CreateNavigator();

            //XmlNamespaceManager manager = new XmlNamespaceManager(nav.NameTable);
            //manager.AddNamespace("def", "https://github.com/schikin/nflac");
            //XPathExpression topSelect = XPathExpression.Compile("/def:NFLACTestConfiguration/def:TestCase");
            //topSelect.SetContext(manager);

            //XPathNodeIterator it = nav.Select(topSelect);

            //Console.WriteLine(it.Current.Name);

            //while (it.MoveNext())
            //{
            //    Console.WriteLine("Dispatching control to child testcase");
            //    ParseNextToken(it.Current);
            //}
        }

        static void ValidationEventHandler(object sender, ValidationEventArgs e) 
        {
            throw new XMLAssertException();
        }

        protected abstract void ParseNextToken(XmlNode node);

    }
}
