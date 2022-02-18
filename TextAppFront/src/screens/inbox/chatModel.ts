import { makeAutoObservable } from "mobx";

class ChatInputModel {
    chatText: string = "";
    constructor(){
        makeAutoObservable(this);
    }
}

export const ChatModel = new ChatInputModel();