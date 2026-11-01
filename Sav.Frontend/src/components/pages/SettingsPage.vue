<template>
    <div class="settings">
        <form class="settings-panel" @submit.prevent="saveAsync">
            <h1 class="title">Settings</h1>

            <p v-if="loading" class="status">Loading settings...</p>

            <template v-else>
                <div class="columns">
                    <section class="column">
                        <div class="field">
                            <label class="field-label" for="steam-id">Last Saved Steam ID</label>
                            <InputText
                                id="steam-id"
                                v-model="steamId"
                                class="control"
                                placeholder="76561198000000000"
                                autocomplete="off"
                                :disabled="saving" />
                            <small class="field-hint">17 digits starting with 7656. Leave empty to sign out.</small>
                        </div>

                        <div class="field">
                            <span class="field-label">Auto Update Interval</span>
                            <div class="interval">
                                <InputNumber
                                    v-model="intervalHours"
                                    class="control interval-control"
                                    inputId="interval-hours"
                                    :min="0"
                                    :max="MAX_INTERVAL_HOURS"
                                    :useGrouping="false"
                                    :disabled="saving" />
                                <span class="unit">h</span>
                                <InputNumber
                                    v-model="intervalMinutes"
                                    class="control interval-control"
                                    inputId="interval-minutes"
                                    :min="0"
                                    :max="59"
                                    :useGrouping="false"
                                    :disabled="saving" />
                                <span class="unit">m</span>
                            </div>
                            <small class="field-hint">Intervals below one minute are saved as 1 hour.</small>
                        </div>

                        <div class="field">
                            <label class="field-label" for="update-type">Update Type</label>
                            <Dropdown
                                v-model="updateOnlyRecentGames"
                                class="control"
                                inputId="update-type"
                                :options="updateTypeOptions"
                                optionLabel="label"
                                optionValue="value"
                                :disabled="saving" />
                        </div>
                    </section>

                    <section class="column">
                        <div class="field">
                            <label class="field-label" for="api-key">Steam Web API Key</label>
                            <Password
                                v-model="steamApiKey"
                                class="control"
                                inputId="api-key"
                                :feedback="false"
                                toggleMask
                                :disabled="saving" />
                            <small class="field-hint">
                                Required. Steam no longer serves its public game list feed, so your library is retrieved through the Steam Web API.
                                Get your API key from
                                <a :href="API_KEY_URL" target="_blank" rel="noopener noreferrer">steamcommunity.com/dev/apikey</a>.
                            </small>
                        </div>
                    </section>
                </div>

                <div class="actions">
                    <Button type="submit" label="Save Settings" :loading="saving" :disabled="!canSave" />
                    <p class="status" :class="{ 'is-error': statusError }" role="status" aria-live="polite">{{ status }}</p>
                </div>
            </template>
        </form>

        <section class="sync-panel">
            <div class="sync-head">
                <div>
                    <h2 class="sync-title">Game Sync Status</h2>
                    <p class="sync-summary" role="status" aria-live="polite">{{ syncSummary }}</p>
                </div>
                <Button label="Refresh" icon="pi pi-refresh" class="p-button-text sync-refresh"
                        :loading="syncLoading" :disabled="!currentUser.isLoggedIn" @click="loadSyncStatusAsync(syncPage)" />
            </div>

            <p v-if="!currentUser.isLoggedIn" class="sync-empty">Save a Steam ID to see the sync status of your games.</p>

            <template v-else>
                <div class="sync-filters">
                    <InputText v-model="syncSearch" class="control" placeholder="Filter by game name" aria-label="Filter games by name" />
                    <Dropdown v-model="syncFilter" class="control sync-filter" inputId="sync-status-filter"
                              :options="syncFilterOptions" optionLabel="label" optionValue="value" />
                </div>

                <p v-if="syncLoading && !syncResult" class="sync-empty">Loading sync status...</p>
                <p v-else-if="!syncItems.length" class="sync-empty">{{ syncEmptyMessage }}</p>

                <table v-else class="sync-table">
                    <thead>
                        <tr>
                            <th>Game</th>
                            <th>Status</th>
                            <th>Last synced</th>
                            <th>Details</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="game in syncItems" :key="game.appId">
                            <td class="game-name" :title="game.name">{{ game.name }}</td>
                            <td><span class="sync-badge" :class="`is-${game.status.toLowerCase()}`">{{ syncStatusLabel(game.status) }}</span></td>
                            <td class="sync-date">{{ formatSyncDate(game.lastSyncedAt) }}</td>
                            <td class="sync-error" :title="game.error ?? ''">{{ game.error || '—' }}</td>
                        </tr>
                    </tbody>
                </table>

                <div class="sync-footer">
                    <Button icon="pi pi-chevron-left" class="p-button-text" aria-label="Previous page"
                            :disabled="syncLoading || syncPage <= 1" @click="loadSyncStatusAsync(syncPage - 1)" />
                    <span>Page {{ syncPage }} of {{ syncPageCount }}</span>
                    <Button icon="pi pi-chevron-right" class="p-button-text" aria-label="Next page"
                            :disabled="syncLoading || syncPage >= syncPageCount" @click="loadSyncStatusAsync(syncPage + 1)" />
                </div>
            </template>
        </section>
    </div>
</template>

<script setup lang="ts">
import { computed, onMounted, onUnmounted, ref, watch } from 'vue';
import { useRouter } from 'vue-router';
import Button from 'primevue/button';
import Dropdown from 'primevue/dropdown';
import InputNumber from 'primevue/inputnumber';
import InputText from 'primevue/inputtext';
import Password from 'primevue/password';
import { getGameSyncStatusAsync, getSettingsAsync, updateSettingsAsync } from '@/api/api';
import type { GameSyncStatusPage } from '@/models/API/GameSyncStatus';
import type { UserSettings } from '@/models/API/UserSettings';
import { PageTypes } from '@/helpers/routing';
import { GAMES_ERRORS, PROFILE_ERRORS, syncSteamIdAsync, type SteamSyncFailure, type SteamSyncStage } from '@/helpers/steamSession';
import { useCurrentUserStore } from '@/stores/currentUser';

const API_KEY_URL = 'https://steamcommunity.com/dev/apikey';
const STEAM_ID64_PATTERN = /^7656\d{13}$/;
const MAX_INTERVAL_HOURS = 168;
const RESULT_DISPLAY_DELAY = 1200;
const SYNC_PAGE_SIZE = 25;
const SYNC_POLL_INTERVAL = 3000;
const SYNC_SEARCH_DELAY = 300;

const SYNC_MESSAGES: Record<SteamSyncStage, string> = {
    profile: 'Loading the new profile...',
    games: 'Loading the game list of the new profile...',
    sync: 'Starting the achievement sync...',
};

const router = useRouter();
const currentUser = useCurrentUserStore();

const loading = ref(true);
const saving = ref(false);
const status = ref('');
const statusError = ref(false);

const steamId = ref('');
const steamApiKey = ref('');
const intervalHours = ref<number | null>(0);
const intervalMinutes = ref<number | null>(0);
const updateOnlyRecentGames = ref(false);
const saved = ref<UserSettings | null>(null);

const syncResult = ref<GameSyncStatusPage | null>(null);
const syncLoading = ref(false);
const syncPage = ref(1);
const syncSearch = ref('');
const syncFilter = ref('All');

let syncPollHandle: number | undefined;
let syncSearchHandle: number | undefined;

const updateTypeOptions = [
    { label: 'Full Update', value: false },
    { label: 'Only Recently Played Games', value: true },
];

const syncFilterOptions = [
    { label: 'All statuses', value: 'All' },
    { label: 'Synced', value: 'Synced' },
    { label: 'Syncing', value: 'Syncing' },
    { label: 'Failed', value: 'Failed' },
    { label: 'Never synced', value: 'NotSynced' },
];

const SYNC_STATUS_LABELS: Record<string, string> = {
    Synced: 'Synced',
    Syncing: 'Syncing',
    Failed: 'Failed',
    NotSynced: 'Never synced',
};

const intervalTotalMinutes = computed(() => (intervalHours.value ?? 0) * 60 + (intervalMinutes.value ?? 0));

const setStatus = (message: string, isError = false) => {
    status.value = message;
    statusError.value = isError;
};

const applySettings = (settings: UserSettings) => {
    saved.value = settings;
    steamId.value = settings.steamId ?? '';
    steamApiKey.value = settings.steamApiKey ?? '';
    intervalHours.value = Math.floor(settings.autoUpdateIntervalMinutes / 60);
    intervalMinutes.value = settings.autoUpdateIntervalMinutes % 60;
    updateOnlyRecentGames.value = settings.updateOnlyRecentGames;
};

const hasChanges = computed(() => {
    if (!saved.value)
        return false;

    return steamId.value.trim() !== (saved.value.steamId ?? '')
        || steamApiKey.value.trim() !== (saved.value.steamApiKey ?? '')
        || intervalTotalMinutes.value !== saved.value.autoUpdateIntervalMinutes
        || updateOnlyRecentGames.value !== saved.value.updateOnlyRecentGames;
});

const canSave = computed(() => !loading.value && !saving.value && hasChanges.value);

const syncLibraryCount = computed(() => {
    const result = syncResult.value;
    if (!result)
        return 0;

    return result.syncedCount + result.syncingCount + result.failedCount + result.notSyncedCount;
});

const syncItems = computed(() => syncResult.value?.items ?? []);

const syncEmptyMessage = computed(() => syncLibraryCount.value === 0
    ? 'No games in the library yet.'
    : 'No games match the current filter.');

const syncPageCount = computed(() => {
    const result = syncResult.value;
    if (!result || result.totalCount <= 0)
        return 1;

    return Math.ceil(result.totalCount / SYNC_PAGE_SIZE);
});

const syncSummary = computed(() => {
    const result = syncResult.value;
    if (!result)
        return 'Loading sync status...';

    if (syncLibraryCount.value === 0)
        return 'No games in the library yet.';

    const parts = [`${result.syncedCount.toLocaleString()} of ${syncLibraryCount.value.toLocaleString()} synced`];
    if (result.syncingCount > 0)
        parts.push(`${result.syncingCount.toLocaleString()} syncing`);
    if (result.failedCount > 0)
        parts.push(`${result.failedCount.toLocaleString()} failed`);
    if (result.notSyncedCount > 0)
        parts.push(`${result.notSyncedCount.toLocaleString()} never synced`);

    return parts.join(' · ');
});

const syncStatusLabel = (status: string) => SYNC_STATUS_LABELS[status] ?? status;

const formatSyncDate = (value: string | null) => value
    ? new Date(value).toLocaleString('en-US', { dateStyle: 'medium', timeStyle: 'short' })
    : '—';

const loadSyncStatusAsync = async (page = syncPage.value) => {
    if (!currentUser.isLoggedIn || syncLoading.value)
        return;

    syncLoading.value = true;
    try {
        const response = await getGameSyncStatusAsync(page, SYNC_PAGE_SIZE, syncSearch.value.trim(), syncFilter.value === 'All' ? undefined : syncFilter.value);
        if (!response.success || !response.data)
            return;

        syncResult.value = response.data;
        syncPage.value = response.data.page;
    } finally {
        syncLoading.value = false;
    }
};

watch([syncSearch, syncFilter], () => {
    window.clearTimeout(syncSearchHandle);
    syncSearchHandle = window.setTimeout(() => loadSyncStatusAsync(1), SYNC_SEARCH_DELAY);
});

const describeFailure = (failure: SteamSyncFailure) => {
    if (failure.stage === 'profile' && failure.result)
        return `Steam ID saved. ${PROFILE_ERRORS[failure.result]}`;

    if (failure.stage === 'games' && failure.result)
        return `Steam ID saved. ${GAMES_ERRORS[failure.result]}`;

    if (failure.stage === 'sync')
        return 'Steam ID saved, but the achievement sync could not be started. Please try again.';

    return 'Steam ID saved, but the profile could not be loaded. Check your connection and try again from the Login page.';
};

const saveAsync = async () => {
    if (saving.value)
        return;

    const id = steamId.value.trim();
    if (id && !STEAM_ID64_PATTERN.test(id)) {
        setStatus('Enter a valid Steam ID: 17 digits starting with 7656 (for example, 76561198064082850).', true);
        return;
    }

    const previousSteamId = saved.value?.steamId ?? '';
    saving.value = true;

    try {
        setStatus('Saving settings...');

        const response = await updateSettingsAsync({
            steamId: id || null,
            steamApiKey: steamApiKey.value.trim() || null,
            autoUpdateIntervalMinutes: intervalTotalMinutes.value,
            updateOnlyRecentGames: updateOnlyRecentGames.value,
        });

        if (!response.success || !response.data) {
            setStatus(response.message || 'Could not save the settings. Please try again.', true);
            return;
        }

        applySettings(response.data);

        if ((response.data.steamId ?? '') === previousSteamId) {
            setStatus('Settings saved.');
            return;
        }

        if (!response.data.steamId) {
            await currentUser.loadAsync();
            setStatus('Signed out. Redirecting to the login page...');
            await router.replace(PageTypes.Auth);
            return;
        }

        const failure = await syncSteamIdAsync(response.data.steamId, stage => setStatus(SYNC_MESSAGES[stage]));
        if (failure) {
            setStatus(describeFailure(failure), true);
            return;
        }

        await currentUser.loadAsync();
        await currentUser.refreshSyncProgressAsync();

        // Another client may have changed the saved Steam ID in the meantime.
        if (!currentUser.isLoggedIn) {
            setStatus('Steam ID saved. Reload the page to continue.', true);
            return;
        }

        setStatus('Achievements are syncing for the new Steam ID. Redirecting...');
        await new Promise(resolve => setTimeout(resolve, RESULT_DISPLAY_DELAY));
        await router.replace(PageTypes.Home);
    } finally {
        saving.value = false;
    }
};

onMounted(async () => {
    void loadSyncStatusAsync(1);
    syncPollHandle = window.setInterval(() => {
        if (syncResult.value?.isRunning || currentUser.isSyncing)
            void loadSyncStatusAsync(syncPage.value);
    }, SYNC_POLL_INTERVAL);

    const response = await getSettingsAsync();

    if (!response.success || !response.data) {
        setStatus('Could not load the settings. Reload the page to try again.', true);
        loading.value = false;
        return;
    }

    applySettings(response.data);
    loading.value = false;
});

onUnmounted(() => {
    window.clearInterval(syncPollHandle);
    window.clearTimeout(syncSearchHandle);
});
</script>

<style scoped lang="scss">
.settings {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 16px;
    height: 100%;
    padding: 24px;
    box-sizing: border-box;
    overflow: auto;
    background-color: #171a21;
    color: #c7d5e0;
}

.settings-panel,
.sync-panel {
    width: min(860px, 100%);
    padding: 28px 32px 24px;
    box-sizing: border-box;
    border: 1px solid #2a475e;
    border-radius: 8px;
    background-color: #1b2838;
    box-shadow: 0 12px 40px rgba(0, 0, 0, 0.45);
}

.settings-panel {
    display: flex;
    flex-direction: column;
}

.title {
    margin: 0 0 20px;
    color: #ffffff;
    font-family: 'Arial Black', Arial, sans-serif;
    font-size: 24px;
}

.columns {
    display: grid;
    grid-template-columns: repeat(2, minmax(0, 1fr));
    gap: 12px 32px;

    @include media-breakpoint-down(md) {
        grid-template-columns: 1fr;
    }
}

.column {
    display: flex;
    flex-direction: column;
    gap: 18px;
    min-width: 0;
}

.field {
    display: flex;
    flex-direction: column;
    gap: 6px;
}

.field-label {
    color: #ffffff;
    font-size: 15px;
    font-weight: 600;
}

.field-hint {
    color: #8f98a0;
    font-size: 12px;
    line-height: 1.45;

    a {
        color: #66c0f4;
    }
}

.control {
    width: 100%;
    background-color: #171a21;
    border-color: #2a475e;
    color: #ffffff;
}

.interval {
    display: flex;
    align-items: center;
    gap: 8px;
}

.interval-control {
    width: 90px;
    flex: 0 0 auto;

    :deep(.p-inputnumber-input) {
        width: 100%;
        background-color: #171a21;
        border-color: #2a475e;
        color: #ffffff;
        text-align: right;
    }
}

.unit {
    color: #ffffff;
    font-weight: 700;
}

.actions {
    display: flex;
    align-items: center;
    gap: 16px;
    margin-top: 24px;
}

.status {
    min-height: 20px;
    margin: 0;
    color: #c7d5e0;
    font-size: 14px;
    line-height: 1.4;

    &.is-error {
        color: #ff8080;
    }
}

.sync-head {
    display: flex;
    align-items: flex-start;
    justify-content: space-between;
    gap: 16px;
}

.sync-title {
    margin: 0;
    color: #ffffff;
    font-family: 'Arial Black', Arial, sans-serif;
    font-size: 20px;
}

.sync-summary {
    margin: 6px 0 0;
    color: #8f98a0;
    font-size: 13px;
}

.sync-refresh {
    flex: 0 0 auto;
    color: #66c0f4;
}

.sync-filters {
    display: flex;
    gap: 8px;
    margin: 18px 0 4px;
}

.sync-filter {
    width: 190px;
    flex: 0 0 auto;
}

.sync-empty {
    margin: 16px 0 0;
    color: #8f98a0;
    font-size: 14px;
}

.sync-table {
    width: 100%;
    margin-top: 12px;
    border-collapse: collapse;
    table-layout: fixed;
    font-size: 13px;

    th {
        padding: 8px 10px;
        border-bottom: 1px solid #2a475e;
        color: #8f98a0;
        font-size: 11px;
        font-weight: 600;
        letter-spacing: 0.06em;
        text-align: left;
        text-transform: uppercase;
    }

    td {
        overflow: hidden;
        padding: 8px 10px;
        border-bottom: 1px solid rgba(42, 71, 94, 0.55);
        white-space: nowrap;
        text-overflow: ellipsis;
    }

    tbody tr:last-child td {
        border-bottom: none;
    }
}

.game-name {
    width: 34%;
    color: #ffffff;
}

.sync-date {
    width: 22%;
    color: #8f98a0;
}

.sync-error {
    color: #8f98a0;
}

.sync-badge {
    display: inline-block;
    padding: 2px 8px;
    border-radius: 10px;
    font-size: 11px;
    font-weight: 700;
    letter-spacing: 0.04em;
    text-transform: uppercase;

    &.is-synced {
        background-color: rgba(164, 208, 7, 0.16);
        color: #a4d007;
    }

    &.is-syncing {
        background-color: rgba(102, 192, 244, 0.16);
        color: #66c0f4;
    }

    &.is-failed {
        background-color: rgba(255, 128, 128, 0.14);
        color: #ff8080;
    }

    &.is-notsynced {
        background-color: rgba(143, 152, 160, 0.14);
        color: #8f98a0;
    }
}

.sync-footer {
    display: flex;
    align-items: center;
    justify-content: flex-end;
    gap: 8px;
    margin-top: 12px;
    color: #8f98a0;
    font-size: 12px;
}
</style>
