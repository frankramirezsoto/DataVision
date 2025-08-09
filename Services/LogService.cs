using DataVision.Data;
using DataVision.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace DataVision.Services
{
    public class LogService
    {
        private readonly ApplicationDbContext _context;

        public LogService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateLogAsync(int userId, string endpoint)
        {
            var log = new Log
            {
                UserId = userId,
                EndpointConsultado = endpoint,
                FechaConsulta = DateTime.UtcNow
            };

            _context.Logs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
