import { getAuthToken } from '../utils/auth';

const BASE_URL = "http://192.168.39.94/api";

const post = async (url: string, body: any) => {
  const response = await fetch(`${BASE_URL}/${url}`, {
    method: "POST",
    body: JSON.stringify(body),
    headers: {
      "Content-Type": "application/json",
    },
  });
  console.log({response});
  return response;
};

const get = async (url: string) => {
  const response = await fetch(`http://192.168.39.94/api/${url}`, {
    method: "GET",
    headers: {
      "Authorization": `Bearer ${getAuthToken()}`,
    },
  });
  return response;
}

export default {
    post,
    get,
}
