import react from '@vitejs/plugin-react'
import { fileURLToPath } from 'node:url'
import { defineConfig } from 'vitest/config'

const apiBase = JSON.stringify(process.env.TEAM_TASKS_API_BASE_URL ?? 'http://localhost:5276')

export default defineConfig({
  plugins: [react()],
  define: {
    'import.meta.env.VITE_API_BASE_URL': apiBase,
  },
  test: {
    environment: 'jsdom',
    setupFiles: ['./src/setupTests.ts'],
    globals: true,
  },
  resolve: {
    dedupe: ['react', 'react-dom'],
    alias: {
      react: fileURLToPath(new URL('node_modules/react', import.meta.url)),
      'react-dom': fileURLToPath(new URL('node_modules/react-dom', import.meta.url)),
      '@client': fileURLToPath(new URL('../../client/src', import.meta.url)),
    },
  },
})
