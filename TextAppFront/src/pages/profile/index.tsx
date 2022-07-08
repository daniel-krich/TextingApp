import React, { useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { runInAction } from 'mobx';
import { Container, Navbar, Nav, NavDropdown, Dropdown, Form, Row, Col, Button } from 'react-bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';
import './profile.css';
import { observer } from 'mobx-react';
import { useGlobalStore } from '../../services/globalContext';
import { NavBar } from '../../components';
import { ProfileModel, User } from './profileModel';
import { SEARCH_USER_BY_NAME } from '../../services';

export const Profile = observer(() => {
    const { profileId } = useParams();
    const navigate = useNavigate();
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
                    <Row className='justify-content-center'>
                        <div className='profile-box'>
                            <p className='fullname'>
                                {ProfileModel.currentUser?.firstName + ' ' + ProfileModel.currentUser?.lastName}
                            </p>
                            <p className='username'>
                                {ProfileModel.currentUser?.username}
                            </p>
                            {currentUser.username !== ProfileModel.currentUser.username &&
                            (
                                <>
                                    <hr />
                                    <Button onClick={() => navigate('/inbox/' + ProfileModel.currentUser?.username)} className='start-chat'>
                                        Message me
                                    </Button>
                                </>
                            )
                            }
                            
                        </div>
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