type BeforeSendHandler = { (url: string, request: RequestInit): void };
const beforeSendHandlers: BeforeSendHandler[] = [];
import { useHttpStore } from "@/store/http";

export interface RequestOptions {
  headers: object | undefined;
}

export interface PreparedRequest {
  body: string;
  contentType: string;
}

type Headers = {
  [key: string]: string;
};

export type HttpError = {
  statusText: string;
  body: any;
  status: number;
};

const handleResponse = async (response: Response): Promise<any> => {
  const headers: Headers = {};
  response.headers.forEach((value, key) => (headers[key] = value));
  const contentType = headers["content-type"];
  let isJson = false;
  if (contentType && contentType.includes("application/json")) {
    isJson = true;
  }

  if (!response.ok) {
    throw <HttpError>{
      body: isJson ? await response.json() : await response.text(),
      status: response.status,
      statusText: response.statusText,
    };
  }

  const httpStore = useHttpStore();
  httpStore.decreaseNumberOfCurrentHttpCalls();
  return isJson ? response.json() : response.text();
};

const handleError = (error: any): void => {
  const httpStore = useHttpStore();
  httpStore.decreaseNumberOfCurrentHttpCalls();
  throw error;
};

function prepareRequest(input: any): PreparedRequest {
  switch (typeof input) {
    case "string":
      return <PreparedRequest>{
        body: input,
        contentType: "text/plain",
      };
    case "object":
      return <PreparedRequest>{
        body: JSON.stringify(input),
        contentType: "application/json",
      };
    default:
      return <PreparedRequest>{
        body: "",
        contentType: "",
      };
  }
}

const handleBeforeSend = (url: string, request: RequestInit): void => {
  for (const handler of beforeSendHandlers) {
    handler(url, request);
  }
};

export function addBeforeSendHandler(action: BeforeSendHandler): void {
  beforeSendHandlers.push(action);
}

export function get(url: string, options?: RequestOptions): Promise<any> {
  options = options || {
    headers: {},
  };
  const request = <RequestInit>{
    method: "GET",
    headers: options.headers || {},
  };
  handleBeforeSend(url, request);
  const httpSore = useHttpStore();
  httpSore.increaseNumberOfCurrentHttpCalls();
  return fetch(url, request).then(handleResponse).catch(handleError);
}

export function del(url: string, options?: RequestOptions): Promise<any> {
  options = options || {
    headers: {},
  };
  const request = <RequestInit>{
    method: "DELETE",
    headers: options.headers || {},
  };
  handleBeforeSend(url, request);
  const httpSore = useHttpStore();
  httpSore.increaseNumberOfCurrentHttpCalls();
  return fetch(url, request).then(handleResponse).catch(handleError);
}

export function put(
  url: string,
  body: any,
  options?: RequestOptions
): Promise<any> {
  const preparedRequest = prepareRequest(body);
  options = options || {
    headers: {},
  };
  const headers = Object.assign(
    { "content-type": preparedRequest.contentType },
    options.headers || {}
  );
  const request = {
    method: "PUT",
    headers,
    body: preparedRequest.body,
  };
  handleBeforeSend(url, request);
  const httpSore = useHttpStore();
  httpSore.increaseNumberOfCurrentHttpCalls();
  return fetch(url, request).then(handleResponse).catch(handleError);
}

export function post(
  url: string,
  body: any,
  options?: RequestOptions
): Promise<any> {
  const preparedRequest = prepareRequest(body);
  options = options || {
    headers: {},
  };
  const headers = Object.assign(
    { "content-type": preparedRequest.contentType },
    options.headers || {}
  );
  const request = {
    method: "POST",
    headers,
    body: preparedRequest.body,
  };
  handleBeforeSend(url, request);
  const httpSore = useHttpStore();
  httpSore.increaseNumberOfCurrentHttpCalls();
  return fetch(url, request).then(handleResponse).catch(handleError);
}

export function patch(
  url: string,
  body: any,
  options?: RequestOptions
): Promise<any> {
  const preparedRequest = prepareRequest(body);
  options = options || {
    headers: {},
  };
  const headers = Object.assign(
    { "content-type": preparedRequest.contentType },
    options.headers || {}
  );
  const request = {
    method: "PATCH",
    headers,
    body: preparedRequest.body,
  };
  handleBeforeSend(url, request);
  const httpSore = useHttpStore();
  httpSore.increaseNumberOfCurrentHttpCalls();
  return fetch(url, request).then(handleResponse).catch(handleError);
}
