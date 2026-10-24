import { Routes, Route, Navigate } from 'react-router-dom'
import Layout from './components/Layout'
import DashboardPage from './pages/DashboardPage'
import JobsPage from './pages/JobsPage'
import ContactsPage from './pages/ContactsPage'
import ResumesPage from './pages/ResumesPage'
import DocumentsPage from './pages/DocumentsPage'
import ProfilePage from './pages/ProfilePage'
import CompaniesPage from './pages/CompaniesPage'

function App() {
  return (
    <Layout>
      <Routes>
        <Route path="/" element={<Navigate to="/dashboard" replace />} />
        <Route path="/dashboard" element={<DashboardPage />} />
        <Route path="/profile" element={<ProfilePage />} />
        <Route path="/companies" element={<CompaniesPage />} />
        <Route path="/jobs" element={<JobsPage />} />
        <Route path="/contacts" element={<ContactsPage />} />
        <Route path="/resumes" element={<ResumesPage />} />
        <Route path="/documents" element={<DocumentsPage />} />
      </Routes>
    </Layout>
  )
}

export default App
