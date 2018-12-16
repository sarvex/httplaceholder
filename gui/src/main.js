// General
import Vue from 'vue'
import App from './App.vue'
import router from './router'
import './registerServiceWorker'

// Styling
import '@/css/style.css'
import '@/css/light-theme.css'
import '@/css/dark-theme.css'

// VuejsDialog
import VuejsDialog from 'vuejs-dialog';
import 'vuejs-dialog/dist/vuejs-dialog.min.css';

// VueShortkey
import VueShortKey from 'vue-shortkey'

// FontAwesome
import 'font-awesome/css/font-awesome.css'
import 'toastr/build/toastr.css'

// Bootstrap
import 'bootstrap/dist/css/bootstrap.css'
import 'bootstrap/dist/css/bootstrap-grid.css'
import 'bootstrap/dist/css/bootstrap-reboot.css'
import 'bootstrap/dist/js/bootstrap.bundle'

// CodeMirror
import VueCodemirror from 'vue-codemirror'
import '@/css/codemirror-solarized.css'
import 'codemirror/lib/codemirror.css'
import 'codemirror/mode/yaml/yaml.js'

// Filters
import './filters/yaml'
import './filters/datetime'

import './functions/stringFormat'

import store from './store/store.js'

Vue.config.productionTip = false

Vue.use(VueCodemirror)
Vue.use(VueShortKey)
Vue.use(VuejsDialog)

new Vue({
  store,
  router,
  render: h => h(App)
}).$mount('#app')
