<template>
    <aside class="sidebar">
        <div class="brand">
            <i class="fa-brands fa-steam brand-icon" aria-hidden="true"></i>
            <span class="brand-title">Steam Achievement Viewer</span>
        </div>
        <nav class="nav" aria-label="Main navigation">
            <RouterLink v-for="page in appPages" :key="page.path" :to="page.path" class="nav-item">
                <i :class="page.icon" aria-hidden="true"></i>
                <span class="nav-label">{{ page.label }}</span>
            </RouterLink>
        </nav>
    </aside>
</template>

<script setup lang="ts">
import { appPages } from '@/router/pages';
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
    font-size: 14px;
    font-weight: 700;
    line-height: 1.25;
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
}
</style>
