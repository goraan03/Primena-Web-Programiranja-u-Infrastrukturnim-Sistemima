import axiosInstance from './axiosInstance';

const travelApi = {
    getAll: () => axiosInstance.get('/travel').then((res) => res.data),
    getById: (id) => axiosInstance.get(`/travel/${id}`).then((res) => res.data),
    create: (data) => axiosInstance.post('/travel', data).then((res) => res.data),
    update: (id, data) => axiosInstance.put(`/travel/${id}`, data).then((res) => res.data),
    delete: (id) => axiosInstance.delete(`/travel/${id}`).then((res) => res.data),
};

export default travelApi;