<!-- eslint-disable vue/valid-v-slot -->
<template>
  <v-layout class="app-layout">
    <!-- App Bar -->
    <v-app-bar ref="appBar" color="primary">
      <v-app-bar-title>Synthetis Automate</v-app-bar-title>

      <v-btn icon @click="dialogOlivan = true"><v-icon>mdi-heart</v-icon></v-btn>
      <v-btn icon @click="logout()"><v-icon>mdi-exit-to-app</v-icon></v-btn>
    </v-app-bar>

    <!-- Conteúdo com scroll controlado -->
    <v-window v-model="navigation" :touch="false">

      <!-- WINDOW 1 -->
      <v-window-item :value="1" class="my-5">
        <v-main class="scrollable-main content-offset">
          <v-container fluid class="d-flex align-center justify-center pa-0">
            <v-row class="w-100" justify="center">
              <v-col cols="12" md="10" lg="8">

                <!-- CARD COM CABEÇALHO FIXO E SCROLL INTERNO -->
                <v-card class="pa-0 emissao-card card-scroll" elevation="1" style="position: relative;">

                  <!-- CABEÇALHO FIXO -->
                  <div class="card-header-fixed">

                    <v-row class="align-center" no-gutters>
                      <v-col cols="12" sm="4" md="4" class="my-5">
                        <v-row>
                          <v-col cols="12" class="ml-4">
                            <v-text-field v-model="searchModelos" density="compact" label="Buscar"
                              prepend-inner-icon="mdi-magnify" variant="solo-filled" flat hide-details />
                          </v-col>
                        </v-row>

                      </v-col>
                      <v-col cols="12" sm="4" md="4" class="d-flex align-center mb-3 mb-sm-0">
                        <v-row>
                          <v-col class="d-flex justify-center ma-0 pa-0">
                            <span class="text-h6">Emissão de relatórios</span>
                          </v-col>
                        </v-row>
                      </v-col>
                      <v-col cols="12" sm="4" md="4" class="d-flex justify-end fab-wrapper">
                        <v-fab class="d-flex justify-end mr-5" color="primary" icon="mdi-plus"
                          @click="dialogNovoModelo = true"></v-fab>
                      </v-col>
                    </v-row>
                  </div>

                  <!-- ÁREA ROLÁVEL -->
                  <div class="card-scroll-area">
                    <v-row align="center" justify="center" dense>
                      <v-col cols="12" md="12" sm="12" lg="6" v-for="modelo in modelosFiltrados" :key="modelo.id">

                        <v-card color="indigo" variant="elevated">
                          <v-card-title class="mb-0 pb-0">
                            <v-row>
                              <v-col>
                                {{ modelo['titulo'] }}
                              </v-col>
                              <v-col class="d-flex justify-end pa-0 ma-0 py-3">
                                <v-btn :icon="isFavorito(modelo.id)
                                  ? 'mdi-heart'
                                  : 'mdi-heart-outline'" variant="text" density="comfortable"
                                  @click="toggleFavorito(modelo.id)">
                                </v-btn>
                              </v-col>
                            </v-row>

                          </v-card-title>
                          <v-card-subtitle class="mt-0 pt-0">
                            {{ modelo['descricao'] }}
                          </v-card-subtitle>
                          <v-card-actions>
                            <v-row>
                              <v-col cols="12" class="d-flex justify-center">
                                <v-btn icon="mdi-pencil" size="large"></v-btn>
                                <v-btn icon="mdi-information" size="large"></v-btn>
                                <v-btn icon="mdi-delete" @click="abrirDialogDeletarModelo(modelo)" size="large"></v-btn>
                              </v-col>

                              <v-col class="d-flex justify-start mt-0 pt-0">
                                <v-btn prepend-icon="mdi-play-box" color="green" variant="elevated" block size="large"
                                  @click="preencherFormulario(modelo)">
                                  PREENCHER RELATÓRIO
                                </v-btn>
                              </v-col>
                            </v-row>
                          </v-card-actions>
                        </v-card>
                      </v-col>
                      <v-row v-if="modelosFiltrados.length === 0" justify="center">
                        <v-col cols="12" class="text-center">
                          Nenhum modelo encontrado.
                        </v-col>
                      </v-row>
                    </v-row>
                  </div>

                </v-card>
              </v-col>
            </v-row>
          </v-container>
        </v-main>
      </v-window-item>

      <!-- WINDOW 2 AJUSTADO -->
      <v-window-item :value="2" class="my-2">
        <v-main class="scrollable-main content-offset">
          <v-container fluid class="d-flex align-center justify-center pa-0">
            <v-row class="w-100" justify="center">
              <v-col cols="12" md="12" lg="10" xl="8">

                <!-- CARD CENTRALIZADO E COM SCROLL APENAS NA TABELA -->
                <v-card class="pa-4 window2-card" elevation="1" rounded="lg"
                  :style="{ maxHeight: availableHeight, width: '100%' }">

                  <!-- HEADER -->
                  <v-row class="align-center" no-gutters>
                    <v-col cols="12" sm="6" class="d-flex align-center mb-2 mb-sm-0">
                      <v-icon class="me-2">mdi-file</v-icon>
                      <span class="text-h6">Consulta de relatórios</span>
                    </v-col>

                    <v-col cols="12" sm="6">
                      <v-text-field v-model="search" density="compact" label="Buscar" prepend-inner-icon="mdi-magnify"
                        variant="solo-filled" flat hide-details />
                    </v-col>
                  </v-row>

                  <!-- ÁREA COM SCROLL NA TABELA -->
                  <div class="window2-table-container mt-4">
                    <v-data-table :headers="headers" :items="relatorios" :items-per-page="itemsPerPage" :page="page"
                      @update:page="page = $event" :search="search" class="elevation-1">

                      <template v-slot:[`item.data_hora`]="{ item }">
                        {{ new Date(item.emitido_em).toLocaleString("pt-BR") }}
                      </template>

                      <template v-slot:[`item.actions`]="{ item }">
                        <v-btn icon="mdi-download" color="primary" variant="text"
                          @click="baixarRelatorio(item.raw ? item.raw.id : item.id, item.item_pendente)" />

                        <v-btn icon="mdi-delete" color="error" variant="text"
                          @click="abrirDialogDeletarRelatorio(item)" />


                        <v-btn v-if="temPendencias(item.item_pendente)" @click="abrirDialogItemPendente(item)"
                          variant="text" icon="mdi-file-edit" color="warning"></v-btn>


                      </template>

                      <template #bottom>
                        <div class="text-center pt-2">
                          <v-pagination v-model="page" :length="pageCount" />
                        </div>
                      </template>

                    </v-data-table>

                  </div>

                </v-card>

              </v-col>
            </v-row>
          </v-container>
        </v-main>
      </v-window-item>


      <v-window-item :value="3">
        teste
      </v-window-item>
    </v-window>


    <!-- Bottom Navigation fixo -->
    <v-bottom-navigation ref="bottomNav" grow v-model="navigation">
      <v-btn @click="navigation = 1" value="1" :active="navigation == 1">
        <v-icon>mdi-pencil-box-multiple</v-icon>
        <span>Relatórios</span>
      </v-btn>

      <v-btn @click="navigation = 2" :active="navigation == 2">
        <v-icon>mdi-file-multiple</v-icon>
        <span>Arquivos</span>
      </v-btn>

      <!-- <v-btn @click="navigation = 3" :active="navigation == 3">
        <v-icon>mdi-account</v-icon>
        <span>Perfil</span>
      </v-btn> -->
    </v-bottom-navigation>
    <NovoModelo v-model="dialogNovoModelo" :app="app" :alert="alert" />
    <PreencherFormulario v-model="dialogPreencherForm" :app="app" :alert="alert" :modelo="modeloForm" />
    <AdicionarItensPendentes v-model="dialogItemPendente" :relatorio="relatorioSelecionado"
      @enviar-pendencias="receberPendencias" />

    <!-- DIALOG de confirmação -->
    <v-dialog transition="dialog-bottom-transition" width="80vh" v-model="dialogConfirmarDownload" persistent>
      <v-card color="warning" prepend-icon="mdi-camera"
        text="Seu relatório tem um ou mais itens pendentes para adicionar, deseja baixa-lo mesmo incompleto? Você pode adicionar os itens pendentes no botão com ícone laranja."
        title="Deseja prosseguir com o download?">

        <v-card-actions>
          <v-row class="my-1" style="width:100%">
            <v-col class="mx-1 my-0 py-0">
              <v-btn variant="outlined" @click="cancelarConfirmacao">
                Voltar
              </v-btn>
            </v-col>
            <v-col class="d-flex justify-end mx-1 my-0 py-0">
              <v-btn variant="elevated" @click="confirmarDownloadSim" color="success">
                Confirmar
              </v-btn>
            </v-col>
          </v-row>
        </v-card-actions>
      </v-card>
    </v-dialog>


    <v-dialog transition="dialog-bottom-transition" width="80vh" v-model="dialogConfirmarDelete" persistent>
      <v-card color="warning" prepend-icon="mdi-info"
        text="Ao clicar em confirmar você não terá como recuperar esse relatório. Você tem certeza dessa ação?."
        title="Deseja deletar esse relatório?">

        <v-card-actions>
          <v-row class="my-1" style="width:100%">
            <v-col class="mx-1 my-0 py-0">
              <v-btn variant="outlined" @click="dialogConfirmarDelete = false">
                Voltar
              </v-btn>
            </v-col>
            <v-col class="d-flex justify-end mx-1 my-0 py-0">
              <v-btn variant="elevated" @click="deletarRelatorio" color="success">
                Confirmar
              </v-btn>
            </v-col>
          </v-row>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-dialog transition="dialog-bottom-transition" width="80vh" v-model="dialogConfirmarDeleteModel" persistent>
      <v-card color="warning" prepend-icon="mdi-info"
        text="Ao clicar em confirmar você não terá como recuperar esse modelo de relatório. Você tem certeza dessa ação?."
        title="Deseja deletar esse modelo de relatório?">

        <v-card-actions>
          <v-row class="my-1" style="width:100%">
            <v-col class="mx-1 my-0 py-0">
              <v-btn variant="outlined" @click="dialogConfirmarDeleteModel = false">
                Voltar
              </v-btn>
            </v-col>
            <v-col class="d-flex justify-end mx-1 my-0 py-0">
              <v-btn variant="elevated" @click="deletarModelo" color="success">
                Confirmar
              </v-btn>
            </v-col>
          </v-row>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-dialog transition="dialog-bottom-transition" width="80vh" v-model="dialogOlivan" persistent>
      <v-card>
        <v-card-title class="text-center">
          Vamos pra frente que o dia fácil foi ontem!
        </v-card-title>
        <v-img src="/olivan.jpeg" height="75vh"></v-img>
        <v-card-actions>
          <v-row class="my-1" style="width:100%">
            <v-col class="mx-1 my-0 py-0">
              <v-btn variant="outlined" @click="dialogOlivan = false">
                Voltar
              </v-btn>
            </v-col>
            <v-col class="d-flex justify-end mx-1 my-0 py-0">
            </v-col>
          </v-row>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-layout>
</template>

<script>
import { useAppStore } from '@/stores/app/app'
import { useAlertStore } from '@/stores/alert'
import NovoModelo from '@/views/dialogs/App/NovoModelo.vue'
import PreencherFormulario from '@/views/dialogs/App/PreencherFormulario.vue'
import router from '@/router';
import AdicionarItensPendentes from '@/views/dialogs/App/AdicionarItensPendentes.vue';

export default {
  components: {
    NovoModelo, PreencherFormulario, AdicionarItensPendentes
  },
  data() {
    return {
      app: useAppStore(),
      alert: useAlertStore(),
      dialogNovoModelo: false,
      dialogPreencherForm: false,
      dialogConfirmarDownload: false,
      dialogItemPendente: false,
      dialogOlivan: false,
      dialogConfirmarDelete: false,
      dialogConfirmarDeleteModel: false,
      confirmarDownload: false,
      modeloForm: null,
      relatorioSelecionado: null,
      modeloSelecionado: null,
      navigation: 1,
      page: 1,
      itemsPerPage: 15,
      search: '',
      searchModelos: '',
      headers: [
        { title: "Relatório", key: "nome_arquivo", align: "start" },
        { title: "Modelo", key: "modelo" },
        { title: "Emitido em", key: "data_hora" },
        { title: "Ações", key: "actions", sortable: false }
      ],
      // Offsets para calcular espaços dinâmicos
      topOffset: 64,
      bottomOffset: 56,
      resizeObserver: null,
      favoritos: []
    }
  },
  computed: {
    modelosFiltrados() {
      if (!this.searchModelos?.trim()) {

        return [...(this.app.modelos || [])]
          .sort((a, b) => {

            const favA = this.favoritos.includes(a.id)
            const favB = this.favoritos.includes(b.id)

            if (favA && !favB) return -1
            if (!favA && favB) return 1

            return 0
          })
      }

      const termo = this.searchModelos
        .toLowerCase()
        .trim()

      return (this.app.modelos || [])
        .filter(modelo => {
          return (
            modelo.id?.toString().includes(termo) ||
            modelo.titulo?.toLowerCase().includes(termo) ||
            modelo.descricao?.toLowerCase().includes(termo)
          )
        })
        .sort((a, b) => {

          const favA = this.favoritos.includes(a.id)
          const favB = this.favoritos.includes(b.id)

          if (favA && !favB) return -1
          if (!favA && favB) return 1

          return 0
        })
    },

    relatorios() {
      return this.app.relatorios || []
    },
    availableHeight() {
      const extra = 32;
      return `calc(100vh - ${this.topOffset}px - ${this.bottomOffset}px - ${extra}px)`;
    },

    pageCount() {
      return Math.max(1, Math.ceil((this.relatorios?.length || 0) / this.itemsPerPage));
    }
  },
  methods: {
    logout() {
      localStorage.clear()
      router.push('/')
    },
    preencherFormulario(model) {
      this.modeloForm = model
      this.dialogPreencherForm = true
    },
    async baixarRelatorio(idRelatorio, item_pendente) {
      const pendencia = this.temPendencias(item_pendente)
      if (pendencia) {
        const confirmado = await this.abrirDialogConfirmacao()

        if (!confirmado) return  // usuário cancelou

        await this.app.baixarRelatorio(idRelatorio)
      } else {
        await this.app.baixarRelatorio(idRelatorio)
      }
    },
    abrirDialogConfirmacao() {
      return new Promise((resolve) => {
        this.confirmarDownload = resolve
        this.dialogConfirmarDownload = true
      })
    },
    confirmarDownloadSim() {
      if (this.confirmarDownload) this.confirmarDownload(true)
      this.dialogConfirmarDownload = false
    },

    cancelarConfirmacao() {
      if (this.confirmarDownload) this.confirmarDownload(false)
      this.dialogConfirmarDownload = false
    },
    temPendencias(valor) {
      try {
        return JSON.parse(valor || "[]").length > 0;
      } catch {
        return false;
      }
    },
    abrirDialogItemPendente(item) {
      this.relatorioSelecionado = item
      this.dialogItemPendente = true
    },
    async receberPendencias(payload) {
      try {
        const { relatorio_id, itens } = payload
        console.log(payload)
        if (!relatorio_id || !Array.isArray(itens)) {
          console.error("Dados inválidos recebidos do componente:", payload)
          return
        }

        // Envia para a store
        const ok = await this.app.enviarItensPendentes(relatorio_id, itens)
        if (ok != false) {
          // Atualiza lista exibida
          await this.app.recuperarRelatórios().then((data) => {
            if (data?.name === "ERR_NETWORK") {
              this.alert.callAlert({
                type: "warning",
                title: "Sistema indisponível. Contate o suporte.",
              });
            }
          })
          this.app.recuperarRelatórios()
          this.dialogItemPendente = false
          this.alert.callAlert({
            type: "success",
            title: "Relatório atualizado com sucesso!",
          });
        } else {
          this.alert.callAlert({
            type: "alarm",
            title: "Não foi possível gerar o relatório.",
            text: "Contate o suporte técnico."
          });
        }

      } catch (err) {
        console.error("Erro na função receberPendencias:", err)
        this.alert.callAlert({
          type: "error",
          title: "Erro interno ao enviar pendências",
        });
      }
    },
    async deletarModelo() {
      await this.app.deletarModelo(this.modeloSelecionado.id).then(() => {
        this.alert.callAlert({
          type: 'success',
          title: "Modelo de relatório excluído com sucesso!"
        })
      })
      const user = JSON.parse(localStorage.getItem('usuario')) || { equipe: '' }
      await this.app.getModels(user).then((data) => {
        if (data?.name === "ERR_NETWORK") {
          this.alert.callAlert({
            type: "warning",
            title: "Sistema indisponível. Contate o suporte.",
          });
        }
      })
      this.dialogConfirmarDeleteModel = false
    },
    abrirDialogDeletarModelo(item) {
      this.modeloSelecionado = item
      this.dialogConfirmarDeleteModel = true
    },
    async deletarRelatorio() {
      await this.app.deletarRelatorio(this.relatorioSelecionado.id).then(() => {
        this.alert.callAlert({
          type: 'success',
          title: "Relatório excluído com sucesso!"
        })
      })
      await this.app.recuperarRelatórios().then((data) => {
        if (data?.name === "ERR_NETWORK") {
          this.alert.callAlert({
            type: "warning",
            title: "Sistema indisponível. Contate o suporte.",
          });
        }
      })
      this.dialogConfirmarDelete = false
    },
    abrirDialogDeletarRelatorio(item) {
      this.relatorioSelecionado = item
      this.dialogConfirmarDelete = true
    },
    computeOffsets() {
      const getEl = (ref) => {
        if (!ref) return null
        return ref.$el ? ref.$el : (ref instanceof HTMLElement ? ref : null)
      }

      const appBarEl = getEl(this.$refs.appBar)
      const bottomEl = getEl(this.$refs.bottomNav)

      const topH = appBarEl ? Math.ceil(appBarEl.getBoundingClientRect().height) : 64
      const bottomH = bottomEl ? Math.ceil(bottomEl.getBoundingClientRect().height) : 56

      if (this.topOffset !== topH) this.topOffset = topH
      if (this.bottomOffset !== bottomH) this.bottomOffset = bottomH
    },
    onResize() {
      this.computeOffsets()
    },
    toggleFavorito(idModelo) {
      console.log('cliquei')
      const index = this.favoritos.indexOf(idModelo)

      if (index > -1) {
        this.favoritos.splice(index, 1)
      } else {
        this.favoritos.push(idModelo)
      }

      localStorage.setItem(
        "modelosFavoritos",
        JSON.stringify(this.favoritos)
      )
    },
    isFavorito(idModelo) {
      return this.favoritos.includes(idModelo)
    },
  },
  created() {
    const favoritosSalvos =
      JSON.parse(localStorage.getItem("modelosFavoritos") || "[]")

    this.favoritos = favoritosSalvos

    const user = JSON.parse(localStorage.getItem('usuario')) || { equipe: '' }

    this.app.getModels(user)

    this.app.recuperarRelatórios().then((data) => {
      if (data?.name === "ERR_NETWORK") {
        this.alert.callAlert({
          type: "warning",
          title: "Sistema indisponível. Contate o suporte.",
        });
      }
    })
  },
  mounted() {
    this.$nextTick(() => {
      this.computeOffsets()
    })
    window.addEventListener('resize', this.onResize)
    if (window.ResizeObserver) {
      this.resizeObserver = new ResizeObserver(() => {
        this.computeOffsets()
      })
      const appBarEl = this.$refs.appBar && (this.$refs.appBar.$el || this.$refs.appBar)
      const bottomEl = this.$refs.bottomNav && (this.$refs.bottomNav.$el || this.$refs.bottomNav)
      if (appBarEl) this.resizeObserver.observe(appBarEl)
      if (bottomEl) this.resizeObserver.observe(bottomEl)
    }
  },
  beforeUnmount() {
    window.removeEventListener('resize', this.onResize)
    if (this.resizeObserver) {
      try {
        const appBarEl = this.$refs.appBar && (this.$refs.appBar.$el || this.$refs.appBar)
        const bottomEl = this.$refs.bottomNav && (this.$refs.bottomNav.$el || this.$refs.bottomNav)
        if (appBarEl) this.resizeObserver.unobserve(appBarEl)
        if (bottomEl) this.resizeObserver.unobserve(bottomEl)
      } catch (e) { console.log(e) }
      this.resizeObserver.disconnect()
    }
  },
  watch: {
    navigation(newVal) {
      if (newVal == 2) {
        this.app.recuperarRelatórios().then((data) => {
          if (data?.name === "ERR_NETWORK") {
            this.alert.callAlert({
              type: "warning",
              title: "Sistema indisponível. Contate o suporte.",
            });
          }
        })
      }
    }
  }
}
</script>

<style scoped>
.window2-card {
  display: flex;
  flex-direction: column;
  overflow: hidden;
  margin-top: 12px;
  margin-bottom: 12px;
}

.window2-table-container {
  flex: 1;
  min-height: 0;
  overflow-y: auto;
  -webkit-overflow-scrolling: touch;
  padding-right: 6px;
}

/* Mobile ajustar espaçamentos */
@media (max-width: 600px) {
  .window2-card {
    margin-top: 8px;
    margin-bottom: 8px;
  }
}


/* layout base */
.app-layout {
  height: 100vh;
  display: flex;
  flex-direction: column;
}

/* Remove transições do v-window para evitar 'pulo' */
.no-window-transition .v-window-item {
  transition: none !important;
}

/* Main: centraliza conteúdo e evita scroll global */
.scrollable-main {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 0 16px;
  box-sizing: border-box;
}

/* garante espaço para appbar (evita sobreposição) */
.content-offset {
  padding-top: 64px;
}

/* FAB responsivo */
.fab-wrapper {
  display: flex;
  justify-content: flex-end;
}

@media (max-width: 600px) {
  .fab-wrapper {
    justify-content: center !important;
    margin-top: 8px;
  }
}

/* Emissão card */
.emissao-card {
  width: 100%;
  max-height: calc(100vh - 64px - 56px - 48px);
  display: flex;
  flex-direction: column;
  overflow: hidden;
  border-radius: 12px;
}

/* Cabeçalho fixo dentro do card */
.card-header-fixed {
  position: sticky;
  top: 0;
  z-index: 10;
  background: white;
  padding-top: 8px;
  padding-bottom: 8px;
  border-bottom: 1px solid #eee;
}

/* Área rolável dentro do card de emissão */
.card-scroll-area {
  max-height: calc(100vh - 64px - 56px - 120px);
  overflow-y: auto;
  padding: 16px;
  -webkit-overflow-scrolling: touch;
}

/* ajuste visual das mini-cards dentro da lista */
.card-scroll-area .v-card {
  margin-bottom: 12px;
}

/* Card de consulta (window 2) sem scroll interno por padrão, usa availableHeight */
.no-scroll-card {
  display: flex;
  flex-direction: column;
  overflow: visible;
  box-sizing: border-box;
}

/* garante que a tabela não quebre horizontalmente */
.v-data-table {
  overflow-x: auto;
}

/* bottom nav fixo (mantido) */
.v-bottom-navigation {
  position: fixed;
  bottom: 0;
  left: 0;
  right: 0;
  z-index: 2000;
}

/* responsive tweaks */
@media (max-width: 600px) {
  .content-offset {
    padding-top: 56px;
  }

  .emissao-card {
    max-height: calc(100vh - 56px - 56px - 32px);
    padding: 12px;
  }

  .card-scroll-area {
    max-height: calc(100vh - 56px - 56px - 120px);
  }
}
</style>
