using CCEnvs.UnityX.Items;
using Moq;
using NUnit.Framework;

#nullable enable
namespace CCEnvs.UnityX.Tests
{
    [TestFixture]
    public class ItemContainerTests
    {
        private ItemContainer container = null!;

        private Mock<IItem> cigarettesMock = null!;

        private IItem cigarettes => cigarettesMock.Object;

        [SetUp]
        public void Setup()
        {
            container = new ItemContainer();
            cigarettesMock = GetItemMock();
        }

        [TearDown]
        public void TearDown()
        {
            container.Dispose();
            container = null!;
            cigarettesMock = null!;
        }

        [Test]
        public void CheckPutItem()
        {
            container.PutItem(cigarettes);
            Assert.AreEqual(cigarettes, container.Item);
        }

        [Test]
        public void CheckItemCount()
        {
            const int PUT_ITEM_COUNT = 15000;
            ReadOnlyItemContainer restItems = container.PutItem(cigarettes, PUT_ITEM_COUNT);

            Assert.AreEqual(0, restItems.ItemCount);
            Assert.AreEqual(PUT_ITEM_COUNT, container.ItemCount);
        }

        [Test]
        public void CheckFreeSpace()
        {
            int putItemCount = cigarettes.MaxItemCount / 2;
            container.PutItem(cigarettes, putItemCount);
            Assert.AreEqual(container.Capacity - putItemCount, container.FreeSpace);
        }

        [Test]
        public void CheckTakeItem()
        {
            const int PUT_ITEM_COUNT = 15000;
            const int TAKE_ITEM_COUNT = 15000;

            container.PutItem(cigarettes, PUT_ITEM_COUNT);
            ReadOnlyItemContainer takenItems = container.TakeItem(TAKE_ITEM_COUNT);

            Assert.AreEqual(TAKE_ITEM_COUNT, takenItems.ItemCount);
            Assert.AreEqual(PUT_ITEM_COUNT - TAKE_ITEM_COUNT, container.ItemCount);
        }

        [Test]
        public void CheckIcon()
        {
            container.PutItem(cigarettes, 1500);

            Assert.IsNotNull(container.Item);
            Assert.AreEqual(UCC.TransparentSprite, container.Item.IfNotNull(x => x.Icon));

            container.TakeItem();
            Assert.AreEqual(UCC.TransparentSprite, container.Item.IfNotNull(x => x.Icon));
        }

        private Mock<IItem> GetItemMock()
        {
            var itemMock = new Mock<IItem>();

            itemMock.Setup(item => item.Name).Returns("Cigarettes");
            itemMock.Setup(item => item.ID).Returns(1);
            itemMock.Setup(item => item.MaxItemCount).Returns(int.MaxValue / 2);
            itemMock.Setup(item => item.Icon).Returns(UCC.TransparentSprite);

            return itemMock;
        }
    }
}
