<template>
    <img
        v-if="source && !failed"
        :src="source"
        :alt="alt"
        :class="imgClass"
        loading="lazy"
        @error="advance" />
    <slot v-else name="fallback"></slot>
</template>

<script setup lang="ts">
import { computed, ref, watch } from 'vue';

const props = defineProps<{
    sources: (string | null | undefined)[];
    alt?: string;
    imgClass?: string;
}>();

// Newer Steam apps only publish their artwork under hashed store_item_assets paths, so the
// constructed ".../steam/apps/{appId}/header.jpg" can 404. Callers pass the stored art as
// extra candidates and we walk through them until one loads.
const candidates = computed(() =>
    Array.from(new Set(
        props.sources
            .filter((src): src is string => typeof src === 'string' && src.trim().length > 0)
            .map(src => src.trim()),
    )));

const index = ref(0);
const failed = ref(false);

const source = computed(() => candidates.value[index.value] ?? '');

const advance = () => {
    if (index.value + 1 < candidates.value.length)
        index.value += 1;
    else
        failed.value = true;
};

// Cards are keyed by position in a list, so a different game can reuse this instance.
watch(() => candidates.value.join('|'), () => {
    index.value = 0;
    failed.value = false;
});
</script>
