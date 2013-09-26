﻿#region Copyright
// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2013
// by DotNetNuke Corporation
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion

using System;
using DotNetNuke.Services.Social.Subscriptions;
using DotNetNuke.Services.Social.Subscriptions.Data;
using DotNetNuke.Services.Social.Subscriptions.Entities;
using DotNetNuke.Tests.Core.Controllers.Messaging.Builders;
using DotNetNuke.Tests.Core.Controllers.Messaging.Mocks;
using Moq;
using NUnit.Framework;

namespace DotNetNuke.Tests.Core.Controllers.Messaging
{
    [TestFixture]
    public class SubscriptionControllerTests
    {
        private Mock<IDataService> mockDataService;
        private Mock<ISubscriptionSecurityController> subscriptionSecurityController;

        private SubscriptionController subscriptionController;

        [SetUp]
        public void SetUp()
        {
            // Setup Mocks and Stub
            mockDataService = new Mock<IDataService>();
            subscriptionSecurityController = new Mock<ISubscriptionSecurityController>();

            DataService.SetTestableInstance(mockDataService.Object);
            SubscriptionSecurityController.SetTestableInstance(subscriptionSecurityController.Object);

            // Setup SUT
            subscriptionController = new SubscriptionController();
        }
        
        [TearDown]
        public void TearDown()
        {
            DataService.ClearInstance();
            SubscriptionSecurityController.ClearInstance();
        }

        #region IsSubscribed method tests
        [Test]
        public void IsSubscribed_ShouldReturnFalse_IfUserIsNotSubscribed()
        {
            // Arrange
            var subscription = new SubscriptionBuilder()
                .Build();

            mockDataService.Setup(ds => ds.IsSubscribed(
                subscription.UserId,
                subscription.PortalId,
                subscription.SubscriptionTypeId,
                subscription.ObjectKey,
                It.IsAny<int>(),
                It.IsAny<int>())).Returns(MockHelper.CreateEmptySubscriptionReader());
            
            //Act
            var isSubscribed = subscriptionController.IsSubscribed(
                subscription.UserId,
                subscription.PortalId,
                subscription.SubscriptionTypeId,
                subscription.ObjectKey);

            // Assert
            Assert.AreEqual(false, isSubscribed);
        }

        [Test]
        [Ignore]
        public void IsSubscribed_ShouldReturnFalse_WhenUserDoesNotHavePermissionOnTheSubscription()
        {
            // Arrange
            var subscription = new SubscriptionBuilder()
                .Build();

            var subscriptionCollection = new[] {subscription};

            mockDataService.Setup(ds => ds.IsSubscribed(
                subscription.UserId,
                subscription.PortalId,
                subscription.SubscriptionTypeId,
                subscription.ObjectKey,
                It.IsAny<int>(),
                It.IsAny<int>())).Returns(MockHelper.CreateSubscriptionReader(subscriptionCollection));

            subscriptionSecurityController
                .Setup(ssc => ssc.HasPermission(It.IsAny<Subscription>())).Returns(false);

            //Act
            var isSubscribed = subscriptionController.IsSubscribed(
                subscription.UserId, 
                subscription.PortalId,
                subscription.SubscriptionTypeId,
                subscription.ObjectKey);

            // Assert
            Assert.AreEqual(false, isSubscribed);
        }

        [Test]
        [Ignore]
        public void IsSubscribed_ShouldReturnTrue_WhenUserHasPermissionOnTheSubscription()
        {
            // Arrange
            var subscription = new SubscriptionBuilder()
                .Build();

            var subscriptionCollection = new[] { subscription };

            mockDataService.Setup(ds => ds.IsSubscribed(
                subscription.UserId,
                subscription.PortalId,
                subscription.SubscriptionTypeId,
                subscription.ObjectKey,
                It.IsAny<int>(),
                It.IsAny<int>())).Returns(MockHelper.CreateSubscriptionReader(subscriptionCollection));

            subscriptionSecurityController
                .Setup(ssc => ssc.HasPermission(It.IsAny<Subscription>())).Returns(true);

            //Act
            var isSubscribed = subscriptionController.IsSubscribed(
                subscription.UserId,
                subscription.PortalId,
                subscription.SubscriptionTypeId,
                subscription.ObjectKey);

            // Assert
            Assert.AreEqual(true, isSubscribed);
        }

        [Test]
        public void IsSubscribed_ShouldCallDataService_WhenNoError()
        {
            // Arrange
            var subscription = new SubscriptionBuilder()
                .Build();

            mockDataService.Setup(ds => ds.IsSubscribed(
                subscription.UserId,
                subscription.PortalId,
                subscription.SubscriptionTypeId,
                subscription.ObjectKey,
                subscription.ModuleId,
                subscription.TabId)).Returns(MockHelper.CreateEmptySubscriptionReader()).Verifiable();

            //Act
            subscriptionController.IsSubscribed(
                subscription.UserId,
                subscription.PortalId,
                subscription.SubscriptionTypeId,
                subscription.ObjectKey);

            // Assert
            mockDataService.Verify(ds => ds.IsSubscribed(
                subscription.UserId,
                subscription.PortalId,
                subscription.SubscriptionTypeId,
                subscription.ObjectKey,
                subscription.ModuleId,
                subscription.TabId), Times.Once);
        }
        #endregion

        #region AddSubscription method tests
        [Test]
        public void AddSubscription_ShouldThrowArgumentNullException_WhenSubscriptionIsNull()
        {
            //Act, Arrange
            Assert.Throws<ArgumentNullException>(() => subscriptionController.AddSubscription(null));
        }

        [Test]
        public void AddSubscription_ShouldThrowArgumentOutOfRangeException_WhenSubscriptionUserIdPropertyIsNegative()
        {
            // Arrange
            var subscription = new SubscriptionBuilder()
                .WithUserId(-1)
                .Build();

            //Act, Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => subscriptionController.AddSubscription(subscription));
        }

        [Test]
        public void AddSubscription_ShouldThrowArgumentOutOfRangeException_WhenSubscriptionSubscriptionTypePropertyIsNegative()
        {
            // Arrange
            var subscription = new SubscriptionBuilder()
                .WithSubscriptionTypeId(-1)
                .Build();

            //Act, Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => subscriptionController.AddSubscription(subscription));
        }

        [Test]
        public void AddSubscription_ShouldThrowArgumentNullException_WhenSubscriptionObjectKeyIsNull()
        {
            // Arrange
            var subscription = new SubscriptionBuilder()
                .WithObjectKey(null)
                .Build();

            //Act, Assert
            Assert.Throws<ArgumentNullException>(() => subscriptionController.AddSubscription(subscription));
        }
        
        [Test]
        public void AddSubscription_ShouldCallDataService_WhenNoError()
        {
            // Arrange
            var subscription = new SubscriptionBuilder()
                .Build();
            
            mockDataService.Setup(ds => ds.AddSubscription(
                subscription.UserId, 
                subscription.PortalId, 
                subscription.SubscriptionTypeId, 
                subscription.ObjectKey, 
                subscription.Description, 
                subscription.ModuleId, 
                subscription.TabId)).Verifiable();

            //Act
            subscriptionController.AddSubscription(subscription);

            // Assert
            mockDataService.Verify(ds => ds.AddSubscription(
                subscription.UserId,
                subscription.PortalId,
                subscription.SubscriptionTypeId,
                subscription.ObjectKey,
                subscription.Description,
                subscription.ModuleId,
                subscription.TabId), Times.Once);
        }

        [Test]
        public void AddSubscription_ShouldReturnsTheSubscriptionId_WhenNoError()
        {
            // Arrange
            const int expectedSubscriptionId = 1;

            var subscription = new SubscriptionBuilder()
                .Build();

            mockDataService.Setup(ds => ds.AddSubscription(
                subscription.UserId,
                subscription.PortalId,
                subscription.SubscriptionTypeId,
                subscription.ObjectKey,
                subscription.Description,
                subscription.ModuleId,
                subscription.TabId)).Returns(expectedSubscriptionId);

            //Act
            var actualSubscriptionId = subscriptionController.AddSubscription(subscription);

            // Assert
           Assert.AreEqual(expectedSubscriptionId, actualSubscriptionId);
        }
        #endregion

        #region DeleteSubscription method tests
        [Test]
        public void DeleteSubscription_ShouldThrowArgumentOutOfRangeException_WhenSubscriptionIdIsNegative()
        {
            //Act, Arrange
            Assert.Throws<ArgumentOutOfRangeException>(() => subscriptionController.DeleteSubscription(-1));
        }

        [Test]
        public void DeleteSubscriptionType_ShouldCallDataService_WhenNoError()
        {
            // Arrange
            const int subscriptionId = 1;

            mockDataService
                .Setup(ds => ds.DeleteSubscription(subscriptionId))
                .Verifiable();

            //Act
            subscriptionController.DeleteSubscription(subscriptionId);

            //Assert
            mockDataService.Verify(ds => ds.DeleteSubscription(subscriptionId), Times.Once());
        }
        #endregion
    }
}