import { getUser, getRequests, deleteAllRequests, getStubs, addStub, deleteStub, getMetadata } from "./serviceAgent"
import { getItem, setItem } from "./session"

const shouldAuthenticate = callback => {
    let username = getItem("username") || "testUser";
    let password = getItem("password") || "testPassword";
    getUser(username, password)
        .then(response => callback(false))
        .catch(error => {
            if (error.response.status === 401) {
                callback(true)
            }
        })
}

const authenticate = (username, password, successCallback, errorCallback) => {
    getUser(username, password)
        .then(response => {
            setItem("username", username);
            setItem("password", password);
            successCallback(response);
        })
        .catch(error => {
            errorCallback(error);
        })
}

const logicGetRequests = () => {
    let username = getItem("username");
    let password = getItem("password");
    return getRequests(username, password);
}

const logicDeleteAllRequests = () => {
    let username = getItem("username");
    let password = getItem("password");
    return deleteAllRequests(username, password);
}

const logicGetStubs = asYaml => {
    if(!asYaml) {
        asYaml = false;
    }

    let username = getItem("username");
    let password = getItem("password");
    return getStubs(username, password, asYaml);
}

const logicAddStub = stub => {
    let username = getItem("username");
    let password = getItem("password");
    return addStub(stub, username, password);
}

const logicDeleteStub = stubId => {
    let username = getItem("username");
    let password = getItem("password");
    return deleteStub(stubId, username, password);
}

const logicGetMetadata = () => {
    return getMetadata();
}

export {
    shouldAuthenticate,
    logicGetRequests,
    logicDeleteAllRequests,
    logicGetStubs,
    authenticate,
    logicAddStub,
    logicDeleteStub,
    logicGetMetadata
}