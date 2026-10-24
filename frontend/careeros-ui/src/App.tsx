import { Routes, Route, Navigate } from 'react-router-dom'
import Layout from './components/Layout'
import DashboardPage from './pages/DashboardPage'
import JobsPage from './pages/JobsPage'
import ContactsPage from './pages/ContactsPage'
import ResumesPage from './pages/ResumesPage'

function App() {
  return (
    <Layout>
      <Routes>
        <Route path="/" element={<Navigate to="/dashboard" replace />} />
        <Route path="/dashboard" element={<DashboardPage />} />
        <Route path="/jobs" element={<JobsPage />} />
        <Route path="/contacts" element={<ContactsPage />} />
        <Route path="/resumes" element={<ResumesPage />} />
      </Routes>
    </Layout>
  )
}

export default App
