import { useEffect, useState } from 'react'
import {
  Box, Typography, Paper, Table, TableBody, TableCell, TableContainer,
  TableHead, TableRow, IconButton, Fab, Dialog, DialogTitle, DialogContent,
  DialogActions, Button, TextField, MenuItem, CircularProgress, Alert,
} from '@mui/material'
import AddIcon from '@mui/icons-material/Add'
import DeleteIcon from '@mui/icons-material/Delete'
import EditIcon from '@mui/icons-material/Edit'
import StatusChip from '../components/StatusChip'
import { jobsApi, type Job, type CreateJobRequest } from '../api/jobs'

const JOB_STATUSES = [
  { value: 0, label: 'Saved' }, { value: 1, label: 'Applied' },
  { value: 2, label: 'Phone Screen' }, { value: 3, label: 'Interview' },
  { value: 4, label: 'Offer' }, { value: 5, label: 'Rejected' },
  { value: 6, label: 'Withdrawn' }, { value: 7, label: 'Accepted' },
]

const emptyForm: CreateJobRequest = { title: '', company: '', location: '', url: '', status: 0, salary: '', notes: '' }

export default function JobsPage() {
  const [jobs, setJobs] = useState<Job[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [dialogOpen, setDialogOpen] = useState(false)
  const [editJob, setEditJob] = useState<Job | null>(null)
  const [form, setForm] = useState<CreateJobRequest>(emptyForm)
  const [saving, setSaving] = useState(false)

  const load = () => {
    setLoading(true)
    jobsApi.getAll().then(setJobs).catch(() => setError('Failed to load jobs')).finally(() => setLoading(false))
  }

  useEffect(() => { load() }, [])

  const openCreate = () => { setEditJob(null); setForm(emptyForm); setDialogOpen(true) }
  const openEdit = (job: Job) => {
    setEditJob(job)
    setForm({ title: job.title, company: job.company, location: job.location ?? '', url: job.url ?? '', status: job.status, salary: job.salary ?? '', notes: job.notes ?? '' })
    setDialogOpen(true)
  }

  const handleSave = async () => {
    setSaving(true)
    try {
      if (editJob) await jobsApi.update(editJob.id, { ...form, appliedAt: editJob.appliedAt, interviewAt: editJob.interviewAt })
      else await jobsApi.create(form)
      setDialogOpen(false)
      load()
    } catch { setError('Failed to save job') }
    finally { setSaving(false) }
  }

  const handleDelete = async (id: string) => {
    if (!confirm('Delete this job?')) return
    try { await jobsApi.delete(id); load() } catch { setError('Failed to delete job') }
  }

  return (
    <Box>
      <Typography variant="h5" gutterBottom fontWeight={700}>Jobs</Typography>
      {error && <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError(null)}>{error}</Alert>}
      {loading ? (
        <Box display="flex" justifyContent="center" mt={8}><CircularProgress /></Box>
      ) : (
        <TableContainer component={Paper}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell><strong>Title</strong></TableCell>
                <TableCell><strong>Company</strong></TableCell>
                <TableCell><strong>Location</strong></TableCell>
                <TableCell><strong>Status</strong></TableCell>
                <TableCell><strong>Salary</strong></TableCell>
                <TableCell><strong>Actions</strong></TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {jobs.length === 0 ? (
                <TableRow><TableCell colSpan={6} align="center" sx={{ py: 6, color: 'text.secondary' }}>No jobs yet. Click + to add one.</TableCell></TableRow>
              ) : jobs.map((job) => (
                <TableRow key={job.id} hover>
                  <TableCell><Typography fontWeight={500}>{job.title}</Typography></TableCell>
                  <TableCell>{job.company}</TableCell>
                  <TableCell>{job.location ?? '—'}</TableCell>
                  <TableCell><StatusChip status={job.status} /></TableCell>
                  <TableCell>{job.salary ?? '—'}</TableCell>
                  <TableCell>
                    <IconButton size="small" onClick={() => openEdit(job)}><EditIcon fontSize="small" /></IconButton>
                    <IconButton size="small" color="error" onClick={() => handleDelete(job.id)}><DeleteIcon fontSize="small" /></IconButton>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      )}
      <Fab color="primary" sx={{ position: 'fixed', bottom: 32, right: 32 }} onClick={openCreate}>
        <AddIcon />
      </Fab>
      <Dialog open={dialogOpen} onClose={() => setDialogOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>{editJob ? 'Edit Job' : 'Add Job'}</DialogTitle>
        <DialogContent sx={{ display: 'flex', flexDirection: 'column', gap: 2, pt: 2 }}>
          <TextField label="Job Title *" value={form.title} onChange={(e) => setForm({ ...form, title: e.target.value })} fullWidth />
          <TextField label="Company *" value={form.company} onChange={(e) => setForm({ ...form, company: e.target.value })} fullWidth />
          <TextField label="Location" value={form.location} onChange={(e) => setForm({ ...form, location: e.target.value })} fullWidth />
          <TextField label="Job URL" value={form.url} onChange={(e) => setForm({ ...form, url: e.target.value })} fullWidth />
          <TextField label="Salary" value={form.salary} onChange={(e) => setForm({ ...form, salary: e.target.value })} fullWidth />
          <TextField select label="Status" value={form.status} onChange={(e) => setForm({ ...form, status: Number(e.target.value) })} fullWidth>
            {JOB_STATUSES.map((s) => <MenuItem key={s.value} value={s.value}>{s.label}</MenuItem>)}
          </TextField>
          <TextField label="Notes" value={form.notes} onChange={(e) => setForm({ ...form, notes: e.target.value })} fullWidth multiline rows={3} />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDialogOpen(false)}>Cancel</Button>
          <Button variant="contained" onClick={handleSave} disabled={saving}>{saving ? 'Saving...' : 'Save'}</Button>
        </DialogActions>
      </Dialog>
    </Box>
  )
}

