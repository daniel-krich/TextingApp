import { Auth } from "../auth";
import { makeAutoObservable, observable, computed, action, flow, runInAction } from "mobx";
import { Chat } from '../chat';

export class GlobalStore {
    authService: Auth = new Auth();
    chatService: Chat = new Chat();
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
        await new Promise(r => setTimeout(r, 500));
    }
}

export const createGlobalStore = () => new GlobalStore();