using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRchat.Models.DataModels
{
    public interface IHangmanRepository
    {
        IQueryable<Question> Questions { get; }
    }
}
