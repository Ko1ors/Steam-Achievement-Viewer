<template>
    <div class="home">
        <SyncProgressCard />

        <div class="info">
            <p v-if="loading" class="loading">&gt;Loading profile data...</p>

            <h2 class="profile-name">{{ profileName }}</h2>

            <p class="stat">
                <span class="stat-label">Games: </span>
                <span>{{ gamesCount }}</span>
            </p>
            <p class="stat">
                <span class="stat-label">Total Achievements: </span>
                <span>{{ totalAchievementsCount }}</span>
            </p>
            <p class="stat">
                <span class="stat-label">Unlocked: </span>
                <span>{{ completedAchievementsCount }}</span>
            </p>

            <div class="radial" :style="radialStyle">
                <div class="radial-inner">
                    <span>Achieved</span>
                    <span>{{ achievementPercentage }}%</span>
                </div>
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { useCurrentUserStore } from '@/stores/currentUser';
import SyncProgressCard from '@/components/shared/SyncProgressCard.vue';

const currentUser = useCurrentUserStore();

const profile = computed(() => currentUser.profile);
const loading = computed(() => currentUser.loading);
const profileName = computed(() => profile.value?.steamID ?? '');
const gamesCount = computed(() => profile.value?.gamesCount ?? 0);
const totalAchievementsCount = computed(() => profile.value?.totalAchievementsCount ?? 0);
const completedAchievementsCount = computed(() => profile.value?.completedAchievementsCount ?? 0);
const achievementPercentage = computed(() => {
    if (!totalAchievementsCount.value)
        return 0;

    return Math.floor(completedAchievementsCount.value / totalAchievementsCount.value * 100);
});
const radialStyle = computed(() => ({ '--percentage': `${achievementPercentage.value}` }));
</script>

<style scoped>
.home {
    display: flex;
    flex-direction: column;
    align-items: center;
    height: 100%;
    padding: 10px;
    overflow: auto;
    background-color: #171a21;
    color: #ffffff;
    font-family: 'Arial Black', Arial, sans-serif;
}

.info {
    display: flex;
    flex-direction: column;
    align-items: center;
}

.loading,
.profile-name,
.stat {
    margin: 10px;
    font-size: 20px;
    font-weight: bold;
}

.stat-label {
    margin-right: 4px;
}

.radial {
    --percentage: 0;
    display: flex;
    align-items: center;
    justify-content: center;
    width: 200px;
    height: 200px;
    margin: 20px 20px 0;
    border-radius: 50%;
    background: conic-gradient(#dfdee8 calc(var(--percentage) * 3.6deg), #2a475e 0);
}

.radial-inner {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    width: 180px;
    height: 180px;
    border-radius: 50%;
    background-color: #171a21;
    font-size: 14px;
}
</style>
