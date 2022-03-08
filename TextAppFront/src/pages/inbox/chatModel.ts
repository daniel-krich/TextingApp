import { makeAutoObservable } from "mobx";
import { ChatStruct, ChatType, UserChatStruct } from '../../services';

class ChatInputModel {
    chatPartner: UserChatStruct = {} as UserChatStruct;
    currentChat: ChatStruct | undefined;
    currentChatType: ChatType = {} as ChatType;
    chatText: string = "";
    constructor(){
        makeAutoObservable(this);
    }
}

export const ChatModel = new ChatInputModel();