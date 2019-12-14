using System;
using System.Collections.Generic;
using GildedRose.Domain;
using GildedRose.Domain.Models;

namespace GildedRose.Logic
{
    public class ItemProcessor
    {
        private readonly List<Item> _items;

        public ItemProcessor(List<Item> items)
        {
            _items = items;
        }

        public void ListItems()
        {
            foreach (var item in _items)
            {
                Console.WriteLine("Item: {0}\nQuality: {1}\nSellIn: {2}", item.Name, item.Quality, item.SellIn);
            }
        }

        public List<Item> GetItems()
        {
            return _items;
        }

        public void UpdateQuality()
        {
            foreach (var item in _items)
            {
                UpdateItemQuality(item);

                UpdateItemSellIn(item);
            }
        }

        private void UpdateItemQuality(Item item)
        {
            if (item.Quality == Constants.Quality.Maximum)
            {
                return;
            }

            if (item.Name == Constants.ProductNames.AgedBrie)
            {
                item.IncrementQualityBy(1);

                if (item.SellIn < 0 && item.Quality < 50)
                {
                    item.IncrementQualityBy(1);
                }

                return;
            }

            if (item.Name == Constants.ProductNames.BackStagePass)
            {
                if (item.SellIn <= Constants.Quality.BackStagePass5DayCutOff)
                {
                    item.IncrementQualityBy(3);
                }
                else if (item.SellIn <= Constants.Quality.BackStagePass10DayCutOff)
                {
                    item.IncrementQualityBy(2);
                }
                else
                {
                    item.IncrementQualityBy(1);
                }

                return;
            }

            if (item.Name != Constants.ProductNames.Sulfaras)
            {
                item.DecrementQualityBy(1);

                if (item.Name.Contains("Conjured"))
                {
                    item.DecrementQualityBy(1);
                }
            }
        }

        private void UpdateItemSellIn(Item item)
        {
            if (item.Name != Constants.ProductNames.Sulfaras)
            {
                item.SellIn -= 1;
            }

            if (item.SellIn >= 0)
            {
                return;
            }

            if (item.Name == Constants.ProductNames.BackStagePass)
            {
                item.DecrementQualityBy(item.Quality);
                return;
            }

            if (item.Name != Constants.ProductNames.Sulfaras)
            {
                item.Quality -= 1;
            }
        }
    }
}
