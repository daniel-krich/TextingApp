import React, { useEffect } from 'react';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react';
import { useGlobalStore } from '../../services';
import { useNavigate, Navigate } from "react-router-dom";
import { Container, Row, Col } from 'react-bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';
import './index.css';

function ParseTimeOffset(time: Date): string{
    const timeOffsetSeconds = (new Date().getTime() - time.getTime()) / 1000;
    if(timeOffsetSeconds >= 86400) { // days
        return `${Math.round(timeOffsetSeconds/86400)}d`;
    }
    else if(timeOffsetSeconds >= 3600) { // hours
        return `${Math.round(timeOffsetSeconds/3600)}h`;
    }
    else if(timeOffsetSeconds > 60) { // mins
        return `${Math.round(timeOffsetSeconds/60)}m`;
    }
    else if(timeOffsetSeconds >= 1) { // secs
        return `${Math.round(timeOffsetSeconds)}s`;
    }
    else {
        return "Now";
    }
}

export function Post({text, date, creator}: {text: string, date: Date, creator: string}) {

    return (
        <Container className='post-container'>
            <Row>
                <Col className='fw-bold text-start'>{creator}</Col>
                <Col className='fw-light text-end'>{ParseTimeOffset(date)}</Col>
            </Row>
            <Row className='text-muted ms-2'>
                {text.split('\n').map(txt => <Container>{txt}</Container>)}
            </Row>
        </Container>
    );
}