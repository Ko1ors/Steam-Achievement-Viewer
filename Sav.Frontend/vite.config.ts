import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import { fileURLToPath, URL } from "node:url";

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [vue()],
  build: {
    outDir: '../Sav.WebApp/wwwroot',
    // outDir sits outside the Vite root, so stale bundles are only removed when this is explicit
    emptyOutDir: true,
    sourcemap: true,
  },
  css: {
    preprocessorOptions: {
        scss: {
            additionalData: '@import "./src/style.scss";',
        },
    },
  },
  resolve: {
    alias: 
    [
      { find: '@', replacement: fileURLToPath(new URL('./src', import.meta.url)) },
    ],
  },
})
