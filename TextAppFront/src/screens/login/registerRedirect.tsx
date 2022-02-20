import React from 'react';
import { observer } from 'mobx-react';
import { runInAction } from 'mobx';
import { Form, Button, Container, Row, Col } from 'react-bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';
import './loginForm.css'
import { useGlobalStore } from '../../services';
import { loginModel } from './loginFormModel';
import { useNavigate } from 'react-router-dom';

export function RegisterRedirect() {
    const navigate = useNavigate();
    return (
        <>
            <Container className='registerRedirect-box'>
                <Row className='d-flex justify-content-center'>
                    <Col className='text-center'>
                        <small className="text-muted">Not a member?</small>
                        
                    </Col>
                    <Col className='text-center'>
                        <button className="btn btn-link btn-sm" onClick={() => navigate('/register')}>Create account</button>
                    </Col>
                </Row>
            </Container>
        </>
    );
}