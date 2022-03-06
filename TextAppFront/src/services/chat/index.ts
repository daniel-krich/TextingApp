import { FetchPolicy } from "@apollo/client";
import { makeAutoObservable, runInAction } from "mobx";
import { Ajax, TokenStore, EventStream, Consts, NotifyService, Apollo } from '../';
import { GET_CHATS, GET_CHAT_BY_ID, GET_CHAT_MESSAGES_BY_OFFSET, SEARCH_USER_BY_NAME, SEND_MESSAGE, SUBSCRIBE_LISTEN_MESSAGES } from "../apolloClient";

export enum ChatType {
    Regular = "REGULAR",
    Group = "GROUP"
}

export interface ChatStruct {
    chatId: string,
    type: ChatType,
    messages: {
        items: MessageStruct[]
    }
}

export interface SearchUserInfo {
    searchUser: {
        items: UserChatStruct[]
    }
}

export interface ChatHistoryStruct {
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
    }
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
    chatHistory: {
        items: ChatHistoryStruct[]
    } = {items: {} as ChatHistoryStruct[]};
    handleMessagesEvent: {method:{ (message: ResponseListenMessages): void;}}[] = [] as {method:{ (message: ResponseListenMessages): void; }}[];
    apollo: Apollo;
    constructor(notify: NotifyService, apollo: Apollo) {
        this.apollo = apollo;
        makeAutoObservable(this);
    }

    async loadChats() {
        const res = (await this.apollo.instance.query({query: GET_CHATS, variables: {offset: this.chatHistory.items.length ?? 0}})).data["userChats"] as {items: ChatHistoryStruct[]};
        runInAction(() => this.chatHistory = res);
        this.apollo.instance.subscribe({query: SUBSCRIBE_LISTEN_MESSAGES}).subscribe((o) => this.handleIncommingMessages(o as ResponseListenMessages));
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

    async loadChatChunk(currentChat: ChatHistoryStruct | undefined): Promise<Boolean> {
        const res = (await this.apollo.instance.query({query: GET_CHAT_MESSAGES_BY_OFFSET, variables: {chatId: currentChat?.chatId, chatType: currentChat?.type, messageOffset: currentChat?.messages?.items?.length ?? 0}})).data["userChatByChatId"] as {items: ChatHistoryStruct[]};
        if(res?.items[0] != undefined) {
            const chat = this.chatHistory.items.find(c => c.chatId == currentChat?.chatId) as ChatHistoryStruct;
            console.log(chat);
            res.items[0].messages.items.forEach((o, index) => {
                console.log(o.message + " " + index);
                if(chat.messages == undefined)
                {
                    runInAction(() => {
                        chat.messages = {} as {items: MessageStruct[], pageInfo: {hasNextPage: Boolean}};
                        chat.messages.items = [] as MessageStruct[];
                    });
                }

                runInAction(() => chat.messages.items.push(o));
            });
            
            runInAction(() => chat.messages.items = chat.messages?.items.sort((a,b) => new Date(a.time).getTime() - new Date(b.time).getTime()));
            return res.items[0].messages.pageInfo.hasNextPage;
        }
        return false;

    }

    async sendMessage(chatId: string, typeChat: ChatType, message: string) {
        const res = (await this.apollo.instance.mutate({mutation: SEND_MESSAGE, variables: {chatId: chatId, chatType: typeChat, message: message}})).data["addMessageToChat"] as {lastMessage: {message: string}};
        console.log(res.lastMessage.message);
    }

    async handleIncommingMessages(e: ResponseListenMessages) {
        const ChatIndex = this.chatHistory.items.findIndex(o => o.chatId == e.data.listenChatUpdates.chatId && o.type == e.data.listenChatUpdates.type);
        if(ChatIndex >= 0)
        {
            if(this.chatHistory.items[ChatIndex].messages == undefined)
                runInAction(() => {
                    this.chatHistory.items[ChatIndex].messages = {} as {items: MessageStruct[], pageInfo: {hasNextPage: Boolean}};
                    this.chatHistory.items[ChatIndex].messages.items = [] as MessageStruct[];
                });

            runInAction(() => this.chatHistory.items[ChatIndex].messages.items.push(e.data.listenChatUpdates.lastMessage));
            runInAction(() => this.chatHistory.items[ChatIndex].lastMessage = e.data.listenChatUpdates.lastMessage);
            console.log(this.chatHistory.items[ChatIndex]);
        }
        else
        {
            const res = (await this.apollo.instance.query({query: GET_CHAT_BY_ID, variables: {chatId: e.data.listenChatUpdates.chatId, chatType: e.data.listenChatUpdates.type}})).data["userChatByChatId"] as {items: ChatHistoryStruct[]};
            runInAction(() => this.chatHistory.items.push(res.items[0]));
        }
        runInAction(() => this.chatHistory.items = this.chatHistory.items.sort((a,b) => new Date(b.lastMessage.time).getTime() - new Date(a.lastMessage.time).getTime()));

        this.handleMessagesEvent.forEach(o => 
            o?.method.call(this, e)
        );
    }
}