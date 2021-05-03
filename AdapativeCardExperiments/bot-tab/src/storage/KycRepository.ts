import IndexedDb from './IndexedDb'

class KycRepository {
    private dbName: string = "kycdb";
    private tableName: string = "userform";
    private processedRecords: string = "userFormProcessed";
    private kycdb: IndexedDb;
    private channel: BroadcastChannel;
    private kycFormPostUrl: string = "https://botexplorations.azurefd.net/api/v1/kycrecord";

    constructor() {
        this.channel = new BroadcastChannel('datasync');
        this.kycdb = new IndexedDb(this.dbName);
        this.kycdb.createObjectStore([this.tableName, this.processedRecords]);
    }

    public async syncData() {
        await this.kycdb.createObjectStore([this.tableName]);
        this.kycdb.getAllValue(this.tableName)
        .then(records => records.forEach((rec: any) => this.sendToServer(rec, this.kycdb)));
    }

    public async getPendingRecordsCount(): Promise<number> {
        await this.kycdb.createObjectStore([this.tableName]);
        var keys = await this.kycdb.getAllKeys(this.tableName);
        return keys.length;
    }

    public async enqueueForSync(kycRecord: any) {
        await this.kycdb.createObjectStore([this.tableName]);
        this.kycdb.putValue(this.tableName, kycRecord);
    }

    public async addProcessedRecord(kycRecord: any) {
        await this.kycdb.createObjectStore([this.processedRecords]);
        this.kycdb.putValue(this.processedRecords, kycRecord);
    }

    public async getProcessedRecordsCount(): Promise<number> {
        await this.kycdb.createObjectStore([this.processedRecords]);
        var keys = await this.kycdb.getAllKeys(this.processedRecords);
        return keys.length;
    }


    public async sendToServer(kycRecord: any, db: IndexedDb) {
        var myHeaders = new Headers();
        myHeaders.append("Content-Type", "application/json");
        
        var raw = JSON.stringify(kycRecord, ['name', 'email', 'phone', 'birthday']);
          
        return fetch(this.kycFormPostUrl, {method: 'post', headers: myHeaders, body: raw, redirect: 'follow'})
          .then(response => response.text())
          .then(result => {
            console.log("POST success!");
            this.addProcessedRecord({userId: JSON.parse(result).id});
            db.deleteValue('userform', kycRecord.id);
            this.channel.postMessage({synced: kycRecord.id});
          })
          .catch(error => console.log('error', error));
    }
}

export default KycRepository;