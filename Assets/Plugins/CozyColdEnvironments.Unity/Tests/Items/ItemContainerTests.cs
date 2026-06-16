using CCEnvs.UnityX.Items;
using Moq;
using NUnit.Framework;

#nullable enable
namespace CCEnvs.UnityX.Tests
{
    [TestFixture]
    public class ItemContainerTests
    {
        private ItemContainer itemContainer = null!;

        [SetUp]
        public void Setup()
        {
            itemContainer = new ItemContainer();
        }

        [TearDown]
        public void TearDown()
        {
            itemContainer.Dispose();
            itemContainer = null!;
        }

        [Test]
        public void CheckPutItem()
        {
            Mock<IItem> itemMock = GetItemMock();
            IItem item = itemMock.Object;
            itemContainer.PutItem(item);
            Assert.AreEqual(item, itemContainer.Item);
        }

        [Test]
        public void CheckItemCount()
        {
            Mock<IItem> itemMock = GetItemMock();
            IItem item = itemMock.Object;
            ReadOnlyItemContainer restItems = itemContainer.PutItem(item, int.MaxValue);

            Assert.AreEqual(int.MaxValue - item.MaxItemCount, restItems.ItemCount);
            Assert.AreEqual(item.MaxItemCount, itemContainer.ItemCount);
        }

        [Test]
        public void CheckFreeSpace()
        {
            Mock<IItem> itemMock = GetItemMock();
            IItem item = itemMock.Object;
            int putItemCount = item.MaxItemCount / 2;
            itemContainer.PutItem(item, putItemCount);
            Assert.AreEqual(itemContainer.Capacity - putItemCount, itemContainer.FreeSpace);
        }

        [Test]
        public void CheckTakeItem()
        {
            Mock<IItem> itemMock = GetItemMock();
            IItem item = itemMock.Object;
            itemContainer.PutItem(item, int.MaxValue);

            int takeItemCount = int.MaxValue / 4;

            ReadOnlyItemContainer takenItems = itemContainer.TakeItem(takeItemCount);

            Assert.AreEqual(takeItemCount, takenItems.ItemCount);
            Assert.AreEqual(itemContainer.Capacity - takeItemCount, itemContainer.ItemCount);
        }

        private Mock<IItem> GetItemMock()
        {
            var itemMock = new Mock<IItem>();

            itemMock.Setup(item => item.Name).Returns("Cigarettes");
            itemMock.Setup(item => item.ID).Returns(1);
            itemMock.Setup(item => item.MaxItemCount).Returns(int.MaxValue / 2);

            return itemMock;
        }
    }
}
