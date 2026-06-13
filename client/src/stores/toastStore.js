import { defineStore } from 'pinia';

export const useToastStore = defineStore('toast', {
  state: () => ({
    toasts: [],
  }),
  actions: {
    addToast(message, type = 'info', duration = 4000, details = null) {
      const id = Date.now() + Math.random();
      this.toasts.push({ id, message, type, duration, details });
      setTimeout(() => {
        this.removeToast(id);
      }, duration);
    },
    removeToast(id) {
      this.toasts = this.toasts.filter(t => t.id !== id);
    },
    success(message, duration) {
      this.addToast(message, 'success', duration);
    },
    error(message, duration, details = null) {
      this.addToast(message, 'error', duration, details);
    },
    info(message, duration) {
      this.addToast(message, 'info', duration);
    },
    warning(message, duration) {
      this.addToast(message, 'warning', duration);
    },
  },
});