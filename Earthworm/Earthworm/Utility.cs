using System.Collections.Generic;

namespace Earthworm
{
    internal static class Utility
    {
        #region Sequence

        public static IEnumerable<List<T>> Partition<T>(this IEnumerable<T> items, int size)
        {
            var list = new List<T>();

            foreach (var item in items)
            {
                list.Add(item);

                if (list.Count == size)
                {
                    yield return list;
                    list = new List<T>();
                }
            }

            if (list.Count > 0)
                yield return list;
        }

        #endregion
    }
}
