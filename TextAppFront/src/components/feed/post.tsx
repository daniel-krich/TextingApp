import React, { useEffect, useState } from 'react';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react';
import { useGlobalStore } from '../../services';
import { useNavigate, Navigate } from "react-router-dom";
import { Container, Row, Col } from 'react-bootstrap';
import { Utils } from '../../services';
import 'bootstrap/dist/css/bootstrap.min.css';
import './index.css';


export function Post({text, date, creator}: {text: string, date: Date, creator: string}) {
    
    const [currentTime, setCurrentTime] = useState(new Date());
    return (
        <Container className='post-container'>
            <Row>
                <Col className='fw-bold text-start'>{creator}</Col>
                <Col className='fw-light text-end'>{Utils.ParseTimeOffset(currentTime, date)}</Col>
            </Row>
            <Row className='text-muted ms-2'>
                {text.split('\n').map(txt => <Container>{txt}</Container>)}
            </Row>
        </Container>
    );
}