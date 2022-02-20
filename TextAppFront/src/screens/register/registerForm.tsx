import React, { useState } from 'react';
import { observer } from 'mobx-react';
import { runInAction } from 'mobx';
import { Form, Button, Container, Col, Row, Spinner } from 'react-bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';
import './registerForm.css'
import { useGlobalStore } from '../../services';
import { registerModel } from './registerFormModel';
import { useNavigate } from 'react-router-dom';

function RegisterFormComp() {
    const [validated, setValidated] = useState(false);
    const navigate = useNavigate();
    const globalStore = useGlobalStore();
    const register = async (evt: React.FormEvent<HTMLFormElement>) => {
        evt.preventDefault();
        evt.stopPropagation();
        if (evt.currentTarget.reportValidity())
        {
            setValidated(false);
            const responseMod = await globalStore.authService.accountRegister(registerModel.Username, registerModel.Password, 
                registerModel.Email, registerModel.Firstname, registerModel.Lastname);
            if(responseMod.ErrorId == undefined) // success
            {
                navigate('/login');
            }
            else
            {
                runInAction(() => registerModel.ErrorText = responseMod.Comment);
            }
        }
        else
        {
            setValidated(true);
        }
    };
    return (
    <Container className='registerBox'>
        <Form noValidate validated={validated} onSubmit={register}>
        <Row className="mb-3">
            <Form.Group as={Col} controlId="formUsername">
                <Form.Label>Username</Form.Label>
                <Form.Control required minLength={4} maxLength={24} type="text" value={registerModel.Username} onChange={(evt) => runInAction(() => registerModel.Username = evt.target.value)} placeholder="Enter username" />
            </Form.Group>

            <Form.Group as={Col} controlId="formPassword">
                <Form.Label>Password</Form.Label>
                <Form.Control required minLength={6} maxLength={24} type="password" value={registerModel.Password} onChange={(evt) => runInAction(() => registerModel.Password = evt.target.value)} placeholder="Password" />
            </Form.Group>
        </Row>
        <Row className="mb-3">
            <Form.Group as={Col} controlId="formName">
                <Form.Label>First name</Form.Label>
                <Form.Control required maxLength={30} type="text" value={registerModel.Firstname} onChange={(evt) => runInAction(() => registerModel.Firstname = evt.target.value)} placeholder="First name" />
            </Form.Group>

            <Form.Group as={Col} controlId="formSurname">
                <Form.Label>Last name</Form.Label>
                <Form.Control required maxLength={30} type="text" value={registerModel.Lastname} onChange={(evt) => runInAction(() => registerModel.Lastname = evt.target.value)} placeholder="Last name" />
            </Form.Group>
        </Row>
        <Row className="mb-3">

            <Form.Group as={Col} controlId="formEmail">
                <Form.Label>Email</Form.Label>
                <Form.Control required minLength={10} maxLength={60} type="text" value={registerModel.Email} onChange={(evt) => runInAction(() => registerModel.Email = evt.target.value)} placeholder="Email" />
            </Form.Group>
        </Row>

            <Form.Group className="mb-3">
                <Form.Text className='text-danger'>
                {registerModel.ErrorText}
                </Form.Text>
            </Form.Group>

            <Form.Group className="mb-3" controlId="formBasicCheckbox">
                <Form.Check required type="checkbox" label="I agree to the terms and conditions." />
            </Form.Group>

            <Row className="mt-4">
                <Col className="d-flex justify-content-center">
                    <Button variant="outline-primary" type="submit">Register</Button>
                </Col>
            </Row>

        </Form>
    </Container>
    );
}

export const RegisterForm = observer(RegisterFormComp);