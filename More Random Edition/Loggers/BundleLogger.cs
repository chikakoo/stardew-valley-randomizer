namespace Randomizer;

//TODO: initialize this AND actually use it!
/// <summary>
/// Spoiler-free log of all bundles for easy tracking
/// </summary>
public class BundleLogger : Logger
{
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="farmName">The name of the farm - used to easily identify the log</param>
	public BundleLogger(string farmName)
	{
		if (!Globals.Config.CreateBundleLog) { return; }

		PathPrefix = "BundleLog";
		InitializePath(farmName);
	}

	/// <summary>
	/// Writes text to the file, provided the settings allow it
	/// </summary>
	public override void WriteFile()
	{
		if (!Globals.Config.CreateBundleLog)
		{
			TextToWrite = "";
			return;
		}

		base.WriteFile();
	}
}
