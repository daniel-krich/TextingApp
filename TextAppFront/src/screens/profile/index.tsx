import React, { useEffect } from 'react';
import { useParams } from 'react-router-dom';
import { runInAction } from 'mobx';
import { Container, Navbar, Nav, NavDropdown, Dropdown, Form, Row, Col, Button } from 'react-bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';
import { observer } from 'mobx-react';
import { useGlobalStore } from '../../services/globalContext';
import { NavBar } from '../../components';
import { ProfileModel } from './profileModel';

export const Profile = observer(() => {
    const { profileId } = useParams();
    const globalcontext = useGlobalStore();
    useEffect(() => {
        if(profileId ?? false) // if profileId is not empty
        {
            ProfileModel.tryGetProfileInfo(profileId ?? '');
        }
        else
        {
            ProfileModel.getProfileInfo(globalcontext.authService.account);
        }
    }, [profileId]);
    return(
        <>
            <NavBar />
            { ProfileModel.currentUser.Username != undefined ?
                <Container>
                    <Row>
                        <Col>Profile</Col>
                        <Col>{ProfileModel.currentUser.FirstName} {ProfileModel.currentUser.LastName}</Col>
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