<script setup lang="ts">
import { computed } from 'vue';
import { designTokens, type AvatarCircleColor, type StatusBadgeType } from '@/config/designTokens';
import BaseStatusBadge from './BaseStatusBadge.vue';

interface Props {
  name: string;
  emoji: string;
  details: string;
  status?: StatusBadgeType;
  statusText?: string;
  avatarColor: AvatarCircleColor;
}

const props = defineProps<Props>();

const avatarStyle = computed(() => ({
  width: '40px',
  height: '40px',
  borderRadius: '50%',
  backgroundColor: designTokens.colors.avatarCircles[props.avatarColor],
  display: 'flex',
  alignItems: 'center',
  justifyContent: 'center',
  fontWeight: 'bold',
}));
</script>

<template>
  <div class="workout-item d-flex align-center pa-3 rounded-lg" :style="{ backgroundColor: '#F9FAFB' }">
    <div :style="avatarStyle" class="mr-4">
      <span class="text-h6">{{ emoji }}</span>
    </div>
    <div class="flex-grow-1">
      <div class="text-subtitle-1 font-weight-semibold">{{ name }}</div>
      <div class="text-caption text-medium-emphasis">{{ details }}</div>
    </div>
    <BaseStatusBadge v-if="status && statusText" :type="status" :text="statusText" />
  </div>
</template>
<style scoped>
.workout-item:not(:last-child) {
  border-bottom: 1px solid rgba(0, 0, 0, 0.12);
}
</style>

