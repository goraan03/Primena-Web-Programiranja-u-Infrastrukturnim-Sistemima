import axiosInstance from './axiosInstance';

const travelApi = {
    getAll: () => axiosInstance.get('/travel').then((r) => r.data),
    getById: (id) => axiosInstance.get(`/travel/${id}`).then((r) => r.data),
    create: (data) => axiosInstance.post('/travel', data).then((r) => r.data),
    update: (id, data) => axiosInstance.put(`/travel/${id}`, data).then((r) => r.data),
    delete: (id) => axiosInstance.delete(`/travel/${id}`),

    getDestinations: (travelId) => axiosInstance.get(`/travel/${travelId}/destinations`).then((r) => r.data),
    addDestination: (travelId, data) => axiosInstance.post(`/travel/${travelId}/destinations`, data).then((r) => r.data),
    updateDestination: (id, data) => axiosInstance.put(`/travel/destinations/${id}`, data).then((r) => r.data),
    deleteDestination: (id) => axiosInstance.delete(`/travel/destinations/${id}`),

    getActivities: (travelId) => axiosInstance.get(`/travel/${travelId}/activities`).then((r) => r.data),
    addActivity: (travelId, data) => axiosInstance.post(`/travel/${travelId}/activities`, data).then((r) => r.data),
    updateActivity: (id, data) => axiosInstance.put(`/travel/activities/${id}`, data).then((r) => r.data),
    deleteActivity: (id) => axiosInstance.delete(`/travel/activities/${id}`),

    getExpenses: (travelId) => axiosInstance.get(`/travel/${travelId}/expenses`).then((r) => r.data),
    addExpense: (travelId, data) => axiosInstance.post(`/travel/${travelId}/expenses`, data).then((r) => r.data),
    updateExpense: (id, data) => axiosInstance.put(`/travel/expenses/${id}`, data).then((r) => r.data),
    deleteExpense: (id) => axiosInstance.delete(`/travel/expenses/${id}`),

    getBudgetSummary: (travelId) => axiosInstance.get(`/travel/${travelId}/budget-summary`).then((r) => r.data),

    getChecklist: (travelId) => axiosInstance.get(`/travel/${travelId}/checklist`).then((r) => r.data),
    addChecklistItem: (travelId, name) => axiosInstance.post(`/travel/${travelId}/checklist`, { name }).then((r) => r.data),
    toggleChecklistItem: (id) => axiosInstance.put(`/travel/checklist/${id}/toggle`).then((r) => r.data),
    deleteChecklistItem: (id) => axiosInstance.delete(`/travel/checklist/${id}`),

    createShareLink: (travelId, accessType) => axiosInstance.post(`/share/${travelId}`, { accessType, expiresInDays: 7 }).then((r) => r.data),
};

export default travelApi;