import React from 'react';
import * as microsoftTeams from '@microsoft/teams-js'
import { Button, TextArea, Input } from '@fluentui/react-northstar';
import { getBaseUrl } from '../ConfigureVariables';

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
            title: "",
            height: 0,
            width: 0,
            url: "",
            card: "",
            fallbackUrl: "",
            completionBotId: "",
        };

        taskInfo.url = `${getBaseUrl()}/message?host=msteams`;
        taskInfo.title = "Sending Alert to your team";
        taskInfo.height = 510;
        taskInfo.width = 430;
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