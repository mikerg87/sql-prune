﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Comsec.SqlPrune.Interfaces.Services;

namespace Comsec.SqlPrune.Services
{
    /// <summary>
    /// Wrapper service for access to <see cref="System.IO.File"/> and <see cref="System.IO.Directory"/>
    /// </summary>
    public class FileService : IFileService
    {
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
            catch(FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }

            return false;
        }

        /// <summary>
        /// Returns the names of files (including their paths) that match the specified search pattern in the specified directory.
        /// </summary>
        /// <param name="dirPath">The directory to search.</param>
        /// <param name="searchPattern">The search patter (e.g. "*.txt").</param>
        /// <returns>
        /// A list of files.
        /// </returns>
        /// <remarks>
        /// System Files and Folders will be ignored
        /// </remarks>
        public IList<string> GetFiles(string dirPath, string searchPattern)
        {
            var info = new DirectoryInfo(dirPath);

            var result = WalkDirectory(info, "*.bak");

            return result;
        }

        /// <summary>
        /// Recursive method to walks the directory including any subdirectories and return all files found matching the given <see cref="searchPattern"/>.
        /// </summary>
        /// <param name="root">The root.</param>
        /// <param name="searchPattern">The search pattern.</param>
        /// <returns></returns>
        /// <remarks>
        /// Inspired from code example on http://msdn.microsoft.com/en-us/library/bb513869.aspx
        /// </remarks>
        private static IList<string> WalkDirectory(DirectoryInfo root, string searchPattern)
        {
            var result = new List<string>();

            if (root.Name != "$RECYCLE.BIN")
            {
                try
                {
                    var files = root.GetFiles(searchPattern);

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
                    var filesInSubDirectory = WalkDirectory(dirInfo, searchPattern);

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
    }
}
