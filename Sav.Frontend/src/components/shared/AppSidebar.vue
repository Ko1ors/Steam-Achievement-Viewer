<template>
    <Transition name="backdrop">
        <div v-if="isMobile && isOpen" class="sidebar-backdrop" @click="emit('close')"></div>
    </Transition>

    <aside
        id="app-sidebar"
        class="sidebar"
        :class="{ 'is-open': isOpen }"
        :inert="isMobile && !isOpen"
        :aria-hidden="isMobile && !isOpen ? 'true' : undefined">
        <div class="brand">
            <i class="fa-brands fa-steam brand-icon" aria-hidden="true"></i>
            <span class="brand-title">Steam Achievement Viewer</span>
            <button
                v-if="isMobile"
                ref="closeButton"
                type="button"
                class="sidebar-close"
                aria-label="Close navigation"
                @click="emit('close')">
                <i class="pi pi-times" aria-hidden="true"></i>
            </button>
        </div>
        <nav class="nav" aria-label="Main navigation">
            <RouterLink
                v-for="page in appPages"
                :key="page.path"
                :to="page.path"
                class="nav-item"
                :class="{ 'is-disabled': isDisabled(page) }"
                :aria-disabled="isDisabled(page)"
                @click="preventNavigation($event, page)">
                <i :class="page.icon" aria-hidden="true"></i>
                <span class="nav-label">{{ page.label }}</span>
            </RouterLink>
        </nav>
    </aside>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { appPages, type AppPage } from '@/router/pages';
import { PageTypes } from '@/helpers/routing';
import { useCurrentUserStore } from '@/stores/currentUser';

defineProps<{
    isOpen: boolean;
    isMobile: boolean;
}>();

const emit = defineEmits<{
    close: [];
}>();

const currentUser = useCurrentUserStore();
const closeButton = ref<HTMLButtonElement | null>(null);

// v1 kept Login and Settings reachable while no user was loaded; the API key is added in Settings
const ALWAYS_AVAILABLE: PageTypes[] = [PageTypes.Auth, PageTypes.Settings];

const isDisabled = (page: AppPage) => !currentUser.isLoggedIn && !ALWAYS_AVAILABLE.includes(page.path);

const preventNavigation = (event: MouseEvent, page: AppPage) => {
    if (isDisabled(page))
        event.preventDefault();
};

defineExpose({
    focusCloseButton: () => closeButton.value?.focus(),
});
</script>

<style scoped lang="scss">
.sidebar {
    display: flex;
    flex-direction: column;
    flex-shrink: 0;
    width: 240px;
    padding: 16px 12px;
    background-color: #1b2838;
    border-right: 1px solid #2a475e;
    overflow-y: auto;
    overscroll-behavior: contain;
}

.brand {
    display: flex;
    align-items: center;
    gap: 10px;
    padding: 0 8px 20px;
    color: #c7d5e0;
}

.brand-icon {
    font-size: 26px;
    color: #66c0f4;
}

.brand-title {
    flex: 1;
    font-size: 14px;
    font-weight: 700;
    line-height: 1.25;
}

.sidebar-close {
    display: inline-flex;
    align-items: center;
    justify-content: center;
    flex-shrink: 0;
    width: 36px;
    height: 36px;
    margin-right: -4px;
    padding: 0;
    border: none;
    border-radius: 4px;
    background-color: transparent;
    color: #8f98a0;
    font-size: 16px;
    cursor: pointer;

    &:hover {
        background-color: #2a475e;
        color: #ffffff;
    }

    &:focus-visible {
        outline: 2px solid #66c0f4;
        outline-offset: 2px;
    }
}

.nav {
    display: flex;
    flex-direction: column;
    gap: 4px;
}

.nav-item {
    display: flex;
    align-items: center;
    gap: 10px;
    padding: 10px 12px;
    border-radius: 4px;
    color: #c6d4df;
    font-size: 14px;
    text-decoration: none;
    transition: background-color 0.15s ease, color 0.15s ease;

    .pi {
        width: 16px;
        font-size: 14px;
    }

    &:hover {
        background-color: #2a475e;
        color: #ffffff;
    }

    &.router-link-exact-active {
        background-color: #2a475e;
        color: #ffffff;
        box-shadow: inset 3px 0 0 #66c0f4;
    }

    &.is-disabled {
        opacity: 0.45;
        pointer-events: none;
    }
}

.sidebar-backdrop {
    position: fixed;
    inset: 0;
    z-index: 55;
    background-color: rgba(8, 12, 18, 0.65);
}

.backdrop-enter-active,
.backdrop-leave-active {
    transition: opacity 0.2s ease;
}

.backdrop-enter-from,
.backdrop-leave-to {
    opacity: 0;
}

@include media-breakpoint-down(lg) {
    .sidebar {
        position: fixed;
        top: 0;
        bottom: 0;
        left: 0;
        z-index: 60;
        width: min(280px, 82vw);
        padding-bottom: calc(16px + env(safe-area-inset-bottom, 0px));
        box-shadow: 0 0 24px rgba(0, 0, 0, 0.5);
        transform: translateX(-100%);
        transition: transform 0.22s ease;

        &.is-open {
            transform: translateX(0);
        }
    }

    .nav-item {
        padding: 12px;
        font-size: 15px;
    }
}

@media (prefers-reduced-motion: reduce) {
    .sidebar {
        transition: none;
    }

    .backdrop-enter-active,
    .backdrop-leave-active {
        transition: none;
    }
}
</style>
