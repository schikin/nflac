using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using org.nflac.structure;
using org.nflac.structure.metaheaders;
using System.IO;
using System.Media;
using NAudio.Wave;

namespace nFLAC_test
{
    class Program
    {
        static void Main(string[] args)
        {
            //FlacFile f = new FlacFile("C:\\Documents and Settings\\Администратор\\Рабочий стол\\Pink Floyd . 1975 . Wish You Were Here (CDP 7 46035 2) (black)\\Pink Floyd - Wish You Were Here.Black Face.Harvest.Japan.flac");
            FlacFile f = new FlacFile("Z:\\Documents\\LiadovPrevin.flac");
            
            f.ParseFile();

            foreach (Metadata header in f.Headers)
            {
                Console.WriteLine(header);
            }

            Stream outStr = File.OpenWrite("C:\\Documents and Settings\\Администратор\\Рабочий стол\\test.wav");

            //NAudio.Wave.WaveStream playStr = new NAudio.Wave.WaveFileReader(f.WaveStream);

            //WaveOut device = new WaveOut();
            //device.Init(playStr);

            //device.Play();
            int b;
            int i=0;

            Stream str = f.WaveStream;

            while ((b = str.ReadByte()) != -1)
            {
                outStr.WriteByte((byte)b);
                if (i++ == 31) 
                {
                  //break
                }
            }

            outStr.Close();

            Console.ReadLine();

        }
    }
}
