﻿using NUnit.Framework;

namespace Comsec.SqlPrune.Services.Providers
{
    [TestFixture]
    public class LocalFilesystemProviderTest
    {
        private LocalFileSystemProvider provider;

        [SetUp]
        public void Setup()
        {
            provider = new LocalFileSystemProvider();
        }

        [Test]
        public void TestShouldRun()
        {
            Assert.IsTrue(provider.ShouldRun("d:"));
            Assert.IsTrue(provider.ShouldRun(@"\folder\on\current\drive"));
            Assert.IsTrue(provider.ShouldRun(@"d:\"));
            Assert.IsTrue(provider.ShouldRun(@"d:\folder"));
        }

        [Test]
        public void TestShouldNotRun()
        {
            Assert.IsTrue(provider.ShouldRun(@"\\network-share\folder"));
            Assert.IsTrue(provider.ShouldRun(@"smb://boo"));
            Assert.IsTrue(provider.ShouldRun(@"s3://boo"));
            Assert.IsTrue(provider.ShouldRun(@"ftp://boo"));
            Assert.IsTrue(provider.ShouldRun(@"http://boo"));
        }

        [Test]
        public void TestExtractFilenameFromPath()
        {
            var result = provider.ExtractFilenameFromPath(@"E:\folder\sub\file.ext");

            Assert.AreEqual("file.ext", result);
        }

        [Test]
        public void TestGetFileSizeWhenFileDoesNotExist()
        {
            var result = provider.GetFileSize(@"C:\this\folder\should\not\exists\and\this\file.either");

            Assert.AreEqual(-1, result);
        }
    }
}
