using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RockPaperScissorGameBot.Models
{
    public class Game
    {
        public string GameId { get; set; }
        private Dictionary<string, PlayerChoice> PlayersScore;
        public Game(string GameId)
        {
            this.GameId = GameId;
            PlayersScore = new Dictionary<string, PlayerChoice>();
        }

        public void AddNewPlayer(string playerName)
        {
            PlayersScore.Add(playerName, null);
        }

        public void RecordPlayersChoice(PlayerChoice playerChoice)
        {
            if (!PlayersScore.ContainsKey(playerChoice.PlayerName))
                return;

            //this player has already recorded his choice, don't record it again
            if (PlayersScore[playerChoice.PlayerName] != null)
                return;

            PlayersScore[playerChoice.PlayerName] = playerChoice;
        }

        public bool isGameOver()
        {
            foreach (var player in PlayersScore.Keys)
            {
                //Some player has not recorded his choice yet, game not over yet
                if (PlayersScore[player] == null)
                    return false;
            }
            return true;
        }

        public Dictionary<string, int> GetAllPlayerScores()
        {
            var gameScoreForAllPlayers = new Dictionary<string, int>();
            PlayerChoice[] arr = PlayersScore.Values.ToArray();

            //initialize all player scores with 0
            for (int i = 0; i < arr.Length; i++)
            {
                gameScoreForAllPlayers.Add(arr[i].PlayerName, 0);
            }

            for (int i = 0; i < arr.Length; i++)
            {
                for (int j = i + 1; j < arr.Length; j++)
                {
                    PlayerChoice tmp = Whowon(arr[i], arr[j]);
                    if (tmp == null)
                    {
                        continue;
                    }
                    gameScoreForAllPlayers[tmp.PlayerName]++;
                }
            }
            return gameScoreForAllPlayers;
        }

        private PlayerChoice Whowon(PlayerChoice one, PlayerChoice two)
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
    }
}
