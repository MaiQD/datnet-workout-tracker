<script setup lang="ts">
import { useAuthStore } from '@/stores/auth'
import { ref } from 'vue'
import { useRouter } from 'vue-router'

const authStore = useAuthStore()
const router = useRouter()
const isLoading = ref(false)
const error = ref<string | null>(null)

const handleGoogleLogin = async () => {
  isLoading.value = true
  error.value = null
  
  try {
    await authStore.loginWithGoogle()
    router.push('/dashboard')
  } catch (err: any) {
    error.value = err.message || 'Login failed. Please try again.'
  } finally {
    isLoading.value = false
  }
}
</script>

<template>
  <div class="google-login-form">
    <div class="text-center mb-6">
      <h2 class="text-h5 font-weight-medium mb-2">Welcome to dotFitness</h2>
      <p class="text-body-2 text-medium-emphasis">Sign in to track your workouts and achieve your fitness goals</p>
    </div>

    <v-btn
      size="large"
      block
      color="primary"
      :loading="isLoading"
      :disabled="isLoading"
      @click="handleGoogleLogin"
      class="mb-4"
      prepend-icon="mdi-google"
    >
      {{ isLoading ? 'Signing in...' : 'Sign in with Google' }}
    </v-btn>

    <v-alert
      v-if="error"
      type="error"
      variant="tonal"
      class="mb-4"
    >
      {{ error }}
    </v-alert>

    <div class="text-center">
      <p class="text-body-2 text-medium-emphasis">
        By signing in, you agree to our 
        <a href="#" class="text-primary text-decoration-none">Terms of Service</a> and 
        <a href="#" class="text-primary text-decoration-none">Privacy Policy</a>
      </p>
    </div>
  </div>
</template>

<style scoped>
.google-login-form {
  max-width: 400px;
  margin: 0 auto;
}
</style>
