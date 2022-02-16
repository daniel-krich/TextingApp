import React, { useState, useEffect } from 'react';
import { Container, Row, Col, Spinner } from 'react-bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';
import './loading.css'
import { useGlobalStore } from '../../services';
import { observer } from 'mobx-react';

export const Loading = observer(() => {
    const globalStore = useGlobalStore();

    return (
        globalStore.isAppLoaded ?
        <Container className='loading-screen hidden'>
            <Container className='vertical-center'>
                <Row className='w-100 h-100 justify-content-center'>
                   <Spinner className='my-auto' animation="grow" variant="light" />
                </Row>
            </Container>
        </Container>
        :
        <Container className='loading-screen'>
            <Container className='vertical-center'>
                <Row className='w-100 h-100 justify-content-center'>
                   <Spinner className='my-auto' animation="grow" variant="light" />
                </Row>
            </Container>
        </Container>
    );
});