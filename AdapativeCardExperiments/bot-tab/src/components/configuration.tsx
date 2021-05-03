import React from 'react';
import * as microsoftTeams from '@microsoft/teams-js'
import { Button, Input } from '@fluentui/react-northstar';
import { getBaseUrl } from '../ConfigureVariables';


class configuration extends React.Component {
    tabName: any;
    constructor(props: {}) {
        super(props);
        this.tabName = null;
        this.onAdd = this.onAdd.bind(this);
    }

    onAdd() {
        microsoftTeams.initialize();
        var tab = this.tabName?.value;
        microsoftTeams.settings.registerOnSaveHandler(function (saveEvent) {
            microsoftTeams.settings.setSettings({
                entityId: "myconfigTab",
                contentUrl: `${getBaseUrl()}/kycform?host=msteams&from=config`,
                suggestedDisplayName: tab,
                websiteUrl: "https://botexplorations.azurefd.net"
            });

            saveEvent.notifySuccess();
        });

        microsoftTeams.settings.setValidityState(true);
    }

    render() {
        return (
            <div>
                <div>
                    <Input fluid placeholder="type tab name here..." ref={(el:any) => this.tabName = el} />
                </div>
                <Button content="Add" onClick={this.onAdd} primary />
            </div>
        );
    }
}

export default configuration;