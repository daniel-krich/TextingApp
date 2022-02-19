import React, { BaseSyntheticEvent, FormEvent, UIEventHandler, useEffect, useState } from 'react';
import { Container, Row, Col, ListGroup, Form, FormControl, Button } from 'react-bootstrap';
import { observer } from 'mobx-react';
import { runInAction } from 'mobx';
import { useSearchParams } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.min.css';
import './chat.css';
import { useGlobalStore } from '../../services';
import { ChatHistoryStruct, ChatType } from '../../services/chat';
import { ChatModel } from './chatModel';

export const Chat = observer(({ chatId } : {chatId : string}) => {
    const [dontRenderChunks, setDontRenderChunks] = useState(false);
    const [searchParams, setSearchParams] = useSearchParams();
    const chatService = useGlobalStore().chatService;
    const chats = useGlobalStore().chatService.chatHistory;
    const user = useGlobalStore().authService.account;
    //
    const onScrollEvent = async function(evt: BaseSyntheticEvent) {
        if(evt.target.scrollHeight + evt.target.scrollTop == evt.target.clientHeight && !dontRenderChunks) {
            
            const chunkLength = await chatService.loadChatChunk(chatService.currentChat);
            console.log("loaded next chunk");
            console.log(chatService.currentChat?.Messages?.map(o => o.Message));
            if(chunkLength <= 0)
                setDontRenderChunks(true);
        }
    };

    const onSubmitMessage = async function(evt: FormEvent) {
        evt.preventDefault();
        if(chatService.currentChat != undefined)
            await chatService.sendMessage(chatService.currentChat?.ChatId, chatService.currentChat?.Type, ChatModel.chatText);
        else
            await chatService.sendMessage(chatId, ChatType.Regular, ChatModel.chatText);
        runInAction(() => ChatModel.chatText = '');
    };
    useEffect(() => { (async () => {
        runInAction(() => {
            if((chatService.currentChat = chats.find(o => o.ChatId == chatId)) == undefined)
            {
                chatService.loadChat(chatId).then(o => runInAction(() => {
                    chatService.currentChat = o;
                    chatService.chatPartner = chatService.currentChat?.Participants.find(e => e.Username == chatService.currentChat?.ChatId)
                }));
            }
            else
            {
                chatService.chatPartner = chatService.currentChat?.Participants.find(o => o.Username == chatService.currentChat?.ChatId)
            }
        });
        if(chatService.currentChat?.Messages == undefined && chatService.currentChat != undefined)
        {
            chatService.loadChatChunk(chatService.currentChat);
            console.log("loaded chunk");
        }})();
    }, []);
    return (
        <>
            
            <Container>
                <Row className='p-3 bg-white'>
                    <Col md={2}><h6 role="button" onClick={() => setSearchParams('')} className='text-center display-4 text-black'>Back</h6></Col>
                    <Col md={8}><h6 className='text-center display-4 text-black'>{chatService.currentChat?.Type == ChatType.Regular ? chatService.chatPartner?.FirstName + ' ' + chatService.chatPartner?.LastName : chatService.currentChat?.Name}</h6></Col>
                </Row>

                <Row className='chat-box' onScroll={onScrollEvent}>
                <ListGroup as="ol" className='p-0'>
                    {chatService.currentChat?.Messages?.map((o, index) => 
                        <ListGroup.Item key={index} as="div" className='border-0'>
                        <div className={`ms-2 me-auto ${o.Sender.Username == user.Username ? 'text-start' : 'text-end'}`}>
                        <div className="fw-bold">{o.Sender.FirstName} {o.Sender.LastName} ({new Date(o.Time).toLocaleDateString()})</div>
                        {o.Message}
                        </div>
                        </ListGroup.Item>
                    )}
                </ListGroup>
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