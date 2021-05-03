import React from 'react';
import * as microsoftTeams from '@microsoft/teams-js'
import { Button } from '@fluentui/react-northstar';
import { getAppId } from '../ConfigureVariables';

export interface IAppState {
    teamContext: microsoftTeams.Context | null
}

export class MessageForm extends React.Component<{}, IAppState> {
    constructor(props: {}) {
        super(props);
        this.onClickSeverity1 = this.onClickSeverity1.bind(this);
        this.onClickSeverity2 = this.onClickSeverity2.bind(this);
        this.onClickSeverity3 = this.onClickSeverity3.bind(this);
        microsoftTeams.initialize();

        this.state = {
            teamContext: null
        };

        microsoftTeams.getContext(context => {
            this.setState({ teamContext: context });
        });
    }

    onClickSeverity1() {
        let alertmsg = {
            "severity": "Severity1"
        };
        microsoftTeams.tasks.submitTask(alertmsg, getAppId());
    }

    onClickSeverity2() {
        let alertmsg = {
            "severity": "Severity2"
        };
        microsoftTeams.tasks.submitTask(alertmsg, getAppId());
    }

    onClickSeverity3() {
        let alertmsg = {
            "severity": "Severity3"
        };
        microsoftTeams.tasks.submitTask(alertmsg, getAppId());
    }

    render() {
        return (
            <div>
                Hello {this.state.teamContext?.userPrincipalName}, Send Alert message to your team members.
                <div>
                    <Button content="Severity-1" onClick={this.onClickSeverity1} primary />
                </div>
                <div>
                    <Button content="Severity-2" onClick={this.onClickSeverity2} primary />
                </div>
                <div>
                    <Button content="Severity-3" onClick={this.onClickSeverity3} primary />
                </div>
            </div>
        );
    }
}