import { IDBPDatabase, openDB } from 'idb';

class TestIndexedDb {
    private database: string;
    private db: any;

    constructor(database: string) {
        this.database = database;
    }

    public async initDb(tableName: string) {
        try {
            this.db = await openDB(this.database, 1, {
                upgrade(db: IDBPDatabase) {
                    if (!db.objectStoreNames.contains(tableName)) {
                        db.createObjectStore(tableName, { autoIncrement: true, keyPath: 'id' });                        
                    }
                },
            });
        } catch (error) {
            return false;
        }
    }

    public async SaveMap(tableName: string, myRecord: Map<string, any>) {
        await this.initDb(tableName);
        const tx = this.db.transaction(tableName, 'readwrite');
        const store = tx.objectStore(tableName);
        const result = await store.put(myRecord);
        console.log('Put Data ', JSON.stringify(result));
        return result;
    }

    public async getValue(tableName: string, id: number) {
        await this.initDb(tableName);
        const tx = this.db.transaction(tableName, 'readonly');
        const store = tx.objectStore(tableName);
        const result = await store.get(id);
        console.log('Get Data ', JSON.stringify(result));
        return result;
    }

    public async putValue(tableName: string, value: object) {
        await this.initDb(tableName);
        const tx = this.db.transaction(tableName, 'readwrite');
        const store = tx.objectStore(tableName);
        const result = await store.put(value);
        console.log('Put Item: ', JSON.stringify(result));
        return result;
    }
}

export default TestIndexedDb;
