using OSBBProject1.Models;
using OSBBProject1.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OSBBProject1.Services
{
    public class ChangeHistoryService : IChangeHistoryService
    {
        private readonly ApplicationDbContext _context;

        public ChangeHistoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddChangeHistory(int id, string action, string changedBy, string entityType, int entityId, string oldValue, string newValue)
        {
            var changeHistory = new ChangeHistory
            {
                Id = id,
                Action = action, 
                ChangedBy = changedBy, 
                EntityType = entityType,
                EntityId = entityId, 
                OldValue = oldValue, 
                NewValue = newValue, 
                ChangeDate = DateTime.Now 
            };

            _context.ChangeHistories.Add(changeHistory); 
            await _context.SaveChangesAsync(); 
        }


        public async Task<List<ChangeHistory>> GetAllChangesAsync()
        {
            return await _context.ChangeHistories
                                  .OrderByDescending(c => c.ChangeDate)  
                                  .ToListAsync();
        }
    }
}
