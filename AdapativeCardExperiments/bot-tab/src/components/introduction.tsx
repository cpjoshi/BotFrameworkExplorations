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
            /*card: "",*/
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
                Hello and Hi. {this.state.teamContext?.userPrincipalName}, this is your tab!
                <div>
                    Your userAgent is: {navigator.userAgent}
                </div>
                <div>
                    <Button content="Launch Task Module" onClick={this.onShowTaskModule} primary />
                </div>
                <div>
                    <a href="/sample.pdf" download>Click This to Download</a>
                </div>
                <div>
                    <a href="/sso?host=msteams">Visit sso page!</a>
                </div>
                <div>
                    <form>
                        <h1>Hello...</h1>
                        <p>Enter your name:</p>
                        <input type='text' name='username' />
                        <p>Enter your age:</p>
                        <input type='text' name='age' />
                        <p>Enter your brothers age:</p>
                        <input type='text' name='agebrother' />
                        <p>Enter your sisters age:</p>
                        <input type='text' name='agesister' />
                        <p>Enter your grandma age:</p>
                        <input type='text' name='agegrandma' />
                        <p>Enter your great grandfathers age:</p>
                        <input type='text' name='agefather' />
                        <p>Enter your age1:</p>
                        <input type='text' name='age1' />
                        <p>Enter your age2:</p>
                        <input type='text' name='age2' />
                        <p>Enter your age3:</p>
                        <input type='text' name='age3' />
                    </form>
                </div>
            </div>
            );
    }
}

export default Introduction;