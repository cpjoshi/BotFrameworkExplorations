import React from 'react';
import * as microsoftTeams from '@microsoft/teams-js'
import { Button, Flex, Segment, Text } from '@fluentui/react-northstar';

export interface IAppState {
    teamContext: microsoftTeams.Context | null;
    appName: String | null;
}

class LaunchConfirmation extends React.Component<{}, IAppState> {
    constructor(props: {}) {
        super(props);
        microsoftTeams.initialize();
        this.onAppLaunched = this.onAppLaunched.bind(this);
        this.onSpeakingStateChangeHandler = this.onSpeakingStateChangeHandler.bind(this);

        let urlParams = new URLSearchParams(window.location.search);
        let appNameFromUrl = urlParams.get('app');

        this.state = {
            teamContext: null,
            appName: `Open in ${appNameFromUrl} App`
        };

        microsoftTeams.getContext(context => {
            console.log('getContext Success:', JSON.stringify(context));
            this.setState({ teamContext: context});
        });
    }

    onAppLaunched() {
        alert("Launch app via deep link here....");
    }

    onSpeakingStateChangeHandler() {
        microsoftTeams.meeting.registerSpeakingStateChangeHandler((state: microsoftTeams.meeting.ISpeakingState) => {
            console.log('Speaking Status: ', state);
            console.log('Test accessing object properties: ', state.isSpeakingDetected);
        })
    }


    render() {
        return (
            <div className="wrapper">
                <Flex column>
                    <br />
                    <br />
                    <Segment color="red" content="Watch Videos Together" inverted />
                    <br />
                    <Text color="brand" content={`[cowatch.presenter] is hosting a Watch Party`} size="large" />
                    <br />
                    <div>
                        <Button content={this.state.appName} onClick={this.onAppLaunched} fluid />
                    </div> 
                    <div>
                        <Button content="SpeakingStateChangeHandler" onClick={this.onSpeakingStateChangeHandler} fluid />
                    </div> 

                </Flex>
            </div>

            );
    }
}

export default LaunchConfirmation;