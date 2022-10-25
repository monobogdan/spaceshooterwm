using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SharpDX;
using SharpDX.DirectSound;

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
        internal SecondarySoundBuffer buffer;

        public unsafe SoundStream(WavHeader header, byte[] data)
        {
            SoundBufferDescription desc = new SoundBufferDescription();
            desc.BufferBytes = data.Length;
            desc.Flags = BufferFlags.None;
            desc.Format = new SharpDX.Multimedia.WaveFormat(header.sampleRate, header.bitsPerSample, header.numChannels);

            buffer = new SecondarySoundBuffer(Engine.Current.Sound.device, desc);

            DataStream sp;
            DataStream pp = buffer.Lock(0, desc.BufferBytes, LockFlags.EntireBuffer, out sp);
            pp.WriteRange(data);
            buffer.Unlock(pp, sp);
        }

        public static unsafe SoundStream FromWave(string fileName)
        {
            Stream strm = Engine.Current.OpenAsset(fileName);

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

                return new SoundStream(hdr, data);
            }

            return null;
        }
    }

    public sealed class SoundInstance
    {
        private SoundBuffer buf;

        public SoundInstance(SoundStream strm)
        {
            buf = Engine.Current.Sound.device.DuplicateSoundBuffer(strm.buffer);
        }

        public void Play()
        {
            buf.Play(0, PlayFlags.None);
        }

        public void Stop()
        {
            buf.Stop();
        }
    }

    public sealed class SoundManager
    {
        internal DirectSound device;
        private PrimarySoundBuffer soundBuf;

        public SoundManager()
        {
            device = new DirectSound();
            device.SetCooperativeLevel(Engine.Current.Window.Handle, CooperativeLevel.Normal);

            SoundBufferDescription desc = new SoundBufferDescription();
            desc.BufferBytes = 0;
            desc.Flags = BufferFlags.PrimaryBuffer;

            soundBuf = new PrimarySoundBuffer(device, desc);
        }


    }
}
