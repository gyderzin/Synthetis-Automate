
import { createRouter, createWebHistory } from 'vue-router'
import PageLogin from  '@/components/Login/PageLogin.vue'
import HomePage from '@/components/App/HomePage.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      component: PageLogin
    },
    {
      path: '/home',
      component: HomePage
    },
  ],
})
router.beforeEach((to, from, next) => {
  const token = localStorage.getItem('token');

  // Se está tentando acessar qualquer rota diferente da '/' e não tem token, redireciona para login
  if (to.path !== '/' && !token) {
    console.log(to.fullPath)
    return next({ path: '/' });
  }

  // Se já tem token e tenta ir para '/', redireciona para /home
  if (to.path === '/' && token) {
    return next({ path: '/home' });
  }

  // Libera navegação normalmente
  return next();
});


export default router
