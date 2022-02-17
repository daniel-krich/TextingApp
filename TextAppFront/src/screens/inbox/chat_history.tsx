import React from 'react';
import { useNavigate } from 'react-router-dom';
import { observer } from 'mobx-react';
import { Container, Row, Col, ListGroup, Badge } from 'react-bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';
import './chat.css';
import { useGlobalStore } from '../../services';

export const ChatHistory = observer(() => {
    const navigate = useNavigate();
    const chatService = useGlobalStore().chatService;
    return (
            <ListGroup className='chat-history' as="ol">
                {chatService.chatHistory.map((chat, index) =>
                    <ListGroup.Item role="button" key={index} onClick={() => navigate({ search: '?chat=' + chat.ChatId })} as="li" className="d-flex justify-content-between align-items-start">
                    <div className="ms-2 me-auto">
                    <div className="fw-bold">{chat.LastMessage.Sender.Username}</div>
                    {chat.LastMessage.Message}
                    </div>
                    <Badge bg="primary" pill>
                    1
                    </Badge>
                    </ListGroup.Item>
                )}
            </ListGroup>
    );
});