import { makeAutoObservable, runInAction } from "mobx";
import { Ajax, TokenStore } from '../';

export enum ChatType {
    Regular,
    Group
}

export interface ChatStruct {
    ChatId: string,
    Name: string,
    Type: ChatType,
    Participants: UserChatStruct[],
    LastMessage: MessageStruct
}

export interface UserChatStruct {
    Username: string,
    FirstName: string,
    LastName: string
}

export interface MessageStruct {
    Sender: UserChatStruct,
    Message: string,
    Time: Date
}

export class Chat {
    chatHistory: ChatStruct[] = {} as ChatStruct[];

    constructor(){
        makeAutoObservable(this);
    }

    async loadChats() {
        var res = await (await Ajax.Post("https://localhost:44310/api/chat", { Token: TokenStore.token })).json() as ChatStruct[];
        runInAction(() => this.chatHistory = res);
    }
}