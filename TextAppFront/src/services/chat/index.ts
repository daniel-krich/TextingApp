import { FetchPolicy } from "@apollo/client";
import { makeAutoObservable, runInAction } from "mobx";
import { Ajax, TokenStore, EventStream, Consts, NotifyService, Apollo } from '../';
import { GET_CHATS, GET_CHAT_BY_ID, GET_CHAT_MESSAGES_BY_OFFSET, SEND_MESSAGE, SUBSCRIBE_LISTEN_MESSAGES } from "../apolloClient";

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
    currentChat: ChatHistoryStruct | undefined;
    chatPartner: UserChatStruct | undefined;
    apollo: Apollo;
    constructor(notify: NotifyService,
                apollo: Apollo){
        this.apollo = apollo;
        //notify.addNotification("init", "chat initiated", "cool stuff, we passed the notification service to other service");
        makeAutoObservable(this);
    }

    async loadChats() {
        const res = (await this.apollo.instance.query({query: GET_CHATS, variables: {offset: this.chatHistory.items.length ?? 0}})).data["userChats"] as {items: ChatHistoryStruct[]};
        //const res = await (await Ajax.Get(Consts.URL + '/api/chat', true).response).json() as ChatHistoryStruct[];
        runInAction(() => this.chatHistory = res);
        //
        //var handleMessages = new EventSource('https://localhost:44310/api/Message/pull/' + TokenStore.token);
        //handleMessages.onmessage = (e) => this.handleMessages(e);

        /*var source = new EventStream(Consts.URL + '/api/Message/pull', {
            payload: '',
            headers: {
                'authorization': 'Bearer ' + TokenStore.token
            },
            method: 'GET',
            withCredentials: ''
        });
        source.addEventListener('message', (e: any) => this.handleMessages(e));
        source.stream();*/

        this.apollo.instance.subscribe({query: SUBSCRIBE_LISTEN_MESSAGES}).subscribe((o) => this.handleIncommingMessages(o as ResponseListenMessages));
    }

    async loadChat(chatId: string | undefined): Promise<ChatHistoryStruct> {
        const res = await (await Ajax.Post(Consts.URL + '/api/chat/contact',
        {
            ChatId: chatId
        }, true).response).json() as ChatHistoryStruct;
        if(this.chatHistory.items.findIndex(o => o.chatId == res.chatId) == -1 && res.chatId != undefined)
        {
            
            console.log("added chat to history");
            return this.chatHistory.items[this.chatHistory.items.push(res)-1];
        }
        throw 'Invalid chat';
    }

    async loadChatChunk(currentChat: ChatHistoryStruct | undefined): Promise<Boolean> {
        /*const res = await (await Ajax.Post(Consts.URL + '/api/Chat/messages', {
            chatId: currentChat?.chatId,
            typeChat: currentChat?.type,
            messageOffset: currentChat?.messages?.items?.length || 0
        }, true).response).json() as ChatStruct;*/
        const res = (await this.apollo.instance.query({query: GET_CHAT_MESSAGES_BY_OFFSET, variables: {chatId: currentChat?.chatId, chatType: currentChat?.type, messageOffset: currentChat?.messages?.items?.length ?? 0}})).data["userChatByChatId"] as {items: ChatHistoryStruct[]};
        //console.log(res.items[0].messages.items[0].message);
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
        console.log("chat type " + typeChat);
        /*await (await Ajax.Post(Consts.URL + '/api/Message/push', {
            chatId: chatId,
            typeChat: typeChat,
            message: message
        }, true).response).json() as ChatHistoryStruct[];*/
        const res = (await this.apollo.instance.mutate({mutation: SEND_MESSAGE, variables: {chatId: chatId, chatType: typeChat, message: message}})).data["addMessageToChat"] as {lastMessage: {message: string}};
        console.log(res.lastMessage.message);
    }

    /*handleMessages(e: MessageEvent) {
        const ChatData = JSON.parse(e.data) as ChatHistoryStruct;
        const ChatIndex = this.chatHistory.items.findIndex(o => o.chatId == ChatData.chatId);
        if(ChatIndex >= 0)
        {
            if(this.chatHistory.items[ChatIndex].messages == undefined)
                runInAction(() => this.chatHistory.items[ChatIndex].messages = {} as {items: MessageStruct[], pageInfo: {hasNextPage: Boolean}});

            runInAction(() => this.chatHistory.items[ChatIndex].messages.items.push(ChatData.lastMessage));
            this.chatHistory.items[ChatIndex].lastMessage = ChatData.lastMessage;
            console.log(this.chatHistory.items[ChatIndex]);
        }
        else
        {
            runInAction(() => this.chatHistory.items.push(ChatData));
        }
        runInAction(() => this.chatHistory.items = this.chatHistory.items.sort((a,b) => new Date(b.lastMessage.time).getTime() - new Date(a.lastMessage.time).getTime()));
    }*/

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
    }
}