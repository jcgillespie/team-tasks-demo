import path from 'node:path'
import { fileURLToPath } from 'node:url'
import { defineConfig } from 'vitest/config'

const __dirname = path.dirname(fileURLToPath(import.meta.url))
const clientSrc = path.resolve(__dirname, '../../../client/src')

export default defineConfig({
  plugins: [],
  resolve: {
    alias: {
      '@': clientSrc,
    },
  },
  define: {
    'import.meta.env.VITE_API_BASE_URL': JSON.stringify('http://127.0.0.1:9'),
  },
  test: {
    environment: 'node',
    include: ['unit/**/*.spec.ts'],
    globals: false,
  },
})
