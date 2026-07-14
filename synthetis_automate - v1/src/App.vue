<template>
  <div>
    <transition name="slide" mode="out-in">
      <RouterView />
    </transition>

    <alert-box class="alert-overlay" />
  </div>
</template>

<script setup>
import { RouterView } from 'vue-router'
import AlertBox from './views/AlertBox.vue';
</script>

<style>
/* Importando as fontes */
@import url('https://fonts.googleapis.com/css2?family=DM+Serif+Display&family=Inter:wght@400;500;600&display=swap');

/* Aplicando as fontes globais */
body {
  margin: 0;
  padding: 0;
  background-color: #f9f9f9;
  color: #222;
  font-family: 'Inter', sans-serif;
  line-height: 1.6;
}

/* Estilizando títulos com fonte elegante */
h1,
h2,
h3,
h4,
h5,
h6 {
  font-family: 'DM Serif Display', serif;
  font-weight: normal;
  margin: 0.5em 0;
  color: #111;
}

/* Estilizando botões (opcional) */
button {
  font-family: 'Inter', sans-serif;
  font-weight: 500;
}

/* Estilização para inputs também usarem Inter */
input,
textarea,
select {
  font-family: 'Inter', sans-serif;
}

.centralizar-div {
  display: flex;
  justify-content: center;
}
.alert-overlay {
  position: fixed;
  top: 20px;
  left: 50%;
  transform: translateX(-50%);
  z-index: 9999;
  pointer-events: none; /* evita atrapalhar cliques abaixo */
}
.page-container {
  position: relative;
  overflow: hidden; /* importante para esconder a tela que sai */
  width: 100%;
  height: 100%;
}

/* Animação de slide simples */
.slide-enter-from {
  transform: translateX(100%);
  opacity: 0;
}

.slide-enter-to {
  transform: translateX(0);
  opacity: 1;
}

.slide-leave-from {
  transform: translateX(0);
  opacity: 1;
}

.slide-leave-to {
  transform: translateX(-20%);
  opacity: 0;
}

.slide-enter-active,
.slide-leave-active {
  transition: all 1s ease;
  position: absolute;
  width: 100%;
}


</style>
