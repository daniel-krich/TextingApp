import React from 'react';
import { Container, Row, Col } from 'react-bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';
import './loginForm.css'
import { useGlobalStore } from '../../services';
import { LoginForm } from './loginForm';
import { RegisterRedirect } from './registerRedirect';

export function Login() {
    return (
        <Container fluid>
            <Row className="loginHeaderRow">
                <Col className='my-auto'>
                    <h1 className='text-center display-4'>Login page</h1>
                </Col>
            </Row>
            <Row>
                <LoginForm />
            </Row>
            <Row>
                <RegisterRedirect/>
            </Row>
        </Container>
    );
}