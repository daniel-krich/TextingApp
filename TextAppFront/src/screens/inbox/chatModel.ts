import { makeAutoObservable } from "mobx";
import { ChatHistoryStruct, ChatType, UserChatStruct } from '../../services';

class ChatInputModel {
    chatPartner: UserChatStruct | undefined;
    currentChat: ChatHistoryStruct | undefined;
    currentChatType: ChatType | undefined;
    chatText: string = "";
    constructor(){
        makeAutoObservable(this);
    }
}

export const ChatModel = new ChatInputModel();