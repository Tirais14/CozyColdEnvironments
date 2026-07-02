using CCEnvs.UnityX.Items;
using Moq;
using NUnit.Framework;

#nullable enable
namespace CCEnvs.Unity.Tests
{
    [TestFixture]
    public class InventoryTests
    {
        private Inventory inventory = null!;

        [SetUp]
        public void Setup()
        {
            inventory = new Inventory()
            {
                ContainerSample = new ItemContainer()
            };
        }

        [TearDown]
        public void TearDown()
        {
            inventory.Dispose();
            inventory = null!;
        }

        [Test]
        public void CheckPutItem()
        {
            Mock<IItem> itemMock = GetItemMock();
            IItem item = itemMock.Object;

            inventory.InstantiateContainers(2);
            ReadOnlyItemContainer restItems = inventory.PutItem(item, int.MaxValue);

            Assert.AreEqual(0, restItems.ItemCount);
            Assert.AreEqual(int.MaxValue, inventory.GetItemCount(item));
        }

        [Test]
        public void CheckGetFreeSpace()
        {
            Mock<IItem> itemMock = GetItemMock();
            IItem item = itemMock.Object;

            inventory.InstantiateContainers(2);
            inventory.PutItem(item, item.MaxItemCount);

            Assert.AreEqual(item.MaxItemCount, inventory.GetFreeSpace(item));
        }

        [Test]
        public void CheckAutoSize()
        {
            Mock<IItem> itemMock = GetItemMock();
            IItem item = itemMock.Object;

            inventory.AutoSize = true;
            ReadOnlyItemContainer restItems = inventory.PutItem(item, int.MaxValue);

            Assert.AreEqual(0, restItems.ItemCount);
            Assert.AreEqual(int.MaxValue, inventory.GetItemCount(item));
        }

        [Test]
        public void CheckTakeItem()
        {
            Mock<IItem> itemMock = GetItemMock();
            IItem item = itemMock.Object;

            inventory.AutoSize = true;
            inventory.PutItem(item, int.MaxValue);
            ReadOnlyItemContainer takenItems = inventory.TakeItem(item, int.MaxValue / 2);

            Assert.AreEqual(int.MaxValue / 2, takenItems.ItemCount);
        }

        [Test]
        public void CheckFreeSpace()
        {
            Mock<IItem> itemMock = GetItemMock();
            IItem item = itemMock.Object;

            inventory.AutoSize = true;
            inventory.PutItem(item, 60);

            Assert.AreEqual(item.MaxItemCount - 60, inventory.FreeSpace);
        }

        private Mock<IItem> GetItemMock()
        {
            var itemMock = new Mock<IItem>();

            itemMock.Setup(item => item.Name).Returns("Cigarettes");
            itemMock.Setup(item => item.ID).Returns(1);
            itemMock.Setup(item => item.MaxItemCount).Returns(int.MaxValue / 2 + 1);

            return itemMock;
        }
    }
}
