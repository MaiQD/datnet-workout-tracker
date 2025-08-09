<script setup lang="ts">
import { UserIcon, MailIcon, ListCheckIcon } from 'vue-tabler-icons';
import LogoutButton from '@/components/auth/LogoutButton.vue';
import { useAuthStore } from '@/stores/auth';
import { computed } from 'vue';

const authStore = useAuthStore();

const user = computed(() => authStore.user);
const profilePicture = computed(() => user.value?.profilePicture);
const userName = computed(() => user.value?.name || 'User');
const userEmail = computed(() => user.value?.email || '');
</script>

<template>
    <!-- ---------------------------------------------- -->
    <!-- notifications DD -->
    <!-- ---------------------------------------------- -->
    <v-menu :close-on-content-click="false">
        <template v-slot:activator="{ props }">
            <v-btn class="" variant="text" v-bind="props" icon>
                <v-avatar size="35">
                    <img 
                        v-if="profilePicture" 
                        :src="profilePicture" 
                        height="35" 
                        alt="user" 
                    />
                    <v-icon v-else size="20">mdi-account</v-icon>
                </v-avatar>
            </v-btn>
        </template>
        <v-sheet rounded="xl" width="250" elevation="10" class="mt-2">
            <!-- User Info Header -->
            <div class="pa-4 border-b">
                <div class="d-flex align-center">
                    <v-avatar size="40" class="mr-3">
                        <img 
                            v-if="profilePicture" 
                            :src="profilePicture" 
                            alt="user" 
                        />
                        <v-icon v-else size="24">mdi-account</v-icon>
                    </v-avatar>
                    <div>
                        <div class="text-subtitle-1 font-weight-medium">{{ userName }}</div>
                        <div class="text-caption text-medium-emphasis">{{ userEmail }}</div>
                    </div>
                </div>
            </div>
            
            <v-list class="py-0" lines="one" density="compact">
                <v-list-item value="item1" color="primary" to="/profile">
                    <template v-slot:prepend>
                        <UserIcon stroke-width="1.5" size="20"/>
                    </template>
                    <v-list-item-title class="pl-4 text-body-1">My Profile</v-list-item-title>
                </v-list-item>
                <v-list-item value="item2" color="primary" to="/account">
                    <template v-slot:prepend>
                        <MailIcon stroke-width="1.5" size="20"/>
                    </template>
                    <v-list-item-title  class="pl-4 text-body-1">My Account</v-list-item-title>
                </v-list-item>
                <v-list-item value="item3" color="primary" to="/workouts"> 
                    <template v-slot:prepend>
                        <ListCheckIcon stroke-width="1.5"  size="20"/>
                    </template>
                    <v-list-item-title class="pl-4 text-body-1">My Workouts</v-list-item-title>
                </v-list-item>
            </v-list>
            <div class="pt-4 pb-4 px-5 text-center">
                <LogoutButton 
                    variant="outlined" 
                    color="primary" 
                    button-text="Logout" 
                    :show-icon="false"
                    size="small"
                    class="rounded-pill"
                    style="width: 100%"
                />
            </div>
        </v-sheet>
    </v-menu>
</template>
