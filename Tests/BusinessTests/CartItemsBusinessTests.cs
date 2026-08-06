using Business_Layer.Business;
using Business_Layer.Interfaces;
using Data_Layer.Interfaces;
using Models;
using Moq;
using Moq.Protected;
using Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static Tests.BusinessTests.CartItemsBusinessTests;

namespace Tests.BusinessTests
{
    public class CartItemsBusinessTests
    {
        public class CartItemBusinessForTest : Business_Layer.Business.CartItem
        {
            public CartItemBusinessForTest()
                : base(null, null, null, null, null, null)
            {
            }

            public DataTable Call_ToDataTable(List<NewOrderRequest> items)
            {
                return base.ToDataTable(items);
            }
        }


        [Fact]
        public async Task SyncCartItemsPromocode_ShouldReturnFalse_WhenUserIdIsZero()
        {
            var cartItemDataMock = new Mock<ICartItemData>();
            var usersBusinessMock = new Mock<IUsersBusiness>();

            usersBusinessMock.Setup(u => u.GetUserId()).Returns(0);

            var business = new CartItemBusiness(
                cartItemDataMock.Object,
                usersBusinessMock.Object,
                null, null, null, null
            );

            var result = await business.SyncCartItemsPromocode();

            Assert.False(result);
            cartItemDataMock.Verify(x => x.SyncCartItemsPromocode(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task SyncCartItemsPromocode_ShouldCallDataLayer_WhenUserIdIsValid()
        {
            var cartItemDataMock = new Mock<ICartItemData>();
            var usersBusinessMock = new Mock<IUsersBusiness>();

            usersBusinessMock.Setup(u => u.GetUserId()).Returns(5);
            cartItemDataMock.Setup(d => d.SyncCartItemsPromocode(5)).ReturnsAsync(true);

            var business = new CartItemBusiness(
                cartItemDataMock.Object,
                usersBusinessMock.Object,
                null, null, null, null
            );

            var result = await business.SyncCartItemsPromocode();

            Assert.True(result);
            cartItemDataMock.Verify(x => x.SyncCartItemsPromocode(5), Times.Once);
        }

        [Fact]
        public async Task DeleteCartItem_ShouldReturnFalse_WhenItemIdIsLessThanOne()
        {
            var cartItemDataMock = new Mock<ICartItemData>();
            var usersBusinessMock = new Mock<IUsersBusiness>();

            var business = new CartItemBusiness(
                cartItemDataMock.Object,
                usersBusinessMock.Object,
                null, null, null, null
            );

            var result = await business.DeleteCartItem(0);

            Assert.False(result);
            cartItemDataMock.Verify(x => x.DeleteCartItem(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
            usersBusinessMock.Verify(x => x.GetUserId(), Times.Never);
        }

        [Fact]
        public async Task DeleteCartItem_ShouldReturnFalse_WhenUserIdIsZero()
        {
            var cartItemDataMock = new Mock<ICartItemData>();
            var usersBusinessMock = new Mock<IUsersBusiness>();

            usersBusinessMock.Setup(u => u.GetUserId()).Returns(0);

            var business = new CartItemBusiness(
                cartItemDataMock.Object,
                usersBusinessMock.Object,
                null, null, null, null
            );

            var result = await business.DeleteCartItem(5);

            Assert.False(result);
            cartItemDataMock.Verify(x => x.DeleteCartItem(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task DeleteCartItem_ShouldCallDataLayer_WhenValidItemIdAndUserId()
        {
            var cartItemDataMock = new Mock<ICartItemData>();
            var usersBusinessMock = new Mock<IUsersBusiness>();

            usersBusinessMock.Setup(u => u.GetUserId()).Returns(10);
            cartItemDataMock.Setup(d => d.DeleteCartItem(5, 10)).ReturnsAsync(true);

            var business = new CartItemBusiness(
                cartItemDataMock.Object,
                usersBusinessMock.Object,
                null, null, null, null
            );

            var result = await business.DeleteCartItem(5);

            Assert.True(result);
            cartItemDataMock.Verify(x => x.DeleteCartItem(5, 10), Times.Once);
        }

        [Fact]
        public async Task InsertCartItem_ShouldReturnFalse_WhenProductIdIsZero()
        {
            var cartItemDataMock = new Mock<ICartItemData>();
            var usersBusinessMock = new Mock<IUsersBusiness>();

            var business = new CartItemBusiness(
                cartItemDataMock.Object,
                usersBusinessMock.Object,
                null, null, null, null
            );

            var result = await business.InsertCartItem(0);

            Assert.False(result);
            usersBusinessMock.Verify(x => x.GetUserId(), Times.Never);
            cartItemDataMock.Verify(x => x.InsertCartItem(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task InsertCartItem_ShouldReturnFalse_WhenUserIdIsZero()
        {
            var cartItemDataMock = new Mock<ICartItemData>();
            var usersBusinessMock = new Mock<IUsersBusiness>();

            usersBusinessMock.Setup(u => u.GetUserId()).Returns(0);

            var business = new CartItemBusiness(
                cartItemDataMock.Object,
                usersBusinessMock.Object,
                null, null, null, null
            );

            var result = await business.InsertCartItem(5);

            Assert.False(result);
            cartItemDataMock.Verify(x => x.InsertCartItem(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task InsertCartItem_ShouldReturnTrue_WhenAllValid()
        {
            var cartItemDataMock = new Mock<ICartItemData>();
            var usersBusinessMock = new Mock<IUsersBusiness>();

            usersBusinessMock.Setup(u => u.GetUserId()).Returns(10);
            cartItemDataMock.Setup(d => d.InsertCartItem(5, 10)).ReturnsAsync(true);


            var business = new CartItemBusiness(
                cartItemDataMock.Object,
                usersBusinessMock.Object,
                null, null, null, null
            );

            var result = await business.InsertCartItem(5);

            Assert.True(result);
            cartItemDataMock.Verify(x => x.InsertCartItem(5, 10), Times.Once);
        }

        [Fact]
        public async Task PlusOneCartItem_ShouldReturnFalse_WhenCartItemIdIsLessThanOne()
        {
            var cartItemDataMock = new Mock<ICartItemData>();
            var usersBusinessMock = new Mock<IUsersBusiness>();

            var business = new CartItemBusiness(
                cartItemDataMock.Object,
                usersBusinessMock.Object,
                null, null, null, null
                );

            var result = await business.PlusOneCartItem(0);

            Assert.False(result);
            usersBusinessMock.Verify(u => u.GetUserId(), Times.Never);
            cartItemDataMock.Verify(d => d.UpdateCartItem(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task PlusOneCartItem_ShouldReturnFalse_WhenUserIdIsZero()
        {
            var cartItemDataMock = new Mock<ICartItemData>();
            var usersBusinessMock = new Mock<IUsersBusiness>();

            usersBusinessMock.Setup(u => u.GetUserId()).Returns(0);

            var business = new CartItemBusiness(
                cartItemDataMock.Object,
                usersBusinessMock.Object,
                null, null, null, null
                );

            var result = await business.PlusOneCartItem(5);

            Assert.False(result);
            cartItemDataMock.Verify(d => d.UpdateCartItem(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task PlusOneCartItem_ShouldReturnTrue_WhenAllValid()
        {
            var cartItemDataMock = new Mock<ICartItemData>();
            var usersBusinessMock = new Mock<IUsersBusiness>();

            usersBusinessMock.Setup(u => u.GetUserId()).Returns(10);
            cartItemDataMock.Setup(d => d.UpdateCartItem(5, 1, 10)).ReturnsAsync(true);

            var business = new CartItemBusiness(
                cartItemDataMock.Object,
                usersBusinessMock.Object,
                null, null, null, null
                );

            var result = await business.PlusOneCartItem(5);

            Assert.True(result);
            cartItemDataMock.Verify(d => d.UpdateCartItem(5, 1, 10), Times.Once);
        }

        [Fact]
        public async Task MinusOneCartItem_ShouldReturnFalse_WhenCartItemIdIsLessThanOne()
        {
            var cartItemDataMock = new Mock<ICartItemData>();
            var usersBusinessMock = new Mock<IUsersBusiness>();

            var business = new CartItemBusiness(
                cartItemDataMock.Object,
                usersBusinessMock.Object,
                null, null, null, null
                );

            var result = await business.MinusOneCartItem(0);

            Assert.False(result);
            usersBusinessMock.Verify(u => u.GetUserId(), Times.Never);
            cartItemDataMock.Verify(d => d.UpdateCartItem(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task MinusOneCartItem_ShouldReturnFalse_WhenUserIdIsZero()
        {
            var cartItemDataMock = new Mock<ICartItemData>();
            var usersBusinessMock = new Mock<IUsersBusiness>();

            usersBusinessMock.Setup(u => u.GetUserId()).Returns(0);

            var business = new CartItemBusiness(
                cartItemDataMock.Object,
                usersBusinessMock.Object,
                null, null, null, null
                );

            var result = await business.MinusOneCartItem(5);

            Assert.False(result);
            cartItemDataMock.Verify(d => d.UpdateCartItem(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task MinusOneCartItem_ShouldReturnTrue_WhenAllValid()
        {
            var cartItemDataMock = new Mock<ICartItemData>();
            var usersBusinessMock = new Mock<IUsersBusiness>();

            usersBusinessMock.Setup(u => u.GetUserId()).Returns(10);
            cartItemDataMock.Setup(d => d.UpdateCartItem(5, -1, 10)).ReturnsAsync(true);

            var business = new CartItemBusiness(
                cartItemDataMock.Object,
                usersBusinessMock.Object,
                null, null, null, null
                );

            var result = await business.MinusOneCartItem(5);

            Assert.True(result);
            cartItemDataMock.Verify(d => d.UpdateCartItem(5, -1, 10), Times.Once);
        }

        [Fact]
        public async Task UsePromocode_ShouldReturnFalse_WhenPromocodeIsNull()
        {
            var cartItemDataMock = new Mock<ICartItemData>();
            var usersBusinessMock = new Mock<IUsersBusiness>();

            var business = new CartItemBusiness(
                cartItemDataMock.Object,
                usersBusinessMock.Object,
                null, null, null, null
                );

            var result = await business.UsePromocode(5, null);

            Assert.False(result);
            usersBusinessMock.Verify(u => u.GetUserId(), Times.Never);
            cartItemDataMock.Verify(d => d.UpdateCartItem(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task UsePromocode_ShouldReturnFalse_WhenPromocodeIsEmpty()
        {
            var cartItemDataMock = new Mock<ICartItemData>();
            var usersBusinessMock = new Mock<IUsersBusiness>();

            var business = new CartItemBusiness(
              cartItemDataMock.Object,
              usersBusinessMock.Object,
              null, null, null, null
              );

            var result = await business.UsePromocode(5, "     ");

            Assert.False(result);
            usersBusinessMock.Verify(u => u.GetUserId(), Times.Never);
            cartItemDataMock.Verify(d => d.UsePromocode(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task UsePromocode_ShouldReturnFalse_WhenProductIdLessThanOne()
        {
            var cartItemDataMock = new Mock<ICartItemData>();
            var usersBusinessMock = new Mock<IUsersBusiness>();

            var business = new CartItemBusiness(
              cartItemDataMock.Object,
              usersBusinessMock.Object,
              null, null, null, null
              );

            var result = await business.UsePromocode(0, "XXPromocodeXX");

            Assert.False(result);
            usersBusinessMock.Verify(u => u.GetUserId(), Times.Never);
            cartItemDataMock.Verify(d => d.UsePromocode(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task UsePromocode_ShouldReturnFalse_WhenUserIdIsZero()
        {
            var cartItemDataMock = new Mock<ICartItemData>();
            var usersBusinessMock = new Mock<IUsersBusiness>();

            usersBusinessMock.Setup(u => u.GetUserId()).Returns(0);

            var business = new CartItemBusiness(
              cartItemDataMock.Object,
              usersBusinessMock.Object,
              null, null, null, null
              );

            var result = await business.UsePromocode(5, "XXPromocodeXX");

            Assert.False(result);
            usersBusinessMock.Verify(u => u.GetUserId(), Times.Once);
            cartItemDataMock.Verify(d => d.UsePromocode(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task UsePromocode_ShouldReturnTrue_WhenAllValid()
        {
            string promocode = "XXPromocodeXX";

            var cartItemDataMock = new Mock<ICartItemData>();
            var usersBusinessMock = new Mock<IUsersBusiness>();

            usersBusinessMock.Setup(u => u.GetUserId()).Returns(10);
            cartItemDataMock.Setup(d => d.UsePromocode(5, promocode, 10)).ReturnsAsync(true);

            var business = new CartItemBusiness(
               cartItemDataMock.Object,
               usersBusinessMock.Object,
               null, null, null, null
               );

            var result = await business.UsePromocode(5, promocode);

            Assert.True(result);
            usersBusinessMock.Verify(u => u.GetUserId(), Times.Once);
            cartItemDataMock.Verify(d => d.UsePromocode(5, promocode, 10), Times.Once);
        }

        [Fact]
        public async Task GetCartItemQuantities_ShouldReturnNull_WhenUserIdIsZero()
        {
            var cartItemDataMock = new Mock<ICartItemData>();
            var usersBusinessMock = new Mock<IUsersBusiness>();

            usersBusinessMock.Setup(u => u.GetUserId()).Returns(0);

            var business = new CartItemBusiness(
              cartItemDataMock.Object,
              usersBusinessMock.Object,
              null, null, null, null
              );

            var result = await business.GetCartItemQuantities();

            Assert.Null(result);
            cartItemDataMock.Verify(d => d.GetCartItemQuantities(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task GetCartItemQuantities_ShouldReturnList_WhenUserIdIsValid()
        {
            // Arrange
            var cartItemDataMock = new Mock<ICartItemData>();
            var usersBusinessMock = new Mock<IUsersBusiness>();

            int userId = 10;
            var expectedList = new List<NewOrderRequest>
            {
                 new NewOrderRequest { StockId = 1, Quantity = 2 },
                 new NewOrderRequest { StockId = 2, Quantity = 5 }
            };

            usersBusinessMock.Setup(u => u.GetUserId()).Returns(userId);
            cartItemDataMock.Setup(d => d.GetCartItemQuantities(userId)).ReturnsAsync(expectedList);

            var business = new CartItemBusiness(
              cartItemDataMock.Object,
              usersBusinessMock.Object,
              null, null, null, null
              );

            var result = await business.GetCartItemQuantities();

            Assert.NotNull(result);
            Assert.Equal(expectedList.Count, result.Count);
            Assert.Equal(expectedList, result);
            cartItemDataMock.Verify(d => d.GetCartItemQuantities(userId), Times.Once);
        }

        [Fact]
        public async Task SyncCartItemsCount_ShouldReturnFalse_WhenItemsListIsEmpty()
        {
            var cartItemDataMock = new Mock<ICartItemData>();
            var usersBusinessMock = new Mock<IUsersBusiness>();

            var business = new CartItemBusiness(
             cartItemDataMock.Object,
             usersBusinessMock.Object,
             null, null, null, null
             );

            var result = await business.SyncCartItemsCount(new List<NewOrderRequest>());

            Assert.False(result);
            usersBusinessMock.Verify(u => u.GetUserId(), Times.Never);
            cartItemDataMock.Verify(d => d.SyncCartItemsCount(It.IsAny<DataTable>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task SyncCartItemsCount_ShouldReturnFalse_WhenItemsListIsNull()
        {
            var cartItemDataMock = new Mock<ICartItemData>();
            var usersBusinessMock = new Mock<IUsersBusiness>();

            var business = new CartItemBusiness(
              cartItemDataMock.Object,
              usersBusinessMock.Object,
              null, null, null, null
              );

            var result = await business.SyncCartItemsCount(null);

            Assert.False(result);
            usersBusinessMock.Verify(u => u.GetUserId(), Times.Never);
            cartItemDataMock.Verify(d => d.SyncCartItemsCount(It.IsAny<DataTable>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task SyncCartItemsCount_ShouldReturnFalse_WhenUserIdIsZero()
        {
            var cartItemDataMock = new Mock<ICartItemData>();
            var usersBusinessMock = new Mock<IUsersBusiness>();

            usersBusinessMock.Setup(u => u.GetUserId()).Returns(0);

            var items = new List<NewOrderRequest>
            {
                new NewOrderRequest { StockId = 1, Quantity = 2 }
            };

            var business = new CartItemBusiness(
              cartItemDataMock.Object,
              usersBusinessMock.Object,
              null, null, null, null
              );

            var result = await business.SyncCartItemsCount(items);

            Assert.False(result);
            usersBusinessMock.Verify(u => u.GetUserId(), Times.Once);
            cartItemDataMock.Verify(d => d.SyncCartItemsCount(It.IsAny<DataTable>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task SyncCartItemsCount_ShouldReturnTrue_WhenAllValid()
        {
            var cartItemDataMock = new Mock<ICartItemData>();
            var usersBusinessMock = new Mock<IUsersBusiness>();

            int userId = 10;
            var items = new List<NewOrderRequest>
            {
                new NewOrderRequest { StockId = 1, Quantity = 2 },
                new NewOrderRequest { StockId = 2, Quantity = 5 }
            };

            usersBusinessMock.Setup(u => u.GetUserId()).Returns(userId);
            cartItemDataMock.Setup(d => d.SyncCartItemsCount(It.IsAny<DataTable>(), userId)).ReturnsAsync(true);

            var business = new CartItemBusiness(
             cartItemDataMock.Object,
             usersBusinessMock.Object,
             null, null, null, null
             );

            var result = await business.SyncCartItemsCount(items);

            Assert.True(result);
            usersBusinessMock.Verify(u => u.GetUserId(), Times.Once);
            cartItemDataMock.Verify(d => d.SyncCartItemsCount(It.IsAny<DataTable>(), userId), Times.Once);
        }

        [Fact]
        public void ToDataTable_ShouldReturnNull_WhenItemsNull()
        {
            var business = new CartItemBusinessForTest();

            var result = business.Call_ToDataTable(null);

            Assert.Null(result);
        }

        [Fact]
        public void ToDataTable_ShouldReturnNull_WhenItemsEmptyList()
        {
            var business = new CartItemBusinessForTest();

            var result = business.Call_ToDataTable([]);

            Assert.Null(result);
        }

        [Fact]
        public void ToDataTable_ShouldReturnDataTable_WhenAllValid()
        {
            var business = new CartItemBusinessForTest();

            var items = new List<NewOrderRequest>
            {
                new NewOrderRequest { StockId = 10, Quantity = 3 },
                new NewOrderRequest { StockId = 20, Quantity = 5 }
            };

            var result = business.Call_ToDataTable(items);

            var table = business.Call_ToDataTable(items);

            Assert.NotNull(table);
            Assert.Equal(2, table.Rows.Count);

            Assert.Equal(10, table.Rows[0]["mappingProductId"]);
            Assert.Equal(3, table.Rows[0]["quantity"]);

            Assert.Equal(20, table.Rows[1]["mappingProductId"]);
            Assert.Equal(5, table.Rows[1]["quantity"]);
        }

        [Fact]
        public async Task SyncCartItemsWithStocks_ShouldReturnFalse_WhenInvalidUserId()
        {
            var usersMock = new Mock<IUsersBusiness>();
            var keyGenMock = new Mock<IInventoryKeyGenerator>();
            var cartDataMock = new Mock<ICartItemData>();

            usersMock.Setup(u => u.GetUserId()).Returns(0);

            var items = new List<NewOrderRequest>
            {
                new NewOrderRequest { StockId = 1, Quantity = 2 }
            };
            cartDataMock.Setup(c => c.GetCartItemQuantities(0)).ReturnsAsync(items);
            cartDataMock.Setup(c => c.SyncCartItemsCount(It.IsAny<DataTable>(), It.IsAny<int>())).ReturnsAsync(false);

            keyGenMock.Setup(k => k.GenerateJwt()).Returns("Fake_token");

            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });

            var httpClient = new HttpClient(handlerMock.Object);

            var storeUrls = new StoreUrls { SyncOrderRequest = "http://fake-url.com" };

            var business = new CartItemBusiness(
                cartDataMock.Object,
                usersMock.Object,
                keyGenMock.Object,
                null,
                storeUrls,
                httpClient
            );

            var result = await business.SyncCartItemsWithStocks();

            Assert.False(result);

            usersMock.Verify(u => u.GetUserId(), Times.Once);
            cartDataMock.Verify(c => c.SyncCartItemsCount(It.IsAny<DataTable>(), It.IsAny<int>()), Times.Never);
            cartDataMock.Verify(c => c.GetCartItemQuantities(0), Times.Never);
            keyGenMock.Verify(k => k.GenerateJwt(), Times.Never);
        }

        [Fact]
        public async Task SyncCartItemsWithStocks_ShouldReturnFalse_WhenGetCartItemsQuantitysIsNull()
        {
            var usersMock = new Mock<IUsersBusiness>();
            var keyGenMock = new Mock<IInventoryKeyGenerator>();
            var cartDataMock = new Mock<ICartItemData>();

            usersMock.Setup(u => u.GetUserId()).Returns(10);


            cartDataMock.Setup(c => c.GetCartItemQuantities(10)).ReturnsAsync([]);
            cartDataMock.Setup(c => c.SyncCartItemsCount(It.IsAny<DataTable>(), It.IsAny<int>())).ReturnsAsync(false);

            keyGenMock.Setup(k => k.GenerateJwt()).Returns("Fake_token");

            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });

            var httpClient = new HttpClient(handlerMock.Object);

            var storeUrls = new StoreUrls { SyncOrderRequest = "http://fake-url.com" };

            var business = new CartItemBusiness(
                cartDataMock.Object,
                usersMock.Object,
                keyGenMock.Object,
                null,
                storeUrls,
                httpClient
            );

            var result = await business.SyncCartItemsWithStocks();

            Assert.False(result);

            usersMock.Verify(u => u.GetUserId(), Times.AtLeastOnce);
            cartDataMock.Verify(c => c.GetCartItemQuantities(10), Times.Once);
            cartDataMock.Verify(c => c.SyncCartItemsCount(It.IsAny<DataTable>(), It.IsAny<int>()), Times.Never);
            keyGenMock.Verify(k => k.GenerateJwt(), Times.Never);
        }

        [Fact]
        public async Task SyncCartItemsWithStocks_ShouldReturnFalse_WhenTokenIsEmpty()
        {
            var usersMock = new Mock<IUsersBusiness>();
            var keyGenMock = new Mock<IInventoryKeyGenerator>();
            var cartDataMock = new Mock<ICartItemData>();

            usersMock.Setup(u => u.GetUserId()).Returns(10);

            var items = new List<NewOrderRequest>
            {
                new NewOrderRequest { StockId = 1, Quantity = 2 }
            };
            cartDataMock.Setup(c => c.GetCartItemQuantities(10)).ReturnsAsync(items);
            cartDataMock.Setup(c => c.SyncCartItemsCount(It.IsAny<DataTable>(), It.IsAny<int>())).ReturnsAsync(false);

            keyGenMock.Setup(k => k.GenerateJwt()).Returns(string.Empty);

            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });

            var httpClient = new HttpClient(handlerMock.Object);

            var storeUrls = new StoreUrls { SyncOrderRequest = "http://fake-url.com" };

            var business = new CartItemBusiness(
                cartDataMock.Object,
                usersMock.Object,
                keyGenMock.Object,
                null,
                storeUrls,
                httpClient
            );

            var result = await business.SyncCartItemsWithStocks();

            Assert.False(result);

            cartDataMock.Verify(c => c.SyncCartItemsCount(It.IsAny<DataTable>(), It.IsAny<int>()), Times.Never);
            cartDataMock.Verify(c => c.GetCartItemQuantities(10), Times.Once);
            usersMock.Verify(u => u.GetUserId(), Times.AtLeastOnce);
            keyGenMock.Verify(k => k.GenerateJwt(), Times.Once);
        }

        [Fact]
        public async Task SyncCartItemsWithStocks_ShouldReturnFalse_WhenResponseStatusCodeNotEqual200()
        {
            var usersMock = new Mock<IUsersBusiness>();
            var keyGenMock = new Mock<IInventoryKeyGenerator>();
            var cartDataMock = new Mock<ICartItemData>();

            usersMock.Setup(u => u.GetUserId()).Returns(10);

            var items = new List<NewOrderRequest>
            {
                new NewOrderRequest { StockId = 1, Quantity = 2 }
            };
            cartDataMock.Setup(c => c.GetCartItemQuantities(10)).ReturnsAsync(items);
            cartDataMock.Setup(c => c.SyncCartItemsCount(It.IsAny<DataTable>(), It.IsAny<int>())).ReturnsAsync(false);

            keyGenMock.Setup(k => k.GenerateJwt()).Returns("fake_token");

            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });

            var httpClient = new HttpClient(handlerMock.Object);

            var storeUrls = new StoreUrls { SyncOrderRequest = "http://fake-url.com" };

            var business = new CartItemBusiness(
                cartDataMock.Object,
                usersMock.Object,
                keyGenMock.Object,
                null,
                storeUrls,
                httpClient
            );

            var result = await business.SyncCartItemsWithStocks();

            Assert.False(result);


            cartDataMock.Verify(c => c.SyncCartItemsCount(It.IsAny<DataTable>(), It.IsAny<int>()), Times.Never);
            cartDataMock.Verify(c => c.GetCartItemQuantities(It.IsAny<int>()), Times.Once);
            usersMock.Verify(u => u.GetUserId(), Times.AtLeastOnce);
            keyGenMock.Verify(k => k.GenerateJwt(), Times.Once);
        }

        [Fact]
        public async Task SyncCartItemsWithStocks_ShouldReturnFalse_WhenResponseListIsNull()
        {
            var usersMock = new Mock<IUsersBusiness>();
            var keyGenMock = new Mock<IInventoryKeyGenerator>();
            var cartDataMock = new Mock<ICartItemData>();

            usersMock.Setup(u => u.GetUserId()).Returns(10);

            var items = new List<NewOrderRequest>
            {
                new NewOrderRequest { StockId = 1, Quantity = 2 }
            };
            cartDataMock.Setup(c => c.GetCartItemQuantities(10)).ReturnsAsync(items);
            cartDataMock.Setup(c => c.SyncCartItemsCount(It.IsAny<DataTable>(), 10)).ReturnsAsync(true);

            keyGenMock.Setup(k => k.GenerateJwt()).Returns("fake_token");

            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent("null", Encoding.UTF8, "application/json")
                });

            var httpClient = new HttpClient(handlerMock.Object);

            var storeUrls = new StoreUrls { SyncOrderRequest = "http://fake-url.com" };

            var business = new CartItemBusiness(
                cartDataMock.Object,
                usersMock.Object,
                keyGenMock.Object,
                null,
                storeUrls,
                httpClient
            );

            var result = await business.SyncCartItemsWithStocks();

            Assert.False(result);

            cartDataMock.Verify(c => c.SyncCartItemsCount(It.IsAny<DataTable>(), It.IsAny<int>()), Times.Never);
            usersMock.Verify(u => u.GetUserId(), Times.AtLeastOnce);
            keyGenMock.Verify(k => k.GenerateJwt(), Times.Once);
        }


        [Fact]
        public async Task SyncCartItemsWithStocks_ShouldReturnTrue_WhenAllValid()
        {
            var usersMock = new Mock<IUsersBusiness>();
            var keyGenMock = new Mock<IInventoryKeyGenerator>();
            var cartDataMock = new Mock<ICartItemData>();

            usersMock.Setup(u => u.GetUserId()).Returns(10);

            var items = new List<NewOrderRequest>
            {
                new NewOrderRequest { StockId = 1, Quantity = 2 }
            };
            cartDataMock.Setup(c => c.GetCartItemQuantities(10)).ReturnsAsync(items);
            cartDataMock.Setup(c => c.SyncCartItemsCount(It.IsAny<DataTable>(), 10)).ReturnsAsync(true);

            keyGenMock.Setup(k => k.GenerateJwt()).Returns("fake_token");

            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(items))
                });

            var httpClient = new HttpClient(handlerMock.Object);

            var storeUrls = new StoreUrls { SyncOrderRequest = "http://fake-url.com" };

            var business = new CartItemBusiness(
                cartDataMock.Object,
                usersMock.Object,
                keyGenMock.Object,
                null,
                storeUrls,
                httpClient
            );

            var result = await business.SyncCartItemsWithStocks();

            Assert.True(result);

            cartDataMock.Verify(c => c.SyncCartItemsCount(It.IsAny<DataTable>(), 10), Times.Once);
            usersMock.Verify(u => u.GetUserId(), Times.AtLeastOnce);
            keyGenMock.Verify(k => k.GenerateJwt(), Times.Once);
        }



    }
}
