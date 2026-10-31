<template>
    <div class="master-page">
        <div class="sidebar">
            <div class="menu">
                <Button 
                    label="Completed Games" 
                    icon="pi pi-check-circle"
                    class="w-full mb-2" 
                    @click="navigateTo(PageTypes.CompletedGames)"
                />
                <Button 
                    label="Closest Achievements" 
                    icon="pi pi-star"
                    class="w-full mb-2" 
                    @click="navigateTo(PageTypes.ClosestAchievements)"
                />
                <Button 
                    label="All Closest Achievements" 
                    icon="pi pi-star-fill"
                    class="w-full mb-2" 
                    @click="navigateTo(PageTypes.AllClosestAchievements)"
                />
                <Button 
                    label="Rarest Achievements" 
                    icon="pi pi-trophy"
                    class="w-full mb-2" 
                    @click="navigateTo(PageTypes.RarestAchievements)"
                />
                <Button 
                    label="Easiest Games" 
                    icon="pi pi-chart-line"
                    class="w-full mb-2" 
                    @click="navigateTo(PageTypes.EasiestGames)"
                />
                <Button 
                    label="Last Achieved" 
                    icon="pi pi-clock"
                    class="w-full mb-2" 
                    @click="navigateTo(PageTypes.LastAchieved)"
                />
            </div>
        </div>
        <div class="content">
            <RouterView />
        </div>
    </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router';
import Button from 'primevue/button';
import { PageTypes } from '@/helpers/routing';

const router = useRouter();

const navigateTo = (path: PageTypes) => {
    router.push(path);
};
</script>

<style scoped lang="scss">
.master-page {
    display: flex;
    height: 100vh;
    background-color: #171A21;

    .sidebar {
        width: 250px;
        background-color: #1b2838;
        padding: 1rem;

        .menu {
            margin-top: 2rem;
        }
    }

    .content {
        flex: 1;
        overflow: hidden;
    }
}
</style>