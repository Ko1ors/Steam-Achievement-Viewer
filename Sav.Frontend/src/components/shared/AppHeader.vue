<template>
    <header class="app-header">
        <div class="profile">
            <div class="avatar">
                <img v-if="avatarUrl" :src="avatarUrl" :alt="displayName" class="avatar-image" />
                <i v-else class="pi pi-user avatar-fallback" aria-hidden="true"></i>
                <img v-if="frameUrl" :src="frameUrl" alt="" class="avatar-frame" />
            </div>
            <span v-if="displayName" class="profile-name">{{ displayName }}</span>
        </div>

        <div class="sync-status" :class="{ 'is-syncing': isSyncing }" :title="syncTitle" role="status" aria-live="polite">
            <i :class="isSyncing ? 'pi pi-spin pi-spinner' : 'pi pi-cloud-upload'" aria-hidden="true"></i>
            <span v-if="syncLabel" class="sync-label">{{ syncLabel }}</span>
        </div>
    </header>
</template>

<script setup lang="ts">
import { computed, onMounted, onUnmounted, ref } from 'vue';
import { getQueueSizeAsync, getSteamUserAsync } from '@/api/api';
import type { SteamUser } from '@/models/API/SteamUser';

const SYNC_POLL_INTERVAL = 5000;

const user = ref<SteamUser | null>(null);
const queueSize = ref<number | null>(null);
let pollHandle: ReturnType<typeof setInterval> | undefined;

const displayName = computed(() => user.value?.customURL || user.value?.steamID || user.value?.steamID64 || '');
const avatarUrl = computed(() => user.value?.avatarFull || user.value?.avatarMedium || user.value?.avatarIcon || '');
const frameUrl = computed(() => user.value?.avatarFrame ?? '');
const isSyncing = computed(() => (queueSize.value ?? 0) > 0);
const syncLabel = computed(() => {
    if (queueSize.value === null) {
        return '';
    }

    return isSyncing.value
        ? `Syncing ${queueSize.value} game${queueSize.value === 1 ? '' : 's'}`
        : 'Up to date';
});
const syncTitle = computed(() => {
    if (queueSize.value === null) {
        return 'Achievement sync status is unavailable';
    }

    return isSyncing.value
        ? 'Achievements are being updated'
        : 'Achievement update queue is empty';
});

const refreshUser = async () => {
    const response = await getSteamUserAsync();

    if (!response.success || !response.data) {
        return;
    }

    user.value = response.data;
};

const refreshQueueSize = async () => {
    const response = await getQueueSizeAsync();

    queueSize.value = response.success && typeof response.data === 'number' ? response.data : null;
};

onMounted(() => {
    refreshUser();
    refreshQueueSize();
    pollHandle = setInterval(refreshQueueSize, SYNC_POLL_INTERVAL);
});

onUnmounted(() => {
    if (pollHandle) {
        clearInterval(pollHandle);
    }
});
</script>

<style scoped lang="scss">
.app-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    flex-shrink: 0;
    gap: 16px;
    height: 64px;
    padding: 0 20px;
    background-color: #1b2838;
    border-bottom: 1px solid #2a475e;
}

.profile {
    display: flex;
    align-items: center;
    gap: 12px;
    min-width: 0;
}

.avatar {
    position: relative;
    flex-shrink: 0;
    width: 40px;
    height: 40px;
}

.avatar-image,
.avatar-fallback {
    display: block;
    width: 100%;
    height: 100%;
    border-radius: 4px;
    object-fit: cover;
}

.avatar-fallback {
    display: flex;
    align-items: center;
    justify-content: center;
    background-color: #2a475e;
    color: #66c0f4;
    font-size: 18px;
}

.avatar-frame {
    position: absolute;
    inset: 0;
    width: 100%;
    height: 100%;
    pointer-events: none;
}

.profile-name {
    overflow: hidden;
    color: #ffffff;
    font-size: 15px;
    font-weight: 600;
    white-space: nowrap;
    text-overflow: ellipsis;
}

.sync-status {
    display: flex;
    align-items: center;
    gap: 8px;
    flex-shrink: 0;
    color: #8f98a0;
    font-size: 13px;

    &.is-syncing {
        color: #66c0f4;
    }
}
</style>
