<template>
  <div v-if="active">
    <slot />
  </div>
</template>

<script setup>
defineProps({ name: String, active: Boolean });
</script>