<template>
  <div class="alert-wrapper">
    <transition
      name="custom-classes"
      enter-active-class="animate__animated animate__backInDown"
      leave-active-class="animate__animated animate__backOutUp"
      mode="out-in"
    >
      <v-alert
        v-if="state"
        v-model="state"
        :text="text"
        :title="title"
        :type="type"
        density="compact"
        variant="elevated"
      ></v-alert>
    </transition>
  </div>
</template>

<script>
import { useAlertStore } from '@/stores/alert';
export default {
   setup() {
    const alertState = useAlertStore();
    return { alertState };
  },
  computed: {
    state() {
      return this.alertState.getStateAlert
    },
    text() {
      return this.alertState.getText
    },
    title() {
      return this.alertState.getTitle
    },
    type() {
      return this.alertState.getType
    },
  },
  watch: {
    state(newVal) {
     if(newVal) {
      setTimeout(() => {
        this.alertState.closeAlert()
      }, 4000)
     }
    }
  }
}
</script>

<style scoped>
.alert-wrapper {
  position: fixed;
  top: 24px;
  left: 50%;
  transform: translateX(-50%);
  z-index: 9999;
  width: 100%;
  max-width: 500px;
  padding: 0 1rem;
  pointer-events: none; /* Para não atrapalhar o clique nos elementos abaixo */
}

.alert-wrapper > * {
  pointer-events: all; /* Para permitir interação com o v-alert */
}
</style>
