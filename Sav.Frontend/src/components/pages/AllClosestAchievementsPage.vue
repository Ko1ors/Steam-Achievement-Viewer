<template>
    <div class="close-all-achievements">
        <div class="achievements-scroll">
            <div class="achievements-list">
                <AchievementCard v-for="achievement in achievements" :key="`${achievement.appID}-${achievement.apiname}`"
                    :achievement="achievement" show-game />
            </div>
        </div>

        <div class="load-more-container" v-if="isMoreAvailable">
            <Button label="Load more" class="load-more" size="large" :loading="loading" @click="loadMoreAchievements" />
        </div>
    </div>
</template>

<script setup lang="ts">
import { ref, onMounted, watch } from 'vue';
import Button from 'primevue/button';
import { AchievementComposite } from '@/models/API/AchievementComposite';
import { getClosestAchievementsAsync } from '@/api/api';
import AchievementCard from '@/components/achievements/AchievementCard.vue';
import { useCurrentUserStore } from '@/stores/currentUser';

const currentUser = useCurrentUserStore();

const achievements = ref<AchievementComposite[]>([]);
const take = ref(100);
const skip = ref(0);
const isMoreAvailable = ref(true);
const loading = ref(false);

const fetchCloseAchievements = async () => {
    loading.value = true;

    try {
        const response = await getClosestAchievementsAsync(currentUser.steamId ?? undefined, take.value, skip.value);

        if (!response.success || !response.data)
            return;

        achievements.value.push(...response.data);
        isMoreAvailable.value = response.data.length === take.value;
    } finally {
        loading.value = false;
    }
};

const loadMoreAchievements = async () => {
    skip.value += take.value;
    await fetchCloseAchievements();
};

const reloadForCurrentUserAsync = async () => {
    achievements.value = [];
    skip.value = 0;
    isMoreAvailable.value = true;
    await fetchCloseAchievements();
};

onMounted(() => {
    fetchCloseAchievements();
});

watch(() => currentUser.steamId, reloadForCurrentUserAsync);
</script>

<style scoped>
.close-all-achievements {
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