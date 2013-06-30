using System;
using System.Runtime.InteropServices;

namespace SidplaySharp
{
	/// <summary>
	/// P/Invoke methods for libsidplay2.
	/// </summary>
	public static class SidplayNative
	{
		private const string DllName = "libsidplay";

		// Note that InitializePlayer must be called before calling any other method, or else you'll just get error values.

		/// <summary>
		/// Initializes the player.
		/// </summary>
		/// <param name="data">The SID file to open as a byte array.</param>
		/// <param name="byteLength">The length of the byte array. Just pass data.Length-1</param>
		/// <param name="sampleRate">The desired sample rate to use.</param>
		/// <param name="channels">The number of sound channels to use.</param>
		/// <param name="sidToUse">The type of SID chip to use.</param>
		/// <param name="clockFreqToUse">The clock frequency to use during playback.</param>
		/// <returns>0 if the player initialized. A negative number if an error occurred.</returns>
		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int InitializePlayer(byte[] data, int byteLength, int sampleRate, int channels, SidModel sidToUse, SidClock clockFreqToUse);

		/// <summary>
		/// Deallocates and frees the player.
		/// </summary>
		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void FreePlayer();

		/// <summary>
		/// Generates sampleCount number of samples into the given buffer.
		/// </summary>
		/// <param name="buffer">The buffer to generate samples into.</param>
		/// <param name="sampleCount">The number of samples to generate.</param>
		/// <returns>The number of samples generated into the buffer.</returns>
		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int GenerateSamples(byte[] buffer, int sampleCount);

		/// <summary>
		/// Set a tune to play.
		/// </summary>
		/// <param name="tune">The index of the tune to play.</param>
		/// <returns>True if the tune was able to be set. False otherwise.</returns>
		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool SetTune(int tune);

		/// <summary>
		/// The number of tunes within the SID file.
		/// </summary>
		/// <returns>The number of tunes within the SID file.</returns>
		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int NumberOfSubtunes();

		/// <summary>
		/// Gets the index of the starting tune.
		/// </summary>
		/// <returns>The index of the starting tune.</returns>
		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int StartingTune();

		/// <summary>
		/// Gets the index of the currently initialized song.
		/// </summary>
		/// <returns>The index of the currently initialized song.</returns>
		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int CurrentSong();

		// Gets the title of the SID tune.
		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr TuneTitle();

		// Gets the author of the SID tune.
		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr TuneAuthor();

		// Gets the copyright string of the SID tune.
		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr TuneCopyright();
	}
}
