import { defineStore } from 'pinia'
import api from '@/service/http.js'

export const useAppStore = defineStore('app', {
  state: () => ({
    modelos: [],
    relatorios: [],
  }),
  getters: {},
  actions: {
    async getModels(user) {
      try {
        const response = await api.get('/app/modelos?equipe=' + user.equipe)
        this.modelos = response.data
        this.error = ''
        return true
      } catch {
        return false
      }
    },
    async extrairVariaveis(doc) {
      try {
        const formData = new FormData()
        formData.append('file', doc)

        const response = await api.post('/app/extrair-variaveis', formData, {
          headers: {
            'Content-Type': 'multipart/form-data',
          },
        })
        return response.data
      } catch (error) {
        console.error('Erro ao extrair variáveis:', error.response?.data || error.message)
        return false
      }
    },
    async adicionarNovoFormulario(titulo, descriçao, modelo_automaçao, doc, termografia) {
      let modelo_automaçao_JSON = JSON.stringify(modelo_automaçao)
      let equipe = JSON.parse(localStorage.getItem('usuario')).equipe

      const formData = new FormData()
      formData.append('titulo', titulo)
      formData.append('equipe', equipe)
      formData.append('descriçao', descriçao)
      formData.append('modelo_automacao', modelo_automaçao_JSON)
      formData.append('documento_modelo', doc) // File
      formData.append('termografia', termografia)

      try {
        const response = await api.post('app/novoModelo', formData, {
          headers: {
            'Content-Type': 'multipart/form-data',
          },
        })
        return response.data
      } catch (error) {
        console.error('Erro ao salvar formulário:', error.response?.data || error.message)
        return false
      }
    },
    async gerarRelatorio(
      chaves,
      pendencias,
      modelo,
      chavePendencia,
      itens_pendentes,
      nomeRelatorio,
    ) {
      const converterParaBase64 = async (file) => {
        if (!file) return null
        return new Promise((resolve, reject) => {
          const reader = new FileReader()
          reader.onload = () => resolve(reader.result)
          reader.onerror = (err) => reject(err)
          reader.readAsDataURL(file)
        })
      }

      // Converte dados
      const dadosConvertidos = {}
      for (const key in chaves) {
        const valor = chaves[key]
        dadosConvertidos[key] = valor instanceof File ? await converterParaBase64(valor) : valor
      }

      // Converte pendências
      const pendenciasConvertidas = await Promise.all(
        pendencias.map(async (p) => ({
          titulo: p.titulo,
          descricao: p.descriçao,
          imagem: p.imagem instanceof File ? await converterParaBase64(p.imagem) : p.imagem,
        })),
      )
      console.log(itens_pendentes)
      try {
        let retorno = null

        await api
          .post(
            'automate/gerar-doc',
            {
              modelo_id: modelo,
              equipamento: 'QPE',
              dados: dadosConvertidos,
              responsavel: JSON.parse(localStorage.getItem('usuario')).usuario,
              pendencias: pendenciasConvertidas,
              chavePendencia: chavePendencia,
              itens_pendentes: itens_pendentes,
              nome_relatorio: nomeRelatorio,
            },
            { headers: { 'Content-Type': 'application/json' } },
          )
          .then((res) => {
            retorno = res.status
          })

        return retorno
      } catch (err) {
        console.error('ERRO:', err.response?.data || err.message)
        return false
      }
    },

    async recuperarRelatórios() {
      try {
        await api
          .get('/app/recuperarRelatorios', {
            headers: {
              Authorization: `Bearer ${localStorage.getItem('token')}`,
            },
          })
          .then((res) => {
            this.relatorios = res.data
          })
      } catch (error) {
        return error
      }
    },
    async baixarRelatorio(idRelatorio) {
      try {
        const response = await api.get('/automate/baixarRelatorio/' + idRelatorio, {
          responseType: 'blob',
        })

        const contentDisposition = response.headers['content-disposition']

        let fileName = 'relatorio.docx'

        if (contentDisposition) {
          const match = contentDisposition.match(/filename="(.+)"/)

          if (match?.[1]) {
            fileName = match[1]
          }
        }

        const url = window.URL.createObjectURL(new Blob([response.data]))

        const link = document.createElement('a')

        link.href = url
        link.download = fileName

        document.body.appendChild(link)

        link.click()

        link.remove()

        window.URL.revokeObjectURL(url)
      } catch (error) {
        console.error('Erro ao baixar relatório:', error)
      }
    },
     async deletarModelo(id) {
      try {
        await api.delete('/app/deleteModelo/'+id)
      } catch (error) {
        console.log(error)
      }
    },
    async deletarRelatorio (id) {
      try {
        await api.delete('/app/deleteRelatorio/'+id)
      } catch (error) {
        console.log(error)
      }
    },
    async enviarItensPendentes(id, itens) {
      try {
        const imagensDict = {}
        for (const item of itens) {
          imagensDict[item.chave] = item.imagem
        }
        const res = await api.post(
          'automate/resolver-itens-pendentes',
          {
            relatorio_id: id,
            imagens: imagensDict,
          },
          {
            headers: { 'Content-Type': 'application/json' },
          },
        )

        return res.data || true // retorna algo útil
      } catch (err) {
        console.error('ERRO AO ENVIAR ITENS PENDENTES:', err.response?.data || err.message)
        return false
      }
    },
  },
})
