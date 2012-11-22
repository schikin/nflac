using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using org.nflac.structure.exceptions;
using org.nflac.structure.data;
using org.nflac.structure.metaheaders;
using org.nflac.structure.util;
using org.nflac.audioformat.flac;
using org.nflac.audioformat;
using org.nflac.structure.exceptions.stream;

namespace org.nflac.structure
{
    public class FlacFile
    {
        private List<Metadata> headers = new List<Metadata>();
        private String fileName;

        private Stream inputStream;

        private StreamInfo streamInfo;

        private FLACPCMStream pcmStream;

        public FLACPCMStream PCMStream
        {
            get { return pcmStream; }
        }

        private const ulong FLAC_HEADER = 0x664C6143; //fLaC in ASCII

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

        private void CheckFile()
        {
            ulong header = 0;
            byte buff;

            int i;

            for (i = 0; i < 4; i++)
            {
                header = header << 8;
                buff = (byte)inputStream.ReadByte();
                header |= buff;
            }

            if (header != FLAC_HEADER)
            {
                throw new MalformedFileException("Flac header corrupted");
            }
        }

        private void ParseHeaders()
        {
            Metadata block = null;

            do {
                block = Metadata.Decode(inputStream);

                if (block is StreamInfo) 
                {
                    streamInfo = (StreamInfo)block;
                }

                headers.Add(block);
            } while (block.IsLastBlock ==false);


        }

        private void PrepareStream()
        {
            pcmStream = new FLACPCMStream(inputStream, streamInfo);
        }

        public void WriteWav(Stream output)
        {
            WaveStream str = new WaveStream(new FLACWaveHeader(streamInfo), new FLACPCMStream(inputStream, streamInfo));

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
                return new WaveStream(new FLACWaveHeader(streamInfo), new FLACPCMStream(inputStream, streamInfo));
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
    }
}
