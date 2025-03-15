import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import { fileURLToPath, URL } from "node:url";

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [vue()],
  build: {
    outDir: '../Sav.WebApp/wwwroot',
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
