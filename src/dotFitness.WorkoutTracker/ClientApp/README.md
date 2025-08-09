# dotFitness Client App

A Vue.js-based frontend for the dotFitness workout tracking application.

## Features

- **Google OAuth Authentication** - Secure login with Google accounts
- **Workout Dashboard** - Track your fitness progress and recent workouts
- **Exercise Management** - Browse and manage your exercise library
- **Progress Tracking** - Monitor your fitness goals and achievements
- **Responsive Design** - Works on desktop and mobile devices

## Setup

### Prerequisites

- Node.js 18+ 
- npm or yarn
- Google OAuth Client ID

### Installation

1. Install dependencies:
   ```bash
   npm install
   ```

2. Create environment configuration:
   Create a `.env` file in the ClientApp directory with:
   ```env
   VITE_GOOGLE_CLIENT_ID=your_google_client_id_here
   VITE_API_BASE_URL=http://localhost:5000/api
   ```

3. Get Google OAuth Client ID:
   - Go to [Google Cloud Console](https://console.cloud.google.com/)
   - Create a new project or select existing one
   - Enable Google+ API
   - Create OAuth 2.0 credentials
   - Add authorized origins: `http://localhost:3000`, `http://localhost:5050`
   - Add authorized redirect URIs: `http://localhost:3000`, `http://localhost:5050`

## Google OAuth Setup

To enable Google login functionality, you need to:

1. **Create a Google OAuth 2.0 Client ID:**
   - Go to [Google Cloud Console](https://console.cloud.google.com/)
   - Create a new project or select an existing one
   - Enable the Google+ API
   - Go to "Credentials" → "Create Credentials" → "OAuth 2.0 Client IDs"
   - Set Application Type to "Web application"
   - Add authorized JavaScript origins:
     - `http://localhost:5173` (for development - Vite default)
     - `http://localhost:7000` (for backend)
   - Add authorized redirect URIs:
     - `http://localhost:5173` (for SPA OAuth flow)
   - Copy the Client ID

2. **Create Environment Configuration:**
   Create a `.env` file in the ClientApp directory:
   ```bash
   # Google OAuth Configuration
   VITE_GOOGLE_CLIENT_ID=your_google_client_id_here
   
   # API Configuration
   VITE_API_BASE_URL=http://localhost:7000/api/v1.0
   
   # Frontend URL (for reference)
   # Dev server runs on: http://localhost:5173
   
   # Environment
   NODE_ENV=development
   ```

3. **Replace `your_google_client_id_here`** with the actual Client ID from step 1.

## Development

```bash
# Install dependencies
npm install

# Start development server (runs on http://localhost:5173)
npm run dev

# Build for production
npm run build

# Preview production build (runs on http://localhost:5050)
npm run preview
```

**Note**: The development server runs on port 5173 (Vite default), not port 3000. The preview server runs on port 5050.

## Project Structure

```
src/
├── components/
│   ├── auth/           # Authentication components
│   ├── dashboards/     # Dashboard widgets
│   └── shared/         # Shared UI components
├── config/             # Configuration files
├── layouts/            # Layout components
├── router/             # Vue Router configuration
├── services/           # API and external services
├── stores/             # Pinia state management
├── types/              # TypeScript type definitions
├── views/              # Page components
└── main.ts             # Application entry point
```

## Authentication Flow

1. User clicks "Sign in with Google"
2. Google OAuth popup opens
3. User authenticates with Google
4. Google returns access token
5. Frontend sends token to backend
6. Backend validates and returns user data + JWT
7. User is logged in and redirected to dashboard

## API Integration

The app integrates with the dotFitness backend API for:
- User authentication and management
- Exercise and workout data
- Progress tracking
- Goal management

## Styling

- **Vuetify 3** - Material Design component library
- **SCSS** - Custom styling and theming
- **Responsive** - Mobile-first design approach

## Browser Support

- Chrome 90+
- Firefox 88+
- Safari 14+
- Edge 90+

## Contributing

1. Follow the established code style
2. Use TypeScript for type safety
3. Write meaningful commit messages
4. Test your changes thoroughly

## License

This project is part of the dotFitness workout tracking system.