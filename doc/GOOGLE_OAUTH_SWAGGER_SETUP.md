# Google OAuth with Swagger Setup Guide

This guide explains how to configure Google OAuth authentication with Swagger UI in the dotFitness API.

## Overview

The dotFitness API now supports Google OAuth authentication through Swagger UI, allowing you to:
- Authenticate with Google directly from the Swagger interface
- Test protected endpoints with OAuth2 tokens
- Use both Bearer token and OAuth2 authentication methods

## Prerequisites

1. **Google Cloud Console Project**: You need a Google Cloud project with OAuth2 credentials
2. **OAuth2 Client ID and Secret**: Generated from Google Cloud Console
3. **Authorized Redirect URIs**: Configured in Google Cloud Console

## Step 1: Google Cloud Console Setup

### 1.1 Create or Select a Project
1. Go to [Google Cloud Console](https://console.cloud.google.com/)
2. Create a new project or select an existing one
3. Enable the Google+ API (if not already enabled)

### 1.2 Configure OAuth Consent Screen
1. Navigate to **APIs & Services** > **OAuth consent screen**
2. Choose **External** user type (unless you have a Google Workspace)
3. Fill in the required information:
   - App name: `dotFitness API`
   - User support email: Your email
   - Developer contact information: Your email
4. Add scopes:
   - `openid`
   - `email`
   - `profile`
5. Add test users (your email addresses)

### 1.3 Create OAuth2 Credentials
1. Navigate to **APIs & Services** > **Credentials**
2. Click **Create Credentials** > **OAuth 2.0 Client IDs**
3. Choose **Web application**
4. Configure authorized redirect URIs:
   - `https://localhost:7001/swagger/oauth2-redirect.html` (Development)
   - `https://your-production-domain.com/swagger/oauth2-redirect.html` (Production)
5. Note down the **Client ID** and **Client Secret**

## Step 2: API Configuration

### 2.1 Update Configuration Files

**Development (`appsettings.Development.json`):**
```json
{
  "GoogleOAuth": {
    "ClientId": "your-actual-google-client-id",
    "ClientSecret": "your-actual-google-client-secret",
    "RedirectUri": "https://localhost:7001/swagger/oauth2-redirect.html"
  }
}
```

**Production (`appsettings.json`):**
```json
{
  "GoogleOAuth": {
    "ClientId": "your-actual-google-client-id",
    "ClientSecret": "your-actual-google-client-secret",
    "RedirectUri": "https://your-production-domain.com/swagger/oauth2-redirect.html"
  }
}
```

### 2.2 Environment Variables (Recommended for Production)
```bash
export GoogleOAuth__ClientId="your-actual-google-client-id"
export GoogleOAuth__ClientSecret="your-actual-google-client-secret"
export GoogleOAuth__RedirectUri="https://your-production-domain.com/swagger/oauth2-redirect.html"
```

## Step 3: Using Swagger with Google OAuth

### 3.1 Access Swagger UI
1. Start your API: `dotnet run`
2. Navigate to: `https://localhost:7001` (Swagger UI is now served at root)
3. You'll see the Swagger interface with OAuth2 authentication options

### 3.2 Authenticate with Google
1. Click the **Authorize** button (lock icon) in Swagger UI
2. Choose **OAuth2** authentication
3. Click **Authorize**
4. You'll be redirected to Google's OAuth consent screen
5. Sign in with your Google account
6. Grant permissions for the requested scopes
7. You'll be redirected back to Swagger UI with an access token

### 3.3 Test Protected Endpoints
1. After authentication, you can test any protected endpoint
2. Swagger will automatically include the OAuth token in requests
3. You can also manually enter a Bearer token if you have one

## Step 4: Authentication Flow

### 4.1 OAuth2 Flow (Swagger UI)
```
1. User clicks "Authorize" in Swagger UI
2. Swagger redirects to Google OAuth
3. User authenticates with Google
4. Google redirects back with authorization code
5. Swagger exchanges code for access token
6. Swagger includes token in API requests
```

### 4.2 Bearer Token Flow (Manual)
```
1. User authenticates via POST /api/v1/auth/google-login
2. API returns JWT token
3. User includes token in Authorization header
4. API validates JWT token
```

## Step 5: Testing Authentication

### 5.1 Test OAuth2 Flow
```http
### Test OAuth2 Authentication
GET https://localhost:7001/api/v1/users/profile
Authorization: Bearer {{oauth2_token}}
```

### 5.2 Test Bearer Token Flow
```http
### Login with Google
POST https://localhost:7001/api/v1/auth/google-login
Content-Type: application/json

{
  "googleToken": "your-google-id-token"
}

### Use returned JWT token
GET https://localhost:7001/api/v1/users/profile
Authorization: Bearer {{jwt_token}}
```

## Security Considerations

### 5.1 Development vs Production
- **Development**: Use localhost redirect URIs
- **Production**: Use your actual domain
- **Client Secret**: Never commit to source control
- **HTTPS**: Always use HTTPS in production

### 5.2 Token Security
- JWT tokens expire after 24 hours (configurable)
- OAuth2 tokens have their own expiration
- Store tokens securely in client applications
- Implement token refresh mechanisms

### 5.3 CORS Configuration
The API is configured with permissive CORS for development:
```csharp
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});
```

For production, configure specific origins:
```csharp
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("https://your-frontend-domain.com")
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});
```

## Troubleshooting

### Common Issues

1. **"Invalid redirect_uri" error**
   - Ensure redirect URI in Google Console matches your configuration
   - Check for trailing slashes or protocol mismatches

2. **"Client ID not found" error**
   - Verify Client ID is correct in configuration
   - Ensure OAuth consent screen is configured

3. **CORS errors**
   - Check CORS configuration
   - Verify frontend origin is allowed

4. **Token validation errors**
   - Check JWT settings in configuration
   - Verify token expiration

### Debug Steps
1. Check API logs for authentication errors
2. Verify Google OAuth configuration
3. Test with Postman or curl first
4. Check browser developer tools for redirect issues

## API Endpoints

### Authentication Endpoints
- `POST /api/v1/auth/google-login` - Google OAuth login
- `GET /api/v1/users/profile` - Get user profile (protected)
- `PUT /api/v1/users/profile` - Update user profile (protected)

### Protected Endpoints
All endpoints except `/api/v1/auth/google-login` require authentication.

## Next Steps

1. **Frontend Integration**: Implement Google OAuth in your frontend application
2. **Token Refresh**: Implement automatic token refresh
3. **Role-Based Access**: Configure role-based authorization
4. **Audit Logging**: Add authentication audit logs
5. **Rate Limiting**: Implement rate limiting for auth endpoints

## Resources

- [Google OAuth2 Documentation](https://developers.google.com/identity/protocols/oauth2)
- [Swagger OAuth2 Documentation](https://swagger.io/docs/specification/authentication/oauth2/)
- [ASP.NET Core Authentication](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/)
- [JWT Token Security](https://auth0.com/blog/a-look-at-the-latest-draft-for-jwt-bcp/) 