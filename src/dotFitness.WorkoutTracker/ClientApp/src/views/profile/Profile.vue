<template>
  <div class="profile">
    <v-container>
      <v-row>
        <v-col cols="12">
          <div class="d-flex justify-space-between align-center mb-6">
            <h1 class="text-h4">My Profile</h1>
            <v-btn
                v-if="!isEditing"
                @click="startEditing"
                color="primary"
                variant="outlined"
                prepend-icon="mdi-pencil"
            >
              Edit Profile
            </v-btn>
          </div>

          <!-- Profile Display/Edit Card -->
          <v-card class="mb-6">
            <v-card-title class="bg-primary">
              <v-icon icon="mdi-account-circle" class="mr-2"></v-icon>
              Personal Information
            </v-card-title>

            <v-card-text class="pa-6">
              <v-form ref="profileForm" v-model="isFormValid" @submit.prevent="saveProfile">
                <v-row>
                  <!-- Profile Picture -->
                  <v-col cols="12" class="text-center mb-4">
                    <v-avatar size="120" class="mb-4">
                      <img
                          v-if="user?.profilePicture"
                          :src="user.profilePicture"
                          alt="Profile Picture"
                      />
                      <v-icon v-else size="60">mdi-account</v-icon>
                    </v-avatar>
                    <div class="text-h6">{{ user?.name || 'User' }}</div>
                    <div class="text-body-2 text-medium-emphasis">{{ user?.email }}</div>
                  </v-col>

                  <!-- Display Name -->
                  <v-col cols="12" md="6">
                    <v-text-field
                        v-model="formData.displayName"
                        label="Display Name"
                        :readonly="!isEditing"
                        :variant="isEditing ? 'outlined' : 'plain'"
                        :rules="[rules.required, rules.maxLength(100)]"
                        prepend-inner-icon="mdi-account"
                    ></v-text-field>
                  </v-col>

                  <!-- Gender -->
                  <v-col cols="12" md="6">
                    <v-select
                        v-model="formData.gender"
                        label="Gender"
                        :items="genderOptions"
                        :readonly="!isEditing"
                        :variant="isEditing ? 'outlined' : 'plain'"
                        prepend-inner-icon="mdi-human-male-female"
                        clearable
                    ></v-select>
                  </v-col>

                  <!-- Date of Birth -->
                  <v-col cols="12" md="6">
                    <v-text-field
                        v-model="formData.dateOfBirth"
                        label="Date of Birth"
                        type="date"
                        :readonly="!isEditing"
                        :variant="isEditing ? 'outlined' : 'plain'"
                        :rules="isEditing ? [rules.pastDate] : []"
                        prepend-inner-icon="mdi-calendar"
                    ></v-text-field>
                  </v-col>

                  <!-- Unit Preference -->
                  <v-col cols="12" md="6">
                    <v-select
                        v-model="formData.unitPreference"
                        label="Unit Preference"
                        :items="unitOptions"
                        :readonly="!isEditing"
                        :variant="isEditing ? 'outlined' : 'plain'"
                        prepend-inner-icon="mdi-scale-balance"
                    ></v-select>
                  </v-col>
                </v-row>

                <!-- Action Buttons -->
                <v-row v-if="isEditing" class="mt-4">
                  <v-col cols="12" class="d-flex justify-end gap-2">
                    <v-btn
                        @click="cancelEditing"
                        variant="outlined"
                        :disabled="isSaving"
                    >
                      Cancel
                    </v-btn>
                    <v-btn
                        class="ml-2"
                        type="submit"
                        color="primary"
                        :loading="isSaving"
                        :disabled="!isFormValid || !hasChanges"
                    >
                      Save Changes
                    </v-btn>
                  </v-col>
                </v-row>
              </v-form>
            </v-card-text>
          </v-card>

          <!-- Account Information Card -->
          <v-card>
            <v-card-title class="bg-secondary">
              <v-icon icon="mdi-information" class="mr-2"></v-icon>
              Account Information
            </v-card-title>

            <v-card-text class="pa-6">
              <v-row>
                <v-col cols="12" md="6">
                  <div class="text-subtitle-2 text-medium-emphasis mb-1">Member Since</div>
                  <div class="text-body-1">{{ formatDate(user?.createdAt) }}</div>
                </v-col>
                <v-col cols="12" md="6">
                  <div class="text-subtitle-2 text-medium-emphasis mb-1">Last Updated</div>
                  <div class="text-body-1">{{ formatDate(user?.updatedAt) }}</div>
                </v-col>
                <v-col cols="12" md="6">
                  <div class="text-subtitle-2 text-medium-emphasis mb-1">Login Method</div>
                  <div class="text-body-1">
                    <v-chip size="small" color="primary">{{ user?.loginMethod || 'Google' }}</v-chip>
                  </div>
                </v-col>
                <v-col cols="12" md="6">
                  <div class="text-subtitle-2 text-medium-emphasis mb-1">Roles</div>
                  <div class="text-body-1">
                    <v-chip
                        v-for="role in user?.roles"
                        :key="role"
                        size="small"
                        class="mr-1"
                        color="accent"
                    >
                      {{ role }}
                    </v-chip>
                  </div>
                </v-col>
              </v-row>
            </v-card-text>
          </v-card>
        </v-col>
      </v-row>
    </v-container>

    <!-- Success Snackbar -->
    <v-snackbar
        v-model="showSuccessMessage"
        color="success"
        timeout="3000"
    >
      Profile updated successfully!
      <template v-slot:actions>
        <v-btn variant="text" @click="showSuccessMessage = false">
          Close
        </v-btn>
      </template>
    </v-snackbar>

    <!-- Error Snackbar -->
    <v-snackbar
        v-model="showErrorMessage"
        color="error"
        timeout="5000"
    >
      {{ errorMessage }}
      <template v-slot:actions>
        <v-btn variant="text" @click="showErrorMessage = false">
          Close
        </v-btn>
      </template>
    </v-snackbar>
  </div>
</template>

<script setup lang="ts">
import {ref, computed, watch, onMounted} from 'vue'
import {useAuthStore} from '@/stores/auth'
import api from '@/services/api'

interface UpdateProfileRequest {
  displayName?: string
  gender?: string
  dateOfBirth?: string
  unitPreference?: string
}

const authStore = useAuthStore()
const profileForm = ref()

// Reactive state
const isEditing = ref(false)
const isSaving = ref(false)
const isFormValid = ref(false)
const showSuccessMessage = ref(false)
const showErrorMessage = ref(false)
const errorMessage = ref('')

// User data
const user = computed(() => authStore.user)

// Form data
const formData = ref({
  displayName: '',
  gender: null as string | null,
  dateOfBirth: '',
  unitPreference: 'Metric'
})

// Original data for change detection
const originalData = ref({
  displayName: '',
  gender: null as string | null,
  dateOfBirth: '',
  unitPreference: 'Metric'
})

// Options for selects
const genderOptions = [
  {title: 'Male', value: 'Male'},
  {title: 'Female', value: 'Female'},
  {title: 'Other', value: 'Other'},
  {title: 'Prefer not to say', value: 'PreferNotToSay'}
]

const unitOptions = [
  {title: 'Metric (kg, cm)', value: 'Metric'},
  {title: 'Imperial (lbs, inches)', value: 'Imperial'}
]

// Validation rules
const rules = {
  required: (value: string) => !!value || 'This field is required',
  maxLength: (max: number) => (value: string) =>
      !value || value.length <= max || `Must be ${max} characters or less`,
  pastDate: (value: string) => {
    if (!value) return true
    const date = new Date(value)
    const today = new Date()
    return date < today || 'Date must be in the past'
  }
}

// Computed properties
const hasChanges = computed(() => {
  return (
      formData.value.displayName !== originalData.value.displayName ||
      formData.value.gender !== originalData.value.gender ||
      formData.value.dateOfBirth !== originalData.value.dateOfBirth ||
      formData.value.unitPreference !== originalData.value.unitPreference
  )
})

// Methods
const loadUserData = () => {
  if (user.value) {
    formData.value = {
      displayName: user.value.name || '',
      gender: user.value.gender || null,
      dateOfBirth: user.value.dateOfBirth ? user.value.dateOfBirth.split('T')[0] : '',
      unitPreference: user.value.unitPreference || 'Metric'
    }

    // Store original data
    originalData.value = {...formData.value}
  }
}

const startEditing = () => {
  isEditing.value = true
  loadUserData() // Refresh data when starting to edit
}

const cancelEditing = () => {
  isEditing.value = false
  formData.value = {...originalData.value} // Reset to original values
}

const saveProfile = async () => {
  if (!isFormValid.value || !hasChanges.value) return

  isSaving.value = true

  try {
    const updateRequest: UpdateProfileRequest = {}

    updateRequest.displayName = formData.value.displayName
    updateRequest.gender = formData.value.gender || undefined
    updateRequest.dateOfBirth = formData.value.dateOfBirth || undefined
    updateRequest.unitPreference = formData.value.unitPreference

    const response = await api.put('/users/profile', updateRequest)

    // Update the auth store with the new user data
    authStore.updateUser(response)

    // Update original data to reflect saved changes
    originalData.value = {...formData.value}

    isEditing.value = false
    showSuccessMessage.value = true

  } catch (error: any) {
    console.error('Failed to update profile:', error)
    errorMessage.value = error.response?.data?.error || 'Failed to update profile. Please try again.'
    showErrorMessage.value = true
  } finally {
    isSaving.value = false
  }
}

const formatDate = (dateString?: string) => {
  if (!dateString) return 'N/A'
  return new Date(dateString).toLocaleDateString()
}

// Lifecycle
onMounted(() => {
  loadUserData()
})

// Watch for user changes
watch(user, () => {
  if (user.value && !isEditing.value) {
    loadUserData()
  }
}, {deep: true})
</script>
