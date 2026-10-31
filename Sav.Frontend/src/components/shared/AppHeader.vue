<template>
    <header class="app-header">
        <div class="profile">
            <div class="avatar" :class="{ 'has-frame': !!frameUrl }">
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
import { computed, onMounted, onUnmounted } from 'vue';
import { useCurrentUserStore } from '@/stores/currentUser';

const SYNC_POLL_INTERVAL = 5000;

const currentUser = useCurrentUserStore();
let pollHandle: ReturnType<typeof setInterval> | undefined;

const displayName = computed(() => currentUser.profile?.customURL || currentUser.profile?.steamID || currentUser.profile?.steamID64 || '');
const avatarUrl = computed(() => currentUser.profile?.avatarFull || currentUser.profile?.avatarMedium || currentUser.profile?.avatarIcon || '');
const frameUrl = computed(() => currentUser.profile?.avatarFrame ?? '');
const isSyncing = computed(() => (currentUser.queueSize ?? 0) > 0);
const syncLabel = computed(() => {
    if (currentUser.queueSize === null) {
        return '';
    }

    return isSyncing.value
        ? `Syncing ${currentUser.queueSize} game${currentUser.queueSize === 1 ? '' : 's'}`
        : 'Up to date';
});
const syncTitle = computed(() => {
    if (currentUser.queueSize === null) {
        return 'Achievement sync status is unavailable';
    }

    return isSyncing.value
        ? 'Achievements are being updated'
        : 'Achievement update queue is empty';
});

onMounted(() => {
    currentUser.refreshQueueSizeAsync();
    pollHandle = setInterval(() => currentUser.refreshQueueSizeAsync(), SYNC_POLL_INTERVAL);
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
    --avatar-size: 40px;
    /* Steam frames are authored for a 184px avatar inside a 224px canvas. */
    --avatar-frame-scale: 1.2174;
    position: relative;
    flex-shrink: 0;
    width: var(--avatar-size);
    height: var(--avatar-size);
}

.avatar-image,
.avatar-fallback {
    display: block;
    width: 100%;
    height: 100%;
    border-radius: 4px;
    object-fit: cover;
}

.avatar.has-frame .avatar-image {
    border-radius: 0;
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
    top: 50%;
    left: 50%;
    width: calc(var(--avatar-size) * var(--avatar-frame-scale));
    height: calc(var(--avatar-size) * var(--avatar-frame-scale));
    transform: translate(-50%, -50%);
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
