<script setup lang="ts">
import { ref, onMounted } from 'vue'
import BaseWorkoutItem from '@/components/common/BaseWorkoutItem.vue'
import { designTokens } from '@/config/designTokens'

interface Workout {
  id: string
  name: string
  date: string
  duration: number
  exercises?: number
  type: 'strength' | 'cardio' | 'flexibility'
  status: 'Good' | 'Stronger' | 'Relaxed'
  emoji: string
  avatarColor: 'blue' | 'purple' | 'red' | 'green'
}

const recentWorkouts = ref<Workout[]>([])

onMounted(() => {
  // TODO: Fetch actual workout data from API
  // For now, using mock data matching the sample
  recentWorkouts.value = [
    {
      id: '1',
      name: 'Morning Cardio',
      date: '2025-09-20',
      duration: 30,
      exercises: 3,
      type: 'cardio',
      status: 'Good',
      emoji: '🏃',
      avatarColor: 'blue'
    },
    {
      id: '2',
      name: 'Upper Body Strength',
      date: '2025-09-18',
      duration: 45,
      exercises: 5,
      type: 'strength',
      status: 'Stronger',
      emoji: '💪',
      avatarColor: 'purple'
    },
    {
      id: '3',
      name: 'Flexibility Flow',
      date: '2025-09-17',
      duration: 15,
      exercises: 1,
      type: 'flexibility',
      status: 'Relaxed',
      emoji: '🧘',
      avatarColor: 'red'
    }
  ]
})

const getStatusType = (status: string): 'good' | 'stronger' | 'relaxed' => {
  switch (status) {
    case 'Good': return 'good'
    case 'Stronger': return 'stronger'
    case 'Relaxed': return 'relaxed'
    default: return 'good'
  }
}

const formatWorkoutDetails = (workout: Workout): string => {
  const date = new Date(workout.date).toLocaleDateString('en-GB', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric'
  })
  
  if (workout.exercises) {
    return `${date} - ${workout.exercises} exercises, ${workout.duration} minutes`
  }
  
  return `${date} - ${workout.duration} minutes`
}
</script>

<template>
  <v-card class="recent-workouts" elevation="0" :style="{ borderRadius: designTokens.borderRadius.card, boxShadow: designTokens.shadows.card }">
    <v-card-title class="d-flex align-center">
      <v-icon icon="mdi-clock-outline" class="mr-2" :color="designTokens.colors.primary"></v-icon>
      <span>Recent Workouts</span>
    </v-card-title>
    
    <v-card-text>
      <div v-if="recentWorkouts.length === 0" class="text-center py-8">
        <v-icon icon="mdi-dumbbell" size="48" color="grey-lighten-1"></v-icon>
        <div class="text-subtitle-1 text-medium-emphasis mt-2">No workouts yet</div>
        <div class="text-caption text-medium-emphasis">Start your fitness journey today!</div>
      </div>
      
      <div v-else class="space-y-4">
        <BaseWorkoutItem
          v-for="workout in recentWorkouts"
          :key="workout.id"
          :name="workout.name"
          :emoji="workout.emoji"
          :details="formatWorkoutDetails(workout)"
          :status="getStatusType(workout.status)"
          :status-text="workout.status"
          :avatar-color="workout.avatarColor"
        />
      </div>
    </v-card-text>
    
    <v-card-actions>
      <v-btn
        variant="text"
        :color="designTokens.colors.primary"
        block
        to="/workouts"
      >
        View All Workouts
      </v-btn>
    </v-card-actions>
  </v-card>
</template>

<style scoped>
.recent-workouts {
  height: 100%;
}

.space-y-4 > * + * {
  margin-top: 1rem;
}
</style>
