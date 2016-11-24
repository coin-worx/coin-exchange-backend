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


﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.OrderMatchingEngine;
using CoinExchange.Trades.ReadModel.MemoryImages;
using NUnit.Framework;

namespace CoinExchange.Trades.ReadModel.Tests
{
    [TestFixture]
    class BBOMemoryImageTests
    {
        [Test]
        [Category("Unit")]
        public void BboAddTest_ChecksWhetherTheBestBidAndOfferAreAddedSuccessfully_VerifiesBBoListFromMEmoryImage()
        {
            BBOMemoryImage bboMemoryImage = new BBOMemoryImage();

            DepthLevel bestBid = new DepthLevel(new Price(491));
            bestBid.IncreaseVolume(new Volume(600));
            bestBid.UpdateOrderCount(1);

            DepthLevel bestAsk = new DepthLevel(new Price(496));
            bestAsk.IncreaseVolume(new Volume(500));
            bestAsk.UpdateOrderCount(1);

            bboMemoryImage.OnBBOArrived("XTCUSD", bestBid, bestAsk);

            Assert.AreEqual(1, bboMemoryImage.BBORepresentationList.Count());
            Assert.AreEqual(491, bboMemoryImage.BBORepresentationList.First().BestBidPrice);
            Assert.AreEqual(496, bboMemoryImage.BBORepresentationList.First().BestAskPrice);
            Assert.AreEqual(600, bboMemoryImage.BBORepresentationList.First().BestBidVolume);
            Assert.AreEqual(500, bboMemoryImage.BBORepresentationList.First().BestAskVolume);
            Assert.AreEqual(1, bboMemoryImage.BBORepresentationList.First().BestBidOrderCount);
            Assert.AreEqual(1, bboMemoryImage.BBORepresentationList.First().BestAskOrderCount);
        }

        [Test]
        [Category("Unit")]
        public void AddRateTest_TestIfTheRateForACurrencyPairIsAsExpected_VerifiesThroughTheReturnedRate()
        {
            BBOMemoryImage bboMemoryImage = new BBOMemoryImage();

            DepthLevel bidLevel1 = new DepthLevel(new Price(491));
            DepthLevel bidLevel2 = new DepthLevel(new Price(492));
            DepthLevel bidLevel3 = new DepthLevel(new Price(493));

            DepthLevel askLevel1 = new DepthLevel(new Price(494));
            DepthLevel askLevel2 = new DepthLevel(new Price(495));
            DepthLevel askLevel3 = new DepthLevel(new Price(496));
            bboMemoryImage.OnBBOArrived("XBT/USD", bidLevel1, askLevel1);
            Assert.AreEqual("XBT/USD", bboMemoryImage.RatesList.ToList()[0].CurrencyPair);
            Assert.AreEqual(492.5, bboMemoryImage.RatesList.ToList()[0].RateValue); // MidPoint of bidLevel1 = 491 & askLevel1 = 494

            bboMemoryImage.OnBBOArrived("XBT/USD", bidLevel2, askLevel2);
            Assert.AreEqual("XBT/USD", bboMemoryImage.RatesList.ToList()[0].CurrencyPair);
            Assert.AreEqual(493.5, bboMemoryImage.RatesList.ToList()[0].RateValue); // MidPoint of bidLevel2 = 492 & askLevel2 = 495

            bboMemoryImage.OnBBOArrived("XBT/USD", bidLevel3, askLevel3);
            Assert.AreEqual("XBT/USD", bboMemoryImage.RatesList.ToList()[0].CurrencyPair);
            Assert.AreEqual(494.5, bboMemoryImage.RatesList.ToList()[0].RateValue); // MidPoint of bidLevel3 = 493 & askLevel3 = 496
        }

        [Test]
        [Category("Unit")]
        public void AddRatesForMultiplkeCurrenciesTest_AddRatesForMultipleCurrenciesAndExpectsAListOfRates_VerifiesThroughTheReturnedRateList()
        {
            BBOMemoryImage bboMemoryImage = new BBOMemoryImage();

            DepthLevel xbtUsdBidLevel = new DepthLevel(new Price(491));
            DepthLevel ltcUsdBidLevel = new DepthLevel(new Price(492));
            DepthLevel btcUsdBidLevel = new DepthLevel(new Price(493));

            DepthLevel xbtUsdAskLevel = new DepthLevel(new Price(494));
            DepthLevel ltcUsdAskLevel = new DepthLevel(new Price(495));
            DepthLevel btcUsdAskLevel = new DepthLevel(new Price(496));
            bboMemoryImage.OnBBOArrived("XBT/USD", xbtUsdBidLevel, xbtUsdAskLevel);
            bboMemoryImage.OnBBOArrived("LTC/USD", ltcUsdBidLevel, ltcUsdAskLevel);
            bboMemoryImage.OnBBOArrived("BTC/USD", btcUsdBidLevel, btcUsdAskLevel);
            
            Assert.AreEqual("XBT/USD", bboMemoryImage.RatesList.ToList()[0].CurrencyPair);
            Assert.AreEqual(492.5, bboMemoryImage.RatesList.ToList()[0].RateValue); // MidPoint of xbtUsdBidLevel = 491 & xbtUsdAskLevel = 494
            Assert.AreEqual("LTC/USD", bboMemoryImage.RatesList.ToList()[1].CurrencyPair);
            Assert.AreEqual(493.5, bboMemoryImage.RatesList.ToList()[1].RateValue); // MidPoint of ltcUsdBidLevel = 492 & ltcUsdAskLevel = 495
            Assert.AreEqual("BTC/USD", bboMemoryImage.RatesList.ToList()[2].CurrencyPair);
            Assert.AreEqual(494.5, bboMemoryImage.RatesList.ToList()[2].RateValue); // MidPoint of btcUsdBidLevel = 493 & btcUsdAskLevel = 496
        }
    }
}
