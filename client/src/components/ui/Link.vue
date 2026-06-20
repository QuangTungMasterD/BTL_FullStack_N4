<template>
  <component 
    :is="tag"
    :href="tag === 'a' ? to : undefined"
    :to="tag === 'router-link' ? to : undefined"
    :class="[
      'inline-flex items-center gap-1 transition-all duration-200 cursor-pointer',
      variant === 'primary' && 'text-primary/80 hover:text-primary',
      variant === 'secondary' && 'text-secondary hover:text-secondary/80',
      variant === 'error' && 'text-error hover:text-error/80',
      variant === 'success' && 'text-success hover:text-success/80',
      variant === 'muted' && 'text-on-surface-variant hover:text-primary',
      underline && 'underline underline-offset-4 hover:no-underline',
      disabled && 'opacity-50 pointer-events-none',
      size === 'sm' && 'text-label-md',
      size === 'md' && 'text-body-md',
      size === 'lg' && 'text-title-md',
    ]"
    @click="handleClick"
  >
    <span v-if="iconLeft" class="material-symbols-outlined">{{ iconLeft }}</span>
    <slot />
    <span v-if="iconRight" class="material-symbols-outlined">{{ iconRight }}</span>
  </component>
</template>

<script setup>
import { computed } from 'vue';
import { useRouter } from 'vue-router';

const props = defineProps({
  // Đường dẫn
  to: {
    type: String,
    required: true,
  },
  // Biến thể màu
  variant: {
    type: String,
    default: 'primary', // primary, secondary, error, success, muted
  },
  // Kích thước
  size: {
    type: String,
    default: 'md', // sm, md, lg
  },
  // Gạch chân
  underline: {
    type: Boolean,
    default: false,
  },
  // Disabled
  disabled: {
    type: Boolean,
    default: false,
  },
  // Icon
  iconLeft: String,
  iconRight: String,
  // Mở trong tab mới
  external: {
    type: Boolean,
    default: false,
  },
  // Thay thế history (thay vì push)
  replace: {
    type: Boolean,
    default: false,
  },
});

const router = useRouter();

// Xác định tag (a cho external, router-link cho internal)
const tag = computed(() => {
  if (props.external || props.to.startsWith('http')) return 'a';
  return 'router-link';
});

const handleClick = (event) => {
  if (props.disabled) {
    event.preventDefault();
    return;
  }
  
  // Nếu không phải router-link thì xử lý riêng
  if (tag.value !== 'router-link' && !props.external && !props.to.startsWith('http')) {
    event.preventDefault();
    if (props.replace) {
      router.replace(props.to);
    } else {
      router.push(props.to);
    }
  }
};
</script>
