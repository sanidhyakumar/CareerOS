import { useEffect, useState } from 'react'
import {
  Box, Typography, Grid, Card, CardContent, CardActions, IconButton, Fab,
  Dialog, DialogTitle, DialogContent, DialogActions, Button, TextField,
  FormControlLabel, Switch, Chip, CircularProgress, Alert,
} from '@mui/material'
import AddIcon from '@mui/icons-material/Add'
import DeleteIcon from '@mui/icons-material/Delete'
import EditIcon from '@mui/icons-material/Edit'
import DescriptionIcon from '@mui/icons-material/Description'
import StarIcon from '@mui/icons-material/Star'
import { resumesApi, type Resume, type CreateResumeRequest } from '../api/resumes'

const emptyForm: CreateResumeRequest = { name: '', content: '', isDefault: false, tags: '' }

export default function ResumesPage() {
  const [resumes, setResumes] = useState<Resume[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [dialogOpen, setDialogOpen] = useState(false)
  const [editResume, setEditResume] = useState<Resume | null>(null)
  const [form, setForm] = useState<CreateResumeRequest>(emptyForm)
  const [saving, setSaving] = useState(false)

  const load = () => {
    setLoading(true)
    resumesApi.getAll().then(setResumes).catch(() => setError('Failed to load resumes')).finally(() => setLoading(false))
  }

  useEffect(() => { load() }, [])

  const openCreate = () => { setEditResume(null); setForm(emptyForm); setDialogOpen(true) }
  const openEdit = (r: Resume) => {
    setEditResume(r)
    setForm({ name: r.name, content: r.content ?? '', isDefault: r.isDefault, tags: r.tags ?? '' })
    setDialogOpen(true)
  }

  const handleSave = async () => {
    setSaving(true)
    try {
      if (editResume) await resumesApi.update(editResume.id, form)
      else await resumesApi.create(form)
      setDialogOpen(false); load()
    } catch { setError('Failed to save resume') }
    finally { setSaving(false) }
  }

  const handleDelete = async (id: string) => {
    if (!confirm('Delete this resume?')) return
    try { await resumesApi.delete(id); load() } catch { setError('Failed to delete') }
  }

  return (
    <Box>
      <Typography variant="h5" gutterBottom fontWeight={700}>Resumes</Typography>
      {error && <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError(null)}>{error}</Alert>}
      {loading ? (
        <Box display="flex" justifyContent="center" mt={8}><CircularProgress /></Box>
      ) : (
        <Grid container spacing={3}>
          {resumes.length === 0 ? (
            <Grid item xs={12}>
              <Box textAlign="center" py={8} color="text.secondary">
                <DescriptionIcon sx={{ fontSize: 64, opacity: 0.3 }} />
                <Typography mt={2}>No resumes yet. Click + to add one.</Typography>
              </Box>
            </Grid>
          ) : resumes.map((r) => (
            <Grid item xs={12} sm={6} md={4} key={r.id}>
              <Card>
                <CardContent>
                  <Box display="flex" alignItems="flex-start" justifyContent="space-between">
                    <Box display="flex" alignItems="center" gap={1}>
                      <DescriptionIcon color="primary" />
                      <Typography variant="h6" fontWeight={600}>{r.name}</Typography>
                    </Box>
                    {r.isDefault && <StarIcon sx={{ color: 'warning.main' }} />}
                  </Box>
                  <Typography variant="caption" color="text.secondary">v{r.version}</Typography>
                  {r.tags && (
                    <Box mt={1} display="flex" flexWrap="wrap" gap={0.5}>
                      {r.tags.split(',').map((t) => <Chip key={t} label={t.trim()} size="small" />)}
                    </Box>
                  )}
                  {r.content && (
                    <Typography variant="body2" color="text.secondary" mt={1} sx={{ overflow: 'hidden', textOverflow: 'ellipsis', display: '-webkit-box', WebkitLineClamp: 2, WebkitBoxOrient: 'vertical' }}>
                      {r.content}
                    </Typography>
                  )}
                </CardContent>
                <CardActions>
                  <IconButton size="small" onClick={() => openEdit(r)}><EditIcon fontSize="small" /></IconButton>
                  <IconButton size="small" color="error" onClick={() => handleDelete(r.id)}><DeleteIcon fontSize="small" /></IconButton>
                </CardActions>
              </Card>
            </Grid>
          ))}
        </Grid>
      )}
      <Fab color="primary" sx={{ position: 'fixed', bottom: 32, right: 32 }} onClick={openCreate}><AddIcon /></Fab>
      <Dialog open={dialogOpen} onClose={() => setDialogOpen(false)} maxWidth="md" fullWidth>
        <DialogTitle>{editResume ? 'Edit Resume' : 'Add Resume'}</DialogTitle>
        <DialogContent sx={{ display: 'flex', flexDirection: 'column', gap: 2, pt: 2 }}>
          <TextField label="Resume Name *" value={form.name} onChange={(e) => setForm({ ...form, name: e.target.value })} fullWidth />
          <TextField label="Tags (comma separated)" value={form.tags} onChange={(e) => setForm({ ...form, tags: e.target.value })} fullWidth placeholder="e.g. Software, Senior, Remote" />
          <TextField label="Content / Paste Resume Text" value={form.content} onChange={(e) => setForm({ ...form, content: e.target.value })} fullWidth multiline rows={10} />
          <FormControlLabel control={<Switch checked={form.isDefault} onChange={(e) => setForm({ ...form, isDefault: e.target.checked })} />} label="Set as Default Resume" />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDialogOpen(false)}>Cancel</Button>
          <Button variant="contained" onClick={handleSave} disabled={saving}>{saving ? 'Saving...' : 'Save'}</Button>
        </DialogActions>
      </Dialog>
    </Box>
  )
}

