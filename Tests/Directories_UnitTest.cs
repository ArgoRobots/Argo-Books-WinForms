using Argo_Books.Classes;

namespace Tests
{
    [TestClass]
    public class Directories_UnitTest
    {
        private string _testDirectory = "";

        [TestInitialize]
        public void Setup()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), "TestDirectory");
            Directories.CreateDirectory(_testDirectory);
        }

        [TestCleanup]
        public void Cleanup()
        {
            Directories.DeleteDirectory(_testDirectory, true);
        }

        [TestMethod]
        public void TestCreateDirectory()
        {
            string dir = Path.Combine(_testDirectory, "NewDirectory");
            bool result = Directories.CreateDirectory(dir);
            Assert.IsTrue(result, "Directory should be created.");
            Assert.IsTrue(Directory.Exists(dir), "Directory should exist.");
        }

        [TestMethod]
        public void TestCreateFile()
        {
            string file = Path.Combine(_testDirectory, "TestFile.txt");
            bool result = Directories.CreateFile(file);
            Assert.IsTrue(result, "File should be created.");
            Assert.IsTrue(File.Exists(file), "File should exist.");
        }

        [TestMethod]
        public void TestCopyFile()
        {
            string sourceFile = Path.Combine(_testDirectory, "TestFile.txt");
            Directories.CreateFile(sourceFile);

            string destinationFile = Path.Combine(_testDirectory, "CopiedFile.txt");
            bool result = Directories.CopyFile(sourceFile, destinationFile);
            Assert.IsTrue(result, "File should be copied.");
            Assert.IsTrue(File.Exists(destinationFile), "Copied file should exist.");
        }

        [TestMethod]
        public void TestCopyFileOverwrite()
        {
            string sourceFile = Path.Combine(_testDirectory, "TestFile.txt");
            Directories.CreateFile(sourceFile);

            string destinationFile = Path.Combine(_testDirectory, "CopiedFile.txt");
            // Create destination file to test overwrite behaviour
            Directories.CreateFile(destinationFile);

            bool result = Directories.CopyFile(sourceFile, destinationFile, true);
            Assert.IsTrue(result, "File copy with overwrite should succeed.");
            Assert.IsTrue(File.Exists(destinationFile), "Destination file should still exist after overwrite.");
        }

        [TestMethod]
        public void TestDeleteFile()
        {
            string file = Path.Combine(_testDirectory, "TestFile.txt");
            Directories.CreateFile(file);

            bool result = Directories.DeleteFile(file);
            Assert.IsTrue(result, "File should be deleted.");
            Assert.IsFalse(File.Exists(file), "File should not exist.");
        }

        [TestMethod]
        public void TestMoveFile()
        {
            string sourceFile = Path.Combine(_testDirectory, "TestFile.txt");
            Directories.CreateFile(sourceFile);

            string destinationFile = Path.Combine(_testDirectory, "MovedFile.txt");
            bool result = Directories.MoveFile(sourceFile, destinationFile);
            Assert.IsTrue(result, "File should be moved.");
            Assert.IsFalse(File.Exists(sourceFile), "Source file should not exist.");
            Assert.IsTrue(File.Exists(destinationFile), "Destination file should exist.");
        }

        [TestMethod]
        public void TestRenameFolder()
        {
            string newFolder = Path.Combine(_testDirectory, "RenamedFolder");
            Directories.CreateDirectory(newFolder);
            Directories.RenameFolder(newFolder, "RenamedFolder1");

            Assert.IsTrue(Directory.Exists(Path.Combine(_testDirectory, "RenamedFolder1")), "Folder should be renamed.");
        }

        [TestMethod]
        public void TestGetListOfAllFilesWithoutExtensionInDirectory()
        {
            string file1 = Path.Combine(_testDirectory, "File1.txt");
            string file2 = Path.Combine(_testDirectory, "File2.txt");
            Directories.CreateFile(file1);
            Directories.CreateFile(file2);

            List<string> fileNames = Directories.GetListOfAllFilesWithoutExtensionInDirectory(_testDirectory);
            Assert.AreEqual(2, fileNames.Count, "There should be two files.");
            Assert.IsTrue(fileNames.Contains("File1"), "File1 should be listed.");
            Assert.IsTrue(fileNames.Contains("File2"), "File2 should be listed.");
        }

        [TestMethod]
        public void TestGetListOfAllDirectoriesInDirectory()
        {
            string dir1 = Path.Combine(_testDirectory, "Dir1");
            string dir2 = Path.Combine(_testDirectory, "Dir2");
            Directories.CreateDirectory(dir1);
            Directories.CreateDirectory(dir2);

            List<string> dirNames = Directories.GetListOfAllDirectoriesInDirectory(_testDirectory);
            Assert.AreEqual(2, dirNames.Count, "There should be two directories.");
        }
    }
}