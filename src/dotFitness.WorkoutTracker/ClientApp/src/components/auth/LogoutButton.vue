<template>
  <v-btn
    :variant="variant"
    :color="color"
    :size="size"
    :loading="isLoggingOut"
    :disabled="isLoggingOut"
    @click="confirmLogout"
  >
    <v-icon v-if="showIcon" icon="mdi-logout" class="mr-2"></v-icon>
    <span v-if="isLoggingOut">Logging out...</span>
    <span v-else>{{ buttonText }}</span>
  </v-btn>

  <!-- Logout Confirmation Dialog -->
  <v-dialog v-model="showLogoutConfirm" max-width="400">
    <v-card>
      <v-card-title class="text-h6">
        Confirm Logout
      </v-card-title>
      <v-card-text>
        Are you sure you want to logout? You'll need to sign in again to access your account.
      </v-card-text>
      <v-card-actions>
        <v-spacer></v-spacer>
        <v-btn
          variant="text"
          @click="showLogoutConfirm = false"
          :disabled="isLoggingOut"
        >
          Cancel
        </v-btn>
        <v-btn
          color="primary"
          @click="handleLogout"
          :loading="isLoggingOut"
          :disabled="isLoggingOut"
        >
          Logout
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { useRouter } from 'vue-router'

interface Props {
  variant?: 'text' | 'outlined' | 'flat' | 'elevated' | 'tonal' | 'plain'
  color?: string
  size?: 'small' | 'default' | 'large' | 'x-large'
  buttonText?: string
  showIcon?: boolean
  redirectTo?: string
}

const props = withDefaults(defineProps<Props>(), {
  variant: 'outlined',
  color: 'primary',
  size: 'default',
  buttonText: 'Logout',
  showIcon: true,
  redirectTo: '/auth/login'
})

const authStore = useAuthStore()
const router = useRouter()

const showLogoutConfirm = ref(false)
const isLoggingOut = computed(() => authStore.isLoggingOut)

const confirmLogout = () => {
  showLogoutConfirm.value = true
}

const handleLogout = async () => {
  showLogoutConfirm.value = false
  
  try {
    await authStore.logout()
    // Redirect after successful logout
    router.push(props.redirectTo)
  } catch (error) {
    console.error('Logout failed:', error)
    // Even if logout fails, redirect to login
    router.push(props.redirectTo)
  }
}
</script>
