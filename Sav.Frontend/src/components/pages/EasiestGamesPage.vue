<template>
  <div class="easiest-games">
    <div class="game-list">
      <div class="flex flex-column w-full">
        <CompletionGameCard v-for="(item, index) in completionGames" :key="index" :game="item" />
      </div>
    </div>

    <div class="load-more-container flex justify-content-center p-3" v-if="isMoreGameAvailable">
      <Button label="Load more" class="w-full max-w-sm" size="large" @click="loadMoreGames" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, watch } from 'vue';
import Button from 'primevue/button';
import CompletionGameCard from '../games/CompletionGameCard.vue';
import { getEasiestGamesToCompleteAsync } from '@/api/api';
import { CompletionGameComposite } from '@/models/API/CompletionGameComposite';
import { useCurrentUserStore } from '@/stores/currentUser';

const currentUser = useCurrentUserStore();

// State
const completionGames = ref<CompletionGameComposite[]>([]);
const isMoreGameAvailable = ref(true);
const currentPage = ref(1);
const pageSize = ref(25);

// Fetch games from the API
const fetchGamesAsync = async () => {
  console.log('Fetching games...');
  const response = await getEasiestGamesToCompleteAsync(currentUser.steamId ?? undefined, currentPage.value, pageSize.value);
  console.log('API response:', response);

  if(!response.success || !response.data) {
    console.error('Failed to fetch games:', response);
    return;
  }
  
  completionGames.value.push(...response.data.items ?? []);
  isMoreGameAvailable.value = pageSize.value * response.data.page < response.data.totalCount;
  console.log('Games loaded:', completionGames.value);
};

// Load more games
const loadMoreGames = async () => {
  currentPage.value++;
  await fetchGamesAsync();
};

const reloadForCurrentUserAsync = async () => {
  completionGames.value = [];
  currentPage.value = 1;
  isMoreGameAvailable.value = true;
  await fetchGamesAsync();
};

// Lifecycle hooks
onMounted(() => {
  fetchGamesAsync();
});

watch(() => currentUser.steamId, reloadForCurrentUserAsync);
</script>

<style scoped lang="scss">
.easiest-games {
  display: flex;
  flex-direction: column;
  height: 100%;
  background-color: #171A21;
}

.game-list {
  flex: 1;
  overflow: auto;
  padding: 1rem;

  .flex {
    gap: 1rem;
  }
}

.load-more-container {
  height: 60px;
  background-color: #171A21;
}
</style>