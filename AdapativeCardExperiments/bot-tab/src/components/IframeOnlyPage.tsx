import React from 'react';
import * as microsoftTeams from '@microsoft/teams-js'
import { Button, TextArea, Input } from '@fluentui/react-northstar';
import { IAppState } from './introduction';
import { getAppId } from '../ConfigureVariables';
import Iframe from 'react-iframe';

export class IFrameForm extends React.Component<{}, IAppState> {
    constructor(props: {}) {
        super(props);
        microsoftTeams.initialize();

        this.state = {
            teamContext: null
        };

        microsoftTeams.getContext(context => {
            this.setState({ teamContext: context });
        });
    }


    render() {
        return (
            <div>
                Hello {this.state.teamContext?.userPrincipalName}, We have an iframe Below.

                <Iframe url="https://botexplorations.azurefd.net/intro?host=msteams"
                    width="450px"
                    height="450px"
                    id="myId"
                    position="relative" />

            </div>
        );
    }
}