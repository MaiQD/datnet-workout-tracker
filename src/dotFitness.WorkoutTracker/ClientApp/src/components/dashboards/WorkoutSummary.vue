<script setup lang="ts">
import { ref, onMounted } from 'vue'

interface WorkoutStats {
  totalWorkouts: number
  thisWeek: number
  thisMonth: number
  totalTime: number
  favoriteExercise: string
}

const stats = ref<WorkoutStats>({
  totalWorkouts: 0,
  thisWeek: 0,
  thisMonth: 0,
  totalTime: 0,
  favoriteExercise: 'None yet'
})

onMounted(() => {
  // TODO: Fetch actual workout data from API
  // For now, using mock data
  stats.value = {
    totalWorkouts: 24,
    thisWeek: 3,
    thisMonth: 12,
    totalTime: 18.5,
    favoriteExercise: 'Push-ups'
  }
})
</script>

<template>
  <v-card class="workout-summary">
    <v-card-title class="d-flex align-center">
      <v-icon icon="mdi-dumbbell" class="mr-2" color="primary"></v-icon>
      <span>Workout Summary</span>
    </v-card-title>
    
    <v-card-text>
      <v-row>
        <v-col cols="6" sm="3">
          <div class="text-center">
            <div class="text-h4 font-weight-bold text-primary">{{ stats.totalWorkouts }}</div>
            <div class="text-caption text-medium-emphasis">Total Workouts</div>
          </div>
        </v-col>
        
        <v-col cols="6" sm="3">
          <div class="text-center">
            <div class="text-h4 font-weight-bold text-success">{{ stats.thisWeek }}</div>
            <div class="text-caption text-medium-emphasis">This Week</div>
          </div>
        </v-col>
        
        <v-col cols="6" sm="3">
          <div class="text-center">
            <div class="text-h4 font-weight-bold text-info">{{ stats.thisMonth }}</div>
            <div class="text-caption text-medium-emphasis">This Month</div>
          </div>
        </v-col>
        
        <v-col cols="6" sm="3">
          <div class="text-center">
            <div class="text-h4 font-weight-bold text-warning">{{ stats.totalTime }}h</div>
            <div class="text-caption text-medium-emphasis">Total Time</div>
          </div>
        </v-col>
      </v-row>
      
      <v-divider class="my-4"></v-divider>
      
      <div class="text-center">
        <div class="text-subtitle-2 text-medium-emphasis mb-1">Favorite Exercise</div>
        <div class="text-h6 font-weight-medium">{{ stats.favoriteExercise }}</div>
      </div>
    </v-card-text>
  </v-card>
</template>

<style scoped>
.workout-summary {
  height: 100%;
}
</style>
