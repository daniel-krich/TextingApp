import React from 'react';
import { useNavigate } from "react-router-dom";
import { Container, Navbar, Nav, NavDropdown, Dropdown, Form, Row, Col, Button, ToastContainer, Toast, Collapse } from 'react-bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';
import { useGlobalStore, TokenStore } from '../../services';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react';

export const Notifications = observer(() => {
    const notifyService = useGlobalStore().notifyService;
    return (
        <ToastContainer className='position-absolute bottom-0 start-0 p-3'>
            {
                notifyService.notificationList.map((notif, index) => {
                    setTimeout(() => runInAction(() => notifyService.notificationList.splice(index, 1)), 5000);
                    return (
                        <Toast key={index} onClose={() => runInAction(() => notifyService.notificationList.splice(index, 1))}>
                            <Toast.Header>
                            <img src="holder.js/20x20?text=%20" className="rounded me-2" alt="" />
                            <strong className="me-auto">{notif.Title}</strong>
                            <small className="text-muted">{notif.Type}</small>
                            </Toast.Header>
                            <Toast.Body>{notif.Body}</Toast.Body>
                        </Toast>
                    );
                })
            }
        </ToastContainer>
    );
});