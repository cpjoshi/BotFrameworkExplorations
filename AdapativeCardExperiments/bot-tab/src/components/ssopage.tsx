import React from 'react';
import * as microsoftTeams from '@microsoft/teams-js'
import { Button, TextArea, Input } from '@fluentui/react-northstar';

export interface AuthResult {
    result: string;
    authdone: boolean;
    apiresult: string;
}

export default class SSOPage extends React.Component<{}, AuthResult> {
    apiUrl: any;
    constructor(props: {}) {
        super(props);
        this.apiUrl = null;
        this.state = { result: "", authdone: false, apiresult: "" };
        this.onSignIn = this.onSignIn.bind(this);
        this.onMakeCall = this.onMakeCall.bind(this);
    }

    onSignIn() {
        microsoftTeams.initialize();

        let that = this;

        const authTokenRequest = {
            resources: ["api://botexplorations.azurefd.net/14a6686e-e903-4e55-b945-dc2472381849"],
            successCallback: function (token: string) { that.setState({ result: token, authdone: true }); },
            failureCallback: function (error: string) { that.setState({ result: error, authdone: false }); },
        };

        microsoftTeams.authentication.getAuthToken(authTokenRequest);
    }

    onMakeCall() {
        fetch(this.apiUrl?.state.value, {
            method: "POST",
            headers: {
                Accept: "gzip, deflate, br",
                "Content-Type": "application/json",
                Authorization: "Bearer " + this.state.result
            },
            body: "[]"
        }).then(res => {
            this.setState({
                apiresult: JSON.stringify(res)
            })
        });
    }

    render() {
        return (
            <div>
                <div>
                    <TextArea disabled fluid placeholder="Your token will appear here" value={this.state.result} />
                </div>

                <Button content="Sign In" onClick={this.onSignIn} primary />
                {this.state.authdone && 
                    <div>
                    <Input fluid placeholder="type graph api url here..." ref={ el => this.apiUrl=el }/>
                    <Button content="Call" onClick={this.onMakeCall} primary />
                    <TextArea disabled fluid placeholder="Results will appear here" value={this.state.apiresult} />
                    </div>
                }
            </div>
        );
    }
}