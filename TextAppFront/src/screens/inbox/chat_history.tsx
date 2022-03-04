import React, { useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { observer } from 'mobx-react';
import { Container, Row, Col, ListGroup, Badge } from 'react-bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';
import './chat.css';
import { useGlobalStore } from '../../services';
import { runInAction } from 'mobx';
import { ChatType } from '../../services';

export const ChatHistory = observer(() => {
    const navigate = useNavigate();
    const location = useLocation();
    const chatService = useGlobalStore().chatService;
    useEffect(() => {
        runInAction(() => chatService.chatHistory.items = chatService.chatHistory.items.sort((a,b) => new Date(b.lastMessage.time).getTime() - new Date(a.lastMessage.time).getTime()));
        console.log(chatService.chatHistory.items);
    },[chatService.chatHistory.items]);
    return (
        <>
            <Row className='p-3'>
                <h5 className='text-center display-4 text-black'>Your chats</h5>
            </Row>
            <ListGroup className='chat-history' as="ol">
                {chatService.chatHistory.items.filter(o => o.lastMessage != null).map((chat, index) =>
                    <ListGroup.Item role="button" key={index} onClick={() => navigate('/inbox/' + chat.chatId)} as="li" className="d-flex justify-content-between align-items-start">
                    <div className="ms-2 me-auto">
                    <div className="fw-bold">{chat.type == ChatType.Regular ? chat.chatId : chat.name}</div>
                    {chat.lastMessage.sender.firstName} {chat.lastMessage.sender.lastName}: {chat.lastMessage.message}
                    </div>
                    <Badge bg="primary" pill></Badge>
                    </ListGroup.Item>
                )}
            </ListGroup>
        </>
    );
});