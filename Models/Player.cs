using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRchat.Models
{
    public class Player
    {
        public string Name { get; set; }
        public string ConnectionId { get; set; }
        public bool WaitingForTurn { get; set; }
        public bool IsPlaying { get; set; }
        public bool LookingForOpponent { get; set; }
        public Player Opponent { get; set; }
    }
}
