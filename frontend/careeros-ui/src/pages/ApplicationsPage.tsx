import React, { useState, useEffect } from 'react'
import {
  Box, Typography, Button, Grid, Card, CardContent, Chip, IconButton,
  Dialog, DialogTitle, DialogContent, DialogActions, TextField, MenuItem,
  Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Paper,
  Alert, CircularProgress, Tooltip, InputAdornment, FormControl, InputLabel,
  Select, ToggleButtonGroup, ToggleButton, Divider, Stack
} from '@mui/material'
import AddIcon from '@mui/icons-material/Add'
import EditIcon from '@mui/icons-material/Edit'
import DeleteIcon from '@mui/icons-material/Delete'
import SearchIcon from '@mui/icons-material/Search'
import TimelineIcon from '@mui/icons-material/Timeline'
import CameraAltIcon from '@mui/icons-material/CameraAlt'
import ViewModuleIcon from '@mui/icons-material/ViewModule'
import ViewListIcon from '@mui/icons-material/ViewList'
import DescriptionIcon from '@mui/icons-material/Description'
import BusinessIcon from '@mui/icons-material/Business'
import ArrowForwardIcon from '@mui/icons-material/ArrowForward'
import FingerprintIcon from '@mui/icons-material/Fingerprint'
import AssignmentIcon from '@mui/icons-material/Assignment'

import { applicationsApi, ApplicationStatus } from '../api/applications'
import type {
  JobApplication, ApplicationStats, CreateApplicationRequest,
  ApplicationTimelineEvent, ApplicationSnapshot
} from '../api/applications'
import { jobsApi } from '../api/jobs'
import type { Job } from '../api/jobs'
import { resumeProfilesApi, coverLettersApi, portfolioVersionsApi } from '../api/resumeCenter'
import type { ResumeProfile, CoverLetter, PortfolioVersion } from '../api/resumeCenter'
import { companiesApi } from '../api/companies'
import type { Company } from '../api/companies'
import { contactsApi } from '../api/contacts'
import type { Contact } from '../api/contacts'

const STATUS_LABELS: Record<ApplicationStatus, { label: string; color: 'default' | 'primary' | 'secondary' | 'error' | 'info' | 'success' | 'warning' }> = {
  [ApplicationStatus.Draft]: { label: 'Draft', color: 'default' },
  [ApplicationStatus.Applied]: { label: 'Applied', color: 'info' },
  [ApplicationStatus.UnderReview]: { label: 'Under Review', color: 'info' },
  [ApplicationStatus.PhoneScreen]: { label: 'Phone Screen', color: 'warning' },
  [ApplicationStatus.TechnicalInterview]: { label: 'Technical Interview', color: 'warning' },
  [ApplicationStatus.BehavioralInterview]: { label: 'Behavioral Interview', color: 'warning' },
  [ApplicationStatus.FinalRound]: { label: 'Final Round', color: 'secondary' },
  [ApplicationStatus.OfferReceived]: { label: 'Offer Received', color: 'success' },
  [ApplicationStatus.Accepted]: { label: 'Accepted', color: 'success' },
  [ApplicationStatus.Rejected]: { label: 'Rejected', color: 'error' },
  [ApplicationStatus.Withdrawn]: { label: 'Withdrawn', color: 'default' },
}

const emptyForm: CreateApplicationRequest = {
  jobId: '',
  companyId: undefined,
  resumeVersionId: undefined,
  coverLetterId: undefined,
  portfolioVersionId: undefined,
  status: ApplicationStatus.Applied,
  applicationDate: new Date().toISOString().substring(0, 10),
  channel: 'Company Website',
  referralContactId: undefined,
  notes: '',
}

export default function ApplicationsPage() {
  const [applications, setApplications] = useState<JobApplication[]>([])
  const [stats, setStats] = useState<ApplicationStats | null>(null)
  const [jobs, setJobs] = useState<Job[]>([])
  const [companies, setCompanies] = useState<Company[]>([])
  const [profiles, setProfiles] = useState<ResumeProfile[]>([])
  const [coverLetters, setCoverLetters] = useState<CoverLetter[]>([])
  const [portfolios, setPortfolios] = useState<PortfolioVersion[]>([])
  const [contacts, setContacts] = useState<Contact[]>([])

  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [success, setSuccess] = useState<string | null>(null)

  // Filters
  const [searchTerm, setSearchTerm] = useState('')
  const [selectedStatus, setSelectedStatus] = useState<string>('all')
  const [selectedCompanyId, setSelectedCompanyId] = useState<string>('all')
  const [viewMode, setViewMode] = useState<'grid' | 'table'>('grid')

  // Create / Edit modal
  const [dialogOpen, setDialogOpen] = useState(false)
  const [editAppId, setEditAppId] = useState<string | null>(null)
  const [form, setForm] = useState<CreateApplicationRequest>(emptyForm)
  const [saving, setSaving] = useState(false)

  // Timeline modal
  const [timelineApp, setTimelineApp] = useState<JobApplication | null>(null)
  const [timelineEvents, setTimelineEvents] = useState<ApplicationTimelineEvent[]>([])
  const [timelineOpen, setTimelineOpen] = useState(false)
  const [newMilestoneTitle, setNewMilestoneTitle] = useState('')
  const [newMilestoneStatus, setNewMilestoneStatus] = useState<ApplicationStatus>(ApplicationStatus.PhoneScreen)
  const [newMilestoneNotes, setNewMilestoneNotes] = useState('')
  const [addingMilestone, setAddingMilestone] = useState(false)

  // Snapshot modal
  const [snapshotOpen, setSnapshotOpen] = useState(false)
  const [currentSnapshot, setCurrentSnapshot] = useState<ApplicationSnapshot | null>(null)
  const [loadingSnapshot, setLoadingSnapshot] = useState(false)

  const loadData = async () => {
    setLoading(true)
    try {
      const statusVal = selectedStatus !== 'all' ? (Number(selectedStatus) as ApplicationStatus) : undefined
      const companyVal = selectedCompanyId !== 'all' ? selectedCompanyId : undefined

      const [appsData, statsData, jobsData, companiesData, profilesData, clData, pfData, contactsData] =
        await Promise.all([
          applicationsApi.getAll({
            status: statusVal,
            companyId: companyVal,
            searchTerm: searchTerm || undefined,
          }),
          applicationsApi.getStats().catch(() => null),
          jobsApi.getAll().catch(() => []),
          companiesApi.getAll().catch(() => []),
          resumeProfilesApi.getAll().catch(() => []),
          coverLettersApi.getAll().catch(() => []),
          portfolioVersionsApi.getAll().catch(() => []),
          contactsApi.getAll().catch(() => []),
        ])

      setApplications(appsData)
      setStats(statsData)
      setJobs(jobsData)
      setCompanies(companiesData)
      setProfiles(profilesData)
      setCoverLetters(clData)
      setPortfolios(pfData)
      setContacts(contactsData)
    } catch {
      setError('Failed to load application data.')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    loadData()
  }, [searchTerm, selectedStatus, selectedCompanyId])

  const handleOpenCreate = () => {
    setEditAppId(null)
    setForm({
      ...emptyForm,
      jobId: jobs.length > 0 ? jobs[0].id : '',
      applicationDate: new Date().toISOString().substring(0, 10),
    })
    setDialogOpen(true)
  }

  const handleOpenEdit = (app: JobApplication) => {
    setEditAppId(app.id)
    setForm({
      jobId: app.jobId,
      companyId: app.companyId,
      resumeVersionId: app.resumeVersionId,
      coverLetterId: app.coverLetterId,
      portfolioVersionId: app.portfolioVersionId,
      status: app.status,
      applicationDate: app.applicationDate ? app.applicationDate.substring(0, 10) : new Date().toISOString().substring(0, 10),
      channel: app.channel || 'Company Website',
      referralContactId: app.referralContactId,
      notes: app.notes || '',
    })
    setDialogOpen(true)
  }

  const handleSave = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!form.jobId) {
      setError('Please select a target position.')
      return
    }

    setSaving(true)
    setError(null)
    try {
      if (editAppId) {
        await applicationsApi.update(editAppId, {
          id: editAppId,
          resumeVersionId: form.resumeVersionId,
          coverLetterId: form.coverLetterId,
          portfolioVersionId: form.portfolioVersionId,
          status: form.status,
          applicationDate: new Date(form.applicationDate).toISOString(),
          channel: form.channel,
          referralContactId: form.referralContactId,
          notes: form.notes,
        })
        setSuccess('Application updated successfully.')
      } else {
        await applicationsApi.create({
          ...form,
          applicationDate: new Date(form.applicationDate).toISOString(),
        })
        setSuccess('Application recorded with immutable snapshot & initial timeline milestone.')
      }
      setDialogOpen(false)
      loadData()
    } catch {
      setError('Failed to save application.')
    } finally {
      setSaving(false)
    }
  }

  const handleDelete = async (id: string) => {
    if (!window.confirm('Delete this application record and its timeline history?')) return
    try {
      await applicationsApi.delete(id)
      setSuccess('Application deleted.')
      loadData()
    } catch {
      setError('Failed to delete application.')
    }
  }

  const handleQuickAdvance = async (app: JobApplication, nextStatus: ApplicationStatus) => {
    try {
      await applicationsApi.updateStatus(app.id, {
        newStatus: nextStatus,
        eventTitle: `Advanced to ${STATUS_LABELS[nextStatus]?.label || nextStatus}`,
      })
      setSuccess(`Advanced status to ${STATUS_LABELS[nextStatus]?.label || nextStatus}`)
      loadData()
    } catch {
      setError('Failed to advance application status.')
    }
  }

  const handleOpenTimeline = async (app: JobApplication) => {
    setTimelineApp(app)
    setTimelineOpen(true)
    setNewMilestoneTitle('')
    setNewMilestoneNotes('')
    setNewMilestoneStatus(app.status)
    try {
      const events = await applicationsApi.getTimeline(app.id)
      setTimelineEvents(events)
    } catch {
      setError('Failed to load timeline events.')
    }
  }

  const handleAddMilestone = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!timelineApp || !newMilestoneTitle.trim()) return

    setAddingMilestone(true)
    try {
      await applicationsApi.addTimelineEvent(timelineApp.id, {
        status: newMilestoneStatus,
        title: newMilestoneTitle.trim(),
        notes: newMilestoneNotes.trim() || undefined,
      })
      const events = await applicationsApi.getTimeline(timelineApp.id)
      setTimelineEvents(events)
      setNewMilestoneTitle('')
      setNewMilestoneNotes('')
      setSuccess('Timeline milestone recorded.')
      loadData()
    } catch {
      setError('Failed to add milestone.')
    } finally {
      setAddingMilestone(false)
    }
  }

  const handleOpenSnapshot = async (app: JobApplication) => {
    setSnapshotOpen(true)
    setLoadingSnapshot(true)
    setCurrentSnapshot(null)
    try {
      const snap = await applicationsApi.getSnapshot(app.id)
      setCurrentSnapshot(snap)
    } catch {
      setError('Could not retrieve submission snapshot.')
    } finally {
      setLoadingSnapshot(false)
    }
  }

  // Helper to extract all resume versions across profiles
  const allResumeVersions = profiles.flatMap((p) =>
    (p.versions || []).map((v) => ({
      ...v,
      profileName: p.name,
    }))
  )

  return (
    <Box sx={{ p: 3 }}>
      {/* Page Header */}
      <Box display="flex" justifyContent="space-between" alignItems="flex-start" mb={3}>
        <Box>
          <Box display="flex" alignItems="center" gap={1.5}>
            <AssignmentIcon color="primary" sx={{ fontSize: 32 }} />
            <Typography variant="h4" fontWeight={700}>Applications</Typography>
          </Box>
          <Typography variant="body2" color="text.secondary" mt={0.5}>
            Track active submissions, stage milestones, resume versions used, and immutable snapshot records
          </Typography>
        </Box>
        <Button
          variant="contained"
          startIcon={<AddIcon />}
          onClick={handleOpenCreate}
          sx={{ borderRadius: 2 }}
        >
          New Application
        </Button>
      </Box>

      {/* Notifications */}
      {error && <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError(null)}>{error}</Alert>}
      {success && <Alert severity="success" sx={{ mb: 2 }} onClose={() => setSuccess(null)}>{success}</Alert>}

      {/* KPI Stats Cards */}
      {stats && (
        <Grid container spacing={2} mb={3}>
          <Grid item xs={6} sm={2.4}>
            <Card variant="outlined">
              <CardContent sx={{ py: 1.5, '&:last-child': { pb: 1.5 } }}>
                <Typography variant="caption" color="text.secondary">Total Submissions</Typography>
                <Typography variant="h5" fontWeight={700} color="primary.main">{stats.totalApplications}</Typography>
              </CardContent>
            </Card>
          </Grid>
          <Grid item xs={6} sm={2.4}>
            <Card variant="outlined">
              <CardContent sx={{ py: 1.5, '&:last-child': { pb: 1.5 } }}>
                <Typography variant="caption" color="text.secondary">Active Pipeline</Typography>
                <Typography variant="h5" fontWeight={700} color="info.main">{stats.activePipeline}</Typography>
              </CardContent>
            </Card>
          </Grid>
          <Grid item xs={6} sm={2.4}>
            <Card variant="outlined">
              <CardContent sx={{ py: 1.5, '&:last-child': { pb: 1.5 } }}>
                <Typography variant="caption" color="text.secondary">In Interviews</Typography>
                <Typography variant="h5" fontWeight={700} color="warning.main">{stats.interviewingCount}</Typography>
              </CardContent>
            </Card>
          </Grid>
          <Grid item xs={6} sm={2.4}>
            <Card variant="outlined">
              <CardContent sx={{ py: 1.5, '&:last-child': { pb: 1.5 } }}>
                <Typography variant="caption" color="text.secondary">Offers Received</Typography>
                <Typography variant="h5" fontWeight={700} color="success.main">{stats.offersCount}</Typography>
              </CardContent>
            </Card>
          </Grid>
          <Grid item xs={6} sm={2.4}>
            <Card variant="outlined">
              <CardContent sx={{ py: 1.5, '&:last-child': { pb: 1.5 } }}>
                <Typography variant="caption" color="text.secondary">Offer Conversion</Typography>
                <Typography variant="h5" fontWeight={700} color="secondary.main">{stats.offerRate}%</Typography>
              </CardContent>
            </Card>
          </Grid>
        </Grid>
      )}

      {/* Filter Toolbar */}
      <Paper variant="outlined" sx={{ p: 2, mb: 3 }}>
        <Grid container spacing={2} alignItems="center">
          <Grid item xs={12} sm={4}>
            <TextField
              placeholder="Search by role, company, channel, notes..."
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
          <Grid item xs={6} sm={3}>
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
          <Grid item xs={6} sm={3}>
            <FormControl size="small" fullWidth>
              <InputLabel>Status Stage</InputLabel>
              <Select
                value={selectedStatus}
                label="Status Stage"
                onChange={(e) => setSelectedStatus(e.target.value)}
              >
                <MenuItem value="all">All Stages</MenuItem>
                {Object.entries(STATUS_LABELS).map(([val, info]) => (
                  <MenuItem key={val} value={val}>{info.label}</MenuItem>
                ))}
              </Select>
            </FormControl>
          </Grid>
          <Grid item xs={12} sm={2} display="flex" justifyContent="flex-end">
            <ToggleButtonGroup
              size="small"
              value={viewMode}
              exclusive
              onChange={(_, v) => { if (v) setViewMode(v) }}
            >
              <ToggleButton value="grid"><ViewModuleIcon fontSize="small" /></ToggleButton>
              <ToggleButton value="table"><ViewListIcon fontSize="small" /></ToggleButton>
            </ToggleButtonGroup>
          </Grid>
        </Grid>
      </Paper>

      {/* Applications Content */}
      {loading ? (
        <Box display="flex" justifyContent="center" mt={8}><CircularProgress /></Box>
      ) : applications.length === 0 ? (
        <Box textAlign="center" py={8} color="text.secondary">
          <AssignmentIcon sx={{ fontSize: 64, opacity: 0.3 }} />
          <Typography mt={2}>No applications recorded yet.</Typography>
          <Button variant="outlined" startIcon={<AddIcon />} onClick={handleOpenCreate} sx={{ mt: 2 }}>
            Record Your First Application
          </Button>
        </Box>
      ) : viewMode === 'grid' ? (
        <Grid container spacing={2}>
          {applications.map((app) => {
            const statusConfig = STATUS_LABELS[app.status] || { label: app.statusName, color: 'default' }
            return (
              <Grid item xs={12} md={6} lg={4} key={app.id}>
                <Card variant="outlined" sx={{ height: '100%', display: 'flex', flexDirection: 'column' }}>
                  <CardContent sx={{ flexGrow: 1 }}>
                    <Box display="flex" justifyContent="space-between" alignItems="flex-start" mb={1}>
                      <Typography variant="h6" fontWeight={700} noWrap sx={{ maxWidth: '70%' }}>
                        {app.jobTitle}
                      </Typography>
                      <Chip
                        label={statusConfig.label}
                        color={statusConfig.color}
                        size="small"
                        sx={{ fontWeight: 600 }}
                      />
                    </Box>

                    <Box display="flex" alignItems="center" gap={1} color="text.secondary" mb={1.5}>
                      <BusinessIcon fontSize="small" />
                      <Typography variant="body2" fontWeight={600} color="text.primary">
                        {app.companyName}
                      </Typography>
                      {app.jobLocation && (
                        <Typography variant="caption" color="text.secondary">
                          • {app.jobLocation}
                        </Typography>
                      )}
                    </Box>

                    {/* Metadata tags */}
                    <Stack direction="row" spacing={1} flexWrap="wrap" gap={0.8} mb={2}>
                      <Chip label={`Applied: ${new Date(app.applicationDate).toLocaleDateString()}`} size="small" variant="outlined" />
                      <Chip label={`Via: ${app.channel}`} size="small" variant="outlined" />
                      {app.jobSalary && (
                        <Chip label={app.jobSalary} size="small" color="success" variant="outlined" />
                      )}
                    </Stack>

                    {/* Attachments Used */}
                    <Box sx={{ bgcolor: 'action.hover', p: 1.5, borderRadius: 1.5, mb: 2 }}>
                      <Typography variant="caption" color="text.secondary" fontWeight={600} display="block" mb={0.5}>
                        DOCUMENTS SUBMITTED
                      </Typography>
                      <Typography variant="body2" noWrap display="flex" alignItems="center" gap={0.5}>
                        <DescriptionIcon fontSize="inherit" color="primary" />
                        <strong>Resume:</strong> {app.resumeVersionName || 'Default Resume'}
                      </Typography>
                      {app.coverLetterTitle && (
                        <Typography variant="body2" noWrap display="flex" alignItems="center" gap={0.5} mt={0.3}>
                          <DescriptionIcon fontSize="inherit" color="secondary" />
                          <strong>Cover Letter:</strong> {app.coverLetterTitle}
                        </Typography>
                      )}
                      {app.referralContactName && (
                        <Typography variant="body2" color="info.main" mt={0.3}>
                          <strong>Referred by:</strong> {app.referralContactName}
                        </Typography>
                      )}
                    </Box>

                    {/* Latest Milestone Indicator */}
                    {app.latestEventTitle && (
                      <Box display="flex" alignItems="center" gap={1} mb={1}>
                        <TimelineIcon fontSize="small" color="primary" />
                        <Typography variant="caption" color="text.secondary" noWrap>
                          Latest: {app.latestEventTitle} ({app.timelineEventsCount} events)
                        </Typography>
                      </Box>
                    )}

                    {app.notes && (
                      <Typography variant="caption" color="text.secondary" sx={{
                        display: '-webkit-box',
                        WebkitLineClamp: 2,
                        WebkitBoxOrient: 'vertical',
                        overflow: 'hidden'
                      }}>
                        {app.notes}
                      </Typography>
                    )}
                  </CardContent>

                  <Divider />
                  <Box display="flex" justifyContent="space-between" alignItems="center" px={2} py={1} bgcolor="background.paper">
                    <Stack direction="row" spacing={1}>
                      <Tooltip title="View Interactive Timeline">
                        <IconButton size="small" color="primary" onClick={() => handleOpenTimeline(app)}>
                          <TimelineIcon fontSize="small" />
                        </IconButton>
                      </Tooltip>
                      <Tooltip title="Inspect Frozen Submission Snapshot">
                        <IconButton size="small" color="secondary" onClick={() => handleOpenSnapshot(app)}>
                          <CameraAltIcon fontSize="small" />
                        </IconButton>
                      </Tooltip>
                      <Tooltip title="Edit Application">
                        <IconButton size="small" onClick={() => handleOpenEdit(app)}>
                          <EditIcon fontSize="small" />
                        </IconButton>
                      </Tooltip>
                      <Tooltip title="Delete">
                        <IconButton size="small" color="error" onClick={() => handleDelete(app.id)}>
                          <DeleteIcon fontSize="small" />
                        </IconButton>
                      </Tooltip>
                    </Stack>

                    {/* Quick Stage Progression */}
                    {app.status === ApplicationStatus.Applied && (
                      <Button
                        size="small"
                        endIcon={<ArrowForwardIcon fontSize="small" />}
                        onClick={() => handleQuickAdvance(app, ApplicationStatus.PhoneScreen)}
                      >
                        To Screen
                      </Button>
                    )}
                    {app.status === ApplicationStatus.PhoneScreen && (
                      <Button
                        size="small"
                        endIcon={<ArrowForwardIcon fontSize="small" />}
                        onClick={() => handleQuickAdvance(app, ApplicationStatus.TechnicalInterview)}
                      >
                        To Tech
                      </Button>
                    )}
                    {app.status === ApplicationStatus.TechnicalInterview && (
                      <Button
                        size="small"
                        endIcon={<ArrowForwardIcon fontSize="small" />}
                        onClick={() => handleQuickAdvance(app, ApplicationStatus.OfferReceived)}
                      >
                        To Offer
                      </Button>
                    )}
                  </Box>
                </Card>
              </Grid>
            )
          })}
        </Grid>
      ) : (
        <TableContainer component={Paper} variant="outlined">
          <Table size="small">
            <TableHead>
              <TableRow>
                <TableCell>Role & Company</TableCell>
                <TableCell>Status</TableCell>
                <TableCell>Date & Channel</TableCell>
                <TableCell>Resume Used</TableCell>
                <TableCell>Cover Letter</TableCell>
                <TableCell>Timeline</TableCell>
                <TableCell align="right">Actions</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {applications.map((app) => {
                const statusConfig = STATUS_LABELS[app.status] || { label: app.statusName, color: 'default' }
                return (
                  <TableRow key={app.id} hover>
                    <TableCell>
                      <Typography variant="body2" fontWeight={700}>{app.jobTitle}</Typography>
                      <Typography variant="caption" color="text.secondary">{app.companyName}</Typography>
                    </TableCell>
                    <TableCell>
                      <Chip label={statusConfig.label} color={statusConfig.color} size="small" />
                    </TableCell>
                    <TableCell>
                      <Typography variant="body2">{new Date(app.applicationDate).toLocaleDateString()}</Typography>
                      <Typography variant="caption" color="text.secondary">{app.channel}</Typography>
                    </TableCell>
                    <TableCell>
                      <Typography variant="body2">{app.resumeVersionName || '—'}</Typography>
                    </TableCell>
                    <TableCell>
                      <Typography variant="body2">{app.coverLetterTitle || '—'}</Typography>
                    </TableCell>
                    <TableCell>
                      <Button
                        size="small"
                        startIcon={<TimelineIcon fontSize="small" />}
                        onClick={() => handleOpenTimeline(app)}
                      >
                        {app.timelineEventsCount} events
                      </Button>
                    </TableCell>
                    <TableCell align="right">
                      <Tooltip title="Inspect Snapshot">
                        <IconButton size="small" color="secondary" onClick={() => handleOpenSnapshot(app)}>
                          <CameraAltIcon fontSize="small" />
                        </IconButton>
                      </Tooltip>
                      <Tooltip title="Edit">
                        <IconButton size="small" onClick={() => handleOpenEdit(app)}>
                          <EditIcon fontSize="small" />
                        </IconButton>
                      </Tooltip>
                      <Tooltip title="Delete">
                        <IconButton size="small" color="error" onClick={() => handleDelete(app.id)}>
                          <DeleteIcon fontSize="small" />
                        </IconButton>
                      </Tooltip>
                    </TableCell>
                  </TableRow>
                )
              })}
            </TableBody>
          </Table>
        </TableContainer>
      )}

      {/* CREATE / EDIT APPLICATION MODAL */}
      <Dialog open={dialogOpen} onClose={() => setDialogOpen(false)} maxWidth="md" fullWidth>
        <form onSubmit={handleSave}>
          <DialogTitle fontWeight={700}>
            {editAppId ? 'Edit Application Record' : 'Record New Job Application'}
          </DialogTitle>
          <DialogContent dividers>
            <Grid container spacing={2}>
              <Grid item xs={12} sm={6}>
                <FormControl fullWidth required size="small">
                  <InputLabel>Target Position (Job Requisition)</InputLabel>
                  <Select
                    value={form.jobId}
                    label="Target Position (Job Requisition)"
                    onChange={(e) => {
                      const selectedJob = jobs.find((j) => j.id === e.target.value)
                      setForm({
                        ...form,
                        jobId: e.target.value,
                        companyId: selectedJob?.companyId,
                      })
                    }}
                  >
                    {jobs.map((j) => (
                      <MenuItem key={j.id} value={j.id}>
                        {j.title} — {j.company} {j.location ? `(${j.location})` : ''}
                      </MenuItem>
                    ))}
                  </Select>
                </FormControl>
              </Grid>

              <Grid item xs={12} sm={6}>
                <FormControl fullWidth size="small">
                  <InputLabel>Company (Auto-linked)</InputLabel>
                  <Select
                    value={form.companyId || ''}
                    label="Company (Auto-linked)"
                    onChange={(e) => setForm({ ...form, companyId: e.target.value || undefined })}
                  >
                    <MenuItem value="">None / Independent</MenuItem>
                    {companies.map((c) => (
                      <MenuItem key={c.id} value={c.id}>{c.name}</MenuItem>
                    ))}
                  </Select>
                </FormControl>
              </Grid>

              <Grid item xs={12} sm={6}>
                <FormControl fullWidth size="small">
                  <InputLabel>Resume Version Used</InputLabel>
                  <Select
                    value={form.resumeVersionId || ''}
                    label="Resume Version Used"
                    onChange={(e) => setForm({ ...form, resumeVersionId: e.target.value || undefined })}
                  >
                    <MenuItem value="">None (Generic / Profile)</MenuItem>
                    {allResumeVersions.map((v) => (
                      <MenuItem key={v.id} value={v.id}>
                        {v.profileName}: {v.versionName} (v{v.versionNumber})
                      </MenuItem>
                    ))}
                  </Select>
                </FormControl>
              </Grid>

              <Grid item xs={12} sm={6}>
                <FormControl fullWidth size="small">
                  <InputLabel>Cover Letter Used</InputLabel>
                  <Select
                    value={form.coverLetterId || ''}
                    label="Cover Letter Used"
                    onChange={(e) => setForm({ ...form, coverLetterId: e.target.value || undefined })}
                  >
                    <MenuItem value="">None</MenuItem>
                    {coverLetters.map((cl) => (
                      <MenuItem key={cl.id} value={cl.id}>{cl.title}</MenuItem>
                    ))}
                  </Select>
                </FormControl>
              </Grid>

              <Grid item xs={12} sm={6}>
                <FormControl fullWidth size="small">
                  <InputLabel>Portfolio Version Used</InputLabel>
                  <Select
                    value={form.portfolioVersionId || ''}
                    label="Portfolio Version Used"
                    onChange={(e) => setForm({ ...form, portfolioVersionId: e.target.value || undefined })}
                  >
                    <MenuItem value="">None</MenuItem>
                    {portfolios.map((pf) => (
                      <MenuItem key={pf.id} value={pf.id}>{pf.name}</MenuItem>
                    ))}
                  </Select>
                </FormControl>
              </Grid>

              <Grid item xs={12} sm={6}>
                <FormControl fullWidth size="small">
                  <InputLabel>Referred By (CRM Contact)</InputLabel>
                  <Select
                    value={form.referralContactId || ''}
                    label="Referred By (CRM Contact)"
                    onChange={(e) => setForm({ ...form, referralContactId: e.target.value || undefined })}
                  >
                    <MenuItem value="">None / Direct</MenuItem>
                    {contacts.map((c) => (
                      <MenuItem key={c.id} value={c.id}>{c.name} {c.company ? `(${c.company})` : ''}</MenuItem>
                    ))}
                  </Select>
                </FormControl>
              </Grid>

              <Grid item xs={12} sm={4}>
                <FormControl fullWidth size="small">
                  <InputLabel>Initial Status</InputLabel>
                  <Select
                    value={form.status}
                    label="Initial Status"
                    onChange={(e) => setForm({ ...form, status: Number(e.target.value) as ApplicationStatus })}
                  >
                    {Object.entries(STATUS_LABELS).map(([val, info]) => (
                      <MenuItem key={val} value={Number(val)}>{info.label}</MenuItem>
                    ))}
                  </Select>
                </FormControl>
              </Grid>

              <Grid item xs={12} sm={4}>
                <TextField
                  label="Application Date"
                  type="date"
                  value={form.applicationDate}
                  onChange={(e) => setForm({ ...form, applicationDate: e.target.value })}
                  size="small"
                  fullWidth
                  InputLabelProps={{ shrink: true }}
                />
              </Grid>

              <Grid item xs={12} sm={4}>
                <TextField
                  label="Submission Channel"
                  placeholder="e.g. LinkedIn, Referral, Careers Page"
                  value={form.channel}
                  onChange={(e) => setForm({ ...form, channel: e.target.value })}
                  size="small"
                  fullWidth
                />
              </Grid>

              <Grid item xs={12}>
                <TextField
                  label="Strategy & Submission Notes"
                  placeholder="Application nuances, referral intro message, custom questionnaire responses..."
                  value={form.notes || ''}
                  onChange={(e) => setForm({ ...form, notes: e.target.value })}
                  multiline
                  rows={3}
                  fullWidth
                />
              </Grid>
            </Grid>
          </DialogContent>
          <DialogActions sx={{ px: 3, py: 2 }}>
            <Button onClick={() => setDialogOpen(false)}>Cancel</Button>
            <Button type="submit" variant="contained" disabled={saving}>
              {saving ? 'Recording...' : editAppId ? 'Update Record' : 'Record Application & Freeze Snapshot'}
            </Button>
          </DialogActions>
        </form>
      </Dialog>

      {/* TIMELINE MODAL */}
      <Dialog open={timelineOpen} onClose={() => setTimelineOpen(false)} maxWidth="md" fullWidth>
        <DialogTitle fontWeight={700} display="flex" alignItems="center" gap={1}>
          <TimelineIcon color="primary" />
          Application Timeline & Milestones — {timelineApp?.jobTitle} @ {timelineApp?.companyName}
        </DialogTitle>
        <DialogContent dividers>
          {/* Milestone List */}
          <Box mb={3}>
            {timelineEvents.length === 0 ? (
              <Typography color="text.secondary">No events recorded in timeline.</Typography>
            ) : (
              <Stack spacing={2}>
                {timelineEvents.map((evt, idx) => {
                  const statusInfo = STATUS_LABELS[evt.status] || { label: evt.statusName, color: 'default' }
                  return (
                    <Paper key={evt.id || idx} variant="outlined" sx={{ p: 2, bgcolor: 'background.default' }}>
                      <Box display="flex" justifyContent="space-between" alignItems="center" mb={0.5}>
                        <Box display="flex" alignItems="center" gap={1}>
                          <Chip label={statusInfo.label} color={statusInfo.color} size="small" />
                          <Typography variant="body1" fontWeight={700}>{evt.title}</Typography>
                        </Box>
                        <Typography variant="caption" color="text.secondary">
                          {new Date(evt.eventDate).toLocaleString()}
                        </Typography>
                      </Box>
                      {evt.notes && (
                        <Typography variant="body2" color="text.secondary" sx={{ mt: 1, whiteSpace: 'pre-wrap' }}>
                          {evt.notes}
                        </Typography>
                      )}
                    </Paper>
                  )
                })}
              </Stack>
            )}
          </Box>

          <Divider sx={{ my: 2 }} />

          {/* Add Milestone Form */}
          <Typography variant="subtitle2" fontWeight={700} mb={1.5}>
            Add Timeline Milestone / Stage Update
          </Typography>
          <form onSubmit={handleAddMilestone}>
            <Grid container spacing={2}>
              <Grid item xs={12} sm={4}>
                <FormControl fullWidth size="small">
                  <InputLabel>Stage</InputLabel>
                  <Select
                    value={newMilestoneStatus}
                    label="Stage"
                    onChange={(e) => setNewMilestoneStatus(Number(e.target.value) as ApplicationStatus)}
                  >
                    {Object.entries(STATUS_LABELS).map(([val, info]) => (
                      <MenuItem key={val} value={Number(val)}>{info.label}</MenuItem>
                    ))}
                  </Select>
                </FormControl>
              </Grid>
              <Grid item xs={12} sm={8}>
                <TextField
                  label="Milestone Title"
                  placeholder="e.g. Recruiter Call with Jane, Take-home Assessment Sent"
                  value={newMilestoneTitle}
                  onChange={(e) => setNewMilestoneTitle(e.target.value)}
                  size="small"
                  fullWidth
                  required
                />
              </Grid>
              <Grid item xs={12}>
                <TextField
                  label="Milestone Notes & Feedback"
                  placeholder="Discussion details, next steps, feedback received..."
                  value={newMilestoneNotes}
                  onChange={(e) => setNewMilestoneNotes(e.target.value)}
                  multiline
                  rows={2}
                  size="small"
                  fullWidth
                />
              </Grid>
              <Grid item xs={12} display="flex" justifyContent="flex-end">
                <Button type="submit" variant="contained" disabled={addingMilestone || !newMilestoneTitle.trim()}>
                  {addingMilestone ? 'Adding...' : 'Add Milestone to Timeline'}
                </Button>
              </Grid>
            </Grid>
          </form>
        </DialogContent>
        <DialogActions sx={{ px: 3, py: 1.5 }}>
          <Button onClick={() => setTimelineOpen(false)}>Close</Button>
        </DialogActions>
      </Dialog>

      {/* IMMUTABLE SNAPSHOT INSPECTOR MODAL */}
      <Dialog open={snapshotOpen} onClose={() => setSnapshotOpen(false)} maxWidth="md" fullWidth>
        <DialogTitle fontWeight={700} display="flex" alignItems="center" gap={1}>
          <CameraAltIcon color="secondary" />
          Frozen Submission Snapshot — Cryptographic Audit Trail
        </DialogTitle>
        <DialogContent dividers>
          {loadingSnapshot ? (
            <Box display="flex" justifyContent="center" py={4}><CircularProgress /></Box>
          ) : !currentSnapshot ? (
            <Typography color="text.secondary">No snapshot found for this application.</Typography>
          ) : (
            <Stack spacing={2.5}>
              <Alert severity="info" icon={<FingerprintIcon />}>
                This snapshot was permanently frozen on{' '}
                <strong>{new Date(currentSnapshot.snapshotDate).toLocaleString()}</strong>.
                It guarantees an immutable record of what was submitted, regardless of future edits to job descriptions or resume drafts.
              </Alert>

              {/* Frozen Position Specs */}
              <Box>
                <Typography variant="subtitle2" fontWeight={700} color="text.secondary" gutterBottom>
                  FROZEN POSITION SPECIFICATIONS
                </Typography>
                <Paper variant="outlined" sx={{ p: 2, bgcolor: 'background.default' }}>
                  <Typography variant="h6" fontWeight={700}>{currentSnapshot.jobTitle}</Typography>
                  <Typography variant="body2" color="primary.main" fontWeight={600} mb={1}>
                    {currentSnapshot.companyName} {currentSnapshot.location ? `• ${currentSnapshot.location}` : ''}
                  </Typography>
                  <Stack direction="row" spacing={1} flexWrap="wrap" gap={0.8}>
                    {currentSnapshot.department && <Chip label={`Dept: ${currentSnapshot.department}`} size="small" />}
                    {currentSnapshot.seniority && <Chip label={`Level: ${currentSnapshot.seniority}`} size="small" />}
                    {currentSnapshot.workplaceType && <Chip label={`Mode: ${currentSnapshot.workplaceType}`} size="small" />}
                    {currentSnapshot.salaryRange && <Chip label={`Comp: ${currentSnapshot.salaryRange}`} size="small" color="success" />}
                  </Stack>
                  {currentSnapshot.techStack && (
                    <Typography variant="body2" color="text.secondary" mt={1.5}>
                      <strong>Tech Stack:</strong> {currentSnapshot.techStack}
                    </Typography>
                  )}
                </Paper>
              </Box>

              {/* Frozen Resume Record */}
              <Box>
                <Typography variant="subtitle2" fontWeight={700} color="text.secondary" gutterBottom>
                  FROZEN RESUME EVIDENCE
                </Typography>
                <Paper variant="outlined" sx={{ p: 2, bgcolor: 'background.default' }}>
                  <Typography variant="body1" fontWeight={700}>
                    {currentSnapshot.resumeProfileName || 'Default Profile'}: {currentSnapshot.resumeVersionName || 'Base'}
                    {currentSnapshot.resumeVersionNumber ? ` (v${currentSnapshot.resumeVersionNumber})` : ''}
                  </Typography>
                  {currentSnapshot.resumeDocumentFileName && (
                    <Typography variant="body2" color="text.secondary" mt={0.5}>
                      <strong>File:</strong> {currentSnapshot.resumeDocumentFileName}
                    </Typography>
                  )}
                  {currentSnapshot.resumeDocumentSha256 && (
                    <Box mt={1} p={1} sx={{ bgcolor: 'action.hover', borderRadius: 1, fontFamily: 'monospace', fontSize: '0.8rem' }}>
                      <Box display="flex" alignItems="center" gap={0.5}>
                        <FingerprintIcon fontSize="small" />
                        <strong>SHA-256 Checksum:</strong>
                      </Box>
                      <Typography variant="caption" sx={{ wordBreak: 'break-all' }}>
                        {currentSnapshot.resumeDocumentSha256}
                      </Typography>
                    </Box>
                  )}
                  {currentSnapshot.resumeContentSnapshot && (
                    <Typography variant="body2" color="text.secondary" mt={1}>
                      {currentSnapshot.resumeContentSnapshot}
                    </Typography>
                  )}
                </Paper>
              </Box>

              {/* Frozen Cover Letter Body */}
              {currentSnapshot.coverLetterTitle && (
                <Box>
                  <Typography variant="subtitle2" fontWeight={700} color="text.secondary" gutterBottom>
                    FROZEN COVER LETTER PITCH
                  </Typography>
                  <Paper variant="outlined" sx={{ p: 2, bgcolor: 'background.default' }}>
                    <Typography variant="body1" fontWeight={700} mb={1}>
                      {currentSnapshot.coverLetterTitle}
                    </Typography>
                    <Typography variant="body2" sx={{ whiteSpace: 'pre-wrap', fontStyle: 'italic' }}>
                      {currentSnapshot.coverLetterBodySnapshot || 'No body content.'}
                    </Typography>
                  </Paper>
                </Box>
              )}

              {/* Frozen Submission Context */}
              <Box>
                <Typography variant="subtitle2" fontWeight={700} color="text.secondary" gutterBottom>
                  SUBMISSION METADATA
                </Typography>
                <Paper variant="outlined" sx={{ p: 2, bgcolor: 'background.default' }}>
                  <Typography variant="body2"><strong>Channel:</strong> {currentSnapshot.channel}</Typography>
                  {currentSnapshot.referralContactName && (
                    <Typography variant="body2" mt={0.5}>
                      <strong>Referral:</strong> {currentSnapshot.referralContactName}
                    </Typography>
                  )}
                  {currentSnapshot.submissionNotes && (
                    <Typography variant="body2" color="text.secondary" mt={0.5}>
                      <strong>Notes:</strong> {currentSnapshot.submissionNotes}
                    </Typography>
                  )}
                </Paper>
              </Box>
            </Stack>
          )}
        </DialogContent>
        <DialogActions sx={{ px: 3, py: 1.5 }}>
          <Button onClick={() => setSnapshotOpen(false)}>Close</Button>
        </DialogActions>
      </Dialog>
    </Box>
  )
}
