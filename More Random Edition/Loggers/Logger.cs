using System;
using System.IO;
using IOPath = System.IO.Path;

namespace Randomizer;

public abstract class Logger
{
	/// <summary>
	/// The name to be pre-pended to the path of the log file
	/// </summary>
	protected string PathPrefix = default;

	/// <summary>
	/// The path to the spoiler log
	/// </summary>
	protected string Path { get; set; }

	/// <summary>
	/// The text to write to the spoiler log - it's best to write a big block of text at once
	/// rather than several small ones to avoid the overhead of opening and disposing the file
	/// </summary>
	protected string TextToWrite { get; set; }

	/// <summary>
	/// Initializes Path to be the path to the logger
	/// </summary>
	/// <param name="farmName">The farm name to append to the file name</param>
	protected void InitializePath(string farmName)
	{
		if (PathPrefix == default)
		{
			Globals.ConsoleError("Initializing logger with a default PathPrefix!");
		}

		var spoilerLogName = string.Join("", farmName.Split(IOPath.GetInvalidFileNameChars()));
		Path = Globals.GetFilePath($"{PathPrefix}-{spoilerLogName}.txt");
		File.Create(Path).Close();
	}

	/// <summary>
	/// Adds a line to the buffer
	/// </summary>
	/// <param name="line">The line</param>
	public void BufferLine(string line)
	{
		TextToWrite += $"{line}{Environment.NewLine}";
	}

	/// <summary>
	/// Writes a line to the end of the file
	/// It's expected for this to be overridden so this can be disabled by settings
	/// </summary>
	/// <param name="line">The line</param>
	public virtual void WriteFile()
	{
		using StreamWriter file = new(Path, true);
		file.WriteLine(TextToWrite);
	}
}