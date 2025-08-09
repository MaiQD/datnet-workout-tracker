import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

export interface Exercise {
  id: string
  name: string
  description?: string
  muscleGroups: string[]
  equipment: string[]
  tags: string[]
  isGlobal: boolean
  userId?: string
  createdAt: Date
  updatedAt: Date
}

export interface MuscleGroup {
  id: string
  name: string
  bodyRegion: string
  parentId?: string
  isGlobal: boolean
  userId?: string
}

export interface Equipment {
  id: string
  name: string
  isGlobal: boolean
  userId?: string
}

export const useExercisesStore = defineStore('exercises', () => {
  const exercises = ref<Exercise[]>([])
  const muscleGroups = ref<MuscleGroup[]>([])
  const equipment = ref<Equipment[]>([])
  const isLoading = ref(false)

  const globalExercises = computed(() => exercises.value.filter(e => e.isGlobal))
  const userExercises = computed(() => exercises.value.filter(e => !e.isGlobal))
  const globalMuscleGroups = computed(() => muscleGroups.value.filter(m => m.isGlobal))
  const globalEquipment = computed(() => equipment.value.filter(e => e.isGlobal))

  const fetchExercises = async () => {
    isLoading.value = true
    try {
      // TODO: Implement API call to backend
      // const response = await exercisesApi.getAll()
      // exercises.value = response
    } catch (error) {
      console.error('Failed to fetch exercises:', error)
      throw error
    } finally {
      isLoading.value = false
    }
  }

  const fetchMuscleGroups = async () => {
    try {
      // TODO: Implement API call to backend
      // const response = await muscleGroupsApi.getAll()
      // muscleGroups.value = response
    } catch (error) {
      console.error('Failed to fetch muscle groups:', error)
      throw error
    }
  }

  const fetchEquipment = async () => {
    try {
      // TODO: Implement API call to backend
      // const response = await equipmentApi.getAll()
      // equipment.value = response
    } catch (error) {
      console.error('Failed to fetch equipment:', error)
      throw error
    }
  }

  const createExercise = async (exerciseData: Omit<Exercise, 'id' | 'createdAt' | 'updatedAt'>) => {
    try {
      // TODO: Implement API call to backend
      // const response = await exercisesApi.create(exerciseData)
      // exercises.value.push(response)
      // return response
    } catch (error) {
      console.error('Failed to create exercise:', error)
      throw error
    }
  }

  const updateExercise = async (id: string, exerciseData: Partial<Exercise>) => {
    try {
      // TODO: Implement API call to backend
      // const response = await exercisesApi.update(id, exerciseData)
      // const index = exercises.value.findIndex(e => e.id === id)
      // if (index !== -1) {
      //   exercises.value[index] = response
      // }
      // return response
    } catch (error) {
      console.error('Failed to update exercise:', error)
      throw error
    }
  }

  const deleteExercise = async (id: string) => {
    try {
      // TODO: Implement API call to backend
      // await exercisesApi.delete(id)
      // exercises.value = exercises.value.filter(e => e.id !== id)
    } catch (error) {
      console.error('Failed to delete exercise:', error)
      throw error
    }
  }

  return {
    exercises,
    muscleGroups,
    equipment,
    isLoading,
    globalExercises,
    userExercises,
    globalMuscleGroups,
    globalEquipment,
    fetchExercises,
    fetchMuscleGroups,
    fetchEquipment,
    createExercise,
    updateExercise,
    deleteExercise
  }
})
