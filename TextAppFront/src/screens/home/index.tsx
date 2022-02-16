import React, { useEffect } from 'react';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react';
import { useGlobalStore } from '../../services';
import { useNavigate } from "react-router-dom";
import { NavBar } from '../../components';

export const Home = observer(() => {
    const globalcontext = useGlobalStore();
    const authService = globalcontext.authService;
    const navigate = useNavigate();
    useEffect(() => {
        if(!authService.isLogged && globalcontext.isAppLoaded)
            navigate('/login');
    }, [globalcontext.isAppLoaded]);
    return (
        !authService.isLogged && globalcontext.isAppLoaded ?
        <></>
        :
        <>
            <NavBar/>
            <h1>Welcome, {authService.account.Username}</h1>
            <h3>email: {authService.account.Email}</h3>
        </>
    );
});