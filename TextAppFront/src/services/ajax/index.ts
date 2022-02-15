export class Ajax{

    public static async Post(url: string, jsondata: any): Promise<Response>{
        return fetch(url, {
            method: 'POST',
            mode: 'cors',
            cache: 'no-cache',
            credentials: 'same-origin',
            headers: {
              'Content-Type': 'application/json'
            },
            redirect: 'error',
            referrerPolicy: 'no-referrer',
            body: JSON.stringify(jsondata)
        });
    }

    public static async Get(url: string): Promise<Response>{
        return fetch(url, { method: 'GET' });
    }

    public static async Put(url: string, jsondata: any): Promise<Response>{
        return fetch(url, {
            method: 'PUT',
            mode: 'cors',
            cache: 'no-cache',
            credentials: 'same-origin',
            headers: {
              'Content-Type': 'application/json'
            },
            redirect: 'error',
            referrerPolicy: 'no-referrer',
            body: JSON.stringify(jsondata)
        });

    }

    public static async Delete(url: string): Promise<Response>{
        return fetch(url, { method: 'GET' });

    }

}