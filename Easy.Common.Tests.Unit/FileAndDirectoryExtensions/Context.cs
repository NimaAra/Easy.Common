namespace Easy.Common.Tests.Unit.FileAndDirectoryExtensions;

using System.IO;
using Easy.Common.Extensions;

public class Context
{
    protected bool ResultOne;
    protected bool ResultTwo;
    private string _tmpHiddenFile;
    private string _tmpNonHiddenFile;
        
    private DirectoryInfo _tmpHiddenDirectory;
    private DirectoryInfo _tmpNonHiddenDirectory;
        
    protected void Given_a_temp_hidden_file()
    {
        _tmpHiddenFile = Path.GetTempFileName();
        File.SetAttributes(_tmpHiddenFile, FileAttributes.Hidden);
    }

    protected void Given_a_temp_non_hidden_file()
    {
        _tmpNonHiddenFile = Path.GetTempFileName();
        File.SetAttributes(_tmpNonHiddenFile, FileAttributes.Normal);
    }

    protected void When_checking_if_the_files_are_hidden()
    {
        var fileInfoOne = new FileInfo(_tmpHiddenFile);
        var fileInfoTwo = new FileInfo(_tmpNonHiddenFile);
            
        ResultOne = fileInfoOne.IsHidden();
        ResultTwo = fileInfoTwo.IsHidden();
    }

    protected void Given_a_temp_hidden_directory()
    {
        var baseDir = Path.GetTempPath();
        var dirName = Path.GetRandomFileName();

        var tmpDir = Directory.CreateDirectory(Path.Combine(baseDir, dirName));
        tmpDir.Attributes = FileAttributes.Directory | FileAttributes.Hidden;

        _tmpHiddenDirectory = tmpDir;
    }
        
    protected void Given_a_temp_non_hidden_directory()
    {
        var baseDir = Path.GetTempPath();
        var dirName = Path.GetRandomFileName();

        var tmpDir = Directory.CreateDirectory(Path.Combine(baseDir, dirName));
        tmpDir.Attributes = FileAttributes.Directory | FileAttributes.Normal;

        _tmpNonHiddenDirectory = tmpDir;
    }

    protected void When_checking_if_the_directories_are_hidden()
    {
        ResultOne = _tmpHiddenDirectory.IsHidden();
        ResultTwo = _tmpNonHiddenDirectory.IsHidden();
    }
}