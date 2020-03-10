using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RockPaperScissorGameBot.Models
{
    public class Game: IEnumerable<Player>
    {
        public string GameId { get; set; }
        private Dictionary<string, Player> Players;
        public Game(string GameId)
        {
            this.GameId = GameId;
            Players = new Dictionary<string, Player>();
        }

        public Player AddNewPlayer(string playerName, string playerId)
        {
            var player = new Player() { PlayerId = playerId, PlayerName = playerName };
            Players.Add(playerName, player) ;
            return player;
        }

        public Player GetPlayer(string playerId)
        {
            return Players[playerId];
        }

        /// <summary>
        /// A player has pressed rock/paper/scissor button
        /// </summary>
        /// <param name="playerChoice"></param>
        public void RecordPlayersChoice(string playerName, string playerChoice)
        {
            //unknown player, don't record this
            if (!Players.ContainsKey(playerName))
                return;

            //player has already submitted his choice in the past, don't record it again
            if (Players[playerName].Choice != null)
                return;

            Players[playerName].Choice = playerChoice;
        }

        /// <summary>
        /// Returns true when all players have recorded their choice
        /// </summary>
        /// <returns></returns>
        public bool IsGameOver()
        {
            foreach (var player in Players.Keys)
            {
                //Some player has not recorded his choice yet, game not over yet
                if (Players[player].Choice == null)
                    return false;
            }
            return true;
        }

        public Dictionary<string, int> GetAllPlayerScores()
        {
            var gameScoreForAllPlayers = new Dictionary<string, int>();
            Player[] arr = Players.Values.ToArray();

            //initialize all player scores with 0
            for (int i = 0; i < arr.Length; i++)
            {
                gameScoreForAllPlayers.Add(arr[i].PlayerName, 0);
            }

            for (int i = 0; i < arr.Length; i++)
            {
                for (int j = i + 1; j < arr.Length; j++)
                {
                    Player winner = Whowon(arr[i], arr[j]);
                    if (winner == null)
                    {
                        continue;
                    }
                    gameScoreForAllPlayers[winner.PlayerName]++;
                }
            }
            return gameScoreForAllPlayers;
        }

        private Player Whowon(Player one, Player two)
        {
            if (one.Choice == two.Choice)
            {
                //we have a draw
                return null;
            }

            switch (one.Choice)
            {
                case "rock":
                    {
                        return two.Choice == "scissor" ? one : two;
                    }
                case "paper":
                    {
                        return two.Choice == "rock" ? one : two;
                    }
                case "scissor":
                    {
                        return two.Choice == "paper" ? one : two;
                    }
                default:
                    return null;
            }
        }

        public IEnumerator<Player> GetEnumerator()
        {
            foreach(var player in Players.Values)
            {
                yield return player;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
