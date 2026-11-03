<template>
    <div class="app-shell">
        <AppSidebar ref="sidebar" :is-open="isSidebarOpen" :is-mobile="isMobile" @close="closeSidebar" />
        <div class="app-main" :inert="isMobile && isSidebarOpen">
            <AppHeader ref="header" :is-mobile="isMobile" :is-sidebar-open="isSidebarOpen" @toggle-sidebar="toggleSidebar" />
            <main class="app-content">
                <RouterView />
            </main>
        </div>
        <Toast position="top-right" />
    </div>
</template>

<script setup lang="ts">
import { nextTick, onMounted, onUnmounted, ref, watch } from 'vue';
import { useRoute } from 'vue-router';
import Toast from 'primevue/toast';
import { useToast } from 'primevue/usetoast';
import AppHeader from '@/components/shared/AppHeader.vue';
import AppSidebar from '@/components/shared/AppSidebar.vue';
import { subscribeToToasts } from '@/helpers/notifications';
import { useMediaQuery } from '@/composables/useMediaQuery';

const toast = useToast();
const route = useRoute();
const isMobile = useMediaQuery('(max-width: 992px)');
const isSidebarOpen = ref(false);
const sidebar = ref<InstanceType<typeof AppSidebar> | null>(null);
const header = ref<InstanceType<typeof AppHeader> | null>(null);

const closeSidebar = () => {
    if (!isSidebarOpen.value)
        return;

    isSidebarOpen.value = false;
    nextTick(() => header.value?.focusMenuButton());
};

const toggleSidebar = () => {
    if (isSidebarOpen.value) {
        closeSidebar();
        return;
    }

    isSidebarOpen.value = true;
    nextTick(() => sidebar.value?.focusCloseButton());
};

const onKeydown = (event: KeyboardEvent) => {
    if (event.key === 'Escape')
        closeSidebar();
};

watch(() => route.fullPath, closeSidebar);
watch(isMobile, mobile => {
    if (!mobile)
        closeSidebar();
});

let unsubscribe: (() => void) | undefined;

onMounted(() => {
    unsubscribe = subscribeToToasts(options => toast.add(options));
    document.addEventListener('keydown', onKeydown);
});

onUnmounted(() => {
    unsubscribe?.();
    document.removeEventListener('keydown', onKeydown);
});
</script>

<style scoped lang="scss">
.app-shell {
    display: flex;
    height: 100vh;
    /* Mobile browser chrome shrinks the visible viewport; dvh keeps the shell fitting it. */
    height: 100dvh;
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