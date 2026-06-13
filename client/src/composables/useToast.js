import { useToastStore } from '@/stores/toastStore';

export function useToast() {
  const toastStore = useToastStore();
  return {
    success: (msg, duration) => toastStore.success(msg, duration),
    error: (msg, duration, details) => toastStore.error(msg, duration, details),
    info: (msg, duration) => toastStore.info(msg, duration),
    warning: (msg, duration) => toastStore.warning(msg, duration),
  };
}