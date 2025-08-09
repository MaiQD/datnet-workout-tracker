<script setup lang="ts">
import { useAuthStore } from '@/stores/auth';
import { useRouter } from 'vue-router';
import { computed } from 'vue';

const authStore = useAuthStore();
const router = useRouter();

const user = computed(() => authStore.user);
const isAuthenticated = computed(() => authStore.isAuthenticated);

const handleLogout = async () => {
  await authStore.logout();
  router.push('/auth/login');
};
</script>

<template>
  <div class="feature-topbar w-full py-4 px-6 w-100">
    <div class="d-flex flex-lg-row flex-column gap-3 justify-space-between align-center">
      <div class="d-flex align-center ga-6">
        <div class="d-flex align-center">
          <v-icon icon="mdi-dumbbell" size="32" color="primary" class="mr-2"></v-icon>
          <span class="text-h5 font-weight-bold text-primary">dotFitness</span>
        </div>
      </div>
      
      <div class="d-flex flex-md-row flex-column align-center ga-4">
        <div v-if="isAuthenticated" class="d-flex align-center ga-3">
          <v-avatar size="40" color="primary" class="mr-2">
            <v-icon icon="mdi-account" color="white"></v-icon>
          </v-avatar>
          <div class="d-none d-md-block">
            <div class="text-subtitle-2 font-weight-medium">{{ user?.name || 'User' }}</div>
            <div class="text-caption text-medium-emphasis">{{ user?.email }}</div>
          </div>
          <v-menu>
            <template v-slot:activator="{ props }">
              <v-btn icon variant="text" v-bind="props">
                <v-icon icon="mdi-dots-vertical"></v-icon>
              </v-btn>
            </template>
            <v-list density="compact" elevation="10" class="pa-3">
              <v-list-item to="/profile">
                <v-list-item-title class="d-flex align-center gap-3">
                  <v-icon icon="mdi-account" size="18"></v-icon>
                  Profile
                </v-list-item-title>
              </v-list-item>
              <v-list-item @click="handleLogout">
                <v-list-item-title class="d-flex align-center gap-3">
                  <v-icon icon="mdi-logout" size="18"></v-icon>
                  Logout
                </v-list-item-title>
              </v-list-item>
            </v-list>
          </v-menu>
        </div>
        
        <div v-else>
          <v-btn
            variant="outlined"
            color="primary"
            to="/auth/login"
          >
            Sign In
          </v-btn>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.feature-topbar {
  background: white;
  border-bottom: 1px solid rgba(0, 0, 0, 0.12);
}
</style>
