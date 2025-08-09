declare global {
  interface Window {
    google: {
      accounts: {
        oauth2: {
          initTokenClient: (config: any) => any
          revoke: (token: string, callback: (response?: any) => void) => void
        }
        id: {
          initialize: (config: any) => any
          renderButton: (element: HTMLElement, options: any) => void
          prompt: () => void
          disableAutoSelect: () => void
        }
      }
    }
  }
}

export {}
