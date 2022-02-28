import {
    ApolloClient,
    InMemoryCache,
    ApolloProvider,
    useQuery,
    gql,
    NormalizedCacheObject
} from "@apollo/client";

export class Apollo {
    instance: ApolloClient<NormalizedCacheObject>;
    constructor(){
        this.instance = createApolloClient("");
    }

    setJWT(jwt: string) {
        this.instance = createApolloClient(jwt);
    }
}

function createApolloClient(jwt: string): ApolloClient<NormalizedCacheObject> {
    if(jwt.length > 0) {
        return new ApolloClient({
            uri: 'https://localhost:44310/graphql',
            headers: {
                'authorization': 'Bearer ' + jwt
            },
            cache: new InMemoryCache()
        });
    }
    else {
        return new ApolloClient({
            uri: 'https://localhost:44310/graphql',
            cache: new InMemoryCache()
        });
    }
}

export * from './queries';