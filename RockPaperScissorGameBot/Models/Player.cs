using Microsoft.Bot.Schema;

namespace RockPaperScissorGameBot.Models
{
    public class Player
    {
        public string PlayerId { get; set; }
        public string PlayerName { get; set; }
        /// <summary>
        /// One of the following strings: rock, paper, scissor
        /// </summary>
        public string Choice { get; set; }
        public UserConversationState userConversationState { get; set; }
    }
}
