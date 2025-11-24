namespace Argo_Books.Classes
{
    public static class CollectionExtensions
    {
        /// <summary>
        /// Removes multiple items from a collection.
        /// </summary>
        public static void RemoveRange<T>(this ICollection<T> collection, IEnumerable<T> itemsToRemove)
        {
            foreach (T? item in itemsToRemove)
            {
                collection.Remove(item);
            }
        }

        /// <summary>
        /// Safely removes multiple rows from a DataGridView row collection, only if they exist.
        /// </summary>
        public static void RemoveRowsIfExists(this DataGridViewRowCollection rows, IEnumerable<DataGridViewRow> rowsToRemove)
        {
            List<DataGridViewRow> rowsToRemoveList = rowsToRemove.ToList();

            for (int i = rowsToRemoveList.Count - 1; i >= 0; i--)
            {
                if (rows.Contains(rowsToRemoveList[i]))
                {
                    rows.Remove(rowsToRemoveList[i]);
                }
            }
        }
    }
}