import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { googleAuthService } from '@/services/googleAuth'
import type { GoogleAuthResponse } from '@/services/googleAuth'

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

  const loginWithGoogle = async () => {
    isLoading.value = true
    try {
      const response: GoogleAuthResponse = await googleAuthService.signIn()
      
      // Create user object from response
      user.value = {
        id: response.userId,
        email: response.email,
        name: response.displayName,
        roles: response.roles
      }
      
      token.value = response.token
      localStorage.setItem('auth_token', response.token)
      
      return response
    } catch (error) {
      console.error('Google login failed:', error)
      throw error
    } finally {
      isLoading.value = false
    }
  }

  const logout = async () => {
    try {
      await googleAuthService.signOut()
    } catch (error) {
      console.error('Logout error:', error)
    } finally {
      user.value = null
      token.value = null
      localStorage.removeItem('auth_token')
    }
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

  // Initialize auth state from localStorage
  const initializeAuth = () => {
    const storedToken = localStorage.getItem('auth_token')
    if (storedToken) {
      token.value = storedToken
      // TODO: Validate token and fetch user info
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
    loginWithGoogle,
    logout,
    updateProfile,
    initializeAuth
  }
})
