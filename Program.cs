using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using OsLib;
using RaiImage;

namespace iorg;

public static class Icons
{
	public const char Error = '\uea87';
	public const char Info = '\uea74';
	public const char Help = '\uf059';
	public const char NotAvailable = '\ueabd';
	public const char File = '\uea7b';
	public const char Folder = '\uea83';
	public const char Banner = '\ueb1e';
	public const char NoBanner = '\ueb24';
}

public static class Messages
{
	public static bool Debug { get; set; }
	public static bool Banner { get; set; } = true;
	public static string? CloudProvider { get; set; }
	public static RaiPath? DestinationRoot { get; set; }
	public static RaiPath? SourceRoot { get; set; }
	public static string? Subscriber { get; set; }
	public static string Filter { get; set; } = "*.jpg";
	public static PathConventionType PathConvention { get; set; } = PathConventionType.ItemIdTree8x2;
	public static ImageNamingConvention NamingConvention { get; set; } = ImageNamingConvention.Structured;

	public static string[] Help =>
	[
		$"-h, --help\t{Icons.Help}\tprint out all options",
		$"-v, --version\t{Icons.Info}\tprint version info",
		$"-n, --nologo\t{(Banner ? Icons.Banner : Icons.NoBanner)}\tdo not display the banner",
		$"-b, --debug\t{Icons.Info}\tenable debug output",
		$"-c, --cloud\t{Icons.Folder}\t{CloudDescription()}",
		$"-r, --root\t{Icons.Folder}\t{DestinationDescription()}",
		$"-s, --source\t{Icons.File}\t{SourceDescription()}",
		$"-f, --filter\t{Icons.File}\tfile filter, currently {Filter}",
		$"-p, --pathconvention\t{Icons.Info}\t{PathConvention}; options: {string.Join(", ", Enum.GetNames<PathConventionType>())}",
		$"-m, --namingconvention\t{Icons.Info}\t{NamingConvention}; options: {string.Join(", ", Enum.GetNames<ImageNamingConvention>())}",
		$"{Icons.Info} Subscriber\t{Icons.File}\t{SubscriberDescription()}",
	];

	private static string CloudDescription()
	{
		if (!string.IsNullOrWhiteSpace(CloudProvider))
		{
			var cloudDir = Os.Config?.Cloud?[CloudProvider];
			return $"{CloudProvider}: {cloudDir ?? "(not configured)"}";
		}
		return "cloud provider name (looks up root in Os.Config)";
	}

	private static string DestinationDescription()
	{
		return DestinationRoot != null
			? DestinationRoot.FullPath
			: "destination image root, resolved under --cloud when provided";
	}

	private static string SourceDescription()
	{
		return SourceRoot != null
			? SourceRoot.FullPath
			: "source image directory";
	}

	private static string SubscriberDescription()
	{
		return !string.IsNullOrWhiteSpace(Subscriber)
			? Subscriber
			: "subscriber directory name (e.g., nomsa)";
	}

	public static void WriteError(string text) => WriteHighlighted(text, ConsoleColor.DarkRed, ConsoleColor.White);
	public static void WriteSuccess(string text) => WriteHighlighted(text, ConsoleColor.DarkGreen);
	public static void WriteDebug(string text) { if (Debug) WriteHighlighted(text, ConsoleColor.DarkYellow); }

	private static void WriteHighlighted(string text, ConsoleColor foreground = ConsoleColor.Black, ConsoleColor? background = null)
	{
		var oldForeground = Console.ForegroundColor;
		var oldBackground = Console.BackgroundColor;
		Console.ForegroundColor = foreground;
		Console.BackgroundColor = background ?? oldBackground;
		Console.WriteLine(text);
		Console.ForegroundColor = oldForeground;
		Console.BackgroundColor = oldBackground;
	}

	public static void WriteBanner(string text)
	{
		WriteLine(text);
		Console.WriteLine(text);
		WriteLine(text);
	}

	private static void WriteLine(string text, char underlineChar = '=')
	{
		for (int i = 0; i < text.Length; i++) Console.Write(underlineChar);
		Console.WriteLine();
	}

	public static void WriteHelp()
	{
		foreach (var line in Help) WriteSuccess(line);
	}
}

public static class ImageOrganizer
{
	public static int Organize(
		RaiPath sourceRoot,
		RaiPath subscriberRoot,
		string subscriber,
		string filter = "*.jpg",
		PathConventionType pathConvention = PathConventionType.ItemIdTree8x2,
		ImageNamingConvention namingConvention = ImageNamingConvention.Structured,
		RaiPath? tempRoot = null,
		TextWriter? output = null)
	{
		if (string.IsNullOrWhiteSpace(subscriber))
			throw new ArgumentException("Subscriber is required.", nameof(subscriber));

		output ??= Console.Out;
		var stagingRoot = (tempRoot ?? Os.TempDir) / new RaiRelPath(subscriber);
		var count = 0;

		foreach (var source in sourceRoot.EnumerateFiles(filter))
		{
			var normalizedFullName = ImageFile.EasyFileName(source.FullName);
			var normalized = new ImageFile(normalizedFullName, namingConvention);
			var staged = new RaiFile(stagingRoot, normalized.NameWithExtension);
			staged.mkdir();
			staged.cp(source);

			var destination = new ImageTreeFile(
				subscriberRoot,
				normalized.ItemId,
				string.Empty,
				normalized.Ext,
				pathConvention,
				namingConvention)
			{
				ImageNumber = normalized.ImageNumber
			};

			destination.mv(staged);
			output.WriteLine($"copied {source.FullName}");
			output.WriteLine($"=> {destination.FullName}");
			count++;
		}

		return count;
	}
}

internal static class Program
{
	private static int Main(string[] args)
	{
		try
		{
			if (HasOption(args, "-v", "--version"))
			{
				Messages.WriteSuccess(GetVersion());
				return 0;
			}

			Messages.Debug = HasOption(args, "-b", "--debug");
			Messages.Banner = !HasOption(args, "-n", "--nologo");
			var showHelp = HasOption(args, "-h", "--help");
			Messages.CloudProvider = ParamValue(args, "-c", "--cloudprovider", "--cloud");
			var rootParam = ParamValue(args, "-r", "--root", "--imageroot");
			var sourceParam = ParamValue(args, "-s", "--source");
			Messages.Filter = ParamValue(args, "-f", "--filter") ?? Messages.Filter;
			Messages.Subscriber = PositionalArg(args);

			if (!TryParseEnum(ParamValue(args, "-p", "--pathconvention", "--path-convention"), out PathConventionType pathConvention))
				return 1;
			if (!TryParseEnum(ParamValue(args, "-m", "--namingconvention", "--naming-convention"), out ImageNamingConvention namingConvention))
				return 1;
			Messages.PathConvention = pathConvention;
			Messages.NamingConvention = namingConvention;

			Messages.SourceRoot = !string.IsNullOrWhiteSpace(sourceParam) ? new RaiPath(sourceParam) : null;
			var destinationRoot = ResolveDestinationRoot(Messages.CloudProvider, rootParam, Messages.Subscriber);
			Messages.DestinationRoot = destinationRoot;

			if (Messages.Banner)
				Messages.WriteBanner($"{Icons.Info} Image Organizer CLI");

			var canRun = Messages.SourceRoot != null
				&& destinationRoot != null
				&& !string.IsNullOrWhiteSpace(Messages.Subscriber);

			if (showHelp || !canRun)
			{
				Messages.WriteHelp();
				return showHelp ? 0 : 1;
			}

			Messages.WriteDebug($"SourceRoot: {Messages.SourceRoot?.FullPath}");
			Messages.WriteDebug($"DestinationRoot: {destinationRoot?.FullPath}");
			Messages.WriteDebug($"Subscriber: {Messages.Subscriber}");
			Messages.WriteDebug($"Filter: {Messages.Filter}");
			Messages.WriteDebug($"PathConvention: {pathConvention}");
			Messages.WriteDebug($"NamingConvention: {namingConvention}");

			var count = ImageOrganizer.Organize(
				Messages.SourceRoot!,
				destinationRoot!,
				Messages.Subscriber!,
				Messages.Filter,
				pathConvention,
				namingConvention);

			Messages.WriteSuccess($"{count} files copied.");
			return 0;
		}
		catch (Exception ex)
		{
			Messages.WriteError(ex.Message);
			if (Messages.Debug)
				Console.Error.WriteLine(ex);
			return 1;
		}
	}

	private static RaiPath? ResolveDestinationRoot(string? cloudProvider, string? rootParam, string? subscriber)
	{
		if (string.IsNullOrWhiteSpace(subscriber))
			return null;

		RaiPath? root = null;
		if (!string.IsNullOrWhiteSpace(cloudProvider))
		{
			string? cloudDir = Os.Config?.Cloud?[cloudProvider];
			if (string.IsNullOrWhiteSpace(cloudDir))
				throw new InvalidOperationException($"The requested cloud provider '{cloudProvider}' is missing or empty in {Os.DefaultConfigFileLocation}.");

			var cloudRoot = new RaiPath(cloudDir);
			root = !string.IsNullOrWhiteSpace(rootParam)
				? cloudRoot / new RaiRelPath(rootParam.TrimStart('/', '\\'))
				: cloudRoot;
		}
		else if (!string.IsNullOrWhiteSpace(rootParam))
		{
			root = new RaiPath(rootParam);
		}

		return root != null
			? root / new RaiRelPath(subscriber)
			: null;
	}

	private static bool TryParseEnum<TEnum>(string? value, out TEnum result)
		where TEnum : struct, Enum
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			result = default;
			if (typeof(TEnum) == typeof(PathConventionType))
				result = (TEnum)(object)PathConventionType.ItemIdTree8x2;
			else if (typeof(TEnum) == typeof(ImageNamingConvention))
				result = (TEnum)(object)ImageNamingConvention.Structured;
			return true;
		}

		if (Enum.TryParse(value, ignoreCase: true, out result))
			return true;

		Messages.WriteError($"Invalid {typeof(TEnum).Name}: {value}. Options: {string.Join(", ", Enum.GetNames<TEnum>())}");
		return false;
	}

	private static bool HasOption(string[] args, params string[] names)
	{
		return args.Any(arg => names.Contains(arg, StringComparer.OrdinalIgnoreCase));
	}

	private static string? ParamValue(string[] args, params string[] names)
	{
		for (int i = 0; i < args.Length; i++)
		{
			if (!names.Contains(args[i], StringComparer.OrdinalIgnoreCase))
				continue;

			return i + 1 < args.Length ? args[i + 1] : null;
		}
		return null;
	}

	private static string? PositionalArg(string[] args)
	{
		var optionNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"-h", "--help", "-v", "--version", "-n", "--nologo", "-b", "--debug",
			"-c", "--cloudprovider", "--cloud", "-r", "--root", "--imageroot",
			"-s", "--source", "-f", "--filter", "-p", "--pathconvention",
			"--path-convention", "-m", "--namingconvention", "--naming-convention"
		};

		for (int i = 0; i < args.Length; i++)
		{
			if (optionNames.Contains(args[i]))
			{
				if (OptionTakesValue(args[i]))
					i++;
				continue;
			}

			if (!args[i].StartsWith("-", StringComparison.Ordinal))
				return args[i];
		}

		return null;
	}

	private static bool OptionTakesValue(string arg)
	{
		return !new[] { "-h", "--help", "-v", "--version", "-n", "--nologo", "-b", "--debug" }
			.Contains(arg, StringComparer.OrdinalIgnoreCase);
	}

	private static string GetVersion()
	{
		return typeof(Program).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
			?? typeof(Program).Assembly.GetName().Version?.ToString()
			?? "unknown";
	}
}
