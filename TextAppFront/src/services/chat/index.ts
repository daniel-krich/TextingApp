import { makeAutoObservable, runInAction } from "mobx";
import { Ajax, TokenStore, EventStream, Consts, NotifyService } from '../';

export enum ChatType {
    Regular,
    Group
}

export interface ChatStruct {
    ChatId: string,
    Type: ChatType,
    Messages: MessageStruct[]
}

export interface ChatHistoryStruct {
    ChatId: string,
    Name: string,
    Type: ChatType,
    Participants: UserChatStruct[],
    LastMessage: MessageStruct,
    Messages: MessageStruct[]
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
    chatHistory: ChatHistoryStruct[] = {} as ChatHistoryStruct[];
    currentChat: ChatHistoryStruct | undefined;
    chatPartner: UserChatStruct | undefined;
    constructor(notify: NotifyService){
        //notify.addNotification("init", "chat initiated", "cool stuff, we passed the notification service to other service");
        makeAutoObservable(this);
    }

    async loadChats() {
        const res = await (await Ajax.Get(Consts.URL + '/api/chat', true).response).json() as ChatHistoryStruct[];
        runInAction(() => this.chatHistory = res);
        //
        //var handleMessages = new EventSource('https://localhost:44310/api/Message/pull/' + TokenStore.token);
        //handleMessages.onmessage = (e) => this.handleMessages(e);

        var source = new EventStream(Consts.URL + '/api/Message/pull', {
            payload: '',
            headers: {
                'authorization': 'Bearer ' + TokenStore.token
            },
            method: 'GET',
            withCredentials: ''
        });
        source.addEventListener('message', (e: any) => this.handleMessages(e));
        source.stream();
    }

    async loadChat(chatId: string | undefined): Promise<ChatHistoryStruct> {
        const res = await (await Ajax.Post(Consts.URL + '/api/chat/contact',
        {
            ChatId: chatId
        }, true).response).json() as ChatHistoryStruct;
        if(this.chatHistory.findIndex(o => o.ChatId == res.ChatId) == -1 && res.ChatId != undefined)
        {
            
            console.log("added chat to history");
            return this.chatHistory[this.chatHistory.push(res)-1];
        }
        throw 'Invalid chat';
    }

    async loadChatChunk(currentChat: ChatHistoryStruct | undefined): Promise<number> {
        const res = await (await Ajax.Post(Consts.URL + '/api/Chat/messages', {
            chatId: currentChat?.ChatId,
            typeChat: currentChat?.Type,
            messageOffset: currentChat?.Messages?.length || 0
        }, true).response).json() as ChatStruct;

        if(res.Messages ?? undefined) {

            const chat = this.chatHistory.find(c => c.ChatId == res.ChatId) as ChatHistoryStruct;

            res.Messages.forEach((o, index) => {
                console.log(o.Message + " " + index);
                if(chat.Messages == undefined)
                    runInAction(() => chat.Messages = [] as MessageStruct[]);

                runInAction(() => chat.Messages.push(o));
            });
            
            runInAction(() => chat.Messages = chat.Messages?.sort((a,b) => new Date(a.Time).getTime() - new Date(b.Time).getTime()));
            return res.Messages.length;
        }
        return 0;

    }

    async sendMessage(chatId: string | undefined, typeChat: ChatType | undefined, message: string) {
        await (await Ajax.Post(Consts.URL + '/api/Message/push', {
            chatId: chatId,
            typeChat: typeChat,
            message: message
        }, true).response).json() as ChatHistoryStruct[];
    }

    handleMessages(e: MessageEvent) {
        const ChatData = JSON.parse(e.data) as ChatHistoryStruct;
        const ChatIndex = this.chatHistory.findIndex(o => o.ChatId == ChatData.ChatId);
        if(ChatIndex >= 0)
        {
            if(this.chatHistory[ChatIndex].Messages == undefined)
                runInAction(() => this.chatHistory[ChatIndex].Messages = [] as MessageStruct[]);

            runInAction(() => this.chatHistory[ChatIndex].Messages.push(ChatData.LastMessage));
            this.chatHistory[ChatIndex].LastMessage = ChatData.LastMessage;
            console.log(this.chatHistory[ChatIndex]);
        }
        else
        {
            runInAction(() => this.chatHistory.push(ChatData));
        }
        runInAction(() => this.chatHistory = this.chatHistory.sort((a,b) => new Date(b.LastMessage.Time).getTime() - new Date(a.LastMessage.Time).getTime()));
    }
}