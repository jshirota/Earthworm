using System.Collections.Generic;
using System.Linq;

namespace Earthworm
{
    internal static class Utility
    {
        #region Sequence

        public static IEnumerable<IEnumerable<T>> Partition<T>(this IEnumerable<T> items, int size)
        {
            return items
                .Select((item, i) => new { group = i / size, item })
                .GroupBy(o => o.group, o => o.item)
                .Select(g => g.Select(o => o));
        }

        #endregion
    }
}
