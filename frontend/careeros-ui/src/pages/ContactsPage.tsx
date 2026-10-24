import { useEffect, useState } from 'react'
import {
  Box, Typography, Paper, Table, TableBody, TableCell, TableContainer,
  TableHead, TableRow, IconButton, Fab, Dialog, DialogTitle, DialogContent,
  DialogActions, Button, TextField, CircularProgress, Alert,
} from '@mui/material'
import AddIcon from '@mui/icons-material/Add'
import DeleteIcon from '@mui/icons-material/Delete'
import EditIcon from '@mui/icons-material/Edit'
import { contactsApi, type Contact, type CreateContactRequest } from '../api/contacts'

const emptyForm: CreateContactRequest = { name: '', email: '', phone: '', company: '', jobTitle: '', linkedIn: '', notes: '' }

export default function ContactsPage() {
  const [contacts, setContacts] = useState<Contact[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [dialogOpen, setDialogOpen] = useState(false)
  const [editContact, setEditContact] = useState<Contact | null>(null)
  const [form, setForm] = useState<CreateContactRequest>(emptyForm)
  const [saving, setSaving] = useState(false)

  const load = () => {
    setLoading(true)
    contactsApi.getAll().then(setContacts).catch(() => setError('Failed to load contacts')).finally(() => setLoading(false))
  }

  useEffect(() => { load() }, [])

  const openCreate = () => { setEditContact(null); setForm(emptyForm); setDialogOpen(true) }
  const openEdit = (c: Contact) => {
    setEditContact(c)
    setForm({ name: c.name, email: c.email ?? '', phone: c.phone ?? '', company: c.company ?? '', jobTitle: c.jobTitle ?? '', linkedIn: c.linkedIn ?? '', notes: c.notes ?? '' })
    setDialogOpen(true)
  }

  const handleSave = async () => {
    setSaving(true)
    try {
      if (editContact) await contactsApi.update(editContact.id, form)
      else await contactsApi.create(form)
      setDialogOpen(false); load()
    } catch { setError('Failed to save contact') }
    finally { setSaving(false) }
  }

  const handleDelete = async (id: string) => {
    if (!confirm('Delete this contact?')) return
    try { await contactsApi.delete(id); load() } catch { setError('Failed to delete') }
  }

  return (
    <Box>
      <Typography variant="h5" gutterBottom fontWeight={700}>Contacts</Typography>
      {error && <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError(null)}>{error}</Alert>}
      {loading ? (
        <Box display="flex" justifyContent="center" mt={8}><CircularProgress /></Box>
      ) : (
        <TableContainer component={Paper}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell><strong>Name</strong></TableCell>
                <TableCell><strong>Email</strong></TableCell>
                <TableCell><strong>Company</strong></TableCell>
                <TableCell><strong>Title</strong></TableCell>
                <TableCell><strong>Phone</strong></TableCell>
                <TableCell><strong>Actions</strong></TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {contacts.length === 0 ? (
                <TableRow><TableCell colSpan={6} align="center" sx={{ py: 6, color: 'text.secondary' }}>No contacts yet. Click + to add one.</TableCell></TableRow>
              ) : contacts.map((c) => (
                <TableRow key={c.id} hover>
                  <TableCell><Typography fontWeight={500}>{c.name}</Typography></TableCell>
                  <TableCell>{c.email ?? '—'}</TableCell>
                  <TableCell>{c.company ?? '—'}</TableCell>
                  <TableCell>{c.jobTitle ?? '—'}</TableCell>
                  <TableCell>{c.phone ?? '—'}</TableCell>
                  <TableCell>
                    <IconButton size="small" onClick={() => openEdit(c)}><EditIcon fontSize="small" /></IconButton>
                    <IconButton size="small" color="error" onClick={() => handleDelete(c.id)}><DeleteIcon fontSize="small" /></IconButton>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      )}
      <Fab color="primary" sx={{ position: 'fixed', bottom: 32, right: 32 }} onClick={openCreate}><AddIcon /></Fab>
      <Dialog open={dialogOpen} onClose={() => setDialogOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>{editContact ? 'Edit Contact' : 'Add Contact'}</DialogTitle>
        <DialogContent sx={{ display: 'flex', flexDirection: 'column', gap: 2, pt: 2 }}>
          <TextField label="Name *" value={form.name} onChange={(e) => setForm({ ...form, name: e.target.value })} fullWidth />
          <TextField label="Email" value={form.email} onChange={(e) => setForm({ ...form, email: e.target.value })} fullWidth />
          <TextField label="Phone" value={form.phone} onChange={(e) => setForm({ ...form, phone: e.target.value })} fullWidth />
          <TextField label="Company" value={form.company} onChange={(e) => setForm({ ...form, company: e.target.value })} fullWidth />
          <TextField label="Job Title" value={form.jobTitle} onChange={(e) => setForm({ ...form, jobTitle: e.target.value })} fullWidth />
          <TextField label="LinkedIn URL" value={form.linkedIn} onChange={(e) => setForm({ ...form, linkedIn: e.target.value })} fullWidth />
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

