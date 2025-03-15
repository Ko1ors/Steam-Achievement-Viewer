import { createApp } from 'vue'
import App from './App.vue'
import { createRouter, createWebHistory } from 'vue-router'
import PrimeVue from 'primevue/config';
import Tooltip from 'primevue/tooltip';
import MasterPage from './components/shared/MasterPage.vue';
import ToastService from 'primevue/toastservice';
import ConfirmationService from 'primevue/confirmationservice';

import "primevue/resources/themes/aura-dark-noir/theme.css";
import "primevue/resources/primevue.min.css";
import "primeicons/primeicons.css";
import "./assets/fontawesome/css/all.css";

import { createPinia } from 'pinia';


const routes = [
 // { path: PageTypes.Home, component: MasterPage },
  { path: '/:pathMatch(.*)*', component: MasterPage },
]


const router = createRouter({
  history: createWebHistory(),
  routes,
})

const pinia = createPinia()
const app = createApp(App)

app.use(router);
app.use(PrimeVue);
app.use(ToastService);
app.use(ConfirmationService);
app.directive('tooltip', Tooltip);
app.use(pinia)
app.mount('#app');
