using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRchat.Models.DataModels
{
    public class HangmanDbContext : DbContext
    {
        public HangmanDbContext(DbContextOptions<HangmanDbContext> options) : base(options) { }
        public DbSet<Question> Questions { get; set; }
    }
}
