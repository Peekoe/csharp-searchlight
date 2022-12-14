﻿using System.Collections.Generic;

namespace Searchlight.Caching
{
    /// <summary>
    /// Keeps track of a collection
    /// </summary>
    /// <typeparam name="ITEM"></typeparam>
    public class ListCache<ITEM> : ObjectCache<List<ITEM>>
    {
        /// <summary>
        /// Shortcut function for getting all items
        /// </summary>
        /// <returns></returns>
        public virtual List<ITEM> GetAll()
        {
            return Get();
        }
    }
}
