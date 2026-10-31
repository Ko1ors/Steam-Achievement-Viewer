<template>
    <div class="achievement-card">
        <div class="achievement-content">
            <div class="achievement-icon-container">
                <img :src="achievement[iconField]" alt="Achievement icon" class="achievement-icon"/>
            </div>

            <div class="achievement-details">
                <div v-if="showGame && achievement.gameName" class="achievement-game">
                    <img v-if="achievement.gameIcon" :src="achievement.gameIcon" alt="" class="game-icon" />
                    <span class="game-name">{{ achievement.gameName }}</span>
                </div>
                <h3 class="achievement-name">{{ achievement.name }}</h3>
                <p class="achievement-description">{{ achievement.description }}</p>
                <p class="achievement-percent">{{ achievement.percent }}%</p>
                <p v-if="showUnlockTime" class="achievement-percent">{{ formatUnlockTime(achievement.unlockTime) }}</p>
            </div>
        </div>
    </div>
</template>

<script setup>
// Props
const props = defineProps({
    achievement: {
        type: Object,
        required: true
    },
    showUnlockTime: {
        type: Boolean,
        default: false
    },
    iconField: {
        type: String,
        default: 'iconOpen'
    },
    showGame: {
        type: Boolean,
        default: false
    }
});

const formatUnlockTime = (unlockTime) => {
    if (!unlockTime) return '';

    const date = new Date(unlockTime);
    if (isNaN(date.getTime())) return '';

    return date.toLocaleString('en-US');
};
</script>

<style scoped>
.achievement-card {
    width: 630px;
    background-color: #1B2838;
    margin: 10px;
}

.achievement-content {
    margin: 10px;
    display: flex;
    align-items: center;
}

.achievement-icon-container {
    width: 60px;
    height: 60px;
    margin: 0 10px 0 10px;
    background-color: white;
    display: flex;
    align-items: center;
    justify-content: center;
}

.achievement-icon {
    width: 100%;
    height: 100%;
    object-fit: contain;
}

.achievement-details {
    margin: 5px;
    max-width: 480px;
}

.achievement-game {
    display: flex;
    align-items: center;
    gap: 6px;
    margin-bottom: 5px;
    color: #8f98a0;
    font-size: 11px;
}

.achievement-game .game-icon {
    width: 16px;
    height: 16px;
    flex-shrink: 0;
    object-fit: contain;
}

.achievement-game .game-name {
    overflow: hidden;
    max-width: 400px;
    white-space: nowrap;
    text-overflow: ellipsis;
}

.achievement-name {
    color: white;
    font-weight: bold;
    font-size: 18px;
    font-family: 'Arial Black', Arial, sans-serif;
    margin: 0 0 5px 0;
    word-wrap: break-word;
}

.achievement-description {
    color: whitesmoke;
    font-size: 13px;
    font-family: 'Arial Black', Arial, sans-serif;
    margin: 0 0 5px 0;
    word-wrap: break-word;
}

.achievement-percent {
    color: whitesmoke;
    font-size: 15px;
    font-family: 'Arial Black', Arial, sans-serif;
    margin: 0;
}
</style>