import { useEffect, useState } from 'react'
import {
  Box, Typography, Paper, Table, TableBody, TableCell, TableContainer,
  TableHead, TableRow, IconButton, Dialog, DialogTitle, DialogContent,
  DialogActions, Button, TextField, MenuItem, CircularProgress, Alert,
  Grid, Card, CardContent, Chip, Tooltip, InputAdornment, FormControl,
  InputLabel, Select, ToggleButtonGroup, ToggleButton,
} from '@mui/material'
import AddIcon from '@mui/icons-material/Add'
import DeleteIcon from '@mui/icons-material/Delete'
import EditIcon from '@mui/icons-material/Edit'
import SearchIcon from '@mui/icons-material/Search'
import WorkIcon from '@mui/icons-material/Work'
import BusinessIcon from '@mui/icons-material/Business'
import LaunchIcon from '@mui/icons-material/Launch'
import ViewModuleIcon from '@mui/icons-material/ViewModule'
import ViewListIcon from '@mui/icons-material/ViewList'
import AttachMoneyIcon from '@mui/icons-material/AttachMoney'
import StarIcon from '@mui/icons-material/Star'
import LocationOnIcon from '@mui/icons-material/LocationOn'

import StatusChip from '../components/StatusChip'
import {
  jobsApi,
  JobStatus,
  JobPriority,
  EmploymentType,
  WorkplaceType,
  SeniorityLevel,
  type Job,
  type JobStats,
  type CreateJobRequest,
} from '../api/jobs'
import { companiesApi, type Company } from '../api/companies'

const JOB_STATUSES = [
  { value: JobStatus.Saved, label: 'Saved' },
  { value: JobStatus.Applied, label: 'Applied' },
  { value: JobStatus.PhoneScreen, label: 'Phone Screen' },
  { value: JobStatus.Interview, label: 'Interview' },
  { value: JobStatus.Offer, label: 'Offer' },
  { value: JobStatus.Rejected, label: 'Rejected' },
  { value: JobStatus.Withdrawn, label: 'Withdrawn' },
  { value: JobStatus.Accepted, label: 'Accepted' },
]

const emptyForm: CreateJobRequest = {
  title: '',
  company: '',
  companyId: undefined,
  department: '',
  location: 'Remote',
  url: '',
  description: '',
  notes: '',
  status: JobStatus.Saved,
  priority: JobPriority.Medium,
  employmentType: EmploymentType.FullTime,
  workplaceType: WorkplaceType.Remote,
  seniority: SeniorityLevel.Senior,
  minSalary: undefined,
  maxSalary: undefined,
  currency: 'USD',
  salary: '',
  equity: '',
  deadline: undefined,
  techStack: '',
}

export default function JobsPage() {
  const [jobs, setJobs] = useState<Job[]>([])
  const [stats, setStats] = useState<JobStats | null>(null)
  const [companies, setCompanies] = useState<Company[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [success, setSuccess] = useState<string | null>(null)

  // Filters
  const [searchTerm, setSearchTerm] = useState('')
  const [selectedCompanyId, setSelectedCompanyId] = useState<string>('all')
  const [selectedStatus, setSelectedStatus] = useState<string>('all')
  const [selectedPriority, setSelectedPriority] = useState<string>('all')
  const [selectedSeniority, setSelectedSeniority] = useState<string>('all')
  const [selectedWorkplace, setSelectedWorkplace] = useState<string>('all')
  const [viewMode, setViewMode] = useState<'grid' | 'table'>('grid')

  // Dialog
  const [dialogOpen, setDialogOpen] = useState(false)
  const [editJobId, setEditJobId] = useState<string | null>(null)
  const [form, setForm] = useState<CreateJobRequest>(emptyForm)
  const [saving, setSaving] = useState(false)

  const loadData = async () => {
    setLoading(true)
    try {
      const statusVal = selectedStatus !== 'all' ? (Number(selectedStatus) as JobStatus) : undefined
      const priorityVal = selectedPriority !== 'all' ? (Number(selectedPriority) as JobPriority) : undefined
      const seniorityVal = selectedSeniority !== 'all' ? (Number(selectedSeniority) as SeniorityLevel) : undefined
      const workplaceVal = selectedWorkplace !== 'all' ? (Number(selectedWorkplace) as WorkplaceType) : undefined
      const companyVal = selectedCompanyId !== 'all' ? selectedCompanyId : undefined

      const [jobsData, statsData, companiesData] = await Promise.all([
        jobsApi.getAll({
          searchTerm: searchTerm || undefined,
          companyId: companyVal,
          status: statusVal,
          priority: priorityVal,
          seniority: seniorityVal,
          workplaceType: workplaceVal,
        }),
        jobsApi.getStats().catch(() => null),
        companiesApi.getAll().catch(() => []),
      ])

      setJobs(jobsData)
      setStats(statsData)
      setCompanies(companiesData)
    } catch {
      setError('Failed to load job positions')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    loadData()
  }, [searchTerm, selectedCompanyId, selectedStatus, selectedPriority, selectedSeniority, selectedWorkplace])

  const openCreateDialog = () => {
    setEditJobId(null)
    setForm(emptyForm)
    setDialogOpen(true)
  }

  const openEditDialog = (job: Job) => {
    setEditJobId(job.id)
    setForm({
      title: job.title,
      company: job.company,
      companyId: job.companyId,
      department: job.department ?? '',
      location: job.location ?? '',
      url: job.url ?? '',
      description: job.description ?? '',
      notes: job.notes ?? '',
      status: job.status,
      priority: job.priority,
      employmentType: job.employmentType,
      workplaceType: job.workplaceType,
      seniority: job.seniority,
      minSalary: job.minSalary,
      maxSalary: job.maxSalary,
      currency: job.currency ?? 'USD',
      salary: job.salary ?? '',
      equity: job.equity ?? '',
      deadline: job.deadline ? job.deadline.split('T')[0] : undefined,
      techStack: job.techStack ?? '',
      appliedAt: job.appliedAt,
      interviewAt: job.interviewAt,
    })
    setDialogOpen(true)
  }

  const handleSave = async () => {
    setSaving(true)
    try {
      if (editJobId) {
        await jobsApi.update(editJobId, form)
        setSuccess('Position updated successfully')
      } else {
        await jobsApi.create(form)
        setSuccess('Position created and added to tracking')
      }
      setDialogOpen(false)
      loadData()
    } catch {
      setError('Failed to save position')
    } finally {
      setSaving(false)
    }
  }

  const handleDelete = async (id: string, title: string) => {
    if (!confirm(`Delete position "${title}"?`)) return
    try {
      await jobsApi.delete(id)
      setSuccess('Position deleted')
      loadData()
    } catch {
      setError('Failed to delete position')
    }
  }

  const handleCompanySelectChange = (companyId: string) => {
    if (!companyId) {
      setForm({ ...form, companyId: undefined })
      return
    }
    const matched = companies.find((c) => c.id === companyId)
    setForm({
      ...form,
      companyId: companyId,
      company: matched ? matched.name : form.company,
    })
  }

  return (
    <Box>
      {/* Header */}
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={2}>
        <Box>
          <Typography variant="h4" fontWeight={700}>Job Positions</Typography>
          <Typography variant="body2" color="text.secondary">
            Manage target roles, departments, salary ranges, seniority levels, and employer linkages.
          </Typography>
        </Box>
        <Button variant="contained" startIcon={<AddIcon />} onClick={openCreateDialog}>
          Add Position
        </Button>
      </Box>

      {error && <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError(null)}>{error}</Alert>}
      {success && <Alert severity="success" sx={{ mb: 2 }} onClose={() => setSuccess(null)}>{success}</Alert>}

      {/* KPI Stats Banner */}
      {stats && (
        <Grid container spacing={2} mb={3}>
          <Grid item xs={6} sm={2.4}>
            <Card variant="outlined">
              <CardContent sx={{ py: 1.5, '&:last-child': { pb: 1.5 } }}>
                <Typography variant="caption" color="text.secondary">Total Positions</Typography>
                <Typography variant="h5" fontWeight={700} color="primary.main">{stats.totalJobs}</Typography>
              </CardContent>
            </Card>
          </Grid>
          <Grid item xs={6} sm={2.4}>
            <Card variant="outlined">
              <CardContent sx={{ py: 1.5, '&:last-child': { pb: 1.5 } }}>
                <Typography variant="caption" color="text.secondary">Active Pipeline</Typography>
                <Typography variant="h5" fontWeight={700} color="info.main">{stats.activePositions}</Typography>
              </CardContent>
            </Card>
          </Grid>
          <Grid item xs={6} sm={2.4}>
            <Card variant="outlined">
              <CardContent sx={{ py: 1.5, '&:last-child': { pb: 1.5 } }}>
                <Typography variant="caption" color="text.secondary">Dream Priority</Typography>
                <Typography variant="h5" fontWeight={700} color="warning.main">{stats.dreamPriorityCount}</Typography>
              </CardContent>
            </Card>
          </Grid>
          <Grid item xs={6} sm={2.4}>
            <Card variant="outlined">
              <CardContent sx={{ py: 1.5, '&:last-child': { pb: 1.5 } }}>
                <Typography variant="caption" color="text.secondary">Remote Roles</Typography>
                <Typography variant="h5" fontWeight={700} color="success.main">{stats.remoteCount}</Typography>
              </CardContent>
            </Card>
          </Grid>
          <Grid item xs={6} sm={2.4}>
            <Card variant="outlined">
              <CardContent sx={{ py: 1.5, '&:last-child': { pb: 1.5 } }}>
                <Typography variant="caption" color="text.secondary">Avg Target Midpoint</Typography>
                <Typography variant="h5" fontWeight={700} color="secondary.main">
                  {stats.averageMidSalary ? `$${stats.averageMidSalary.toLocaleString()}` : '—'}
                </Typography>
              </CardContent>
            </Card>
          </Grid>
        </Grid>
      )}

      {/* Filter Toolbar */}
      <Paper variant="outlined" sx={{ p: 2, mb: 3 }}>
        <Grid container spacing={2} alignItems="center">
          <Grid item xs={12} sm={3}>
            <TextField
              placeholder="Search positions, tech stack..."
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
          <Grid item xs={6} sm={2}>
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
              <InputLabel>Priority</InputLabel>
              <Select
                value={selectedPriority}
                label="Priority"
                onChange={(e) => setSelectedPriority(e.target.value)}
              >
                <MenuItem value="all">All Priorities</MenuItem>
                <MenuItem value={JobPriority.Dream}>⭐ Dream Priority</MenuItem>
                <MenuItem value={JobPriority.High}>🔥 High Priority</MenuItem>
                <MenuItem value={JobPriority.Medium}>Medium Priority</MenuItem>
                <MenuItem value={JobPriority.Low}>Low Priority</MenuItem>
              </Select>
            </FormControl>
          </Grid>
          <Grid item xs={6} sm={1.8}>
            <FormControl size="small" fullWidth>
              <InputLabel>Seniority</InputLabel>
              <Select
                value={selectedSeniority}
                label="Seniority"
                onChange={(e) => setSelectedSeniority(e.target.value)}
              >
                <MenuItem value="all">All Levels</MenuItem>
                <MenuItem value={SeniorityLevel.Staff}>Staff</MenuItem>
                <MenuItem value={SeniorityLevel.Principal}>Principal</MenuItem>
                <MenuItem value={SeniorityLevel.Senior}>Senior</MenuItem>
                <MenuItem value={SeniorityLevel.Lead}>Lead</MenuItem>
                <MenuItem value={SeniorityLevel.Mid}>Mid-Level</MenuItem>
              </Select>
            </FormControl>
          </Grid>
          <Grid item xs={6} sm={1.8}>
            <FormControl size="small" fullWidth>
              <InputLabel>Workplace</InputLabel>
              <Select
                value={selectedWorkplace}
                label="Workplace"
                onChange={(e) => setSelectedWorkplace(e.target.value)}
              >
                <MenuItem value="all">All Modes</MenuItem>
                <MenuItem value={WorkplaceType.Remote}>Remote</MenuItem>
                <MenuItem value={WorkplaceType.Hybrid}>Hybrid</MenuItem>
                <MenuItem value={WorkplaceType.Onsite}>On-Site</MenuItem>
              </Select>
            </FormControl>
          </Grid>
          <Grid item xs={6} sm={1.8}>
            <FormControl size="small" fullWidth>
              <InputLabel>Status</InputLabel>
              <Select
                value={selectedStatus}
                label="Status"
                onChange={(e) => setSelectedStatus(e.target.value)}
              >
                <MenuItem value="all">All Statuses</MenuItem>
                {JOB_STATUSES.map((s) => (
                  <MenuItem key={s.value} value={s.value}>{s.label}</MenuItem>
                ))}
              </Select>
            </FormControl>
          </Grid>
          <Grid item xs={12} sm={1.4} display="flex" justifyContent="flex-end">
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

      {/* Positions Content */}
      {loading ? (
        <Box display="flex" justifyContent="center" mt={8}><CircularProgress /></Box>
      ) : jobs.length === 0 ? (
        <Box textAlign="center" py={8} color="text.secondary">
          <WorkIcon sx={{ fontSize: 64, opacity: 0.3 }} />
          <Typography mt={2}>No job positions found.</Typography>
          <Typography variant="body2" mt={0.5}>Adjust your filters or add a new position to begin tracking.</Typography>
          <Button variant="outlined" startIcon={<AddIcon />} sx={{ mt: 2 }} onClick={openCreateDialog}>
            Add First Position
          </Button>
        </Box>
      ) : viewMode === 'grid' ? (
        /* GRID CARDS VIEW */
        <Grid container spacing={3}>
          {jobs.map((job) => (
            <Grid item xs={12} sm={6} md={4} key={job.id}>
              <Card variant="outlined" sx={{ height: '100%', display: 'flex', flexDirection: 'column' }}>
                <CardContent sx={{ flexGrow: 1 }}>
                  {/* Header */}
                  <Box display="flex" justifyContent="space-between" alignItems="flex-start">
                    <Box>
                      <Box display="flex" alignItems="center" gap={1}>
                        <Typography variant="h6" fontWeight={700} lineHeight={1.2}>
                          {job.title}
                        </Typography>
                        {job.priority === JobPriority.Dream && (
                          <Tooltip title="Dream Opportunity">
                            <StarIcon sx={{ color: 'warning.main', fontSize: 18 }} />
                          </Tooltip>
                        )}
                      </Box>
                      <Box display="flex" alignItems="center" gap={1} mt={0.5}>
                        <BusinessIcon fontSize="small" color="primary" />
                        <Typography variant="subtitle2" fontWeight={600} color="primary.main">
                          {job.companyName ?? job.company}
                        </Typography>
                        {job.department && (
                          <Typography variant="caption" color="text.secondary">
                            ({job.department})
                          </Typography>
                        )}
                      </Box>
                    </Box>
                    <Box display="flex">
                      <IconButton size="small" onClick={() => openEditDialog(job)}>
                        <EditIcon fontSize="small" />
                      </IconButton>
                      <IconButton size="small" color="error" onClick={() => handleDelete(job.id, job.title)}>
                        <DeleteIcon fontSize="small" />
                      </IconButton>
                    </Box>
                  </Box>

                  {/* Badges / Taxonomy */}
                  <Box display="flex" flexWrap="wrap" gap={0.5} mt={1.5}>
                    <Chip size="small" label={job.seniorityName} variant="outlined" />
                    <Chip
                      size="small"
                      label={job.priorityName}
                      color={
                        job.priority === JobPriority.Dream ? 'warning' :
                        job.priority === JobPriority.High ? 'error' : 'default'
                      }
                    />
                    <Chip size="small" label={job.workplaceTypeName} color="info" variant="outlined" />
                    <Chip size="small" label={job.employmentTypeName} variant="outlined" />
                  </Box>

                  {/* Compensation & Location */}
                  <Box mt={1.5} display="flex" flexDirection="column" gap={0.5}>
                    {(job.salary || job.minSalary || job.maxSalary) && (
                      <Box display="flex" alignItems="center" gap={0.5}>
                        <AttachMoneyIcon fontSize="small" color="success" />
                        <Typography variant="body2" fontWeight={600} color="success.main">
                          {job.salary || `$${job.minSalary?.toLocaleString()} - $${job.maxSalary?.toLocaleString()} ${job.currency}`}
                        </Typography>
                        {job.equity && (
                          <Typography variant="caption" color="text.secondary">
                            (+ {job.equity})
                          </Typography>
                        )}
                      </Box>
                    )}
                    {job.location && (
                      <Box display="flex" alignItems="center" gap={0.5}>
                        <LocationOnIcon fontSize="small" color="action" />
                        <Typography variant="caption" color="text.secondary">
                          {job.location}
                        </Typography>
                      </Box>
                    )}
                  </Box>

                  {/* Tech stack */}
                  {job.techStack && (
                    <Box display="flex" flexWrap="wrap" gap={0.5} mt={1.5}>
                      {job.techStack.split(',').map((tech) => (
                        <Chip key={tech} size="small" label={tech.trim()} sx={{ fontSize: '0.7rem' }} />
                      ))}
                    </Box>
                  )}

                  {/* Footer status & link */}
                  <Box display="flex" justifyContent="space-between" alignItems="center" mt={2} pt={1} borderTop={1} borderColor="divider">
                    <StatusChip status={job.status} />
                    {job.url && (
                      <Tooltip title="Open Job Posting">
                        <IconButton size="small" component="a" href={job.url} target="_blank" rel="noopener noreferrer">
                          <LaunchIcon fontSize="small" />
                        </IconButton>
                      </Tooltip>
                    )}
                  </Box>
                </CardContent>
              </Card>
            </Grid>
          ))}
        </Grid>
      ) : (
        /* TABLE VIEW */
        <TableContainer component={Paper} variant="outlined">
          <Table>
            <TableHead>
              <TableRow>
                <TableCell><strong>Role / Title</strong></TableCell>
                <TableCell><strong>Company</strong></TableCell>
                <TableCell><strong>Department</strong></TableCell>
                <TableCell><strong>Seniority & Priority</strong></TableCell>
                <TableCell><strong>Workplace</strong></TableCell>
                <TableCell><strong>Compensation</strong></TableCell>
                <TableCell><strong>Status</strong></TableCell>
                <TableCell align="right"><strong>Actions</strong></TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {jobs.map((job) => (
                <TableRow key={job.id} hover>
                  <TableCell>
                    <Box display="flex" alignItems="center" gap={1}>
                      <Typography fontWeight={700}>{job.title}</Typography>
                      {job.priority === JobPriority.Dream && (
                        <StarIcon sx={{ color: 'warning.main', fontSize: 16 }} />
                      )}
                    </Box>
                    {job.url && (
                      <Typography variant="caption">
                        <a href={job.url} target="_blank" rel="noopener noreferrer">Posting Link</a>
                      </Typography>
                    )}
                  </TableCell>
                  <TableCell>{job.companyName ?? job.company}</TableCell>
                  <TableCell>{job.department ?? '—'}</TableCell>
                  <TableCell>
                    <Chip size="small" label={job.seniorityName} variant="outlined" sx={{ mr: 0.5 }} />
                    <Chip
                      size="small"
                      label={job.priorityName}
                      color={
                        job.priority === JobPriority.Dream ? 'warning' :
                        job.priority === JobPriority.High ? 'error' : 'default'
                      }
                    />
                  </TableCell>
                  <TableCell><Chip size="small" label={job.workplaceTypeName} variant="outlined" /></TableCell>
                  <TableCell>{job.salary || '—'}</TableCell>
                  <TableCell><StatusChip status={job.status} /></TableCell>
                  <TableCell align="right">
                    <IconButton size="small" onClick={() => openEditDialog(job)}>
                      <EditIcon fontSize="small" />
                    </IconButton>
                    <IconButton size="small" color="error" onClick={() => handleDelete(job.id, job.title)}>
                      <DeleteIcon fontSize="small" />
                    </IconButton>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      )}

      {/* CREATE / EDIT POSITION DIALOG */}
      <Dialog open={dialogOpen} onClose={() => setDialogOpen(false)} maxWidth="md" fullWidth>
        <DialogTitle>{editJobId ? 'Edit Job Position' : 'Add New Job Position'}</DialogTitle>
        <DialogContent sx={{ display: 'flex', flexDirection: 'column', gap: 2, pt: 2 }}>
          <Grid container spacing={2}>
            <Grid item xs={8}>
              <TextField
                label="Role / Position Title"
                placeholder="e.g. Staff Software Engineer, Engineering Manager"
                value={form.title}
                onChange={(e) => setForm({ ...form, title: e.target.value })}
                fullWidth
                required
              />
            </Grid>
            <Grid item xs={4}>
              <TextField
                label="Department / Team"
                placeholder="e.g. Payments Infrastructure, Core API"
                value={form.department ?? ''}
                onChange={(e) => setForm({ ...form, department: e.target.value })}
                fullWidth
              />
            </Grid>
          </Grid>

          <Grid container spacing={2}>
            <Grid item xs={6}>
              <FormControl fullWidth>
                <InputLabel>Company (From CRM)</InputLabel>
                <Select
                  value={form.companyId ?? ''}
                  label="Company (From CRM)"
                  onChange={(e) => handleCompanySelectChange(e.target.value)}
                >
                  <MenuItem value=""><em>-- Custom / Standalone Employer --</em></MenuItem>
                  {companies.map((c) => (
                    <MenuItem key={c.id} value={c.id}>{c.name}</MenuItem>
                  ))}
                </Select>
              </FormControl>
            </Grid>
            <Grid item xs={6}>
              <TextField
                label="Company Name"
                placeholder="e.g. Stripe, Google"
                value={form.company}
                onChange={(e) => setForm({ ...form, company: e.target.value })}
                fullWidth
                required
              />
            </Grid>
          </Grid>

          <Grid container spacing={2}>
            <Grid item xs={3}>
              <FormControl fullWidth>
                <InputLabel>Seniority</InputLabel>
                <Select
                  value={form.seniority}
                  label="Seniority"
                  onChange={(e) => setForm({ ...form, seniority: Number(e.target.value) as SeniorityLevel })}
                >
                  <MenuItem value={SeniorityLevel.Entry}>Entry Level</MenuItem>
                  <MenuItem value={SeniorityLevel.Mid}>Mid-Level</MenuItem>
                  <MenuItem value={SeniorityLevel.Senior}>Senior</MenuItem>
                  <MenuItem value={SeniorityLevel.Lead}>Lead</MenuItem>
                  <MenuItem value={SeniorityLevel.Staff}>Staff</MenuItem>
                  <MenuItem value={SeniorityLevel.Principal}>Principal</MenuItem>
                  <MenuItem value={SeniorityLevel.Director}>Director</MenuItem>
                  <MenuItem value={SeniorityLevel.Executive}>Executive</MenuItem>
                </Select>
              </FormControl>
            </Grid>
            <Grid item xs={3}>
              <FormControl fullWidth>
                <InputLabel>Priority</InputLabel>
                <Select
                  value={form.priority}
                  label="Priority"
                  onChange={(e) => setForm({ ...form, priority: Number(e.target.value) as JobPriority })}
                >
                  <MenuItem value={JobPriority.Low}>Low</MenuItem>
                  <MenuItem value={JobPriority.Medium}>Medium</MenuItem>
                  <MenuItem value={JobPriority.High}>High</MenuItem>
                  <MenuItem value={JobPriority.Dream}>⭐ Dream Opportunity</MenuItem>
                </Select>
              </FormControl>
            </Grid>
            <Grid item xs={3}>
              <FormControl fullWidth>
                <InputLabel>Employment Type</InputLabel>
                <Select
                  value={form.employmentType}
                  label="Employment Type"
                  onChange={(e) => setForm({ ...form, employmentType: Number(e.target.value) as EmploymentType })}
                >
                  <MenuItem value={EmploymentType.FullTime}>Full-Time</MenuItem>
                  <MenuItem value={EmploymentType.Contract}>Contract (W2)</MenuItem>
                  <MenuItem value={EmploymentType.ContractToHire}>Contract to Hire</MenuItem>
                  <MenuItem value={EmploymentType.Freelance}>Freelance / 1099</MenuItem>
                  <MenuItem value={EmploymentType.PartTime}>Part-Time</MenuItem>
                  <MenuItem value={EmploymentType.Internship}>Internship</MenuItem>
                </Select>
              </FormControl>
            </Grid>
            <Grid item xs={3}>
              <FormControl fullWidth>
                <InputLabel>Workplace Policy</InputLabel>
                <Select
                  value={form.workplaceType}
                  label="Workplace Policy"
                  onChange={(e) => setForm({ ...form, workplaceType: Number(e.target.value) as WorkplaceType })}
                >
                  <MenuItem value={WorkplaceType.Remote}>Remote</MenuItem>
                  <MenuItem value={WorkplaceType.Hybrid}>Hybrid</MenuItem>
                  <MenuItem value={WorkplaceType.Onsite}>Onsite</MenuItem>
                </Select>
              </FormControl>
            </Grid>
          </Grid>

          <Grid container spacing={2}>
            <Grid item xs={4}>
              <TextField
                label="Min Base Salary"
                placeholder="e.g. 180000"
                type="number"
                value={form.minSalary ?? ''}
                onChange={(e) => setForm({ ...form, minSalary: e.target.value ? Number(e.target.value) : undefined })}
                fullWidth
              />
            </Grid>
            <Grid item xs={4}>
              <TextField
                label="Max Base Salary"
                placeholder="e.g. 240000"
                type="number"
                value={form.maxSalary ?? ''}
                onChange={(e) => setForm({ ...form, maxSalary: e.target.value ? Number(e.target.value) : undefined })}
                fullWidth
              />
            </Grid>
            <Grid item xs={4}>
              <TextField
                label="Equity / Bonus"
                placeholder="e.g. $100k RSU + 15% Bonus"
                value={form.equity ?? ''}
                onChange={(e) => setForm({ ...form, equity: e.target.value })}
                fullWidth
              />
            </Grid>
          </Grid>

          <Grid container spacing={2}>
            <Grid item xs={6}>
              <TextField
                label="Location (City / State / Remote Details)"
                placeholder="e.g. San Francisco, CA (or US Remote)"
                value={form.location ?? ''}
                onChange={(e) => setForm({ ...form, location: e.target.value })}
                fullWidth
              />
            </Grid>
            <Grid item xs={6}>
              <TextField
                label="Job Posting URL"
                placeholder="https://..."
                value={form.url ?? ''}
                onChange={(e) => setForm({ ...form, url: e.target.value })}
                fullWidth
              />
            </Grid>
          </Grid>

          <TextField
            label="Tech Stack & Key Technologies (Comma-separated)"
            placeholder="e.g. .NET 9, React, TypeScript, Kafka, Docker, Kubernetes, AWS"
            value={form.techStack ?? ''}
            onChange={(e) => setForm({ ...form, techStack: e.target.value })}
            fullWidth
          />

          <Grid container spacing={2}>
            <Grid item xs={6}>
              <FormControl fullWidth>
                <InputLabel>Pipeline Status</InputLabel>
                <Select
                  value={form.status}
                  label="Pipeline Status"
                  onChange={(e) => setForm({ ...form, status: Number(e.target.value) as JobStatus })}
                >
                  {JOB_STATUSES.map((s) => (
                    <MenuItem key={s.value} value={s.value}>{s.label}</MenuItem>
                  ))}
                </Select>
              </FormControl>
            </Grid>
            <Grid item xs={6}>
              <TextField
                label="Application Deadline (Optional)"
                type="date"
                InputLabelProps={{ shrink: true }}
                value={form.deadline ?? ''}
                onChange={(e) => setForm({ ...form, deadline: e.target.value || undefined })}
                fullWidth
              />
            </Grid>
          </Grid>

          <TextField
            label="Job Description Summary"
            placeholder="Key responsibilities, team charter, expectations..."
            value={form.description ?? ''}
            onChange={(e) => setForm({ ...form, description: e.target.value })}
            multiline
            rows={3}
            fullWidth
          />

          <TextField
            label="Personal Notes & Strategy"
            placeholder="Referral notes, questions to ask, key achievements to highlight..."
            value={form.notes ?? ''}
            onChange={(e) => setForm({ ...form, notes: e.target.value })}
            multiline
            rows={2}
            fullWidth
          />
        </DialogContent>
        <DialogActions sx={{ px: 3, pb: 2 }}>
          <Button onClick={() => setDialogOpen(false)}>Cancel</Button>
          <Button variant="contained" onClick={handleSave} disabled={!form.title || !form.company || saving}>
            {saving ? 'Saving...' : 'Save Position'}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  )
}
