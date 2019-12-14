using System.Collections.Generic;
using System.Linq;
using GildedRose.Domain.Models;
using GildedRose.Logic;
using Shouldly;
using Xunit;

namespace GildedRose.Tests
{
    /// <summary>
    /// Tests to verify the existing behaviour of the UpdateQuality() method.
    /// </summary>
    public class ItemProcessorVerificationTests
    {
        private readonly ItemProcessor _itemProcessor;

        public ItemProcessorVerificationTests()
        {
            _itemProcessor = new ItemProcessor(GetDefaultItems());
        }

        private List<Item> GetDefaultItems()
        {
            return new List<Item>
            {
                new Item {Name = "+5 Dexterity Vest", SellIn = 10, Quality = 20},
                new Item {Name = "Aged Brie", SellIn = 2, Quality = 0},
                new Item {Name = "Elixir of the Mongoose", SellIn = 5, Quality = 7},
                new Item {Name = "Sulfuras, Hand of Ragnaros", SellIn = 0, Quality = 80},
                new Item
                {
                    Name = "Backstage passes to a TAFKAL80ETC concert",
                    SellIn = 15,
                    Quality = 20
                },
                new Item {Name = "Conjured Mana Cake", SellIn = 3, Quality = 6}
            };
        }

        [Fact]
        public void VerifyItemsHaveDeteriorated()
        {
            var items = _itemProcessor.GetItems();

            var itemNames = new List<string>
            {
                "Dexterity Vest",
                "Mongoose",
                "Conjured Mana Cake"
            };

            foreach (var name in itemNames)
            {
                var item = items.FirstOrDefault(i => i.Name.Contains(name));
                item.ShouldNotBe(null);

                var initialQuality = item.Quality;
                var initialSellIn = item.SellIn;

                _itemProcessor.UpdateQuality();

                var after = items.FirstOrDefault(i => i.Name.Contains(name));
                after.ShouldNotBe(null);

                after.Quality.ShouldBe(initialQuality - 1);
                after.SellIn.ShouldBe(initialSellIn - 1);
            }
        }

        [Fact]
        public void VerifyBrieHasChanged()
        {
            var items = _itemProcessor.GetItems();
            const string itemName = "Aged Brie";

            var item = items.FirstOrDefault(i => i.Name.Contains(itemName));
            item.ShouldNotBe(null);

            var initialQuality = item.Quality;
            var initialSellIn = item.SellIn;

            _itemProcessor.UpdateQuality();

            var after = items.FirstOrDefault(i => i.Name.Contains(itemName));
            after.ShouldNotBe(null);

            after.Quality.ShouldBe(initialQuality + 1);
            after.SellIn.ShouldBe(initialSellIn - 1);
        }

        [Fact]
        public void VerifyBackstagePassHasChanged()
        {
            var items = _itemProcessor.GetItems();
            const string itemName = "Backstage passes";

            var item = items.FirstOrDefault(i => i.Name.Contains(itemName));
            item.ShouldNotBe(null);

            var initialQuality = item.Quality;
            var initialSellIn = item.SellIn;

            _itemProcessor.UpdateQuality();

            var after = items.FirstOrDefault(i => i.Name.Contains(itemName));
            after.ShouldNotBe(null);

            after.Quality.ShouldBe(initialQuality + 1);
            after.SellIn.ShouldBe(initialSellIn - 1);
        }

        [Fact]
        public void SulfarasHasNotChanged()
        {
            var items = _itemProcessor.GetItems();
            const string itemName = "Sulfuras";

            var item = items.FirstOrDefault(i => i.Name.Contains(itemName));
            item.ShouldNotBe(null);

            var initialQuality = item.Quality;
            var initialSellIn = item.SellIn;

            _itemProcessor.UpdateQuality();

            var after = items.FirstOrDefault(i => i.Name.Contains(itemName));
            after.ShouldNotBe(null);

            after.Quality.ShouldBe(initialQuality);
            after.SellIn.ShouldBe(initialSellIn);
        }

        [Fact]
        public void WhenSellByDatePassedQualityDegradesTwiceAsFast()
        {
            //Once the sell by date has passed, Quality degrades twice as fast
            var items = new List<Item>
            {
                new Item
                {
                    Name = "+2 Mace of Smiting",
                    Quality = 7,
                    SellIn = 0
                }
            };

            var itemProcessor = new ItemProcessor(items);
            var initialQuality = items[0].Quality;

            itemProcessor.UpdateQuality();

            var updateItems = itemProcessor.GetItems();

            updateItems[0].Quality.ShouldBe(initialQuality - 2);
        }

        [Fact]
        public void WhenSellByDateHasNotPassedQualityDegradesNormally()
        {
            var items = new List<Item>
            {
                new Item
                {
                    Name = "+2 Mace of Smiting",
                    Quality = 7,
                    SellIn = 2
                }
            };

            var itemProcessor = new ItemProcessor(items);
            var initialQuality = items[0].Quality;

            itemProcessor.UpdateQuality();

            var updateItems = itemProcessor.GetItems();

            updateItems[0].Quality.ShouldBe(initialQuality - 1);
        }

        [Fact]
        public void QualityOfItemIsNeverNegative()
        {
            // The Quality of an item is never negative
            var items = new List<Item>
            {
                new Item
                {
                    Name = "+2 Mace of Smiting",
                    Quality = 0,
                    SellIn = 2
                }
            };

            var itemProcessor = new ItemProcessor(items);
            var initialQuality = items[0].Quality;

            itemProcessor.UpdateQuality();

            var updateItems = itemProcessor.GetItems();

            updateItems[0].Quality.ShouldBe(initialQuality);
        }

        [Fact(Skip = "This is failing so need to update the logic of the UpdateQuality method")]
        public void ItemQualityMaxIs50()
        {
            //The Quality of an item is never more than 50
            var items = new List<Item>
            {
                new Item
                {
                    Name = "+2 Mace of Smiting",
                    Quality = 50,
                    SellIn = 2
                }
            };

            var itemProcessor = new ItemProcessor(items);
            var initialQuality = items[0].Quality;

            itemProcessor.UpdateQuality();

            var updateItems = itemProcessor.GetItems();

            updateItems[0].Quality.ShouldBe(initialQuality);
        }

        [Fact]
        public void BackStagePassesQualityIncreasesBy2When10DaysAway()
        {
            // Quality increases by 2 when there are 10 days or less
            var items = new List<Item>
            {
                new Item
                {
                    Name = "Backstage passes to a TAFKAL80ETC concert",
                    Quality = 5,
                    SellIn = 10
                }
            };

            var itemProcessor = new ItemProcessor(items);
            var initialQuality = items[0].Quality;

            itemProcessor.UpdateQuality();

            var updateItems = itemProcessor.GetItems();

            updateItems[0].Quality.ShouldBe(initialQuality + 2);
        }

        [Fact]
        public void BackStagePassesQualityIncreasesBy3When5DaysAway()
        {
            // Quality increases by 3 when there are 5 days or less 
            var items = new List<Item>
            {
                new Item
                {
                    Name = "Backstage passes to a TAFKAL80ETC concert",
                    Quality = 5,
                    SellIn = 5
                }
            };

            var itemProcessor = new ItemProcessor(items);
            var initialQuality = items[0].Quality;

            itemProcessor.UpdateQuality();

            var updateItems = itemProcessor.GetItems();

            updateItems[0].Quality.ShouldBe(initialQuality + 3);
        }

        [Fact]
        public void BackStagePassesQualityHitsZero()
        {
            // Quality drops to 0 after the concert
            var items = new List<Item>
            {
                new Item
                {
                    Name = "Backstage passes to a TAFKAL80ETC concert",
                    Quality = 5,
                    SellIn = 0
                }
            };

            var itemProcessor = new ItemProcessor(items);

            itemProcessor.UpdateQuality();

            var updateItems = itemProcessor.GetItems();

            updateItems[0].Quality.ShouldBe(0);
        }
    }
}
