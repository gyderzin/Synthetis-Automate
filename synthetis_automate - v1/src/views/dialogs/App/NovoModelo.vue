<!-- eslint-disable vue/valid-v-slot -->
<template>
  <div class="text-center pa-4">
    <v-dialog v-model="dialog" transition="dialog-bottom-transition" fullscreen>
      <v-card>
        <v-toolbar color="primary">
          <v-btn icon="mdi-close" @click="closeDialog"></v-btn>

          <v-toolbar-title class="text-center">Novo Modelo de Formulário</v-toolbar-title>
        </v-toolbar>
        <!-- INICIO STEP ANEXO MODELO -->
        <v-stepper v-model="step" :items="items" hide-actions>
          <template v-slot:[`item.1`]>
            <v-form ref="form">
              <v-text-field label="Nome do Formulário" placeholder="Quadro de Parada de Emergência"
                variant="solo-filled" prepend-icon="mdi-text"
                :rules="[value => !!value || 'Preencha o nome do formulário']" class="mb-2"
                v-model="nomeFormulario"></v-text-field>
              <v-text-field label="Breve Descrição" placeholder="Manutenção Preventiva - QPE" variant="solo-filled"
                prepend-icon="mdi-subtitles" :rules="[value => !!value || 'Preencha uma descrição para o formulário']"
                class="mb-2" v-model="descriçaoFormulario"></v-text-field>
              <v-file-input v-model="docModelo" :rules="rulesFile" accept=".docx" label="Modelo de relatório"
                placeholder="Carregue seu modelo" prepend-icon="mdi-file" variant="solo-filled" required></v-file-input>
              <v-switch v-model="termografia" color="primary" inset label="Possui termografia?"></v-switch>
            </v-form>
            <v-row>
              <v-col class="my-2">
                <v-btn variant="text" disabled>
                  Voltar
                </v-btn>
              </v-col>
              <v-col class="d-flex justify-end my-2">
                <v-btn variant="flat" :loading="loadingFile" color="primary" @click="enviarDoc" type="submit">
                  AVANÇAR
                </v-btn>
              </v-col>
            </v-row>
          </template>
          <!-- FINAL STEP ANEXO MODELO -->

          <!-- INICIO STEP CRIAÇAO FORMULÁRIO -->
          <template v-slot:[`item.2`]>
            <div style="max-height: 70vh; overflow-y: auto; padding: 16px;">
              <v-row>
                <v-col cols="6">
                  <v-list lines="one">
                    <v-list-subheader>Chaves para automaçao</v-list-subheader>

                    <v-list-item v-for="chave in chavesAutomaçao" :key="chave" :title="chave">
                      <template v-slot:prepend>
                        <v-avatar color="grey-lighten-1">
                          <v-icon color="white">mdi-information</v-icon>
                        </v-avatar>
                      </template>

                      <template v-slot:append>
                        <v-btn color="primary" icon="mdi-chevron-right" variant="text"
                          @click="openDialogAddKey(chave)"></v-btn>
                      </template>
                    </v-list-item>
                  </v-list>
                </v-col>
                <v-divider vertical></v-divider>
                <v-col cols="6">
                  <v-list lines="one">
                    <v-list-subheader>Campos de Formulário</v-list-subheader>
                    <v-list-item v-for="chaveForm in chavesAutomaçaoForm" :key="chaveForm" :title="chaveForm.chave"
                      :subtitle="chaveForm.campo">
                      <template v-slot:prepend>
                        <v-avatar color="primary">
                          <v-icon color="white" v-if="chaveForm.campo == 'texto'">
                            mdi-message-processing
                          </v-icon>
                          <v-icon color="white" v-else-if="chaveForm.campo == 'imagem'">
                            mdi-image
                          </v-icon>
                           <v-icon color="white" v-else-if="chaveForm.campo == 'termografia'">
                            mdi-thermometer
                          </v-icon>
                          <v-icon color="white" v-else-if="chaveForm.campo == 'selecao'">
                            mdi-checkbox-multiple-marked
                          </v-icon>
                          <v-icon color="white" v-else-if="chaveForm.campo == 'data'">
                            mdi-calendar
                          </v-icon>
                          <v-icon color="white" v-else-if="chaveForm.campo == 'técnico'">
                            mdi-account-hard-hat
                          </v-icon>
                          <v-icon color="white" v-else-if="chaveForm.campo == 'pendências'">
                            mdi-alert
                          </v-icon>
                        </v-avatar>
                      </template>
                      <template v-slot:append>
                        <v-btn color="primary" icon="mdi-pencil" variant="text"
                          @click="openDialogEditKey(chaveForm)"></v-btn>
                      </template>
                    </v-list-item>
                  </v-list>
                </v-col>
              </v-row>
            </div>
            <v-row>
              <v-col class="my-2">
                <v-btn variant="text" @click="resetCriaçao">
                  Voltar
                </v-btn>
              </v-col>
              <v-col class="d-flex justify-end my-2">
                <v-btn variant="flat"
                  :disabled="chavesAutomaçao.length != null && chavesAutomaçao.length != 0 ? true : false"
                  @click="enviarNovoFormulário" :loading="loadingSendForm" color="primary" type="submit">
                  CRIAR FORMULÁRIO
                </v-btn>
              </v-col>
            </v-row>
          </template>
        </v-stepper>
      </v-card>
      <!-- FINAL STEP CRIAÇAO FORMULÁRIO -->

      <!-- INICIO DIALOG ADD KEY -->
      <v-dialog v-model="dialogAddKey" width="auto" persistent="">
        <v-card max-width="600" prepend-icon="mdi-form-select" :title="'Novo campo de formulário - ' + key">
          <v-form ref="formAddKey" class="ma-0">
            <v-radio-group class="mx-2 my-0" inline v-model="keySelect" :rules="[v => !!v || 'Selecione uma opção.']">
              <template v-slot:label>
                Defina o tipo de dado que será preenchido para a chave
                "<span class="font-weight-bold">{{ key }}</span>"
              </template>

              <v-radio label="Texto" value="texto" inline></v-radio>
              <v-radio label="Imagem" value="imagem" inline></v-radio>
              <v-radio v-if="termografia == true" label="Termografia" value="termografia" inline></v-radio>
              <v-radio label="Seleção" value="selecao" inline></v-radio>
              <v-radio label="Data" value="data" inline></v-radio>
              <v-radio label="Técnico Resp." value="técnico" inline></v-radio>
              <v-radio label="Pendências" value="pendências" inline></v-radio>
            </v-radio-group>
            <v-divider color="info" class="my-2 border-opacity-25"></v-divider>
            <v-text-field label="Texto" model-value="Exemplo de campo de texto" variant="outlined" disabled
              class="mx-5 mt-5" prepend-icon="mdi-message-processing"></v-text-field>
            <v-file-input class="mx-5" disabled label="Exemplo de campo de arquivo (adicionar imagens)"
              variant="outlined"></v-file-input>
            <v-radio-group label="Exemplo de campo de seleçao" disabled>
              <v-radio label="Conforme"></v-radio>
              <v-radio label="Nao conforme"></v-radio>
            </v-radio-group>
          </v-form>
          <template v-slot:actions>
            <v-row class="ma-0">
              <v-col>
                <v-btn class="ms-auto" text="VOLTAR" @click="dialogAddKey = false"></v-btn>
              </v-col>
              <v-col class="d-flex justify-end">
                <v-btn class="ms-auto" text="ADICIONAR" color="primary" @click="addKey"></v-btn>
              </v-col>
            </v-row>
          </template>
        </v-card>
      </v-dialog>
      <!-- FINAL DIALOG ADD KEY -->

      <!-- INICIO DIALOG EDIT KEY -->
      <v-dialog v-model="dialogEditKey" width="auto" persistent="">
        <v-card max-width="600" prepend-icon="mdi-form-select" :title="'Editar campo de formulário - ' + keyEdit.chave">
          <v-form ref="formEditKey" class="ma-0">
            <v-radio-group class="mx-2 my-0" inline v-model="keySelectEdit"
              :rules="[v => !!v || 'Selecione uma opção.']">
              <template v-slot:label>
                Defina o tipo de dado que será preenchido para a chave
                "<span class="font-weight-bold">{{ keyEdit.chave }}</span>"
              </template>

              <v-radio label="Texto" value="texto" inline></v-radio>
              <v-radio label="Imagem" value="imagem" inline></v-radio>
              <v-radio v-if="termografia == true" label="Termografia" value="termografia" inline></v-radio>
              <v-radio label="Seleção" value="selecao" inline></v-radio>
              <v-radio label="Data" value="data" inline></v-radio>
              <v-radio label="Técnico Resp." value="técnico" inline></v-radio>
              <v-radio label="Pendências" value="pendências" inline></v-radio>
            </v-radio-group>
            <v-divider color="info" class="my-2 border-opacity-25"></v-divider>
            <v-text-field label="Texto" model-value="Exemplo de campo de texto" variant="outlined" disabled
              class="mx-5 mt-5" prepend-icon="mdi-message-processing"></v-text-field>
            <v-file-input class="mx-5" disabled label="Exemplo de campo de arquivo (adicionar imagens)"
              variant="outlined"></v-file-input>
            <v-radio-group label="Exemplo de campo de seleçao" disabled>
              <v-radio label="Conforme"></v-radio>
              <v-radio label="Nao conforme"></v-radio>
            </v-radio-group>
          </v-form>
          <template v-slot:actions>
            <v-row class="ma-0">
              <v-col>
                <v-btn class="ms-auto" text="VOLTAR" @click="dialogEditKey = false"></v-btn>
              </v-col>
              <v-col class="d-flex justify-end">
                <v-btn class="ms-auto" text="ATUALIZAR" color="primary" @click="editKey"></v-btn>
              </v-col>
            </v-row>
          </template>
        </v-card>
      </v-dialog>
    </v-dialog>
  </div>
</template>

<script>
// oxlint-disable no-unused-expressions
export default {
  name: 'NovoModelo',
  props: {
    modelValue: {
      type: Boolean,
      required: true
    },
    app: {
      required: true
    },
    alert: {
      required: true
    }
  },
  data() {
    return {
      shipping: 0,
      step: 1,
      items: [
        'Anexo de Documento Modelo',
        'Criaçao de Formulário'
      ],
      termografia: false,
      docModelo: null,
      nomeFormulario: null,
      descriçaoFormulario: null,
      rulesFile: [
        v => v instanceof File || (v && v.length > 0) || 'Carregue o arquivo para prosseguir.',
      ],
      loadingFile: false,
      loadingSendForm: false,
      chavesAutomaçao: [],
      copyChavesAutomaçao: [],
      chavesAutomaçaoForm: [],
      dialogAddKey: false,
      dialogEditKey: false,
      keySelect: null,
      keySelectEdit: null,
      key: null,
      keyEdit: null
    }
  },
  methods: {
    resetFormulario() {
      this.step = 1
      this.docModelo = null
      this.nomeFormulario = null
      this.descriçaoFormulario = null
      this.loadingFile = false
      this.loadingSendForm = false
      this.chavesAutomaçao = []
      this.copyChavesAutomaçao = []
      this.chavesAutomaçaoForm = []
      this.dialogAddKey = false
      this.dialogEditKey = false
      this.keySelect = null
      this.keySelectEdit = null
      this.key = null
      this.keyEdit = null
    },
    closeDialog() {
      this.dialog = false
      this.resetFormulario()
    },
    async enviarDoc() {
      const { valid } = await this.$refs.form.validate()
      if (valid) {
        this.loadingFile = true
        await this.app.extrairVariaveis(this.docModelo).then((res) => {
          this.loadingFile = false
          this.chavesAutomaçao = res.variaveis
          this.copyChavesAutomaçao = [...res.variaveis]
          this.step++
        })
      }
    },
    openDialogAddKey(key) {
      this.key = key
      this.dialogAddKey = true
    },
    openDialogEditKey(key) {
      this.keyEdit = key
      this.keySelectEdit = key.campo,
        this.dialogEditKey = true
    },
    async addKey() {
      const { valid } = await this.$refs.formAddKey.validate()
      if (valid) {
        this.chavesAutomaçaoForm.push({
          "chave": this.key,
          "campo": this.keySelect
        })
        let indiciceKey = this.chavesAutomaçao.indexOf(this.key)
        this.chavesAutomaçao.splice(indiciceKey, 1)
        this.key = null
        this.keySelect = null
        this.dialogAddKey = this.false
      }
    },
    async editKey() {
      const { valid } = await this.$refs.formEditKey.validate()
      if (valid) {
        for (let x = 0; x < this.chavesAutomaçaoForm.length; x++) {
          if (this.chavesAutomaçaoForm[x].chave == this.keyEdit.chave) {
            this.chavesAutomaçaoForm[x] = {
              "chave": this.keyEdit.chave,
              "campo": this.keySelectEdit
            }
          }
        }
        this.dialogEditKey = false
      }
    },
    resetCriaçao() {
      this.chavesAutomaçao = [],
        this.chavesAutomaçaoForm = [],
        this.step--
    },
    async enviarNovoFormulário() {
      if (this.chavesAutomaçao.length === 0) {
        this.loadingSendForm = true
        const ordenado = [...this.copyChavesAutomaçao].map(chaveOriginal => {
          return this.chavesAutomaçaoForm.find(
            item => item.chave.trim().toLowerCase() === chaveOriginal.trim().toLowerCase()
          );
        }).filter(item => item);

        await this.app.adicionarNovoFormulario(
          this.nomeFormulario,
          this.descriçaoFormulario,
          ordenado,
          this.docModelo,
          this.termografia
        ).then(() => {
          this.app.getModels(JSON.parse(localStorage.getItem('usuario'))).then(() => {
            this.loadingSendForm = false
            this.alert.callAlert({
              type: 'success',
              title: 'Formulário criado com sucesso!',
            })
            this.closeDialog()
          })
        })
      }
    }
  },
  computed: {
    dialog: {
      get() {
        return this.modelValue
      },
      set(val) {
        this.$emit('update:modelValue', val)
      }
    }
  }
}
</script>
