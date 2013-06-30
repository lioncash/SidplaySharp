namespace SidplaySharp
{
	/// <summary>
	/// Clock speeds that libsidplay2 can use.
	/// </summary>
	public enum SidClock
	{
		/// <summary>The clock speed that the SID file was made for.</summary>
		Correct = 0,

		/// <summary>Force playback using PAL clock frequency.</summary>
		PAL     = 1,

		/// <summary>Force playback using NTSC clock frequency.</summary>
		NTSC    = 2,
	};
}
