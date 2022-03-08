import { makeAutoObservable } from "mobx";

class RegisterModel {
    Username: string = "";
    Password: string = "";
    Firstname: string = "";
    Lastname: string = "";
    Email: string = "";
    ErrorText: string = "";
    constructor(){
        makeAutoObservable(this);
    }
}

export const registerModel = new RegisterModel();