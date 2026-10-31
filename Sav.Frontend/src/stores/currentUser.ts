import { computed, ref } from 'vue';
import { defineStore } from 'pinia';
import { getCurrentUserAsync, getQueueSizeAsync, updateCurrentUserAsync } from '@/api/api';
import type { CurrentUser } from '@/models/API/CurrentUser';
import type { UserProfile } from '@/models/API/UserProfile';

export const useCurrentUserStore = defineStore('currentUser', () => {
    const steamId = ref<string | null>(null);
    const profile = ref<UserProfile | null>(null);
    const queueSize = ref<number | null>(null);
    const loading = ref(false);
    const loaded = ref(false);

    const isLoggedIn = computed(() => steamId.value !== null);

    let pendingLoad: Promise<void> | null = null;

    const applyCurrentUser = (currentUser: CurrentUser) => {
        steamId.value = currentUser.steamId;
        profile.value = currentUser.profile;
        queueSize.value = currentUser.queueSize;
    };

    const fetchCurrentUserAsync = async () => {
        loading.value = true;
        try {
            const response = await getCurrentUserAsync();

            if (response.success && response.data)
                applyCurrentUser(response.data);
        } finally {
            loading.value = false;
        }
    };

    const loadAsync = async () => {
        pendingLoad ??= fetchCurrentUserAsync().finally(() => {
            pendingLoad = null;
        });

        await pendingLoad;
        loaded.value = true;
    };

    const ensureLoadedAsync = async () => {
        if (loaded.value)
            return;

        await loadAsync();
    };

    const refreshQueueSizeAsync = async () => {
        const response = await getQueueSizeAsync();

        queueSize.value = response.success && typeof response.data === 'number' ? response.data : null;
    };

    const setCurrentUserAsync = async (id: string) => {
        const response = await updateCurrentUserAsync(id);

        if (!response.success || !response.data)
            return false;

        applyCurrentUser(response.data);
        loaded.value = true;
        return true;
    };

    return {
        steamId,
        profile,
        queueSize,
        loading,
        loaded,
        isLoggedIn,
        loadAsync,
        ensureLoadedAsync,
        refreshQueueSizeAsync,
        setCurrentUserAsync,
    };
});
