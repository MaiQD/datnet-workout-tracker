import { apiService } from './api'
import { environment } from '@/config/environment'

export interface GoogleAuthResponse {
  token: string
  userId: string
  email: string
  displayName: string
  roles: string[]
  expiresAt: string
  profilePicture?: string
}

class GoogleAuthService {
  private clientId: string
  private googleAuth: any = null

  constructor() {
    this.clientId = environment.googleClientId
    this.initializeGoogleAuth()
  }

  private initializeGoogleAuth() {
    // Load Google Identity Services
    const script = document.createElement('script')
    script.src = 'https://accounts.google.com/gsi/client'
    script.async = true
    script.defer = true
    script.onload = () => {
      if (window.google) {
        this.googleAuth = window.google.accounts.oauth2.initTokenClient({
          client_id: this.clientId,
          scope: 'openid profile email',
          redirect_uri: window.location.origin,
          callback: this.handleAuthCallback.bind(this)
        })
      }
    }
    document.head.appendChild(script)
  }

  private async handleAuthCallback(response: any) {
    if (response.error) {
      throw new Error(`Google OAuth error: ${response.error}`)
    }

    try {
      // Exchange Google access token for our backend token
      const authResponse = await apiService.post<GoogleAuthResponse>('/auth/google-login', {
        googleToken: response.access_token
      })

      // Store the token
      localStorage.setItem('auth_token', authResponse.token)
      
      return authResponse
    } catch (error) {
      console.error('Failed to authenticate with backend:', error)
      throw error
    }
  }

  async signIn(): Promise<GoogleAuthResponse> {
    if (!this.googleAuth) {
      throw new Error('Google Auth not initialized')
    }

    return new Promise((resolve, reject) => {
      this.googleAuth.callback = (response: any) => {
        if (response.error) {
          reject(new Error(`Google OAuth error: ${response.error}`))
        } else {
          this.handleAuthCallback(response)
            .then(resolve)
            .catch(reject)
        }
      }
      
      this.googleAuth.requestAccessToken()
    })
  }

  async signOut(): Promise<void> {
    try {
      // Get the current token
      const currentToken = localStorage.getItem('auth_token')
      
      if (window.google && window.google.accounts?.oauth2 && currentToken) {
        // Revoke the Google OAuth token
        return new Promise<void>((resolve) => {
          window.google.accounts.oauth2.revoke(currentToken, (response?: any) => {
            if (response?.error) {
              console.warn('Google token revocation failed:', response.error)
              // Don't reject, just log the warning
            }
            
            resolve()
          })
        })
      }
    } catch (error) {
      console.warn('Google signOut error:', error)
      // Don't throw, ensure cleanup continues
    } finally {
      // Always clear local storage regardless of Google API response
      localStorage.removeItem('auth_token')
      localStorage.removeItem('google_token')
    }
  }
}

export const googleAuthService = new GoogleAuthService()
export default googleAuthService
