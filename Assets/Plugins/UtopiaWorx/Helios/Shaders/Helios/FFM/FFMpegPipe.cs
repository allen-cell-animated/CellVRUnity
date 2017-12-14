using System.Diagnostics;
using System.IO;
using System;
using UnityEngine;

namespace FFmpegOut
{
    // A stream pipe class that invokes ffmpeg and connect to it.
    class FFmpegPipe
    {
        #region Public properties

        public enum Codec { ProRes, H264, VP8, AVI }

        public string Filename { get; private set; }


        #endregion

        #region Public methods

        public FFmpegPipe(string name, int width, int height, int framerate, string pxfmt)
        {
            string OS = "";
#if UNITY_EDITOR_WIN
            OS = "win";
#endif

#if UNITY_EDITOR_OSX
					OS = "osx";
#endif

            
            name += DateTime.Now.ToString(" yyyy MMdd HHmmss");
            Filename = name.Replace(" ", "_") + ".mp4";

            var opt = "-y -f rawvideo -vcodec rawvideo -pixel_format " + pxfmt;
            opt += " -video_size " + width + "x" + height;
            opt += " -framerate " + framerate;
            opt += " -loglevel warning -i - " + GetOptions(Codec.H264);
            opt += " " + Filename;
            var info = new ProcessStartInfo(Application.dataPath + "/" + "UtopiaWorx/Helios/Bin/ffmpeg/" + OS + "/" + @"ffmpeg.exe", opt);
            UnityEngine.Debug.Log(opt);
            info.UseShellExecute = false;
            info.CreateNoWindow = true;
            info.RedirectStandardInput = true;
            info.RedirectStandardOutput = true;
            info.RedirectStandardError = true;

            _subprocess = Process.Start(info);
            _stdin = new BinaryWriter(_subprocess.StandardInput.BaseStream);
        }

        public void Write( byte[] data)
        {
            if (_subprocess == null) return;

            _stdin.Write(data);
            _stdin.Flush();
        }

        public void Close()
        {
            if (_subprocess == null) return;

            _subprocess.StandardInput.Close();
            _subprocess.WaitForExit();

            var outputReader = _subprocess.StandardError;


            _subprocess.Close();
            _subprocess.Dispose();

            outputReader.Close();
            outputReader.Dispose();

            _subprocess = null;
            _stdin = null;
        }

        #endregion

        #region Private members

        Process _subprocess;
        BinaryWriter _stdin;



        static string[] _options = {
            "-c:v prores_ks -pix_fmt yuv422p10le",
            "-pix_fmt yuv420p",
            "-c:v libvpx",
            "-c:v huffyuv"
        };


        static string GetOptions(Codec codec)
        {
            return _options[(int)codec];
        }
        #endregion
    }
}