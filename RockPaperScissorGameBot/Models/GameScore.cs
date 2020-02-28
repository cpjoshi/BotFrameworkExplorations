using Microsoft.Recognizers.Text.DateTime.Japanese;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace RockPaperScissorGameBot.Models
{
    public class PlayerChoice
    {
        public string PlayerName;
        public string Choice;
    }
    public class GameScore
    {
        private Dictionary<string, int> dict = new Dictionary<string, int>();
        private List<PlayerChoice> playerScores = new List<PlayerChoice>();
        public void AddScore(PlayerChoice playerChoice)
        {
            playerScores.Add(playerChoice);
            dict[playerChoice.PlayerName] = 0;
        }

        private PlayerChoice Whowon(PlayerChoice one, PlayerChoice two)
        {
            if(one.Choice == two.Choice)
            {
                //we have a draw
                return null;
            }

            switch(one.Choice)
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
        public Dictionary<string, int> GetPlayerScores()
        {
            PlayerChoice[] arr = playerScores.ToArray<PlayerChoice>();
            for (int i=0; i<arr.Length; i++)
            {
                for(int j=i+1; j<arr.Length; j++)
                {
                    PlayerChoice tmp = Whowon(arr[i], arr[j]);
                    if(tmp == null)
                    {
                        continue;
                    }
                    dict[tmp.PlayerName]++;
                }
            }
            return dict;
        }
    }
}
