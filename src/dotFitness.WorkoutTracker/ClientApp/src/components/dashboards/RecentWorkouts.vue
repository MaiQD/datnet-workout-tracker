<script setup lang="ts">
import { ref, onMounted } from 'vue'

interface Workout {
  id: string
  name: string
  date: string
  duration: number
  exercises: number
  type: 'strength' | 'cardio' | 'flexibility'
}

const recentWorkouts = ref<Workout[]>([])

onMounted(() => {
  // TODO: Fetch actual workout data from API
  // For now, using mock data
  recentWorkouts.value = [
    {
      id: '1',
      name: 'Upper Body Strength',
      date: '2024-01-15',
      duration: 45,
      exercises: 6,
      type: 'strength'
    },
    {
      id: '2',
      name: 'Morning Cardio',
      date: '2024-01-14',
      duration: 30,
      exercises: 3,
      type: 'cardio'
    },
    {
      id: '3',
      name: 'Full Body Circuit',
      date: '2024-01-12',
      duration: 60,
      exercises: 8,
      type: 'strength'
    }
  ]
})

const getTypeColor = (type: string) => {
  switch (type) {
    case 'strength': return 'primary'
    case 'cardio': return 'success'
    case 'flexibility': return 'info'
    default: return 'grey'
  }
}

const getTypeIcon = (type: string) => {
  switch (type) {
    case 'strength': return 'mdi-dumbbell'
    case 'cardio': return 'mdi-run'
    case 'flexibility': return 'mdi-yoga'
    default: return 'mdi-help'
  }
}
</script>

<template>
  <v-card class="recent-workouts">
    <v-card-title class="d-flex align-center">
      <v-icon icon="mdi-clock-outline" class="mr-2" color="primary"></v-icon>
      <span>Recent Workouts</span>
    </v-card-title>
    
    <v-card-text>
      <div v-if="recentWorkouts.length === 0" class="text-center py-8">
        <v-icon icon="mdi-dumbbell" size="48" color="grey-lighten-1"></v-icon>
        <div class="text-subtitle-1 text-medium-emphasis mt-2">No workouts yet</div>
        <div class="text-caption text-medium-emphasis">Start your fitness journey today!</div>
      </div>
      
      <div v-else>
        <div
          v-for="workout in recentWorkouts"
          :key="workout.id"
          class="workout-item d-flex align-center py-3"
        >
          <v-avatar
            :color="getTypeColor(workout.type)"
            size="40"
            class="mr-3"
          >
            <v-icon :icon="getTypeIcon(workout.type)" color="white"></v-icon>
          </v-avatar>
          
          <div class="flex-grow-1">
            <div class="text-subtitle-2 font-weight-medium">{{ workout.name }}</div>
            <div class="text-caption text-medium-emphasis">
              {{ new Date(workout.date).toLocaleDateString() }} • {{ workout.duration }}min • {{ workout.exercises }} exercises
            </div>
          </div>
          
          <v-chip
            :color="getTypeColor(workout.type)"
            size="small"
            variant="tonal"
          >
            {{ workout.type }}
          </v-chip>
        </div>
      </div>
    </v-card-text>
    
    <v-card-actions>
      <v-btn
        variant="text"
        color="primary"
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

.workout-item:not(:last-child) {
  border-bottom: 1px solid rgba(0, 0, 0, 0.12);
}
</style>
