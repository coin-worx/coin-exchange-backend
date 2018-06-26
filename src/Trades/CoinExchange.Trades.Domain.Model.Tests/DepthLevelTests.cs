/***************************************************************************** 
* Copyright 2016 Aurora Solutions 
* 
*    http://www.aurorasolutions.io 
* 
* Aurora Solutions is an innovative services and product company at 
* the forefront of the software industry, with processes and practices 
* involving Domain Driven Design(DDD), Agile methodologies to build 
* scalable, secure, reliable and high performance products.
* 
* Coin Exchange is a high performance exchange system specialized for
* Crypto currency trading. It has different general purpose uses such as
* independent deposit and withdrawal channels for Bitcoin and Litecoin,
* but can also act as a standalone exchange that can be used with
* different asset classes.
* Coin Exchange uses state of the art technologies such as ASP.NET REST API,
* AngularJS and NUnit. It also uses design patterns for complex event
* processing and handling of thousands of transactions per second, such as
* Domain Driven Designing, Disruptor Pattern and CQRS With Event Sourcing.
* 
* Licensed under the Apache License, Version 2.0 (the "License"); 
* you may not use this file except in compliance with the License. 
* You may obtain a copy of the License at 
* 
*    http://www.apache.org/licenses/LICENSE-2.0 
* 
* Unless required by applicable law or agreed to in writing, software 
* distributed under the License is distributed on an "AS IS" BASIS, 
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
* See the License for the specific language governing permissions and 
* limitations under the License. 
*****************************************************************************/


﻿using CoinExchange.Trades.Domain.Model.OrderAggregate;
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
            Assert.IsFalse(removeOrder1);
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

        [Test]
        public void DepthLevelComparisonTest_IfDepthLevelIsLess_ReturnsFalse()
        {
            DepthLevel depthLevel1 = new DepthLevel(new Price(1));
            DepthLevel depthLevel2 = new DepthLevel(new Price(2));

            bool falseResult = depthLevel1 > depthLevel2;
            bool trueResult = depthLevel1 < depthLevel2;

            Assert.IsFalse(falseResult);
            Assert.IsTrue(trueResult);
        }
    }
}
