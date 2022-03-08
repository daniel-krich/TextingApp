import { makeAutoObservable } from "mobx";
import { runInAction } from "mobx";
import { Ajax, AbortableRequest, Consts, TokenStore } from '../../services';

export interface User {
    username: string,
    firstName: string,
    lastName: string
}


class SearchModel {
    searchText: string = '';
    searchUsers: User[] = [] as User[];
    request: AbortableRequest = {} as AbortableRequest;
    constructor(){
        makeAutoObservable(this);
    }
}

export const SearchBoxModel = new SearchModel();