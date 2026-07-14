import axios from 'axios';

const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL,
  headers: {
    'Content-Type': 'application/x-www-form-urlencoded',
  },
});

// Token em todas as requests
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');

  if (token && token !== "null" && token !== "undefined") {
    config.headers.Authorization = `Bearer ${token}`;
  }

  return config;
});

// Controle de fila de refresh
let isRefreshing = false;
let failedQueue = [];

const processQueue = (error, token = null) => {
  failedQueue.forEach((p) => {
    if (error) p.reject(error);
    else p.resolve(token);
  });
  failedQueue = [];
};

// Interceptor de resposta
api.interceptors.response.use(
  (response) => response,

  async (error) => {
    const originalRequest = error.config;

    // 🔥 IGNORA INTERCEPTOR QUANDO A ROTA É LOGIN
    if (originalRequest.url.includes('/auth/login')) {
      return Promise.reject(error); // deixa erro ser tratado no componente
    }

    // Se 401 e não tentou renovar ainda → tenta refresh
    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;

      const refreshToken = localStorage.getItem('refresh_token');
      if (!refreshToken) {
        localStorage.clear();
        window.location.href = '/';
        return;
      }

      if (isRefreshing) {
        return new Promise((resolve, reject) => {
          failedQueue.push({ resolve, reject });
        }).then((token) => {
          originalRequest.headers.Authorization = `Bearer ${token}`;
          return api(originalRequest);
        });
      }

      isRefreshing = true;

      try {
        const resp = await axios.post(
          'http://192.168.1.94:8000/auth/refresh',
          `refresh_token=${refreshToken}`,
          {
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
          }
        );

        const newAccess = resp.data.access_token;

        localStorage.setItem('token', newAccess);
        api.defaults.headers.Authorization = `Bearer ${newAccess}`;

        processQueue(null, newAccess);

        originalRequest.headers.Authorization = `Bearer ${newAccess}`;
        return api(originalRequest);

      } catch (err) {
        processQueue(err, null);
        localStorage.clear();
        window.location.href = '/';
        return;
      } finally {
        isRefreshing = false;
      }
    }

    return Promise.reject(error);
  }
);

export default api;
