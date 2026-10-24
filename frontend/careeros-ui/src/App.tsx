import { lazy, Suspense } from 'react'
import { Routes, Route, Navigate } from 'react-router-dom'
import { Box, LinearProgress } from '@mui/material'
import Layout from './components/Layout'
import { ProfileProvider } from './context/ProfileContext'

const DashboardPage = lazy(() => import('./pages/DashboardPage'))
const ProfilesPage = lazy(() => import('./pages/ProfilesPage'))
const JobsPage = lazy(() => import('./pages/JobsPage'))
const ContactsPage = lazy(() => import('./pages/ContactsPage'))
const ResumesPage = lazy(() => import('./pages/ResumesPage'))
const DocumentsPage = lazy(() => import('./pages/DocumentsPage'))
const ProfilePage = lazy(() => import('./pages/ProfilePage'))
const CompaniesPage = lazy(() => import('./pages/CompaniesPage'))
const ApplicationsPage = lazy(() => import('./pages/ApplicationsPage'))
const InterviewsPage = lazy(() => import('./pages/InterviewsPage'))
const WikiPage = lazy(() => import('./pages/WikiPage'))
const SponsorshipPage = lazy(() => import('./pages/SponsorshipPage'))
const KanbanPage = lazy(() => import('./pages/KanbanPage'))
const FollowUpPage = lazy(() => import('./pages/FollowUpPage'))
const SearchPage = lazy(() => import('./pages/SearchPage').then(m => ({ default: m.SearchPage })))
const SecurityPage = lazy(() => import('./pages/SecurityPage').then(m => ({ default: m.SecurityPage })))
const PortabilityPage = lazy(() => import('./pages/PortabilityPage').then(m => ({ default: m.PortabilityPage })))

function App() {
  return (
    <ProfileProvider>
      <Layout>
        <Suspense fallback={<Box sx={{ width: '100%', mt: 2 }}><LinearProgress /></Box>}>
          <Routes>
            <Route path="/" element={<Navigate to="/dashboard" replace />} />
            <Route path="/dashboard" element={<DashboardPage />} />
            <Route path="/profiles" element={<ProfilesPage />} />
            <Route path="/search" element={<SearchPage />} />
          <Route path="/security" element={<SecurityPage />} />
          <Route path="/portability" element={<PortabilityPage />} />
          <Route path="/profile" element={<ProfilePage />} />
          <Route path="/companies" element={<CompaniesPage />} />
          <Route path="/jobs" element={<JobsPage />} />
          <Route path="/board" element={<KanbanPage />} />
          <Route path="/applications" element={<ApplicationsPage />} />
          <Route path="/followup" element={<FollowUpPage />} />
          <Route path="/interviews" element={<InterviewsPage />} />
          <Route path="/wiki" element={<WikiPage />} />
          <Route path="/sponsorship" element={<SponsorshipPage />} />
          <Route path="/contacts" element={<ContactsPage />} />
          <Route path="/resumes" element={<ResumesPage />} />
          <Route path="/documents" element={<DocumentsPage />} />
        </Routes>
      </Suspense>
    </Layout>
  </ProfileProvider>
  )
}

export default App
