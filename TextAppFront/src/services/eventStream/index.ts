enum StreamState {
    INITIALIZING = -1,
    CONNECTING = 0,
    OPEN = 1,
    CLOSED = 2
}

interface EventOptions {
    headers: any | undefined,
    payload: any | undefined,
    method: any | undefined,
    withCredentials: any | undefined
}

interface EventStreamStruct {
    id: any | undefined,
    retry: any | undefined,
    data: any | undefined,
    event: any | undefined
}

export class EventStream {
    url: string;
    options: EventOptions = {} as EventOptions;
    listeners: any = {};
    xhr: any = {};
    chunk: string = '';
    progress: number = 0;
    readyState: string = '';

    constructor(url: string, options: EventOptions = {} as EventOptions){
        this.url = url;
        this.options = options;
    }

    addEventListener(type: string, listener: any) {
        if (this.listeners[type] === undefined) {
            this.listeners[type] = [];
        }
      
        if (this.listeners[type].indexOf(listener) === -1) {
            this.listeners[type].push(listener);
        }
    }
  
    removeEventListener(type: string, listener: any) {
        if (this.listeners[type] === undefined) return;
      
        var filtered: any = [];
        this.listeners[type].forEach(function(element: any) {
            if (element !== listener) {
                filtered.push(element);
            }
        });
        if (filtered.length === 0) {
            delete this.listeners[type];
        }
        else {
            this.listeners[type] = filtered;
        }
    }

    dispatchEvent(e: any): Boolean {
        if (!e) return true;
      
        e.source = this;

        if (this.listeners[e.type]) {
            return this.listeners[e.type].every(function(callback: any) {
                callback(e);
                return !e.defaultPrevented;
            });
        }
        return true;
    }

    _setReadyState(state: string) {
        var event = new CustomEvent('readystatechange');
        this.readyState = state;
        this.dispatchEvent(event);
    }
  
    _onStreamFailure(e: any) {
        this.dispatchEvent(new CustomEvent('error'));
        this.close();
    }
  
    _onStreamProgress(e: any) {
        if (!this.xhr) return;
  
        if (this.xhr.status !== 200) {
            this._onStreamFailure(e);
            return;
        }
  
        if (this.readyState == StreamState.CONNECTING.toString()) {
            this.dispatchEvent(new CustomEvent('open'));
            this._setReadyState(StreamState.OPEN.toString());
        }
  
        var data = this.xhr.responseText.substring(this.progress);
        this.progress += data.length;
        var sourceThis = this;
        data.split(/(\r\n|\r|\n){2}/g).forEach(function(part: any) {
            if (part.trim().length === 0) {
                sourceThis.dispatchEvent(sourceThis._parseEventChunk(sourceThis.chunk.trim()));
                sourceThis.chunk = '';
            }
            else {
                sourceThis.chunk += part;
            }
        }.bind(this));
    }
  
    _onStreamLoaded(e: any) {
        this._onStreamProgress(e);
  
        // Parse the last chunk.
        this.dispatchEvent(this._parseEventChunk(this.chunk));
        this.chunk = '';
    }
  
    _parseEventChunk(chunk: string): any {
        if (!chunk || chunk.length === 0) return null;

        const e = {} as EventStreamStruct;
        e.data = '';
        e.event = 'message';

        chunk.split(/\n|\r\n|\r/).forEach(function(line: any) {
            line = line.trimRight();
            var index = line.indexOf(':');
            if (index <= 0) return;
  
            var field = line.substring(0, index);
            if (!(field in e)) {
                return;
            }
  
            var value = line.substring(index + 1).trimLeft();
            switch(field)
            {
                case 'id':
                    e.id = value;
                    break;
                case 'retry':
                    e.retry = value;
                    break;
                case 'data':
                    e.data += value;
                    break;
                case 'event':
                    e.event = value;
                    break;
            }
        }.bind(this));
  
        var event = (new CustomEvent(e.event) as {}) as EventStreamStruct;
        event.data = e.data;
        event.id = e.id;
        return event;
    }
  
    _checkStreamClosed() {
        if (!this.xhr)return;
  
        if (this.xhr.readyState === XMLHttpRequest.DONE) {
            this._setReadyState(StreamState.CLOSED.toString());
        }
    }
  
    stream() {
        this._setReadyState(StreamState.CONNECTING.toString());
    
        this.xhr = new XMLHttpRequest();
        this.xhr.addEventListener('progress', this._onStreamProgress.bind(this));
        this.xhr.addEventListener('load', this._onStreamLoaded.bind(this));
        this.xhr.addEventListener('readystatechange', this._checkStreamClosed.bind(this));
        this.xhr.addEventListener('error', this._onStreamFailure.bind(this));
        this.xhr.addEventListener('abort', this._onStreamFailure.bind(this));
        this.xhr.open(this.options.method ?? (this.options.payload == undefined ? 'GET' : 'POST'), this.url);
        for (var header in this.options.headers) {
            this.xhr.setRequestHeader(header, this.options.headers[header]);
        }
        this.xhr.withCredentials = this.options.withCredentials;
        this.xhr.send(this.options.payload);
    }
  
    close() {
        if (this.readyState === StreamState.CLOSED.toString()) return;
        this.xhr.abort();
        this.xhr = null;
        this._setReadyState(StreamState.CLOSED.toString());
    }
}