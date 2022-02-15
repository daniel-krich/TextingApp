import React, { FC } from 'react';
import { createGlobalStore, GlobalStore } from './globalStore';

const GlobalContext = React.createContext<GlobalStore>({} as GlobalStore);


export const GlobalContextProvider: FC = ({children}) => {
    const globalStore = createGlobalStore();
    return (
        <GlobalContext.Provider value={globalStore}>
            {children}
        </GlobalContext.Provider>
    );
};

export const useGlobalStore = () => React.useContext(GlobalContext);