declare global {
  interface Window {
    google?: {
      accounts: {
        oauth2: {
          initTokenClient: (config: any) => any
          revoke: (token: string, callback: () => void) => void
        }
      }
    }
  }
}

export {}
