import React, { useEffect } from 'react';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react';
import { useGlobalStore } from '../../services';
import { useNavigate, Navigate } from "react-router-dom";
import { NavBar, Feed } from '../../components';

export const Home = observer(() => {
    const globalcontext = useGlobalStore();
    const authService = globalcontext.authService;
    return (
        globalcontext.isAppLoaded && authService.isLogged ?
        <>
            <NavBar/>
            <Feed/>
        </>
        :
        <Navigate to="/login" />
)});