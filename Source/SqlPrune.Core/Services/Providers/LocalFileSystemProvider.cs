﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Comsec.SqlPrune.Extensions;
using Comsec.SqlPrune.Interfaces.Services.Providers;
using Sugar.Extensions;

namespace Comsec.SqlPrune.Services.Providers
{
    /// <summary>
    /// Wrapper service for access to <see cref="System.IO.File"/> and <see cref="System.IO.Directory"/>
    /// </summary>
    public class LocalFileSystemProvider : IFileProvider
    {
        /// <summary>
        /// Method called by the command to determine which <see cref="IFileProvider" /> implemetation should run.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public bool ShouldRun(string path)
        {
            return path != null && (!path.Contains("://") || !path.StartsWith(@"\\"));
        }

        /// <summary>
        /// Extracts the filename from path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public string ExtractFilenameFromPath(string path)
        {
            return path.SubstringAfterLastChar(@"\");
        }

        /// <summary>
        /// Determines whether the specified path is a directory.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public bool IsDirectory(string path)
        {
            try
            {
                var pathAttributes = File.GetAttributes(path);

                return (pathAttributes & FileAttributes.Directory) == FileAttributes.Directory;
            }
            catch(DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }
            catch(FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }

            return false;
        }

        /// <summary>
        /// Gets the size of the file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public long GetFileSize(string path)
        {
            try
            {
                var info = new FileInfo(path);

                return info.Length;
            }
            catch (FileNotFoundException)
            {
                return -1;
            }
        }

        /// <summary>
        /// Returns the names of files (including their paths) that match the specified search pattern in the specified directory.
        /// </summary>
        /// <param name="dirPath">The directory to search.</param>
        /// <param name="searchPattern">The search patter (e.g. "*.txt").</param>
        /// <returns>
        /// A dictionary listing each file found and its size (in bytes).
        /// </returns>
        /// <remarks>
        /// System Files and Folders will be ignored
        /// </remarks>
        public IDictionary<string, long> GetFiles(string dirPath, params string[] searchPattern)
        {
            var info = new DirectoryInfo(dirPath);

            var matchingFiles = WalkDirectory(info, searchPattern);

            var result = new Dictionary<string, long>(matchingFiles.Count);

            foreach (var filename in matchingFiles)
            {
                var fileInfo = new FileInfo(filename);

                result.Add(filename, fileInfo.Length);
            }

            return result;
        }

        /// <summary>
        /// Recursive method to walks the directory including any subdirectories and return all files found matching the given <see cref="searchPattern"/>.
        /// </summary>
        /// <param name="root">The root.</param>
        /// <param name="searchPatterns">The search patterns (e.g. "*.bak").</param>
        /// <returns></returns>
        /// <remarks>
        /// Inspired from code example on http://msdn.microsoft.com/en-us/library/bb513869.aspx
        /// </remarks>
        private static IList<string> WalkDirectory(DirectoryInfo root, params string[] searchPatterns)
        {
            var result = new List<string>();

            if (root.Name != "$RECYCLE.BIN")
            {
                try
                {
                    var files = root.EnumerateFiles()
                                    .MatchOnAny(x => x.Name, searchPatterns);

                    result.AddRange(files.Select(x => x.Directory + (x.DirectoryName.EndsWith(@"\") ? null : @"\") + x.Name));
                }
                catch (UnauthorizedAccessException e)
                {
                    Console.WriteLine(e.Message);
                }
                catch (DirectoryNotFoundException e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            try
            {
                var subDirs = root.GetDirectories();
                foreach (var dirInfo in subDirs)
                {
                    var filesInSubDirectory = WalkDirectory(dirInfo, searchPatterns);

                    result.AddRange(filesInSubDirectory);
                }
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }
            
            return result;
        }
        
        /// <summary>
        /// Deletes the specified file.
        /// </summary>
        /// <param name="path">The path.</param>
        public void Delete(string path)
        {
            File.Delete(path);
        }

        /// <summary>
        /// Copies to local.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="destination">The destination path.</param>
        public void CopyToLocal(string path, string destination)
        {
            File.Copy(path, destination);
        }

        /// <summary>
        /// Copies to local asynchronously.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="destinationPath">The destination path.</param>
        /// <returns></returns>
        public async Task CopyToLocalAsync(string path, string destinationPath)
        {
            await Task.Factory.StartNew(() => CopyToLocal(path, destinationPath));
        }
    }
}
