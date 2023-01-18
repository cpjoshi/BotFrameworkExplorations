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
            if (!sdkerr && files !== undefined) {
                console.log('image received at:' + Date.now());
                var src = "data:image/png;base64," + files[0].content;
                that.setState({ imgSrc: src });
            } else {
                if(files === undefined) {
                    console.log("filed object is undefined");
                }
                console.log("error while taking photo: " + JSON.stringify(sdkerr));
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
            //reg.sync.register(JSON.stringify(syncdata))
            //.catch((e: DOMException) => {
            //    if(e.message === 'Background Sync is disabled.') {
            //        this.startWebWorker();
            //    }
            //})
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