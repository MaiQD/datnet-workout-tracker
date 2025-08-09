import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

export interface User {
  id: string
  email: string
  name: string
  roles: string[]
  profilePicture?: string
}

export const useAuthStore = defineStore('auth', () => {
  const user = ref<User | null>(null)
  const token = ref<string | null>(null)
  const isLoading = ref(false)

  const isAuthenticated = computed(() => !!token.value)
  const isAdmin = computed(() => user.value?.roles.includes('Admin') ?? false)
  const isPT = computed(() => user.value?.roles.includes('PT') ?? false)
  const isUser = computed(() => user.value?.roles.includes('User') ?? false)

  const login = async (googleToken: string) => {
    isLoading.value = true
    try {
      // TODO: Implement API call to backend
      // const response = await authApi.loginWithGoogle(googleToken)
      // user.value = response.user
      // token.value = response.token
    } catch (error) {
      console.error('Login failed:', error)
      throw error
    } finally {
      isLoading.value = false
    }
  }

  const logout = () => {
    user.value = null
    token.value = null
  }

  const updateProfile = async (profileData: Partial<User>) => {
    if (!user.value) return
    
    try {
      // TODO: Implement API call to backend
      // const response = await userApi.updateProfile(profileData)
      // user.value = { ...user.value, ...response }
    } catch (error) {
      console.error('Profile update failed:', error)
      throw error
    }
  }

  return {
    user,
    token,
    isLoading,
    isAuthenticated,
    isAdmin,
    isPT,
    isUser,
    login,
    logout,
    updateProfile
  }
})
