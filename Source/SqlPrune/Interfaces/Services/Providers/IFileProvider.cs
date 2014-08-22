﻿using System.Collections.Generic;

namespace Comsec.SqlPrune.Interfaces.Services.Providers
{
    /// <summary>
    /// Interface to wrap call to a local or remote file system.
    /// </summary>
    public interface IFileProvider
    {
        /// <summary>
        /// Method called by the command to determine which <see cref="IFileProvider"/> implemetation should run.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        bool ShouldRun(string path);

        /// <summary>
        /// Determines whether the specified path is a directory.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        bool IsDirectory(string path);

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
        IDictionary<string, long> GetFiles(string dirPath, string searchPattern);

        /// <summary>
        /// Deletes the specified file.
        /// </summary>
        /// <param name="path">The path.</param>
        void Delete(string path);

        /// <summary>
        /// Copies the file at the given <see cref="path"/> and to a specified specified local <see cref="destinationFolder"/>.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="destinationFolder">The destination folder.</param>
        void CopyToLocal(string path, string destinationFolder);
    }
}
