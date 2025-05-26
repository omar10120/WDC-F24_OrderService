using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WDC_F24.Application.DTOs.Requests.Orders;
using WDC_F24.Application.DTOs.Responses;

namespace WDC_F24.Application.Interfaces
{
    public interface IOrderService
    {
        Task<GeneralResponse> GetAllAsync();
        Task<GeneralResponse> GetByIdAsync(Guid id);
        Task<GeneralResponse> GetUserOrdersAsync(Guid userId);
        Task<GeneralResponse> AddAsync(CreateOrderRequestDto order, Guid userId);
        Task<GeneralResponse> DeleteAsync(Guid id ,Guid userid);
        Task<GeneralResponse> UpdateStatusAsync(Guid id, string status);
    }

}
