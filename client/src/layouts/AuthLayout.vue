<template>
  <div class="auth-layout">
    <div class="particles-container">
      <div v-for="i in 50" :key="i" class="particle" :style="getParticleStyle(i)"></div>
    </div>
    <router-view />
  </div>
</template>

<script setup>
const getParticleStyle = (index) => {
  const size = Math.random() * 6 + 2
  const left = Math.random() * 100
  const duration = Math.random() * 20 + 10
  const delay = Math.random() * 10
  const opacity = Math.random() * 0.3 + 0.1
  
  return {
    width: `${size}px`,
    height: `${size}px`,
    left: `${left}%`,
    animationDuration: `${duration}s`,
    animationDelay: `${delay}s`,
    opacity: opacity,
  }
}
</script>

<style scoped>
.auth-layout {
  min-height: 100vh;
  width: 100%;
  position: relative;
  background: linear-gradient(135deg, #0a0e27 0%, #1a1a3e 25%, #16213e 50%, #0f3460 75%, #1a1a2e 100%);
  background-size: 400% 400%;
  animation: gradientShift 15s ease infinite;
  overflow: hidden;
}

@keyframes gradientShift {
  0% { background-position: 0% 0%; }
  50% { background-position: 100% 100%; }
  100% { background-position: 0% 0%; }
}

.particles-container {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  overflow: hidden;
  z-index: 0;
}

.particle {
  position: absolute;
  bottom: -10px;
  background: linear-gradient(135deg, rgba(79, 172, 254, 0.6), rgba(0, 242, 254, 0.4));
  border-radius: 50%;
  animation: floatUp linear infinite;
  filter: blur(1px);
}

@keyframes floatUp {
  0% {
    transform: translateY(100vh) rotate(0deg);
    opacity: 0;
  }
  10% {
    opacity: 0.8;
  }
  90% {
    opacity: 0.8;
  }
  100% {
    transform: translateY(-100vh) rotate(720deg);
    opacity: 0;
  }
}
</style>