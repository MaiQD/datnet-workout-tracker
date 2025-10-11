<script setup lang="ts">
import { computed } from 'vue';
import { designTokens, type ActionCardColor } from '@/config/designTokens';

interface Props {
  title: string;
  description?: string;
  emoji: string;
  colorScheme: ActionCardColor;
}

const props = defineProps<Props>();

const emit = defineEmits<{
  click: [];
}>();

const cardStyle = computed(() => ({
  backgroundColor: designTokens.colors.actionCards[props.colorScheme].bg,
  borderRadius: designTokens.borderRadius.card,
  boxShadow: designTokens.shadows.card,
  transition: designTokens.transitions.default,
  cursor: 'pointer',
}));

const iconCircleStyle = computed(() => ({
  width: '48px',
  height: '48px',
  borderRadius: '50%',
  backgroundColor: designTokens.colors.actionCards[props.colorScheme].hover,
  display: 'flex',
  alignItems: 'center',
  justifyContent: 'center',
}));

const accentColor = computed(() => designTokens.colors.actionCards[props.colorScheme].accent);

const handleClick = () => {
  emit('click');
};
</script>

<template>
  <v-card
    :style="cardStyle"
    class="action-card text-center pa-4 hover-opacity"
    elevation="0"
    @click="handleClick"
  >
    <div :style="iconCircleStyle" class="mx-auto mb-2">
      <span class="text-h5">{{ emoji }}</span>
    </div>
    <div class="text-subtitle-1 font-weight-semibold mb-1" :style="{ color: accentColor }">
      {{ title }}
    </div>
    <div v-if="description" class="text-caption text-medium-emphasis">
      {{ description }}
    </div>
  </v-card>
</template>

<style scoped>
.action-card:hover {
  box-shadow: v-bind('designTokens.shadows.cardHover');
  transform: translateY(-2px);
}
</style>
