<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { designTokens } from '@/config/designTokens'

interface WorkoutStats {
  totalWorkouts: number
  thisWeek: number
  thisMonth: number
  totalTime: number
  favoriteExercise: string
}

interface ExerciseProgressData {
  labels: string[];
  data: number[];
}

const stats = ref<WorkoutStats>({
  totalWorkouts: 0,
  thisWeek: 0,
  thisMonth: 0,
  totalTime: 0,
  favoriteExercise: 'None yet'
})

const chartOptions = ref({
  chart: {
    type: 'area',
    height: 250,
    toolbar: { show: false },
    fontFamily: "'Inter', sans-serif",
  },
  colors: ['#3B82F6'],
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
    categories: ['Aug 1', 'Aug 8', 'Aug 15', 'Aug 22', 'Sep 1', 'Sep 8', 'Sep 15', 'Sep 22'],
    labels: { style: { colors: '#6B7280', fontSize: '12px' } },
  },
  yaxis: {
    labels: { style: { colors: '#6B7280', fontSize: '12px' } },
  },
  tooltip: {
    theme: 'dark',
  },
})

const series = ref([{
  name: 'Max Reps',
  data: [15, 18, 17, 20, 22, 21, 24, 25],
}])

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
  <v-card class="workout-summary" elevation="0" :style="{ borderRadius: designTokens.borderRadius.card, boxShadow: designTokens.shadows.card }">
    <v-card-title class="d-flex align-center">
      <v-icon icon="mdi-dumbbell" class="mr-2" :color="designTokens.colors.primary"></v-icon>
      <span>Workout Summary</span>
    </v-card-title>
    
    <v-card-text>
      <v-row>
        <v-col cols="6" sm="3">
          <div class="text-center">
            <div class="text-h4 font-weight-bold" :style="{ color: designTokens.colors.primary }">{{ stats.totalWorkouts }}</div>
            <div class="text-caption text-medium-emphasis">Total Workouts</div>
          </div>
        </v-col>
        
        <v-col cols="6" sm="3">
          <div class="text-center">
            <div class="text-h4 font-weight-bold" :style="{ color: designTokens.colors.primary }">{{ stats.thisWeek }}</div>
            <div class="text-caption text-medium-emphasis">This Week</div>
          </div>
        </v-col>
        
        <v-col cols="6" sm="3">
          <div class="text-center">
            <div class="text-h4 font-weight-bold" :style="{ color: designTokens.colors.primary }">{{ stats.thisMonth }}</div>
            <div class="text-caption text-medium-emphasis">This Month</div>
          </div>
        </v-col>
        
        <v-col cols="6" sm="3">
          <div class="text-center">
            <div class="text-h4 font-weight-bold" :style="{ color: designTokens.colors.primary }">{{ stats.totalTime }}h</div>
            <div class="text-caption text-medium-emphasis">Total Time</div>
          </div>
        </v-col>
      </v-row>
      
      <v-divider class="my-4"></v-divider>
      
      <div class="text-center mb-4">
        <div class="text-subtitle-2 text-medium-emphasis mb-1">Favorite Exercise: {{ stats.favoriteExercise }}</div>
      </div>
      
      <div class="chart-container">
        <apexchart
          type="area"
          height="250"
          :options="chartOptions"
          :series="series"
        ></apexchart>
      </div>
    </v-card-text>
  </v-card>
</template>

<style scoped>
.workout-summary {
  height: 100%;
}

.chart-container {
  position: relative;
  width: 100%;
  height: 250px;
  max-height: 40vh;
}
</style>
