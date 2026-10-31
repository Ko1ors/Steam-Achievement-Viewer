<template>
    <section v-if="progress?.isRunning" class="sync-card" role="status" aria-live="polite">
        <div class="sync-head">
            <i class="pi pi-spin pi-spinner sync-icon" aria-hidden="true"></i>
            <span class="sync-title">Syncing achievements</span>
            <span class="sync-count">{{ processed }}/{{ total }} · {{ left }} left</span>
        </div>

        <ProgressBar :value="percent" :showValue="false" class="sync-bar" />

        <p class="sync-current">{{ currentGame || 'Preparing the queue...' }}</p>
    </section>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import ProgressBar from 'primevue/progressbar';
import { useCurrentUserStore } from '@/stores/currentUser';

const currentUser = useCurrentUserStore();

const progress = computed(() => currentUser.syncProgress);
const percent = computed(() => progress.value?.progressPercent ?? 0);
const processed = computed(() => (progress.value?.processedGames ?? 0).toLocaleString());
const total = computed(() => (progress.value?.totalGames ?? 0).toLocaleString());
const left = computed(() => (progress.value?.queueSize ?? 0).toLocaleString());
const currentGame = computed(() => progress.value?.lastGameName ?? '');
</script>

<style scoped lang="scss">
.sync-card {
    width: min(520px, 100%);
    margin-bottom: 16px;
    padding: 14px 18px;
    box-sizing: border-box;
    border: 1px solid #2a475e;
    border-radius: 8px;
    background-color: #1b2838;
    font-family: Arial, sans-serif;
}

.sync-head {
    display: flex;
    align-items: center;
    gap: 8px;
    color: #66c0f4;
    font-size: 14px;
}

.sync-icon {
    font-size: 15px;
}

.sync-title {
    font-weight: 600;
}

.sync-count {
    margin-left: auto;
    color: #c7d5e0;
    font-size: 13px;
}

.sync-bar {
    height: 6px;
    margin: 10px 0 8px;
    background-color: #2a475e;

    :deep(.p-progressbar-value) {
        background-color: #66c0f4;
    }
}

.sync-current {
    overflow: hidden;
    margin: 0;
    color: #8f98a0;
    font-size: 12px;
    white-space: nowrap;
    text-overflow: ellipsis;
}
</style>
