<template>
  <div class="login-wrapper">
    <v-card class="login-card" elevation="10">
      <!-- Centralização manual da imagem -->
      <div class="centralizar-div">
        <v-img src="/logo-jirau.png" alt="Logo Jirau" class="logo-img" cover />
      </div>

      <v-card-text>
        <v-form ref="form" @submit.prevent>
          <v-text-field v-model="username" :rules="[v => !!v || 'Preencha seu usuário.']" label="Usuário"
            prepend-icon="mdi-account" required class="mb-4"></v-text-field>

          <v-text-field v-model="password" :rules="[v => !!v || 'Preencha sua senha.']" label="Senha" type="password"
            prepend-icon="mdi-lock" required class="mb-4"></v-text-field>

          <v-btn :loading="loadingEntrar" type="button" color="primary" block size="large" @click="handleLogin">
            Entrar
          </v-btn>
        </v-form>
      </v-card-text>
    </v-card>
  </div>
</template>

<script>
import { useAuthStore } from '@/stores/login/auth';
import { useAlertStore } from '@/stores/alert';
import router from '@/router';
export default {
  data: () => ({
    username: '',
    password: '',
    loadingEntrar: false
  }),
  setup() {
    const auth = useAuthStore();
    const alert = useAlertStore();

    return { auth, alert };
  },
  methods: {
    async handleLogin() {
      const { valid } = await this.$refs.form.validate();
      if (!valid) return;
      this.loadingEntrar = true
      const payload = await this.auth.login(this.username, this.password);

      if (payload) {
        this.alert.callAlert({
          type: 'success',
          title: 'Logado com sucesso',
          text: ''
        });
        router.push('/home');
      } else {
        this.tratarErroLogin();
      }
      this.loadingEntrar = false
    },
    tratarErroLogin() {
      let alert = {}
      switch (this.auth.getError) {
        case 1:
          alert = { type: 'error', title: 'Usuário ou senha incorretos' }
          break;
        case 2:
          alert = { type: 'error', title: 'Erro de validação dos campos' }
          break;
        case 3:
          alert = { type: 'warning', title: 'Erro de conexão com o servidor', text: 'Contate o suporte!' }
          break;
        default:
          alert = { type: 'warning', title: 'Erro inesperado', text: 'Contate o suporte!' }
      }
      this.alert.callAlert(alert);
    }

  }
}
</script>

<style scoped>
.login-wrapper {
  position: fixed;
  inset: 0;
  display: flex;
  justify-content: center;
  align-items: center;
  background-color: #f0f2f5;
  padding: 1rem;
}

.login-card {
  width: 100%;
  max-width: 420px;
  min-width: 320px;
  padding: 1.5rem 1.2rem;
  border-radius: 12px;
}

/* Imagem responsiva */
.logo-img {
  max-width: 200px;
  width: 100%;
  height: auto;
  margin-bottom: 1rem;
}
</style>
