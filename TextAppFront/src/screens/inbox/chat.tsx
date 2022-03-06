import React, { BaseSyntheticEvent, FormEvent, UIEventHandler, useEffect, useState } from 'react';
import { Container, Row, Col, ListGroup, Form, FormControl, Button, Alert, Spinner } from 'react-bootstrap';
import { observer } from 'mobx-react';
import { runInAction } from 'mobx';
import { useParams, useNavigate } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.min.css';
import './chat.css';
import { useGlobalStore } from '../../services';
import { ChatHistoryStruct, ChatType, ResponseListenMessages } from '../../services/chat';
import { ChatModel } from './chatModel';
import { ApolloError } from '@apollo/client';

export const Chat = observer(() => {
    const { chatId } = useParams();
    const navigate = useNavigate();
    const [loadingChats, setLoadingChats] = useState(false);
    const [dontRenderChunks, setDontRenderChunks] = useState(false);
    const chatService = useGlobalStore().chatService;
    const chats = useGlobalStore().chatService.chatHistory;
    const user = useGlobalStore().authService.account;
    //
    const onScrollEvent = async function(evt: BaseSyntheticEvent) {
        if(evt.target.scrollHeight + evt.target.scrollTop == evt.target.clientHeight && !dontRenderChunks) {
            
            const isNextChunk = await chatService.loadChatChunk(ChatModel.currentChat);
            console.log("loaded next chunk");
            console.log(ChatModel.currentChat?.messages?.items?.map(o => o.message));
            if(!isNextChunk)
                setDontRenderChunks(true);
        }
    };

    const onSubmitMessage = async function(evt: FormEvent) {
        evt.preventDefault();
        const chatText = ChatModel.chatText;
        runInAction(() => ChatModel.chatText = '');
        if(chatText.length <= 0) return;
        try {
            if(ChatModel.currentChat != undefined)
                await chatService.sendMessage(ChatModel.currentChat?.chatId, ChatModel.currentChat?.type, chatText);
            else
                await chatService.sendMessage(chatId ?? "", ChatType.Regular, chatText);
        }
        catch(e: any){
            if(e instanceof ApolloError)
                console.log(e.message);
        }
    };

    useEffect(() => {
        runInAction(() => {
            if((ChatModel.currentChat = chats.items.find(o => o.chatId == chatId)) == undefined)
            {
                chatService.loadUserChatInfo(chatId ?? "").then(o => runInAction(() => {
                    ChatModel.chatPartner = o.searchUser.items[0];
                    ChatModel.currentChatType = ChatType.Regular;
                }))
                .catch((e: any) => {
                    
                });
            }
            else
            {
                ChatModel.chatPartner = ChatModel.currentChat?.participants.items.find(o => o.username == ChatModel.currentChat?.chatId);
                ChatModel.currentChatType = ChatModel.currentChat.type;
            }
        });
    }, []);

    useEffect(() => {
        if(ChatModel.currentChat != undefined)
        {
            setLoadingChats(true);
            chatService.loadChatChunk(ChatModel.currentChat).then(isMoreChunks =>
            {
                setLoadingChats(false);
                setDontRenderChunks(!isMoreChunks);
            });
            console.log("loaded chunk");
        }
    }, []);

    useEffect(() => {
        const update_current_chat = {
            method: (e: ResponseListenMessages) => {
                if(e.data.listenChatUpdates.chatId == chatId) {
                    ChatModel.currentChat = chats.items.find(o => o.chatId == e.data.listenChatUpdates.chatId);
                    ChatModel.currentChatType = e.data.listenChatUpdates.type;
                }
            }
        };
        let funcIndex = chatService.handleMessagesEvent.findIndex(o => o == chatService.handleMessagesEvent.at(-1))+1;
        chatService.handleMessagesEvent[funcIndex] = update_current_chat;
        console.log("length " + chatService.handleMessagesEvent.length);
        return () => {
            
            delete chatService.handleMessagesEvent[funcIndex];
            console.log("length after destruct " + chatService.handleMessagesEvent.length);
            console.log(chatService.handleMessagesEvent);
        };
    }, []);
    return (
        <>
            
            <Container>
                <Row className='p-3 bg-white'>
                    <Col md={2}><h6 role="button" onClick={() => navigate('/inbox')} className='text-center display-4 text-black'>Back</h6></Col>
                    <Col md={8}><h6 className='text-center display-4 text-black'>
                    {
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
                    (<ListGroup as="ol" className='p-0'>
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