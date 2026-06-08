using iorg;
using OsLib;
using RaiImage;
using Xunit;

namespace Iorg.Tests;

public class SourceImageStructuredTreeTests
{
	private const string Subscriber = "nomsa";
	private static readonly RaiRelPath SourceRelPath = new("TestAfricaStage/Images/NOMSA.net/");

	public static TheoryData<string, string, int, string, string> MixedSourceNames => new()
	{
		{ "nomsa-concert-11.jpg", "NomsaConcert", 11, "NomsaCon", "NomsaConce" },
		{ "SD-State-Sony-149.jpg", "SDStateSony", 149, "SDStateS", "SDStateSon" },
		{ "San-Diego-State-09.24-232.jpg", "SanDiegoState0924", 232, "SanDiego", "SanDiegoSt" },
		{ "SD-State-Fuji-unedited-87.jpg", "SDStateFujiUnedited", 87, "SDStateF", "SDStateFuj" },
	};

	[Theory]
	[MemberData(nameof(MixedSourceNames))]
	public void Organize_CopiesSourceThroughTempAndMovesStructuredNameInto8x2Tree(
		string sourceFileName,
		string expectedItemId,
		int expectedImageNumber,
		string expectedTopdir,
		string expectedSubdir)
	{
		var sourceRoot = GoogleDriveSourceRoot();
		var sourceFile = new RaiFile(sourceRoot, sourceFileName);
		Assert.True(sourceFile.Exists(), $"Missing test image: {sourceFile.FullName}");

		var testRoot = NewTestRoot();
		try
		{
			var subscriberRoot = testRoot / "dest" / Subscriber;
			var tempRoot = testRoot / "temp";
			using var output = new StringWriter();

			var count = ImageOrganizer.Organize(
				sourceRoot,
				subscriberRoot,
				Subscriber,
				sourceFileName,
				PathConventionType.ItemIdTree8x2,
				ImageNamingConvention.Structured,
				tempRoot,
				output);

			var expected = new ImageTreeFile(
				subscriberRoot,
				expectedItemId,
				string.Empty,
				sourceFile.Ext,
				PathConventionType.ItemIdTree8x2,
				ImageNamingConvention.Structured)
			{
				ImageNumber = expectedImageNumber
			};

			Assert.Equal(1, count);
			Assert.True(sourceFile.Exists(), $"Original source should remain untouched: {sourceFile.FullName}");
			Assert.True(expected.Exists(), $"Expected destination file: {expected.FullName}");
			Assert.Equal(expectedTopdir, Assert.Single(expected.Topdir.Segments));
			Assert.Equal(expectedSubdir, Assert.Single(expected.Subdir.Segments));
			Assert.Contains($"copied {sourceFile.FullName}", output.ToString());
			Assert.Contains($"=> {expected.FullName}", output.ToString());
		}
		finally
		{
			Cleanup(testRoot);
		}
	}

	private static RaiPath GoogleDriveSourceRoot()
	{
		var cloudDir = Os.Config?.Cloud?["GoogleDrive"];
		Assert.False(string.IsNullOrWhiteSpace((string?)cloudDir),
			"This test requires Os.Config.Cloud.GoogleDrive to be configured.");

		var root = new RaiPath((string)cloudDir) / SourceRelPath;
		Assert.True(root.Exists(), $"Missing GoogleDrive test image root: {root.FullPath}");
		return root;
	}

	private static RaiPath NewTestRoot()
	{
		var root = Os.TempDir / "RAIkeep" / "iorg-tests" / Guid.NewGuid().ToString("N");
		Cleanup(root);
		root.mkdir();
		return root;
	}

	private static void Cleanup(RaiPath root)
	{
		if (root.Exists())
			root.rmdir(depth: 8, deleteFiles: true);
	}
}
