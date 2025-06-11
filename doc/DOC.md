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