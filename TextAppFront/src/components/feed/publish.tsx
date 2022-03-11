import React, { useEffect, useState } from 'react';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react';
import { useGlobalStore } from '../../services';
import { useNavigate, Navigate } from "react-router-dom";
import { Container, Row, Col, Form } from 'react-bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';

export function Publish() {
    const [postTextBox, setPostTextBox] = useState("");
    return (
        <Container>
            <Row>
            <Form onSubmit={(e) => { e.preventDefault(); e.stopPropagation(); }}>
                <Row className='d-flex justify-content-center mb-4 mt-4'>
                    <Form.Group as={Col} lg={4}>
                        <Form.Control maxLength={30} value={postTextBox} onChange={(e) => setPostTextBox(e.target.value)} className='visible' type="text" placeholder='Whats up?'/>              
                    </Form.Group>
                    <Form.Group as={Col} md={2}>
                        <Form.Control className='visible' value="Publish" type="submit"/>              
                    </Form.Group>
                </Row>
            </Form>
            </Row>
        </Container>
    );
}