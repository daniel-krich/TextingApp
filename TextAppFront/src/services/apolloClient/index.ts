import {
    ApolloClient,
    InMemoryCache,
    ApolloProvider,
    useQuery,
    gql,
    NormalizedCacheObject,
    HttpLink,
    WatchQueryFetchPolicy,
    DefaultOptions,
} from "@apollo/client";

import { split } from '@apollo/client/link/core';

import { getMainDefinition } from '@apollo/client/utilities';

import { WebSocketLink } from "@apollo/client/link/ws";

export class Apollo {
    instance: ApolloClient<NormalizedCacheObject>;
    constructor(){
        this.instance = createApolloClient();
    }

    setJWT(jwt: string) {
        this.instance = createApolloClient(jwt);
    }
}

const httpLink = (jwt: string | undefined) => {
    if(jwt != undefined) {
        return new HttpLink({
            uri: 'https://localhost:44310/graphql',
            headers: {
                'authorization': 'Bearer ' + jwt,
            },
        });
    }
    else {
        return new HttpLink({
            uri: 'https://localhost:44310/graphql'
        });
    }
};
  
const wsLink = (jwt: string | undefined) => {
    if(jwt != undefined){
        return new WebSocketLink({
            uri: 'wss://localhost:44310/graphql',
            options: {
                lazy: true,
                reconnect: true,
                connectionParams: {
                    Authorization: 'Bearer ' + jwt,
                }
            }
        });
    }
    else {
        return new WebSocketLink({
            uri: 'wss://localhost:44310/graphql',
            options: {
                lazy: true,
                reconnect: true
            }
        });
    }
};

const link = (jwt: string | undefined = undefined) => split(
    ({ query }) => {
        const definition = getMainDefinition(query);
        return (
        definition.kind === 'OperationDefinition' &&
        definition.operation === 'subscription'
        );
    },
    wsLink(jwt),
    httpLink(jwt),
);

const defaultOptions: DefaultOptions = {
    watchQuery: {
        fetchPolicy: 'network-only'
    },
    query: {
        fetchPolicy: 'network-only'
    }
}

function createApolloClient(jwt: string | undefined = undefined): ApolloClient<NormalizedCacheObject> {
    return new ApolloClient({
        link: link(jwt),
        cache: new InMemoryCache(),
        defaultOptions: defaultOptions
    });
}

export * from './queries';
export * from './mutations';
export * from './subscriptions';