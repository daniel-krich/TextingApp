import { makeAutoObservable, runInAction } from 'mobx';
import React from 'react';
import { Ajax } from '../'

interface LoginForm{
    Username: string,
    Password: string
}

interface Account{
    Id: number,
    Username: string,
    Email: string,
    Description: string,
    Age: number
}

class Auth {
    account: Account;
    constructor(){
        this.account = {} as Account;
        makeAutoObservable(this);
    }

    async accountLogin(login: LoginForm) {
        const json = await (await Ajax.Post("http://127.0.0.1:3001/api/login", login)).json();
        runInAction(() => this.account = json as Account);
    }

    get isLogged(): boolean{
        if(this.account != null && this.account.Id > 0) return true;
        return false;
    }
}

export default Auth;