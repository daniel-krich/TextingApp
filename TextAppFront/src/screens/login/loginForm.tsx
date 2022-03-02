import React, { useState } from 'react';
import { observer } from 'mobx-react';
import { runInAction } from 'mobx';
import { Form, Button, Container, Row, Col } from 'react-bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';
import './loginForm.css'
import { useGlobalStore } from '../../services';
import { loginModel } from './loginFormModel';
import { useNavigate } from 'react-router-dom';

function LoginFormComp() {
    const [validated, setValidated] = useState(false);
    const navigate = useNavigate();
    const globalStore = useGlobalStore();
    const login = async (evt: React.FormEvent<HTMLFormElement>) => {
        evt.preventDefault();
        evt.stopPropagation();
        if(evt.currentTarget.reportValidity())
        {
            setValidated(false);
            const loginRes = await globalStore.authService.accountLogin(loginModel.Username, loginModel.Password);
            if(loginRes == undefined) // success
            {
                window.location.assign('/');
            }
            else
            {
                runInAction(() => loginModel.ErrorText = loginRes);
            }
        }
        else
        {
            setValidated(true);
        }
    };
    return (
    <Container className='loginBox'>
        <Form noValidate validated={validated} onSubmit={login}>
            <Form.Group className="mb-3" controlId="formBasicText">
                <Form.Label>Username</Form.Label>
                <Form.Control required minLength={4} maxLength={24} type="text" value={loginModel.Username} onChange={(evt) => runInAction(() => loginModel.Username = evt.target.value)} placeholder="Enter username" />
            </Form.Group>

            <Form.Group className="mb-3" controlId="formBasicPassword">
                <Form.Label>Password</Form.Label>
                <Form.Control required minLength={6} maxLength={24} type="password" value={loginModel.Password} onChange={(evt) => runInAction(() => loginModel.Password = evt.target.value)} placeholder="Password" />
                <Form.Text className="text-muted">
                Never share your password with anyone else.
                </Form.Text>
            </Form.Group>

            <Form.Group className="mb-3">
                <Form.Text className='text-danger'>
                {loginModel.ErrorText}
                </Form.Text>
            </Form.Group>

            <Form.Group className="mb-3" controlId="formBasicCheckbox">
                <Form.Check type="checkbox" label="Remember me" />
            </Form.Group>

            <Row className="mt-4">
                <Col className="d-flex justify-content-center">
                    <Button variant="outline-primary" type="submit">Login</Button>
                </Col>
            </Row>

        </Form>
    </Container>
    );
}

export const LoginForm = observer(LoginFormComp);