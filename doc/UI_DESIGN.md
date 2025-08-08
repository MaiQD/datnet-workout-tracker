# UI Design Document: dotFitness

Document Name: dotFitness UI Design Document

Version: 1.1 (Updated)

Date: June 10, 2025

---

## 1. Introduction

### 1.1. Purpose of this Document

This document outlines the high-level business and user requirements for **dotFitness**, a web-based application designed to support individuals in tracking and customizing their home workout routines. It serves as a foundational agreement on the application's scope and functionalities from a non-technical perspective.

### 1.2. UI Design Goals (Updated)

The core UI design goals for **dotFitness** are:

- **Motivating & Engaging:** Inspire users to continue their fitness journey through positive reinforcement, visual progress, and an energetic aesthetic.
- **Clear & Uncluttered:** Provide a clean interface with minimal distractions, especially during active workout sessions.
- **Intuitive & Easy to Use:** Ensure a smooth learning curve for users to navigate, create, log, and track their workouts efficiently.
- **Visually Guided:** Leverage visual aids like exercise videos and clear data visualizations to enhance understanding and engagement.
- **Responsive (Mobile-First):** Adapt seamlessly to various screen sizes (desktop, tablet, mobile), with a strong emphasis on mobile ergonomics (thumb zones, large touch targets, minimal typing) for in-workout usage.
- **Action-Oriented:** Guide users towards key functionalities with prominent and well-labeled calls to action.
- **Themable:** Allow users to switch between a light and dark visual theme based on preference.
- **Secure Access:** Enforce authenticated access for all core application features.

### 1.3. Target Audience (UI/UX Focus)

For the "at-home active" user, the UI must compensate for the lack of a physical trainer by providing clear guidance, strong motivation, and streamlined logging. Users may interact with the app on various devices, from a laptop to a smartphone, so responsiveness and touch-friendly elements are crucial.

## 2. Visual Design Guidelines

### 2.1. Color Palette (Updated for Theming)

The color palette will be clean, modern, and energetic, designed to inspire motivation and focus. It will have distinct variations for light and dark themes.

- **Primary Accent:** A vibrant, athletic blue or green (e.g., `#007BFF` or `#28A745`) for main action buttons, highlights, and progress indicators. This color will be consistently used across both themes.
- **Secondary Accent:** A complementary, slightly softer color (e.g., a warm orange `#FFC107` or light purple `#6F42C1`) for secondary actions or specific data visualization.
- **Neutral Palette (Theme-Dependent):**
    - **Light Theme:**
        - Backgrounds: Light grays (`#F8F9FA`, `#E9ECEF`) to provide a clean canvas.
        - Text: Dark grays to black (`#343A40`, `#212529`) for high readability.
        - Borders/Dividers: Subtle grays (`#DEE2E6`).
    - **Dark Theme:**
        - Backgrounds: Dark grays to black (`#212529`, `#343A40`) to reduce eye strain.
        - Text: Light grays to white (`#F8F9FA`, `#E9ECEF`) for readability against dark backgrounds.
        - Borders/Dividers: Darker, subtle grays (`#495057`).
- **Status Colors:**
    - Success: Green (`#28A745`)
    - Warning: Yellow/Orange (`#FFC107`)
    - Error: Red (`#DC3545`)
    - These will maintain consistency across both themes for immediate recognition.

### 2.2. Typography

Clean, modern, and highly readable sans-serif fonts will be used, ensuring good contrast in both light and dark themes.

- **Headings (H1-H6):** A bold, impactful sans-serif font (e.g., `Inter`, `Montserrat`) for titles and key metrics to convey strength and clarity.
- **Body Text:** A highly readable sans-serif font (e.g., `Open Sans`, `Roboto`) for descriptions, instructions, and general content.
- **Numerical Data:** Potentially a monospace or specific font for numbers in progress charts to enhance precision.

### 2.3. Iconography

- **Style:** Clean, minimalist line icons with a consistent stroke weight. Icons should adapt to theme colors (e.g., light icons on dark backgrounds, dark icons on light backgrounds).
- **Source:** Potentially a library like Google Material Icons or Font Awesome for common actions (e.g., edit, delete, play, add, check).
- **Usage:** Used to quickly convey meaning, especially in navigation and action buttons.

### 2.4. Imagery & Illustrations

- **Style:** Minimalist, geometric, and encouraging illustrations where used. Photos should be high-quality, diverse, and depict active individuals in home environments.
- **Purpose:** To break up content, provide visual interest, and reinforce motivation.
- **Consideration:** Images should be chosen or adapted to look good in both light and dark modes, potentially using transparent backgrounds or subtle overlays.

### 2.5. Overall Aesthetic

**dotFitness** will embody a **clean, modern, and energetic** aesthetic. Whitespace will be used generously to reduce cognitive load. Cards and subtle shadows will help organize content. Positive and encouraging micro-copy will be integrated throughout the UI. The aesthetic will be maintained across both the light and dark themes, with careful adjustments to background, text, and accent colors to ensure visual harmony and readability.

## 3. Layout and Navigation

### 3.1. Responsive Layout (Mobile Priority)

The design will be built mobile-first using Tailwind CSS's utility classes.

- **Small Screens (Mobile):** Primary navigation via a bottom navigation bar or a hamburger menu toggling a full-screen overlay. Content will be stacked vertically. Critical workout session elements fill the screen. Large tap targets (44px+), sticky primary actions, and reduced text input are prioritized.
- **Medium Screens (Tablet):** Content may flow into two columns. Navigation potentially via a collapsible sidebar.
- **Large Screens (Desktop):** Consistent sidebar navigation (always visible). Multi-column layouts for dashboards and lists.

### 3.2. Main Navigation

- **Sidebar Navigation (Desktop/Tablet):** A persistent left-hand sidebar containing primary links:
    - Dashboard
    - My Exercises
    - My Routines
    - Workout History
    - Progress
    - Profile/Settings
    - Admin (if applicable)
- **Bottom Navigation Bar (Mobile):** For quick access to primary sections.
- **Top Bar:** Contains app logo, user profile avatar/dropdown, and potentially global search or notifications. This is also where the **Theme Toggle** will likely reside.

---

## 4. Key Screen UI Elements (Detailed Descriptions)

### 4.0. Welcome/Login Screen (New)

- **Goal:** Provide a clear entry point and promote the primary authentication method.
- **Layout:** Centered content with a minimalist background.
- **Elements:**
    - **App Logo & Name:** Prominent.
    - **Tagline/Value Proposition:** A concise sentence explaining what dotFitness offers.
    - **"Sign in with Google" Button:** Large, prominent, and following Google's brand guidelines for sign-in buttons.
    - Optional: Small, encouraging graphic or illustration.
    - No other navigation or complex features.

### 4.1. Dashboard (Home Screen)

- **Goal:** Quick overview, next steps, motivation.
- **Layout:** Card-based layout.
- **Header:** Prominent greeting ("Hello [User Name]!").
- **"Today's Workout" Card:**
    - Large, central card.
    - Displays routine name (if scheduled) or a suggestion.
    - Preview of 1-3 upcoming exercises.
    - **Large, high-contrast "START WORKOUT" button.**
- **Motivational Metrics:**
    - Dedicated cards or sections for "Workout Streak" (e.g., "🔥 7-Day Streak!") and "Total Workouts Logged" (e.g., "💪 34 Workouts").
    - Large, bold numbers with clear icons.
- **Progress Snapshots:** Small, digestible charts or summary statistics for recent weight changes, or a snapshot of volume.
- **Quick Action Buttons:** A row or grid of buttons/cards for "Start Quick Workout," "Browse Routines," "Create Exercise." Clear labels and relevant icons.

#### 4.1.b. Smart Exercise Suggestions (New)
- A horizontal carousel of suggested exercises tailored to the user (chips show Muscle Groups and Equipment).
- CTA per card: "Add to Routine" or "Start Now" (ad-hoc).

#### 4.1.a. First-Time Onboarding (New)

- **Trigger:** Displayed only when `isOnboarded == false` for the signed-in user.
- **Presentation:** A prominent, dismissible banner or modal at the top of the Dashboard.
- **Content:**
  - Headline: "Let's get you set up!"
  - Copy: "Confirm your preferences and add a baseline weight for better tracking."
  - Primary CTA: "Complete Onboarding"
  - Secondary CTA: "Remind me later"
- **Flow:** Multistep modal or side-panel with 3 concise steps:
  1. Profile: Display Name (prefilled), Theme (Light/Dark)
  2. Preferences: Unit Preference (Metric/Imperial)
  3. Equipment: Multi-select list of equipment (show common first, search + tags)
  4. Focus Muscles: Multi-select of muscle groups grouped by Body Region (collapsible groups)
  5. Baseline: Weight (required), Height (optional), Date (defaults to today)
- **Validation:** Inline validation; weight required if proceeding.
- **Completion:** Show a brief success toast and optionally a suggestion card (e.g., "Try a 20-min full body routine").

UI details:
- Equipment picker supports chips with remove, and a quick "I have none" toggle.
- Focus Muscles grouped sections: `Upper`, `Lower`, `Core`, `FullBody`; search filters across all.

### 4.2. Exercise Management (Browse & Create)

- **"My Exercises" List View:**
  - Suggested section at top (if present) before the full list.
    - A clean, scrollable list of exercise cards.
    - Each card: Exercise Name, primary Muscle Group, Equipment icon.
    - Search bar at the top, with filters for Muscle Groups, Equipment, "Bodyweight Only."
    - "Create New Exercise" prominent button.
- **Exercise Detail/Create/Edit Form:**
    - Clear, organized form fields:
        - **Exercise Name:** Text input.
        - **Description:** Multi-line text area.
        - **Muscle Groups:** Multi-select dropdown/tag input populated from global/user-defined lists.
        - **Equipment:** Multi-select dropdown/tag input from global/user-defined lists.
        - **Video Link:** Text input for URL. A small, embedded video preview will appear below the input once a valid URL is entered.
    - Validation feedback for each field (e.g., red border, error text).
    - "Save" and "Cancel" buttons.

#### 4.2.a. Exercise Import (CSV) (New)

- **Entry Points:** "Import CSV" button in "My Exercises" toolbar and Admin Exercises page for global imports.
- **Flow:**
  1. Upload CSV file (drag & drop or file picker).
  2. Parse & Preview table: show first N rows with validation markers.
  3. Mapping (optional): confirm columns → fields.
  4. Import Options: as Global (Admins only), Overwrite if name matches (optional), Default difficulty.
  5. Confirm & Import: show progress bar; on completion, show summary (created/updated/failed with reasons).
- **Validation:** Inline per row; invalid rows can be skipped with reasons collected.
- **Template:** Link to download sample CSV.
- **UX:** Keep import non-blocking; background job optional if large.

### 4.3. Routine Management (Build & Manage)

- **"My Routines" List View:** Similar to exercises, a list of routine cards showing name, description, and quick actions ("Start," "Edit," "Delete").
- **Routine Builder Form:**
    - **Routine Name & Description:** Text inputs.
    - **Exercise List (within routine):** A dynamic list of exercises.
        - Each exercise item shows: Name, Sets, Reps, Rest Time.
        - **Drag-and-Drop handles** or up/down arrows for reordering exercises.
        - Buttons to "Edit Exercise in Routine" (adjust sets/reps/rest), "Remove Exercise."
        - "Add Exercise" button (triggers a modal picker).
    - **Exercise Picker Modal:** A searchable list of all available exercises. Users can select and add.
    - **Input Fields:** For sets (number input), reps (text input, allowing "AMRAP", "5x5" etc.), and **Rest Time (Number input with "seconds" or "minutes" unit picker)**.
    - "Save Routine" and "Cancel" buttons.

### 4.4. Active Workout Session Screen (High-Priority UX)

This screen requires a focused, full-screen, and highly interactive design.

- **Minimalist Layout:** Eliminate all non-essential navigation and elements. Mobile: full-screen, sticky controls, and large buttons suited for one-handed use.
- **Current Exercise Display:**
    - Large, bold **Exercise Name** at the top.
    - Clear indicator: "Exercise X of Y."
    - **Prominent Embedded Video Player:** Autoplay (muted) with loop option, easy controls.
    - Brief instructions/tips below the video.
- **Set Logging Area:**
    - Clearly labeled "Set 1," "Set 2," etc.
    - Clean input fields for **Reps** (number picker or text), **Weight** (number input), and a **Unit** selector (kg/lbs, pre-filled by user preference).
    - A large, satisfying **"LOG SET" button** that changes state (e.g., briefly shows "Logged!").
- **Rest Timer (Crucial):**
    - After logging a set, a **large, central countdown timer** (e.g., 60s, 90s) appears.
    - Progress bar or ring around the timer.
    - Clearly labeled "REST" text.
    - Buttons: "Skip Rest," "Add 30s," "Extend Rest."
    - An audible chime/vibration when rest ends.
- **Navigation:**
    - "Previous Exercise" and "Next Exercise" buttons (maybe small, subtle arrows) at the bottom.
    - A clear "FINISH WORKOUT" button (perhaps in a corner or modal confirmation).
- **Progress Feedback:** A subtle progress bar at the top or bottom indicating overall workout completion.

### 4.5. Progress & Body Metrics

- **Layout:** Tabs or sections for "Overview," "Workout History," "Body Metrics," "Exercise Progress," "Achievements."
- **Body Metrics Input:**
    - Simple form to input current weight and height with a date picker (defaulting to today).
    - "Save" button.
- **Charts (Chart.js Integration):**
    - **Weight Trend Graph:** Line graph showing weight over time. X-axis: Date, Y-axis: Weight.
    - **BMI Trend Graph:** Line graph showing BMI over time. X-axis: Date, Y-axis: BMI.
    - **Exercise Progress Graph:** User selects an exercise, and a line graph shows reps/sets/volume over time.
    - Interactive tooltips on hover.
    - Clear legends and axis labels.
    - Date range selectors (e.g., "Last 30 Days," "Last 6 Months," "All Time").
- **Personal Bests:** A dedicated section or card displaying achievements like "New PR: 50 Push-ups (July 10, 2025)."
- **Workout History:** A list of past workout sessions, clickable to view details.

---

## 5. Interactive Elements & Feedback

- **Buttons & Calls to Action:** Clear hierarchy (primary, secondary, tertiary). Hover states, active states, and focus states.
- **Forms & Inputs:** Clear labels, placeholder text, visual validation feedback (e.g., red borders for errors, green checkmarks for success), disabled states.
- **Loaders & Spinners:** Subtle loaders for asynchronous operations (e.g., fetching data, saving changes).
- **Toasts/Notifications:** Small, non-intrusive pop-up messages for success confirmations ("Exercise Saved!"), warnings, or errors.
- **Animations & Transitions:** Subtle animations (e.g., fading, sliding) for navigation, component transitions, and state changes to enhance fluidity and perceived performance without being distracting.

## 6. Component-Based Design (Vue.js & Tailwind CSS)

The UI will be built using Vue.js's component-based architecture.

- **Vue Components:** Each distinct UI element (e.g., `Button`, `InputField`, `Card`), and each major section (e.g., `WorkoutSession`, `ExerciseForm`, `ProgressChart`), will be encapsulated as a reusable Vue component.
- **Tailwind CSS:** All styling will be applied directly using Tailwind's utility classes within Vue component templates. This ensures consistency, responsiveness, and minimal custom CSS. The `dark:` prefix will be extensively used for theme-specific styles.

### 6.1. Onboarding Components (New)

- `components/users/onboarding/OnboardingBanner.vue`: Dashboard banner entry point with CTAs.
- `components/users/onboarding/OnboardingModal.vue`: Multistep modal wrapper.
- `components/users/onboarding/steps/ProfileStep.vue`: Display name + theme.
- `components/users/onboarding/steps/PreferencesStep.vue`: Unit preference.
- `components/users/onboarding/steps/EquipmentStep.vue`: Equipment multi-select.
- `components/users/onboarding/steps/FocusMusclesStep.vue`: Muscle groups by Body Region.
- `components/users/onboarding/steps/BaselineStep.vue`: Weight, height, date.

Store additions:

```javascript
// stores/auth/onboarding state (Pinia)
export const useAuthStore = defineStore('auth', {
  state: () => ({
    user: null,
    isOnboarded: false,
    onboardingCompletedAt: null
  }),
  actions: {
    async completeOnboarding(payload) {
      // Calls API to update profile + create baseline metric, then flags onboarding complete
    }
  }
});
```
- **Chart.js Integration:** Chart.js will be used within dedicated Vue components (e.g., `WeightTrendChart.vue`) by passing data as props to the Chart.js instances, ensuring reactive updates. Chart colors will be adjusted based on the active theme.

---

## 7. Special UI/UX Considerations

### 7.1. Authentication State & Access (New)

- **Default State:** Upon opening the application, users will land directly on the **Welcome/Login Screen (4.0)**.
- **Protected Routes:** All other application routes (Dashboard, Exercises, Routines, etc.) will be protected. If an unauthenticated user attempts to access a protected route, they will be redirected to the Login Screen.
- **Backend Protection:** All API endpoints will be protected by authentication and authorization middleware, ensuring no unauthorized data access or manipulation.
- **UI Feedback:** A loading spinner may appear briefly during authentication redirects.

### 7.2. Theming (Dark and Light Theme) (New)
### 7.3. Mobile-First Ergonomics (New)

- Large touch targets (44–48px)
- Bottom-sheet modals and drawers on mobile for onboarding and pickers
- Sticky primary actions (e.g., Start/Log/Finish) during workouts
- Reduced typing: pickers, toggles, defaults
- Haptics and subtle sounds for rest timer and logging feedback (where supported)

- **Mechanism:** Users will have the option to switch between a **Light Theme** (default) and a **Dark Theme**.
- **Theme Toggle:** A prominent toggle (e.g., a sun/moon icon) will be present in the application's top navigation bar or user profile settings.
- **Persistence:** The user's theme preference will be saved (e.g., in `localStorage`) and automatically applied on subsequent visits.
- **Visual Impact:**
    - **Backgrounds:** Will shift from light colors to dark grays/blacks.
    - **Text Colors:** Will invert to maintain readability (dark text on light, light text on dark).
    - **Accent Colors:** Primary and secondary accent colors will remain consistent but may have slight luminance adjustments to pop appropriately on the new backgrounds.
    - **Chart Colors:** Chart elements (lines, bars, text, grids) will adapt their colors to be clearly visible and harmonious within the active theme.
    - **Icons/Illustrations:** Will be chosen or styled to appear well in both themes.
- **Implementation:** Tailwind CSS's built-in `dark:` variant will be extensively used for styling. A root class on the `<html>` element (e.g., `dark`) will control the active theme.

# dotFitness UI Design: Frontend Architecture

> Based on modular monolith best practices from [modular-monolith-with-ddd](https://github.com/MaiQD/modular-monolith-with-ddd/blob/master/README.md)

This document outlines the frontend architecture and design principles for the dotFitness workout tracker application.

## 🎯 Frontend Architecture Overview

The dotFitness frontend follows a **modular architecture** that mirrors the backend's modular monolith structure, providing:

- **Module-based organization**: Each backend module has corresponding frontend components
- **Clean separation**: UI components are organized by business domain
- **Scalable design**: Easy to add new modules without affecting existing ones
- **Consistent UX**: Unified design system across all modules

## 🏗️ Frontend Project Structure

```
dotFitness.WebUI/
├── src/
│   ├── components/
│   │   ├── shared/              # 🔗 Shared Components
│   │   │   ├── layout/
│   │   │   ├── navigation/
│   │   │   ├── forms/
│   │   │   └── charts/
│   │   ├── users/               # 👤 User Module Components
│   │   │   ├── auth/
│   │   │   ├── profile/
│   │   │   └── metrics/
│   │   ├── exercises/           # 💪 Exercise Module Components
│   │   │   ├── management/
│   │   │   ├── search/
│   │   │   └── details/
│   │   ├── routines/            # 📋 Routine Module Components
│   │   │   ├── builder/
│   │   │   ├── templates/
│   │   │   └── execution/
│   │   └── workout-logs/        # 📊 Workout Log Module Components
│   │       ├── tracking/
│   │       ├── analytics/
│   │       └── progress/
│   ├── services/
│   │   ├── api/                 # API Integration
│   │   │   ├── users/
│   │   │   ├── exercises/
│   │   │   ├── routines/
│   │   │   └── workout-logs/
│   │   ├── auth/                # Authentication
│   │   └── storage/             # Local Storage
│   ├── stores/                  # State Management
│   │   ├── auth/
│   │   ├── users/
│   │   ├── exercises/
│   │   ├── routines/
│   │   └── workout-logs/
│   ├── utils/                   # Utilities
│   ├── styles/                  # Styling
│   └── assets/                  # Static Assets
├── public/
└── tests/
    ├── unit/
    ├── integration/
    └── e2e/
```

## 🎨 Design System

### Color Palette
```css
:root {
  /* Primary Colors */
  --primary-50: #eff6ff;
  --primary-100: #dbeafe;
  --primary-500: #3b82f6;
  --primary-600: #2563eb;
  --primary-700: #1d4ed8;
  
  /* Secondary Colors */
  --secondary-50: #f0fdf4;
  --secondary-100: #dcfce7;
  --secondary-500: #22c55e;
  --secondary-600: #16a34a;
  
  /* Neutral Colors */
  --gray-50: #f9fafb;
  --gray-100: #f3f4f6;
  --gray-500: #6b7280;
  --gray-700: #374151;
  --gray-900: #111827;
  
  /* Semantic Colors */
  --success: #10b981;
  --warning: #f59e0b;
  --error: #ef4444;
  --info: #3b82f6;
}
```

### Typography
```css
:root {
  /* Font Families */
  --font-sans: 'Inter', -apple-system, BlinkMacSystemFont, sans-serif;
  --font-mono: 'JetBrains Mono', 'Fira Code', monospace;
  
  /* Font Sizes */
  --text-xs: 0.75rem;
  --text-sm: 0.875rem;
  --text-base: 1rem;
  --text-lg: 1.125rem;
  --text-xl: 1.25rem;
  --text-2xl: 1.5rem;
  --text-3xl: 1.875rem;
  
  /* Font Weights */
  --font-light: 300;
  --font-normal: 400;
  --font-medium: 500;
  --font-semibold: 600;
  --font-bold: 700;
}
```

### Spacing System
```css
:root {
  --space-1: 0.25rem;
  --space-2: 0.5rem;
  --space-3: 0.75rem;
  --space-4: 1rem;
  --space-6: 1.5rem;
  --space-8: 2rem;
  --space-12: 3rem;
  --space-16: 4rem;
}
```

## 🧩 Component Architecture

### Shared Components

#### Layout Components
```vue
<!-- components/shared/layout/AppLayout.vue -->
<template>
  <div class="min-h-screen bg-gray-50">
    <AppHeader />
    <main class="container mx-auto px-4 py-8">
      <slot />
    </main>
    <AppFooter />
  </div>
</template>
```

#### Navigation Components
```vue
<!-- components/shared/navigation/Sidebar.vue -->
<template>
  <nav class="bg-white shadow-lg">
    <div class="px-4 py-6">
      <div class="flex items-center justify-between">
        <h1 class="text-xl font-bold text-gray-900">dotFitness</h1>
        <button @click="$emit('toggle')" class="lg:hidden">
          <MenuIcon class="h-6 w-6" />
        </button>
      </div>
      
      <div class="mt-6">
        <nav class="space-y-2">
          <SidebarLink 
            v-for="item in navigationItems" 
            :key="item.name"
            :item="item"
            :active="item.name === activeModule"
          />
        </nav>
      </div>
    </div>
  </nav>
</template>

<script setup>
const navigationItems = [
  { name: 'Dashboard', href: '/', icon: HomeIcon },
  { name: 'Exercises', href: '/exercises', icon: DumbbellIcon },
  { name: 'Routines', href: '/routines', icon: CalendarIcon },
  { name: 'Workout Logs', href: '/workout-logs', icon: ChartIcon },
  { name: 'Profile', href: '/profile', icon: UserIcon }
];
</script>
```

#### Form Components
```vue
<!-- components/shared/forms/BaseInput.vue -->
<template>
  <div class="form-group">
    <label v-if="label" :for="id" class="form-label">
      {{ label }}
      <span v-if="required" class="text-red-500">*</span>
    </label>
    <input
      :id="id"
      :type="type"
      :value="modelValue"
      :placeholder="placeholder"
      :required="required"
      :disabled="disabled"
      class="form-input"
      :class="{ 'form-input-error': hasError }"
      @input="$emit('update:modelValue', $event.target.value)"
      @blur="$emit('blur')"
    />
    <p v-if="error" class="form-error">{{ error }}</p>
  </div>
</template>

<script setup>
defineProps({
  modelValue: String,
  label: String,
  id: String,
  type: { type: String, default: 'text' },
  placeholder: String,
  required: Boolean,
  disabled: Boolean,
  error: String
});

defineEmits(['update:modelValue', 'blur']);
</script>
```

### Module-Specific Components

#### Users Module
```vue
<!-- components/users/auth/LoginForm.vue -->
<template>
  <div class="max-w-md mx-auto">
    <div class="bg-white rounded-lg shadow-md p-8">
      <h2 class="text-2xl font-bold text-center mb-6">Welcome to dotFitness</h2>
      
      <button
        @click="handleGoogleLogin"
        :disabled="isLoading"
        class="w-full btn btn-primary"
      >
        <GoogleIcon class="w-5 h-5 mr-2" />
        {{ isLoading ? 'Signing in...' : 'Continue with Google' }}
      </button>
      
      <p class="text-sm text-gray-600 text-center mt-4">
        By continuing, you agree to our Terms of Service and Privacy Policy
      </p>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { useAuthStore } from '@/stores/auth';

const authStore = useAuthStore();
const isLoading = ref(false);

const handleGoogleLogin = async () => {
  isLoading.value = true;
  try {
    await authStore.loginWithGoogle();
  } catch (error) {
    console.error('Login failed:', error);
  } finally {
    isLoading.value = false;
  }
};
</script>
```

#### Exercises Module
```vue
<!-- components/exercises/management/ExerciseCard.vue -->
<template>
  <div class="bg-white rounded-lg shadow-md overflow-hidden">
    <div class="p-6">
      <div class="flex items-start justify-between">
        <div class="flex-1">
          <h3 class="text-lg font-semibold text-gray-900">{{ exercise.name }}</h3>
          <p class="text-gray-600 mt-1">{{ exercise.description }}</p>
          
          <div class="mt-4 flex flex-wrap gap-2">
            <span
              v-for="muscleGroup in exercise.muscleGroups"
              :key="muscleGroup"
              class="px-2 py-1 text-xs font-medium bg-blue-100 text-blue-800 rounded"
            >
              {{ muscleGroup }}
            </span>
          </div>
          
          <div class="mt-4 flex flex-wrap gap-2">
            <span
              v-for="equipment in exercise.equipment"
              :key="equipment"
              class="px-2 py-1 text-xs font-medium bg-green-100 text-green-800 rounded"
            >
              {{ equipment }}
            </span>
          </div>
        </div>
        
        <div class="flex space-x-2">
          <button
            @click="$emit('edit', exercise)"
            class="btn btn-secondary btn-sm"
          >
            Edit
          </button>
          <button
            @click="$emit('delete', exercise.id)"
            class="btn btn-danger btn-sm"
          >
            Delete
          </button>
        </div>
      </div>
      
      <div v-if="exercise.videoUrl" class="mt-4">
        <a
          :href="exercise.videoUrl"
          target="_blank"
          class="text-blue-600 hover:text-blue-800 text-sm"
        >
          Watch Demo Video →
        </a>
      </div>
    </div>
  </div>
</template>

<script setup>
defineProps({
  exercise: {
    type: Object,
    required: true
  }
});

defineEmits(['edit', 'delete']);
</script>
```

## 🔌 API Integration

### API Service Structure
```javascript
// services/api/base.js
class ApiService {
  constructor(baseURL) {
    this.baseURL = baseURL;
    this.client = axios.create({
      baseURL,
      timeout: 10000,
      headers: {
        'Content-Type': 'application/json'
      }
    });
    
    this.setupInterceptors();
  }
  
  setupInterceptors() {
    // Request interceptor for authentication
    this.client.interceptors.request.use(
      (config) => {
        const token = localStorage.getItem('auth_token');
        if (token) {
          config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
      },
      (error) => Promise.reject(error)
    );
    
    // Response interceptor for error handling
    this.client.interceptors.response.use(
      (response) => response,
      (error) => {
        if (error.response?.status === 401) {
          // Handle unauthorized access
          this.handleUnauthorized();
        }
        return Promise.reject(error);
      }
    );
  }
  
  handleUnauthorized() {
    localStorage.removeItem('auth_token');
    window.location.href = '/login';
  }
}

// services/api/exercises.js
class ExercisesApiService extends ApiService {
  constructor() {
    super('/api/v1/exercises');
  }
  
  async getAll(params = {}) {
    const response = await this.client.get('', { params });
    return response.data;
  }
  
  async getById(id) {
    const response = await this.client.get(`/${id}`);
    return response.data;
  }
  
  async create(exercise) {
    const response = await this.client.post('', exercise);
    return response.data;
  }
  
  async update(id, exercise) {
    const response = await this.client.put(`/${id}`, exercise);
    return response.data;
  }
  
  async delete(id) {
    await this.client.delete(`/${id}`);
  }
}

export const exercisesApi = new ExercisesApiService();
```

### State Management
```javascript
// stores/exercises.js
import { defineStore } from 'pinia';
import { exercisesApi } from '@/services/api/exercises';

export const useExercisesStore = defineStore('exercises', {
  state: () => ({
    exercises: [],
    loading: false,
    error: null,
    filters: {
      search: '',
      muscleGroups: [],
      equipment: []
    }
  }),
  
  getters: {
    filteredExercises: (state) => {
      let filtered = state.exercises;
      
      if (state.filters.search) {
        filtered = filtered.filter(exercise =>
          exercise.name.toLowerCase().includes(state.filters.search.toLowerCase()) ||
          exercise.description.toLowerCase().includes(state.filters.search.toLowerCase())
        );
      }
      
      if (state.filters.muscleGroups.length > 0) {
        filtered = filtered.filter(exercise =>
          state.filters.muscleGroups.some(group =>
            exercise.muscleGroups.includes(group)
          )
        );
      }
      
      if (state.filters.equipment.length > 0) {
        filtered = filtered.filter(exercise =>
          state.filters.equipment.some(equipment =>
            exercise.equipment.includes(equipment)
          )
        );
      }
      
      return filtered;
    }
  },
  
  actions: {
    async fetchExercises() {
      this.loading = true;
      this.error = null;
      
      try {
        const exercises = await exercisesApi.getAll();
        this.exercises = exercises;
      } catch (error) {
        this.error = error.message;
      } finally {
        this.loading = false;
      }
    },
    
    async createExercise(exercise) {
      try {
        const newExercise = await exercisesApi.create(exercise);
        this.exercises.push(newExercise);
        return newExercise;
      } catch (error) {
        this.error = error.message;
        throw error;
      }
    },
    
    async updateExercise(id, updates) {
      try {
        const updatedExercise = await exercisesApi.update(id, updates);
        const index = this.exercises.findIndex(e => e.id === id);
        if (index !== -1) {
          this.exercises[index] = updatedExercise;
        }
        return updatedExercise;
      } catch (error) {
        this.error = error.message;
        throw error;
      }
    },
    
    async deleteExercise(id) {
      try {
        await exercisesApi.delete(id);
        this.exercises = this.exercises.filter(e => e.id !== id);
      } catch (error) {
        this.error = error.message;
        throw error;
      }
    },
    
    setFilters(filters) {
      this.filters = { ...this.filters, ...filters };
    }
  }
});
```

## 📱 Responsive Design

### Mobile-First Approach
```css
/* Base styles for mobile */
.container {
  padding: 1rem;
}

.exercise-grid {
  display: grid;
  grid-template-columns: 1fr;
  gap: 1rem;
}

/* Tablet styles */
@media (min-width: 768px) {
  .container {
    padding: 2rem;
  }
  
  .exercise-grid {
    grid-template-columns: repeat(2, 1fr);
    gap: 1.5rem;
  }
}

/* Desktop styles */
@media (min-width: 1024px) {
  .exercise-grid {
    grid-template-columns: repeat(3, 1fr);
    gap: 2rem;
  }
}
```

### Navigation Responsiveness
```vue
<!-- components/shared/navigation/ResponsiveNav.vue -->
<template>
  <nav class="bg-white shadow-lg">
    <!-- Mobile menu button -->
    <div class="lg:hidden">
      <button
        @click="isMobileMenuOpen = !isMobileMenuOpen"
        class="p-2 rounded-md text-gray-600 hover:text-gray-900"
      >
        <MenuIcon v-if="!isMobileMenuOpen" class="h-6 w-6" />
        <XIcon v-else class="h-6 w-6" />
      </button>
    </div>
    
    <!-- Desktop navigation -->
    <div class="hidden lg:flex lg:items-center lg:space-x-8">
      <NavLink
        v-for="item in navigationItems"
        :key="item.name"
        :item="item"
        :active="item.name === activeModule"
      />
    </div>
    
    <!-- Mobile navigation -->
    <div
      v-show="isMobileMenuOpen"
      class="lg:hidden absolute top-full left-0 w-full bg-white shadow-lg"
    >
      <div class="px-2 pt-2 pb-3 space-y-1">
        <MobileNavLink
          v-for="item in navigationItems"
          :key="item.name"
          :item="item"
          :active="item.name === activeModule"
          @click="isMobileMenuOpen = false"
        />
      </div>
    </div>
  </nav>
</template>
```

## 🎯 User Experience Patterns

### Loading States
```vue
<!-- components/shared/LoadingSpinner.vue -->
<template>
  <div class="flex items-center justify-center p-8">
    <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-primary-600"></div>
    <span class="ml-2 text-gray-600">{{ message || 'Loading...' }}</span>
  </div>
</template>

<script setup>
defineProps({
  message: String
});
</script>
```

### Error Handling
```vue
<!-- components/shared/ErrorMessage.vue -->
<template>
  <div class="bg-red-50 border border-red-200 rounded-md p-4">
    <div class="flex">
      <ExclamationCircleIcon class="h-5 w-5 text-red-400" />
      <div class="ml-3">
        <h3 class="text-sm font-medium text-red-800">
          {{ title || 'An error occurred' }}
        </h3>
        <div class="mt-2 text-sm text-red-700">
          <p>{{ message }}</p>
        </div>
        <div v-if="retry" class="mt-4">
          <button
            @click="$emit('retry')"
            class="btn btn-secondary btn-sm"
          >
            Try Again
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
defineProps({
  title: String,
  message: String,
  retry: Boolean
});

defineEmits(['retry']);
</script>
```

### Success Feedback
```vue
<!-- components/shared/SuccessMessage.vue -->
<template>
  <div class="bg-green-50 border border-green-200 rounded-md p-4">
    <div class="flex">
      <CheckCircleIcon class="h-5 w-5 text-green-400" />
      <div class="ml-3">
        <h3 class="text-sm font-medium text-green-800">
          {{ title || 'Success!' }}
        </h3>
        <div class="mt-2 text-sm text-green-700">
          <p>{{ message }}</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
defineProps({
  title: String,
  message: String
});
</script>
```

## 🧪 Testing Strategy

### Unit Testing
```javascript
// tests/unit/components/exercises/ExerciseCard.test.js
import { mount } from '@vue/test-utils';
import ExerciseCard from '@/components/exercises/management/ExerciseCard.vue';

describe('ExerciseCard', () => {
  const mockExercise = {
    id: '1',
    name: 'Push-ups',
    description: 'Basic push-ups',
    muscleGroups: ['Chest', 'Triceps'],
    equipment: ['None'],
    videoUrl: 'https://example.com/video'
  };

  it('renders exercise information correctly', () => {
    const wrapper = mount(ExerciseCard, {
      props: { exercise: mockExercise }
    });

    expect(wrapper.text()).toContain('Push-ups');
    expect(wrapper.text()).toContain('Basic push-ups');
    expect(wrapper.text()).toContain('Chest');
    expect(wrapper.text()).toContain('Triceps');
  });

  it('emits edit event when edit button is clicked', async () => {
    const wrapper = mount(ExerciseCard, {
      props: { exercise: mockExercise }
    });

    await wrapper.find('[data-testid="edit-button"]').trigger('click');
    expect(wrapper.emitted('edit')).toBeTruthy();
    expect(wrapper.emitted('edit')[0]).toEqual([mockExercise]);
  });
});
```

### Integration Testing
```javascript
// tests/integration/exercises/ExerciseManagement.test.js
import { mount } from '@vue/test-utils';
import { createPinia, setActivePinia } from 'pinia';
import ExerciseManagement from '@/views/exercises/ExerciseManagement.vue';
import { useExercisesStore } from '@/stores/exercises';

describe('ExerciseManagement Integration', () => {
  let pinia;

  beforeEach(() => {
    pinia = createPinia();
    setActivePinia(pinia);
  });

  it('loads and displays exercises from store', async () => {
    const store = useExercisesStore();
    store.exercises = [
      { id: '1', name: 'Push-ups', description: 'Basic push-ups' },
      { id: '2', name: 'Squats', description: 'Basic squats' }
    ];

    const wrapper = mount(ExerciseManagement, {
      global: { plugins: [pinia] }
    });

    await wrapper.vm.$nextTick();
    expect(wrapper.text()).toContain('Push-ups');
    expect(wrapper.text()).toContain('Squats');
  });
});
```

## 🚀 Performance Optimization

### Code Splitting
```javascript
// router/index.js
import { createRouter, createWebHistory } from 'vue-router';

const routes = [
  {
    path: '/',
    name: 'Dashboard',
    component: () => import('@/views/Dashboard.vue')
  },
  {
    path: '/exercises',
    name: 'Exercises',
    component: () => import('@/views/exercises/ExerciseManagement.vue')
  },
  {
    path: '/routines',
    name: 'Routines',
    component: () => import('@/views/routines/RoutineManagement.vue')
  },
  {
    path: '/workout-logs',
    name: 'WorkoutLogs',
    component: () => import('@/views/workout-logs/WorkoutLogManagement.vue')
  }
];

export default createRouter({
  history: createWebHistory(),
  routes
});
```

### Lazy Loading
```vue
<!-- components/exercises/ExerciseVideo.vue -->
<template>
  <div class="video-container">
    <video
      v-if="isLoaded"
      :src="videoUrl"
      controls
      class="w-full rounded-lg"
    ></video>
    <div v-else class="video-placeholder">
      <button @click="loadVideo" class="btn btn-primary">
        Load Video
      </button>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';

const props = defineProps({
  videoUrl: String
});

const isLoaded = ref(false);

const loadVideo = () => {
  isLoaded.value = true;
};
</script>
```

## 📚 References

- [Modular Monolith with DDD](https://github.com/MaiQD/modular-monolith-with-ddd/blob/master/README.md) - Backend architecture reference
- [Vue.js Best Practices](https://vuejs.org/guide/best-practices/) - Vue.js development guidelines
- [Tailwind CSS](https://tailwindcss.com/docs) - Utility-first CSS framework
- [Pinia State Management](https://pinia.vuejs.org/) - Vue.js state management
- [Vue Test Utils](https://test-utils.vuejs.org/) - Vue.js testing utilities