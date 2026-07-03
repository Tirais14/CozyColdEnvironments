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

            const int PUT_ITEM_COUNT = 15000;
            ReadOnlyItemContainer restItems = itemContainer.PutItem(item, PUT_ITEM_COUNT);

            Assert.AreEqual(0, restItems.ItemCount);
            Assert.AreEqual(PUT_ITEM_COUNT, itemContainer.ItemCount);
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

            const int PUT_ITEM_COUNT = 15000;
            const int TAKE_ITEM_COUNT = 15000;

            itemContainer.PutItem(item, PUT_ITEM_COUNT);
            ReadOnlyItemContainer takenItems = itemContainer.TakeItem(TAKE_ITEM_COUNT);

            Assert.AreEqual(TAKE_ITEM_COUNT, takenItems.ItemCount);
            Assert.AreEqual(PUT_ITEM_COUNT - TAKE_ITEM_COUNT, itemContainer.ItemCount);
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
