import { computed, ref } from 'vue';
import { defineStore } from 'pinia';
import { getCurrentUserAsync, getSyncProgressAsync, updateCurrentUserAsync } from '@/api/api';
import type { CurrentUser } from '@/models/API/CurrentUser';
import type { SyncProgress } from '@/models/API/SyncProgress';
import type { UserProfile } from '@/models/API/UserProfile';

export const useCurrentUserStore = defineStore('currentUser', () => {
    const steamId = ref<string | null>(null);
    const profile = ref<UserProfile | null>(null);
    const queueSize = ref<number | null>(null);
    const syncProgress = ref<SyncProgress | null>(null);
    const loading = ref(false);
    const loaded = ref(false);

    const isLoggedIn = computed(() => steamId.value !== null);
    const isSyncing = computed(() => syncProgress.value?.isRunning ?? (queueSize.value ?? 0) > 0);

    // Stop polling a progress endpoint that keeps failing (e.g. an older running backend).
    const SYNC_PROGRESS_MAX_FAILURES = 3;
    const syncProgressFailures = ref(0);

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

    const refreshSyncProgressAsync = async () => {
        if (syncProgressFailures.value >= SYNC_PROGRESS_MAX_FAILURES)
            return;

        const response = await getSyncProgressAsync();

        if (!response.success || !response.data) {
            syncProgressFailures.value++;
            return;
        }

        syncProgressFailures.value = 0;
        syncProgress.value = response.data;
        queueSize.value = response.data.queueSize;
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
        syncProgress,
        loading,
        loaded,
        isLoggedIn,
        isSyncing,
        loadAsync,
        ensureLoadedAsync,
        refreshSyncProgressAsync,
        setCurrentUserAsync,
    };
});
