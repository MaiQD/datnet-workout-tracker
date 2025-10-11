<script setup lang="ts">
import { useRouter } from 'vue-router'
import LogoutButton from '@/components/auth/LogoutButton.vue'
import BaseActionCard from '@/components/common/BaseActionCard.vue'
import { designTokens } from '@/config/designTokens'

const router = useRouter()

const actions = [
  {
    title: 'Start Workout',
    description: 'Begin a new workout session',
    emoji: '▶',
    colorScheme: 'green' as const,
    route: '/workouts/new'
  },
  {
    title: 'View Exercises',
    description: 'Browse exercise library',
    emoji: '🏋️',
    colorScheme: 'blue' as const,
    route: '/exercises'
  },
  {
    title: 'Track Progress',
    description: 'Monitor your fitness goals',
    emoji: '📈',
    colorScheme: 'orange' as const,
    route: '/progress'
  },
  {
    title: 'Set Goals',
    description: 'Define your fitness objectives',
    emoji: '🎯',
    colorScheme: 'red' as const,
    route: '/goals'
  }
]

const handleAction = (route: string) => {
  router.push(route)
}
</script>

<template>
  <v-card class="quick-actions" elevation="0" :style="{ borderRadius: designTokens.borderRadius.card, boxShadow: designTokens.shadows.card }">
    <v-card-title class="d-flex align-center">
      <v-icon icon="mdi-lightning-bolt" class="mr-2" :color="designTokens.colors.primary"></v-icon>
      <span>Quick Actions</span>
    </v-card-title>
    
    <v-card-text>
      <v-row>
        <v-col
          v-for="action in actions"
          :key="action.title"
          cols="6"
          sm="6"
          lg="6"
        >
          <BaseActionCard
            :title="action.title"
            :description="action.description"
            :emoji="action.emoji"
            :color-scheme="action.colorScheme"
            @click="handleAction(action.route)"
          />
        </v-col>
      </v-row>
      
      <!-- Logout Section -->
      <v-divider class="my-4"></v-divider>
      <div class="text-center">
        <LogoutButton 
          variant="outlined" 
          color="primary" 
          button-text="Sign Out" 
          :show-icon="true"
          size="small"
          button-class="rounded-pill"
        />
      </div>
    </v-card-text>
  </v-card>
</template>

<style scoped>
.quick-actions {
  height: 100%;
  width: 100%;
  max-width: 100%;
  overflow: hidden;
}

/* Make action cards more compact for better fit in lg="4" container */
:deep(.action-card) {
  padding: 12px !important;
  min-height: auto;
  width: 100%;
  max-width: 100%;
}

:deep(.action-card .text-subtitle-1) {
  font-size: 0.875rem !important;
  line-height: 1.25rem !important;
}

:deep(.action-card .text-caption) {
  font-size: 0.75rem !important;
  line-height: 1rem !important;
}

:deep(.action-card .text-h5) {
  font-size: 1.25rem !important;
}

/* Ensure the card content doesn't overflow */
:deep(.v-card-text) {
  padding: 16px !important;
}

:deep(.v-row) {
  margin: 0 !important;
}

:deep(.v-col) {
  padding: 4px !important;
}
</style>
