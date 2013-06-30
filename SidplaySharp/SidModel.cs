namespace SidplaySharp
{
	/// <summary>
	/// The type of SID chip model to use during playback.
	/// </summary>
	public enum SidModel : uint
	{
		/// <summary>The chip the SID file was made for.</summary>
		Correct = 0,

		/// <summary>Force using the MOS6581 SID chip.</summary>
		MOS6581 = 1,

		/// <summary>Force using the MOS8580 SID chip.</summary>
		MOS8580 = 2,
	};
}
