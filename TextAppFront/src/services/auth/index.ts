import { makeAutoObservable, runInAction } from 'mobx';
import React from 'react';
import { Ajax, TokenStore, Consts, NotifyService } from '../'

interface ResponseModel {
    ErrorId: number | undefined,
    Header: string,
    Comment: string
}

interface LoginResponse {
    AccessToken: string
}

export interface LoginTokenResponse {
    Username: string,
    Email: string,
    FirstName: string,
    LastName: string
}

export class Auth {
    account: LoginTokenResponse;
    constructor(notify: NotifyService){
        this.account = {} as LoginTokenResponse;
        makeAutoObservable(this);
    }

    async accountLogin(username: string, password: string): Promise<ResponseModel> {
        const json = await (await Ajax.Post(Consts.URL + '/api/user/login', {
            Username: username,
            Password: password
        }, false).response).json() as LoginResponse;
        if(json.AccessToken != null)
        {
            TokenStore.token = json.AccessToken;
            return (json as {}) as ResponseModel;
        }
        return (json as {}) as ResponseModel;
    }

    async accountRegister(username: string, password: string, email: string, firstname: string, lastname: string): Promise<ResponseModel> {
        const json = await (await Ajax.Post(Consts.URL + '/api/user/register', {
            username: username,
            password: password,
            email: email,
            firstName: firstname,
            lastName: lastname
        }, false).response).json() as ResponseModel;
        return json;
    }

    async accountLoginToken() {
        if(TokenStore.token != null && TokenStore.token != undefined && this.isLogged == false)
        {
            var res = await (await Ajax.Get(Consts.URL + '/api/user/refresh', true).response).json() as LoginTokenResponse;
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