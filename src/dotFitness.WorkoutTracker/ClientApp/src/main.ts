import { createApp } from 'vue'
import { createPinia } from 'pinia'
import App from './App.vue'
import router from './router'
import vuetify from './plugins/vuetify'
import './scss/style.scss'
import { useAuthStore } from './stores/auth'
import PerfectScrollbar from 'vue3-perfect-scrollbar'
import VueTablerIcons from 'vue-tabler-icons'
import VueApexCharts from 'vue3-apexcharts'

const app = createApp(App)
const pinia = createPinia()

app.use(pinia)
app.use(router)
app.use(vuetify)

// Initialize auth store and set up token expiration checking
const authStore = useAuthStore()
authStore.initializeAuth()

// Check token expiration every minute
setInterval(() => {
  authStore.checkTokenExpiration()
}, 60000) // 60 seconds


app.use(VueTablerIcons);
app.use(VueApexCharts);
app.mount('#app');
