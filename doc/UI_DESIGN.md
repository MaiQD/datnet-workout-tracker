# UI Design Document: dotFitness

Document Name: dotFitness UI Design Document

Version: 2.0 (Updated with Modern Design System)

Date: January 2025

---

## 1. Introduction

### 1.1. Purpose of this Document

This document outlines the high-level business and user requirements for **dotFitness**, a web-based application designed to support individuals in tracking and customizing their home workout routines. It serves as a foundational agreement on the application's scope and functionalities from a non-technical perspective.

### 1.2. UI Design Goals (Updated)

The core UI design goals for **dotFitness** are:

- **Motivating & Engaging:** Inspire users to continue their fitness journey through positive reinforcement, visual progress, and an energetic aesthetic.
- **Clean & Modern:** Provide a clean interface with generous whitespace, subtle shadows, and rounded corners for a contemporary feel.
- **Intuitive & Easy to Use:** Ensure a smooth learning curve for users to navigate, create, log, and track their workouts efficiently.
- **Visually Guided:** Leverage visual aids like exercise videos, clear data visualizations, and strategic emoji usage to enhance understanding and engagement.
- **Responsive (Mobile-First):** Adapt seamlessly to various screen sizes (desktop, tablet, mobile), with a strong emphasis on mobile ergonomics (thumb zones, large touch targets, minimal typing) for in-workout usage.
- **Action-Oriented:** Guide users towards key functionalities with prominent and well-labeled calls to action using color-coded action cards.
- **Themable:** Allow users to switch between a light and dark visual theme based on preference.
- **Secure Access:** Enforce authenticated access for all core application features.

### 1.3. Target Audience (UI/UX Focus)

For the "at-home active" user, the UI must compensate for the lack of a physical trainer by providing clear guidance, strong motivation, and streamlined logging. Users may interact with the app on various devices, from a laptop to a smartphone, so responsiveness and touch-friendly elements are crucial.

## 2. Visual Design Guidelines

### 2.1. Color Palette (Updated for Modern Design System)

The color palette is clean, modern, and energetic, designed to inspire motivation and focus. It features a light blue primary color with distinct accent colors for different action types.

#### Primary Colors
- **Primary Blue**: `#3B82F6` - Main brand color for primary actions, navigation, and key metrics
- **Success Green**: `#22C55E` - Success states, positive actions, completion indicators
- **Warning Orange**: `#F59E0B` - Progress tracking, attention-grabbing elements
- **Error Red**: `#EF4444` - Error states, important actions, goal setting

#### Action Card Colors
- **Green Action**: Background `#EBF9F1`, Accent `#22C55E`, Hover `#D1F2E0` - Start actions, positive actions
- **Blue Action**: Background `#EBF5FF`, Accent `#3B82F6`, Hover `#D4E7FE` - View actions, information
- **Orange Action**: Background `#FFF9EB`, Accent `#F59E0B`, Hover `#FFEED1` - Progress actions, tracking
- **Red Action**: Background `#FEF2F2`, Accent `#EF4444`, Hover `#FEE2E2` - Goal setting, important actions
- **Purple Action**: Background `#F3F0FF`, Accent `#8B5CF6`, Hover `#E9DEFF` - Special actions, premium features

#### Status Badge Colors
- **Good**: Background `#D1FAE5`, Text `#065F46` - Successful completion, positive feedback
- **Stronger**: Background `#FEF3C7`, Text `#92400E` - Improvement, progress made
- **Relaxed**: Background `#DBEAFE`, Text `#1E40AF` - Calm activities, flexibility work

#### Background Colors
- **Primary Background**: `#F0F4F8` - Main application background
- **Surface**: `#FFFFFF` - Card backgrounds, content areas
- **Light Background**: `#F9FAFB` - Subtle backgrounds, list items

#### Text Colors
- **Primary Text**: `#111827` - Main content text
- **Secondary Text**: `#6B7280` - Supporting text, metadata

#### Border Colors
- **Border**: `#E5E7EB` - Card borders, dividers
- **Input Border**: `#D1D5DB` - Form input borders

### 2.2. Typography

Clean, modern, and highly readable typography using the Inter font family with a clear weight hierarchy.

#### Font Family
- **Primary**: `'Inter', -apple-system, BlinkMacSystemFont, sans-serif`
- **Fallbacks**: System fonts for optimal performance

#### Font Weights
- **400 (Normal)**: Body text, descriptions, general content
- **500 (Medium)**: Secondary headings, labels, important text
- **600 (Semibold)**: Card titles, section headings, key information
- **700 (Bold)**: Primary headings, important metrics
- **800 (Extrabold)**: Hero text, main titles, emphasis

#### Text Sizes
- **H1-H6**: Clear hierarchy with proper line heights
- **Body Text**: Highly readable sizes for descriptions and instructions
- **Caption**: Smaller text for metadata and secondary information

### 2.3. Iconography

- **Style**: Clean, minimalist line icons with consistent stroke weight
- **Source**: Material Design Icons (MDI) for consistency
- **Usage**: Used to quickly convey meaning, especially in navigation and action buttons
- **Emoji Integration**: Strategic use of emojis for visual interest and quick recognition

### 2.4. Imagery & Illustrations

- **Style**: Minimalist, geometric, and encouraging illustrations
- **Purpose**: To break up content, provide visual interest, and reinforce motivation
- **Emoji Usage**: Strategic placement of emojis in avatars, action cards, and status indicators

### 2.5. Overall Aesthetic

**dotFitness** embodies a **clean, modern, and energetic** aesthetic. The design features:

- Generous whitespace to reduce cognitive load
- Subtle shadows and rounded corners for depth
- Color-coded action cards for clear visual hierarchy
- Strategic emoji usage for personality and recognition
- Smooth transitions and hover effects for interactivity
- Consistent spacing and typography throughout

## 3. Layout and Navigation

### 3.1. Responsive Layout (Mobile Priority)

The design is built mobile-first using Vuetify's responsive grid system.

- **Small Screens (Mobile)**: Primary navigation via hamburger menu. Content stacked vertically with large touch targets (44px+).
- **Medium Screens (Tablet)**: Content flows into two columns. Navigation via collapsible sidebar.
- **Large Screens (Desktop)**: Consistent sidebar navigation (always visible). Multi-column layouts for dashboards and lists.

### 3.2. Main Navigation

- **Sidebar Navigation (Desktop/Tablet)**: A persistent left-hand sidebar containing primary links:
    - Dashboard
    - Workouts
    - Exercises
    - Progress
    - Goals
    - Profile
- **Top Bar**: Contains app logo, user profile avatar/dropdown, and notifications
- **Theme Toggle**: Available in user profile settings for future dark mode implementation

## 4. Key Screen UI Elements (Detailed Descriptions)

### 4.1. Dashboard (Home Screen) - Updated Design

- **Goal**: Quick overview, next steps, motivation with modern card-based layout
- **Layout**: Card-based layout with proper spacing and shadows
- **Header**: Clean card titles with primary blue icons
- **Workout Summary Card**:
    - Large card with 4-metric stat display
    - All metrics use primary blue color (#3B82F6)
    - "Favorite Exercise" subtitle with chart integration
    - ApexCharts area chart showing exercise progress over time
    - Smooth curves, blue gradient fill, minimal grid
- **Quick Actions Card**:
    - 2x2 grid of colored action cards
    - Each action has distinct background color and emoji
    - Hover effects with opacity and transform
    - Sign Out button at bottom
- **Recent Workouts Card**:
    - List of workout items with emoji avatars
    - Colored status badges (Good/Stronger/Relaxed)
    - Light gray background for each item
    - Proper spacing and typography

### 4.2. Exercise Management (Browse & Create)

- **"My Exercises" List View**:
    - Clean, scrollable list of exercise cards
    - Each card: Exercise Name, primary Muscle Group, Equipment icon
    - Search bar at the top with filters
    - "Create New Exercise" prominent button using action card pattern
- **Exercise Detail/Create/Edit Form**:
    - Clear, organized form fields with proper spacing
    - Validation feedback with error states
    - Action buttons using design system colors

### 4.3. Routine Management (Build & Manage)

- **"My Routines" List View**: Similar to exercises with action card patterns
- **Routine Builder Form**:
    - Clean form layout with proper spacing
    - Drag-and-drop handles for reordering
    - Action buttons using design system patterns

### 4.4. Active Workout Session Screen (High-Priority UX)

This screen requires a focused, full-screen, and highly interactive design.

- **Minimalist Layout**: Clean interface with large touch targets
- **Current Exercise Display**:
    - Large, bold Exercise Name
    - Clear progress indicator
    - Embedded video player with controls
- **Set Logging Area**:
    - Clean input fields with proper spacing
    - Large "LOG SET" button using primary blue
- **Rest Timer**:
    - Large, central countdown timer
    - Progress ring around timer
    - Action buttons for rest management

### 4.5. Progress & Body Metrics

- **Layout**: Card-based tabs with proper spacing
- **Charts**: ApexCharts integration with consistent styling
    - Blue color scheme (#3B82F6)
    - Smooth curves and gradients
    - Minimal grid lines
    - Dark tooltips
- **Personal Bests**: Card-based display with proper typography

## 5. Interactive Elements & Feedback

### 5.1. Action Cards

- **Design**: Colored backgrounds with emoji icons in circular containers
- **Hover States**: Opacity reduction (0.9) and slight transform (translateY(-2px))
- **Colors**: Distinct colors for different action types
- **Transitions**: Smooth 0.2s ease-in-out transitions

### 5.2. Status Badges

- **Design**: Pill-shaped badges with appropriate colors
- **Types**: Good, Stronger, Relaxed with distinct color schemes
- **Usage**: Workout performance indicators, completion status

### 5.3. Charts

- **Library**: ApexCharts with consistent configuration
- **Styling**: Blue primary color, smooth curves, gradient fills
- **Interactions**: Hover tooltips, responsive design
- **Typography**: Inter font family throughout

### 5.4. Forms & Inputs

- **Styling**: Outlined variants with primary blue color
- **Validation**: Clear error states with red colors
- **Spacing**: Consistent padding and margins

## 6. Component-Based Design (Vue.js & Vuetify)

The UI is built using Vue.js's component-based architecture with Vuetify components.

### 6.1. Design System Components

- **BaseActionCard**: Reusable colored action cards
- **BaseStatusBadge**: Consistent status indicators
- **BaseWorkoutItem**: Standardized list items
- **Design Tokens**: Centralized color, spacing, and typography values

### 6.2. Chart Integration

- **ApexCharts**: Used within dedicated Vue components
- **Configuration**: Consistent styling with design tokens
- **Responsive**: Charts adapt to container size
- **Theming**: Colors adjust based on design system

## 7. Special UI/UX Considerations

### 7.1. Design System Implementation

- **Design Tokens**: All colors, spacing, shadows centralized in `designTokens.ts`
- **Component Patterns**: Reusable components for consistency
- **Typography Scale**: Clear hierarchy with Inter font family
- **Color Semantics**: Meaningful color usage for different action types

### 7.2. Accessibility

- **Color Contrast**: All color combinations meet WCAG AA standards
- **Touch Targets**: Minimum 44px for mobile interactions
- **Typography**: Readable font sizes and weights
- **Focus States**: Clear focus indicators for keyboard navigation

### 7.3. Performance

- **Font Loading**: Inter font with preconnect for optimal loading
- **Chart Rendering**: Efficient ApexCharts configuration
- **Component Optimization**: Reusable components reduce bundle size

## 8. Implementation Guidelines

### 8.1. For All Future Components

1. **Use Design Tokens**: Import and use `designTokens.ts` for all styling
2. **Typography Hierarchy**: Follow established font weights (400/500/600/700/800)
3. **Color Semantics**: Use action card colors for interactive elements
4. **Consistent Spacing**: Use design token spacing values
5. **Shadows & Depth**: Use design token shadow values
6. **Border Radius**: Use design token radius values
7. **Hover States**: Add hover effects to interactive elements
8. **Emoji Usage**: Use emojis strategically for visual interest
9. **TypeScript**: Define proper interfaces, no `any` types
10. **Accessibility**: Maintain proper contrast ratios

### 8.2. Component Checklist

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

## 9. Migration Strategy

For existing components, gradually migrate to use:

1. Import design tokens
2. Replace hardcoded colors with token references
3. Update typography to use Inter font and new weights
4. Apply new spacing system
5. Update shadows and border radius
6. Add hover states where appropriate
7. Update TypeScript interfaces

## 10. References

- **Design System**: Based on modern dashboard patterns
- **Color Palette**: Inspired by clean, professional interfaces
- **Typography**: Inter font family for optimal readability
- **Component Library**: Vuetify with custom design system
- **Charts**: ApexCharts with consistent styling
- **Documentation**: Component patterns guide for developers

This design system ensures consistency across all components while providing flexibility for different use cases. The modern, clean aesthetic with strategic use of colors and emojis creates an engaging and motivating user experience for fitness enthusiasts.