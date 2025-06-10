// Theme Management for DatNet Workout Tracker
window.setTheme = function(theme) {
    if (theme === 'dark') {
        document.documentElement.setAttribute('data-theme', 'dark');
        document.body.classList.add('dark-theme');
    } else {
        document.documentElement.setAttribute('data-theme', 'light');
        document.body.classList.remove('dark-theme');
    }
    
    // Update Syncfusion theme
    updateSyncfusionTheme(theme);
};

window.updateSyncfusionTheme = function(theme) {
    const syncfusionLink = document.querySelector('link[href*="syncfusion"]');
    if (syncfusionLink) {
        const baseUrl = 'https://cdn.syncfusion.com/blazor/26.1.35/styles/';
        if (theme === 'dark') {
            syncfusionLink.href = baseUrl + 'bootstrap5-dark.css';
        } else {
            syncfusionLink.href = baseUrl + 'bootstrap5.css';
        }
    }
};

// Initialize theme on page load
window.initializeTheme = function() {
    // Check for saved theme preference or default to light mode
    const savedTheme = localStorage.getItem('theme');
    const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
    const theme = savedTheme || (prefersDark ? 'dark' : 'light');
    
    setTheme(theme);
    
    // Listen for system theme changes
    window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', (e) => {
        if (!localStorage.getItem('theme')) {
            // Only auto-switch if user hasn't set a preference
            setTheme(e.matches ? 'dark' : 'light');
        }
    });
};

// Smooth scroll behavior
window.smoothScrollTo = function(elementId, offset = 0) {
    const element = document.getElementById(elementId);
    if (element) {
        const elementPosition = element.getBoundingClientRect().top + window.pageYOffset;
        const offsetPosition = elementPosition - offset;
        
        window.scrollTo({
            top: offsetPosition,
            behavior: 'smooth'
        });
    }
};

// Animate elements on scroll
window.animateOnScroll = function() {
    const observerOptions = {
        threshold: 0.1,
        rootMargin: '0px 0px -50px 0px'
    };
    
    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('animate-fade-in');
                observer.unobserve(entry.target);
            }
        });
    }, observerOptions);
    
    // Observe all cards and sections
    document.querySelectorAll('.card, .stat-card, .exercise-card, .workout-card, .routine-card').forEach(el => {
        observer.observe(el);
    });
};

// Enhanced card hover effects
window.enhanceCardInteractions = function() {
    const cards = document.querySelectorAll('.card:not(.no-hover)');
    
    cards.forEach(card => {
        card.addEventListener('mouseenter', function() {
            this.style.transform = 'translateY(-4px) scale(1.02)';
            this.style.boxShadow = 'var(--card-shadow-hover)';
        });
        
        card.addEventListener('mouseleave', function() {
            this.style.transform = 'translateY(0) scale(1)';
            this.style.boxShadow = 'var(--card-shadow)';
        });
    });
};

// Initialize everything when DOM is loaded
document.addEventListener('DOMContentLoaded', function() {
    initializeTheme();
    
    // Delay animations to allow proper rendering
    setTimeout(() => {
        animateOnScroll();
        enhanceCardInteractions();
    }, 100);
});

// Utility functions for workout tracking
window.workoutUtils = {
    // Format duration (seconds to HH:MM:SS)
    formatDuration: function(seconds) {
        const hours = Math.floor(seconds / 3600);
        const minutes = Math.floor((seconds % 3600) / 60);
        const secs = seconds % 60;
        
        if (hours > 0) {
            return `${hours}:${minutes.toString().padStart(2, '0')}:${secs.toString().padStart(2, '0')}`;
        }
        return `${minutes}:${secs.toString().padStart(2, '0')}`;
    },
    
    // Vibrate on action (mobile)
    vibrate: function(pattern = [100]) {
        if ('vibrate' in navigator) {
            navigator.vibrate(pattern);
        }
    },
    
    // Show success toast
    showSuccessToast: function(message) {
        // This would integrate with a toast notification system
        console.log('Success:', message);
    },
    
    // Play success sound
    playSuccessSound: function() {
        // Create a subtle success sound
        const audioContext = new (window.AudioContext || window.webkitAudioContext)();
        const oscillator = audioContext.createOscillator();
        const gain = audioContext.createGain();
        
        oscillator.connect(gain);
        gain.connect(audioContext.destination);
        
        oscillator.frequency.setValueAtTime(800, audioContext.currentTime);
        oscillator.frequency.setValueAtTime(1000, audioContext.currentTime + 0.1);
        
        gain.gain.setValueAtTime(0.1, audioContext.currentTime);
        gain.gain.exponentialRampToValueAtTime(0.01, audioContext.currentTime + 0.2);
        
        oscillator.start(audioContext.currentTime);
        oscillator.stop(audioContext.currentTime + 0.2);
    }
};

// Mobile optimizations
if (window.innerWidth <= 768) {
    // Add mobile-specific enhancements
    document.body.classList.add('mobile-device');
    
    // Improve touch interactions
    document.addEventListener('touchstart', function() {}, { passive: true });
}
