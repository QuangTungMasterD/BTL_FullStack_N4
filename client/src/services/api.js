import axios from 'axios';

const api = axios.create({
  // baseURL: import.meta.env.VITE_API_URL || 'http://localhost:8080/api',
  baseURL: '/api',
  headers: {
    'Content-Type': 'application/json',
  },
  timeout: 10000,
});

// Request interceptor – gắn token nếu có
// api.interceptors.request.use(
//   (config) => {
//     const token = localStorage.getItem('access_token');
//     if (token) {
//       config.headers.Authorization = `Bearer ${token}`;
//     }
//     return config;
//   },
//   (error) => Promise.reject(error)
// );

// Response interceptor – xử lý cấu trúc ApiResponse chuẩn
api.interceptors.response.use(
  (response) => {
    // Giả sử mọi response đều có dạng { data, success, message, ... }
    const apiResponse = response.data;

    // Nếu không phải dạng ApiResponse (ví dụ file upload, blob) thì trả về nguyên response
    if (apiResponse === undefined || typeof apiResponse.success !== 'boolean') {
      return response;
    }

    // Nếu success === false -> coi như lỗi nghiệp vụ
    if (!apiResponse.success) {
      const error = new Error(apiResponse.message || 'Request failed');
      error.response = response;
      error.data = apiResponse;
      return Promise.reject(error);
    }

    // Thành công: trả về phần data thực tế (để store nhận đúng kiểu)
    return apiResponse.data !== undefined ? apiResponse.data : apiResponse;
  },
  (error) => {
    // Xử lý lỗi HTTP (401, 403, 500...)
    if (error.response?.status === 401) {
      console.error('Unauthorized – token hết hạn hoặc không hợp lệ');
      // Có thể dispatch logout event hoặc chuyển hướng
      // window.location.href = '/login';
    } else if (error.response?.status === 403) {
      console.error('Forbidden – không có quyền truy cập');
    } else if (error.response?.status === 500) {
      console.error('Server error – vui lòng thử lại sau');
    }

    // Ném lỗi để store xử lý
    return Promise.reject(error);
  }
);

export default api;