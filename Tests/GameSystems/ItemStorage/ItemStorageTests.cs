using NUnit.Framework;
using System;
using UTIRLib.GameSystems.Storage;

#nullable enable
namespace UTIRLib.Tests.GameSystems.Storage
{
    public class ItemStorageTests
    {
        private ItemStorage<ItemSlot> itemStorage = null!;

        [SetUp]
        public void Setup()
        {
            var slots = new ItemSlot[10];

            slots.Fill(new ItemSlot(new ItemStack<IItem>()));

            itemStorage = new ItemStorage<ItemSlot>(slots);
        }

        [Test]
        [TestCase(0)]
        [TestCase(100)]
        [TestCase(int.MaxValue)]
        [TestCase(int.MinValue)]
        public void AddItemsTest(int count)
        {
            var item = new TestItem();

            if (count <= 0)
            {
                Assert.Throws<ArgumentException>(() => itemStorage.AddItem(item, count));
                return;
            }
            else
                itemStorage.AddItem(item, count);


            Assert.True(itemStorage.HasItem(item, count));
        }

        [TearDown]
        public void TearDown()
        {
            itemStorage = null!;
        }
    }
}
