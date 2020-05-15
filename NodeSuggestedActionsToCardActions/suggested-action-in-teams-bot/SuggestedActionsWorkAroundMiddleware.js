'use strict';

var __awaiter = (this && this.__awaiter) || function(thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function(resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator.throw(value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function(resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};

const { CardFactory } = require('botbuilder');
const { ActionTypes } = require('botframework-schema');

class SuggestedActionsWorkAroundMiddleware {
    onTurn(context, next) {
        return __awaiter(this, undefined, undefined, function * () {
            if (context === null) {
                throw new Error('context is null');
            }

            // Examine all incoming activities for clicks on CardAction buttons created by this middleware
            // Remove that card from chat pane.
            if (context.activity !== null && context.activity.channelId === 'msteams') {
                const activity = context.activity;
                if (activity.value && activity.value.addedBy && activity.value.addedBy === 'SuggestedActionsWorkAroundMiddleware') {
                    context.deleteActivity(context.activity.replyToId);
                }
            }

            // Examine all outgoing activities for presence of suggested actions
            // Convert suggested actions to card with action buttons
            context.onSendActivities((ctx, activities, nextSend) => __awaiter(this, undefined, undefined, function * () {
                var newActivities = [];
                activities.forEach((act) => {
                    if (act.channelId === 'msteams' && act.suggestedActions) {
                        // convert all CardAction buttons to MessageBack buttons
                        var card = CardFactory.heroCard('', [], this.toMessageBackActions(act.suggestedActions.actions));
                        var newActivity = this.createReply(act);
                        newActivity.attachments = [card];
                        newActivities.push(newActivity);
                        act.suggestedActions = null;
                    }
                });
                newActivities.forEach(act => activities.push(act));
                return yield nextSend();
            }));

            if (next !== null) {
                yield next();
            }
        });
    }

    createReply(activity) {
        return {
            conversation: activity.conversation,
            from: activity.from,
            replyToId: activity.replyToId,
            receipient: activity.receipient,
            serviceUrl: activity.serviceUrl,
            type: activity.type,
            channelId: activity.channelId
        };
    }

    toMessageBackActions(cardActionButtons) {
        var messageBackActions = [];
        cardActionButtons.forEach(button => {
            var msgAction = {
                type: ActionTypes.MessageBack,
                title: button.title,
                displayText: button.value,
                text: button.value,
                // Add marker: the card buttons are created by this middleware
                value: {
                    addedBy: 'SuggestedActionsWorkAroundMiddleware',
                    type: button.type
                }
            };
            messageBackActions.push(msgAction);
        });
        return messageBackActions;
    }
}
exports.SuggestedActionsWorkAroundMiddleware = SuggestedActionsWorkAroundMiddleware;
