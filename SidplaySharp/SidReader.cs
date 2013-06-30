using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using NAudio.Wave;

namespace SidplaySharp
{
	/// <summary>
	/// Reader for playing back Commodore 64 SID files.
	/// </summary>
	public sealed class SidReader : WaveStream
	{
		private readonly WaveFormat waveFormat;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="file">The path to the SID file to open.</param>
		/// <param name="sampleRate">Desired sample rate.</param>
		/// <param name="channels">Desired number of channels to use.</param>
		/// <param name="sidToUse">The type of SID chip to use.</param>
		/// <param name="clockFreqToUse">The clock frequency to use during playback.</param>
		public SidReader(string file, int sampleRate=44100, int channels=2, SidModel sidToUse=SidModel.Correct, SidClock clockFreqToUse=SidClock.Correct)
		{
			// Ensure the sample rate specified is clamped to valid values
			if (sampleRate <= 0)
				sampleRate = 44100;

			// Ensure the number of channels is correct.
			if (channels <= 0 || channels > 2)
				channels = 2;

			// Initialize wave format.
			waveFormat = new WaveFormat(sampleRate, 16, channels);

			// Initialize the SID player.
			byte[] fileBytes = File.ReadAllBytes(file);
			SidplayNative.InitializePlayer(fileBytes, fileBytes.Length - 1, sampleRate, channels, sidToUse, clockFreqToUse);

			// Set the info properties.

			//--- Tune title ---//
			byte[] buffer = new byte[81];
			Marshal.Copy(SidplayNative.TuneTitle(), buffer, 0, buffer.Length);
			string tuneTitle = Encoding.UTF8.GetString(buffer);
			tuneTitle = tuneTitle.Trim((char) 0x00);
			this.TuneTitle = tuneTitle;

			//--- Tune Author --//
			Array.Clear(buffer, 0, buffer.Length);
			Marshal.Copy(SidplayNative.TuneAuthor(), buffer, 0, buffer.Length);
			string tuneAuthor = Encoding.UTF8.GetString(buffer);
			tuneAuthor = tuneAuthor.Trim((char) 0x00);
			this.TuneAuthor = tuneAuthor;

			//--- Tune Copyright ---//
			Array.Clear(buffer, 0, buffer.Length);
			Marshal.Copy(SidplayNative.TuneCopyright(), buffer, 0, buffer.Length);
			string tuneCopyright = Encoding.UTF8.GetString(buffer);
			tuneCopyright = tuneCopyright.Trim((char) 0x00);
			this.TuneCopyright = tuneCopyright;
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			return SidplayNative.GenerateSamples(buffer, count);
		}

		public override WaveFormat WaveFormat
		{
			get { return waveFormat; }
		}

		// TODO: Not possible to read track lengths with sidplay2 yet, I think.
		public override long Length
		{
			get { return 90000000000; }
		}

		// TODO: Possibly implement seeking.
		public override long Position
		{
			get;
			set;
		}

		#region Info Properties

		/// <summary>
		/// The title of the currently loaded SID tune.
		/// </summary>
		public string TuneTitle
		{
			get;
			private set;
		}

		/// <summary>
		/// The author of the currently loaded SID tune.
		/// </summary>
		public string TuneAuthor
		{
			get;
			private set;
		}

		/// <summary>
		/// Copyright string of the currently loaded SID file.
		/// </summary>
		public string TuneCopyright
		{
			get;
			private set;
		}


		#endregion

		/// <summary>
		/// Destructor
		/// </summary>
		~SidReader()
		{
			SidplayNative.FreePlayer();
		}
	}
}
