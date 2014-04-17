using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.Order;
using CoinExchange.Trades.Domain.Model.OrderMatchingEngine;
using NUnit.Framework;

namespace CoinExchange.Trades.Domain.Model.Tests
{
    [TestFixture]
    public class DepthLevelTests
    {
        [Test]
        public void AddVolumeToDepthLevel_IfVolumeAddedSuccessfully_ReturnsTrueCountsAggregatedVolumeAndOrderCount()
        {
            DepthLevel depthLevel = new DepthLevel(new Price(491.32M));
            bool addOrder1 = depthLevel.AddOrder(new Volume(2000));
            bool addOrder2 = depthLevel.AddOrder(new Volume(1000));

            Assert.IsTrue(addOrder1);
            Assert.IsTrue(addOrder2);
            Assert.AreEqual(3000, depthLevel.AggregatedVolume.Value, "Aggregated Volume after addition");
            Assert.AreEqual(2, depthLevel.OrderCount, "Order Count after addition");
        }

        [Test]
        public void RemoveVolumeFromDepthLevel_IfVolumeRemoved_ReturnsTrueChecksVolumeAndOrderCount()
        {
            DepthLevel depthLevel = new DepthLevel(new Price(491.32M));
            bool addOrder1 = depthLevel.AddOrder(new Volume(2000));
            bool addOrder2 = depthLevel.AddOrder(new Volume(1000));

            Assert.IsTrue(addOrder1);
            Assert.IsTrue(addOrder2);
            Assert.AreEqual(3000, depthLevel.AggregatedVolume.Value, "Aggregated Volume after addition");
            Assert.AreEqual(2, depthLevel.OrderCount, "Order Count after addition");

            bool removeOrder1 = depthLevel.CloseOrder(new Volume(500));
            Assert.IsTrue(removeOrder1);
            Assert.AreEqual(2500, depthLevel.AggregatedVolume.Value, "Aggregated Volume after removal");
            Assert.AreEqual(1, depthLevel.OrderCount, "Order Count after removal");
        }

        [Test]
        public void DecreaseTheQUantity_IfDecreased_AggregatedVolumeIsAsExpected()
        {
            DepthLevel depthLevel = new DepthLevel(new Price(491.32M));
            bool addOrder1 = depthLevel.AddOrder(new Volume(2000));
            bool addOrder2 = depthLevel.AddOrder(new Volume(1000));

            Assert.IsTrue(addOrder1);
            Assert.IsTrue(addOrder2);
            Assert.AreEqual(3000, depthLevel.AggregatedVolume.Value, "Aggregated Volume after addition");
            Assert.AreEqual(2, depthLevel.OrderCount, "Order Count after addition");

            bool removeOrder1 = depthLevel.DecreaseVolume(new Volume(500));
            Assert.IsTrue(removeOrder1);
            Assert.AreEqual(2500, depthLevel.AggregatedVolume.Value, "Aggregated Volume after decrease");
            Assert.AreEqual(2, depthLevel.OrderCount, "Order Count after decrease");
        }

        [Test]
        public void IncreaseTheQuantity_IfIncreased_AggregatedVolumeIsAsExpected()
        {
            DepthLevel depthLevel = new DepthLevel(new Price(491.32M));
            bool addOrder1 = depthLevel.AddOrder(new Volume(2000));
            bool addOrder2 = depthLevel.AddOrder(new Volume(1000));

            Assert.IsTrue(addOrder1);
            Assert.IsTrue(addOrder2);
            Assert.AreEqual(3000, depthLevel.AggregatedVolume.Value, "Aggregated Volume after addition");
            Assert.AreEqual(2, depthLevel.OrderCount, "Order Count after addition");

            bool removeOrder1 = depthLevel.IncreaseVolume(new Volume(500));
            Assert.IsTrue(removeOrder1);
            Assert.AreEqual(3500, depthLevel.AggregatedVolume.Value, "Aggregated Volume after decrease");
            Assert.AreEqual(2, depthLevel.OrderCount, "Order Count after decrease");
        }
    }
}
