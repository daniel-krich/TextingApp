import React, { useEffect } from 'react';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react';
import { useGlobalStore } from '../../services/globalContext';

export const Contacts = observer(() => {
    const globalcontext = useGlobalStore();
    return <>
        <h3>{globalcontext.clickme}</h3>
        <button onClick={() => runInAction(() => globalcontext.clickme++)}>click me to update</button>
    </>;
});