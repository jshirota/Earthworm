using System.Collections.Generic;

namespace Earthworm
{
    internal static class Utility
    {
        #region Sequence

        public static IEnumerable<List<T>> Partition<T>(this IEnumerable<T> items, int size)
        {
            List<T> batch = new List<T>();

            foreach (T item in items)
            {
                batch.Add(item);

                if (batch.Count == size)
                {
                    yield return batch;
                    batch.Clear();
                }
            }

            if (batch.Count > 0)
                yield return batch;
        }

        #endregion
    }
}
