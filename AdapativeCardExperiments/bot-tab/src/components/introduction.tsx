import React from 'react';
import * as microsoftTeams from '@microsoft/teams-js'
import { Input, Button, TextArea } from '@fluentui/react-northstar';
import { getBaseUrl, getAppId, GlobalVars } from '../ConfigureVariables';
import * as FileSaver from 'file-saver';
import TestIndexedDb from '../storage/teststore';

export interface IAppState {
    teamContext: microsoftTeams.Context | null;
    pwaFeature: string;
}

class Introduction extends React.Component<{}, IAppState> {
    constructor(props: {}) {
        super(props);
        microsoftTeams.initialize();
        this.onShowTaskModule = this.onShowTaskModule.bind(this);
        this.onPWAFeatureCheck = this.onPWAFeatureCheck.bind(this);
        this.openDeepLink = this.openDeepLink.bind(this);
        this.infiteLoop = this.infiteLoop.bind(this);
        this.onOpenSpecificDeepLink = this.onOpenSpecificDeepLink.bind(this);

        this.state = {
            teamContext: null,
            pwaFeature: ""
        };

        microsoftTeams.getContext(context => {
            this.setState({ teamContext: context});
        });

        window.addEventListener('message', (event) => {console.log(event.data)});
    }

    examineWindow() {
        // If we are in an iframe, our parent window is the one hosting us (i.e., window.parent); otherwise,
        // it's the window that opened us (i.e., window.opener)
        GlobalVars.currentWindow = GlobalVars.currentWindow || window;
        GlobalVars.parentWindow =
            GlobalVars.currentWindow.parent !== GlobalVars.currentWindow.self
                ? GlobalVars.currentWindow.parent
                : GlobalVars.currentWindow.opener;

        if (!GlobalVars.parentWindow) {
            GlobalVars.isFramelessWindow = true;
        } else {
            // For iFrame scenario, add listener to listen 'message'
            console.log("GlobalVars.currentWindow.addEventListener('message', messageListener, false)");
        }

        console.log("window self: " + window.self);
        console.log("window parent: " + window.parent);
        console.log("window opener: " + window.opener);
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

    infiteLoop() {
        alert('starting infinite loop');
        for (;;) {};
    }

    onOpenTeams() {
        window.location.href = "msteams://";
    }

    onOpenSpecificDeepLink() {
        window.location.href = "msteams://teams.microsoft.com/l/channel/19%3a0d56e54bf27c4a23888c2c16c5ace5d7%40thread.skype/Mobile%2520Extensibility?groupId=32e3b156-66b2-4135-9aeb-73295a35a55b&tenantId=72f988bf-86f1-41af-91ab-2d7cd011db47";
    }

    onDownloadBlob() {
        var blob = new Blob(["Hello world using anchor click!"], { type: "text/plain;charset=utf-8" });
        FileSaver.saveAs(blob, "hello_world.txt");
    }

    onDownload() {
        var exportObj = { "name": "download_url_demo" };
        var dataStr = "data:text/json;charset=utf-8," + encodeURIComponent(JSON.stringify(exportObj));
        var downloadAnchorNode = document.createElement('a');
        downloadAnchorNode.setAttribute("href", dataStr);
        downloadAnchorNode.setAttribute("download", "MyFile.json");
        document.body.appendChild(downloadAnchorNode); // required for firefox
        downloadAnchorNode.click();
        downloadAnchorNode.remove();
    }

    async onPWAFeatureCheck() {
//        var myrecordMap = new Map<string, any>();
//        myrecordMap.set('1', {name: 'john'});
//        myrecordMap.set('2', {email: 'john@smith.com'});
//        var db = new TestIndexedDb('myJunkDB');
//        await db.SaveMap('myJunkTable', myrecordMap);

        //check PWA support
        if(navigator.serviceWorker) {
            navigator.serviceWorker.ready.then(reg => {
                const detectFeatures = {
                        'Offline Capabilities': 'caches' in window ,
                        'Push Notifications': 'pushManager' in reg ,
                        'Add_to_Home_Screen': document.createElement('link').relList.supports ('manifest') && 'onbeforeinstallprompt' in window,
                        'Background Sync': 'sync' in reg,
                        'Navigation Preload': 'navigationPreload' in reg,
                        'Storage Estimation': 'storage' in navigator && 'estimate' in navigator.storage,
                        'Media Session': 'mediaSession' in navigator,
                        'Media Capabilities': 'mediaCapabilities' in navigator,
                        'Payment Request': 'PaymentRequest' in window,
                        'Credential Management': 'credentials' in navigator,
                    };
                console.log(detectFeatures);
                this.setState({pwaFeature: JSON.stringify(detectFeatures)});
            });        
        } else {
            const detectFeatures = {
                'service worker': 'navigator.serviceWorker is undefined',
                'Offline Capabilities': 'caches' in window ,
                'Push Notifications': 'No',
                'Add_to_Home_Screen': document.createElement('link').relList.supports ('manifest') && 'onbeforeinstallprompt' in window,
                'Background Sync': 'No',
                'Navigation Preload': 'No',
                'Storage Estimation': 'storage' in navigator && 'estimate' in navigator.storage,
                'Media Session': 'mediaSession' in navigator,
                'Media Capabilities': 'mediaCapabilities' in navigator,
                'Payment Request': 'PaymentRequest' in window,
                'Credential Management': 'credentials' in navigator,
            };
            console.log('navigator.serviceworker is unavailable: ' + detectFeatures);
            this.setState({pwaFeature: JSON.stringify(detectFeatures)});
        }
    }

    onSWPing() {
        navigator.serviceWorker.ready.then(reg => {
            reg.active?.postMessage("Ping!");
        });
    }

    openDeepLink() {
        var str = (document.getElementById("deeplink")  as HTMLInputElement)!.value;
        microsoftTeams.executeDeepLink(str);
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
                    <Button content="Do infinite loop" onClick={this.infiteLoop} primary />
                </div>
                <div>
                    <a href="/sample.pdf" download>Click This to Download</a>
                </div>
                <div>
                    <a href="/sso?host=msteams">Visit sso page!</a>
                </div>
                <div>
                    <input type="text" id="deeplink"/>
                </div>
                <div>
                    <input type="button" onClick={this.openDeepLink} value="Open deeplink"/>
                </div>
                <div>
                    <input type="button" onClick={this.onOpenSpecificDeepLink} value="Open My Channel via Deeplink"/>
                </div>
                <div>
                    <a href="mailto:someone@yoursite.com?subject=Mail from Our Site">Email Us</a>
                </div>
                <div>
                    <a href="msteams://teams.microsoft.com/l/call/0/0?users=devansh@myappshop.onmicrosoft.com" className="button">TeamsCall</a>
                </div>
                <div>
                    <input type="button" onClick= { this.onOpenTeams } value="Open MSTeams Button" />
                </div>
                <div>
                    <input type="button" onClick={this.onDownloadBlob } value="Download using fileSaver" />
                </div>
                <div>
                    <input type="button" onClick={this.onDownload} value="Download using data:// uri" />
                </div>
                <div>
                    <TextArea disabled fluid placeholder="Features supported will appear here..." value={this.state.pwaFeature} rows={15} cols={30}/>
                </div>
                <div>
                    <Button content="Check PWA Features" onClick={this.onPWAFeatureCheck} primary />
                </div>
                <div>
                    <Button content="Ping Service Worker" onClick={this.onSWPing} primary />
                </div>
 
            </div>
            );
    }
}

export default Introduction;