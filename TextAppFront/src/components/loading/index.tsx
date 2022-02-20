import React, { useState, useEffect, FC } from 'react';
import { Container, Row, Col, Spinner } from 'react-bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';
import './loading.css'
import { useGlobalStore } from '../../services';
import { observer } from 'mobx-react';
import { runInAction } from 'mobx';

export const Loading : FC = observer(({children}) => {

    const globalStore = useGlobalStore();
    useEffect(() => {
        globalStore.loadApp().then(_ => runInAction(() => globalStore.isAppLoaded = true))
    }, []);

    return (
        <>
            <Container className={globalStore.isAppLoaded ? 'loading-screen hidden' : 'loading-screen'}>
                <Container className='vertical-center'>
                    <Row className='w-100 h-100 justify-content-center'>
                    <Spinner className='my-auto' animation="grow" variant="light" />
                    </Row>
                </Container>
            </Container>
            {globalStore.isAppLoaded ? children : null}
        </>
    );
});