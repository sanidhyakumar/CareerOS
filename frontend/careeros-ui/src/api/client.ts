import axios from 'axios'

const client = axios.create({
  baseURL: '/api',
  headers: { 'Content-Type': 'application/json' },
})

client.interceptors.request.use((config) => {
  try {
    const profileId = localStorage.getItem('careeros_active_profile_id')
    if (profileId && config.headers) {
      config.headers['X-Career-Profile-Id'] = profileId
    }
  } catch {
    // LocalStorage access failure fallback
  }
  return config
})

client.interceptors.response.use(
  (response) => response,
  (error) => {
    console.error('API Error:', error.response?.data ?? error.message)
    return Promise.reject(error)
  }
)

export default client

