// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityHandler, MessageFactory } = require('botbuilder');

class EchoBot extends ActivityHandler {
    constructor() {
        super();
        // See https://aka.ms/about-bot-activity-message to learn more about the message and other activity types.
        this.onMessage(async (context, next) => {
            switch (context.activity.text) {
            case 'Start':
                await context.sendActivity(this.welcomeMessage());
                break;

            default: {
                const replyText = `Echo: ${ context.activity.text }`;
                await context.sendActivity(MessageFactory.text(replyText, replyText));
            }
            }
            // By calling next() you ensure that the next BotHandler is run.
            await next();
        });

        this.onMembersAdded(async (context, next) => {
            const membersAdded = context.activity.membersAdded;
            for (let cnt = 0; cnt < membersAdded.length; ++cnt) {
                if (membersAdded[cnt].id !== context.activity.recipient.id) {
                    await context.sendActivity(this.welcomeMessage());
                }
            }
            // By calling next() you ensure that the next BotHandler is run.
            await next();
        });
    }

    welcomeMessage() {
        return MessageFactory.suggestedActions(['Employee Profile', 'Apply Leave', 'See Payslip'], 'Hello and welcome, choose one of the actions below');
    }
}

module.exports.EchoBot = EchoBot;
