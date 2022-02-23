import { TokenStore } from "..";

export interface AbortableRequest {
    abort(): any,
    response: Promise<Response> 
}


export class Ajax{

    public static Post(url: string, jsondata: any, useToken: boolean): AbortableRequest{
        const useHeaders: any = useToken ?
            { 'Content-Type': 'application/json', 'authorization': 'Bearer ' + TokenStore.token } :
            { 'Content-Type': 'application/json' };
        const controller = new AbortController();
        const signal = controller.signal;
        return {
            abort: () => controller.abort(),
            response: fetch(url, {
                method: 'POST',
                mode: 'cors',
                cache: 'no-cache',
                credentials: 'same-origin',
                headers: useHeaders,
                redirect: 'error',
                referrerPolicy: 'no-referrer',
                signal,
                body: JSON.stringify(jsondata)
            })
        } as AbortableRequest;
    }

    public static Get(url: string, useToken: boolean): AbortableRequest{
        const useHeaders: any = useToken ?
            { 'Content-Type': 'application/json', 'authorization': 'Bearer ' + TokenStore.token } :
            { 'Content-Type': 'application/json' };
        const controller = new AbortController();
        const signal = controller.signal;
        return {
            abort: () => controller.abort(),
            response: fetch(url, {
                method: 'GET',
                headers: useHeaders,
                signal
            })
        } as AbortableRequest;
    }

    public static Put(url: string, jsondata: any, useToken: boolean): AbortableRequest{
        const useHeaders: any = useToken ?
            { 'Content-Type': 'application/json', 'authorization': 'Bearer ' + TokenStore.token } :
            { 'Content-Type': 'application/json' };
        const controller = new AbortController();
        const signal = controller.signal;
        return {
            abort: () => controller.abort(),
            response: fetch(url, {
                method: 'PUT',
                mode: 'cors',
                cache: 'no-cache',
                credentials: 'same-origin',
                headers: useHeaders,
                redirect: 'error',
                referrerPolicy: 'no-referrer',
                signal,
                body: JSON.stringify(jsondata)
            })
        } as AbortableRequest;

    }

    public static Delete(url: string, useToken: boolean): AbortableRequest{
        const useHeaders: any = useToken ?
            { 'Content-Type': 'application/json', 'authorization': 'Bearer ' + TokenStore.token } :
            { 'Content-Type': 'application/json' };
        const controller = new AbortController();
        const signal = controller.signal;
        return {
            abort: () => controller.abort(),
            response: fetch(url, {
                method: 'GET',
                headers: useHeaders,
                signal
            })
        } as AbortableRequest;

    }

}