<script setup lang="ts">
import { ref } from "vue";
import WorkoutSummary from "@/components/dashboards/WorkoutSummary.vue";
import RecentWorkouts from "@/components/dashboards/RecentWorkouts.vue";
import QuickActions from "@/components/dashboards/QuickActions.vue";
</script>

<template>
    <v-row>
        <v-col cols="12" sm="12" lg="8">
            <WorkoutSummary />
        </v-col>
        <v-col cols="12" sm="12" lg="4">
            <div class="mb-6">
                <QuickActions/>
            </div>
        </v-col>
        <v-col cols="12" sm="12" lg="12">
            <RecentWorkouts/>
        </v-col>
        <v-col class="text-center mt-2">
            <p class="text-muted">Built with ❤️ for fitness enthusiasts</p>
        </v-col>
    </v-row>
</template>