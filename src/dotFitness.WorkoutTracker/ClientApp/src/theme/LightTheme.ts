import type { ThemeTypes } from '@/types/themeTypes/ThemeType';

const BLUE_THEME: ThemeTypes = {
    name: 'BLUE_THEME',
    dark: false,
    variables: {
        'border-color': '#E5E7EB'
    },
    colors: {
        // Primary Colors (Light Blue)
        primary: '#3B82F6',
        lightprimary: '#EBF5FF',
        
        // Semantic Colors
        success: '#22C55E',
        lightsuccess: '#EBF9F1',
        warning: '#F59E0B',
        lightwarning: '#FFF9EB',
        error: '#EF4444',
        lighterror: '#FEF2F2',
        info: '#3B82F6',
        lightinfo: '#EBF5FF',
        
        // Secondary Colors
        secondary: '#6B7280',
        lightsecondary: '#F3F4F6',
        
        // Accent Colors for Actions
        actionGreen: '#22C55E',
        actionGreenLight: '#EBF9F1',
        actionOrange: '#F59E0B',
        actionOrangeLight: '#FFF9EB',
        actionPurple: '#8B5CF6',
        actionPurpleLight: '#F3F0FF',
        actionRed: '#EF4444',
        actionRedLight: '#FEF2F2',
        
        // Backgrounds
        background: '#F0F4F8',
        surface: '#FFFFFF',
        containerBg: '#FFFFFF',
        bglight: '#F9FAFB',
        
        // Text Colors
        textPrimary: '#111827',
        textSecondary: '#6B7280',
        
        // Borders
        borderColor: '#E5E7EB',
        inputBorder: '#D1D5DB',
        
        // Grays
        grey50: '#F9FAFB',
        grey100: '#6B7280',
        grey200: '#111827',
        grey300: '#D1D5DB',
        grey400: '#9CA3AF',
        grey500: '#6B7280',
        grey600: '#4B5563',
        grey700: '#374151',
        grey800: '#1F2937',
        grey900: '#111827',
        
        // Hover States
        hoverColor: '#F9FAFB',
        
        // Legacy colors for compatibility
        indigo:'#8B5CF6',
        lightindigo:'#F3F0FF',
        darkbg:'#374151',
        bgdark:'#111827'
    }
};


export { BLUE_THEME};
