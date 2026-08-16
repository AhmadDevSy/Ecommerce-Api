using Business_Layer.Business;
using Models.DTO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Models.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Business_Layer.Services
{
    public class WarehouseService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<WarehouseService> _logger;

        public WarehouseService([FromKeyedServices("WarehouseService")] HttpClient httpClient, ILogger<WarehouseService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }



        public async Task<bool> SendAddQuantityRequestAsync(int productId, AddProductQuantityRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"api/product/{productId}", request);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send quantity request for Product {ProductId}", productId);
                return false;
            }
        }

        public async Task<bool> SendProductInfoToWarehouseAsync(ProductDTO dto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"api/product", dto);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send new product with Id = {ProductId} to warehouse", dto.Id);
                return false;
            }
        }

        public async Task<bool> ReserveProductsInWarehouseAsync(int orderId)
        {
            List<OrderItemDTO> items = await OrderItem.GetByOrderId(orderId);

            if (items == null || items.Count == 0)
            {
                return false;
            }

            try
            {
                var response = await _httpClient.PostAsJsonAsync($"api/order/{orderId}", items);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send new order info with OrderId = {OrderId} to warehouse", orderId);
                return false;
            }
        }

        public async Task<bool> ConfirmOrderInStore(int orderId)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"api/order/confirm/{orderId}", null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to confirm order with OrderId = {OrderId} to warehouse", orderId);
                return false;
            }
        }

        public async Task<bool> Health()
        {
            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

                var response = await _httpClient.GetAsync("api/health", cts.Token);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Warehouse service health check failed");
                return false;
            }
        }
    }
}
