using GildedRose.Domain.Models;

namespace GildedRose.Domain
{
    public static class ExtensionMethods
    {
        public static void IncrementQualityBy(this Item item, int value)
        {
            item.Quality += value;

            if (item.Quality > 50)
            {
                item.Quality = 50;
            }
        }

        public static void DecrementQualityBy(this Item item, int value)
        {
            item.Quality -= value;

            if (item.Quality < 0)
            {
                item.Quality = 0;
            }
        }
    }
}
