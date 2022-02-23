import { Auth } from "../auth";
import { makeAutoObservable, observable, computed, action, flow, runInAction } from "mobx";
import { Chat } from '../chat';
import { Notifications } from '../notifications';

export class GlobalStore {
    notifyService: Notifications = new Notifications();
    authService: Auth = new Auth(this.notifyService);
    chatService: Chat = new Chat(this.notifyService);
    isAppLoaded: boolean = false;
    constructor(){
        makeAutoObservable(this);
    }

    async loadApp() {
        console.log("Application being loaded.");
        await this.authService.accountLoginToken();
        if(this.authService.isLogged)
        {
            await this.chatService.loadChats();
        }
        await new Promise(r => setTimeout(r, 1000));
    }
}

export const createGlobalStore = () => new GlobalStore();