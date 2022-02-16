import Auth from "../auth";
import { makeAutoObservable, observable, computed, action, flow, runInAction } from "mobx";

export class GlobalStore {
    authService: Auth = new Auth();
    isAppLoaded: boolean = false;
    constructor(){
        makeAutoObservable(this);

        (async () => {
            console.log("loading func");
            await this.authService.accountLoginToken();
            await new Promise(r => setTimeout(r, 500));

        })().then(_ => runInAction(() => this.isAppLoaded = true));
    }
}

export const createGlobalStore = () => new GlobalStore();