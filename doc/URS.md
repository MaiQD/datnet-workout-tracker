# Business User Requirements Specification (URS)

Document Name: dotFitness Business User Requirements Specification

Version: 1.1 (Updated)

Date: June 10, 2025

---

## 1. Introduction

### 1.1. Purpose of this Document

This document outlines the high-level business and user requirements for **dotFitness**, a web-based application designed to support individuals in tracking and customizing their home workout routines. It serves as a foundational agreement on the application's scope and functionalities from a non-technical perspective.

### 1.2. System Overview

**dotFitness** is a personalized digital workout companion focused on making fitness accessible and manageable for users at home. It will allow individuals to create and manage their own exercises and workout routines, log their daily workout activities, and visualize their progress over time. The primary goal is to provide a highly customizable and motivating platform that helps users stay consistent with their fitness journey without needing a traditional gym environment.

### 1.3. Target Audience

The primary target audience for **dotFitness** is:

- **Individuals trying to stay active at home:** Users who may not have access to a gym, prefer home workouts, or require flexibility in their fitness schedule.
- **Users seeking structured guidance:** Individuals who want to plan their workouts effectively but may lack personal trainer access.
- **Progress-driven individuals:** Users who are motivated by seeing tangible improvements in their fitness and body metrics.
- **A small subset of early adopters:** For whom the initial deployment will be focused on providing a robust, yet cost-efficient, experience.

### 1.4. Scope of the Project

This document defines the requirements for the initial release of the **dotFitness** web application.

**In-Scope:**

- User authentication via Google accounts.
- **Mandatory login for all application features.**
- Management of personal user profiles and body metrics.
- Creation and management of custom exercises with detailed attributes.
- Creation and management of personalized workout routines.
- Comprehensive logging of completed workout sessions.
- Visualization of workout and body metric progress.
- Basic administrative functions for managing global exercise data.
- **User-selectable light and dark themes.**
- Focus on a web-based interface.

Optional/Future (Premium):
- Paid plans (Free/Pro) with gated features (e.g., CSV import, advanced analytics)

**Out-of-Scope for Initial Release:**

- Native mobile applications (iOS/Android).
- Integration with wearable devices (e.g., smartwatches, fitness trackers).
- Social features (e.g., sharing workouts, leaderboards).
- Live workout classes or trainer-led sessions.
- Complex nutritional tracking or meal planning.
- Advanced AI-driven personalized coaching beyond basic suggestions.
- Offline functionality.

## 2. Business Requirements (Functional Requirements)

### 2.1. User Management & Profile (UM)

- **UM-001: User Registration/Login:** The system SHALL allow users to register and log in securely using their existing Google accounts.
- **UM-002: Authenticated Access:** The system SHALL require users to be logged in to access any core application features beyond the initial login screen.
- **UM-003: User Profile Management:** The system SHALL allow authenticated users to view and update their profile information.
- **UM-004: Body Metric Tracking:** The system SHALL allow authenticated users to record and update their weight and height.
- **UM-005: Unit Preference:** The system SHALL allow users to select their preferred units for weight and height (e.g., kilograms/pounds, centimeters/inches).
- **UM-006: Admin Access:** The system SHALL allow specific designated users to be recognized as administrators upon their first login, granting them access to administrative functions.
- **UM-007: Role Assignment:** The system SHALL support user roles `Admin`, `PT` (Personal Trainer), and `User`. Roles SHALL be embedded in authentication tokens and enforced via server-side authorization policies.
- **UM-008: Admin Determination:** Users whose email addresses are listed in a secure, server-side configuration (email whitelist) SHALL automatically be granted the `Admin` role upon their first successful Google login. This whitelist is managed via `AdminSettings.AdminEmails` and is not exposed in client applications.
- **UM-009: PT–Client Assignment (Future):** The system SHOULD support assigning users to a `PT` for coaching. Assigned `PT`s SHALL have read/write access to their clients’ workout-related data only. (See Section 2.7.)

### 2.2. Exercise Management (EM)

- **EM-001: Custom Exercise Creation:** The system SHALL allow authenticated users to create new custom exercises.
- **EM-002: Exercise Details:** For each exercise, the system SHALL allow users to define:
    - Exercise Name.
    - Optional Description.
    - Associated Muscle Groups (e.g., Chest, Biceps, Quads).
    - Required Equipment (e.g., Dumbbells, Resistance Band, Bodyweight).
    - A Video Link for demonstration.
    - Exercise Instructions (step-by-step guide).
    - Difficulty Level (Beginner, Intermediate, Advanced, Expert).
    - Optional Exercise Image.
    - Tags for categorization.
- **EM-003: Exercise Listing & Search:** The system SHALL allow users to view a combined list of their custom exercises and global exercises, with options to search and filter by name, muscle group, equipment, or difficulty level.
- **EM-004: Exercise Editing/Deletion:** The system SHALL allow users to edit or delete their custom exercises. Users SHALL NOT be able to edit or delete global exercises.
- **EM-005: Custom Muscle Groups/Equipment:** The system SHALL allow users to add new muscle group and equipment tags that are not already present in the global list, for their personal use.
- **EM-006: Global Exercise Access:** The system SHALL provide users access to a library of global exercises created by administrators, which can be used in routines but cannot be modified by regular users.
- **EM-007: Muscle Group Body Region:** Each Muscle Group SHALL include a "Body Region" attribute to support grouping and filtering (e.g., `Upper`, `Lower`, `Core`, `FullBody`).
- **EM-008: Seeded Muscle Groups:** The system SHALL ship with a comprehensive, standardized list of Muscle Groups as seed data. Seed data SHALL be idempotently applied at startup to ensure presence and allow updates without duplication.
- **EM-009: Exercise Import (CSV):** The system SHALL allow importing exercises from a CSV file. Users can import into their own library; Admins can import as global exercises. The importer SHALL validate rows, provide a preview, and return a summary report (created/updated/failed with reasons).
- **EM-010: Smart Exercise Suggestions:** The system SHOULD suggest exercises tailored to each user based on their selected focus muscle groups, available equipment, and experience level. Suggestions SHALL include global and personal exercises and be surfaced on the Dashboard and Exercises screens.

### 2.3. Routine Management (RM)

- **RM-001: Routine Creation:** The system SHALL allow authenticated users to create new workout routines.
- **RM-002: Routine Composition:** The system SHALL allow users to add exercises to a routine from their existing custom exercises.
- **RM-003: Exercise Configuration in Routine:** For each exercise within a routine, the system SHALL allow users to define:
    - Target Sets.
    - Target Reps (e.g., "8-12", "AMRAP", "5x5").
    - Recommended Rest Time between sets (in seconds).
- **RM-004: Routine Organization:** The system SHALL allow users to name their routines and optionally provide a description.
- **RM-005: Routine Listing:** The system SHALL allow users to view a list of their saved routines.
- **RM-006: Routine Editing/Deletion:** The system SHALL allow users to edit or delete their custom routines.
- **RM-007: Quick Workout Suggestions:** The system SHOULD be able to suggest or generate a workout routine based on user-defined criteria such as desired duration, target muscle groups, and available equipment.
- **RM-008: Pre-built Templates:** The system SHALL provide a set of pre-defined home workout routine templates that users can adopt or customize.

### 2.4. Workout Tracking (WT)

- **WT-001: Start Workout Session:** The system SHALL allow users to initiate a workout session based on a saved routine or as an ad-hoc workout.
- **WT-002: Exercise Guidance:** During a workout session, the system SHALL display the current exercise's name, instructions, and provide access to its demonstration video.
- **WT-003: Set Logging:** The system SHALL allow users to log the actual sets, reps, and weight (if applicable) completed for each exercise during a session.
- **WT-004: Rest Timer:** The system SHALL provide an integrated countdown timer for recommended rest periods between sets.
- **WT-005: Workout Completion:** The system SHALL allow users to mark exercises as completed and finish a workout session.
- **WT-006: Post-Workout Summary:** Upon completion, the system SHALL provide a summary of the workout session.

### 2.5. Progress & Analytics (PA)

- **PA-001: Dashboard Overview:** The system SHALL display key motivational metrics on the user's dashboard, such as workout streak and total workouts completed.
- **PA-002: Historical Workout Logs:** The system SHALL allow users to view a historical log of all their completed workout sessions.
- **PA-003: Body Metric Trend Visualization:** The system SHALL display graphical trends of the user's weight and BMI over time.
- **PA-004: Exercise Progress Tracking:** The system SHALL provide graphs showing the progress (e.g., reps, sets, volume) for specific exercises over time.
- **PA-005: Personal Bests:** The system SHALL identify and highlight personal bests for exercises.
- **PA-006: Achievement Tracking:** The system SHALL track and display user achievements or milestones based on their workout activity.

### 2.6. System Administration (SA)

- **SA-001: Global Muscle Group Management:** The system SHALL allow authenticated administrators to add, edit, or delete global muscle group definitions.
- **SA-002: Global Equipment Management:** The system SHALL allow authenticated administrators to add, edit, or delete global equipment definitions.
- **SA-003: Global Exercise Management:** The system SHALL allow authenticated administrators to add, edit, or delete global exercise definitions that will be available to all users.
- **SA-004: Role Management:** The system SHALL allow administrators to promote/demote users between `User`, `PT`, and `Admin` roles. Changes SHALL be reflected in subsequent authentication tokens.
- **SA-005: PT–Client Management (Future):** The system SHOULD allow administrators to assign and transfer clients between `PT`s.
- **SA-006: Admin Email Whitelist:** The system SHALL load the admin email whitelist from secure configuration and apply it on first login to grant `Admin` role automatically.

### 2.7. Billing & Premium (BP) — Future

- **BP-001: Plans:** The system SHOULD support plans: `Free` and `Pro` (premium).
- **BP-002: Gated Features:** Premium-only features MAY include CSV import, advanced analytics, template libraries, and future PT tooling.
- **BP-003: Subscription Management:** Users SHOULD be able to purchase/manage subscriptions via a hosted billing portal (e.g., Stripe Customer Portal).
- **BP-004: Webhooks:** The system SHALL update subscription state from trusted billing webhooks.
- **BP-005: Cancellation & Trials:** Support trials and cancel-at-period-end with access until period end.
- **BP-006: Entitlements:** APIs SHALL enforce premium access via server-side checks/policies (e.g., `PremiumOnly`).

### 2.7. Authorization & Roles (AR)

- **AR-001: Roles:** The system defines three roles:
  - `Admin`: System-level manager with unrestricted access. Manages global data (muscle groups, equipment, global exercises), roles, and PT–client assignments.
  - `PT`: Coach with scoped access to their assigned clients. Can create PT-owned resources (e.g., routines) and assign them to their clients. Cannot access other PTs’ clients.
  - `User`: End-user. Can manage their own workouts, routines, and exercises.
- **AR-002: Policy Enforcement:** The API SHALL enforce access using policy-based authorization with, at minimum, the following policies:
  - `AdminOnly`: `Admin` role required.
  - `PTOnly`: `PT` role required.
  - `UserOnly`: `User` role required.
  - `SelfOrAdmin`: Access permitted if the acting user is the resource owner (by `UserId`) or has `Admin` role.
  - `PTAssignedToUserOrAdmin` (Future): Access permitted if acting user is the assigned `PT` for the target `UserId` or has `Admin` role.
  - `OwnerPTOrAdmin` (Future): PT-owned resource accessible to its owning `PT` or `Admin`.
  - `OwnerUserOrAdmin`: User-owned resource accessible to its owning `User` or `Admin`.
- **AR-003: Data Scoping:** Query handlers SHALL filter returned data by role:
  - `Admin`: No data restrictions.
  - `PT` (Future): Restricted to resources owned by the `PT` or their assigned clients.
  - `User`: Restricted to resources owned by the acting user.
- **AR-004: Token Claims:** Authentication tokens (JWT) SHALL include the user identifier and role claims. Authorization decisions SHALL be made server-side based on these claims and repository checks (e.g., PT–client assignment).

### 2.8. Onboarding (OB)

- **OB-001: First-Time Onboarding Prompt:** After a user's first successful login, the dashboard SHALL display a quick onboarding experience to capture essential preferences and baseline data.
- **OB-002: Required Inputs:** The onboarding flow SHALL allow users to confirm or set:
  - Display Name (prefilled from Google if available)
  - Unit Preference (Metric/Imperial)
  - Baseline Body Metrics: weight (required), height (optional)
- **OB-003: Optional Inputs:** Users MAY set:
  - Theme Preference (Light/Dark)
  - Experience Level (Beginner/Intermediate/Advanced)
  - Primary Goal (e.g., Fat Loss, Strength, Endurance)
- **OB-004a: Available Equipment Selection:** The onboarding flow SHALL allow users to select the equipment they have available at home from the global/user equipment list. This selection SHALL be stored with the user profile and used to tailor routine suggestions and exercise filters.
- **OB-004b: Focus Muscle Selection:** The onboarding flow SHALL allow users to select muscle groups they want to focus on, presented grouped by "Body Region". This selection SHALL be stored with the user profile and used to tailor suggestions and analytics.
- **OB-004: Skippable & Remind Later:** Users MAY skip the onboarding. The system SHALL gently remind them later via a dismissible dashboard banner until completed.
- **OB-005: Idempotency & Editability:** Onboarding SHALL be idempotent. Users can complete it in multiple sessions. All captured data SHALL be editable later in Profile/Settings.
- **OB-006: Persistence:** The system SHALL track onboarding completion using a boolean flag and completion timestamp per user.
- **OB-007: Security:** Onboarding updates SHALL be limited to the acting user account (`SelfOrAdmin` policy).
- **OB-008: Smart Suggestions (Optional):** Upon completion, the system MAY suggest a starter routine template based on selected goals/experience.
- **OB-009: Data Sources:** Equipment and Muscle Group options presented during onboarding SHALL be fetched from existing endpoints. Only global entries and the current user's private entries SHALL be included.

## 3. Non-Functional Requirements (NFR)

### 3.1. Performance

- **NFR-P-001:** The application UI SHALL load within 3 seconds on a standard broadband connection.
- **NFR-P-002:** API response times for common operations (e.g., logging a set, fetching routines) SHALL be under 1 second.
- **NFR-P-003:** Data visualizations (graphs) SHALL render within 2 seconds for typical data volumes.

### 3.2. Security

- **NFR-S-001:** All communication between the client and server SHALL be encrypted using HTTPS.
- **NFR-S-002:** User data (workouts, personal metrics) SHALL be securely stored and accessible only by the owning user or authorized administrators.
- **NFR-S-003:** Authentication SHALL be handled by Google's secure OAuth 2.0 protocol.
- **NFR-S-004:** The system SHALL protect against common web vulnerabilities (e.g., XSS, CSRF, SQL Injection equivalent for NoSQL).
- **NFR-S-005:** Sensitive information (e.g., API keys, database credentials) SHALL be managed securely outside of the codebase (e.g., environment variables, cloud secrets management).

### 3.3. Usability

- **NFR-U-001:** The user interface SHALL be intuitive and easy to navigate for users with basic computer literacy.
- **NFR-U-002:** The application SHALL be responsive, adapting to different screen sizes (desktop, tablet, mobile phone).
- **NFR-U-003:** Key actions (e.g., "Start Workout," "Log Set") SHALL be clearly visible and accessible.
- **NFR-U-004:** Error messages SHALL be clear, user-friendly, and provide actionable guidance.
- **NFR-U-005: Theme Customization:** The system SHALL allow users to select between a **light theme** and a **dark theme**, and SHALL persist this preference across sessions.

### 3.4. Reliability

- **NFR-R-001:** The system SHALL ensure data consistency, particularly for workout logs and progress tracking, even during transient network or application failures (via Outbox Pattern).
- **NFR-R-002:** The system SHALL aim for high uptime (acknowledging free-tier limitations).
- **NFR-R-003:** Data SHALL be persisted reliably in the database, with mechanisms to prevent accidental loss.

### 3.5. Scalability

- **NFR-SC-001:** The architecture SHALL be designed to allow for future horizontal scaling of the API and database, should the user base grow beyond initial small subset. (Acknowledging current free tier limits).
- **NFR-SC-002:** The modular monolith architecture SHALL facilitate adding new features or modules without significantly impacting existing ones.

### 3.6. Maintainability

- **NFR-M-001:** The codebase SHALL adhere to established coding standards and best practices (e.g., Clean Architecture principles, unit testing).
- **NFR-M-002:** The system SHALL have comprehensive logging to aid in troubleshooting and monitoring.
- **NFR-M-003:** Infrastructure provisioning SHALL be managed via Infrastructure as Code (Terraform) for ease of replication and modification.

### 3.7. Cost

- **NFR-C-001:** The initial deployment and ongoing operation of the application SHALL primarily utilize free-tier cloud resources (e.g., Azure App Service F1, MongoDB Atlas M0, Azure Static Web Apps Free).
- **NFR-C-002:** The system SHALL be designed to minimize resource consumption to stay within free-tier limits for the intended small user base.

## 4. Assumptions

- **A-001:** Users will have a stable internet connection to use the application.
- **A-002:** Users will have an active Google account for authentication purposes.
- **A-003:** Exercise video links will be hosted externally (e.g., YouTube, Vimeo) and linked to, not uploaded directly to dotFitness.
- **A-004:** The initial focus is on the web application, without immediate plans for dedicated native mobile apps.
- **A-005:** The initial user base will be a small subset of individuals, allowing for utilization of free cloud tiers.
- **A-006:** A secure process for defining initial administrative users (via email whitelist) will be implemented.

## 5. Constraints

- **C-001:** Initial deployment must adhere to a zero-cost infrastructure model.
- **C-002:** Development will primarily be on macOS environments.
- **C-003:** The core technology stack (ASP.NET Core, Vue.js, MongoDB) is fixed for this project.
- **C-004:** All development and deployment will follow CI/CD principles using GitHub Actions.

---

## Appendix A: Standard Muscle Groups (Seed Data)

The following muscle groups SHALL be seeded as global entries. Each entry includes a display name and `bodyRegion`. Some groups include sub-groups (children) for finer tagging while keeping the main group selectable.

- Upper (bodyRegion: `Upper`)
  - Chest (Pectorals)
  - Back
    - Lats (Latissimus Dorsi)
    - Traps (Trapezius: upper/mid/lower)
    - Rhomboids
    - Erector Spinae (Thoracic)
  - Shoulders (Deltoids)
    - Anterior Deltoid
    - Lateral Deltoid
    - Posterior Deltoid
  - Arms
    - Biceps (Biceps Brachii/Brachialis)
    - Triceps (Triceps Brachii)
    - Forearms (Flexors/Extensors)
  - Neck (Sternocleidomastoid/Upper Traps)

- Core (bodyRegion: `Core`)
  - Abs (Rectus Abdominis)
  - Obliques (External/Internal)
  - Transverse Abdominis
  - Lower Back (Lumbar Erector Spinae/Quadratus Lumborum)
  - Hip Flexors (Iliopsoas)

- Lower (bodyRegion: `Lower`)
  - Glutes (Maximus/Medius/Minimus)
  - Quadriceps (Rectus Femoris, Vastus Lateralis/Medialis/Intermedius)
  - Hamstrings (Biceps Femoris, Semitendinosus, Semimembranosus)
  - Calves (Gastrocnemius/Soleus)
  - Hip Abductors (Glute Medius/Minimus, TFL)
  - Hip Adductors (Adductor group)
  - Tibialis Anterior

- Full Body (bodyRegion: `FullBody`)
  - Full Body (compound/general)

Notes:
- The UI MAY present sub-groups nested under main groups; tagging can accept either the main group or sub-group.
- Admins MAY add additional groups; users MAY add private groups per EM-005.