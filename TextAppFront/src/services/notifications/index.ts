import { makeAutoObservable, runInAction } from "mobx";

export interface NotifyService {
    notificationList: NotificationStruct[],
    addNotification(type: string, title: string, body: string): any
}

export interface NotificationStruct {
    Type: string,
    Title: string,
    Body: string,
    ReceiveDate: Date,
    get timeElapsed(): number
}

export class Notifications implements NotifyService {
    notificationList: NotificationStruct[] = [] as NotificationStruct[];
    constructor(){
        makeAutoObservable(this);
    }

    addNotification(type: string, title: string, body: string) {
        const notify: NotificationStruct = {
            Type: type,
            Title: title,
            Body: body,
            ReceiveDate: new Date(),
            get timeElapsed(): number {
                return Math.round(new Date((new Date().getTime() - this.ReceiveDate.getTime())).getTime()/1000);
            }
        };
        runInAction(() => this.notificationList.push(notify));
    }
}