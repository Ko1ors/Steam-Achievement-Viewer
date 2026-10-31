<template>
    <div class="rare-achievements">
        <div class="achievements-scroll">
            <div class="achievements-list">
                <AchievementCard v-for="achievement in achievements" :key="`${achievement.appID}-${achievement.apiname}`"
                    :achievement="achievement" show-game />
            </div>
        </div>

        <div class="load-more-container" v-if="isMoreAchievementAvailable">
            <Button label="Load more" class="load-more" size="large" @click="loadMoreAchievements" />
        </div>
    </div>
</template>

<script setup lang="ts">
import { ref, onMounted, watch } from 'vue';
import Button from 'primevue/button';
import { AchievementComposite } from '@/models/API/AchievementComposite';
import { getRarestAchievementsAsync } from '@/api/api';
import AchievementCard from '@/components/achievements/AchievementCard.vue';
import { useCurrentUserStore } from '@/stores/currentUser';

const currentUser = useCurrentUserStore();

const achievements = ref<AchievementComposite[]>([]);
const currentPage = ref(1);
const pageSize = ref(100);
const isMoreAchievementAvailable = ref(true);

const fetchAchievements = async () => {
    const response = await getRarestAchievementsAsync(currentUser.steamId ?? undefined, currentPage.value, pageSize.value);

    if (!response.success || !response.data)
        return;

    achievements.value.push(...response.data);
    isMoreAchievementAvailable.value = response.data.length === pageSize.value;
};

const loadMoreAchievements = async () => {
    currentPage.value++;
    await fetchAchievements();
};

const reloadForCurrentUserAsync = async () => {
    achievements.value = [];
    currentPage.value = 1;
    isMoreAchievementAvailable.value = true;
    await fetchAchievements();
};

onMounted(() => {
    fetchAchievements();
});

watch(() => currentUser.steamId, reloadForCurrentUserAsync);
</script>

<style scoped>
.rare-achievements {
    display: flex;
    flex-direction: column;
    height: 100%;
    background-color: #171A21;
}

.achievements-scroll {
    flex: 1;
    min-height: 0;
    width: 100%;
    overflow-y: auto;
}

.achievements-list {
    display: flex;
    flex-direction: column;
    align-items: center;
}

.load-more-container {
    display: flex;
    align-items: center;
    justify-content: center;
    flex-shrink: 0;
    height: 60px;
    background-color: #171A21;
}

.load-more {
    width: 200px;
    height: 40px;
    font-size: 16px;
}
</style>
