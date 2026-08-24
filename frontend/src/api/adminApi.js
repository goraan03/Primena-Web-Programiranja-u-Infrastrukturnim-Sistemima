import axiosInstance from './axiosInstance';

const adminApi = {
    getUsers: () => axiosInstance.get('/admin/users').then((r) => r.data),
    deleteUser: (id) => axiosInstance.delete(`/admin/users/${id}`),
    getAllTravelPlans: () => axiosInstance.get('/admin/travel-plans').then((r) => r.data),
};

export default adminApi;