import React from 'react';
import * as microsoftTeams from '@microsoft/teams-js';
import { Button, Input, Form, Image } from '@fluentui/react-northstar';
import Worker from '../worker';
import KycRepository from '../storage/KycRepository';

const instanceWorker = new Worker();

export interface IAppState {
    teamContext: microsoftTeams.Context | null;
    imgSrc: string;
    name: string;
    email: string;
    phone: string;
    birthday: Date;
    pending: number;
    completed: number;
}

export class kycform extends React.Component<{}, IAppState> {
    channel: BroadcastChannel;
    kycRecordsRepository: KycRepository;

    constructor(props: {}) {
        super(props);
        microsoftTeams.initialize();
        this.kycRecordsRepository = new KycRepository();

        this.state = {
            teamContext: null,
            imgSrc: '/take-a-photo.png',
            name: 'CPJoshi',
            email: 'cpj@mymail.com',
            phone: '726317863',
            birthday: new Date("2011-01-16"),
            pending: 0,
            completed: 0
        };

        microsoftTeams.getContext(context => {
            this.setState({ teamContext: context });
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
            },
            {
                id: "changeApproval",
                title: "Modify approval request",
                enabled: true,
                viewData: null as any,
                icon: "PHN2ZyB4bWxuczp4bGluaz0naHR0cDovL3d3dy53My5vcmcvMTk5OS94bGluaycgd2lkdGg9JzMycHgnIGhlaWdodD0nMzJweCcgdmlld0JveD0nMCAwIDI4IDI4Jz48ZyBpZD0nUHJvZHVjdC1JY29ucycgc3Ryb2tlPSdub25lJyBzdHJva2Utd2lkdGg9JzEnIGZpbGw9J25vbmUnIGZpbGwtcnVsZT0nZXZlbm9kZCc+PGcgaWQ9J2ljX2ZsdWVudF9hZGRfMjhfcmVndWxhcicgZmlsbD0nIzIxMjEyMScgZmlsbC1ydWxlPSdub256ZXJvJz48cGF0aCBkPSdNMTQuNSwxMyBMMTQuNSwzLjc1Mzc4NTc3IEMxNC41LDMuMzM5Nzg1NzcgMTQuMTY0LDMuMDAzNzg1NzcgMTMuNzUsMy4wMDM3ODU3NyBDMTMuMzM2LDMuMDAzNzg1NzcgMTMsMy4zMzk3ODU3NyAxMywzLjc1Mzc4NTc3IEwxMywxMyBMMy43NTM4NzU3MywxMyBDMy4zMzk4NzU3MywxMyAzLjAwMzg3NTczLDEzLjMzNiAzLjAwMzg3NTczLDEzLjc1IEMzLjAwMzg3NTczLDE0LjE2NCAzLjMzOTg3NTczLDE0LjUgMy43NTM4NzU3MywxNC41IEwxMywxNC41IEwxMywyMy43NTIzNjUxIEMxMywyNC4xNjYzNjUxIDEzLjMzNiwyNC41MDIzNjUxIDEzLjc1LDI0LjUwMjM2NTEgQzE0LjE2NCwyNC41MDIzNjUxIDE0LjUsMjQuMTY2MzY1MSAxNC41LDIzLjc1MjM2NTEgTDE0LjUsMTQuNSBMMjMuNzQ5ODI2MiwxNC41MDMwNzU0IEMyNC4xNjM4MjYyLDE0LjUwMzA3NTQgMjQuNDk5ODI2MiwxNC4xNjcwNzU0IDI0LjQ5OTgyNjIsMTMuNzUzMDc1NCBDMjQuNDk5ODI2MiwxMy4zMzkwNzU0IDI0LjE2MzgyNjIsMTMuMDAzMDc1NCAyMy43NDk4MjYyLDEzLjAwMzA3NTQgTDE0LjUsMTMgWicgaWQ9J0NvbG9yJy8+PC9nPjwvZz48L3N2Zz4="
            }
        ];
        microsoftTeams.menus.setNavBarMenu(navBarMenu, (s: string) => {
            console.log('received click of: ' + s);
            return true;
        });

        });

        this.channel = new BroadcastChannel('datasync');
        this.takePhoto = this.takePhoto.bind(this);
        this.saveLocal = this.saveLocal.bind(this);
        this.startWebWorker = this.startWebWorker.bind(this);
        this.startServiceWorker = this.startServiceWorker.bind(this);
        this.updateUIState = this.updateUIState.bind(this);
        
        this.channel.onmessage = e => {
            this.updateUIState();
            console.log("BroadCastChannel: " + e);
        }
        this.updateUIState();
        this.startWebWorker();
    }

    takePhoto() {
        var that = this;
        microsoftTeams.media.captureImage((sdkerr, files) => {
            if (!sdkerr) {
                var src = "data:image/png;base64," + files[0].content;
                that.setState({ imgSrc: src });
            }
        })
    }

    async saveLocal(e: any) {
        var quota = await navigator.storage.estimate();
        var totalSpace = quota.quota;
        var usedSpace = quota.usage;
        console.log("TotalSpace: " + totalSpace + " UsedSapce:" + usedSpace);
        
        await this.kycRecordsRepository.enqueueForSync({
            name: this.state.name,
            email: this.state.email,
            phone: this.state.phone,
            birthday: this.state.birthday,
            imgSrc: this.state.imgSrc
        });

        this.updateUIState();
        this.startWebWorker();
        //this.startServiceWorker();

        var navBarMenu: microsoftTeams.menus.MenuItem[] = [{
            id: "filter",
            title: "Filter",
            enabled: false,
            viewData: null as any,
            icon: "PHN2ZyB4bWxuczp4bGluaz0naHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmcnIHdpZHRoPSczMnB4JyBoZWlnaHQ9JzMycHgnIHZpZXdCb3g9JzAgMCAyOCAyOCcgdmVyc2lvbj0nMS4xJz48ZyBpZD0nUHJvZHVjdC1JY29ucycgc3Ryb2tlPSdub25lJyBzdHJva2Utd2lkdGg9JzEnIGZpbGw9J25vbmUnIGZpbGwtcnVsZT0nZXZlbm9kZCc+PGcgaWQ9J2ljX2ZsdWVudF9maWx0ZXJfMjhfcmVndWxhcicgZmlsbD0nIzIxMjEyMScgZmlsbC1ydWxlPSdub256ZXJvJz48cGF0aCBkPSdNMTcuMjUsMTkgQzE3LjY2NDIxMzYsMTkgMTgsMTkuMzM1Nzg2NCAxOCwxOS43NSBDMTgsMjAuMTY0MjEzNiAxNy42NjQyMTM2LDIwLjUgMTcuMjUsMjAuNSBMMTAuNzUsMjAuNSBDMTAuMzM1Nzg2NCwyMC41IDEwLDIwLjE2NDIxMzYgMTAsMTkuNzUgQzEwLDE5LjMzNTc4NjQgMTAuMzM1Nzg2NCwxOSAxMC43NSwxOSBMMTcuMjUsMTkgWiBNMjEuMjUsMTMgQzIxLjY2NDIxMzYsMTMgMjIsMTMuMzM1Nzg2NCAyMiwxMy43NSBDMjIsMTQuMTY0MjEzNiAyMS42NjQyMTM2LDE0LjUgMjEuMjUsMTQuNSBMNi43NSwxNC41IEM2LjMzNTc4NjQ0LDE0LjUgNiwxNC4xNjQyMTM2IDYsMTMuNzUgQzYsMTMuMzM1Nzg2NCA2LjMzNTc4NjQ0LDEzIDYuNzUsMTMgTDIxLjI1LDEzIFogTTI0LjI1LDcgQzI0LjY2NDIxMzYsNyAyNSw3LjMzNTc4NjQ0IDI1LDcuNzUgQzI1LDguMTY0MjEzNTYgMjQuNjY0MjEzNiw4LjUgMjQuMjUsOC41IEwzLjc1LDguNSBDMy4zMzU3ODY0NCw4LjUgMyw4LjE2NDIxMzU2IDMsNy43NSBDMyw3LjMzNTc4NjQ0IDMuMzM1Nzg2NDQsNyAzLjc1LDcgTDI0LjI1LDcgWicgaWQ9J0NvbG9yJy8+PC9nPjwvZz48L3N2Zz4="
        },
        {
            id: "newApproval",
            title: "New approval request",
            enabled: false,
            viewData: null as any,
            icon: "PHN2ZyB4bWxuczp4bGluaz0naHR0cDovL3d3dy53My5vcmcvMTk5OS94bGluaycgd2lkdGg9JzMycHgnIGhlaWdodD0nMzJweCcgdmlld0JveD0nMCAwIDI4IDI4Jz48ZyBpZD0nUHJvZHVjdC1JY29ucycgc3Ryb2tlPSdub25lJyBzdHJva2Utd2lkdGg9JzEnIGZpbGw9J25vbmUnIGZpbGwtcnVsZT0nZXZlbm9kZCc+PGcgaWQ9J2ljX2ZsdWVudF9hZGRfMjhfcmVndWxhcicgZmlsbD0nIzIxMjEyMScgZmlsbC1ydWxlPSdub256ZXJvJz48cGF0aCBkPSdNMTQuNSwxMyBMMTQuNSwzLjc1Mzc4NTc3IEMxNC41LDMuMzM5Nzg1NzcgMTQuMTY0LDMuMDAzNzg1NzcgMTMuNzUsMy4wMDM3ODU3NyBDMTMuMzM2LDMuMDAzNzg1NzcgMTMsMy4zMzk3ODU3NyAxMywzLjc1Mzc4NTc3IEwxMywxMyBMMy43NTM4NzU3MywxMyBDMy4zMzk4NzU3MywxMyAzLjAwMzg3NTczLDEzLjMzNiAzLjAwMzg3NTczLDEzLjc1IEMzLjAwMzg3NTczLDE0LjE2NCAzLjMzOTg3NTczLDE0LjUgMy43NTM4NzU3MywxNC41IEwxMywxNC41IEwxMywyMy43NTIzNjUxIEMxMywyNC4xNjYzNjUxIDEzLjMzNiwyNC41MDIzNjUxIDEzLjc1LDI0LjUwMjM2NTEgQzE0LjE2NCwyNC41MDIzNjUxIDE0LjUsMjQuMTY2MzY1MSAxNC41LDIzLjc1MjM2NTEgTDE0LjUsMTQuNSBMMjMuNzQ5ODI2MiwxNC41MDMwNzU0IEMyNC4xNjM4MjYyLDE0LjUwMzA3NTQgMjQuNDk5ODI2MiwxNC4xNjcwNzU0IDI0LjQ5OTgyNjIsMTMuNzUzMDc1NCBDMjQuNDk5ODI2MiwxMy4zMzkwNzU0IDI0LjE2MzgyNjIsMTMuMDAzMDc1NCAyMy43NDk4MjYyLDEzLjAwMzA3NTQgTDE0LjUsMTMgWicgaWQ9J0NvbG9yJy8+PC9nPjwvZz48L3N2Zz4="
        }
        ];
        microsoftTeams.menus.setNavBarMenu(navBarMenu, (s: string) => {
            console.log('received click of: ' + s);
            return true;
        });

        e.preventDefault();
    }

    async updateUIState() {
        this.setState({
            pending: await this.kycRecordsRepository.getPendingRecordsCount(),
            completed: await this.kycRecordsRepository.getProcessedRecordsCount()
        });
    }
    
    startWebWorker() {
        console.log("Invoking web worker here...");
        instanceWorker.startSync();
    }

    startServiceWorker() {
        navigator.serviceWorker.ready.then(reg => {
            var authtoken = "this_is_sample_token";
            var syncdata = {"tag": "kyc-sync", "authtoken":authtoken};
            reg.sync.register(JSON.stringify(syncdata))
            .catch((e: DOMException) => {
                if(e.message === 'Background Sync is disabled.') {
                    this.startWebWorker();
                }
            })
        });
    }

    render() {
        return (
            <div className="wrapper">
                <div className="wrapper-row">
                    <button className='pending'>Pending: {this.state.pending}</button>
                    <button className='completed' >Processed: {this.state.completed}</button>
                </div>
            <Form onSubmit={this.saveLocal}>
                <Image id="kycphoto" width="150" height="150" alt="Take the photo" src={this.state.imgSrc} onClick={this.takePhoto} /> 
                <Form.Input fluid clearable
                    label="Name"
                    name="name"
                    id="name"
                    key="name"
                    value={this.state.name}
                    successIndicator="true"
                    onChange={(e: any) => {
                        this.setState({ name: e.target.value });
                    }}
                />
                <Form.Input fluid clearable
                    label="Email"
                    name="email"
                    id="email"
                    key="email"
                    type="email"
                    value={this.state.email}
                    onChange= {(e: any) => {
                        this.setState({ email: e.target.value });
                    }}
                />
                <Form.Input fluid clearable
                    label="Phone"
                    name="phone"
                    id="phone"
                    key="phone"
                    type="phone"
                    value={this.state.phone}
                    onChange={(e: any) => {
                        this.setState({ phone: e.target.value });
                    }}
                />

                <Form.Input fluid
                    label="Birthday"
                    name="birthday"
                    id="birthday"
                    key= "birthday"
                    type="date"
                    onChange= {(e: any) => {
                        this.setState({ birthday: e.target.value });
                    }}
                />
                <Form.Field control={{ as: Button, content: 'Submit' }} />
            </Form>
            </div>
        );
    }
}