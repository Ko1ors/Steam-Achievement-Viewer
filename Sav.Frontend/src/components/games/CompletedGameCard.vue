<template>
    <div class="game-card">
      <Card class="w-full">
        <template #content>
          <div class="game-container">
            <div class="game-image">
              <img
                v-if="!logoFailed"
                :src="logoSource"
                alt="Game logo"
                class="w-full h-full object-contain"
                @error="handleLogoError"
              />
              <div v-else class="game-logo-fallback">{{ game.game.name }}</div>
            </div>
            
            <div class="game-details">
              <h2 class="game-title">{{ game.game.name }}</h2>
              
              <div class="stats-grid">
                <div class="stat-item">
                  <div class="stat-label">TOTAL PLAYED</div>
                  <div class="stat-value">{{ game.hoursPlayed }} hours</div>
                </div>
                
                <div class="stat-item">
                  <div class="stat-label">COMPLETED AT</div>
                  <div class="stat-value">{{ formatDate(game.completedAt) }}</div>
                </div>
                
                <div class="stat-item">
                  <div class="stat-label">ACHIEVEMENTS {{ game.achievements.length }}/{{ game.achievements.length }}</div>
                  <ProgressBar :value="100" class="achievement-progress" />
                </div>
              </div>
              
              <div class="achievements-preview">
                <div 
                  v-for="(achievement, index) in previewAchievements" 
                  :key="index" 
                  class="achievement-icon"
                >
                  <img :src="achievement.iconClosed" alt="Achievement" />
                </div>
                
                <div v-if="game.previewCount > 0" class="more-achievements">
                  <span>+{{ game.previewCount }}</span>
                </div>
              </div>
            </div>
          </div>
        </template>
      </Card>
    </div>
  </template>
  
  <script setup lang="ts">
  import { computed, ref, watch } from 'vue';
  import Card from 'primevue/card';
  import ProgressBar from 'primevue/progressbar';
  
  // Props
  const props = defineProps({
    game: {
      type: Object,
      required: true
    }
  });
  
  // Computed properties
  const previewAchievements = computed(() => {
    return props.game.previewAchievements || [];
  });

  // Newer Steam apps only publish their header artwork under hashed store_item_assets paths,
  // so the constructed ".../steam/apps/{appId}/header.jpg" URL can 404. Walk through the
  // store assets we already have until one loads.
  const logoCandidates = computed<string[]>(() =>
    [props.game.logoUrl, props.game.game?.gameLogoSmall, props.game.game?.gameIcon]
      .filter((src): src is string => typeof src === 'string' && src.trim().length > 0)
      .map(src => src.trim())
  );

  const logoIndex = ref(0);
  const logoFailed = ref(false);
  const logoSource = computed(() => logoCandidates.value[logoIndex.value] ?? '');

  const handleLogoError = () => {
    const failedSource = logoSource.value;
    let next = logoIndex.value + 1;

    while (next < logoCandidates.value.length && logoCandidates.value[next] === failedSource)
      next++;

    if (next < logoCandidates.value.length)
      logoIndex.value = next;
    else
      logoFailed.value = true;
  };

  // Cards are keyed by position in the list, so a different game can reuse this component.
  watch(() => props.game, () => {
    logoIndex.value = 0;
    logoFailed.value = false;
  });
  

  const formatDate = (dateString: string) => {
    if (!dateString) return '';
    
    const date = new Date(dateString);
    const options = { day: 'numeric', month: 'long', year: 'numeric' } as const;
    return date.toLocaleDateString('en-US', options);
  };
  </script>
  
  <style scoped lang="scss">
  .game-card {
    margin: 5px;
  }
  
  :deep(.p-card) {
    background-color: #16202D;
    border-radius: 0;
    box-shadow: none;
    padding: 0;
  }
  
  :deep(.p-card-content) {
    padding: 0;
  }
  
  .game-container {
    display: grid;
    grid-template-columns: 259px 1fr;
    height: 140px;
    max-width: 936px;
    padding: 10px;

    @include media-breakpoint-down(md) {
      grid-template-columns: minmax(0, 1fr);
      height: auto;
      gap: 10px;
    }
  }

  .game-image {
    height: 100%;
    display: flex;
    align-items: center;
    justify-content: center;

    @include media-breakpoint-down(md) {
      height: auto;
      justify-content: flex-start;

      img {
        width: 100%;
        max-width: 320px;
        height: auto;
      }
    }
  }
  
  .game-logo-fallback {
    width: 100%;
    height: 100%;
    display: flex;
    align-items: center;
    justify-content: center;
    background-color: #1B2838;
    color: #8F98A0;
    font-size: 14px;
    text-align: center;
    padding: 0 10px;
    overflow: hidden;
    word-break: break-word;
  }
  
  .game-details {
    display: flex;
    flex-direction: column;
    min-width: 0;
    padding-left: 10px;

    @include media-breakpoint-down(md) {
      padding-left: 0;
    }
  }

  .game-title {
    color: white;
    font-size: 20px;
    margin: 0 0 10px 0;
    overflow-wrap: anywhere;

    @include media-breakpoint-down(md) {
      font-size: 17px;
      margin-bottom: 8px;
    }
  }

  .stats-grid {
    display: grid;
    grid-template-columns: auto auto auto;
    gap: 10px;
    margin-bottom: 10px;

    @include media-breakpoint-down(md) {
      grid-template-columns: repeat(2, minmax(0, 1fr));
    }
  }
  
  .stat-label {
    color: white;
    font-size: 12px;
    margin-bottom: 5px;
  }
  
  .stat-value {
    color: white;
    font-size: 12px;
  }
  
  .achievement-progress {
    height: 12px;
    background-color: transparent;
  }
  
  :deep(.p-progressbar-value) {
    background-color: #1A9FFF;
    border-radius: 2px;
  }
  
  .achievements-preview {
    display: flex;
    flex-wrap: wrap;
    gap: 2px;
  }
  
  .achievement-icon {
    width: 48px;
    height: 48px;
    background-color: white;
  }
  
  .achievement-icon img {
    width: 100%;
    height: 100%;
    object-fit: cover;
  }
  
  .more-achievements {
    width: 48px;
    height: 48px;
    background-color: #2e3238;
    display: flex;
    align-items: center;
    justify-content: center;
    color: white;
    font-size: 10px;
  }
  </style>