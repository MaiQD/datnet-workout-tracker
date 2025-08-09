const MainRoutes = {
    path: '/main',
    meta: {
        requiresAuth: true
    },
    redirect: '/dashboard',
    component: () => import('@/layouts/full/FullLayout.vue'),
    children: [
        {
            name: 'Dashboard',
            path: '/dashboard',
            component: () => import('@/views/dashboard/index.vue')
        },
        {
            name: 'Workouts',
            path: '/workouts',
            component: () => import('@/views/workouts/WorkoutsList.vue')
        },
        {
            name: 'NewWorkout',
            path: '/workouts/new',
            component: () => import('@/views/workouts/NewWorkout.vue')
        },
        {
            name: 'Exercises',
            path: '/exercises',
            component: () => import('@/views/exercises/ExercisesList.vue')
        },
        {
            name: 'Progress',
            path: '/progress',
            component: () => import('@/views/progress/Progress.vue')
        },
        {
            name: 'Goals',
            path: '/goals',
            component: () => import('@/views/goals/Goals.vue')
        },
        {
            name: 'Profile',
            path: '/profile',
            component: () => import('@/views/profile/Profile.vue')
        }
    ]
};

export default MainRoutes;
