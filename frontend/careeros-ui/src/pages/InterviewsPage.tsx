import React, { useEffect, useState } from 'react'
import {
  Box, Typography, Paper, Table, TableBody, TableCell, TableContainer,
  TableHead, TableRow, IconButton, Dialog, DialogTitle, DialogContent,
  DialogActions, Button, TextField, CircularProgress, Alert, Grid, Card,
  CardContent, Chip, Tooltip, InputAdornment, FormControl, InputLabel,
  Select, MenuItem, ToggleButtonGroup, ToggleButton, Divider, Stack, Tabs, Tab,
  Rating
} from '@mui/material'
import AddIcon from '@mui/icons-material/Add'
import DeleteIcon from '@mui/icons-material/Delete'
import EditIcon from '@mui/icons-material/Edit'
import SearchIcon from '@mui/icons-material/Search'
import EventIcon from '@mui/icons-material/Event'
import VideoCallIcon from '@mui/icons-material/VideoCall'
import PhoneIcon from '@mui/icons-material/Phone'
import BusinessIcon from '@mui/icons-material/Business'
import TimelineIcon from '@mui/icons-material/Timeline'
import CalendarMonthIcon from '@mui/icons-material/CalendarMonth'
import ViewListIcon from '@mui/icons-material/ViewList'
import CheckCircleIcon from '@mui/icons-material/CheckCircle'
import LaunchIcon from '@mui/icons-material/Launch'
import PersonIcon from '@mui/icons-material/Person'
import CodeIcon from '@mui/icons-material/Code'
import ArchitectureIcon from '@mui/icons-material/Architecture'
import PsychologyIcon from '@mui/icons-material/Psychology'
import SupervisorAccountIcon from '@mui/icons-material/SupervisorAccount'
import StarIcon from '@mui/icons-material/Star'

import {
  interviewsApi, InterviewType, InterviewFormat, InterviewStatus, InterviewOutcome
} from '../api/interviews'
import type {
  Interview, InterviewStats, CreateInterviewRequest, UpdateInterviewRequest
} from '../api/interviews'
import { companiesApi } from '../api/companies'
import type { Company } from '../api/companies'
import { applicationsApi } from '../api/applications'
import type { JobApplication } from '../api/applications'
import { contactsApi } from '../api/contacts'
import type { Contact } from '../api/contacts'

const TYPE_CONFIG: Record<InterviewType, { label: string; icon: React.ReactElement; color: 'primary' | 'secondary' | 'info' | 'success' | 'warning' | 'default' }> = {
  [InterviewType.Recruiter]: { label: 'Recruiter Screen', icon: <PersonIcon fontSize="small" />, color: 'info' },
  [InterviewType.Technical]: { label: 'Technical Coding', icon: <CodeIcon fontSize="small" />, color: 'primary' },
  [InterviewType.SystemDesign]: { label: 'System Design', icon: <ArchitectureIcon fontSize="small" />, color: 'secondary' },
  [InterviewType.Behavioral]: { label: 'Behavioral & Leadership', icon: <PsychologyIcon fontSize="small" />, color: 'success' },
  [InterviewType.Manager]: { label: 'Engineering Manager', icon: <SupervisorAccountIcon fontSize="small" />, color: 'warning' },
  [InterviewType.Final]: { label: 'Final Round / Onsite', icon: <StarIcon fontSize="small" />, color: 'secondary' },
  [InterviewType.TakeHomeReview]: { label: 'Take-Home Review', icon: <CodeIcon fontSize="small" />, color: 'default' },
  [InterviewType.Other]: { label: 'Other Round', icon: <EventIcon fontSize="small" />, color: 'default' },
}

const FORMAT_CONFIG: Record<InterviewFormat, { label: string; icon: React.ReactElement }> = {
  [InterviewFormat.VideoCall]: { label: 'Video Call', icon: <VideoCallIcon fontSize="small" /> },
  [InterviewFormat.Phone]: { label: 'Phone', icon: <PhoneIcon fontSize="small" /> },
  [InterviewFormat.Onsite]: { label: 'In-Person Onsite', icon: <BusinessIcon fontSize="small" /> },
  [InterviewFormat.Asynchronous]: { label: 'Asynchronous', icon: <EventIcon fontSize="small" /> },
}

const OUTCOME_CONFIG: Record<InterviewOutcome, { label: string; color: 'default' | 'success' | 'error' | 'warning' }> = {
  [InterviewOutcome.Pending]: { label: 'Pending Outcome', color: 'default' },
  [InterviewOutcome.Passed]: { label: 'Passed', color: 'success' },
  [InterviewOutcome.StrongHire]: { label: 'Strong Hire', color: 'success' },
  [InterviewOutcome.Hire]: { label: 'Hire', color: 'success' },
  [InterviewOutcome.Neutral]: { label: 'Neutral', color: 'warning' },
  [InterviewOutcome.Failed]: { label: 'Did Not Advance', color: 'error' },
  [InterviewOutcome.NoHire]: { label: 'No Hire', color: 'error' },
}

const emptyForm: CreateInterviewRequest = {
  title: '',
  interviewType: InterviewType.Technical,
  format: InterviewFormat.VideoCall,
  scheduledAt: new Date(Date.now() + 86400000).toISOString().substring(0, 16),
  durationMinutes: 60,
  jobApplicationId: undefined,
  jobId: undefined,
  companyId: undefined,
  meetingLink: '',
  location: '',
  interviewerNames: '',
  preparationNotes: '',
  interviewerContactIds: [],
}

export default function InterviewsPage() {
  const [interviews, setInterviews] = useState<Interview[]>([])
  const [stats, setStats] = useState<InterviewStats | null>(null)
  const [companies, setCompanies] = useState<Company[]>([])
  const [applications, setApplications] = useState<JobApplication[]>([])
  const [contacts, setContacts] = useState<Contact[]>([])

  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [success, setSuccess] = useState<string | null>(null)

  // View & Filters
  const [viewMode, setViewMode] = useState<'timeline' | 'calendar' | 'table'>('timeline')
  const [searchTerm, setSearchTerm] = useState('')
  const [selectedTypeTab, setSelectedTypeTab] = useState<string>('all')
  const [selectedStatus, setSelectedStatus] = useState<string>('all')
  const [selectedCompanyId, setSelectedCompanyId] = useState<string>('all')

  // Create / Edit modal
  const [dialogOpen, setDialogOpen] = useState(false)
  const [editInterviewId, setEditInterviewId] = useState<string | null>(null)
  const [form, setForm] = useState<CreateInterviewRequest>(emptyForm)
  const [editOutcome, setEditOutcome] = useState<InterviewOutcome>(InterviewOutcome.Pending)
  const [editStatus, setEditStatus] = useState<InterviewStatus>(InterviewStatus.Scheduled)
  const [editQuestions, setEditQuestions] = useState('')
  const [editFeedback, setEditFeedback] = useState('')
  const [editRating, setEditRating] = useState<number | null>(null)
  const [saving, setSaving] = useState(false)

  // Quick feedback modal
  const [quickCompleteOpen, setQuickCompleteOpen] = useState(false)
  const [quickTarget, setQuickTarget] = useState<Interview | null>(null)
  const [quickOutcome, setQuickOutcome] = useState<InterviewOutcome>(InterviewOutcome.Passed)
  const [quickFeedback, setQuickFeedback] = useState('')
  const [quickRating, setQuickRating] = useState<number | null>(4)

  const loadData = async () => {
    setLoading(true)
    try {
      const typeVal = selectedTypeTab !== 'all' ? (Number(selectedTypeTab) as InterviewType) : undefined
      const statusVal = selectedStatus !== 'all' ? (Number(selectedStatus) as InterviewStatus) : undefined
      const companyVal = selectedCompanyId !== 'all' ? selectedCompanyId : undefined

      const [interviewsData, statsData, companiesData, appsData, contactsData] =
        await Promise.all([
          interviewsApi.getAll({
            type: typeVal,
            status: statusVal,
            companyId: companyVal,
            searchTerm: searchTerm || undefined,
          }),
          interviewsApi.getStats().catch(() => null),
          companiesApi.getAll().catch(() => []),
          applicationsApi.getAll().catch(() => []),
          contactsApi.getAll().catch(() => []),
        ])

      setInterviews(interviewsData)
      setStats(statsData)
      setCompanies(companiesData)
      setApplications(appsData)
      setContacts(contactsData)
    } catch {
      setError('Failed to load interview center data.')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    loadData()
  }, [searchTerm, selectedTypeTab, selectedStatus, selectedCompanyId])

  const openCreate = () => {
    setEditInterviewId(null)
    setForm(emptyForm)
    setEditStatus(InterviewStatus.Scheduled)
    setEditOutcome(InterviewOutcome.Pending)
    setEditQuestions('')
    setEditFeedback('')
    setEditRating(null)
    setDialogOpen(true)
  }

  const openEdit = (item: Interview) => {
    setEditInterviewId(item.id)
    setForm({
      title: item.title,
      interviewType: item.interviewType,
      format: item.format,
      scheduledAt: item.scheduledAt ? item.scheduledAt.substring(0, 16) : new Date().toISOString().substring(0, 16),
      durationMinutes: item.durationMinutes,
      jobApplicationId: item.jobApplicationId,
      jobId: item.jobId,
      companyId: item.companyId,
      meetingLink: item.meetingLink || '',
      location: item.location || '',
      interviewerNames: item.interviewerNames || '',
      preparationNotes: item.preparationNotes || '',
      interviewerContactIds: (item.interviewers || []).map((ii) => ii.contactId),
    })
    setEditStatus(item.status)
    setEditOutcome(item.outcome)
    setEditQuestions(item.questionsAsked || '')
    setEditFeedback(item.feedbackNotes || '')
    setEditRating(item.rating || null)
    setDialogOpen(true)
  }

  const handleSave = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!form.title.trim()) return

    setSaving(true)
    setError(null)
    try {
      if (editInterviewId) {
        const updatePayload: UpdateInterviewRequest = {
          ...form,
          id: editInterviewId,
          status: editStatus,
          outcome: editOutcome,
          scheduledAt: new Date(form.scheduledAt).toISOString(),
          questionsAsked: editQuestions.trim() || undefined,
          feedbackNotes: editFeedback.trim() || undefined,
          rating: editRating || undefined,
        }
        await interviewsApi.update(editInterviewId, updatePayload)
        setSuccess('Interview round updated.')
      } else {
        await interviewsApi.create({
          ...form,
          scheduledAt: new Date(form.scheduledAt).toISOString(),
        })
        setSuccess('Interview round scheduled and recorded.')
      }
      setDialogOpen(false)
      loadData()
    } catch {
      setError('Failed to save interview.')
    } finally {
      setSaving(false)
    }
  }

  const handleDelete = async (id: string) => {
    if (!window.confirm('Delete this interview record?')) return
    try {
      await interviewsApi.delete(id)
      setSuccess('Interview deleted.')
      loadData()
    } catch {
      setError('Failed to delete interview.')
    }
  }

  const handleOpenQuickComplete = (item: Interview) => {
    setQuickTarget(item)
    setQuickOutcome(InterviewOutcome.Passed)
    setQuickFeedback(item.feedbackNotes || '')
    setQuickRating(item.rating || 4)
    setQuickCompleteOpen(true)
  }

  const handleSaveQuickComplete = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!quickTarget) return

    try {
      await interviewsApi.updateStatus(quickTarget.id, {
        status: InterviewStatus.Completed,
        outcome: quickOutcome,
        feedbackNotes: quickFeedback.trim() || undefined,
        rating: quickRating || undefined,
      })
      setSuccess('Interview completed and outcome recorded.')
      setQuickCompleteOpen(false)
      loadData()
    } catch {
      setError('Failed to complete interview.')
    }
  }

  return (
    <Box sx={{ p: 3 }}>
      {/* Header */}
      <Box display="flex" justifyContent="space-between" alignItems="flex-start" mb={3}>
        <Box>
          <Box display="flex" alignItems="center" gap={1.5}>
            <CalendarMonthIcon color="primary" sx={{ fontSize: 32 }} />
            <Typography variant="h4" fontWeight={700}>Interview Center</Typography>
          </Box>
          <Typography variant="body2" color="text.secondary" mt={0.5}>
            Manage interview rounds, technical coding screens, system designs, behavioral prep, and post-interview debriefs
          </Typography>
        </Box>
        <Button
          variant="contained"
          startIcon={<AddIcon />}
          onClick={openCreate}
          sx={{ borderRadius: 2 }}
        >
          Schedule Interview
        </Button>
      </Box>

      {/* Notifications */}
      {error && <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError(null)}>{error}</Alert>}
      {success && <Alert severity="success" sx={{ mb: 2 }} onClose={() => setSuccess(null)}>{success}</Alert>}

      {/* KPI Stats Cards */}
      {stats && (
        <Grid container spacing={2} mb={3}>
          <Grid item xs={6} sm={2}>
            <Card variant="outlined">
              <CardContent sx={{ py: 1.5, '&:last-child': { pb: 1.5 } }}>
                <Typography variant="caption" color="text.secondary">Total Rounds</Typography>
                <Typography variant="h5" fontWeight={700} color="primary.main">{stats.totalInterviews}</Typography>
              </CardContent>
            </Card>
          </Grid>
          <Grid item xs={6} sm={2}>
            <Card variant="outlined">
              <CardContent sx={{ py: 1.5, '&:last-child': { pb: 1.5 } }}>
                <Typography variant="caption" color="text.secondary">Upcoming</Typography>
                <Typography variant="h5" fontWeight={700} color="info.main">{stats.upcomingCount}</Typography>
              </CardContent>
            </Card>
          </Grid>
          <Grid item xs={6} sm={2}>
            <Card variant="outlined">
              <CardContent sx={{ py: 1.5, '&:last-child': { pb: 1.5 } }}>
                <Typography variant="caption" color="text.secondary">Technical Coding</Typography>
                <Typography variant="h5" fontWeight={700} color="primary.light">{stats.technicalCount}</Typography>
              </CardContent>
            </Card>
          </Grid>
          <Grid item xs={6} sm={2}>
            <Card variant="outlined">
              <CardContent sx={{ py: 1.5, '&:last-child': { pb: 1.5 } }}>
                <Typography variant="caption" color="text.secondary">System Design</Typography>
                <Typography variant="h5" fontWeight={700} color="secondary.main">{stats.systemDesignCount}</Typography>
              </CardContent>
            </Card>
          </Grid>
          <Grid item xs={6} sm={2}>
            <Card variant="outlined">
              <CardContent sx={{ py: 1.5, '&:last-child': { pb: 1.5 } }}>
                <Typography variant="caption" color="text.secondary">Behavioral</Typography>
                <Typography variant="h5" fontWeight={700} color="success.main">{stats.behavioralCount}</Typography>
              </CardContent>
            </Card>
          </Grid>
          <Grid item xs={6} sm={2}>
            <Card variant="outlined">
              <CardContent sx={{ py: 1.5, '&:last-child': { pb: 1.5 } }}>
                <Typography variant="caption" color="text.secondary">Pass Conversion</Typography>
                <Typography variant="h5" fontWeight={700} color="warning.main">{stats.passRate}%</Typography>
              </CardContent>
            </Card>
          </Grid>
        </Grid>
      )}

      {/* Round Type Tabs */}
      <Paper variant="outlined" sx={{ mb: 2 }}>
        <Tabs
          value={selectedTypeTab}
          onChange={(_, val) => setSelectedTypeTab(val)}
          variant="scrollable"
          scrollButtons="auto"
          sx={{ px: 2, minHeight: 48 }}
        >
          <Tab label="All Interview Rounds" value="all" />
          <Tab label="Recruiter Screen" value={String(InterviewType.Recruiter)} />
          <Tab label="Technical Coding" value={String(InterviewType.Technical)} />
          <Tab label="System Design" value={String(InterviewType.SystemDesign)} />
          <Tab label="Behavioral & Leadership" value={String(InterviewType.Behavioral)} />
          <Tab label="Engineering Manager" value={String(InterviewType.Manager)} />
          <Tab label="Final Round / Onsite" value={String(InterviewType.Final)} />
        </Tabs>
      </Paper>

      {/* Filter & View Toolbar */}
      <Paper variant="outlined" sx={{ p: 2, mb: 3 }}>
        <Grid container spacing={2} alignItems="center">
          <Grid item xs={12} sm={5}>
            <TextField
              placeholder="Search title, company, questions asked, prep notes..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              size="small"
              fullWidth
              InputProps={{
                startAdornment: (
                  <InputAdornment position="start">
                    <SearchIcon fontSize="small" />
                  </InputAdornment>
                ),
              }}
            />
          </Grid>
          <Grid item xs={6} sm={2.5}>
            <FormControl size="small" fullWidth>
              <InputLabel>Company</InputLabel>
              <Select
                value={selectedCompanyId}
                label="Company"
                onChange={(e) => setSelectedCompanyId(e.target.value)}
              >
                <MenuItem value="all">All Companies</MenuItem>
                {companies.map((c) => (
                  <MenuItem key={c.id} value={c.id}>{c.name}</MenuItem>
                ))}
              </Select>
            </FormControl>
          </Grid>
          <Grid item xs={6} sm={2}>
            <FormControl size="small" fullWidth>
              <InputLabel>Status</InputLabel>
              <Select
                value={selectedStatus}
                label="Status"
                onChange={(e) => setSelectedStatus(e.target.value)}
              >
                <MenuItem value="all">All Statuses</MenuItem>
                <MenuItem value={InterviewStatus.Scheduled}>Scheduled</MenuItem>
                <MenuItem value={InterviewStatus.Completed}>Completed</MenuItem>
                <MenuItem value={InterviewStatus.Cancelled}>Cancelled</MenuItem>
              </Select>
            </FormControl>
          </Grid>
          <Grid item xs={12} sm={2.5} display="flex" justifyContent="flex-end">
            <ToggleButtonGroup
              size="small"
              value={viewMode}
              exclusive
              onChange={(_, v) => { if (v) setViewMode(v) }}
            >
              <ToggleButton value="timeline">
                <Tooltip title="Timeline View"><TimelineIcon fontSize="small" /></Tooltip>
              </ToggleButton>
              <ToggleButton value="calendar">
                <Tooltip title="Schedule / Calendar View"><CalendarMonthIcon fontSize="small" /></Tooltip>
              </ToggleButton>
              <ToggleButton value="table">
                <Tooltip title="Table View"><ViewListIcon fontSize="small" /></Tooltip>
              </ToggleButton>
            </ToggleButtonGroup>
          </Grid>
        </Grid>
      </Paper>

      {/* Content Rendering */}
      {loading ? (
        <Box display="flex" justifyContent="center" mt={8}><CircularProgress /></Box>
      ) : interviews.length === 0 ? (
        <Box textAlign="center" py={8} color="text.secondary">
          <CalendarMonthIcon sx={{ fontSize: 64, opacity: 0.3 }} />
          <Typography mt={2}>No interviews found in this view.</Typography>
          <Button variant="outlined" startIcon={<AddIcon />} onClick={openCreate} sx={{ mt: 2 }}>
            Schedule an Interview Round
          </Button>
        </Box>
      ) : viewMode === 'timeline' ? (
        /* TIMELINE VIEW */
        <Stack spacing={2.5}>
          {interviews.map((item) => {
            const typeConfig = TYPE_CONFIG[item.interviewType] || TYPE_CONFIG[InterviewType.Other]
            const formatConfig = FORMAT_CONFIG[item.format] || FORMAT_CONFIG[InterviewFormat.VideoCall]
            const outcomeConfig = OUTCOME_CONFIG[item.outcome] || OUTCOME_CONFIG[InterviewOutcome.Pending]
            const isUpcoming = item.status === InterviewStatus.Scheduled && new Date(item.scheduledAt) >= new Date()

            return (
              <Card key={item.id} variant="outlined" sx={{
                borderLeft: isUpcoming ? '5px solid #00E676' : '5px solid #2979FF',
                transition: 'transform 0.1s ease',
                '&:hover': { bgcolor: 'action.hover' }
              }}>
                <CardContent sx={{ pb: 1.5 }}>
                  <Box display="flex" justifyContent="space-between" alignItems="flex-start" flexWrap="wrap" gap={1} mb={1}>
                    <Box display="flex" alignItems="center" gap={1}>
                      <Chip
                        icon={typeConfig.icon}
                        label={typeConfig.label}
                        color={typeConfig.color}
                        size="small"
                        sx={{ fontWeight: 600 }}
                      />
                      <Typography variant="h6" fontWeight={700}>
                        {item.title}
                      </Typography>
                    </Box>
                    <Box display="flex" alignItems="center" gap={1}>
                      <Chip
                        label={outcomeConfig.label}
                        color={outcomeConfig.color}
                        size="small"
                        variant={item.status === InterviewStatus.Completed ? 'filled' : 'outlined'}
                      />
                      {item.rating && (
                        <Rating value={item.rating} readOnly size="small" />
                      )}
                    </Box>
                  </Box>

                  {/* Company & Role */}
                  <Box display="flex" alignItems="center" gap={1} color="text.secondary" mb={1.5}>
                    <BusinessIcon fontSize="small" />
                    <Typography variant="body2" fontWeight={600} color="text.primary">
                      {item.companyName || 'Independent'}
                    </Typography>
                    {item.jobTitle && (
                      <Typography variant="body2" color="text.secondary">
                        • {item.jobTitle}
                      </Typography>
                    )}
                  </Box>

                  {/* Time & Format Info */}
                  <Stack direction="row" spacing={1.5} flexWrap="wrap" gap={1} mb={2}>
                    <Chip
                      icon={<EventIcon />}
                      label={new Date(item.scheduledAt).toLocaleString(undefined, {
                        weekday: 'short', month: 'short', day: 'numeric', hour: '2-digit', minute: '2-digit'
                      })}
                      size="small"
                      variant="outlined"
                    />
                    <Chip
                      icon={formatConfig.icon}
                      label={`${formatConfig.label} (${item.durationMinutes} min)`}
                      size="small"
                      variant="outlined"
                    />
                    {item.meetingLink && (
                      <Button
                        size="small"
                        variant="outlined"
                        color="primary"
                        endIcon={<LaunchIcon fontSize="inherit" />}
                        component="a"
                        href={item.meetingLink}
                        target="_blank"
                        rel="noopener noreferrer"
                        sx={{ py: 0, height: 24, fontSize: '0.75rem' }}
                      >
                        Launch Call
                      </Button>
                    )}
                  </Stack>

                  {/* Interviewers */}
                  {(item.interviewerNames || item.interviewers.length > 0) && (
                    <Box sx={{ mb: 1.5, p: 1, bgcolor: 'background.default', borderRadius: 1 }}>
                      <Typography variant="caption" color="text.secondary" fontWeight={600} display="block">
                        INTERVIEWERS:
                      </Typography>
                      <Stack direction="row" spacing={1} flexWrap="wrap" gap={0.5} mt={0.5}>
                        {item.interviewers.map((ii) => (
                          <Chip
                            key={ii.contactId}
                            avatar={<PersonIcon />}
                            label={ii.contactName}
                            size="small"
                            variant="outlined"
                          />
                        ))}
                        {item.interviewerNames && (
                          <Typography variant="caption" color="text.secondary" alignSelf="center">
                            {item.interviewerNames}
                          </Typography>
                        )}
                      </Stack>
                    </Box>
                  )}

                  {/* Preparation Notes */}
                  {item.preparationNotes && (
                    <Box sx={{ mb: 1.5 }}>
                      <Typography variant="caption" color="text.secondary" fontWeight={600} display="block">
                        PREPARATION NOTES & TALKING POINTS:
                      </Typography>
                      <Typography variant="body2" sx={{ whiteSpace: 'pre-wrap', bgcolor: 'action.hover', p: 1, borderRadius: 1, mt: 0.5 }}>
                        {item.preparationNotes}
                      </Typography>
                    </Box>
                  )}

                  {/* Questions Asked */}
                  {item.questionsAsked && (
                    <Box sx={{ mb: 1.5 }}>
                      <Typography variant="caption" color="text.secondary" fontWeight={600} display="block">
                        QUESTIONS ASKED / CODING PROMPT:
                      </Typography>
                      <Typography variant="body2" sx={{ whiteSpace: 'pre-wrap', bgcolor: 'action.hover', p: 1, borderRadius: 1, mt: 0.5 }}>
                        {item.questionsAsked}
                      </Typography>
                    </Box>
                  )}

                  {/* Feedback Notes */}
                  {item.feedbackNotes && (
                    <Box>
                      <Typography variant="caption" color="text.secondary" fontWeight={600} display="block">
                        DEBRIEF & FEEDBACK:
                      </Typography>
                      <Typography variant="body2" sx={{ fontStyle: 'italic', color: 'text.secondary', mt: 0.5 }}>
                        {item.feedbackNotes}
                      </Typography>
                    </Box>
                  )}
                </CardContent>

                <Divider />
                <Box display="flex" justifyContent="space-between" alignItems="center" px={2} py={1} bgcolor="background.paper">
                  <Box display="flex" alignItems="center" gap={1}>
                    {item.status === InterviewStatus.Scheduled && (
                      <Button
                        size="small"
                        color="success"
                        startIcon={<CheckCircleIcon />}
                        onClick={() => handleOpenQuickComplete(item)}
                      >
                        Record Outcome
                      </Button>
                    )}
                  </Box>
                  <Box>
                    <IconButton size="small" onClick={() => openEdit(item)}>
                      <EditIcon fontSize="small" />
                    </IconButton>
                    <IconButton size="small" color="error" onClick={() => handleDelete(item.id)}>
                      <DeleteIcon fontSize="small" />
                    </IconButton>
                  </Box>
                </Box>
              </Card>
            )
          })}
        </Stack>
      ) : viewMode === 'calendar' ? (
        /* CALENDAR / SCHEDULE CARDS VIEW */
        <Grid container spacing={2}>
          {interviews.map((item) => {
            const typeConfig = TYPE_CONFIG[item.interviewType] || TYPE_CONFIG[InterviewType.Other]
            const formatConfig = FORMAT_CONFIG[item.format] || FORMAT_CONFIG[InterviewFormat.VideoCall]
            const isUpcoming = item.status === InterviewStatus.Scheduled && new Date(item.scheduledAt) >= new Date()

            return (
              <Grid item xs={12} sm={6} md={4} key={item.id}>
                <Card variant="outlined" sx={{
                  height: '100%',
                  display: 'flex',
                  flexDirection: 'column',
                  bgcolor: isUpcoming ? 'background.paper' : 'background.default',
                  borderTop: isUpcoming ? '4px solid #00E676' : '4px solid rgba(255,255,255,0.12)'
                }}>
                  <CardContent sx={{ flexGrow: 1 }}>
                    <Box display="flex" justifyContent="space-between" alignItems="center" mb={1}>
                      <Chip label={typeConfig.label} color={typeConfig.color} size="small" />
                      <Typography variant="caption" fontWeight={700} color={isUpcoming ? 'success.main' : 'text.secondary'}>
                        {isUpcoming ? 'UPCOMING' : item.statusName.toUpperCase()}
                      </Typography>
                    </Box>

                    <Typography variant="h6" fontWeight={700} noWrap>
                      {item.title}
                    </Typography>

                    <Typography variant="body2" color="primary.main" fontWeight={600} mb={1.5}>
                      {item.companyName || 'Independent'} {item.jobTitle ? `• ${item.jobTitle}` : ''}
                    </Typography>

                    <Box sx={{ p: 1.5, bgcolor: 'action.hover', borderRadius: 1.5, mb: 2 }}>
                      <Typography variant="body2" fontWeight={700} display="flex" alignItems="center" gap={0.5}>
                        <EventIcon fontSize="small" color="primary" />
                        {new Date(item.scheduledAt).toLocaleDateString(undefined, {
                          weekday: 'short', month: 'short', day: 'numeric'
                        })}
                      </Typography>
                      <Typography variant="caption" color="text.secondary" display="block" ml={3}>
                        {new Date(item.scheduledAt).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })} ({item.durationMinutes} min)
                      </Typography>
                    </Box>

                    {item.meetingLink && (
                      <Button
                        variant="outlined"
                        size="small"
                        fullWidth
                        startIcon={formatConfig.icon}
                        endIcon={<LaunchIcon fontSize="small" />}
                        component="a"
                        href={item.meetingLink}
                        target="_blank"
                        rel="noopener noreferrer"
                        sx={{ mb: 1.5 }}
                      >
                        Launch Meeting Room
                      </Button>
                    )}

                    {item.preparationNotes && (
                      <Typography variant="caption" color="text.secondary" sx={{
                        display: '-webkit-box',
                        WebkitLineClamp: 2,
                        WebkitBoxOrient: 'vertical',
                        overflow: 'hidden'
                      }}>
                        Prep: {item.preparationNotes}
                      </Typography>
                    )}
                  </CardContent>

                  <Divider />
                  <Box display="flex" justifyContent="space-between" alignItems="center" px={2} py={1} bgcolor="background.paper">
                    {item.status === InterviewStatus.Scheduled ? (
                      <Button size="small" color="success" onClick={() => handleOpenQuickComplete(item)}>
                        Complete
                      </Button>
                    ) : (
                      <Chip label={item.outcomeName} size="small" variant="outlined" />
                    )}
                    <Box>
                      <IconButton size="small" onClick={() => openEdit(item)}>
                        <EditIcon fontSize="small" />
                      </IconButton>
                      <IconButton size="small" color="error" onClick={() => handleDelete(item.id)}>
                        <DeleteIcon fontSize="small" />
                      </IconButton>
                    </Box>
                  </Box>
                </Card>
              </Grid>
            )
          })}
        </Grid>
      ) : (
        /* TABLE VIEW */
        <TableContainer component={Paper} variant="outlined">
          <Table size="small">
            <TableHead>
              <TableRow>
                <TableCell>Round & Company</TableCell>
                <TableCell>Type</TableCell>
                <TableCell>Scheduled At</TableCell>
                <TableCell>Format</TableCell>
                <TableCell>Status</TableCell>
                <TableCell>Outcome</TableCell>
                <TableCell>Rating</TableCell>
                <TableCell align="right">Actions</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {interviews.map((item) => {
                const typeConfig = TYPE_CONFIG[item.interviewType] || TYPE_CONFIG[InterviewType.Other]
                const outcomeConfig = OUTCOME_CONFIG[item.outcome] || OUTCOME_CONFIG[InterviewOutcome.Pending]
                return (
                  <TableRow key={item.id} hover>
                    <TableCell>
                      <Typography variant="body2" fontWeight={700}>{item.title}</Typography>
                      <Typography variant="caption" color="text.secondary">
                        {item.companyName || 'Independent'} {item.jobTitle ? `• ${item.jobTitle}` : ''}
                      </Typography>
                    </TableCell>
                    <TableCell>
                      <Chip label={typeConfig.label} color={typeConfig.color} size="small" />
                    </TableCell>
                    <TableCell>
                      <Typography variant="body2">
                        {new Date(item.scheduledAt).toLocaleString(undefined, { month: 'short', day: 'numeric', hour: '2-digit', minute: '2-digit' })}
                      </Typography>
                    </TableCell>
                    <TableCell>
                      <Typography variant="body2">{item.formatName}</Typography>
                    </TableCell>
                    <TableCell>
                      <Typography variant="body2">{item.statusName}</Typography>
                    </TableCell>
                    <TableCell>
                      <Chip label={outcomeConfig.label} color={outcomeConfig.color} size="small" />
                    </TableCell>
                    <TableCell>
                      {item.rating ? <Rating value={item.rating} readOnly size="small" /> : '—'}
                    </TableCell>
                    <TableCell align="right">
                      <IconButton size="small" onClick={() => openEdit(item)}>
                        <EditIcon fontSize="small" />
                      </IconButton>
                      <IconButton size="small" color="error" onClick={() => handleDelete(item.id)}>
                        <DeleteIcon fontSize="small" />
                      </IconButton>
                    </TableCell>
                  </TableRow>
                )
              })}
            </TableBody>
          </Table>
        </TableContainer>
      )}

      {/* CREATE / EDIT INTERVIEW MODAL */}
      <Dialog open={dialogOpen} onClose={() => setDialogOpen(false)} maxWidth="md" fullWidth>
        <form onSubmit={handleSave}>
          <DialogTitle fontWeight={700}>
            {editInterviewId ? 'Edit Interview Round' : 'Schedule New Interview Round'}
          </DialogTitle>
          <DialogContent dividers>
            <Grid container spacing={2}>
              <Grid item xs={12} sm={8}>
                <TextField
                  label="Interview Round Title"
                  required
                  placeholder="e.g. Distributed Consensus System Design with Principal Engineer"
                  value={form.title}
                  onChange={(e) => setForm({ ...form, title: e.target.value })}
                  size="small"
                  fullWidth
                />
              </Grid>

              <Grid item xs={12} sm={4}>
                <FormControl fullWidth size="small">
                  <InputLabel>Interview Stage Type</InputLabel>
                  <Select
                    value={form.interviewType}
                    label="Interview Stage Type"
                    onChange={(e) => setForm({ ...form, interviewType: Number(e.target.value) as InterviewType })}
                  >
                    {Object.entries(TYPE_CONFIG).map(([val, info]) => (
                      <MenuItem key={val} value={Number(val)}>{info.label}</MenuItem>
                    ))}
                  </Select>
                </FormControl>
              </Grid>

              <Grid item xs={12} sm={6}>
                <FormControl fullWidth size="small">
                  <InputLabel>Linked Application / Position</InputLabel>
                  <Select
                    value={form.jobApplicationId || ''}
                    label="Linked Application / Position"
                    onChange={(e) => {
                      const selectedApp = applications.find((a) => a.id === e.target.value)
                      setForm({
                        ...form,
                        jobApplicationId: e.target.value || undefined,
                        jobId: selectedApp?.jobId,
                        companyId: selectedApp?.companyId,
                      })
                    }}
                  >
                    <MenuItem value="">Independent Round</MenuItem>
                    {applications.map((a) => (
                      <MenuItem key={a.id} value={a.id}>
                        {a.jobTitle} @ {a.companyName}
                      </MenuItem>
                    ))}
                  </Select>
                </FormControl>
              </Grid>

              <Grid item xs={12} sm={6}>
                <FormControl fullWidth size="small">
                  <InputLabel>Employer / Company</InputLabel>
                  <Select
                    value={form.companyId || ''}
                    label="Employer / Company"
                    onChange={(e) => setForm({ ...form, companyId: e.target.value || undefined })}
                  >
                    <MenuItem value="">Independent</MenuItem>
                    {companies.map((c) => (
                      <MenuItem key={c.id} value={c.id}>{c.name}</MenuItem>
                    ))}
                  </Select>
                </FormControl>
              </Grid>

              <Grid item xs={12} sm={4}>
                <TextField
                  label="Date & Time"
                  type="datetime-local"
                  required
                  value={form.scheduledAt}
                  onChange={(e) => setForm({ ...form, scheduledAt: e.target.value })}
                  size="small"
                  fullWidth
                  InputLabelProps={{ shrink: true }}
                />
              </Grid>

              <Grid item xs={12} sm={4}>
                <TextField
                  label="Duration (Minutes)"
                  type="number"
                  value={form.durationMinutes}
                  onChange={(e) => setForm({ ...form, durationMinutes: Number(e.target.value) })}
                  size="small"
                  fullWidth
                />
              </Grid>

              <Grid item xs={12} sm={4}>
                <FormControl fullWidth size="small">
                  <InputLabel>Format</InputLabel>
                  <Select
                    value={form.format}
                    label="Format"
                    onChange={(e) => setForm({ ...form, format: Number(e.target.value) as InterviewFormat })}
                  >
                    {Object.entries(FORMAT_CONFIG).map(([val, info]) => (
                      <MenuItem key={val} value={Number(val)}>{info.label}</MenuItem>
                    ))}
                  </Select>
                </FormControl>
              </Grid>

              <Grid item xs={12} sm={8}>
                <TextField
                  label="Meeting Link (Google Meet / Zoom / Chime)"
                  placeholder="https://meet.google.com/xyz or https://zoom.us/j/123"
                  value={form.meetingLink || ''}
                  onChange={(e) => setForm({ ...form, meetingLink: e.target.value })}
                  size="small"
                  fullWidth
                />
              </Grid>

              <Grid item xs={12} sm={4}>
                <TextField
                  label="Location (Virtual or Office Address)"
                  placeholder="e.g. Remote / Building 4, Floor 3"
                  value={form.location || ''}
                  onChange={(e) => setForm({ ...form, location: e.target.value })}
                  size="small"
                  fullWidth
                />
              </Grid>

              <Grid item xs={12} sm={6}>
                <TextField
                  label="Interviewer Names (Free text)"
                  placeholder="e.g. Sarah Jenkins (Tech Lead), Alex Rivera (VP)"
                  value={form.interviewerNames || ''}
                  onChange={(e) => setForm({ ...form, interviewerNames: e.target.value })}
                  size="small"
                  fullWidth
                />
              </Grid>

              <Grid item xs={12} sm={6}>
                <FormControl fullWidth size="small">
                  <InputLabel>Interviewer Contacts (From CRM)</InputLabel>
                  <Select
                    multiple
                    value={form.interviewerContactIds || []}
                    label="Interviewer Contacts (From CRM)"
                    onChange={(e) => {
                      const val = typeof e.target.value === 'string' ? e.target.value.split(',') : e.target.value
                      setForm({ ...form, interviewerContactIds: val })
                    }}
                    renderValue={(selected) => (
                      <Box display="flex" flexWrap="wrap" gap={0.5}>
                        {selected.map((cid) => {
                          const c = contacts.find((ct) => ct.id === cid)
                          return <Chip key={cid} label={c?.name || cid} size="small" />
                        })}
                      </Box>
                    )}
                  >
                    {contacts.map((c) => (
                      <MenuItem key={c.id} value={c.id}>
                        {c.name} {c.company ? `(${c.company})` : ''}
                      </MenuItem>
                    ))}
                  </Select>
                </FormControl>
              </Grid>

              <Grid item xs={12}>
                <TextField
                  label="Preparation Notes & Talking Points"
                  placeholder="Key accomplishments to highlight, STAR stories prepared, questions to ask the interviewers..."
                  value={form.preparationNotes || ''}
                  onChange={(e) => setForm({ ...form, preparationNotes: e.target.value })}
                  multiline
                  rows={2}
                  fullWidth
                />
              </Grid>

              {/* Status and outcome fields (enabled during edit) */}
              {editInterviewId && (
                <>
                  <Grid item xs={12}><Divider><Chip label="Post-Interview Debrief" size="small" /></Divider></Grid>

                  <Grid item xs={12} sm={4}>
                    <FormControl fullWidth size="small">
                      <InputLabel>Round Status</InputLabel>
                      <Select
                        value={editStatus}
                        label="Round Status"
                        onChange={(e) => setEditStatus(Number(e.target.value) as InterviewStatus)}
                      >
                        <MenuItem value={InterviewStatus.Scheduled}>Scheduled</MenuItem>
                        <MenuItem value={InterviewStatus.Completed}>Completed</MenuItem>
                        <MenuItem value={InterviewStatus.Cancelled}>Cancelled</MenuItem>
                        <MenuItem value={InterviewStatus.Rescheduled}>Rescheduled</MenuItem>
                      </Select>
                    </FormControl>
                  </Grid>

                  <Grid item xs={12} sm={4}>
                    <FormControl fullWidth size="small">
                      <InputLabel>Outcome</InputLabel>
                      <Select
                        value={editOutcome}
                        label="Outcome"
                        onChange={(e) => setEditOutcome(Number(e.target.value) as InterviewOutcome)}
                      >
                        {Object.entries(OUTCOME_CONFIG).map(([val, info]) => (
                          <MenuItem key={val} value={Number(val)}>{info.label}</MenuItem>
                        ))}
                      </Select>
                    </FormControl>
                  </Grid>

                  <Grid item xs={12} sm={4}>
                    <Box display="flex" alignItems="center" gap={1} height="100%">
                      <Typography variant="body2" color="text.secondary">Self-Rating:</Typography>
                      <Rating
                        value={editRating}
                        onChange={(_, v) => setEditRating(v)}
                      />
                    </Box>
                  </Grid>

                  <Grid item xs={12}>
                    <TextField
                      label="Questions Asked / Coding Problems"
                      placeholder="e.g. Design a high-throughput distributed event log with raft consensus, implement LRU cache..."
                      value={editQuestions}
                      onChange={(e) => setEditQuestions(e.target.value)}
                      multiline
                      rows={2}
                      fullWidth
                    />
                  </Grid>

                  <Grid item xs={12}>
                    <TextField
                      label="Feedback & Self-Assessment Debrief"
                      placeholder="What went well, what could be improved, signals from the interviewer..."
                      value={editFeedback}
                      onChange={(e) => setEditFeedback(e.target.value)}
                      multiline
                      rows={2}
                      fullWidth
                    />
                  </Grid>
                </>
              )}
            </Grid>
          </DialogContent>
          <DialogActions sx={{ px: 3, py: 2 }}>
            <Button onClick={() => setDialogOpen(false)}>Cancel</Button>
            <Button type="submit" variant="contained" disabled={saving}>
              {saving ? 'Saving...' : editInterviewId ? 'Update Interview Round' : 'Schedule Round'}
            </Button>
          </DialogActions>
        </form>
      </Dialog>

      {/* QUICK COMPLETE / DEBRIEF MODAL */}
      <Dialog open={quickCompleteOpen} onClose={() => setQuickCompleteOpen(false)} maxWidth="sm" fullWidth>
        <form onSubmit={handleSaveQuickComplete}>
          <DialogTitle fontWeight={700}>
            Record Interview Outcome & Debrief — {quickTarget?.title}
          </DialogTitle>
          <DialogContent dividers>
            <Grid container spacing={2}>
              <Grid item xs={12}>
                <FormControl fullWidth size="small">
                  <InputLabel>Outcome</InputLabel>
                  <Select
                    value={quickOutcome}
                    label="Outcome"
                    onChange={(e) => setQuickOutcome(Number(e.target.value) as InterviewOutcome)}
                  >
                    <MenuItem value={InterviewOutcome.Passed}>Passed</MenuItem>
                    <MenuItem value={InterviewOutcome.StrongHire}>Strong Hire</MenuItem>
                    <MenuItem value={InterviewOutcome.Hire}>Hire</MenuItem>
                    <MenuItem value={InterviewOutcome.Neutral}>Neutral / Mixed</MenuItem>
                    <MenuItem value={InterviewOutcome.Failed}>Did Not Advance</MenuItem>
                  </Select>
                </FormControl>
              </Grid>

              <Grid item xs={12}>
                <Box display="flex" alignItems="center" gap={1}>
                  <Typography variant="body2">Self-Rating Performance:</Typography>
                  <Rating
                    value={quickRating}
                    onChange={(_, v) => setQuickRating(v)}
                  />
                </Box>
              </Grid>

              <Grid item xs={12}>
                <TextField
                  label="Debrief Notes & Immediate Feedback"
                  placeholder="Key impressions, interviewer response, problems solved..."
                  value={quickFeedback}
                  onChange={(e) => setQuickFeedback(e.target.value)}
                  multiline
                  rows={3}
                  fullWidth
                />
              </Grid>
            </Grid>
          </DialogContent>
          <DialogActions sx={{ px: 3, py: 1.5 }}>
            <Button onClick={() => setQuickCompleteOpen(false)}>Cancel</Button>
            <Button type="submit" variant="contained" color="success">
              Save Outcome & Debrief
            </Button>
          </DialogActions>
        </form>
      </Dialog>
    </Box>
  )
}
