import React from 'react';
import { observer } from 'mobx-react';
import { runInAction } from 'mobx';
import { Form, Button, Container } from 'react-bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';
import './loginForm.css'
import { useGlobalStore } from '../../services';
import { loginModel } from './loginFormModel';
import { useNavigate } from 'react-router-dom';

function LoginFormComp() {
    const navigate = useNavigate();
    const globalStore = useGlobalStore();
    const login = async (evt: React.FormEvent) => {
        evt.preventDefault();
        await globalStore.authService.accountLogin(loginModel);
        if(globalStore.authService.isLogged)
        {
            navigate('/');
        }
        else
        {
            loginModel.ErrorText = "Invalid username or password";
        }
    };
    return (
    <Container className='loginBox'>
        <Form onSubmit={login}>
            <Form.Group className="mb-3" controlId="formBasicText">
                <Form.Label>Username</Form.Label>
                <Form.Control type="text" value={loginModel.Username} onChange={(evt) => runInAction(() => loginModel.Username = evt.target.value)} placeholder="Enter username" />
            </Form.Group>

            <Form.Group className="mb-3" controlId="formBasicPassword">
                <Form.Label>Password</Form.Label>
                <Form.Control type="password" value={loginModel.Password} onChange={(evt) => runInAction(() => loginModel.Password = evt.target.value)} placeholder="Password" />
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
                <Form.Check type="checkbox" label="This is my personal device" />
            </Form.Group>

            <Button variant="outline-primary" type="submit">
                Submit
            </Button>

        </Form>
    </Container>
    );
}

export const LoginForm = observer(LoginFormComp);