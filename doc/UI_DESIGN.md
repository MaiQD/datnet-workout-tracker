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
- **Responsive:** Adapt seamlessly to various screen sizes (desktop, tablet, mobile) for flexibility in home workout environments.
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

### 3.1. Responsive Layout

The design will be built mobile-first using Tailwind CSS's utility classes.

- **Small Screens (Mobile):** Primary navigation via a bottom navigation bar or a hamburger menu toggling a full-screen overlay. Content will be stacked vertically. Critical workout session elements will fill the screen.
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

### 4.2. Exercise Management (Browse & Create)

- **"My Exercises" List View:**
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

- **Minimalist Layout:** Eliminate all non-essential navigation and elements.
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
- **Chart.js Integration:** Chart.js will be used within dedicated Vue components (e.g., `WeightTrendChart.vue`) by passing data as props to the Chart.js instances, ensuring reactive updates. Chart colors will be adjusted based on the active theme.

---

## 7. Special UI/UX Considerations

### 7.1. Authentication State & Access (New)

- **Default State:** Upon opening the application, users will land directly on the **Welcome/Login Screen (4.0)**.
- **Protected Routes:** All other application routes (Dashboard, Exercises, Routines, etc.) will be protected. If an unauthenticated user attempts to access a protected route, they will be redirected to the Login Screen.
- **Backend Protection:** All API endpoints will be protected by authentication and authorization middleware, ensuring no unauthorized data access or manipulation.
- **UI Feedback:** A loading spinner may appear briefly during authentication redirects.

### 7.2. Theming (Dark and Light Theme) (New)

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