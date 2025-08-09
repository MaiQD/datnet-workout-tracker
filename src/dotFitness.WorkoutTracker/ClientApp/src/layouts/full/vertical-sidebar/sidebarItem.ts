import {
    LayoutDashboardIcon,
    BorderAllIcon,
    AlertCircleIcon,
    CircleDotIcon,
    BoxMultiple1Icon,
    LoginIcon,
    MoodHappyIcon,
    ApertureIcon,
    UserPlusIcon
} from 'vue-tabler-icons';

export interface menu {
    header?: string;
    title?: string;
    icon?: any;
    to?: string;
    chip?: string;
    BgColor?: string;
    chipBgColor?: string;
    chipColor?: string;
    chipVariant?: string;
    chipIcon?: string;
    children?: menu[];
    disabled?: boolean;
    type?: string;
    subCaption?: string;
    external?: boolean;
}

const sidebarItem: menu[] = [
    { header: 'Fitness' },
    {
        title: 'Dashboard',
        icon: 'mdi-view-dashboard',
        to: '/dashboard'
    },
    {
        title: 'Workouts',
        icon: 'mdi-dumbbell',
        to: '/workouts'
    },
    {
        title: 'Exercises',
        icon: 'mdi-format-list-bulleted',
        to: '/exercises'
    },
    {
        title: 'Progress',
        icon: 'mdi-chart-line',
        to: '/progress'
    },
    {
        title: 'Goals',
        icon: 'mdi-target',
        to: '/goals'
    },
    { header: 'Account' },
    {
        title: 'Profile',
        icon: 'mdi-account',
        to: '/profile'
    }
];

export default sidebarItem;
