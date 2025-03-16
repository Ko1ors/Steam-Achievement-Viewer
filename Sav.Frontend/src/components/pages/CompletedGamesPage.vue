<template>
  <div class="completed-games">
    <div class="game-list">

      <div class="flex flex-column w-full">
        <GameCard v-for="(item, index) in completedGames" :key="index" :game="item" />
      </div>
    </div>

    <div class="load-more-container flex justify-content-center p-3" v-if="isMoreGameAvailable">
      <Button label="Load more" class="w-full max-w-sm" size="large" @click="loadMoreGames" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import Button from 'primevue/button';
import GameCard from '../games/CompletedGameCard.vue';
import { getCompletedGamesAsync } from '@/api/api';
import { CompletedGameComposite } from '@/models/API/CompletedGameComposite';

// State
const completedGames = ref<CompletedGameComposite[]>([]);
const isMoreGameAvailable = ref(true);
const skip = ref(0);
const take = ref(25);

// Fetch completed games from the API
const fetchCompletedGamesAsync = async () => {
  const response = await getCompletedGamesAsync("76561198064082850", take.value, skip.value);

  if(!response.success || !response.data)
    return;
  
  completedGames.value.push(...response.data.items ?? []);
  isMoreGameAvailable.value = response.data.totalCount > skip.value + take.value;
};

// Load more games
const loadMoreGames = async () => {
  skip.value += take.value;
  await fetchCompletedGamesAsync();
};

// Lifecycle hooks
onMounted(() => {
  fetchCompletedGamesAsync();
});
</script>

<style scoped lang="scss">
.completed-games {
  display: flex;
  flex-direction: column;
  height: 100%;
  background-color: #171A21;
}

.game-list {
  flex: 1;
  overflow: auto;
}

.load-more-container {
  height: 60px;
  background-color: #171A21;
}

:deep(.p-virtualscroller) {
  background-color: #171A21;
  border: none;
}

:deep(.p-button) {
  font-size: 16px;
  height: 40px;
}
</style>