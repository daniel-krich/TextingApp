import { makeAutoObservable } from "mobx";
import { ChatStruct, ChatType, UserChatStruct } from '../../services';

class ChatHisModel {
    chatsFeed: ChatStruct[] | undefined;
    constructor(){
        makeAutoObservable(this);
    }
}

export const chatHistoryModel = new ChatHisModel();