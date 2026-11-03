<template>
    <header class="app-header">
        <div class="header-start">
            <button
                v-if="isMobile"
                ref="menuButton"
                type="button"
                class="menu-button"
                aria-label="Open navigation"
                aria-controls="app-sidebar"
                :aria-expanded="isSidebarOpen"
                @click="emit('toggleSidebar')">
                <i class="pi pi-bars" aria-hidden="true"></i>
            </button>
            <div class="profile">
                <div class="avatar" :class="{ 'has-frame': !!frameUrl }">
                    <img v-if="avatarUrl" :src="avatarUrl" :alt="displayName" class="avatar-image" />
                    <i v-else class="pi pi-user avatar-fallback" aria-hidden="true"></i>
                    <img v-if="frameUrl" :src="frameUrl" alt="" class="avatar-frame" />
                </div>
                <span v-if="displayName" class="profile-name">{{ displayName }}</span>
            </div>
        </div>

        <div v-if="currentUser.isLoggedIn && syncLabel" class="sync-status" :class="{ 'is-syncing': isSyncing }" :title="syncTitle" role="status" aria-live="polite">
            <i :class="isSyncing ? 'pi pi-spin pi-spinner' : 'pi pi-cloud-upload'" aria-hidden="true"></i>
            <span class="sync-label">{{ syncLabel }}</span>
        </div>
    </header>
</template>

<script setup lang="ts">
import { computed, onMounted, onUnmounted, ref } from 'vue';
import { useCurrentUserStore } from '@/stores/currentUser';

defineProps<{
    isMobile: boolean;
    isSidebarOpen: boolean;
}>();

const emit = defineEmits<{
    toggleSidebar: [];
}>();

const SYNC_POLL_INTERVAL = 2000;

const currentUser = useCurrentUserStore();
const menuButton = ref<HTMLButtonElement | null>(null);
let pollHandle: ReturnType<typeof setInterval> | undefined;

const displayName = computed(() => currentUser.profile?.customURL || currentUser.profile?.steamID || currentUser.profile?.steamID64 || '');
const avatarUrl = computed(() => currentUser.profile?.avatarFull || currentUser.profile?.avatarMedium || currentUser.profile?.avatarIcon || '');
const frameUrl = computed(() => currentUser.profile?.avatarFrame ?? '');
const isSyncing = computed(() => currentUser.isSyncing);
const formatNumber = (value: number) => value.toLocaleString();
const syncLabel = computed(() => {
    const progress = currentUser.syncProgress;

    if (!progress)
        return '';

    if (!progress.isRunning)
        return 'Up to date';

    if (progress.totalGames <= 0)
        return progress.queueSize > 0
            ? `Syncing ${formatNumber(progress.queueSize)} game${progress.queueSize === 1 ? '' : 's'}`
            : 'Syncing achievements';

    return `Syncing ${formatNumber(progress.processedGames)}/${formatNumber(progress.totalGames)} · ${formatNumber(progress.queueSize)} left`;
});
const syncTitle = computed(() => {
    const progress = currentUser.syncProgress;

    if (!progress) {
        return 'Achievement sync status is unavailable';
    }

    if (!progress.isRunning) {
        return 'Achievement update queue is empty';
    }

    return progress.lastGameName
        ? `Updating ${progress.lastGameName}`
        : 'Achievements are being updated';
});

onMounted(() => {
    currentUser.refreshSyncProgressAsync();
    pollHandle = setInterval(() => currentUser.refreshSyncProgressAsync(), SYNC_POLL_INTERVAL);
});

onUnmounted(() => {
    if (pollHandle) {
        clearInterval(pollHandle);
    }
});

defineExpose({
    focusMenuButton: () => menuButton.value?.focus(),
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

.header-start {
    display: flex;
    align-items: center;
    gap: 12px;
    min-width: 0;
}

.menu-button {
    display: inline-flex;
    align-items: center;
    justify-content: center;
    flex-shrink: 0;
    width: 40px;
    height: 40px;
    padding: 0;
    border: none;
    border-radius: 4px;
    background-color: transparent;
    color: #c6d4df;
    font-size: 18px;
    cursor: pointer;

    &:hover {
        background-color: #2a475e;
        color: #ffffff;
    }

    &:focus-visible {
        outline: 2px solid #66c0f4;
        outline-offset: 2px;
    }
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
    min-width: 0;
    color: #8f98a0;
    font-size: 13px;

    &.is-syncing {
        color: #66c0f4;
    }
}

.sync-label {
    overflow: hidden;
    white-space: nowrap;
    text-overflow: ellipsis;
}

@include media-breakpoint-down(lg) {
    .app-header {
        gap: 10px;
        padding: 0 12px;
    }

    .header-start {
        gap: 8px;
    }
}

@include media-breakpoint-down(sm) {
    .profile-name {
        display: none;
    }
}
</style>
