<template>
    <div class="auth">
        <form class="auth-panel" @submit.prevent="signInAsync">
            <h1 class="title">Enter your Steam ID</h1>

            <p class="hint">
                Your SteamID64 is a 17-digit number starting with 7656.
                <a :href="HELP_URL" target="_blank" rel="noopener noreferrer">Where do I find it?</a>
            </p>

            <div class="controls">
                <InputText
                    v-model="steamId"
                    class="steam-id"
                    placeholder="76561198000000000"
                    aria-label="Steam ID"
                    autocomplete="off"
                    autofocus
                    :disabled="busy" />
                <Button type="submit" label="Enter" :loading="busy" :disabled="!canSubmit" />
            </div>

            <p class="status" :class="{ 'is-error': statusError }" role="status" aria-live="polite">{{ status }}</p>

            <p v-if="queuedGames > 0" class="queue">Games in queue: {{ queuedGames }}</p>
        </form>
    </div>
</template>

<script setup lang="ts">
import { computed, ref, watch } from 'vue';
import { useRouter } from 'vue-router';
import Button from 'primevue/button';
import InputText from 'primevue/inputtext';
import { queueAchievementsUpdateAsync, updateGamesAsync, updateProfileAsync } from '@/api/api';
import type { GamesUpdateResult } from '@/models/API/GamesUpdateResult';
import type { ProfileUpdateResult } from '@/models/API/ProfileUpdateResult';
import { PageTypes } from '@/helpers/routing';
import { useCurrentUserStore } from '@/stores/currentUser';

const HELP_URL = 'https://github.com/Ko1ors/Steam-Achievement-Viewer/blob/master/README.md#login';
const STEAM_ID64_PATTERN = /^7656\d{13}$/;
const SUCCESS_REDIRECT_DELAY = 1200;

const PROFILE_ERRORS: Record<ProfileUpdateResult, string> = {
    Success: '',
    NotFound: 'Steam ID not found. Check the number and try again.',
    Private: 'Your Steam profile is private. Set your profile and game details to public, then try again.',
    Failed: 'Failed to get profile information. Check that the Steam ID is correct and the profile is public.',
};

const GAMES_ERRORS: Record<GamesUpdateResult, string> = {
    Success: '',
    MissingApiKey: 'A Steam Web API key is required to read your game list. Add your Steam Web API key in Settings and try again.',
    Failed: 'Failed to get the game list. Make sure your profile and game details are public, then try again.',
};

const router = useRouter();
const currentUser = useCurrentUserStore();

const steamId = ref(currentUser.steamId ?? '');
const busy = ref(false);
const status = ref('');
const statusError = ref(false);
const queuedGames = ref(0);

watch(() => currentUser.steamId, value => {
    steamId.value = value ?? '';
});

const canSubmit = computed(() => !busy.value && steamId.value.trim().length > 0);

const setStatus = (message: string, isError = false) => {
    status.value = message;
    statusError.value = isError;
};

const signInAsync = async () => {
    if (busy.value)
        return;

    const id = steamId.value.trim();
    if (!STEAM_ID64_PATTERN.test(id)) {
        setStatus('Enter a valid Steam ID: 17 digits starting with 7656 (for example, 76561198064082850).', true);
        return;
    }

    busy.value = true;
    queuedGames.value = 0;

    try {
        setStatus('Retrieving profile data...');
        const profileResponse = await updateProfileAsync(id);

        if (!profileResponse.success) {
            setStatus('Could not reach the server. Please try again.', true);
            return;
        }

        if (profileResponse.data !== 'Success') {
            setStatus(PROFILE_ERRORS[profileResponse.data ?? 'Failed'] || PROFILE_ERRORS.Failed, true);
            return;
        }

        setStatus('Retrieving the game list...');
        const gamesResponse = await updateGamesAsync(id);

        if (!gamesResponse.success) {
            setStatus('Could not reach the server. Please try again.', true);
            return;
        }

        if (gamesResponse.data !== 'Success') {
            setStatus(GAMES_ERRORS[gamesResponse.data ?? 'Failed'] || GAMES_ERRORS.Failed, true);
            return;
        }

        setStatus('Starting the achievement sync...');
        const queueResponse = await queueAchievementsUpdateAsync();

        if (!queueResponse.success) {
            setStatus('Failed to start the achievement sync. Please try again.', true);
            return;
        }

        await currentUser.loadAsync();
        await currentUser.refreshSyncProgressAsync();
        queuedGames.value = currentUser.queueSize ?? 0;

        setStatus(queuedGames.value > 0
            ? `Achievements are syncing for ${queuedGames.value} game${queuedGames.value === 1 ? '' : 's'}. Redirecting...`
            : 'Achievements are syncing. Redirecting...');

        await new Promise(resolve => setTimeout(resolve, SUCCESS_REDIRECT_DELAY));
        await router.replace(PageTypes.Home);
    } finally {
        busy.value = false;
    }
};
</script>

<style scoped lang="scss">
.auth {
    display: flex;
    align-items: center;
    justify-content: center;
    height: 100%;
    padding: 24px;
    box-sizing: border-box;
    overflow: auto;
    background-color: #171a21;
    color: #c7d5e0;
}

.auth-panel {
    display: flex;
    flex-direction: column;
    width: min(520px, 100%);
    padding: 32px;
    box-sizing: border-box;
    border: 1px solid #2a475e;
    border-radius: 8px;
    background-color: #1b2838;
    box-shadow: 0 12px 40px rgba(0, 0, 0, 0.45);
}

.title {
    margin: 0 0 8px;
    color: #ffffff;
    font-family: 'Arial Black', Arial, sans-serif;
    font-size: 24px;
    text-align: center;
}

.hint {
    margin: 0 0 20px;
    color: #8f98a0;
    font-size: 13px;
    line-height: 1.5;
    text-align: center;

    a {
        color: #66c0f4;
    }
}

.controls {
    display: flex;
    align-items: stretch;
    gap: 8px;
}

.steam-id {
    flex: 1;
    background-color: #171a21;
    border-color: #2a475e;
    color: #ffffff;
    font-family: 'Arial Black', Arial, sans-serif;
}

.status {
    min-height: 20px;
    margin: 16px 0 0;
    color: #c7d5e0;
    font-size: 14px;
    line-height: 1.4;
    text-align: center;

    &.is-error {
        color: #ff8080;
    }
}

.queue {
    margin: 8px 0 0;
    color: #8f98a0;
    font-size: 13px;
    text-align: right;
}
</style>
