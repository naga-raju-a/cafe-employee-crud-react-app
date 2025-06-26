import axios from 'axios';

const api = axios.create({ baseURL: 'https://localhost:7140/' });

export const getCafes = () => api.get('/cafes');
export const getCafe = (id) => api.get(`/cafes/${id}`);
export const saveCafe = (data) => data.id ? api.put(`/cafes/${data.id}`, data) : api.post('/cafes', data);
export const deleteCafe = (id) => api.delete(`/cafes/${id}`);

export const getEmployees = () => api.get('/employees');
export const getEmployee = (id) => api.get(`/employees/${id}`);
export const saveEmployee = (data) => data.id ? api.put(`/employees/${data.id}`, data) : api.post('/employees', data);
export const deleteEmployee = (id) => api.delete(`/employees/${id}`);
