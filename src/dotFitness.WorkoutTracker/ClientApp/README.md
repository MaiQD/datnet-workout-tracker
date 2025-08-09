# dotFitness ClientApp

Vue.js frontend application for the dotFitness workout tracking system.

## Tech Stack

- **Vue 3** - Progressive JavaScript framework
- **TypeScript** - Type-safe JavaScript
- **Vuetify 3** - Material Design component library
- **Vite** - Fast build tool and dev server
- **Pinia** - State management for Vue
- **Vue Router** - Client-side routing
- **Vue ApexCharts** - Interactive charts and graphs
- **SASS** - CSS preprocessor

## Getting Started

### Prerequisites
- Node.js 18+ 
- npm or yarn

### Installation
```bash
npm install
```

### Development
```bash
npm run dev
```

### Build
```bash
npm run build
```

### Preview Build
```bash
npm run preview
```

## Project Structure

```
src/
├── components/     # Reusable Vue components
├── views/         # Page components
├── layouts/       # Layout components
├── router/        # Vue Router configuration
├── plugins/       # Vue plugins (Vuetify, etc.)
├── stores/        # Pinia stores for state management
├── types/         # TypeScript type definitions
├── scss/          # Global styles and variables
└── assets/        # Static assets
```

## Features

- Responsive Material Design UI
- Dark/Light theme support
- Mobile-first design
- TypeScript support
- Component-based architecture
- State management with Pinia
- Chart integration for progress visualization

## Integration with Backend

This frontend integrates with the dotFitness ASP.NET Core backend API through REST endpoints for:
- User authentication and management
- Exercise management
- Routine building
- Workout tracking
- Progress analytics