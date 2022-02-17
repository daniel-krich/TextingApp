import React from 'react';
import ReactDOM from 'react-dom';
import './index.css';
import reportWebVitals from './reportWebVitals';
import { GlobalContextProvider } from './services';
import ContentRouter from './router';
import { Loading } from './screens';

ReactDOM.render(
  <React.StrictMode>
    <GlobalContextProvider>
        <Loading>
            <ContentRouter />
        </Loading>
    </GlobalContextProvider>
  </React.StrictMode>,
  document.getElementById('root')
);

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();
