<template>
    <div class="close-all-achievements">
        <ScrollPanel class="achievements-scroll">
            <div class="achievements-list">
                <AchievementCard v-for="achievement in achievements" :key="`${achievement.appID}-${achievement.apiname}`"
                    :achievement="achievement" />
            </div>
        </ScrollPanel>
    </div>
</template>

<script setup lang="ts">
import { ref, onMounted, watch } from 'vue';
import ScrollPanel from 'primevue/scrollpanel';
import { AchievementComposite } from '@/models/API/AchievementComposite';
import { getClosestAchievementsAsync } from '@/api/api';
import AchievementCard from '@/components/achievements/AchievementCard.vue';
import { useCurrentUserStore } from '@/stores/currentUser';

const currentUser = useCurrentUserStore();

const achievements = ref<AchievementComposite[]>([]);
const take = ref(25);
const skip = ref(0);

const fetchCloseAchievements = async () => {

    const response = await getClosestAchievementsAsync(currentUser.steamId ?? undefined, take.value, skip.value);
    if (!response.success || !response.data)
        return;

    achievements.value = response.data;
};

const reloadForCurrentUserAsync = async () => {
    achievements.value = [];
    skip.value = 0;
    await fetchCloseAchievements();
};

onMounted(() => {
    fetchCloseAchievements();
});

watch(() => currentUser.steamId, reloadForCurrentUserAsync);
</script>

<style scoped>
.close-all-achievements {
    height: 100%;
    background-color: #171A21;
}

.achievements-scroll {
    width: 100%;
    height: 100%;
}

.achievements-list {
    display: flex;
    flex-direction: column;
    align-items: center;
}
</style>