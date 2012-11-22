using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Xml.XPath;
using org.nflac.structure;
using System.Diagnostics;
using System.Xml;
using org.nflac.tests.exceptions;

namespace org.nflac.tests
{
    [TestClass]
    public class SampleRate : ConfigurableTest
    {
        public SampleRate() : base (File.OpenRead("samplerate\\testConfig.xsd"),File.OpenRead("samplerate\\testConfig.xml"))
        {

        }

        protected override void ParseNextToken(XmlNode node)
        {
            int sampleRate = 0;
            Stream wavReferenceStream = null;
            Stream flacStream = null;
            Stream wavOutputStream = null;

            Console.WriteLine("--------------------------------------");

            foreach (XmlNode confLine in node)
            {
                switch (confLine.Name)
                {
                    case "TestName":
                        Console.WriteLine("Test: " + confLine.InnerText);
                        break;
                    case "FLACFile":
                        Console.WriteLine("Parsing file: samplerate\\" + confLine.InnerText);
                        flacStream = File.OpenRead("samplerate\\" + confLine.InnerText);
                        break;
                    case "WAVFile":
                        Console.WriteLine("Comparing to file: samplerate\\" + confLine.InnerText);
                        wavReferenceStream = File.OpenRead("samplerate\\" + confLine.InnerText);
                        break;
                    case "SampleRate":
                        sampleRate = Int32.Parse(confLine.InnerText);
                        break;
                }
            }

            if (flacStream == null)
            {
                throw new GenericException("No FLAC file found in the config and/or stream open failed");
            }

            if (wavReferenceStream == null)
            {
                throw new GenericException("No WAV file found in the config and/or stream open failed");
            }

            if (sampleRate == 0)
            {
                throw new GenericException("No sample rate specified in the config and/or stream open failed");
            }
            

            FlacFile decoder = new FlacFile(flacStream);

            decoder.ParseFile();

            Console.WriteLine("FLAC StreamInfo: "+ Environment.NewLine +decoder.Headers[0]);
            
            wavOutputStream = decoder.WaveStream;

            Assert.AreEqual(wavOutputStream.Length,wavReferenceStream.Length);
            
            int bReference;
            int bOut;

            int i = 0;

            while ((bReference = wavReferenceStream.ReadByte()) != -1)
            {
                i++;
                Assert.AreEqual(bReference, wavOutputStream.ReadByte());
            }

            Console.WriteLine(i + " WAV bytes successfully read");
        }
    }
}
