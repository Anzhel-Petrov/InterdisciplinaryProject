using Microsoft.AspNetCore.SignalR;
using SignalRchat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace SignalRChat.Hubs
{
    public class ChatHub : Hub
    {
        // create a list of players so we can register the connected players and also a list of games for we have a pool of games
        public static List<Player> players = new List<Player>();
        public static List<Game> games = new List<Game>();
        private object synchRoot = new object();
        public async Task SendAnswer(string shareAnswer)
        {
            await Clients.All.SendAsync("ReceiveAnswer", shareAnswer);
        }

        public async Task SendMistakes(int shareMistake)
        {
            await Clients.All.SendAsync("ReceiveMistakes", shareMistake);
        }

        public async Task SendWordStatus(string shareWordStatus)
        {
            await Clients.All.SendAsync("ReceiveWordStatus", shareWordStatus);
        }

        public async Task SendGuessedLetter(string[] shareGuessed, string shareLetter)
        {
            await Clients.All.SendAsync("ReceiveGuessedLetter", shareGuessed, shareLetter);
        }

        public async Task SendKeyboard(string shareKeyboard)
        {
            await Clients.All.SendAsync("ReceiveKeyboard", shareKeyboard);
        }

        public async Task ResetGuessed(string[] resetGuessed)
        {
            await Clients.All.SendAsync("ReceiveResetGuessed", resetGuessed);
        }
        // the client registration method
        public void RegisterClient(string data)
        {
            // make the thread safe 
            lock (synchRoot)
            {
                // go trough the list of registered players and look for the corrent connected client
                var client = players.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
                // if the client is not in the list create it as a new Player object and add it to the list of players
                if (client == null)
                {
                    client = new Player { ConnectionId = Context.ConnectionId, Name = data };
                    players.Add(client);
                }
                // change the boolean to false (it's set to false by default anyway) 
                client.IsPlaying = false;
            }
            // send a message to all connected clients - we use that to show who is joining the lobby
            Clients.All.SendAsync("ShowUsers", players);
        }

        // the player matching method
        public void FindOpponent()
        {
            // go trough the list of registered players and look for the player who invoked the method (current connection)
            var firstPlayer = players.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (firstPlayer == null) return;
            // change the player boolean to true - he is looking for another player
            firstPlayer.LookingForOpponent = true;

            // look for a second player in the list of player who is not the current player (first player), is not playing a game and is also looking for a player
            // the guid should act as a random player match - it should get the first player from a list of players by assigning a random guid to each of them
            var secondPlayer = players.Where(x => x.ConnectionId != Context.ConnectionId && x.LookingForOpponent && !x.IsPlaying).OrderBy(x => Guid.NewGuid()).FirstOrDefault();
            // if there is no second player then send a message to the first player that he is in a queue and waiting for another player
            if (secondPlayer == null)
            {
                Clients.Caller.SendAsync("noOpponent", "We Are Looking For The Best Teammate!");
            }
            // if there are two players they are matched and we change the booleans accordingly
            else
            {
                firstPlayer.IsPlaying = true;
                firstPlayer.LookingForOpponent = false;
                // here we say that the first player is always going to be picking a letter first
                firstPlayer.WaitingForTurn = false;

                secondPlayer.IsPlaying = true;
                secondPlayer.LookingForOpponent = false;
                secondPlayer.WaitingForTurn = true;

                firstPlayer.Opponent = secondPlayer;
                secondPlayer.Opponent = firstPlayer;
                // send messages to the player to inform them who is their teammember
                Clients.Caller.SendAsync("foundOpponent", "We found your teammate - " + secondPlayer.Name);
                Clients.Client(secondPlayer.ConnectionId).SendAsync("foundOpponent", "We found your teammate - " + firstPlayer.Name);
                // we disable the input for one of the players and inform him that it's the other player's turn
                Clients.Client(firstPlayer.ConnectionId).SendAsync("disableKey");
                Clients.Caller.SendAsync("info", "It's " + secondPlayer.Name + " turn.");

                // add the players to a list of games
                lock (synchRoot)
                {
                    games.Add(new Game { Player1 = firstPlayer, Player2 = secondPlayer });
                }
            }
        }
        public void Play()
        {
            // identify the game by looking in the list of games and comparing the players connectionIds with the current conection id - if one of the two players 
            // match the current connectionid then this is the game that is being played
            var game = games.FirstOrDefault(x => x.Player1.ConnectionId == Context.ConnectionId || x.Player2.ConnectionId == Context.ConnectionId);
            // the turn based logic
            // if player1 is in turn then...
            if (game.Player1.WaitingForTurn == false)
            {
                //...enable his keyboard and disable it for the other player
                Clients.Client(game.Player1.ConnectionId).SendAsync("enableKey");
                Clients.Client(game.Player2.ConnectionId).SendAsync("disableKey");
                // switch the booleans so player one is waiting for a turn and player2 is in turn
                game.Player1.WaitingForTurn = true;
                game.Player2.WaitingForTurn = false;
                // send a message to the player who is waiting that it's the other player turn
                Clients.Caller.SendAsync("info", "It's " + game.Player1.Name + " turn.");
                Clients.Client(game.Player1.ConnectionId).SendAsync("info", "");
            }
            // if player2 is in turn then (same logix as above but reversed)
            else if (game.Player2.WaitingForTurn == false)
            {
                Clients.Client(game.Player1.ConnectionId).SendAsync("disableKey");
                Clients.Client(game.Player2.ConnectionId).SendAsync("enableKey");
                game.Player2.WaitingForTurn = true;
                game.Player1.WaitingForTurn = false;
                Clients.Caller.SendAsync("info", "It's " + game.Player2.Name + " turn.");
                Clients.Client(game.Player2.ConnectionId).SendAsync("info", "");
            }
        }
    }
}