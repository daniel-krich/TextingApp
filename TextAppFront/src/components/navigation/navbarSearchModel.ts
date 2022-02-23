import { makeAutoObservable } from "mobx";
import { runInAction } from "mobx";
import { Ajax, AbortableRequest, Consts, TokenStore } from '../../services';

interface User {
    Username: string,
    FirstName: string,
    LastName: string
}


class SearchModel {
    searchText: string = '';
    searchUsers: User[] = [] as User[];
    request: AbortableRequest = {} as AbortableRequest;
    constructor(){
        makeAutoObservable(this);
    }

    searchForUsers() {
        if(!this.searchText.length) return;
        if(this.request.abort != undefined) {
            this.request.abort();
        }
        this.request = Ajax.Post(Consts.URL + '/api/User/search', {
            Query: this.searchText
        }, true);
        this.request.response.then(async (res) => {
            const searchResult = await res.json() as User[] | undefined;
            this.clearSearchUsers();
            if(!!searchResult?.length) // check if array not empty
            {
                searchResult.forEach(user => {
                    runInAction(() => this.searchUsers.push(user));
                });
            }
        }).catch(_ => {});
    }

    clearSearchUsers() {
        runInAction(() => this.searchUsers = []);
    }
}

export const SearchBoxModel = new SearchModel();