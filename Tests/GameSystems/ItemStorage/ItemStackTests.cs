using NUnit.Framework;
using UTIRLib.GameSystems.ItemStorageSystem;

#nullable enable
namespace UTIRLib.Tests.GameSystems.Storage
{
    public class ItemStackTests
    {
        private ItemStack itemStack = null!;

        [SetUp]
        public void Setup()
        {
            itemStack = new ItemStack();
        }

        [Test]
        public void AddItems()
        {
            var toSetItem = new TestItem();
            int toAddCount = 100;
            itemStack.AddItem(toSetItem, toAddCount);

            Assert.AreEqual(toSetItem, itemStack.Item, nameof(itemStack.Item));
            Assert.AreEqual(toAddCount, itemStack.ItemCount, nameof(itemStack.ItemCount));
            Assert.False(!itemStack.HasItem);
        }

        [Test]
        public void Clear()
        {
            itemStack.AddItem(new TestItem(), 100);
            itemStack.Clear();

            Assert.True(!itemStack.HasItem);
        }

        [TearDown]
        public void TearDown()
        {
            itemStack = null!;
        }
    }
}
