using CCEnvs.UnityX.Items;
using Moq;
using NUnit.Framework;

#nullable enable
namespace CCEnvs.Unity.Tests
{
    [TestFixture]
    public class InventoryTests
    {
        private const int PUT_ITEM_COUNT = 15000;

        private Inventory inventory = null!;

        private Mock<IItem> cigarettes = null!;

        private IItem Cigarettes => cigarettes.Object;

        [SetUp]
        public void Setup()
        {
            inventory = new Inventory()
            {
                ContainerSample = new ItemContainer()
            };

            cigarettes = GetItemMock();
        }

        [TearDown]
        public void TearDown()
        {
            inventory.Dispose();
            inventory = null!;
            cigarettes = null!;
        }

        [Test]
        public void CheckPutItem()
        {
            inventory.InstantiateContainers(2);
            LargeReadOnlyItemContainer restItems = inventory.PutItem(Cigarettes, PUT_ITEM_COUNT);

            Assert.AreEqual(0, restItems.ItemCount);
            Assert.AreEqual(PUT_ITEM_COUNT, inventory.GetItemCount(Cigarettes));
        }

        [Test]
        public void CheckGetFreeSpace()
        {
            inventory.InstantiateContainers(2);
            inventory.PutItem(Cigarettes, PUT_ITEM_COUNT);

            Assert.AreEqual(Cigarettes.MaxItemCount * inventory.ContainerCount - PUT_ITEM_COUNT, inventory.GetFreeSpace(Cigarettes));
        }

        [Test]
        public void CheckAutoSize()
        {
            inventory.AutoSize = true;
            LargeReadOnlyItemContainer restItems = inventory.PutItem(Cigarettes, PUT_ITEM_COUNT);

            Assert.AreEqual(0, restItems.ItemCount);
            Assert.AreEqual(PUT_ITEM_COUNT, inventory.GetItemCount(Cigarettes));
        }

        [Test]
        public void CheckTakeItem()
        {
            inventory.AutoSize = true;
            inventory.PutItem(Cigarettes, PUT_ITEM_COUNT);
            LargeReadOnlyItemContainer takenItems = inventory.TakeItem(Cigarettes, PUT_ITEM_COUNT / 2);

            Assert.AreEqual(PUT_ITEM_COUNT / 2, takenItems.ItemCount);
        }

        [Test]
        public void CheckIsEmpty()
        {
            Assert.IsTrue(inventory.IsEmpty);

            inventory.AutoSize = true;
            inventory.PutItem(Cigarettes, PUT_ITEM_COUNT);

            Assert.IsFalse(inventory.IsEmpty);
        }

        [Test]
        public void CheckIsFull()
        {
            Assert.IsTrue(inventory.IsFull);

            inventory.InstantiateContainers(1);
            Assert.IsFalse(inventory.IsFull);

            inventory.PutItem(Cigarettes, Cigarettes.MaxItemCount);
            Assert.IsTrue(inventory.IsFull);
        }

        [Test]
        public void CheckGetItemCount()
        {
            inventory.InstantiateContainers(2);
            inventory.PutItem(Cigarettes, Cigarettes.MaxItemCount * inventory.ContainerCount);

            long inventoryItemCount = inventory.GetItemCount(Cigarettes);
            Assert.AreEqual(Cigarettes.MaxItemCount * inventory.ContainerCount, inventoryItemCount);

        }

        [Test]
        public void CheckContainsItem()
        {
            inventory.InstantiateContainers(4);
            inventory.PutItem(Cigarettes, Cigarettes.MaxItemCount * inventory.ContainerCount);

            Assert.IsTrue(inventory.ContainsItem(Cigarettes, Cigarettes.MaxItemCount * inventory.ContainerCount));
        }

        [Test]
        public void CheckCanPut()
        {
            Assert.IsFalse(inventory.CanPutItem(Cigarettes));

            inventory.InstantiateContainers(1);

            Assert.IsTrue(inventory.CanPutItem(Cigarettes));
        }

        private Mock<IItem> GetItemMock()
        {
            var itemMock = new Mock<IItem>();

            itemMock.Setup(item => item.Name).Returns("Cigarettes");
            itemMock.Setup(item => item.ID).Returns(1);
            itemMock.Setup(item => item.MaxItemCount).Returns(short.MaxValue);

            return itemMock;
        }
    }
}
