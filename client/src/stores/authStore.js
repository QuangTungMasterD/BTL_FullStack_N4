import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

export const useAuthStore = defineStore('auth', () => {

  const token = ref(
    localStorage.getItem('authToken') || null
  )

  const user = ref(
    JSON.parse(
      localStorage.getItem('user') || 'null'
    )
  )

  const isAuthenticated = computed(
    () => !!token.value
  )

  const setToken = (newToken, userData) => {

    token.value = newToken
    user.value = userData

    localStorage.setItem(
      'authToken',
      newToken
    )

    localStorage.setItem(
      'user',
      JSON.stringify(userData)
    )
  }

  const logout = () => {

    token.value = null
    user.value = null

    localStorage.removeItem('authToken')
    localStorage.removeItem('user')
  }

  const userRole = computed(
    () => user.value?.role || null
  )

  return {
    token,
    user,
    isAuthenticated,
    userRole,
    setToken,
    logout
  }
})