import api from './api';

const scheduleChangeRequestService = {
  create(data) {
    return api.post('/v1/schedule-change-requests', data);
  },
  getMyRequests() {
    return api.get('/v1/schedule-change-requests/my-requests');
  },
  getPendingRequests() {
    return api.get('/v1/schedule-change-requests/pending');
  },
  processRequest(data) {
    return api.post('/v1/schedule-change-requests/process', data);
  }
};

export default scheduleChangeRequestService;