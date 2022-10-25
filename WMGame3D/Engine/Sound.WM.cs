using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace WMGame3D
{
    public struct WavHeader
    {
        public int chunkId;
        public int chunkSize;
        public int format;
        public int subchunkid;
        public int subchunksize;
        public short audioFormat;
        public short numChannels;
        public int sampleRate;
        public int byteRate;
        public short blockAlign;
        public short bitsPerSample;
        public int subchunk2Id;
        public int subchunk2Size;
    }

    public sealed class SoundStream
    {
        public unsafe SoundStream(WavHeader header, byte[] data)
        {

        }

        public static unsafe SoundStream FromWave(string fileName)
        {
            /*Stream strm = Engine.Current.OpenAsset(fileName);

            if (strm != null)
            {

                BinaryReader reader = new BinaryReader(strm);
                WavHeader hdr = new WavHeader();
                hdr.chunkId = reader.ReadInt32();
                hdr.chunkSize = reader.ReadInt32();
                hdr.format = reader.ReadInt32();
                hdr.subchunkid = reader.ReadInt32();
                hdr.subchunksize = reader.ReadInt32();
                hdr.audioFormat = reader.ReadInt16();
                hdr.numChannels = reader.ReadInt16();
                hdr.sampleRate = reader.ReadInt32();
                hdr.byteRate = reader.ReadInt32();
                hdr.blockAlign = reader.ReadInt16();
                hdr.bitsPerSample = reader.ReadInt16();
                hdr.subchunk2Id = reader.ReadInt32();
                hdr.subchunk2Size = reader.ReadInt32();

                byte[] data = new byte[strm.Length - sizeof(WavHeader)];
                reader.Read(data, 0, data.Length);

                
            }
            */
            return null;
        }
    }

    public sealed class SoundInstance
    {
        public SoundInstance(SoundStream strm)
        {

        }

        public void Play()
        {

        }

        public void Stop()
        {

        }
    }

    public sealed class SoundManager
    {

        public SoundManager()
        {

        }


    }
}
