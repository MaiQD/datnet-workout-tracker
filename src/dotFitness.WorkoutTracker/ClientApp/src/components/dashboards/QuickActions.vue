<script setup lang="ts">
import { useRouter } from 'vue-router'

const router = useRouter()

const actions = [
  {
    title: 'Start Workout',
    description: 'Begin a new workout session',
    icon: 'mdi-play-circle',
    color: 'success',
    route: '/workouts/new'
  },
  {
    title: 'View Exercises',
    description: 'Browse exercise library',
    icon: 'mdi-dumbbell',
    color: 'primary',
    route: '/exercises'
  },
  {
    title: 'Track Progress',
    description: 'Monitor your fitness goals',
    icon: 'mdi-chart-line',
    color: 'info',
    route: '/progress'
  },
  {
    title: 'Set Goals',
    description: 'Define your fitness objectives',
    icon: 'mdi-target',
    color: 'warning',
    route: '/goals'
  }
]

const handleAction = (route: string) => {
  router.push(route)
}
</script>

<template>
  <v-card class="quick-actions">
    <v-card-title class="d-flex align-center">
      <v-icon icon="mdi-lightning-bolt" class="mr-2" color="primary"></v-icon>
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
          <v-card
            :color="action.color"
            variant="tonal"
            class="action-card text-center pa-4 cursor-pointer"
            @click="handleAction(action.route)"
          >
            <v-icon
              :icon="action.icon"
              :color="action.color"
              size="32"
              class="mb-2"
            ></v-icon>
            <div class="text-subtitle-2 font-weight-medium mb-1">{{ action.title }}</div>
            <div class="text-caption text-medium-emphasis">{{ action.description }}</div>
          </v-card>
        </v-col>
      </v-row>
    </v-card-text>
  </v-card>
</template>

<style scoped>
.quick-actions {
  height: 100%;
}

.action-card {
  transition: transform 0.2s ease-in-out;
  cursor: pointer;
}

.action-card:hover {
  transform: translateY(-2px);
}

.cursor-pointer {
  cursor: pointer;
}
</style>
