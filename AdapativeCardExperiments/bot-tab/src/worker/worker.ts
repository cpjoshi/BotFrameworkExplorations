import KycRepository from '../storage/KycRepository';

export async function startSync() {
    var repo = new KycRepository();
    var pending = await repo.getPendingRecordsCount();
    if(pending > 0) {
        if(navigator.onLine) {
            repo.syncData(); 
        } else {
            console.log("***No Network, will retry shortly***");
            setTimeout(() => startSync(), 5000);
        }
    }
}

