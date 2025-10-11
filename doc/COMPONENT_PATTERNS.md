# Component Patterns Guide: dotFitness Design System

## Overview

This document outlines the reusable component patterns established for the dotFitness application, based on the modern design system inspired by dashboard-sample.html. All future components should follow these patterns to maintain consistency and visual harmony.

## Design Philosophy

- **Clean & Modern**: Generous whitespace, subtle shadows, rounded corners
- **Light Blue Primary**: #3B82F6 as the core brand color
- **Color-Coded Actions**: Distinct background colors for different action types
- **Emoji-Enhanced**: Strategic use of emojis for visual interest and quick recognition
- **Typography Hierarchy**: Clear font weight progression (400/500/600/700/800)
- **Interactive Feedback**: Hover states, smooth transitions, visual affordances

## Design Tokens

### Color Palette

```typescript
// Primary Colors
primary: '#3B82F6'        // Light blue
success: '#22C55E'        // Green
warning: '#F59E0B'        // Orange
error: '#EF4444'          // Red

// Action Card Colors
actionCards: {
  green: { bg: '#EBF9F1', accent: '#22C55E', hover: '#D1F2E0' },
  blue: { bg: '#EBF5FF', accent: '#3B82F6', hover: '#D4E7FE' },
  orange: { bg: '#FFF9EB', accent: '#F59E0B', hover: '#FFEED1' },
  red: { bg: '#FEF2F2', accent: '#EF4444', hover: '#FEE2E2' },
  purple: { bg: '#F3F0FF', accent: '#8B5CF6', hover: '#E9DEFF' },
}

// Status Badge Colors
statusBadges: {
  good: { bg: '#D1FAE5', text: '#065F46' },
  stronger: { bg: '#FEF3C7', text: '#92400E' },
  relaxed: { bg: '#DBEAFE', text: '#1E40AF' },
}

// Avatar Circle Colors
avatarCircles: {
  blue: '#DBEAFE',
  purple: '#E9D5FF',
  red: '#FECACA',
  green: '#D1FAE5',
}
```

### Spacing System

```typescript
spacing: {
  cardPadding: '1.5rem',
  cardGap: '1.5rem',
  sectionGap: '1.5rem',
}
```

### Border Radius

```typescript
borderRadius: {
  card: '0.75rem',
  button: '0.5rem',
  badge: '9999px',
}
```

### Shadows

```typescript
shadows: {
  card: '0 1px 3px 0 rgb(0 0 0 / 0.1), 0 1px 2px -1px rgb(0 0 0 / 0.1)',
  cardHover: '0 4px 6px -1px rgb(0 0 0 / 0.1), 0 2px 4px -2px rgb(0 0 0 / 0.1)',
}
```

### Typography

```typescript
typography: {
  fontFamily: "'Inter', -apple-system, BlinkMacSystemFont, sans-serif",
  weights: {
    normal: 400,
    medium: 500,
    semibold: 600,
    bold: 700,
    extrabold: 800,
  },
}
```

## Component Patterns

### 1. BaseActionCard

**Purpose**: Colored action cards for interactive elements throughout the app.

**Usage**:
```vue
<BaseActionCard
  title="Start Workout"
  description="Begin a new workout session"
  emoji="▶"
  color-scheme="green"
  @click="handleAction"
/>
```

**Props**:
- `title: string` - Main action text
- `description?: string` - Optional description
- `emoji: string` - Emoji icon
- `colorScheme: 'green' | 'blue' | 'orange' | 'red' | 'purple'` - Color scheme

**Features**:
- Automatic hover effects (opacity + transform)
- Consistent styling with design tokens
- Circular emoji container with appropriate background
- Click event emission

### 2. BaseStatusBadge

**Purpose**: Status indicators for various states (workout performance, completion status, etc.).

**Usage**:
```vue
<BaseStatusBadge
  type="good"
  text="Good"
/>
```

**Props**:
- `type: 'good' | 'stronger' | 'relaxed' | 'success' | 'warning' | 'error'` - Badge type
- `text: string` - Display text

**Features**:
- Automatic color mapping based on type
- Consistent styling with design tokens
- Pill-shaped design

### 3. BaseWorkoutItem

**Purpose**: List items for workout history, exercise lists, etc.

**Usage**:
```vue
<BaseWorkoutItem
  name="Morning Cardio"
  emoji="🏃"
  details="20/09/2025 - 30 minutes"
  status="good"
  status-text="Good"
  avatar-color="blue"
/>
```

**Props**:
- `name: string` - Item name
- `emoji: string` - Emoji avatar
- `details: string` - Secondary information
- `status?: StatusBadgeType` - Optional status
- `statusText?: string` - Status text
- `avatarColor: 'blue' | 'purple' | 'red' | 'green'` - Avatar background color

**Features**:
- Emoji avatar in colored circle
- Optional status badge
- Light gray background
- Consistent spacing and typography

## Chart Configuration Standards

### ApexCharts Setup

**Base Configuration**:
```typescript
const chartOptions = {
  chart: {
    type: 'area', // or 'line', 'bar'
    height: 250,
    toolbar: { show: false },
    fontFamily: "'Inter', sans-serif",
  },
  colors: ['#3B82F6'], // Use design token primary color
  dataLabels: { enabled: false },
  stroke: {
    curve: 'smooth',
    width: 3,
  },
  fill: {
    type: 'gradient',
    gradient: {
      opacityFrom: 0.4,
      opacityTo: 0.1,
    },
  },
  grid: {
    borderColor: '#F3F4F6',
    strokeDashArray: 3,
  },
  xaxis: {
    labels: { style: { colors: '#6B7280', fontSize: '12px' } },
  },
  yaxis: {
    labels: { style: { colors: '#6B7280', fontSize: '12px' } },
  },
  tooltip: {
    theme: 'dark',
  },
};
```

**Usage**:
```vue
<apexchart
  type="area"
  height="250"
  :options="chartOptions"
  :series="series"
></apexchart>
```

## Color Usage Guidelines

### When to Use Which Colors

**Primary Blue (#3B82F6)**:
- Main action buttons
- Primary navigation elements
- Chart lines and fills
- Key metrics and numbers

**Action Card Colors**:
- **Green**: Start actions, positive actions (Start Workout, Complete)
- **Blue**: View actions, information (View Exercises, Browse)
- **Orange**: Progress actions, tracking (Track Progress, Monitor)
- **Red**: Goal-setting, important actions (Set Goals, Delete)
- **Purple**: Special actions, premium features

**Status Colors**:
- **Good**: Successful completion, positive feedback
- **Stronger**: Improvement, progress made
- **Relaxed**: Calm activities, flexibility work
- **Success**: General success states
- **Warning**: Caution, attention needed
- **Error**: Failures, problems

## Typography Scale and Usage

### Font Weights

- **400 (Normal)**: Body text, descriptions
- **500 (Medium)**: Secondary headings, labels
- **600 (Semibold)**: Card titles, important text
- **700 (Bold)**: Primary headings, key metrics
- **800 (Extrabold)**: Hero text, main titles

### Text Sizes

- **text-h4**: Large numbers, key metrics
- **text-subtitle-1**: Card titles, section headings
- **text-subtitle-2**: Secondary headings, labels
- **text-body-1**: Main content text
- **text-caption**: Small text, metadata

## Spacing System

### Card Spacing

- **Card Padding**: 1.5rem (24px)
- **Card Gap**: 1.5rem (24px) between cards
- **Section Gap**: 1.5rem (24px) between sections

### Internal Spacing

- **Small**: 0.5rem (8px)
- **Medium**: 1rem (16px)
- **Large**: 1.5rem (24px)
- **Extra Large**: 2rem (32px)

## Shadow System

### Card Shadows

- **Default**: Subtle shadow for depth
- **Hover**: Enhanced shadow for interactive feedback
- **Elevated**: Stronger shadow for modals, overlays

## Border Radius Standards

- **Cards**: 0.75rem (12px) - Main content containers
- **Buttons**: 0.5rem (8px) - Interactive elements
- **Badges**: 9999px (pill shape) - Status indicators
- **Inputs**: 0.5rem (8px) - Form elements

## Implementation Guidelines

### For All Future Components

1. **Use Design Tokens**: Import and use `designTokens.ts` for all colors, spacing, shadows
2. **Typography Hierarchy**: Follow established font weights (400/500/600/700/800)
3. **Color Semantics**: Use action card colors for interactive elements, status colors for feedback
4. **Consistent Spacing**: Use design token spacing values
5. **Shadows & Depth**: Use design token shadow values for cards and elevation
6. **Border Radius**: Use design token radius values
7. **Hover States**: Add hover effects to interactive elements
8. **Emoji Usage**: Use emojis strategically for visual interest and recognition
9. **TypeScript**: Define proper interfaces, no `any` types
10. **Accessibility**: Maintain proper contrast ratios (WCAG AA minimum)

### Component Checklist

- [ ] Uses design tokens for colors
- [ ] Follows typography hierarchy
- [ ] Has proper TypeScript types
- [ ] Includes hover states for interactive elements
- [ ] Uses consistent spacing
- [ ] Has proper border radius
- [ ] Includes appropriate shadows
- [ ] Responsive design
- [ ] Accessibility compliant
- [ ] Documented in component patterns guide

## Migration Strategy for Existing Components

For future work, gradually migrate existing components to use:

1. Import design tokens
2. Replace hardcoded colors with token references
3. Update typography to use Inter font and new weights
4. Apply new spacing system
5. Update shadows and border radius
6. Add hover states where appropriate
7. Update TypeScript interfaces

## Examples

### Dashboard Card Pattern

```vue
<template>
  <v-card 
    elevation="0" 
    :style="{ 
      borderRadius: designTokens.borderRadius.card, 
      boxShadow: designTokens.shadows.card 
    }"
  >
    <v-card-title class="d-flex align-center">
      <v-icon icon="mdi-icon" class="mr-2" :color="designTokens.colors.primary"></v-icon>
      <span>Card Title</span>
    </v-card-title>
    
    <v-card-text>
      <!-- Content -->
    </v-card-text>
  </v-card>
</template>
```

### Action Button Pattern

```vue
<template>
  <BaseActionCard
    :title="action.title"
    :description="action.description"
    :emoji="action.emoji"
    :color-scheme="action.colorScheme"
    @click="handleAction"
  />
</template>
```

### Status Display Pattern

```vue
<template>
  <BaseStatusBadge
    :type="getStatusType(status)"
    :text="statusText"
  />
</template>
```

This design system ensures consistency across all components while providing flexibility for different use cases. All patterns are reusable and follow the established visual language of the dotFitness application.
