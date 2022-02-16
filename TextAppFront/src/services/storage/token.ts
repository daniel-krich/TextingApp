import Storage from './storageBase'

enum TokenNames {
    AUTH_TOKEN = '_ATOKEN'
}

class Tokens extends Storage<string> {
    constructor(){
        super();
    }

    set token(settoken: string) {
        this.set(TokenNames.AUTH_TOKEN, settoken);
    }

    get token(): any {
        return this.get(TokenNames.AUTH_TOKEN);
    }

    clearTokens() {
        this.clearItem(TokenNames.AUTH_TOKEN);
    }
}

export const TokenStore = new Tokens();