import React, { useState, BaseSyntheticEvent, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { observer } from 'mobx-react';
import { Container, Row, Col, ListGroup, Badge, Spinner } from 'react-bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';
import './chat.css';
import { ChatsStruct, ChatStruct, ResponseListenMessages, useGlobalStore } from '../../services';
import { runInAction } from 'mobx';
import { ChatType } from '../../services';
import { chatHistoryModel } from './chatHistoryModel';

export const ChatHistory = observer(() => {
    const navigate = useNavigate();
    const location = useLocation();
    const [loadingChats, setLoadingChats] = useState(true);
    const [dontRenderChunks, setDontRenderChunks] = useState(false);
    const chatService = useGlobalStore().chatService;
    const onScrollEvent = async function(evt: BaseSyntheticEvent) {
        if(evt.target.scrollTop + evt.target.clientHeight == evt.target.scrollHeight && !dontRenderChunks) {
            const chatsBatch = await chatService.loadChats(chatHistoryModel.chatsFeed?.length) as ChatsStruct;
            runInAction(() => chatHistoryModel.chatsFeed = chatHistoryModel.chatsFeed?.concat(chatsBatch.items));
            if(!chatsBatch.pageInfo.hasNextPage){
                setDontRenderChunks(true);
            }
        }
    };

    useEffect(() => {
        let isActive = true;
        (async () => {
            const initChatsBatch = await chatService.loadChats(0) as ChatsStruct;
            if(isActive) {
                runInAction(() => chatHistoryModel.chatsFeed = initChatsBatch.items);
                if(!initChatsBatch.pageInfo.hasNextPage){
                    setDontRenderChunks(true);
                }
                setLoadingChats(false);
            }
        })();

        return () => {
            isActive = false;
        };

    }, []);

    useEffect(() => {
        let isActive = true;
        function ProcessChatUpdates(e: ResponseListenMessages) {
            if(chatHistoryModel.chatsFeed) {
                const cursor = chatHistoryModel.chatsFeed?.find(o => o.chatId == e.data.listenChatUpdates.chatId);
                if(cursor) {
                    runInAction(() => cursor.lastMessage = e.data.listenChatUpdates.lastMessage);
                    
                    runInAction(() => {
                        if(chatHistoryModel.chatsFeed) {
                            delete chatHistoryModel.chatsFeed[chatHistoryModel.chatsFeed?.findIndex(o => o.chatId == e.data.listenChatUpdates.chatId) ?? 0];
                            chatHistoryModel.chatsFeed.unshift(cursor);
                        }
                    });
                    
                }
                else {
                    chatService.loadChat(e.data.listenChatUpdates.chatId, e.data.listenChatUpdates.type).then(o => {
                        if(!chatHistoryModel.chatsFeed?.find(o => o.chatId == e.data.listenChatUpdates.chatId)) {
                            runInAction(() => {
                                chatHistoryModel.chatsFeed?.unshift(o);
                            });
                        }
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
                <Row className='p-3'>
                    <h5 className='text-center display-4 text-black'>Your chats</h5>
                </Row>
                {
                loadingChats
                ?
                    (<ListGroup as="ol" className='chat-history p-0 align-items-center justify-content-center'>
                        <Spinner animation="grow" variant="dark">
                            <span className="visually-hidden">Loading...</span>
                        </Spinner>
                    </ListGroup>)
                :

                chatHistoryModel.chatsFeed && chatHistoryModel.chatsFeed.length > 0 ?
                    (<ListGroup className='chat-history' as="ol" onScroll={onScrollEvent}>
                    {chatHistoryModel.chatsFeed?.map((chat, index) =>
                        <ListGroup.Item role="button" key={index} onClick={() => navigate('/inbox/' + chat.chatId)} as="li" className="d-flex justify-content-between align-items-start">
                        <div className="ms-2 me-auto">
                        <div className="fw-bold">{chat.type == ChatType.Regular ? chat.chatId : chat.name}</div>
                        {chat.lastMessage.sender.firstName} {chat.lastMessage.sender.lastName}: {chat.lastMessage.message}
                        </div>
                        <Badge bg="primary" pill></Badge>
                        </ListGroup.Item>
                    )}
                    </ListGroup>)
                    :
                    (<ListGroup as="ol" className='chat-history p-0 align-items-center justify-content-center'>
                            <p className='text-center display-6 text-black text-muted'>Nothing here.</p>
                    </ListGroup>)
                
                }
            </Container>
        </>
    );
});