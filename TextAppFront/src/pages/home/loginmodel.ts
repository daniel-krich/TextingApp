import { makeAutoObservable } from "mobx";

class LoginModel{
    Username: string = "";
    Password: string = "";
    constructor(){
        makeAutoObservable(this);
    }
}

export const loginModel = new LoginModel();