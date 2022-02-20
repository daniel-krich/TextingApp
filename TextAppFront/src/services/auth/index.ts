import { makeAutoObservable, runInAction } from 'mobx';
import React from 'react';
import { Ajax, TokenStore, Consts } from '../'

interface ResponseModel {
    ErrorId: number | undefined,
    Header: string,
    Comment: string
}

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

    async accountLogin(username: string, password: string): Promise<ResponseModel> {
        const json = await (await Ajax.Post(Consts.URL + '/api/user/signin', {
            Username: username,
            Password: password
        })).json() as LoginResponse;
        if(json.Token != null)
        {
            TokenStore.token = json.Token;
            return (json as {}) as ResponseModel;
        }
        return (json as {}) as ResponseModel;
    }

    async accountRegister(username: string, password: string, email: string, firstname: string, lastname: string): Promise<ResponseModel> {
        const json = await (await Ajax.Post(Consts.URL + '/api/user/signup', {
            username: username,
            password: password,
            email: email,
            firstName: firstname,
            lastName: lastname
        })).json() as ResponseModel;
        return json;
    }

    async accountLoginToken() {
        if(TokenStore.token != null && TokenStore.token != undefined && this.isLogged == false)
        {
            var res = await (await Ajax.Post(Consts.URL + '/api/user/auth_token', { Token: TokenStore.token })).json() as LoginTokenResponse;
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