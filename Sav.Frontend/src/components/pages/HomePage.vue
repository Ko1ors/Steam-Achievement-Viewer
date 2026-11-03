<template>
    <div class="home">
        <div class="home-inner">
            <div v-if="currentUser.isSyncing" class="sync-slot">
                <SyncProgressCard />
            </div>

            <section class="hero" aria-labelledby="profile-name">
                <div class="hero-backdrop" aria-hidden="true">
                    <img v-if="avatarUrl" :src="avatarUrl" alt="" class="hero-backdrop-image" />
                </div>

                <div class="hero-inner">
                    <div class="hero-main">
                        <div class="identity">
                            <div class="avatar" :class="{ 'has-frame': !!frameUrl }">
                                <img v-if="avatarUrl" :src="avatarUrl" :alt="displayName" class="avatar-image" />
                                <span v-else class="avatar-fallback">
                                    <i class="pi pi-user" aria-hidden="true"></i>
                                </span>
                                <img v-if="frameUrl" :src="frameUrl" alt="" class="avatar-frame" />
                            </div>

                            <div class="identity-text">
                                <p class="eyebrow">Steam profile</p>
                                <h1 id="profile-name" class="profile-name">{{ displayName }}</h1>
                                <p v-if="handle" class="profile-handle">@{{ handle }}</p>
                                <p v-if="headline" class="profile-tagline">{{ headline }}</p>

                                <ul class="profile-meta">
                                    <li v-if="onlineLabel" class="meta-item" :class="`is-${onlineState}`">
                                        <span class="status-dot" aria-hidden="true"></span>{{ onlineLabel }}
                                    </li>
                                    <li v-if="realName" class="meta-item">
                                        <i class="pi pi-user" aria-hidden="true"></i>{{ realName }}
                                    </li>
                                    <li v-if="location" class="meta-item">
                                        <i class="pi pi-map-marker" aria-hidden="true"></i>{{ location }}
                                    </li>
                                    <li v-if="memberSinceLabel" class="meta-item">
                                        <i class="pi pi-calendar" aria-hidden="true"></i>Member since {{ memberSinceLabel }}
                                    </li>
                                    <li v-if="lastSyncLabel" class="meta-item">
                                        <i class="pi pi-sync" aria-hidden="true"></i>{{ lastSyncLabel }}
                                    </li>
                                </ul>
                            </div>
                        </div>

                        <div class="hero-actions">
                            <a v-if="steamProfileUrl" :href="steamProfileUrl" target="_blank" rel="noopener noreferrer"
                                class="steam-link">
                                <i class="fa-brands fa-steam" aria-hidden="true"></i>
                                <span>Steam profile</span>
                                <i class="pi pi-external-link" aria-hidden="true"></i>
                            </a>

                            <div v-if="steamIdValue" class="id-chip">
                                <span class="id-label">SteamID64</span>
                                <code class="id-value">{{ steamIdValue }}</code>
                                <button type="button" class="copy-button" :title="copied ? 'Copied' : 'Copy Steam ID'"
                                    :aria-label="copied ? 'Steam ID copied' : 'Copy Steam ID'" @click="copySteamIdAsync">
                                    <i :class="copied ? 'pi pi-check' : 'pi pi-copy'" aria-hidden="true"></i>
                                </button>
                            </div>
                        </div>
                    </div>

                    <div class="hero-progress">
                        <div class="ring" :style="ringStyle" role="img"
                            :aria-label="`${completionRate}% of all achievements unlocked`">
                            <div class="ring-inner">
                                <span class="ring-value">{{ completionRate }}%</span>
                                <span class="ring-label">Achieved</span>
                            </div>
                        </div>

                        <p class="ring-caption">
                            <strong>{{ formatNumber(completedAchievementsCount) }}</strong>
                            <span>of {{ formatNumber(totalAchievementsCount) }} unlocked</span>
                        </p>
                    </div>
                </div>
            </section>

            <section class="stats" aria-label="Library at a glance">
                <article v-for="stat in stats" :key="stat.label" class="stat-card" :class="stat.tone">
                    <i :class="stat.icon" class="stat-icon" aria-hidden="true"></i>
                    <p class="stat-value">{{ stat.value }}</p>
                    <p class="stat-label">{{ stat.label }}</p>
                    <p class="stat-note">{{ stat.note }}</p>
                </article>
            </section>

            <div class="panels">
                <section class="panel panel--wide">
                    <header class="panel-head">
                        <div>
                            <h2 class="panel-title">
                                <i class="pi pi-bolt" aria-hidden="true"></i>Recently unlocked
                            </h2>
                            <p class="panel-subtitle">Your latest achievement unlocks</p>
                        </div>
                        <RouterLink :to="PageTypes.LastAchieved" class="panel-link">View all</RouterLink>
                    </header>

                    <div v-if="highlightsLoading && !recentAchievements.length" class="tile-grid" aria-hidden="true">
                        <div v-for="n in RECENT_ACHIEVEMENT_LIMIT" :key="n" class="achievement-tile skeleton"></div>
                    </div>

                    <div v-else-if="recentAchievements.length" class="tile-grid">
                        <article v-for="achievement in recentAchievements"
                            :key="`${achievement.appID}-${achievement.apiname}`" class="achievement-tile">
                            <div class="achievement-icon-wrap">
                                <img :src="achievement.iconClosed" alt="" class="achievement-icon" loading="lazy"
                                    @error="hideBrokenImage" />
                                <span class="unlock-badge" aria-hidden="true">
                                    <i class="pi pi-check"></i>
                                </span>
                            </div>
                            <div class="tile-body">
                                <p class="tile-name">{{ achievement.name }}</p>
                                <p class="tile-game">
                                    <img v-if="achievement.gameIcon" :src="achievement.gameIcon" alt=""
                                        class="tile-game-icon" />
                                    <span>{{ achievement.gameName }}</span>
                                </p>
                                <p class="tile-meta">
                                    <span>{{ formatRelative(achievement.unlockTime) }}</span>
                                    <span aria-hidden="true">·</span>
                                    <span :title="'Global unlock rate'">{{ formatPercent(achievement.percent) }}</span>
                                </p>
                            </div>
                        </article>
                    </div>

                    <p v-else-if="highlightsLoading" class="panel-empty">Loading achievements…</p>
                    <p v-else class="panel-empty">No achievements unlocked yet.</p>
                </section>

                <section class="panel">
                    <header class="panel-head">
                        <div>
                            <h2 class="panel-title">
                                <i class="pi pi-compass" aria-hidden="true"></i>Closest to unlock
                            </h2>
                            <p class="panel-subtitle">Most commonly unlocked achievements still missing</p>
                        </div>
                        <RouterLink :to="PageTypes.AllClosestAchievements" class="panel-link">View all</RouterLink>
                    </header>

                    <ul v-if="closestAchievements.length" class="achievement-list">
                        <li v-for="achievement in closestAchievements"
                            :key="`${achievement.appID}-${achievement.apiname}`" class="achievement-row">
                            <img :src="achievement.iconClosed" alt="" class="row-icon" loading="lazy"
                                @error="hideBrokenImage" />
                            <div class="row-body">
                                <p class="row-name">{{ achievement.name }}</p>
                                <p class="row-game">{{ achievement.gameName }}</p>
                            </div>
                            <div class="row-rate" title="Global Steam unlock rate">
                                <span class="row-percent">{{ formatPercent(achievement.percent) }}</span>
                            </div>
                        </li>
                    </ul>

                    <p v-else-if="highlightsLoading" class="panel-empty">Loading achievements…</p>
                    <p v-else class="panel-empty">Nothing left to unlock.</p>
                </section>

                <section class="panel panel--wide">
                    <header class="panel-head">
                        <div>
                            <h2 class="panel-title">
                                <i class="pi pi-verified" aria-hidden="true"></i>Recently perfected
                            </h2>
                            <p class="panel-subtitle">Games where you have earned every achievement</p>
                        </div>
                        <RouterLink :to="PageTypes.CompletedGames" class="panel-link">View all</RouterLink>
                    </header>

                    <div v-if="highlightsLoading && !completedGames.length" class="completed-grid" aria-hidden="true">
                        <div v-for="n in COMPLETED_GAME_LIMIT" :key="n" class="completed-card skeleton"></div>
                    </div>

                    <div v-else-if="completedGames.length" class="completed-grid">
                        <RouterLink v-for="game in completedGames" :key="`${game.game.appID}-${game.completedAt}`"
                            :to="PageTypes.CompletedGames" class="completed-card">
                            <div class="completed-cover">
                                <img v-if="completedCoverUrl(game) && !isCoverFailed(game)" :src="completedCoverUrl(game)"
                                    :alt="game.game.name" loading="lazy" @error="markCoverFailed(game)" />
                                <span v-else class="completed-fallback">
                                    <i class="pi pi-desktop" aria-hidden="true"></i>
                                </span>
                                <span class="completed-badge">
                                    <i class="pi pi-check" aria-hidden="true"></i>100%
                                </span>
                            </div>
                            <div class="completed-body">
                                <p class="completed-name">{{ game.game.name }}</p>
                                <p class="completed-meta">{{ formatDate(game.completedAt) }}</p>
                            </div>
                        </RouterLink>
                    </div>

                    <p v-else-if="highlightsLoading" class="panel-empty">Loading perfected games…</p>
                    <p v-else class="panel-empty">No perfected games yet.</p>
                </section>

                <section class="panel">
                    <header class="panel-head">
                        <div>
                            <h2 class="panel-title">
                                <i class="pi pi-trophy" aria-hidden="true"></i>Rarest locked
                            </h2>
                            <p class="panel-subtitle">Lowest global unlock rates you have not earned</p>
                        </div>
                        <RouterLink :to="PageTypes.RarestAchievements" class="panel-link">View all</RouterLink>
                    </header>

                    <ul v-if="rarestAchievements.length" class="achievement-list">
                        <li v-for="achievement in rarestAchievements"
                            :key="`${achievement.appID}-${achievement.apiname}`" class="achievement-row">
                            <img :src="achievement.iconClosed" alt="" class="row-icon" loading="lazy"
                                @error="hideBrokenImage" />
                            <div class="row-body">
                                <p class="row-name">{{ achievement.name }}</p>
                                <p class="row-game">{{ achievement.gameName }}</p>
                            </div>
                            <div class="row-rate" title="Global Steam unlock rate">
                                <span class="row-percent is-rare">{{ formatPercent(achievement.percent) }}</span>
                            </div>
                        </li>
                    </ul>

                    <p v-else-if="highlightsLoading" class="panel-empty">Loading achievements…</p>
                    <p v-else class="panel-empty">Every achievement you can reach is unlocked.</p>
                </section>
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
import { computed, onMounted, onUnmounted, ref, watch } from 'vue';
import { useCurrentUserStore } from '@/stores/currentUser';
import SyncProgressCard from '@/components/shared/SyncProgressCard.vue';
import {
    getClosestAchievementsAsync,
    getCompletedGamesAsync,
    getLatestAchievementsAsync,
    getRarestAchievementsAsync,
} from '@/api/api';
import type { AchievementComposite } from '@/models/API/AchievementComposite';
import type { CompletedGameComposite } from '@/models/API/CompletedGameComposite';
import { PageTypes } from '@/helpers/routing';

const RECENT_ACHIEVEMENT_LIMIT = 6;
const CLOSEST_ACHIEVEMENT_LIMIT = 5;
const RAREST_ACHIEVEMENT_LIMIT = 5;
const COMPLETED_GAME_LIMIT = 4;

const currentUser = useCurrentUserStore();

const profile = computed(() => currentUser.profile);
const avatarUrl = computed(() => profile.value?.avatarFull || profile.value?.avatarMedium || profile.value?.avatarIcon || '');
const frameUrl = computed(() => profile.value?.avatarFrame ?? '');
const displayName = computed(() => profile.value?.steamID || profile.value?.customURL || 'Steam player');
const steamIdValue = computed(() => profile.value?.steamID64 ?? '');
const realName = computed(() => profile.value?.realName ?? '');
const location = computed(() => profile.value?.location ?? '');
const headline = computed(() => profile.value?.headline ?? '');
const onlineState = computed(() => (profile.value?.onlineState ?? '').toLowerCase());

const handle = computed(() => {
    const customUrl = profile.value?.customURL;
    if (!customUrl)
        return '';

    return customUrl.toLowerCase() === displayName.value.toLowerCase() ? '' : customUrl;
});

const steamProfileUrl = computed(() => {
    if (profile.value?.customURL)
        return `https://steamcommunity.com/id/${encodeURIComponent(profile.value.customURL)}`;

    if (profile.value?.steamID64)
        return `https://steamcommunity.com/profiles/${profile.value.steamID64}`;

    return '';
});

const onlineLabel = computed(() => {
    switch (onlineState.value) {
        case 'online':
            return 'Online';
        case 'offline':
            return 'Offline';
        case 'in-game':
            return 'In game';
        default:
            return onlineState.value ? onlineState.value.charAt(0).toUpperCase() + onlineState.value.slice(1) : '';
    }
});

const gamesCount = computed(() => profile.value?.gamesCount ?? 0);
const totalAchievementsCount = computed(() => profile.value?.totalAchievementsCount ?? 0);
const completedAchievementsCount = computed(() => profile.value?.completedAchievementsCount ?? 0);
const remainingAchievementsCount = computed(() => Math.max(0, totalAchievementsCount.value - completedAchievementsCount.value));
const completionRate = computed(() => {
    if (!totalAchievementsCount.value)
        return 0;

    return Math.round(completedAchievementsCount.value / totalAchievementsCount.value * 1000) / 10;
});
const ringStyle = computed(() => ({ '--percentage': `${completionRate.value}%` }));

const numberFormatter = new Intl.NumberFormat();
const formatNumber = (value: number) => numberFormatter.format(value);

const formatPercent = (value: number | null | undefined) => {
    if (value === null || value === undefined || value < 0)
        return '—';

    return `${value.toFixed(1)}%`;
};

const formatDate = (value: string | null | undefined) => {
    if (!value)
        return '';

    const date = new Date(value);
    return Number.isNaN(date.getTime())
        ? ''
        : date.toLocaleDateString(undefined, { day: 'numeric', month: 'short', year: 'numeric' });
};

const formatMemberSince = (value: string | null | undefined) => {
    if (!value)
        return '';

    const date = new Date(value);
    return Number.isNaN(date.getTime())
        ? ''
        : date.toLocaleDateString(undefined, { month: 'long', year: 'numeric' });
};

type RelativeUnit = 'year' | 'month' | 'week' | 'day' | 'hour' | 'minute';
const RELATIVE_UNITS: Array<[RelativeUnit, number]> = [
    ['year', 365 * 24 * 60 * 60],
    ['month', 30 * 24 * 60 * 60],
    ['week', 7 * 24 * 60 * 60],
    ['day', 24 * 60 * 60],
    ['hour', 60 * 60],
    ['minute', 60],
];
const relativeFormatter = new Intl.RelativeTimeFormat(undefined, { numeric: 'auto' });

const formatRelative = (value: string | null | undefined) => {
    if (!value)
        return '';

    const date = new Date(value);
    if (Number.isNaN(date.getTime()))
        return '';

    const deltaSeconds = Math.round((date.getTime() - Date.now()) / 1000);
    const absoluteSeconds = Math.abs(deltaSeconds);

    for (const [unit, secondsInUnit] of RELATIVE_UNITS) {
        if (absoluteSeconds >= secondsInUnit)
            return relativeFormatter.format(Math.round(deltaSeconds / secondsInUnit), unit);
    }

    return relativeFormatter.format(0, 'minute');
};

const memberSinceLabel = computed(() => formatMemberSince(profile.value?.memberSince));
const lastSyncLabel = computed(() => {
    if (currentUser.isSyncing)
        return 'Syncing now';

    return currentUser.syncProgress?.finishedAt
        ? `Synced ${formatRelative(currentUser.syncProgress.finishedAt)}`
        : '';
});

const recentAchievements = ref<AchievementComposite[]>([]);
const closestAchievements = ref<AchievementComposite[]>([]);
const rarestAchievements = ref<AchievementComposite[]>([]);
const completedGames = ref<CompletedGameComposite[]>([]);
const completedGamesTotal = ref<number | null>(null);
const highlightsLoading = ref(true);
const highlightsLoaded = ref(false);
let loadToken = 0;

const completedCoverUrl = (game: CompletedGameComposite) =>
    game.logoUrl || game.game.gameLogoSmall || game.game.gameIcon || '';

const failedCovers = ref(new Set<string>());
const isCoverFailed = (game: CompletedGameComposite) => failedCovers.value.has(game.game.appID);
const markCoverFailed = (game: CompletedGameComposite) => {
    failedCovers.value = new Set(failedCovers.value).add(game.game.appID);
};

const hideBrokenImage = (event: Event) => {
    const image = event.target as HTMLImageElement | null;
    if (image)
        image.style.visibility = 'hidden';
};

const loadHighlightsAsync = async () => {
    const steamId = currentUser.steamId;
    if (!steamId) {
        highlightsLoading.value = false;
        return;
    }

    const token = ++loadToken;
    highlightsLoading.value = true;

    try {
        const [latest, closest, rarest, completed] = await Promise.all([
            getLatestAchievementsAsync(steamId, 1, RECENT_ACHIEVEMENT_LIMIT, true),
            getClosestAchievementsAsync(steamId, CLOSEST_ACHIEVEMENT_LIMIT, 0, true),
            getRarestAchievementsAsync(steamId, 1, RAREST_ACHIEVEMENT_LIMIT, true),
            getCompletedGamesAsync(steamId, COMPLETED_GAME_LIMIT, 0, true),
        ]);

        if (token !== loadToken)
            return;

        if (latest.success && latest.data)
            recentAchievements.value = latest.data;

        if (closest.success && closest.data)
            closestAchievements.value = closest.data;

        if (rarest.success && rarest.data)
            rarestAchievements.value = rarest.data;

        if (completed.success && completed.data) {
            completedGames.value = completed.data.items ?? [];
            completedGamesTotal.value = completed.data.totalCount ?? 0;
        }

        highlightsLoaded.value = true;
    } finally {
        if (token === loadToken)
            highlightsLoading.value = false;
    }
};

const reloadAsync = async () => {
    recentAchievements.value = [];
    closestAchievements.value = [];
    rarestAchievements.value = [];
    completedGames.value = [];
    completedGamesTotal.value = null;
    failedCovers.value = new Set();
    highlightsLoaded.value = false;
    await loadHighlightsAsync();
};

const stats = computed(() => [
    {
        label: 'Games in library',
        value: formatNumber(gamesCount.value),
        note: 'owned on Steam',
        icon: 'pi pi-th-large',
        tone: 'is-blue',
    },
    {
        label: 'Perfect games',
        value: completedGamesTotal.value === null ? '—' : formatNumber(completedGamesTotal.value),
        note: completedGamesTotal.value === null
            ? (highlightsLoaded.value ? 'unavailable' : 'loading…')
            : 'every achievement earned',
        icon: 'pi pi-verified',
        tone: 'is-green',
    },
    {
        label: 'Achievements unlocked',
        value: formatNumber(completedAchievementsCount.value),
        note: `${completionRate.value.toFixed(1)}% complete`,
        icon: 'pi pi-trophy',
        tone: 'is-gold',
    },
    {
        label: 'Still locked',
        value: formatNumber(remainingAchievementsCount.value),
        note: `${formatNumber(totalAchievementsCount.value)} total`,
        icon: 'pi pi-lock',
        tone: 'is-slate',
    },
]);

const copied = ref(false);
let copyTimer: number | undefined;

const copySteamIdAsync = async () => {
    if (!steamIdValue.value)
        return;

    try {
        await navigator.clipboard.writeText(steamIdValue.value);
        copied.value = true;

        if (copyTimer)
            window.clearTimeout(copyTimer);

        copyTimer = window.setTimeout(() => {
            copied.value = false;
        }, 1600);
    } catch {
        copied.value = false;
    }
};

onMounted(() => {
    loadHighlightsAsync();
});

onUnmounted(() => {
    if (copyTimer)
        window.clearTimeout(copyTimer);
});

watch(() => currentUser.steamId, () => {
    reloadAsync();
});

watch(() => currentUser.isSyncing, (isSyncing, wasSyncing) => {
    if (wasSyncing && !isSyncing)
        loadHighlightsAsync();
});
</script>

<style scoped lang="scss">
.home {
    --canvas: #171a21;
    --surface: #1b2838;
    --rule: #2a475e;
    --text: #eef4f8;
    --text-soft: #c7d5e0;
    --text-muted: #8f98a0;
    --accent: #66c0f4;
    --gold: #e0b45c;
    --green: #78c98a;

    height: 100%;
    overflow-y: auto;
    background:
        radial-gradient(circle at 12% -10%, rgba(102, 192, 244, 0.12), transparent 38%),
        radial-gradient(circle at 92% 0%, rgba(42, 71, 94, 0.55), transparent 42%),
        var(--canvas);
    color: var(--text);
    font-family: 'Segoe UI', Arial, sans-serif;
}

.home-inner {
    display: flex;
    flex-direction: column;
    gap: 16px;
    width: min(1200px, 100%);
    margin: 0 auto;
    padding: 24px 24px 32px;
    box-sizing: border-box;
}

.sync-slot {
    display: flex;
    justify-content: center;

    :deep(.sync-card) {
        width: min(680px, 100%);
        margin-bottom: 0;
    }
}

.hero {
    position: relative;
    overflow: hidden;
    border: 1px solid var(--rule);
    border-radius: 12px;
    background: linear-gradient(135deg, #1d3347 0%, #1b2838 58%, #171f2b 100%);
    box-shadow: 0 20px 48px rgba(0, 0, 0, 0.32);
}

.hero-backdrop {
    position: absolute;
    inset: 0;
    overflow: hidden;
    pointer-events: none;

    &::after {
        content: '';
        position: absolute;
        inset: 0;
        background:
            linear-gradient(90deg, rgba(23, 26, 33, 0.97) 0%, rgba(23, 26, 33, 0.86) 46%, rgba(23, 26, 33, 0.62) 100%),
            radial-gradient(circle at 82% 18%, rgba(102, 192, 244, 0.28), transparent 52%);
    }
}

.hero-backdrop-image {
    position: absolute;
    inset: -12%;
    width: 124%;
    height: 124%;
    object-fit: cover;
    filter: blur(40px) saturate(1.2);
    opacity: 0.5;
}

.hero-inner {
    position: relative;
    display: grid;
    grid-template-columns: minmax(0, 1fr) auto;
    align-items: center;
    gap: 28px;
    padding: 26px 28px;
}

.hero-main {
    display: flex;
    flex-direction: column;
    gap: 20px;
    min-width: 0;
}

.identity {
    display: flex;
    align-items: center;
    gap: 20px;
    min-width: 0;
}

.avatar {
    --avatar-size: 96px;
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
    border-radius: 8px;
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
    color: var(--accent);
    font-size: 34px;
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

.identity-text {
    min-width: 0;
}

.eyebrow {
    margin: 0 0 6px;
    color: var(--accent);
    font-size: 10.5px;
    font-weight: 700;
    letter-spacing: 0.16em;
    text-transform: uppercase;
}

.profile-name {
    overflow: hidden;
    margin: 0;
    color: #ffffff;
    font-family: 'Arial Black', Arial, sans-serif;
    font-size: clamp(26px, 2.6vw, 34px);
    letter-spacing: -0.02em;
    line-height: 1.1;
    text-overflow: ellipsis;
    white-space: nowrap;
}

.profile-handle {
    margin: 4px 0 0;
    color: var(--accent);
    font-size: 13px;
}

.profile-tagline {
    max-width: 620px;
    margin: 8px 0 0;
    color: var(--text-soft);
    font-size: 13.5px;
    line-height: 1.5;
}

.profile-meta {
    display: flex;
    flex-wrap: wrap;
    gap: 6px 16px;
    margin: 14px 0 0;
    padding: 0;
    color: var(--text-muted);
    font-size: 12px;
    list-style: none;
}

.meta-item {
    display: flex;
    align-items: center;
    gap: 6px;
    min-width: 0;

    .pi {
        color: #6d8698;
        font-size: 12px;
    }
}

.status-dot {
    width: 7px;
    height: 7px;
    flex-shrink: 0;
    border-radius: 50%;
    background-color: #6d8698;
}

.meta-item.is-online .status-dot {
    background-color: var(--green);
    box-shadow: 0 0 0 3px rgba(120, 201, 138, 0.16);
}

.meta-item.is-in-game .status-dot {
    background-color: var(--accent);
    box-shadow: 0 0 0 3px rgba(102, 192, 244, 0.16);
}

.hero-actions {
    display: flex;
    flex-wrap: wrap;
    align-items: center;
    gap: 10px;
}

.steam-link {
    display: inline-flex;
    align-items: center;
    gap: 8px;
    padding: 8px 13px;
    border: 1px solid var(--rule);
    border-radius: 6px;
    background-color: rgba(23, 26, 33, 0.45);
    color: var(--text-soft);
    font-size: 12.5px;
    text-decoration: none;
    transition: border-color 0.15s ease, background-color 0.15s ease, color 0.15s ease;

    .fa-steam {
        font-size: 15px;
    }

    .pi-external-link {
        font-size: 10px;
    }

    &:hover {
        border-color: var(--accent);
        background-color: rgba(102, 192, 244, 0.1);
        color: #ffffff;
    }
}

.id-chip {
    display: flex;
    align-items: center;
    gap: 9px;
    padding: 5px 6px 5px 12px;
    border: 1px solid rgba(42, 71, 94, 0.85);
    border-radius: 6px;
    background-color: rgba(23, 26, 33, 0.35);
}

.id-label {
    color: var(--text-muted);
    font-size: 9.5px;
    font-weight: 700;
    letter-spacing: 0.1em;
    text-transform: uppercase;
}

.id-value {
    color: var(--text-soft);
    font-family: Consolas, 'Courier New', monospace;
    font-size: 12px;
    font-variant-numeric: tabular-nums;
}

.copy-button {
    display: grid;
    place-items: center;
    width: 24px;
    height: 24px;
    padding: 0;
    border: 1px solid transparent;
    border-radius: 4px;
    background-color: transparent;
    color: var(--text-muted);
    font-size: 11px;
    cursor: pointer;

    &:hover {
        border-color: var(--rule);
        background-color: rgba(102, 192, 244, 0.12);
        color: #ffffff;
    }
}

.hero-progress {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 10px;
    padding-left: 28px;
    border-left: 1px solid rgba(102, 192, 244, 0.16);
}

.ring {
    --percentage: 0%;
    position: relative;
    display: grid;
    place-items: center;
    width: 148px;
    height: 148px;
    border-radius: 50%;
    background: conic-gradient(var(--accent) var(--percentage), rgba(102, 192, 244, 0.14) 0);
    box-shadow: 0 0 0 1px rgba(42, 71, 94, 0.9);

    &::before {
        content: '';
        position: absolute;
        inset: 7px;
        border-radius: 50%;
        background-color: #17212d;
    }
}

.ring-inner {
    position: relative;
    z-index: 1;
    display: flex;
    flex-direction: column;
    align-items: center;
}

.ring-value {
    color: #ffffff;
    font-family: 'Arial Black', Arial, sans-serif;
    font-size: 26px;
    font-variant-numeric: tabular-nums;
    letter-spacing: -0.03em;
}

.ring-label {
    margin-top: 2px;
    color: var(--text-muted);
    font-size: 9.5px;
    font-weight: 700;
    letter-spacing: 0.14em;
    text-transform: uppercase;
}

.ring-caption {
    margin: 0;
    color: var(--text-muted);
    font-size: 11.5px;
    text-align: center;

    strong {
        display: block;
        color: var(--text);
        font-size: 14px;
        font-variant-numeric: tabular-nums;
    }
}

.stats {
    display: grid;
    grid-template-columns: repeat(4, minmax(0, 1fr));
    gap: 12px;
}

.stat-card {
    position: relative;
    overflow: hidden;
    padding: 15px 16px;
    border: 1px solid var(--rule);
    border-radius: 10px;
    background: linear-gradient(180deg, rgba(30, 45, 63, 0.7), rgba(27, 40, 56, 0.92));

    &::before {
        content: '';
        position: absolute;
        top: 0;
        left: 0;
        width: 100%;
        height: 2px;
        background-color: var(--accent);
        opacity: 0.7;
    }

    &.is-green::before { background-color: var(--green); }
    &.is-gold::before { background-color: var(--gold); }
    &.is-slate::before { background-color: #7e93a3; }
}

.stat-icon {
    margin-bottom: 9px;
    color: var(--accent);
    font-size: 15px;
}

.stat-card.is-green .stat-icon { color: var(--green); }
.stat-card.is-gold .stat-icon { color: var(--gold); }
.stat-card.is-slate .stat-icon { color: #8fa7ba; }

.stat-value {
    margin: 0;
    color: #ffffff;
    font-family: 'Arial Black', Arial, sans-serif;
    font-size: 24px;
    font-variant-numeric: tabular-nums;
    letter-spacing: -0.02em;
    line-height: 1.1;
}

.stat-label {
    margin: 4px 0 0;
    color: var(--text-soft);
    font-size: 12px;
    font-weight: 600;
}

.stat-note {
    margin: 6px 0 0;
    color: var(--text-muted);
    font-size: 11px;
}

.panels {
    display: grid;
    grid-template-columns: minmax(0, 1.55fr) minmax(0, 1fr);
    align-items: start;
    gap: 16px;
}

.panel {
    min-width: 0;
    padding: 17px 18px;
    border: 1px solid var(--rule);
    border-radius: 10px;
    background-color: rgba(27, 40, 56, 0.72);
}

.panel-head {
    display: flex;
    align-items: flex-start;
    justify-content: space-between;
    gap: 12px;
    margin-bottom: 13px;
}

.panel-title {
    display: flex;
    align-items: center;
    gap: 8px;
    margin: 0;
    color: #ffffff;
    font-size: 14.5px;
    font-weight: 650;

    .pi {
        color: var(--accent);
        font-size: 13px;
    }
}

.panel-subtitle {
    margin: 4px 0 0;
    color: var(--text-muted);
    font-size: 11px;
    line-height: 1.35;
}

.panel-link {
    flex-shrink: 0;
    color: var(--accent);
    font-size: 11.5px;
    text-decoration: none;

    &:hover {
        text-decoration: underline;
    }
}

.tile-grid {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(205px, 1fr));
    gap: 8px;
}

.achievement-tile {
    display: flex;
    align-items: center;
    gap: 10px;
    min-width: 0;
    padding: 9px 10px;
    border: 1px solid rgba(42, 71, 94, 0.65);
    border-radius: 8px;
    background-color: rgba(23, 26, 33, 0.42);
    transition: border-color 0.15s ease, background-color 0.15s ease;

    &:hover {
        border-color: rgba(102, 192, 244, 0.5);
        background-color: rgba(102, 192, 244, 0.07);
    }
}

.achievement-icon-wrap {
    position: relative;
    flex-shrink: 0;
    width: 44px;
    height: 44px;
}

.achievement-icon {
    display: block;
    width: 100%;
    height: 100%;
    border-radius: 4px;
    background-color: #0f151c;
    object-fit: contain;
}

.unlock-badge {
    position: absolute;
    right: -3px;
    bottom: -3px;
    display: grid;
    place-items: center;
    width: 15px;
    height: 15px;
    border-radius: 50%;
    background-color: var(--green);
    color: #102016;
    font-size: 8px;
    box-shadow: 0 0 0 2px #17212d;
}

.tile-body {
    min-width: 0;
}

.tile-name {
    display: -webkit-box;
    overflow: hidden;
    margin: 0;
    color: #ffffff;
    font-size: 12.5px;
    font-weight: 600;
    line-height: 1.25;
    -webkit-box-orient: vertical;
    -webkit-line-clamp: 2;
}

.tile-game {
    display: flex;
    align-items: center;
    gap: 5px;
    overflow: hidden;
    margin: 3px 0 0;
    color: var(--text-muted);
    font-size: 10.5px;
    white-space: nowrap;
    text-overflow: ellipsis;

    span {
        overflow: hidden;
        text-overflow: ellipsis;
    }
}

.tile-game-icon {
    width: 12px;
    height: 12px;
    flex-shrink: 0;
    object-fit: contain;
}

.tile-meta {
    display: flex;
    gap: 5px;
    margin: 4px 0 0;
    color: #6f8494;
    font-size: 10px;
    font-variant-numeric: tabular-nums;
}

.achievement-list {
    display: flex;
    flex-direction: column;
    gap: 1px;
    margin: 0;
    padding: 0;
    list-style: none;
}

.achievement-row {
    display: flex;
    align-items: center;
    gap: 10px;
    padding: 7px 6px;
    border-radius: 6px;

    &:hover {
        background-color: rgba(102, 192, 244, 0.06);
    }
}

.row-icon {
    width: 32px;
    height: 32px;
    flex-shrink: 0;
    border-radius: 3px;
    background-color: #0f151c;
    object-fit: contain;
}

.row-body {
    flex: 1;
    min-width: 0;
}

.row-name {
    overflow: hidden;
    margin: 0;
    color: #dfe9f0;
    font-size: 12px;
    font-weight: 600;
    white-space: nowrap;
    text-overflow: ellipsis;
}

.row-game {
    overflow: hidden;
    margin: 2px 0 0;
    color: var(--text-muted);
    font-size: 10.5px;
    white-space: nowrap;
    text-overflow: ellipsis;
}

.row-rate {
    flex-shrink: 0;
    text-align: right;
}

.row-percent {
    color: var(--accent);
    font-size: 12.5px;
    font-weight: 700;
    font-variant-numeric: tabular-nums;

    &.is-rare {
        color: var(--gold);
    }
}

.completed-grid {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(190px, 1fr));
    gap: 10px;
}

.completed-card {
    display: flex;
    flex-direction: column;
    overflow: hidden;
    border: 1px solid rgba(42, 71, 94, 0.7);
    border-radius: 8px;
    background-color: rgba(23, 26, 33, 0.42);
    color: inherit;
    text-decoration: none;
    transition: border-color 0.15s ease, background-color 0.15s ease;

    &:hover {
        border-color: rgba(102, 192, 244, 0.5);
        background-color: rgba(102, 192, 244, 0.06);
    }
}

.completed-cover {
    position: relative;
    aspect-ratio: 460 / 215;
    background-color: #0f151c;

    img {
        display: block;
        width: 100%;
        height: 100%;
        object-fit: cover;
    }
}

.completed-fallback {
    display: grid;
    place-items: center;
    width: 100%;
    height: 100%;
    color: #4a6b85;
    font-size: 24px;
}

.completed-badge {
    position: absolute;
    right: 7px;
    bottom: 7px;
    display: inline-flex;
    align-items: center;
    gap: 4px;
    padding: 2px 6px;
    border-radius: 4px;
    background-color: rgba(15, 21, 28, 0.86);
    color: var(--green);
    font-size: 10px;
    font-weight: 700;
    font-variant-numeric: tabular-nums;
}

.completed-body {
    padding: 8px 10px 10px;
}

.completed-name {
    overflow: hidden;
    margin: 0;
    color: #ffffff;
    font-size: 12px;
    font-weight: 650;
    white-space: nowrap;
    text-overflow: ellipsis;
}

.completed-meta {
    margin: 3px 0 0;
    color: var(--text-muted);
    font-size: 10.5px;
}

.panel-empty {
    margin: 6px 0 0;
    padding: 18px 10px;
    border: 1px dashed rgba(42, 71, 94, 0.65);
    border-radius: 8px;
    color: #6f8494;
    font-size: 12px;
    text-align: center;
}

.skeleton {
    min-height: 62px;
    border-color: rgba(42, 71, 94, 0.4);
    background: linear-gradient(100deg, rgba(42, 71, 94, 0.18), rgba(42, 71, 94, 0.32), rgba(42, 71, 94, 0.18));
}

.completed-card.skeleton {
    min-height: 150px;
}

@include media-breakpoint-down(lg) {
    .stats {
        grid-template-columns: repeat(2, minmax(0, 1fr));
    }

    .panels {
        grid-template-columns: 1fr;
    }
}

@include media-breakpoint-down(md) {
    .hero-inner {
        grid-template-columns: 1fr;
        gap: 20px;
    }

    .hero-progress {
        flex-direction: row;
        justify-content: space-between;
        padding-top: 18px;
        padding-left: 0;
        border-top: 1px solid rgba(102, 192, 244, 0.16);
        border-left: 0;
    }

    .ring {
        width: 112px;
        height: 112px;
    }

    .ring-value {
        font-size: 20px;
    }

    .ring-caption {
        text-align: right;
    }
}

@include media-breakpoint-down(sm) {
    .home-inner {
        gap: 12px;
        padding: 14px 14px 24px;
    }

    .hero-inner {
        padding: 18px;
    }

    .identity {
        align-items: flex-start;
    }

    .avatar {
        --avatar-size: 72px;
    }

    .profile-name {
        font-size: 24px;
    }

    .stats {
        grid-template-columns: 1fr;
    }

    .hero-actions {
        align-items: flex-start;
        flex-direction: column;
    }

    .id-chip {
        max-width: 100%;
    }
}
</style>
