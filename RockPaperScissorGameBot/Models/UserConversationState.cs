using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RockPaperScissorGameBot.Models
{
    public class UserConversationState
    {
        public TeamsChannelAccount TeamMember { get; set; }
        public ConversationReference Conversation { get; set; }
        public string ActivityId { get; set; }
    }
}
