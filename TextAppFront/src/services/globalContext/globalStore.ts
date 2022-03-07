import { Auth } from "../auth";
import { Apollo } from '../apolloClient';
import { makeAutoObservable, observable, computed, action, flow, runInAction } from "mobx";
import { Chat } from '../chat';
import { Notifications } from '../notifications';
import { ApolloClient, NormalizedCacheObject } from "@apollo/client";

export class GlobalStore {
    apolloService: Apollo = new Apollo();
    notifyService: Notifications = new Notifications();
    authService: Auth = new Auth(this.notifyService, this.apolloService);
    chatService: Chat = new Chat(this.notifyService, this.apolloService);
    isAppLoaded: boolean = false;
    constructor(){
        makeAutoObservable(this);
    }

    async loadApp() {
        console.log("Application being loaded.");
        await this.authService.accountLoginToken();
        if(this.authService.isLogged)
        {
            // init stuff if user logged-in.

            this.chatService.registerListenMessages();
        }
    }
}

export const createGlobalStore = () => new GlobalStore();