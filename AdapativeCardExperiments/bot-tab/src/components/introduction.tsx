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
    testIndexDb: TestIndexedDb;

    constructor(props: {}) {
        super(props);
        microsoftTeams.initialize();
        this.testIndexDb = new TestIndexedDb("myjunkdb");
        this.onShowTaskModule = this.onShowTaskModule.bind(this);
        this.onPWAFeatureCheck = this.onPWAFeatureCheck.bind(this);
        this.openDeepLink = this.openDeepLink.bind(this);
        this.infiteLoop = this.infiteLoop.bind(this);
        this.onOpenSpecificDeepLink = this.onOpenSpecificDeepLink.bind(this);
        this.onSaveInIndexDb = this.onSaveInIndexDb.bind(this);

        this.state = {
            teamContext: null,
            pwaFeature: ""
        };

        microsoftTeams.getContext(context => {
            this.setState({ teamContext: context});
        });

        var navBarMenu: microsoftTeams.menus.MenuItem[] = [{
            id: "filter",
            title: "Filter",
            enabled: true,
            viewData: null as any,
            icon: "PHN2ZyB4bWxuczp4bGluaz0naHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmcnIHdpZHRoPSczMnB4JyBoZWlnaHQ9JzMycHgnIHZpZXdCb3g9JzAgMCAyOCAyOCcgdmVyc2lvbj0nMS4xJz48ZyBpZD0nUHJvZHVjdC1JY29ucycgc3Ryb2tlPSdub25lJyBzdHJva2Utd2lkdGg9JzEnIGZpbGw9J25vbmUnIGZpbGwtcnVsZT0nZXZlbm9kZCc+PGcgaWQ9J2ljX2ZsdWVudF9maWx0ZXJfMjhfcmVndWxhcicgZmlsbD0nIzIxMjEyMScgZmlsbC1ydWxlPSdub256ZXJvJz48cGF0aCBkPSdNMTcuMjUsMTkgQzE3LjY2NDIxMzYsMTkgMTgsMTkuMzM1Nzg2NCAxOCwxOS43NSBDMTgsMjAuMTY0MjEzNiAxNy42NjQyMTM2LDIwLjUgMTcuMjUsMjAuNSBMMTAuNzUsMjAuNSBDMTAuMzM1Nzg2NCwyMC41IDEwLDIwLjE2NDIxMzYgMTAsMTkuNzUgQzEwLDE5LjMzNTc4NjQgMTAuMzM1Nzg2NCwxOSAxMC43NSwxOSBMMTcuMjUsMTkgWiBNMjEuMjUsMTMgQzIxLjY2NDIxMzYsMTMgMjIsMTMuMzM1Nzg2NCAyMiwxMy43NSBDMjIsMTQuMTY0MjEzNiAyMS42NjQyMTM2LDE0LjUgMjEuMjUsMTQuNSBMNi43NSwxNC41IEM2LjMzNTc4NjQ0LDE0LjUgNiwxNC4xNjQyMTM2IDYsMTMuNzUgQzYsMTMuMzM1Nzg2NCA2LjMzNTc4NjQ0LDEzIDYuNzUsMTMgTDIxLjI1LDEzIFogTTI0LjI1LDcgQzI0LjY2NDIxMzYsNyAyNSw3LjMzNTc4NjQ0IDI1LDcuNzUgQzI1LDguMTY0MjEzNTYgMjQuNjY0MjEzNiw4LjUgMjQuMjUsOC41IEwzLjc1LDguNSBDMy4zMzU3ODY0NCw4LjUgMyw4LjE2NDIxMzU2IDMsNy43NSBDMyw3LjMzNTc4NjQ0IDMuMzM1Nzg2NDQsNyAzLjc1LDcgTDI0LjI1LDcgWicgaWQ9J0NvbG9yJy8+PC9nPjwvZz48L3N2Zz4="
        },
        {
            id: "newApproval",
            title: "New approval request",
            enabled: true,
            viewData: null as any,
            icon: "PHN2ZyB4bWxuczp4bGluaz0naHR0cDovL3d3dy53My5vcmcvMTk5OS94bGluaycgd2lkdGg9JzMycHgnIGhlaWdodD0nMzJweCcgdmlld0JveD0nMCAwIDI4IDI4Jz48ZyBpZD0nUHJvZHVjdC1JY29ucycgc3Ryb2tlPSdub25lJyBzdHJva2Utd2lkdGg9JzEnIGZpbGw9J25vbmUnIGZpbGwtcnVsZT0nZXZlbm9kZCc+PGcgaWQ9J2ljX2ZsdWVudF9hZGRfMjhfcmVndWxhcicgZmlsbD0nIzIxMjEyMScgZmlsbC1ydWxlPSdub256ZXJvJz48cGF0aCBkPSdNMTQuNSwxMyBMMTQuNSwzLjc1Mzc4NTc3IEMxNC41LDMuMzM5Nzg1NzcgMTQuMTY0LDMuMDAzNzg1NzcgMTMuNzUsMy4wMDM3ODU3NyBDMTMuMzM2LDMuMDAzNzg1NzcgMTMsMy4zMzk3ODU3NyAxMywzLjc1Mzc4NTc3IEwxMywxMyBMMy43NTM4NzU3MywxMyBDMy4zMzk4NzU3MywxMyAzLjAwMzg3NTczLDEzLjMzNiAzLjAwMzg3NTczLDEzLjc1IEMzLjAwMzg3NTczLDE0LjE2NCAzLjMzOTg3NTczLDE0LjUgMy43NTM4NzU3MywxNC41IEwxMywxNC41IEwxMywyMy43NTIzNjUxIEMxMywyNC4xNjYzNjUxIDEzLjMzNiwyNC41MDIzNjUxIDEzLjc1LDI0LjUwMjM2NTEgQzE0LjE2NCwyNC41MDIzNjUxIDE0LjUsMjQuMTY2MzY1MSAxNC41LDIzLjc1MjM2NTEgTDE0LjUsMTQuNSBMMjMuNzQ5ODI2MiwxNC41MDMwNzU0IEMyNC4xNjM4MjYyLDE0LjUwMzA3NTQgMjQuNDk5ODI2MiwxNC4xNjcwNzU0IDI0LjQ5OTgyNjIsMTMuNzUzMDc1NCBDMjQuNDk5ODI2MiwxMy4zMzkwNzU0IDI0LjE2MzgyNjIsMTMuMDAzMDc1NCAyMy43NDk4MjYyLDEzLjAwMzA3NTQgTDE0LjUsMTMgWicgaWQ9J0NvbG9yJy8+PC9nPjwvZz48L3N2Zz4="
        }
        ];
        microsoftTeams.menus.setNavBarMenu(navBarMenu, (s: string) => {
            console.log('received click of: ' + s);
            return true;
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

    async onSaveInIndexDb() {
        let saved = await this.testIndexDb.putValue("myjunktable", {
            name: 'test',
            email: 'test@email.com',
            phone: '98082316521',
            birthday: '01/02/1978'
        });

        alert('Saved: ' + JSON.stringify(saved));
    } 

    openChildWindow() {
        window.open(document.location.href)
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
                    <input type="button" onClick= { this.onSaveInIndexDb } value="Save Data in IndexDb" />
                </div>
                <div>
                    <input type="button" onClick= { this.openChildWindow } value="Open Child Window" />
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