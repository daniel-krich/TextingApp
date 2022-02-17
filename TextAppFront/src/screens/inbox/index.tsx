import React from 'react';
import { Container, Row, Col } from 'react-bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';
import './chat.css';
import { useGlobalStore } from '../../services';
import { NavBar } from '../../components';
import { ChatHistory } from './chat_history';
import { Chat } from './chat';
import { useSearchParams } from 'react-router-dom';

export function Inbox() {
    const [searchParams, setSearchParams] = useSearchParams();
    const chatIdParam = searchParams.get('chat') || '';
    return (
        <>
            <NavBar/>
            <Container>
                <Row className='chat-row'>
                    <Col sm={12}>
                        {
                            chatIdParam.length > 0 ?
                            <Chat/> : <ChatHistory/>

                        }
                    </Col>
                </Row>
            </Container>
        </>
    );
}