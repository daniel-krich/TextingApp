import { ApolloClient, ApolloError, NormalizedCacheObject } from '@apollo/client';
import { makeAutoObservable, runInAction } from 'mobx';
import React from 'react';
import { Ajax, TokenStore, Consts, NotifyService, GET_USER_INFO, Apollo } from '../'
import { REGISTER, USER_LOGIN } from '../apolloClient';

interface ResponseModel {
    ErrorId: number | undefined,
    Header: string,
    Comment: string
}

interface LoginResponse {
    token: string
}

export interface LoginTokenResponse {
    username: string,
    email: string,
    firstName: string,
    lastName: string
}

export class Auth {
    account: LoginTokenResponse;
    apollo: Apollo;
    constructor(notify: NotifyService,
                apollo: Apollo){
        this.apollo = apollo;
        this.account = {} as LoginTokenResponse;
        makeAutoObservable(this);
    }

    async accountLogin(username: string, password: string): Promise<LoginResponse> {
        /*const json = await (await Ajax.Post(Consts.URL + '/api/user/login', {
            Username: username,
            Password: password
        }, false).response).json() as LoginResponse;*/
        try {
            const res = (await this.apollo.instance.query({query: USER_LOGIN, variables: {username: username, password: password}})).data as LoginResponse;
            TokenStore.token = res.token;
            return res;
        }
        catch(err: any) {
            if(err instanceof ApolloError) {
                console.log(err.message);
                this.apollo.setJWT("");
                TokenStore.clearTokens();
            }
            throw err;
        }
    }

    async accountRegister(username: string, password: string, email: string, firstname: string, lastname: string): Promise<[Boolean, string]> {
        try {
            const res = (await this.apollo.instance.mutate({mutation: REGISTER, variables: {
                username: username,
                password: password,
                email: email,
                firstName: firstname,
                lastName: lastname
            }})).data["user"] as LoginTokenResponse;
            return [true, "Successfully created new user ("+ res.username +")"]
        }
        catch(err: any) {
            if(err instanceof ApolloError) {
                console.log(err.message);
                this.apollo.setJWT("");
                TokenStore.clearTokens();
                return [false, err.message];
            }
            return [false, "Unknown error"];
        }
        /*const json = await (await Ajax.Post(Consts.URL + '/api/user/register', {
            username: username,
            password: password,
            email: email,
            firstName: firstname,
            lastName: lastname
        }, false).response).json() as ResponseModel;
        return json;*/
    }

    async accountLoginToken() {
        if(TokenStore.token != null && TokenStore.token != undefined && this.isLogged == false)
        {
            try {
                this.apollo.setJWT(TokenStore.token);
                const res = (await this.apollo.instance.query({query: GET_USER_INFO})).data["user"] as LoginTokenResponse;
                console.log(res);
                //var res = await (await Ajax.Get(Consts.URL + '/api/user/refresh', true).response).json() as LoginTokenResponse;
                runInAction(() => this.account = res);
                if(!this.isLogged)
                {
                    this.apollo.setJWT("");
                    TokenStore.clearTokens();
                }
            }
            catch(err: any) {
                if(err instanceof ApolloError) {
                    console.log("invalid auth token");
                    this.apollo.setJWT("");
                    TokenStore.clearTokens();
                }
            }
        }
    }

    get isLogged(): boolean{
        if(this.account != null && this.account.username?.length > 0) return true;
        return false;
    }
}