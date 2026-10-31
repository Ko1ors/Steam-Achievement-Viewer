<template>
  <div class="easiest-games">
    <div class="game-list">
      <div class="game-items">
        <CompletionGameCard v-for="item in completionGames" :key="item.appID" :game="item" />
      </div>

      <p v-if="hasLoaded && !completionGames.length" class="empty">
        No games with unfinished achievements yet.
      </p>
    </div>

    <div class="load-more-container" v-if="isMoreGameAvailable && completionGames.length">
      <Button label="Load more" class="load-more" size="large" :loading="loading" @click="loadMoreGames" />
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
const loading = ref(false);
const hasLoaded = ref(false);

// Fetch games from the API
const fetchGamesAsync = async () => {
  loading.value = true;

  try {
    const response = await getEasiestGamesToCompleteAsync(currentUser.steamId ?? undefined, currentPage.value, pageSize.value);

    if (!response.success || !response.data)
      return;

    hasLoaded.value = true;
    completionGames.value.push(...(response.data.items ?? []));
    isMoreGameAvailable.value = currentPage.value * pageSize.value < response.data.totalCount;
  } finally {
    loading.value = false;
  }
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
  hasLoaded.value = false;
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
  background-color: #171a21;
}

.game-list {
  flex: 1;
  overflow: auto;
  padding: 16px;
}

.game-items {
  display: flex;
  flex-direction: column;
  gap: 12px;
  max-width: 1000px;
  margin: 0 auto;
}

.empty {
  max-width: 1000px;
  margin: 48px auto 0;
  color: #8f98a0;
  font-size: 15px;
  text-align: center;
}

.load-more-container {
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
  height: 60px;
  background-color: #171a21;
}

.load-more {
  width: 200px;
  height: 40px;
  font-size: 16px;
}
</style>