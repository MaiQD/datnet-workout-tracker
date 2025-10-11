<script setup lang="ts">
import { ref } from "vue";
import WorkoutSummary from "@/components/dashboards/WorkoutSummary.vue";
import RecentWorkouts from "@/components/dashboards/RecentWorkouts.vue";
import QuickActions from "@/components/dashboards/QuickActions.vue";
import { designTokens } from '@/config/designTokens';
</script>

<template>
    <v-row>
        <v-col cols="12" lg="8">
            <WorkoutSummary />
        </v-col>
        <v-col cols="12" lg="4">
            <div class="mb-6">
                <QuickActions/>
            </div>
        </v-col>
        <v-col cols="12">
            <RecentWorkouts/>
        </v-col>
    </v-row>
</template>