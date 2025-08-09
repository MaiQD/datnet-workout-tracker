export const environment = {
  googleClientId: import.meta.env.VITE_GOOGLE_CLIENT_ID || '',
  apiBaseUrl: import.meta.env.VITE_API_BASE_URL || 'http://localhost:7000/api/v1.0',
  isDevelopment: import.meta.env.DEV,
  isProduction: import.meta.env.PROD
}

export const requiredEnvVars = {
  googleClientId: environment.googleClientId || 'VITE_GOOGLE_CLIENT_ID',
  apiBaseUrl: environment.apiBaseUrl || 'VITE_API_BASE_URL'
}
