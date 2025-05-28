using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WDC_F24.infrastructure.Data;
using WDC_F24.Application;
using WDC_F24.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using WDC_F24.Application.Interfaces;
using WDC_F24.Application.DTOs.Responses;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Policy;
using WDC_F24.Application.DTOs.Requests.Orders;


namespace WDC_F24.infrastructure.Repositories
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;
        private readonly IRabbitMQPublisher _publisher;

        public OrderService(AppDbContext context, IRabbitMQPublisher publisher)
        {
            _context = context;
            _publisher = publisher;
        }

        public async Task<GeneralResponse> GetAllAsync()
        {
            try
            {
                var orders = await _context.Orders
                .Include(o => o.Items)
                .ToListAsync();

                return GeneralResponse.Ok("Orders retrieved", orders);
            }
            catch (DbUpdateException dbEx)
            {
                return GeneralResponse.BadRequest($"Database error: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (Exception ex)
            {
                return GeneralResponse.BadRequest($"Unexpected error: {ex.Message}");
            }
        }

        public async Task<GeneralResponse> GetUserOrdersAsync(Guid userId)
        {
            var orders = await _context.Orders
                .Include(o => o.Items)
                .Where(o => o.UserId == userId)
                .ToListAsync();

            return GeneralResponse.Ok("User orders retrieved", orders);
        }

        public async Task<GeneralResponse> GetByIdAsync(Guid id)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);

            return order == null
                ? GeneralResponse.NotFound("Order not found")
                : GeneralResponse.Ok("Order retrieved", order);
        }

        public async Task<GeneralResponse> AddAsync(CreateOrderRequestDto dto, Guid userId)
        {
            try
            {
                var order = new Order
                {
                    UserId = userId,
                    Items = dto.Items.Select(i => new OrderItem
                    {
                        ProductId = i.ProductId,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice
                    }).ToList()
                };
          
                 await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync();
                
                _publisher.Publish("order-created", new
                {
                    OrderId = order.Id,
                    UserId = userId,
                    Items = order.Items,
                    Message = $"Order {order.Id} was created successfully"
                } );

                return GeneralResponse.Ok("Order created", order);
            }
            catch (DbUpdateException dbEx)
            {
                return GeneralResponse.BadRequest($"Database error: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (Exception ex)
            {
                return GeneralResponse.BadRequest($"Unexpected error: {ex.Message}");
            }
        }

        public async Task<GeneralResponse> DeleteAsync(Guid id, Guid userid)
        {
            try
            {
                var order = await _context.Orders
                .Where(x=>x.UserId == userid)
             .Include(o => o.Items)
             .FirstOrDefaultAsync(o => o.Id == id);

                if (order == null)
                    return GeneralResponse.NotFound("Order not found");

                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();

                _publisher.Publish("order-deleted", new { order.Id , Message = $"Order {order.Id} was deleted successfully" }  );

                return GeneralResponse.Ok("Order deleted");
            }
            catch (DbUpdateException dbEx)
            {
                return GeneralResponse.BadRequest($"Database error: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (Exception ex)
            {
                return GeneralResponse.BadRequest($"Unexpected error: {ex.Message}");
            }
        }

        public async Task<GeneralResponse> UpdateStatusAsync(Guid id, string status)
        {
            try
            {
                var order = await _context.Orders.FindAsync(id);
                if (order == null)
                    return GeneralResponse.NotFound("Order not found");

                order.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                _publisher.Publish("order-status-updated", new { order.Id, status  , Message = $"Order {order.Id} was updated successfully" });

                return GeneralResponse.Ok("Order status updated");
            }
            catch (DbUpdateException dbEx)
            {
                return GeneralResponse.BadRequest($"Database error: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (Exception ex)
            {
                return GeneralResponse.BadRequest($"Unexpected error: {ex.Message}");
            }
        }
    }

}
