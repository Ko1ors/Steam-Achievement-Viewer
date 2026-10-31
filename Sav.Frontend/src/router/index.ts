import { createRouter, createWebHistory, type RouteRecordRaw } from 'vue-router';
import MasterPage from '@/components/shared/MasterPage.vue';
import { LandingPage, appPages } from '@/router/pages';

const routes: RouteRecordRaw[] = [
    {
        path: '/',
        component: MasterPage,
        children: [
            { path: '', redirect: LandingPage },
            ...appPages.map(page => ({ path: page.path, component: page.component })),
        ],
    },
    { path: '/:pathMatch(.*)*', redirect: '/' },
];

const router = createRouter({
    history: createWebHistory(),
    routes,
});

export default router;
