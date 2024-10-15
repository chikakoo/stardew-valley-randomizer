namespace Randomizer;

/// <summary>
/// Used to log the randomization so players can see what was done
/// </summary>
public class SpoilerLogger : Logger
{
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="farmName">The name of the farm - used to easily identify the log</param>
	public SpoilerLogger(string farmName)
	{
		if (!Globals.Config.CreateSpoilerLog) { return; }

		PathPrefix = "SpoilerLog";
		InitializePath(farmName);
	}

	/// <summary>
	/// Writes text to the file, provided the settings allow it
	/// </summary>
	public override void WriteFile()
	{
		if (!Globals.Config.CreateSpoilerLog)
		{
			TextToWrite = "";
			return;
		}

		base.WriteFile();
	}
}
