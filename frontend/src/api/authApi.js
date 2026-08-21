import axiosInstance from './axiosInstance';

const authApi = {
    register: (data) => axiosInstance.post('/auth/register', data).then((res) => res.data),
    login: (data) => axiosInstance.post('/auth/login', data).then((res) => res.data),
};

export default authApi;