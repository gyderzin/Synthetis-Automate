import { createApp } from 'vue'
import { createPinia } from 'pinia'
import { createVuetify } from 'vuetify'
import { pt } from 'vuetify/locale'

import * as components from 'vuetify/components'
import * as directives from 'vuetify/directives'
import * as styles from 'vuetify/styles'

import App from './App.vue'
import router from './router'

const app = createApp(App)

const vuetify = createVuetify({
  components,
  directives,
  styles,
  locale: {
    locale: 'pt',
    messages: { pt },
  },
})

app.use(createPinia())
app.use(router)
app.use(vuetify)

app.mount('#app')
