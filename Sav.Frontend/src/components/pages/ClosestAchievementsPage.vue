<template>
    <div class="close-achievements">
        <div class="games-panel">
            <div class="search-container">
                <span class="pi pi-search search-icon" aria-hidden="true"></span>
                <InputText v-model="searchQuery" placeholder="Search games..." class="search-input"
                    aria-label="Search games" @keydown.esc="searchQuery = ''" />
                <button v-if="searchQuery" type="button" class="search-clear" aria-label="Clear search"
                    @click="searchQuery = ''">
                    <span class="pi pi-times" aria-hidden="true"></span>
                </button>
            </div>

            <div class="game-list-container">
                <ListBox v-model="selectedGame" :options="filteredGames" optionLabel="name" class="game-list"
                 listStyle="height:100%"
                    :virtualScrollerOptions="{ itemSize: 26 }" @change="loadAchievements" >
                    <template #option="slotProps">
                        <div class="game-list-item p-2">
                            <img :src="slotProps.option.gameIcon" alt="Game icon" class="game-icon mr-2" />
                            <span class="game-name" :title="slotProps.option.name">{{ slotProps.option.name }}</span>
                        </div>
                    </template>
                </ListBox>
            </div>
        </div>

        <div class="achievements-panel">
            <div class="achievements-scroll">
                <div class="achievements-grid">
                    <AchievementCard v-for="achievement in closeAchievements" :key="achievement.apiname"
                        :achievement="achievement" />
                </div>
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue';
import InputText from 'primevue/inputtext';
import ListBox from 'primevue/listbox';
import AchievementCard from '@/components/achievements/AchievementCard.vue';
import { GameDto } from '@/models/API/GameDto';
import { getGameClosestAchievementsAsync, getIncompleteGamesAsync } from '@/api/api';
import { AchievementComposite } from '@/models/API/AchievementComposite';
import { useCurrentUserStore } from '@/stores/currentUser';

const currentUser = useCurrentUserStore();

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
    const response = await getIncompleteGamesAsync(currentUser.steamId ?? undefined, 9999999, 0);

    if(!response.success || !response.data)
    return;

    games.value.push(...response.data ?? []);
};

const loadAchievements = async () => {
    if(!selectedGame.value)
        return;

    const response = await getGameClosestAchievementsAsync(currentUser.steamId ?? undefined, selectedGame.value.appID);

    if(!response.success || !response.data)
        return;

    closeAchievements.value = response.data;
};

const reloadForCurrentUserAsync = async () => {
    games.value = [];
    selectedGame.value = null;
    closeAchievements.value = [];
    await fetchGames();
};

onMounted(() => {
    fetchGames();
});

watch(() => currentUser.steamId, reloadForCurrentUserAsync);
</script>

<style scoped lang="scss">
.close-achievements {
    display: flex;
    height: 100%;
    background-color: #1B2838;
    padding: 10px;
    box-sizing: border-box;

    @include media-breakpoint-down(md) {
        flex-direction: column;
        gap: 8px;
        padding: 8px;
    }
}

.games-panel {
    width: 200px;
    flex-shrink: 0;
    background-color: #171A21;
    border-right: 1px solid #2A475E;
    display: flex;
    flex-direction: column;

    @include media-breakpoint-down(md) {
        width: 100%;
        height: 180px;
        border-right: none;
        border-bottom: 1px solid #2A475E;
    }
}

.search-container {
    position: relative;
    display: flex;
    align-items: center;
    flex-shrink: 0;
    padding: 8px;
    background-color: #1B2838;
    border-bottom: 1px solid #2A475E;
}

.search-icon {
    position: absolute;
    left: 17px;
    font-size: 12px;
    color: #8F98A0;
    pointer-events: none;
}

.search-input {
    width: 100%;
    min-width: 0;
    height: 30px;
    padding: 0 26px 0 27px;
    background-color: #171A21;
    color: #C7D5E0;
    border: 1px solid #2A475E;
    border-radius: 4px;
    font-family: Arial, Helvetica, sans-serif;
    font-size: 12px;
    transition: border-color 0.15s ease, box-shadow 0.15s ease;
}

.search-input::placeholder {
    color: #8F98A0;
}

.search-input:enabled:hover {
    border-color: #3D6A8F;
}

.search-input:enabled:focus {
    border-color: #66C0F4;
    box-shadow: 0 0 0 2px rgba(102, 192, 244, 0.25);
}

.search-clear {
    position: absolute;
    right: 15px;
    display: flex;
    align-items: center;
    justify-content: center;
    width: 16px;
    height: 16px;
    padding: 0;
    border: none;
    border-radius: 50%;
    background-color: transparent;
    color: #8F98A0;
    font-size: 10px;
    line-height: 1;
    cursor: pointer;
}

.search-clear:hover {
    background-color: #2A475E;
    color: #FFFFFF;
}

.game-list-container {
    position: relative;
    flex-grow: 1;
    overflow: hidden;
}

:deep(.game-list) {
    background-color: #171A21;
    border: none;
    height: 100%;
}

/* nowrap option labels must not widen the virtual list past the panel */
:deep(.p-virtualscroller-content) {
    max-width: 100%;
}

:deep(.p-listbox-item) {
    padding: 0;
}

.game-list-item {
    display: flex;
    align-items: center;
    gap: 6px;
    padding: 5px;
    min-width: 0;
}

.game-icon {
    width: 16px;
    height: 16px;
    flex-shrink: 0;
    object-fit: cover;
}

.game-name {
    flex: 1;
    min-width: 0;
    color: white;
    font-weight: bold;
    font-size: 11px;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
}

.achievements-panel {
    flex-grow: 1;
    min-height: 0;
    overflow: hidden;
    background-color: #171A21;
}

.achievements-scroll {
    width: 100%;
    height: 100%;
    overflow-y: auto;
}

.achievements-grid {
    display: flex;
    flex-wrap: wrap;
    justify-content: center;
}
</style>