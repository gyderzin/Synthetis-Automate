import { defineStore } from 'pinia'
import api from '@/service/http.js'

export const useAuthStore = defineStore('auth', {
  state: () => ({
    usuario: JSON.parse(localStorage.getItem('usuario')) || null,
    token: localStorage.getItem('token') || '',
    error: '',
  }),
  getters: {
    getError(state) {
      return state.error
    },
    isAuthenticated(state) {
      return !!state.token
    },
    getUsuario(state) {
      return state.usuario
    },
  },
  actions: {
    async login(username, password) {
      console.log(username, password)
      try {
        const params = new URLSearchParams()
        params.append('nomeUsuario', username)
        params.append('senha', password)

        const response = await api.post('/auth/login', params)

        this.token = response.data.access_token
        this.usuario = response.data.usuario // <-- Certifique-se que o backend envia isso!

        localStorage.setItem('token', this.token)
        localStorage.setItem('usuario', JSON.stringify(this.usuario))

        this.error = ''
        return true
      } catch (err) {
        if (err.response) {
          if (err.response.status === 401) {
            this.error = 1
          } else if (err.response.status === 422) {
            this.error = 2
          }
        } else if (err.request) {
          console.log('Erro de conexão com o servidor:', err.message)
          this.error = 3
        } else {
          console.error('Erro inesperado:', err.message)
          this.error = 4
        }

        return false
      }
    },

    logout() {
      this.token = ''
      this.usuario = null
      localStorage.removeItem('token')
      localStorage.removeItem('usuario')
    },
  },
})
