using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRchat.Models
{
    public class Game
    {
        public int GameId { get; set; }
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public int MovesLeft { get; } = 6;
        public bool Reset { get; set; }
    }
}
