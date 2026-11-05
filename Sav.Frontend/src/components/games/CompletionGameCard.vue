<template>
  <article class="game-card">
    <a
      class="game-cover"
      :href="game.storeLink"
      target="_blank"
      rel="noopener noreferrer"
      tabindex="-1"
      :aria-label="`${game.name} on Steam`">
      <GameCoverImage :sources="[coverUrl, game.gameLogoSmall, game.gameIcon]" :alt="game.name">
        <template #fallback>
          <span class="game-cover-fallback" aria-hidden="true">
            <i class="pi pi-desktop"></i>
          </span>
        </template>
      </GameCoverImage>
    </a>

    <div class="game-info">
      <h3 class="game-title">
        <a :href="game.storeLink" target="_blank" rel="noopener noreferrer">{{ game.name }}</a>
      </h3>

      <dl class="game-stats">
        <div class="stat" v-tooltip.top="'Average percentage of Steam players who unlocked each achievement'">
          <dt>Average unlock rate</dt>
          <dd>{{ formatPercent(game.averageAchievementsPercentage) }}</dd>
        </div>
        <div class="stat" v-tooltip.top="'The achievement unlocked by the fewest Steam players in this game'">
          <dt>Rarest achievement</dt>
          <dd>{{ formatPercent(game.lowestAchievementPercentage) }}</dd>
        </div>
        <div class="stat" v-if="game.hoursOnRecord">
          <dt>Hours played</dt>
          <dd>{{ game.hoursOnRecord }}</dd>
        </div>
      </dl>
    </div>

    <div class="game-actions">
      <Button
        label="Store page"
        icon="pi pi-external-link"
        severity="secondary"
        outlined
        size="small"
        @click="openSteamPage" />
      <Button
        v-if="game.globalStatsLink"
        label="Stats"
        icon="pi pi-chart-bar"
        severity="secondary"
        outlined
        size="small"
        @click="openStatsPage" />
    </div>
  </article>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import Button from 'primevue/button';
import GameCoverImage from '@/components/shared/GameCoverImage.vue';
import type { CompletionGameComposite } from '@/models/API/CompletionGameComposite';

const HEADER_IMAGE_URL = 'https://cdn.cloudflare.steamstatic.com/steam/apps';

const props = defineProps<{
  game: CompletionGameComposite
}>();

const coverUrl = computed(() => `${HEADER_IMAGE_URL}/${props.game.appID}/header.jpg`);

const formatPercent = (value: number | null | undefined) => `${(value ?? 0).toFixed(1)}%`;

const openSteamPage = () => {
  window.open(props.game.storeLink, '_blank', 'noopener');
};

const openStatsPage = () => {
  if (props.game.globalStatsLink)
    window.open(props.game.globalStatsLink, '_blank', 'noopener');
};
</script>

<style scoped lang="scss">
.game-card {
  display: grid;
  grid-template-columns: 200px minmax(0, 1fr) auto;
  align-items: center;
  gap: 16px;
  padding: 12px;
  border: 1px solid #2a475e;
  border-radius: 6px;
  background-color: #1b2838;
  transition: background-color 0.15s ease, border-color 0.15s ease;

  &:hover {
    border-color: #3a5a78;
    background-color: #1e2d40;
  }

  @include media-breakpoint-down(md) {
    grid-template-columns: 1fr;

    .game-cover {
      max-width: 320px;
    }

    .game-actions {
      justify-content: flex-start;
    }
  }
}

.game-cover {
  display: block;
  overflow: hidden;
  aspect-ratio: 460 / 215;
  border-radius: 4px;
  background-color: #16202d;

  img {
    display: block;
    width: 100%;
    height: 100%;
    object-fit: cover;
  }
}

.game-cover-fallback {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 100%;
  height: 100%;
  color: #4a6b85;
  font-size: 28px;
}

.game-info {
  display: flex;
  flex-direction: column;
  gap: 12px;
  min-width: 0;
}

.game-title {
  display: -webkit-box;
  overflow: hidden;
  margin: 0;
  color: #ffffff;
  font-family: 'Arial Black', Arial, sans-serif;
  font-size: 17px;
  font-weight: 700;
  line-height: 1.3;
  -webkit-box-orient: vertical;
  -webkit-line-clamp: 2;

  a {
    color: inherit;
    text-decoration: none;

    &:hover {
      color: #66c0f4;
    }
  }
}

.game-stats {
  display: flex;
  flex-wrap: wrap;
  gap: 10px 32px;
  margin: 0;

  .stat {
    min-width: 0;
  }

  dt {
    color: #8f98a0;
    font-size: 11px;
    letter-spacing: 0.04em;
    text-transform: uppercase;
  }

  dd {
    margin: 3px 0 0;
    color: #c7d5e0;
    font-size: 15px;
    font-weight: 600;
  }
}

.game-actions {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
  justify-content: flex-end;
}
</style>