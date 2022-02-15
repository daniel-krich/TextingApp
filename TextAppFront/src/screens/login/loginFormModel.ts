import { makeAutoObservable } from "mobx";

class LoginModel {
    Username: string = "";
    Password: string = "";
    ErrorText: string = "";
    constructor(){
        makeAutoObservable(this);
    }
}

export const loginModel = new LoginModel();