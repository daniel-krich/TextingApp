import React from 'react';
import { Container, Row, Col } from 'react-bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';
import './chat.css';
import { useGlobalStore } from '../../services';
import { NavBar } from '../../components';
import { ChatHistory } from './chat_history';
import { Chat } from './chat';
import { useSearchParams, useParams, Routes, Route, Link, Outlet } from 'react-router-dom';

export function Inbox() {
    const { chatId } = useParams();
    return (
        <>
            <NavBar/>
            <Container>
                <Row className='chat-row'>
                    <Col sm={12}>
                        {chatId ? <Chat/> : <ChatHistory/>}
                    </Col>
                </Row>
            </Container>
        </>
    );
}