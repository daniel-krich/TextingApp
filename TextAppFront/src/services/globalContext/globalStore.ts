import Auth from "../auth";
import { makeAutoObservable, observable, computed, action, flow } from "mobx"

export class GlobalStore {
    authService: Auth;
    clickme: number = 0;
    constructor(){
        //Instantiate new services here
        this.authService = new Auth();
        //
        makeAutoObservable(this);
    }
}

export const createGlobalStore = () => new GlobalStore();