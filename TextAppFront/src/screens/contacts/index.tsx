import React, { useEffect } from 'react';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react';
import { useGlobalStore } from '../../services/globalContext';

export const Contacts = observer(() => {
    const globalcontext = useGlobalStore();
    return <>
    </>;
});