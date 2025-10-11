export const designTokens = {
  colors: {
    primary: '#3B82F6',
    success: '#22C55E',
    warning: '#F59E0B',
    error: '#EF4444',
    
    actionCards: {
      green: { bg: '#EBF9F1', accent: '#22C55E', hover: '#D1F2E0' },
      blue: { bg: '#EBF5FF', accent: '#3B82F6', hover: '#D4E7FE' },
      orange: { bg: '#FFF9EB', accent: '#F59E0B', hover: '#FFEED1' },
      red: { bg: '#FEF2F2', accent: '#EF4444', hover: '#FEE2E2' },
      purple: { bg: '#F3F0FF', accent: '#8B5CF6', hover: '#E9DEFF' },
    },
    
    statusBadges: {
      good: { bg: '#D1FAE5', text: '#065F46' },
      stronger: { bg: '#FEF3C7', text: '#92400E' },
      relaxed: { bg: '#DBEAFE', text: '#1E40AF' },
    },
    
    avatarCircles: {
      blue: '#DBEAFE',
      purple: '#E9D5FF',
      red: '#FECACA',
      green: '#D1FAE5',
    },
  },
  
  spacing: {
    cardPadding: '1.5rem',
    cardGap: '1.5rem',
    sectionGap: '1.5rem',
  },
  
  borderRadius: {
    card: '0.75rem',
    button: '0.5rem',
    badge: '9999px',
  },
  
  shadows: {
    card: '0 1px 3px 0 rgb(0 0 0 / 0.1), 0 1px 2px -1px rgb(0 0 0 / 0.1)',
    cardHover: '0 4px 6px -1px rgb(0 0 0 / 0.1), 0 2px 4px -2px rgb(0 0 0 / 0.1)',
  },
  
  transitions: {
    default: 'all 0.2s ease-in-out',
  },
  
  typography: {
    fontFamily: "'Inter', -apple-system, BlinkMacSystemFont, sans-serif",
    weights: {
      normal: 400,
      medium: 500,
      semibold: 600,
      bold: 700,
      extrabold: 800,
    },
  },
};

export type ActionCardColor = keyof typeof designTokens.colors.actionCards;
export type StatusBadgeType = keyof typeof designTokens.colors.statusBadges;
export type AvatarCircleColor = keyof typeof designTokens.colors.avatarCircles;
