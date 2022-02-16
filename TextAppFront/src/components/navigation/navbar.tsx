import React from 'react';
import { useNavigate } from "react-router-dom";
import { Container, Navbar, Nav, NavDropdown } from 'react-bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';
import { useGlobalStore } from '../../services';
import { TokenStore } from '../../services';

export function NavBar() {
    const navigation = useNavigate();
    const globalStore = useGlobalStore();
    const logout = () => {
        TokenStore.clearTokens();
        window.location.assign('/');
    };
    return (
        <Navbar collapseOnSelect expand="lg" bg="dark" variant="dark">
            <Container>
                <Navbar.Brand>My Text App</Navbar.Brand>
                <Navbar.Toggle aria-controls="responsive-navbar-nav" />
                <Navbar.Collapse id="responsive-navbar-nav">
                    <Nav className="me-auto">
                    <Nav.Link onClick={() => navigation('/')}>Home</Nav.Link>
                    {globalStore.authService.isLogged &&
                        <>
                            <Nav.Link onClick={() => navigation('/inbox')}>Inbox</Nav.Link>
                        </>
                    }
                    <NavDropdown title="Info" id="collasible-nav-dropdown">
                        <NavDropdown.Item onClick={() => navigation('/about')}>About</NavDropdown.Item>
                        <NavDropdown.Item onClick={() => navigation('/terms')}>Terms and conditions</NavDropdown.Item>
                    </NavDropdown>
                   
                    </Nav>
                    <Nav>
                    {globalStore.authService.isLogged ?
                        <>
                            <NavDropdown title={"Logged as: " + globalStore.authService.account.Username} id="collasible-nav-dropdown">
                                <NavDropdown.Item onClick={() => navigation('/myaccount')}>My account</NavDropdown.Item>
                                <NavDropdown.Item onClick={() => navigation('/settings')}>Settings</NavDropdown.Item>
                                <NavDropdown.Divider />
                                <NavDropdown.Item onClick={() => logout()}>Log out</NavDropdown.Item>
                            </NavDropdown>
                        </>
                        :
                        <>
                            <Nav.Link onClick={() => navigation('/signup')}>Sign up</Nav.Link>
                            <Nav.Link onClick={() => navigation('/login')}>Log in</Nav.Link>
                        </>
                    }
                    </Nav>
                </Navbar.Collapse>
            </Container>
        </Navbar>
    );
}
