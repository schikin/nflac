using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Org.Nflac.Flac.Metaheaders;

namespace Org.Nflac
{
    /* public class FlacFile
    {
        
        private String fileName;

        private Stream inputStream;

       

        private FLACPCMStream pcmStream;

        public FLACPCMStream PCMStream
        {
            get { return pcmStream; }
        }

        

        public FlacFile(String file)
        {
            fileName = file;

            inputStream = File.OpenRead(fileName);
        }

        public FlacFile(Stream inputStream)
        {
            this.inputStream = inputStream;
        }

        public void ParseFile()
        {
            try
            {

                CheckFile();
                ParseHeaders();
                PrepareStream();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                //inputStream.Close();
            }
        }



        

        private void PrepareStream()
        {
            pcmStream = new FLACPCMStream(inputStream, streamInfo);
        }

        public void WriteWav(Stream output)
        {
            WaveStream str = new WaveStream(new FLACNFlacInfo(streamInfo), new FLACPCMStream(inputStream, streamInfo));

            int b;

            while ((b = str.ReadByte()) != -1)
            {
                output.WriteByte((byte)b);
            }
        }

        public WaveStream WaveStream
        {
            get
            {
                return new WaveStream(new FLACNFlacInfo(streamInfo), new FLACPCMStream(inputStream, streamInfo));
            }
        }

        public String FileName
        {
            get { return fileName; }
            set 
            { 
                fileName = value;
                ParseFile();
            }
        }

        public List<Metadata> Headers
        {
            get { return headers; }
            set { headers = value; }
        }
    } */
}
