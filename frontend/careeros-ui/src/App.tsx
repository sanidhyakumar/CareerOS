import { Routes, Route, Navigate } from 'react-router-dom'
import Layout from './components/Layout'
import DashboardPage from './pages/DashboardPage'
import JobsPage from './pages/JobsPage'
import ContactsPage from './pages/ContactsPage'
import ResumesPage from './pages/ResumesPage'
import DocumentsPage from './pages/DocumentsPage'
import ProfilePage from './pages/ProfilePage'
import CompaniesPage from './pages/CompaniesPage'
import ApplicationsPage from './pages/ApplicationsPage'
import InterviewsPage from './pages/InterviewsPage'
import WikiPage from './pages/WikiPage'
import SponsorshipPage from './pages/SponsorshipPage'
import KanbanPage from './pages/KanbanPage'
import FollowUpPage from './pages/FollowUpPage'
import { SearchPage } from './pages/SearchPage'
import { SecurityPage } from './pages/SecurityPage'
import { PortabilityPage } from './pages/PortabilityPage'

function App() {
  return (
    <Layout>
      <Routes>
        <Route path="/" element={<Navigate to="/dashboard" replace />} />
        <Route path="/dashboard" element={<DashboardPage />} />
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
    </Layout>
  )
}

export default App
