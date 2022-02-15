import React, { useEffect } from 'react';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react';
import { useGlobalStore } from '../../services';
import { useNavigate } from "react-router-dom";
import { loginModel } from './loginmodel';
import { NavBar } from '../../components';

export const Home = observer(() => {
    const globalcontext = useGlobalStore();
    const authService = globalcontext.authService;
    const navigate = useNavigate();
    if(authService.isLogged)
    {
        return <>
            <NavBar/>
            <h1>Welcome, {authService.account.Username}</h1>
            <h3>email: {authService.account.Email}</h3>
            <h3>about: {authService.account.Description}</h3>
        </>;
    }
    else
    {
        return <>
            <NavBar/>
            <h1>hello visitor</h1>
        </>;
    }
});