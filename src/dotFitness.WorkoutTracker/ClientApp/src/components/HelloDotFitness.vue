<template>
  <v-card class="mx-auto" max-width="400">
    <v-card-title class="text-center">
      🏋️‍♂️ Welcome to dotFitness!
    </v-card-title>
    <v-card-text>
      <p class="text-body-1">
        Your Vue.js frontend is successfully set up with:
      </p>
      <v-list density="compact">
        <v-list-item prepend-icon="mdi-vuejs" title="Vue 3 + TypeScript" />
        <v-list-item prepend-icon="mdi-material-design" title="Vuetify 3" />
        <v-list-item prepend-icon="mdi-pin" title="Pinia State Management" />
        <v-list-item prepend-icon="mdi-chart-line" title="ApexCharts" />
        <v-list-item prepend-icon="mdi-cellphone" title="Mobile-First Design" />
      </v-list>
    </v-card-text>
    <v-card-actions class="justify-center">
      <v-btn color="primary" @click="testStore">
        Test Store
      </v-btn>
    </v-card-actions>
  </v-card>
</template>

<script setup lang="ts">
import { useAuthStore } from '@/stores'

const authStore = useAuthStore()

const testStore = () => {
  console.log('Auth store state:', {
    isAuthenticated: authStore.isAuthenticated,
    user: authStore.user,
    isLoading: authStore.isLoading
  })
}
</script>

<style scoped>
.v-card-title {
  color: #1976d2;
}
</style>
