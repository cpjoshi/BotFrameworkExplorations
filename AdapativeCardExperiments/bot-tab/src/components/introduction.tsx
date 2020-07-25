import React from 'react';
import * as microsoftTeams from '@microsoft/teams-js'
import { Button, TextArea, Input } from '@fluentui/react-northstar';
import { getBaseUrl, getAppId } from '../ConfigureVariables';

export interface IAppState {
    teamContext: microsoftTeams.Context | null;
}

class Introduction extends React.Component<{}, IAppState> {
    constructor(props: {}) {
        super(props);
        microsoftTeams.initialize();
        this.onShowTaskModule = this.onShowTaskModule.bind(this);

        this.state = {
            teamContext: null
        };

        microsoftTeams.getContext(context => {
            this.setState({ teamContext: context});
        });
    }

    onShowTaskModule() {
        let taskInfo: microsoftTeams.TaskInfo = {
            title: "Sending Alert to your team",
            height: 510,
            width: 500,
            url: `${getBaseUrl()}/message?host=msteams`,
            card: "",
            fallbackUrl: "",
            completionBotId: getAppId(),
        };

        let submitHandler = (err: any, result: any) => {
            console.log(`Submit handler - err: ${err}`);
            console.log(`Submit handler - result: ${result}`);
        };

        microsoftTeams.tasks.startTask(taskInfo, submitHandler);
    }


    render() {
        return (
            <div>
                Hello {this.state.teamContext?.userPrincipalName}, this is your tab!
                <div>
                    Your userAgent is: {navigator.userAgent}
                </div>
                <div>
                    <Button content="Launch Task Module" onClick={this.onShowTaskModule} primary />
                </div>
            </div>
            );
    }
}

export default Introduction;