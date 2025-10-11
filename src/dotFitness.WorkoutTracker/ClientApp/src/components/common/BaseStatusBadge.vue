<script setup lang="ts">
import { computed } from 'vue';
import { designTokens, type StatusBadgeType } from '@/config/designTokens';

interface Props {
  type: StatusBadgeType;
  text: string;
}

const props = defineProps<Props>();

const badgeStyle = computed(() => ({
  backgroundColor: designTokens.colors.statusBadges[props.type].bg,
  color: designTokens.colors.statusBadges[props.type].text,
  borderRadius: designTokens.borderRadius.badge,
  fontSize: '0.75rem',
  fontWeight: 500,
}));
</script>

<template>
  <v-chip
    :style="badgeStyle"
    size="small"
    label
  >
    {{ text }}
  </v-chip>
</template>
