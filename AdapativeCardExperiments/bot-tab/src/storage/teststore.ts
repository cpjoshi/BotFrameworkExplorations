import { IDBPDatabase, openDB } from 'idb';

class TestIndexedDb {
    private database: string;
    private db: any;

    constructor(database: string) {
        this.database = database;
    }

    public async SaveMap(tableName: string, myRecord: Map<string, any>) {
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

        const tx = this.db.transaction(tableName, 'readwrite');
        const store = tx.objectStore(tableName);
        const result = await store.put(myRecord);
        console.log('Put Data ', JSON.stringify(result));
        return result;
    }
}

export default TestIndexedDb;
