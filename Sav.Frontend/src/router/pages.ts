import type { RouteComponent } from 'vue-router';
import { PageTypes } from '@/helpers/routing';
import CompletedGamesPage from '@/components/pages/CompletedGamesPage.vue';
import ClosestAchievementsPage from '@/components/pages/ClosestAchievementsPage.vue';
import AllClosestAchievementsPage from '@/components/pages/AllClosestAchievementsPage.vue';
import EasiestGamesPage from '@/components/pages/EasiestGamesPage.vue';
import RareAchievementsPage from '@/components/pages/RareAchievementsPage.vue';
import LastAchievedPage from '@/components/pages/LastAchievedPage.vue';

export interface AppPage {
    path: PageTypes;
    label: string;
    icon: string;
    component: RouteComponent;
}

export const appPages: AppPage[] = [
    {
        path: PageTypes.CompletedGames,
        label: 'Completed Games',
        icon: 'pi pi-check-circle',
        component: CompletedGamesPage
    },
    {
        path: PageTypes.ClosestAchievements,
        label: 'Closest Achievements',
        icon: 'pi pi-star',
        component: ClosestAchievementsPage
    },
    {
        path: PageTypes.AllClosestAchievements,
        label: 'All Closest Achievements',
        icon: 'pi pi-star-fill',
        component: AllClosestAchievementsPage
    },
    {
        path: PageTypes.RarestAchievements,
        label: 'Rarest Achievements',
        icon: 'pi pi-trophy',
        component: RareAchievementsPage
    },
    {
        path: PageTypes.EasiestGames,
        label: 'Easiest Games',
        icon: 'pi pi-chart-line',
        component: EasiestGamesPage
    },
    {
        path: PageTypes.LastAchieved,
        label: 'Last Achieved',
        icon: 'pi pi-clock',
        component: LastAchievedPage
    }
];

export const LandingPage = PageTypes.LastAchieved;
