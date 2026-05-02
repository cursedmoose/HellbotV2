namespace Hellbot.UI.Components.Extensions
{
    public static class LinqExtensions
    {
        public static IEnumerable<T> OrderByDynamic<T>(
            this IEnumerable<T> source,
            string property,
            bool asc)
        {
            var prop = typeof(T).GetProperty(property);

            return asc
                ? source.OrderBy(x => prop!.GetValue(x, null))
                : source.OrderByDescending(x => prop!.GetValue(x, null));
        }
    }
}
