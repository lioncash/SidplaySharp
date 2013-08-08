using System;
using SidplaySharp;
using NAudio.Wave;

namespace SidplaySharpExample
{
	/// <summary>
	/// Example/Test program.
	/// </summary>
	internal class Program
	{
		#region Fields

		private static int track;
		private static SidReader sid;
		private static bool isPlaying;

		#endregion

		#region Methods

		internal static void Main(string[] args)
		{
			// Create a SID reader.
			sid = new SidReader("Tests/2short1s.sid", 44100, 2, SidModel.MOS8580, SidClock.NTSC);

			// Initialize the player and start.
			IWavePlayer player = new WaveOut();
			player.Init(sid);

			// Initialize track count
			track = sid.CurrentSong;

			// Display the initial track information.
			DisplayTrackInfo();

			// Loop to check if the user hits left or right arrow keys to change tracks.
			ConsoleKeyInfo keyInfo;
			do
			{
				keyInfo = Console.ReadKey();
				if (keyInfo.Key == ConsoleKey.LeftArrow)
				{
					if (track > 1)
					{
						track--;
						sid.SetTune(track);
						DisplayTrackInfo();
					}
				}
				else if (keyInfo.Key == ConsoleKey.RightArrow)
				{
					if (track != sid.NumberOfSubtunes)
					{
						track++;
						sid.SetTune(track);
						DisplayTrackInfo();
					}
				}
				else if (keyInfo.Key == ConsoleKey.Spacebar)
				{
					if (isPlaying)
					{
						player.Pause();
						isPlaying = false;
					}
					else
					{
						player.Play();
						isPlaying = true;
					}
				}
			} while (keyInfo.Key != ConsoleKey.Escape && keyInfo.Key != ConsoleKey.Enter);
		}

		private static void DisplayTrackInfo()
		{
			Console.Clear();

			Console.WriteLine("Title: "     + sid.TuneTitle);
			Console.WriteLine("Author: "    + sid.TuneAuthor);
			Console.WriteLine("Copyright: " + sid.TuneCopyright);
			Console.WriteLine();
			Console.WriteLine("Current Track: " + track);
			Console.WriteLine("Total Subtunes: " + sid.NumberOfSubtunes);
			Console.WriteLine("-----------------------------------------");
			Console.WriteLine("Controls: ");
			Console.WriteLine();
			Console.WriteLine("Spacebar              : Pause/Play");
			Console.WriteLine("Left/Right Arrow Keys : Next/Prev track (if present)");
			Console.WriteLine("Enter/Escape          : Exit application");
		}

		#endregion
	}
}
