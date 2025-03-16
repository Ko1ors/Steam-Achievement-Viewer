<template>
    <div class="close-achievements">
        <div class="games-panel">
            <div class="search-container">
                <InputText v-model="searchQuery" placeholder="Search games..." class="w-full search-input" />
            </div>

            <div class="game-list-container">
                <ListBox v-model="selectedGame" :options="filteredGames" optionLabel="name" class="game-list"
                 listStyle="height:100vh"
                    :virtualScrollerOptions="{ itemSize: 30 }" @change="loadAchievements" >
                    <template #option="slotProps">
                        <div class="game-list-item p-2">
                            <img :src="slotProps.option.gameIcon" alt="Game icon" class="game-icon mr-2" />
                            <span class="game-name">{{ slotProps.option.name }}</span>
                        </div>
                    </template>
                </ListBox>
            </div>
        </div>

        <div class="achievements-panel">
            <ScrollPanel class="achievements-scroll">
                <div class="achievements-grid">
                    <AchievementCard v-for="achievement in closeAchievements" :key="achievement.apiname"
                        :achievement="achievement" />
                </div>
            </ScrollPanel>
        </div>
    </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import InputText from 'primevue/inputtext';
import ListBox from 'primevue/listbox';
import ScrollPanel from 'primevue/scrollpanel';
import AchievementCard from '@/components/achievements/AchievementCard.vue';
import { GameDto } from '@/models/API/GameDto';
import { getGameClosestAchievementsAsync, getIncompleteGamesAsync } from '@/api/api';
import { AchievementComposite } from '@/models/API/AchievementComposite';


const games = ref<GameDto[]>([]);
const searchQuery = ref("");
const selectedGame = ref<GameDto | null>(null);
const closeAchievements = ref<AchievementComposite[]>([]);


const filteredGames = computed(() => {
    if(games.value.length === 0)
        return [];

    if (!searchQuery.value) {
        return games.value;
    }

    const query = searchQuery.value.toLowerCase();
    return games.value.filter(game =>
        game.name.toLowerCase().includes(query)
    );
});


const fetchGames = async () => {
    const response = await getIncompleteGamesAsync("76561198064082850", 9999999, 0);

    if(!response.success || !response.data)
    return;

    games.value.push(...response.data ?? []);
};

const loadAchievements = async () => {
    if(!selectedGame.value)
        return;

    const response = await getGameClosestAchievementsAsync("76561198064082850", selectedGame.value.appID);

    if(!response.success || !response.data)
        return;

    closeAchievements.value = response.data;
};

onMounted(() => {
    fetchGames();
});
</script>

<style scoped>
.close-achievements {
    display: flex;
    height: 100%;
    background-color: #1B2838;
    padding: 10px;
    max-height: 100vh;
}

.games-panel {
    width: 160px;
    background-color: black;
    display: flex;
    flex-direction: column;
}

.search-container {
    height: 25px;
    padding-right: 10px;
}

.search-input {
    background-color: #1B2838;
    color: white;
    border-color: white;
    font-family: 'Arial Black', Arial, sans-serif;
    font-size: 14px;
    height: 25px;
}

.game-list-container {
    flex-grow: 1;
    overflow: hidden;
}

:deep(.game-list) {
    background-color: #171A21;
    border: none;
    height: 100%;
}

:deep(.p-listbox-item) {
    padding: 0;
}

.game-list-item {
    display: flex;
    align-items: center;
    padding: 5px;
}

.game-icon {
    width: 16px;
    height: 16px;
    object-fit: cover;
}

.game-name {
    color: white;
    font-weight: bold;
    font-size: 10px;
    max-width: 100px;
    white-space: normal;
    word-wrap: break-word;
}

.achievements-panel {
    flex-grow: 1;
    overflow: hidden;
    background-color: #171A21;
}

.achievements-scroll {
    width: 100%;
    height: 100%;
}

.achievements-grid {
    display: flex;
    flex-wrap: wrap;
    justify-content: center;
}
</style>