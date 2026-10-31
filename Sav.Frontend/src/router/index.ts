import { createRouter, createWebHistory, type RouteRecordRaw } from 'vue-router';
import MasterPage from '@/components/shared/MasterPage.vue';
import { appPages } from '@/router/pages';
import { PageTypes } from '@/helpers/routing';
import { useCurrentUserStore } from '@/stores/currentUser';

const routes: RouteRecordRaw[] = [
    {
        path: '/',
        component: MasterPage,
        children: appPages.map(page => ({
            path: page.path === PageTypes.Home ? '' : page.path,
            component: page.component,
        })),
    },
    { path: '/:pathMatch(.*)*', redirect: '/' },
];

const router = createRouter({
    history: createWebHistory(),
    routes,
});

router.beforeEach(async to => {
    const currentUser = useCurrentUserStore();
    await currentUser.ensureLoadedAsync();

    // v1 kept Settings reachable while no user was loaded (the Steam Web API key lives there)
    const isAlwaysAvailable = to.path === PageTypes.Auth || to.path === PageTypes.Settings;

    if (!currentUser.isLoggedIn && !isAlwaysAvailable)
        return { path: PageTypes.Auth };

    return true;
});

export default router;
