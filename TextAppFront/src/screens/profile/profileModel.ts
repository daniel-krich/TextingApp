import { makeAutoObservable } from "mobx";
import { runInAction } from "mobx";
import { Ajax, Consts, LoginTokenResponse } from '../../services';

interface User {
    Username: string,
    FirstName: string,
    LastName: string
}

class Profile {
    currentUser: User = {} as User;
    constructor(){
        makeAutoObservable(this);
    }

    async tryGetProfileInfo(username: string) {
        const res = await (await Ajax.Post(Consts.URL + '/api/User/find', {
            Query: username
        }, true).response).json() as User;
        runInAction(() => this.currentUser = res);
    }

    getProfileInfo(user: LoginTokenResponse) {
        runInAction(() => {
            this.currentUser.Username = user.Username;
            this.currentUser.FirstName = user.FirstName;
            this.currentUser.LastName = user.LastName;
        });
    }
}

export const ProfileModel = new Profile();