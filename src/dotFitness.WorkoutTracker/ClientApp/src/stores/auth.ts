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
  const isLoggingOut = ref(false)

  const isAuthenticated = computed(() => !!token.value)
  const isAdmin = computed(() => user.value?.roles.includes('Admin') ?? false)
  const isPT = computed(() => user.value?.roles.includes('PT') ?? false)
  const isUser = computed(() => user.value?.roles.includes('User') ?? false)

  const loginWithGoogle = async () => {
    isLoading.value = true
    try {
      const response: GoogleAuthResponse = await googleAuthService.signIn()
      
      // Create user object from response
      const userData = {
        id: response.userId,
        email: response.email,
        name: response.displayName,
        roles: response.roles,
        profilePicture: response.profilePicture
      }
      
      user.value = userData
      token.value = response.token
      
      // Store auth data in localStorage for persistence
      localStorage.setItem('auth_token', response.token)
      localStorage.setItem('auth_user', JSON.stringify(userData))
      
      return response
    } catch (error) {
      console.error('Google login failed:', error)
      throw error
    } finally {
      isLoading.value = false
    }
  }

  const logout = async () => {
    if (isLoggingOut.value) return // Prevent multiple logout calls
    
    isLoggingOut.value = true
    try {
      // Revoke Google OAuth token
      await googleAuthService.signOut()
      
      // Clear local state
      user.value = null
      token.value = null
      
      // Clear all auth-related data from localStorage
      localStorage.removeItem('auth_token')
      localStorage.removeItem('auth_user')
      localStorage.removeItem('google_token')
      
      // Clear any other auth-related data
      sessionStorage.clear()
      
      console.log('User logged out successfully')
    } catch (error) {
      console.error('Logout error:', error)
      // Even if Google logout fails, clear local state
      user.value = null
      token.value = null
      localStorage.removeItem('auth_token')
      localStorage.removeItem('google_token')
      sessionStorage.clear()
    } finally {
      isLoggingOut.value = false
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
    const storedUser = localStorage.getItem('auth_user')
    
    if (storedToken && storedUser) {
      try {
        token.value = storedToken
        user.value = JSON.parse(storedUser)
      } catch (error) {
        console.error('Failed to restore auth state:', error)
        // Clear corrupted data
        localStorage.removeItem('auth_token')
        localStorage.removeItem('auth_user')
      }
    }
  }

  // Check if token is expired
  const isTokenExpired = (): boolean => {
    if (!token.value) return true
    
    try {
      // Decode JWT token to check expiration
      const payload = JSON.parse(atob(token.value.split('.')[1]))
      const expirationTime = payload.exp * 1000 // Convert to milliseconds
      return Date.now() >= expirationTime
    } catch {
      return true // If token can't be decoded, consider it expired
    }
  }

  // Auto-logout if token is expired
  const checkTokenExpiration = () => {
    if (isTokenExpired()) {
      console.log('Token expired, logging out user')
      logout()
    }
  }

  return {
    user,
    token,
    isLoading,
    isLoggingOut,
    isAuthenticated,
    isAdmin,
    isPT,
    isUser,
    loginWithGoogle,
    logout,
    updateProfile,
    initializeAuth,
    isTokenExpired,
    checkTokenExpiration
  }
})
