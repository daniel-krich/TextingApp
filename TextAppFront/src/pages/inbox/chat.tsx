import React, { BaseSyntheticEvent, FormEvent, UIEventHandler, useEffect, useState } from 'react';
import { Container, Row, Col, ListGroup, Form, FormControl, Button, Alert, Spinner } from 'react-bootstrap';
import { observer } from 'mobx-react';
import { runInAction } from 'mobx';
import { useParams, useNavigate } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.min.css';
import './chat.css';
import { useGlobalStore } from '../../services';
import { ChatStruct, ChatType, MessageStruct, ResponseListenMessages, UserChatStruct } from '../../services/chat';
import { ChatModel } from './chatModel';
import { ApolloError } from '@apollo/client';

export const Chat = observer(() => {
    const { chatId } = useParams();
    const navigate = useNavigate();
    const [loadingChats, setLoadingChats] = useState(true);
    const [dontRenderChunks, setDontRenderChunks] = useState(false);
    const chatService = useGlobalStore().chatService;
    const user = useGlobalStore().authService.account;
    //
    const onScrollEvent = async function(evt: BaseSyntheticEvent) {
        if(evt.target.scrollHeight + evt.target.scrollTop == evt.target.clientHeight && !dontRenderChunks) {
            
            const chatChunk = (await chatService.loadChatChunk(chatId ?? "", ChatModel.currentChat?.messages?.items?.length ?? 0, ChatModel.currentChatType))[0] as ChatStruct;
            runInAction(() => {
                if(ChatModel.currentChat) {
                    if(ChatModel.currentChat.messages.items == undefined) {
                        ChatModel.currentChat.messages.items = [] as MessageStruct[];
                    }
                    ChatModel.currentChat.messages.items = ChatModel.currentChat.messages.items.concat(chatChunk.messages.items);
    
                    console.log(ChatModel.currentChat.messages.items);
                }
                //chat.messages.items = chat.messages?.items.sort((a,b) => new Date(a.time).getTime() - new Date(b.time).getTime())
            });
            if(!chatChunk.messages.pageInfo.hasNextPage)
                setDontRenderChunks(true);
        }
    };

    const onSubmitMessage = async function(evt: FormEvent) {
        evt.preventDefault();
        const chatText = ChatModel.chatText;
        runInAction(() => ChatModel.chatText = '');
        if(chatText.length > 0) {
            try {
                await chatService.sendMessage(chatId ?? "", ChatModel.currentChatType ?? ChatType.Regular, chatText);
            }
            catch(e: any){
                if(e instanceof ApolloError)
                    console.log(e.message);
            }
        }
    };

    useEffect(() => {
        let isActive = true;
        (async () => {
            var chatChunk = (await chatService.loadChatChunk(chatId ?? "", 0))[0] as ChatStruct;
            if(chatChunk) {
                if(isActive) {
                    setDontRenderChunks(!chatChunk.messages.pageInfo.hasNextPage);
                    runInAction(() => {
                        ChatModel.currentChat = chatChunk;
                        ChatModel.chatPartner = chatChunk.participants.items.find(p => p.username == chatId) ?? {} as UserChatStruct;
                        ChatModel.currentChatType = chatChunk.type;
                    });
                }
            }
            else {
                try {
                    const search = await chatService.loadUserChatInfo(chatId ?? "");
                    if(isActive) {
                        runInAction(() => {
                            ChatModel.chatPartner = search.searchUser.items[0];
                            ChatModel.currentChatType = ChatType.Regular;
                        });
                    }
                }
                catch { }
            }
            //await new Promise(r => setTimeout(r, 500));
            if(isActive) setLoadingChats(false);
        })();
        return () => {
            isActive = false;
        };
    }, []);

    useEffect(() => {
        let isActive = true;
        function ProcessChatUpdates(e: ResponseListenMessages) {
            if(e.data.listenChatUpdates.chatId == chatId) {
                runInAction(() => {
                    if(ChatModel.currentChat) {
                        if(ChatModel.currentChat.messages.items == undefined) {
                            ChatModel.currentChat.messages.items = [] as MessageStruct[];
                        }
                        ChatModel.currentChat.messages.items.unshift(e.data.listenChatUpdates.lastMessage);
                    }
                });
                
                if(!ChatModel.currentChat) {
                    chatService.loadChatChunk(chatId ?? "", 0, e.data.listenChatUpdates.type).then((resChunk: ChatStruct[]) => {
                        runInAction(() => ChatModel.currentChat = resChunk[0]);
                    });
                }
            }
        }
        let unsubscribe = chatService.handleMessagesEvent?.subscribe((e: any) => {
            if(isActive) {
                ProcessChatUpdates(e);
            }
            else {
                unsubscribe?.unsubscribe();
            }
        });
        return () => {
            isActive = false;
        };
    }, []);

    return (
        <>
            
            <Container>
                <Row className='p-3 bg-white'>
                    <Col md={2}><h6 role="button" onClick={() => navigate('/inbox')} className='text-center display-4 text-black'>Back</h6></Col>
                    <Col md={8}><h6 className='text-center display-4 text-black'>
                    {
                        loadingChats ?
                        (<Spinner animation="grow" variant="dark">
                                <span className="visually-hidden">Loading...</span>
                        </Spinner>)
                        :
                        (ChatModel.currentChatType == ChatType.Regular ? ChatModel.chatPartner?.firstName + ' ' + ChatModel.chatPartner?.lastName : ChatModel.currentChat?.name)
                        ??
                        (<p className='text-muted'>Unknown</p>)
                    }
                    </h6></Col>
                </Row>

                <Row className='chat-box' onScroll={onScrollEvent}>
                {
                loadingChats ?
                    (<ListGroup as="ol" className='p-0 h-50 align-items-center'>
                        <Spinner animation="grow" variant="dark">
                            <span className="visually-hidden">Loading...</span>
                        </Spinner>
                    </ListGroup>)
                    :
                    (<ListGroup as="ol" className='p-0 flex-column-reverse'>
                        {ChatModel.currentChat?.messages?.items?.map((o, index) => 
                            <ListGroup.Item key={index} as="div" className='border-0'>
                            <div className={`ms-2 me-auto ${o.sender.username == user.username ? 'text-start' : 'text-end'}`}>
                            <div className="fw-bold">{o.sender.firstName} {o.sender.lastName} ({new Date(o.time).toLocaleDateString()})</div>
                            {o.message}
                            </div>
                            </ListGroup.Item>)
                        ??
                            <Alert className='mb-0 rounded-0' variant="secondary">
                            <Alert.Heading>No messages found</Alert.Heading>
                            <p>
                                You can start a conversation by sending a message.
                            </p>
                            </Alert>
                        }
                    </ListGroup>)
                }
                </Row>

                <Row className='chat-box-input d-flex mt-3'>
                <Form onSubmit={onSubmitMessage}>
                    <Form.Group className="mb-3 mt-3 d-flex flex-row">
                        <Form.Control
                        type='text'
                        value={ChatModel.chatText}
                        onChange={(evt) => runInAction(() => ChatModel.chatText = evt.target.value)}
                        placeholder="Enter your message..."
                        aria-describedby="send-message-btn"
                        />
                        <Button type="submit" variant="outline-success" id="send-message-btn">
                        Send
                        </Button>
                    </Form.Group>
                </Form>
                </Row>
            </Container>
        </>
    );
});