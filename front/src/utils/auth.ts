import jwt_decode from "jwt-decode";

const storage = window.sessionStorage;

export const isAuthed = () : boolean => {
    const token = storage.getItem("token");
    return !!token;
}

export const setAuthToken = (token: string) => {
    storage.setItem("token", token);
}

export const clearAuthToken = () => {
    storage.removeItem("token");
}

export const getAuthToken = () => {
    return storage.getItem("token");
}

export const isAdmin = () => {
    const token = getAuthToken();
    if(!token) return false;
    const decoded_jwt = jwt_decode(token) as any;
    return decoded_jwt.role == "ADMIN"
}

