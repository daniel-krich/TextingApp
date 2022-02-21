export interface AbortableRequest {
    abort(): any,
    response: Promise<Response> 
}


export class Ajax{

    public static Post(url: string, jsondata: any): AbortableRequest{
        const controller = new AbortController();
        const signal = controller.signal;
        return {
            abort: () => controller.abort(),
            response: fetch(url, {
                method: 'POST',
                mode: 'cors',
                cache: 'no-cache',
                credentials: 'same-origin',
                headers: {
                  'Content-Type': 'application/json'
                },
                redirect: 'error',
                referrerPolicy: 'no-referrer',
                signal,
                body: JSON.stringify(jsondata)
            })
        } as AbortableRequest;
    }

    public static Get(url: string): AbortableRequest{
        const controller = new AbortController();
        const signal = controller.signal;
        return {
            abort: () => controller.abort(),
            response: fetch(url, { method: 'GET', signal })
        } as AbortableRequest;
    }

    public static Put(url: string, jsondata: any): AbortableRequest{
        const controller = new AbortController();
        const signal = controller.signal;
        return {
            abort: () => controller.abort(),
            response: fetch(url, {
                method: 'PUT',
                mode: 'cors',
                cache: 'no-cache',
                credentials: 'same-origin',
                headers: {
                  'Content-Type': 'application/json'
                },
                redirect: 'error',
                referrerPolicy: 'no-referrer',
                signal,
                body: JSON.stringify(jsondata)
            })
        } as AbortableRequest;

    }

    public static Delete(url: string): AbortableRequest{
        const controller = new AbortController();
        const signal = controller.signal;
        return {
            abort: () => controller.abort(),
            response: fetch(url, { method: 'GET', signal })
        } as AbortableRequest;

    }

}