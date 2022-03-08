import React, { useEffect } from 'react';
import { useParams } from 'react-router-dom';
import { runInAction } from 'mobx';
import { Container, Navbar, Nav, NavDropdown, Dropdown, Form, Row, Col, Button } from 'react-bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';
import { observer } from 'mobx-react';
import { useGlobalStore } from '../../services/globalContext';
import { NavBar } from '../../components';
import { ProfileModel, User } from './profileModel';
import { SEARCH_USER_BY_NAME } from '../../services';

export const Profile = observer(() => {
    const { profileId } = useParams();
    const globalcontext = useGlobalStore();
    const apolloInstance = globalcontext.apolloService.instance;
    const currentUser = globalcontext.authService.account;
    useEffect(() => {
        (async () => {
            const queryResponse = (await apolloInstance.query({query: SEARCH_USER_BY_NAME, variables: {username: profileId ?? currentUser.username, exact: true}})).data["searchUser"]["items"][0] as User;
            runInAction(() => ProfileModel.currentUser = queryResponse);
        })();
    }, [profileId]);
    return(
        <>
            <NavBar />
            { ProfileModel.currentUser ?
                <Container>
                    <Row>
                        <Col>Profile</Col>
                        <Col>{ProfileModel.currentUser.firstName} {ProfileModel.currentUser.lastName}</Col>
                    </Row>
                </Container>
                :
                <>
                Not found
                </>
            }
        </>
    );
});