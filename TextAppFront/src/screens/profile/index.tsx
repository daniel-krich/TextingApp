import React, { useEffect } from 'react';
import { useParams } from 'react-router-dom';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react';
import { useGlobalStore } from '../../services/globalContext';

export const Profile = observer(() => {
    const { profileId } = useParams();
    const globalcontext = useGlobalStore();
    return <>
        Hi, {profileId ?? 'not specified'}
    </>;
});