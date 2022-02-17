import { makeAutoObservable, runInAction } from 'mobx';
import React from 'react';
import { Ajax, TokenStore } from '../'

interface LoginResponse {
    Token: string
}

interface LoginTokenResponse {
    Username: string,
    Email: string,
    FirstName: string,
    LastName: string
}

export class Auth {
    account: LoginTokenResponse;
    constructor(){
        this.account = {} as LoginTokenResponse;
        makeAutoObservable(this);
    }

    async accountLogin(username: string, password: string): Promise<boolean> {
        const json = await (await Ajax.Post("https://localhost:44310/api/user/signin", {
            Username: username,
            Password: password
        })).json() as LoginResponse;
        if(json.Token != null)
        {
            TokenStore.token = json.Token;
            return true;
        }
        return false;
    }

    async accountLoginToken() {
        if(TokenStore.token != null && TokenStore.token != undefined && this.isLogged == false)
        {
            var res = await (await Ajax.Post("https://localhost:44310/api/user/auth_token", { Token: TokenStore.token })).json() as LoginTokenResponse;
            runInAction(() => this.account = res);
            if(!this.isLogged)
            {
                TokenStore.clearTokens();
            }
        }
    }

    get isLogged(): boolean{
        if(this.account != null && this.account.Username?.length > 0) return true;
        return false;
    }
}