import { makeAutoObservable } from "mobx";

export interface User {
    username: string,
    firstName: string,
    lastName: string
}

class Profile {
    currentUser: User = {} as User;
    constructor(){
        makeAutoObservable(this);
    }
}

export const ProfileModel = new Profile();