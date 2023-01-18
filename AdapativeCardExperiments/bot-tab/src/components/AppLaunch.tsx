import React from 'react';
import * as microsoftTeams from '@microsoft/teams-js'
import { Button, CallVideoIcon, Flex, Segment, Text } from '@fluentui/react-northstar';
import { getBaseUrl, getAppId, GlobalVars } from '../ConfigureVariables';


export interface IAppState {
    teamContext: microsoftTeams.Context | null;
}

class AppLaunch extends React.Component<{}, IAppState> {
    constructor(props: {}) {
        super(props);
        microsoftTeams.initialize();
        this.onNetflixClicked = this.onNetflixClicked.bind(this);
        this.onPrimeClicked = this.onPrimeClicked.bind(this);
        this.onCheckShareToStage = this.onCheckShareToStage.bind(this);
        this.onSetSettings = this.onSetSettings.bind(this);
        this.onGetSettings = this.onGetSettings.bind(this);
        this.onGetAppContentStageSharingCapabilities = this.onGetAppContentStageSharingCapabilities.bind(this);

        this.state = {
            teamContext: null,
        };

        microsoftTeams.getContext(context => {
            console.log('getContext Success:', JSON.stringify(context));
            this.setState({ teamContext: context});
        });

        microsoftTeams.meeting.getMeetingDetails((error, meetingDetails) => {
            console.log('error: ' + JSON.stringify(error));
            console.log('meetingDetails: ' + JSON.stringify(meetingDetails))
        });
    }

    onNetflixClicked() {
        microsoftTeams.meeting.shareAppContentToStage(function (err, result) {
            console.log('received result:' + JSON.stringify(result));
            console.log('received error:' + JSON.stringify(err))
        }, `${getBaseUrl()}/launch?app=NetFlix`);
    }

    onPrimeClicked() {
        microsoftTeams.meeting.shareAppContentToStage(function (err, result) {
            console.log('received result:' + JSON.stringify(result));
            console.log('received error:' + JSON.stringify(err))
        }, `${getBaseUrl()}/launch?app=Prime`);

    }

    onCheckShareToStage() {
        microsoftTeams.meeting.getAppContentStageSharingState(function (err, result) {
            console.log('received result:' + JSON.stringify(result));
            console.log('received error:' + JSON.stringify(err))
        });
    }

    onGetAppContentStageSharingCapabilities() {
        microsoftTeams.meeting.getAppContentStageSharingCapabilities(function (err, result) {
            console.log('received result:' + JSON.stringify(result));
            console.log('received error:' + JSON.stringify(err))
        });
    }

    onSetSettings() {
        var tab = "CoWatch-" + Math.floor(Math.random() * 10) + 1;
        microsoftTeams.settings.setSettings({
            entityId: "myconfigTab",
            contentUrl: `${getBaseUrl()}/apps?host=msteams&from=config`,
            suggestedDisplayName: tab,
            websiteUrl: "https://botexplorations.azurefd.net"
        });
    }

    onGetSettings() {
        microsoftTeams.settings.getSettings(function (result) {
            console.log("Settings: ", JSON.stringify(result));
        });
    }

    render() {
        return (
            <div className="wrapper">
                <Flex column>
                    <br />
                    <br />
                    <Segment color="red" content="Watch Videos Together" inverted />
                    <div>
                    <br />
                    <Text color="brand" content={`${this.state.teamContext?.userPrincipalName}, What do you want to Co-Watch?`} size="large" />                        
                    <br />
                    <br />
                        <div>
                            <Button content="NetFlix" onClick={this.onNetflixClicked} icon={<CallVideoIcon />} fluid  />
                            <br />
                            <br />
                            <Button content="Amazon Prime" onClick={this.onPrimeClicked} icon={<CallVideoIcon />} fluid  />
                            <br />
                            <br />
                            <Button content="Check Share" onClick={this.onCheckShareToStage} icon={<CallVideoIcon />} fluid  />
                            <br />
                            <br />
                            <Button content="getAppContentStageSharingCapabilities" onClick={this.onGetAppContentStageSharingCapabilities} icon={<CallVideoIcon />} fluid  />
                            <br />
                            <Button content="Save Settings" onClick={this.onSetSettings} icon={<CallVideoIcon />} fluid  />
                            <br />
                            <Button content="Log GetSettings" onClick={this.onGetSettings} icon={<CallVideoIcon />} fluid  />

                        </div>
                </div>
                </Flex>            
            </div>
            );
    }
}

export default AppLaunch;