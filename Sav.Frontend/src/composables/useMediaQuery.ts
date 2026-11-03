import { onUnmounted, ref } from 'vue';

export function useMediaQuery(query: string) {
    const matches = ref(false);

    if (typeof window === 'undefined')
        return matches;

    const mediaQuery = window.matchMedia(query);
    matches.value = mediaQuery.matches;

    const update = (event: MediaQueryListEvent) => {
        matches.value = event.matches;
    };

    mediaQuery.addEventListener('change', update);
    onUnmounted(() => mediaQuery.removeEventListener('change', update));

    return matches;
}
