import type { ToastMessageOptions } from 'primevue/toast';

type ToastListener = (options: ToastMessageOptions) => void;

const listeners = new Set<ToastListener>();

export const subscribeToToasts = (listener: ToastListener): (() => void) => {
    listeners.add(listener);
    return () => {
        listeners.delete(listener);
    };
};

export const notify = (options: ToastMessageOptions) => {
    listeners.forEach(listener => listener(options));
};

export const notifyError = (summary: string, detail?: string) => {
    notify({ severity: 'error', summary, detail, life: 6000 });
};
