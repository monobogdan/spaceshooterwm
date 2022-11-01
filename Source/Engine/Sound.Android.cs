using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Android.Media;

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
        internal int ID;

        public unsafe SoundStream(string fileName)
        {
            ID = Engine.Current.Sound.Context.Load(fileName, 1);
        }

        ~SoundStream()
        {
            Engine.Current.Sound.Context.Unload(ID);
        }

        public static unsafe SoundStream FromWave(string fileName)
        {
            string path = Engine.Current.Window.GetAssetPath(fileName);

            if (File.Exists(path))
                return new SoundStream(path);
            
            return null;
        }
    }

    public sealed class SoundInstance
    {
        public SoundStream Stream;
        private int playbackSource;

        public SoundInstance(SoundStream strm)
        {
            Stream = strm;
        }

        public void Play()
        {
            if(Stream != null)
                playbackSource = Engine.Current.Sound.Context.Play(Stream.ID, 1, 1, 0, 0, 1.0f);
        }

        public void Stop()
        {
            if (playbackSource != 0)
            {
                Engine.Current.Sound.Context.Stop(playbackSource);

                playbackSource = 0;
            }
        }
    }

    public sealed class SoundManager
    {
        internal SoundPool Context;

        public bool IsInitialized;

        public SoundManager()
        {
            Engine.Current.Log("SoundManager backend: SoundPool");

            Context = new SoundPool(64, Android.Media.Stream.Music, 1);
        }


    }
}
