using OSBBProject1.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace OSBBProject1.Services.Interfaces
{
    public interface IChangeHistoryService
    {
        Task AddChangeHistory(int id, string action, string changedBy, string entityType, int entityId, string oldValue, string newValue);

        Task<List<ChangeHistory>> GetAllChangesAsync();
    }
}
