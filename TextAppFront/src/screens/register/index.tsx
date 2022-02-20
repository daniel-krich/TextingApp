import React from 'react';
import { Container, Row, Col } from 'react-bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';
import './registerForm.css'
import { useGlobalStore } from '../../services';
import { RegisterForm } from './registerForm';
import { LoginRedirect } from './loginRedirect';

export function Register() {
    return (
        <Container fluid>
            <Row className="registerHeaderRow">
                <Col className='my-auto'>
                    <h1 className='text-center display-4'>Register page</h1>
                </Col>
            </Row>
            <Row>
                <RegisterForm />
            </Row>
            <Row>
                <LoginRedirect/>
            </Row>
        </Container>
    );
}