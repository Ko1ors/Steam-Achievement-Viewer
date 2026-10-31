<template>
    <div class="app-shell">
        <AppSidebar />
        <div class="app-main">
            <AppHeader />
            <main class="app-content">
                <RouterView />
            </main>
        </div>
        <Toast position="top-right" />
    </div>
</template>

<script setup lang="ts">
import { onMounted, onUnmounted } from 'vue';
import Toast from 'primevue/toast';
import { useToast } from 'primevue/usetoast';
import AppHeader from '@/components/shared/AppHeader.vue';
import AppSidebar from '@/components/shared/AppSidebar.vue';
import { subscribeToToasts } from '@/helpers/notifications';

const toast = useToast();
let unsubscribe: (() => void) | undefined;

onMounted(() => {
    unsubscribe = subscribeToToasts(options => toast.add(options));
});

onUnmounted(() => {
    unsubscribe?.();
});
</script>

<style scoped lang="scss">
.app-shell {
    display: flex;
    height: 100vh;
    overflow: hidden;
    background-color: #171a21;
    color: #c7d5e0;
}

.app-main {
    display: flex;
    flex: 1;
    flex-direction: column;
    min-width: 0;
}

.app-content {
    flex: 1;
    min-height: 0;
    overflow: hidden;
}
</style>