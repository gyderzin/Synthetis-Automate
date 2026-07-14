import { defineConfig, globalIgnores } from 'eslint/config'
import globals from 'globals'
import js from '@eslint/js'
import pluginVue from 'eslint-plugin-vue'
import pluginOxlint from 'eslint-plugin-oxlint'
import skipFormatting from '@vue/eslint-config-prettier/skip-formatting'

export default defineConfig([
  {
    name: 'base',
    files: ['**/*.{js,mjs,jsx,vue}'],
    rules: {}
  },

  globalIgnores(['**/dist/**', '**/dist-ssr/**', '**/coverage/**']),

  // 🔥 CARREGA AS CONFIGURAÇÕES DO VUE ANTES
  ...pluginVue.configs['flat/essential'],

  // 🔥 AQUI SIM É O LUGAR CERTO PARA DESABILITAR A REGRA
  {
    rules: {
      'vue/valid-v-slot': 'off',
      'vue/no-deprecated-v-on-native-modifier': 'off',
      'vue/no-deprecated-slot-attribute': 'off',
    },
  },

  {
    languageOptions: {
      globals: {
        ...globals.browser,
      },
    },
  },

  js.configs.recommended,
  ...pluginOxlint.configs['flat/recommended'],
  skipFormatting,
])
