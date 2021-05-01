using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRchat.Models.DataModels
{
    public class HangmanRepository : IHangmanRepository
    {
        private HangmanDbContext context;
        public HangmanRepository(HangmanDbContext ctx)
        {
            context = ctx;
        }
        public IQueryable<Question> Questions => context.Questions;
        //public IQueryable<Question> Questions => context.Questions.FromSqlRaw("EXEC dbo.SelectAllWords");

        //CREATE PROCEDURE SelectAllWords
        //AS
        //SELECT FROM Questions
        //GO;
    }
}
