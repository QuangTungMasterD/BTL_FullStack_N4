import axios from 'axios';

const api = axios.create({
  baseURL: 'http://localhost:5000', // ✅ CHỈ LOCALHOST:5000, BỎ /api
  headers: {
    'Content-Type': 'application/json',
  },
  timeout: 60000,
});

// Request interceptor - gắn token
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('authToken');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Response interceptor
api.interceptors.response.use(
  (response) => response.data,
  (error) => {
    console.error('API Error:', error);
    return Promise.reject(error);
  }
);

export default api;