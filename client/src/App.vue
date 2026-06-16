<template>
  <router-view />
</template>

<script setup>
// App now only contains router-view - no global layout
</script>

<style>
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

html, body {
  height: 100%;
  width: 100%;
  overflow-x: hidden;
}

#app {
  height: 100%;
  width: 100%;
}
</style>