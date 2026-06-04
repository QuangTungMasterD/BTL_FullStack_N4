import { createApp } from 'vue'
// import './style.css'
import './tailwind.css'
import App from './App.vue'

import { createPinia } from 'pinia';
import { registerPlugins } from './plugins';

const app = createApp(App)

registerPlugins(app)

app.mount('#app')
