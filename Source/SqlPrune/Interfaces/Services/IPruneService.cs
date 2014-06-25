﻿using System;
using System.Collections.Generic;
using Comsec.SqlPrune.Models;

namespace Comsec.SqlPrune.Interfaces.Services
{
    /// <summary>
    /// Interface for the pruning service (business logic that decides wether or not to prune a backup from a set).
    /// </summary>
    public interface IPruneService
    {
        /// <summary>
        /// Sets prunable backups in set.
        /// </summary>
        /// <param name="set">The set of backups for a given database.</param>
        /// <param name="now">The current date and time.</param>
        /// <returns></returns>
        void FlagPrunableBackupsInSet(IEnumerable<BakModel> set, DateTime now);
    }
}
