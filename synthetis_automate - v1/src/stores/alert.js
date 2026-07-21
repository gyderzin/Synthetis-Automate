import { defineStore } from 'pinia'

export const useAlertStore = defineStore('alert', {
  state: () => ({
    stateAlert: false,
    type: 'info',
    title: '',
    text: '',
  }),
  actions: {
    callAlert(payload) {
      this.type = payload.type
      this.title = payload.title
      this.text = payload.text
      this.stateAlert = true
    },
    closeAlert() {
      this.stateAlert = false
    }
  },
  getters: {
    getStateAlert(state) {
      return state.stateAlert
    },
    getType(state) {
      return state.type
    },
    getTitle(state) {
      return state.title
    },
    getText(state) {
      return state.text
    },
  },
})
