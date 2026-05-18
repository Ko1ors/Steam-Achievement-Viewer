<template>
  <div class="game-card p-3">
    <div class="flex gap-3">
      <img :src="'https://cdn.cloudflare.steamstatic.com/steam/apps/' + game.appID + '/header.jpg'" :alt="game.name" class="game-image" />
      <div class="flex flex-column justify-content-between flex-1">
        <div>
          <h3 class="game-title mb-2">{{ game.name }}</h3>
          <div class="achievement-stats mb-2">
            <div class="stat">
              <span>Average completion: {{ game.averageAchievementsPercentage.toFixed(1) }}%</span>
            </div>
            <div class="stat">
              <span>Lowest achievement: {{ game.lowestAchievementPercentage.toFixed(1) }}%</span>
            </div>
            <div class="stat" v-if="game.hoursOnRecord">
              <span>Hours played: {{ game.hoursOnRecord }}</span>
            </div>
          </div>
        </div>
        <div class="flex align-items-center gap-2">
          <Button 
            icon="pi pi-external-link" 
            class="p-button-sm" 
            @click="openSteamPage"
            tooltip="Open in Steam"
            tooltipOptions="{ position: 'top' }"
          />
          <Button 
            icon="pi pi-chart-line" 
            class="p-button-sm" 
            @click="openStatsPage"
            v-if="game.globalStatsLink"
            tooltip="View Stats"
            tooltipOptions="{ position: 'top' }"
          />
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { CompletionGameComposite } from '@/models/API/CompletionGameComposite';
import Button from 'primevue/button';

const props = defineProps<{
  game: CompletionGameComposite
}>();

const openSteamPage = () => {
  window.open(props.game.storeLink, '_blank');
};

const openStatsPage = () => {
  if (props.game.globalStatsLink) {
    window.open(props.game.globalStatsLink, '_blank');
  }
};
</script>

<style scoped lang="scss">
.game-card {
  background-color: #1b2838;
  border-radius: 4px;
  transition: transform 0.2s ease;

  &:hover {
    transform: translateY(-2px);
  }

  .game-image {
    width: 200px;
    height: 100px;
    object-fit: cover;
    border-radius: 4px;
  }

  .game-title {
    color: #ffffff;
    margin: 0;
    font-size: 1.2rem;
  }

  .achievement-stats {
    color: #66c0f4;
    font-size: 0.9rem;

    .stat {
      margin-bottom: 0.5rem;
    }
  }
}
</style>