import type { Preview } from "@storybook/vue3";
import { setup } from '@storybook/vue3'
import "primevue/resources/themes/lara-dark-purple/theme.css";
import "primevue/resources/primevue.min.css";
import "primeicons/primeicons.css";
import PrimeVue from 'primevue/config';
import Tooltip from 'primevue/tooltip';
import ToastService from 'primevue/toastservice';
import ConfirmationService from 'primevue/confirmationservice';

setup((app) => {
  app.use(PrimeVue);
  app.use(ToastService);
  app.use(ConfirmationService);
  app.directive('tooltip', Tooltip);
})

const preview: Preview = {
  parameters: {
    controls: {
      matchers: {
        color: /(background|color)$/i,
        date: /Date$/i,
      },
    },
  },
};

export default preview;
