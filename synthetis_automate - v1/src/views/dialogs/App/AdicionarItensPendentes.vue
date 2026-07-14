<template>
  <v-dialog v-model="dialog" fullscreen transition="dialog-bottom-transition">
    <v-card>
      <v-toolbar color="primary" dark>
        <!-- Corrigido: executar as duas ações corretamente -->
        <v-btn icon="mdi-close" @click="dialog = false"></v-btn>
        <v-toolbar-title class="text-center">Itens Pendentes | {{ relatorio.nome_arquivo }}</v-toolbar-title>
      </v-toolbar>
      <div style="height: 90vh; overflow-y: auto; overflow-x: hidden;" class="mt-5">
        <v-form ref="form">
          <v-row dense class="">
            <template v-for="(p, index) in pendencias" :key="index">
              <v-col cols="12" class="px-4 py-1">
                <v-file-input :label="p.chave" v-model="p.imagem" :rules="[regraObrigatoria]" prepend-icon="mdi-camera"
                  accept="image/*" capture="environment" outlined dense>
                </v-file-input>
              </v-col>
            </template>
          </v-row>
        </v-form>
      </div>
      <v-row class="my-2 px-5">
        <v-col>

        </v-col>
        <v-col class="d-flex justify-end">
          <!-- passar true indica que está finalizando no step de termografia -->
          <v-btn color="primary" @click="enviarImagens">
            Enviar
          </v-btn>
        </v-col>
      </v-row>
    </v-card>
  </v-dialog>
</template>

<script>
export default {
  props: {
    relatorio: { type: Object, required: true },
    modelValue: { type: Boolean, required: true }
  },

  data() {
    return {
      pendencias: [],
    }
  },
  computed: {
    dialog: {
      get() {
        return this.modelValue
      },
      set(v) {
        this.$emit("update:modelValue", v)
      }
    }
  },

  watch: {
    relatorio: {
      immediate: true,
      handler(val) {
        if (!val) return
        try {
          this.pendencias = JSON.parse(val.item_pendente || "[]")
        } catch {
          this.pendencias = []
        }

        // garante que cada pendência terá um campo imagem
        this.pendencias.forEach(p => {
          p.imagem = null
        })
      }
    }
  },

  methods: {
    fechar() {
      this.dialog = false
    },

    // converter File → base64
    fileToBase64(file) {
      return new Promise((resolve, reject) => {
        const reader = new FileReader()
        reader.onload = () => resolve(reader.result)
        reader.onerror = e => reject(e)
        reader.readAsDataURL(file)
      })
    },

    async enviarImagens() {
      const { valid } = await this.$refs.form.validate();
      if (valid) {
        const listaFinal = []
        for (const p of this.pendencias) {
          const registro = {
            chave: p.chave,
            campo: p.campo,
            imagem: null
          }
          if (p.imagem instanceof File) {
            registro.imagem = await this.fileToBase64(p.imagem)
          }

          listaFinal.push(registro)
        }
        // envia para o pai
        this.$emit("enviar-pendencias", {
          relatorio_id: this.relatorio.id,
          itens: listaFinal
        })
      }
    },
    regraObrigatoria(v) {
      return !!v || "Campo obrigatório"
    },
  }
}
</script>
