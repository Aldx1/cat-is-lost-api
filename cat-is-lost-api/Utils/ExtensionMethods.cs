namespace cat_is_lost_api.Utils
{
    public static class ExtensionMethods
    {
        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> collection)
        {
            if (collection == null) return Enumerable.Empty<T>();
            return collection;
        }
    }
}
