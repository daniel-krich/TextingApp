import React from 'react';
import { observer } from 'mobx-react';
import { runInAction } from 'mobx';
import { Form, Button, Container, Row, Col } from 'react-bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';
import './registerForm.css'
import { useGlobalStore } from '../../services';
import { registerModel } from './registerFormModel';
import { useNavigate } from 'react-router-dom';

export function LoginRedirect() {
    const navigate = useNavigate();
    return (
        <>
            <Container className='loginRedirect-box'>
                <Row className='d-flex justify-content-center'>
                    <Col className='text-center'>
                        <small className="text-muted">Have an account?</small>
                        
                    </Col>
                    <Col className='text-center'>
                        <button className="btn btn-link btn-sm" onClick={() => navigate('/login')}>Sign in</button>
                    </Col>
                </Row>
            </Container>
        </>
    );
}