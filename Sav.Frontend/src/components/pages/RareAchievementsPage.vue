<template>
    <div class="rare-achievements">
        <ScrollPanel class="achievements-scroll">
            <div class="achievements-list">
                <AchievementCard v-for="achievement in achievements" :key="`${achievement.appID}-${achievement.apiname}`"
                    :achievement="achievement" />
            </div>
        </ScrollPanel>

        <div class="load-more-container flex justify-content-center p-3" v-if="isMoreAchievementAvailable">
            <Button label="Load more" class="w-full max-w-sm" size="large" @click="loadMoreAchievements" />
        </div>
    </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import Button from 'primevue/button';
import ScrollPanel from 'primevue/scrollpanel';
import { AchievementComposite } from '@/models/API/AchievementComposite';
import { getRarestAchievementsAsync } from '@/api/api';
import AchievementCard from '@/components/achievements/AchievementCard.vue';

const achievements = ref<AchievementComposite[]>([]);
const currentPage = ref(1);
const pageSize = ref(100);
const isMoreAchievementAvailable = ref(true);

const fetchAchievements = async () => {
    const response = await getRarestAchievementsAsync("76561198064082850", currentPage.value, pageSize.value);

    if (!response.success || !response.data)
        return;

    achievements.value.push(...response.data);
    isMoreAchievementAvailable.value = response.data.length === pageSize.value;
};

const loadMoreAchievements = async () => {
    currentPage.value++;
    await fetchAchievements();
};

onMounted(() => {
    fetchAchievements();
});
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
    width: 100%;
}

.achievements-list {
    display: flex;
    flex-direction: column;
    align-items: center;
}

.load-more-container {
    height: 60px;
    background-color: #171A21;
}
</style>
