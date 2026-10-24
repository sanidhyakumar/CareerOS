import { useEffect, useState } from 'react'
import { Grid, Typography, Box, Alert, CircularProgress } from '@mui/material'
import WorkIcon from '@mui/icons-material/Work'
import PeopleIcon from '@mui/icons-material/People'
import DescriptionIcon from '@mui/icons-material/Description'
import SendIcon from '@mui/icons-material/Send'
import InterpreterModeIcon from '@mui/icons-material/InterpreterMode'
import EmojiEventsIcon from '@mui/icons-material/EmojiEvents'
import StatCard from '../components/StatCard'
import { dashboardApi, type DashboardStats } from '../api/dashboard'

export default function DashboardPage() {
  const [stats, setStats] = useState<DashboardStats | null>(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    dashboardApi.getStats()
      .then(setStats)
      .catch(() => setError('Could not load stats. Make sure the API is running.'))
      .finally(() => setLoading(false))
  }, [])

  if (loading) return <Box display="flex" justifyContent="center" mt={8}><CircularProgress /></Box>

  return (
    <Box>
      <Typography variant="h5" gutterBottom fontWeight={700}>Welcome back 👋</Typography>
      <Typography variant="body2" color="text.secondary" mb={3}>
        Your career journey at a glance.
      </Typography>

      {error && <Alert severity="warning" sx={{ mb: 3 }}>{error}</Alert>}

      <Grid container spacing={3}>
        <Grid item xs={12} sm={6} md={4}>
          <StatCard title="Total Jobs" value={stats?.totalJobs ?? 0} icon={<WorkIcon />} color="#5C6BC0" />
        </Grid>
        <Grid item xs={12} sm={6} md={4}>
          <StatCard title="Total Contacts" value={stats?.totalContacts ?? 0} icon={<PeopleIcon />} color="#26C6DA" />
        </Grid>
        <Grid item xs={12} sm={6} md={4}>
          <StatCard title="Resumes" value={stats?.totalResumes ?? 0} icon={<DescriptionIcon />} color="#AB47BC" />
        </Grid>
        <Grid item xs={12} sm={6} md={4}>
          <StatCard title="Applications Sent" value={stats?.appliedJobs ?? 0} icon={<SendIcon />} color="#42A5F5" />
        </Grid>
        <Grid item xs={12} sm={6} md={4}>
          <StatCard title="Interviews" value={stats?.interviewJobs ?? 0} icon={<InterpreterModeIcon />} color="#FF9800" />
        </Grid>
        <Grid item xs={12} sm={6} md={4}>
          <StatCard title="Offers" value={stats?.offersReceived ?? 0} icon={<EmojiEventsIcon />} color="#4CAF50" />
        </Grid>
      </Grid>
    </Box>
  )
}

