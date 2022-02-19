import { makeAutoObservable } from "mobx";
import { ChatHistoryStruct, UserChatStruct } from '../../services';

class ChatInputModel {
    chatText: string = "";
    constructor(){
        makeAutoObservable(this);
    }
}

export const ChatModel = new ChatInputModel();