import { FetchPolicy, FetchResult, Observable, Observer } from "@apollo/client";
import { makeAutoObservable, runInAction } from "mobx";
import { Ajax, TokenStore, EventStream, Consts, NotifyService, Apollo } from '../';
import { GET_CHATS, GET_FULLCHAT_BY_ID_WITH_MESSAGES_CHUNK, GET_CHAT_MESSAGES_CHUNK_BY_OFFSET, SEARCH_USER_BY_NAME, SEND_MESSAGE, SUBSCRIBE_LISTEN_MESSAGES, GET_CHAT_BY_ID } from "../apolloClient";

export enum ChatType {
    Regular = "REGULAR",
    Group = "GROUP"
}

export interface SearchUserInfo {
    searchUser: {
        items: UserChatStruct[]
    }
}

export interface ChatsStruct {
    items: ChatStruct[],
    pageInfo: {hasNextPage: Boolean}
}

export interface ChatStruct {
    chatId: string,
    name: string,
    type: ChatType,
    participants: {
        items: UserChatStruct[]
    },
    lastMessage: MessageStruct,
    messages: {
        items: MessageStruct[],
        pageInfo: {hasNextPage: Boolean}
    },
}

export interface ResponseListenMessages {
    data: {
        listenChatUpdates: {
            chatId: string,
            type: ChatType,
            lastMessage: {
                sender: {
                    username: string,
                    firstName: string,
                    lastName: string
                },
                message: string,
                time: Date
            }
        }
    }
}

export interface UserChatStruct {
    username: string,
    firstName: string,
    lastName: string
}

export interface MessageStruct {
    sender: UserChatStruct,
    message: string,
    time: Date
}

export class Chat {
    handleMessagesEvent: Observable<FetchResult<any, Record<string, any>, Record<string, any>>> | undefined;
    apollo: Apollo;

    constructor(notify: NotifyService, apollo: Apollo) {
        this.apollo = apollo;
        makeAutoObservable(this);
    }

    registerListenMessages() {
        this.handleMessagesEvent = this.apollo.instance.subscribe({query: SUBSCRIBE_LISTEN_MESSAGES});
    }

    async loadChats(offset: number = 0): Promise<ChatsStruct> {
        return (await this.apollo.instance.query({query: GET_CHATS, variables: {offset: offset}})).data["userChats"];
    }

    async loadUserChatInfo(username: string): Promise<SearchUserInfo> {
        const res = (await this.apollo.instance.query({query: SEARCH_USER_BY_NAME, variables: {username: username, exact: true}})).data as SearchUserInfo;
        if(res.searchUser.items.length == 0) {
            throw 'Invalid username';
        }
        else {
            return res;
        }
    }

    async loadChat(chatId: string, chatType: ChatType | undefined = undefined): Promise<ChatStruct> {
        return (await this.apollo.instance.query({query: GET_CHAT_BY_ID, variables: {chatId: chatId, chatType: chatType}})).data["userChatByChatId"]["items"][0] as ChatStruct;
    }

    async loadChatChunk(chatId: string, messageOffset: number, chatType: ChatType | undefined = undefined): Promise<ChatStruct[]> {
        if(messageOffset > 0) {
            return (await this.apollo.instance.query({query: GET_CHAT_MESSAGES_CHUNK_BY_OFFSET, variables: {chatId: chatId, chatType: chatType, messageOffset: messageOffset}})).data["userChatByChatId"]["items"] as ChatStruct[];
        }
        else {
            return (await this.apollo.instance.query({query: GET_FULLCHAT_BY_ID_WITH_MESSAGES_CHUNK, variables: {chatId: chatId, chatType: chatType, messageOffset: messageOffset}})).data["userChatByChatId"]["items"] as ChatStruct[]; 
        }
    }

    async sendMessage(chatId: string, typeChat: ChatType, message: string) {
        const res = (await this.apollo.instance.mutate({mutation: SEND_MESSAGE, variables: {chatId: chatId, chatType: typeChat, message: message}})).data["addMessageToChat"] as {lastMessage: {message: string}};
        console.log(res.lastMessage.message);
    }
}