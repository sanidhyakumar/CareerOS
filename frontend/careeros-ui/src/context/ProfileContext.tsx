import React, { createContext, useContext, useState, useEffect } from 'react'
import { type CareerProfileDto, profilesApi } from '../api/profiles'

interface ProfileContextType {
  activeProfile: CareerProfileDto | null
  profiles: CareerProfileDto[]
  loading: boolean
  switchProfile: (id: string) => Promise<void>
  refreshProfiles: () => Promise<void>
}

const ProfileContext = createContext<ProfileContextType>({
  activeProfile: null,
  profiles: [],
  loading: true,
  switchProfile: async () => {},
  refreshProfiles: async () => {},
})

export const useProfile = () => useContext(ProfileContext)

export const ProfileProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [activeProfile, setActiveProfile] = useState<CareerProfileDto | null>(null)
  const [profiles, setProfiles] = useState<CareerProfileDto[]>([])
  const [loading, setLoading] = useState<boolean>(true)

  const loadData = async () => {
    try {
      setLoading(true)
      const list = await profilesApi.getAll()
      setProfiles(list)

      const savedId = localStorage.getItem('careeros_active_profile_id')
      if (savedId) {
        const found = list.find((p) => p.id === savedId)
        if (found) {
          setActiveProfile(found)
          return
        }
      }

      // Default to active from backend or primary
      const active = await profilesApi.getActive()
      setActiveProfile(active)
      if (active) {
        localStorage.setItem('careeros_active_profile_id', active.id)
      }
    } catch (err) {
      console.warn('Failed to load profile data', err)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    loadData()
  }, [])

  const switchProfile = async (id: string) => {
    try {
      await profilesApi.setActive(id)
      const target = profiles.find((p) => p.id === id)
      if (target) {
        setActiveProfile(target)
        localStorage.setItem('careeros_active_profile_id', id)
      }
      await refreshProfiles()
    } catch (err) {
      console.error('Failed to switch profile', err)
    }
  }

  const refreshProfiles = async () => {
    try {
      const list = await profilesApi.getAll()
      setProfiles(list)
      const active = list.find((p) => p.id === activeProfile?.id) || list.find((p) => p.isActive) || list[0]
      if (active) {
        setActiveProfile(active)
        localStorage.setItem('careeros_active_profile_id', active.id)
      }
    } catch (err) {
      console.warn('Failed to refresh profiles', err)
    }
  }

  return (
    <ProfileContext.Provider
      value={{
        activeProfile,
        profiles,
        loading,
        switchProfile,
        refreshProfiles,
      }}
    >
      {children}
    </ProfileContext.Provider>
  )
}
