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
    return (
        !authService.isLogged && globalcontext.isAppLoaded ?
        <>
            <NavBar/>
            <h6>hi, nothing here, you're not logged in</h6>
        </>
        :
        <>
            <NavBar/>
            <h1>Welcome, {authService.account.username}</h1>
            <h3>email: {authService.account.email}</h3>
        </>
    );
});