import React from 'react';
import { useNavigate } from "react-router-dom";
import { Container, Navbar, Nav, NavDropdown, Dropdown, Form, Row, Col, Button } from 'react-bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';
import './navbar.css';
import { useGlobalStore, TokenStore } from '../../services';
import { SearchBoxModel } from './navbarSearchModel';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react';

export const NavBar = observer(() => {
    const navigation = useNavigate();
    const globalStore = useGlobalStore();
    const onSearchChange = function(e: React.ChangeEvent<HTMLInputElement>) {
        runInAction(() => SearchBoxModel.searchText = e.target.value);
        SearchBoxModel.searchForUsers();
    };
    const logout = () => {
        TokenStore.clearTokens();
        window.location.assign('/');
    };
    return (
        <Navbar collapseOnSelect expand="md" bg="dark" variant="dark">
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
                    
                    {globalStore.authService.isLogged &&
                        <Nav className="me-auto">
                            <Form onSubmit={(e) => { e.preventDefault(); e.stopPropagation(); }}>
                                <Row className='d-flex'>
                                    <Form.Group as={Col}>
                                        
                                        <Dropdown>
                                            <Dropdown.Toggle className='p-0 m-0 d-inline-flex invisible'>
                                                <Form.Control maxLength={30} value={SearchBoxModel.searchText} onChange={onSearchChange} className='visible' type="text" placeholder='search...'/>
                                            </Dropdown.Toggle>

                                            <Dropdown.Menu className='search-results'>
                                            {
                                                !SearchBoxModel.searchUsers.length ?
                                                <>
                                                    <Dropdown.Header>No results</Dropdown.Header>
                                                </>
                                                :
                                                <>
                                                    {
                                                        !!SearchBoxModel.searchText.length ?
                                                        <Dropdown.Header>Search results</Dropdown.Header> :
                                                        <Dropdown.Header>Previous results</Dropdown.Header>
                                                    }
                                                    
                                                    {
                                                        SearchBoxModel.searchUsers.map((user, index) =>
                                                            <Dropdown.Item onClick={() => navigation('/profile/' + user.Username)} key={index}>{user.FirstName + ' ' + user.LastName}</Dropdown.Item>
                                                        )
                                                    }
                                                </>
                                            }
                                            </Dropdown.Menu>
                                        </Dropdown>
                                        
                                    </Form.Group>
                                </Row>
                            </Form>
                        </Nav>
                    }

                    <Nav>
                    {globalStore.authService.isLogged ?
                        <>
                            <NavDropdown title={"Logged as: " + globalStore.authService.account.username} id="collasible-nav-dropdown">
                                <NavDropdown.Item onClick={() => navigation('/myaccount')}>My account</NavDropdown.Item>
                                <NavDropdown.Item onClick={() => navigation('/settings')}>Settings</NavDropdown.Item>
                                <NavDropdown.Divider />
                                <NavDropdown.Item onClick={() => logout()}>Log out</NavDropdown.Item>
                            </NavDropdown>
                        </>
                        :
                        <>
                            <Nav.Link onClick={() => navigation('/register')}>Sign up</Nav.Link>
                            <Nav.Link onClick={() => navigation('/login')}>Log in</Nav.Link>
                        </>
                    }
                    </Nav>
                </Navbar.Collapse>
            </Container>
        </Navbar>
    );
});
