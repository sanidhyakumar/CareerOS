import React, { useEffect, useState } from 'react'
import {
  Box, Typography, Paper, Table, TableBody, TableCell, TableContainer,
  TableHead, TableRow, IconButton, Dialog, DialogTitle, DialogContent,
  DialogActions, Button, TextField, CircularProgress, Alert, Grid, Card,
  CardContent, Chip, Tooltip, InputAdornment, FormControl, InputLabel,
  Select, MenuItem, ToggleButtonGroup, ToggleButton, Divider, Stack, Tabs, Tab
} from '@mui/material'
import AddIcon from '@mui/icons-material/Add'
import DeleteIcon from '@mui/icons-material/Delete'
import EditIcon from '@mui/icons-material/Edit'
import SearchIcon from '@mui/icons-material/Search'
import StarIcon from '@mui/icons-material/Star'
import StarBorderIcon from '@mui/icons-material/StarBorder'
import EmailIcon from '@mui/icons-material/Email'
import PhoneIcon from '@mui/icons-material/Phone'
import LinkedInIcon from '@mui/icons-material/LinkedIn'
import BusinessIcon from '@mui/icons-material/Business'
import PeopleIcon from '@mui/icons-material/People'
import HistoryIcon from '@mui/icons-material/History'
import ViewModuleIcon from '@mui/icons-material/ViewModule'
import ViewListIcon from '@mui/icons-material/ViewList'
import AlarmIcon from '@mui/icons-material/Alarm'

import {
  contactsApi, ContactCategory
} from '../api/contacts'
import type {
  Contact, ContactStats, CreateContactRequest, ContactInteraction
} from '../api/contacts'
import { companiesApi } from '../api/companies'
import type { Company } from '../api/companies'
import { jobsApi } from '../api/jobs'
import type { Job } from '../api/jobs'

const CATEGORY_CONFIG: Record<ContactCategory, { label: string; color: 'primary' | 'secondary' | 'info' | 'success' | 'warning' | 'error' | 'default' }> = {
  [ContactCategory.Recruiter]: { label: 'Recruiter', color: 'info' },
  [ContactCategory.HiringManager]: { label: 'Hiring Manager', color: 'primary' },
  [ContactCategory.HumanResources]: { label: 'HR', color: 'secondary' },
  [ContactCategory.Referral]: { label: 'Referral', color: 'success' },
  [ContactCategory.ImmigrationContact]: { label: 'Immigration', color: 'warning' },
  [ContactCategory.PeerColleague]: { label: 'Peer / Colleague', color: 'default' },
  [ContactCategory.Executive]: { label: 'Executive', color: 'secondary' },
  [ContactCategory.Other]: { label: 'Other', color: 'default' },
}

const emptyForm: CreateContactRequest = {
  name: '',
  category: ContactCategory.Recruiter,
  email: '',
  phone: '',
  company: '',
  companyId: undefined,
  jobTitle: '',
  department: '',
  linkedIn: '',
  twitter: '',
  location: '',
  isStarred: false,
  notes: '',
  tags: '',
  jobId: undefined,
}

export default function ContactsPage() {
  const [contacts, setContacts] = useState<Contact[]>([])
  const [stats, setStats] = useState<ContactStats | null>(null)
  const [companies, setCompanies] = useState<Company[]>([])
  const [jobs, setJobs] = useState<Job[]>([])

  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [success, setSuccess] = useState<string | null>(null)

  // Filters
  const [searchTerm, setSearchTerm] = useState('')
  const [selectedCategoryTab, setSelectedCategoryTab] = useState<string>('all')
  const [selectedCompanyId, setSelectedCompanyId] = useState<string>('all')
  const [viewMode, setViewMode] = useState<'grid' | 'table'>('grid')

  // Create / Edit Contact Modal
  const [dialogOpen, setDialogOpen] = useState(false)
  const [editContactId, setEditContactId] = useState<string | null>(null)
  const [form, setForm] = useState<CreateContactRequest>(emptyForm)
  const [saving, setSaving] = useState(false)

  // Interactions Modal
  const [interactionContact, setInteractionContact] = useState<Contact | null>(null)
  const [interactionList, setInteractionList] = useState<ContactInteraction[]>([])
  const [interactionOpen, setInteractionOpen] = useState(false)
  const [newSummary, setNewSummary] = useState('')
  const [newChannel, setNewChannel] = useState('Email')
  const [newNotes, setNewNotes] = useState('')
  const [newFollowUpReq, setNewFollowUpReq] = useState(false)
  const [newFollowUpDate, setNewFollowUpDate] = useState('')
  const [savingInteraction, setSavingInteraction] = useState(false)

  const loadData = async () => {
    setLoading(true)
    try {
      let categoryFilter: ContactCategory | undefined = undefined
      let starredFilter: boolean | undefined = undefined

      if (selectedCategoryTab === 'starred') {
        starredFilter = true
      } else if (selectedCategoryTab !== 'all') {
        categoryFilter = Number(selectedCategoryTab) as ContactCategory
      }

      const companyFilter = selectedCompanyId !== 'all' ? selectedCompanyId : undefined

      const [contactsData, statsData, companiesData, jobsData] = await Promise.all([
        contactsApi.getAll({
          searchTerm: searchTerm || undefined,
          category: categoryFilter,
          companyId: companyFilter,
          isStarred: starredFilter,
        }),
        contactsApi.getStats().catch(() => null),
        companiesApi.getAll().catch(() => []),
        jobsApi.getAll().catch(() => []),
      ])

      setContacts(contactsData)
      setStats(statsData)
      setCompanies(companiesData)
      setJobs(jobsData)
    } catch {
      setError('Failed to load contacts CRM data.')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    loadData()
  }, [searchTerm, selectedCategoryTab, selectedCompanyId])

  const openCreate = () => {
    setEditContactId(null)
    setForm(emptyForm)
    setDialogOpen(true)
  }

  const openEdit = (c: Contact) => {
    setEditContactId(c.id)
    setForm({
      name: c.name,
      category: c.category ?? ContactCategory.Recruiter,
      email: c.email || '',
      phone: c.phone || '',
      company: c.company || '',
      companyId: c.companyId,
      jobTitle: c.jobTitle || '',
      department: c.department || '',
      linkedIn: c.linkedIn || '',
      twitter: c.twitter || '',
      location: c.location || '',
      isStarred: c.isStarred,
      notes: c.notes || '',
      tags: c.tags || '',
      jobId: c.jobId,
    })
    setDialogOpen(true)
  }

  const handleSaveContact = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!form.name.trim()) return

    setSaving(true)
    setError(null)
    try {
      if (editContactId) {
        await contactsApi.update(editContactId, {
          ...form,
          id: editContactId,
        })
        setSuccess('Contact updated successfully.')
      } else {
        await contactsApi.create(form)
        setSuccess('New contact added to CRM.')
      }
      setDialogOpen(false)
      loadData()
    } catch {
      setError('Failed to save contact.')
    } finally {
      setSaving(false)
    }
  }

  const handleDelete = async (id: string) => {
    if (!window.confirm('Delete this contact and all interaction history?')) return
    try {
      await contactsApi.delete(id)
      setSuccess('Contact deleted.')
      loadData()
    } catch {
      setError('Failed to delete contact.')
    }
  }

  const handleToggleStar = async (contact: Contact) => {
    try {
      await contactsApi.toggleStar(contact.id)
      loadData()
    } catch {
      setError('Failed to update favorite status.')
    }
  }

  const handleOpenInteractions = async (contact: Contact) => {
    setInteractionContact(contact)
    setInteractionOpen(true)
    setNewSummary('')
    setNewNotes('')
    setNewChannel('Email')
    setNewFollowUpReq(false)
    setNewFollowUpDate('')
    try {
      const items = await contactsApi.getInteractions(contact.id)
      setInteractionList(items)
    } catch {
      setError('Failed to load interaction history.')
    }
  }

  const handleAddInteraction = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!interactionContact || !newSummary.trim()) return

    setSavingInteraction(true)
    try {
      await contactsApi.addInteraction(interactionContact.id, {
        summary: newSummary.trim(),
        channel: newChannel,
        notes: newNotes.trim() || undefined,
        followUpRequired: newFollowUpReq,
        followUpDate: newFollowUpReq && newFollowUpDate ? new Date(newFollowUpDate).toISOString() : undefined,
      })
      const items = await contactsApi.getInteractions(interactionContact.id)
      setInteractionList(items)
      setNewSummary('')
      setNewNotes('')
      setNewFollowUpReq(false)
      setNewFollowUpDate('')
      setSuccess('Interaction logged.')
      loadData()
    } catch {
      setError('Failed to log interaction.')
    } finally {
      setSavingInteraction(false)
    }
  }

  const handleDeleteInteraction = async (interactionId: string) => {
    if (!interactionContact) return
    try {
      await contactsApi.deleteInteraction(interactionContact.id, interactionId)
      const items = await contactsApi.getInteractions(interactionContact.id)
      setInteractionList(items)
      loadData()
    } catch {
      setError('Failed to delete interaction.')
    }
  }

  return (
    <Box sx={{ p: 3 }}>
      {/* Page Header */}
      <Box display="flex" justifyContent="space-between" alignItems="flex-start" mb={3}>
        <Box>
          <Box display="flex" alignItems="center" gap={1.5}>
            <PeopleIcon color="primary" sx={{ fontSize: 32 }} />
            <Typography variant="h4" fontWeight={700}>CRM Contacts</Typography>
          </Box>
          <Typography variant="body2" color="text.secondary" mt={0.5}>
            Manage recruiters, hiring managers, HR, employee referrals, and immigration contacts
          </Typography>
        </Box>
        <Button
          variant="contained"
          startIcon={<AddIcon />}
          onClick={openCreate}
          sx={{ borderRadius: 2 }}
        >
          Add Contact
        </Button>
      </Box>

      {/* Notifications */}
      {error && <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError(null)}>{error}</Alert>}
      {success && <Alert severity="success" sx={{ mb: 2 }} onClose={() => setSuccess(null)}>{success}</Alert>}

      {/* CRM Stats Banner */}
      {stats && (
        <Grid container spacing={2} mb={3}>
          <Grid item xs={6} sm={2}>
            <Card variant="outlined">
              <CardContent sx={{ py: 1.5, '&:last-child': { pb: 1.5 } }}>
                <Typography variant="caption" color="text.secondary">Total Contacts</Typography>
                <Typography variant="h5" fontWeight={700} color="primary.main">{stats.totalContacts}</Typography>
              </CardContent>
            </Card>
          </Grid>
          <Grid item xs={6} sm={2}>
            <Card variant="outlined">
              <CardContent sx={{ py: 1.5, '&:last-child': { pb: 1.5 } }}>
                <Typography variant="caption" color="text.secondary">Recruiters</Typography>
                <Typography variant="h5" fontWeight={700} color="info.main">{stats.recruitersCount}</Typography>
              </CardContent>
            </Card>
          </Grid>
          <Grid item xs={6} sm={2}>
            <Card variant="outlined">
              <CardContent sx={{ py: 1.5, '&:last-child': { pb: 1.5 } }}>
                <Typography variant="caption" color="text.secondary">Hiring Managers</Typography>
                <Typography variant="h5" fontWeight={700} color="primary.light">{stats.hiringManagersCount}</Typography>
              </CardContent>
            </Card>
          </Grid>
          <Grid item xs={6} sm={2}>
            <Card variant="outlined">
              <CardContent sx={{ py: 1.5, '&:last-child': { pb: 1.5 } }}>
                <Typography variant="caption" color="text.secondary">Referrals</Typography>
                <Typography variant="h5" fontWeight={700} color="success.main">{stats.referralsCount}</Typography>
              </CardContent>
            </Card>
          </Grid>
          <Grid item xs={6} sm={2}>
            <Card variant="outlined">
              <CardContent sx={{ py: 1.5, '&:last-child': { pb: 1.5 } }}>
                <Typography variant="caption" color="text.secondary">Immigration</Typography>
                <Typography variant="h5" fontWeight={700} color="warning.main">{stats.immigrationCount}</Typography>
              </CardContent>
            </Card>
          </Grid>
          <Grid item xs={6} sm={2}>
            <Card variant="outlined">
              <CardContent sx={{ py: 1.5, '&:last-child': { pb: 1.5 } }}>
                <Typography variant="caption" color="text.secondary">Follow-ups Due</Typography>
                <Typography variant="h5" fontWeight={700} color={stats.followUpsDueCount > 0 ? 'error.main' : 'text.secondary'}>
                  {stats.followUpsDueCount}
                </Typography>
              </CardContent>
            </Card>
          </Grid>
        </Grid>
      )}

      {/* Category Tabs */}
      <Paper variant="outlined" sx={{ mb: 2 }}>
        <Tabs
          value={selectedCategoryTab}
          onChange={(_, val) => setSelectedCategoryTab(val)}
          variant="scrollable"
          scrollButtons="auto"
          sx={{ px: 2, minHeight: 48 }}
        >
          <Tab label="All Contacts" value="all" />
          <Tab label="⭐ Starred" value="starred" />
          <Tab label="Recruiters" value={String(ContactCategory.Recruiter)} />
          <Tab label="Hiring Managers" value={String(ContactCategory.HiringManager)} />
          <Tab label="Human Resources" value={String(ContactCategory.HumanResources)} />
          <Tab label="Employee Referrals" value={String(ContactCategory.Referral)} />
          <Tab label="Immigration Contacts" value={String(ContactCategory.ImmigrationContact)} />
        </Tabs>
      </Paper>

      {/* Filter Toolbar */}
      <Paper variant="outlined" sx={{ p: 2, mb: 3 }}>
        <Grid container spacing={2} alignItems="center">
          <Grid item xs={12} sm={6}>
            <TextField
              placeholder="Search by name, company, title, email, tags..."
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
          <Grid item xs={6} sm={4}>
            <FormControl size="small" fullWidth>
              <InputLabel>Company Filter</InputLabel>
              <Select
                value={selectedCompanyId}
                label="Company Filter"
                onChange={(e) => setSelectedCompanyId(e.target.value)}
              >
                <MenuItem value="all">All Companies</MenuItem>
                {companies.map((c) => (
                  <MenuItem key={c.id} value={c.id}>{c.name}</MenuItem>
                ))}
              </Select>
            </FormControl>
          </Grid>
          <Grid item xs={6} sm={2} display="flex" justifyContent="flex-end">
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

      {/* Contacts Content */}
      {loading ? (
        <Box display="flex" justifyContent="center" mt={8}><CircularProgress /></Box>
      ) : contacts.length === 0 ? (
        <Box textAlign="center" py={8} color="text.secondary">
          <PeopleIcon sx={{ fontSize: 64, opacity: 0.3 }} />
          <Typography mt={2}>No contacts found in this category.</Typography>
          <Button variant="outlined" startIcon={<AddIcon />} onClick={openCreate} sx={{ mt: 2 }}>
            Add Contact
          </Button>
        </Box>
      ) : viewMode === 'grid' ? (
        <Grid container spacing={2}>
          {contacts.map((c) => {
            const catConfig = CATEGORY_CONFIG[c.category] || { label: c.categoryName, color: 'default' }
            const isFollowUpDue = c.nextFollowUpDate && new Date(c.nextFollowUpDate) <= new Date(Date.now() + 7 * 86400000)
            return (
              <Grid item xs={12} sm={6} md={4} key={c.id}>
                <Card variant="outlined" sx={{ height: '100%', display: 'flex', flexDirection: 'column' }}>
                  <CardContent sx={{ flexGrow: 1 }}>
                    <Box display="flex" justifyContent="space-between" alignItems="flex-start" mb={1}>
                      <Box display="flex" alignItems="center" gap={1}>
                        <IconButton size="small" onClick={() => handleToggleStar(c)} color={c.isStarred ? 'warning' : 'default'}>
                          {c.isStarred ? <StarIcon fontSize="small" /> : <StarBorderIcon fontSize="small" />}
                        </IconButton>
                        <Typography variant="h6" fontWeight={700} noWrap sx={{ maxWidth: 180 }}>
                          {c.name}
                        </Typography>
                      </Box>
                      <Chip label={catConfig.label} color={catConfig.color} size="small" />
                    </Box>

                    {/* Job Title & Company */}
                    {(c.jobTitle || c.company) && (
                      <Box display="flex" alignItems="center" gap={1} color="text.secondary" mb={1.5}>
                        <BusinessIcon fontSize="small" />
                        <Typography variant="body2" fontWeight={600} color="text.primary">
                          {c.jobTitle ? `${c.jobTitle}` : ''}
                          {c.jobTitle && c.company ? ' at ' : ''}
                          {c.company ? `${c.company}` : ''}
                        </Typography>
                      </Box>
                    )}

                    {/* Quick Channels */}
                    <Stack direction="row" spacing={1} mb={2}>
                      {c.email && (
                        <Tooltip title={`Email: ${c.email}`}>
                          <IconButton size="small" component="a" href={`mailto:${c.email}`} color="primary">
                            <EmailIcon fontSize="small" />
                          </IconButton>
                        </Tooltip>
                      )}
                      {c.phone && (
                        <Tooltip title={`Call: ${c.phone}`}>
                          <IconButton size="small" component="a" href={`tel:${c.phone}`} color="info">
                            <PhoneIcon fontSize="small" />
                          </IconButton>
                        </Tooltip>
                      )}
                      {c.linkedIn && (
                        <Tooltip title="Open LinkedIn Profile">
                          <IconButton size="small" component="a" href={c.linkedIn} target="_blank" rel="noopener noreferrer" color="primary">
                            <LinkedInIcon fontSize="small" />
                          </IconButton>
                        </Tooltip>
                      )}
                      {c.location && (
                        <Chip label={c.location} size="small" variant="outlined" />
                      )}
                    </Stack>

                    {/* Tags */}
                    {c.tags && (
                      <Box display="flex" flexWrap="wrap" gap={0.5} mb={1.5}>
                        {c.tags.split(',').map((t, i) => (
                          <Chip key={i} label={t.trim()} size="small" sx={{ fontSize: '0.75rem', height: 22 }} />
                        ))}
                      </Box>
                    )}

                    {/* Follow-up / Last Contact Alerts */}
                    <Box display="flex" flexDirection="column" gap={0.5}>
                      {c.nextFollowUpDate && (
                        <Chip
                          icon={<AlarmIcon />}
                          label={`Follow-up: ${new Date(c.nextFollowUpDate).toLocaleDateString()}`}
                          size="small"
                          color={isFollowUpDue ? 'error' : 'default'}
                          variant={isFollowUpDue ? 'filled' : 'outlined'}
                        />
                      )}
                      {c.lastContactDate && (
                        <Typography variant="caption" color="text.secondary">
                          Last contacted: {new Date(c.lastContactDate).toLocaleDateString()}
                        </Typography>
                      )}
                    </Box>

                    {c.notes && (
                      <Typography variant="caption" color="text.secondary" sx={{
                        mt: 1.5,
                        display: '-webkit-box',
                        WebkitLineClamp: 2,
                        WebkitBoxOrient: 'vertical',
                        overflow: 'hidden'
                      }}>
                        {c.notes}
                      </Typography>
                    )}
                  </CardContent>

                  <Divider />
                  <Box display="flex" justifyContent="space-between" alignItems="center" px={2} py={1} bgcolor="background.paper">
                    <Button
                      size="small"
                      startIcon={<HistoryIcon />}
                      onClick={() => handleOpenInteractions(c)}
                    >
                      {c.interactionsCount} Interactions
                    </Button>
                    <Box>
                      <IconButton size="small" onClick={() => openEdit(c)}>
                        <EditIcon fontSize="small" />
                      </IconButton>
                      <IconButton size="small" color="error" onClick={() => handleDelete(c.id)}>
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
        <TableContainer component={Paper} variant="outlined">
          <Table size="small">
            <TableHead>
              <TableRow>
                <TableCell width={40}></TableCell>
                <TableCell>Name</TableCell>
                <TableCell>Category</TableCell>
                <TableCell>Company & Title</TableCell>
                <TableCell>Contact Details</TableCell>
                <TableCell>Follow-Up</TableCell>
                <TableCell>Interactions</TableCell>
                <TableCell align="right">Actions</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {contacts.map((c) => {
                const catConfig = CATEGORY_CONFIG[c.category] || { label: c.categoryName, color: 'default' }
                return (
                  <TableRow key={c.id} hover>
                    <TableCell>
                      <IconButton size="small" onClick={() => handleToggleStar(c)} color={c.isStarred ? 'warning' : 'default'}>
                        {c.isStarred ? <StarIcon fontSize="small" /> : <StarBorderIcon fontSize="small" />}
                      </IconButton>
                    </TableCell>
                    <TableCell>
                      <Typography variant="body2" fontWeight={700}>{c.name}</Typography>
                      {c.location && <Typography variant="caption" color="text.secondary">{c.location}</Typography>}
                    </TableCell>
                    <TableCell>
                      <Chip label={catConfig.label} color={catConfig.color} size="small" />
                    </TableCell>
                    <TableCell>
                      <Typography variant="body2">{c.company || '—'}</Typography>
                      <Typography variant="caption" color="text.secondary">{c.jobTitle || ''}</Typography>
                    </TableCell>
                    <TableCell>
                      <Stack direction="row" spacing={1}>
                        {c.email && (
                          <Tooltip title={c.email}>
                            <IconButton size="small" component="a" href={`mailto:${c.email}`} color="primary">
                              <EmailIcon fontSize="small" />
                            </IconButton>
                          </Tooltip>
                        )}
                        {c.phone && (
                          <Tooltip title={c.phone}>
                            <IconButton size="small" component="a" href={`tel:${c.phone}`} color="info">
                              <PhoneIcon fontSize="small" />
                            </IconButton>
                          </Tooltip>
                        )}
                        {c.linkedIn && (
                          <Tooltip title="LinkedIn">
                            <IconButton size="small" component="a" href={c.linkedIn} target="_blank" rel="noopener noreferrer" color="primary">
                              <LinkedInIcon fontSize="small" />
                            </IconButton>
                          </Tooltip>
                        )}
                      </Stack>
                    </TableCell>
                    <TableCell>
                      {c.nextFollowUpDate ? (
                        <Typography variant="caption">
                          {new Date(c.nextFollowUpDate).toLocaleDateString()}
                        </Typography>
                      ) : '—'}
                    </TableCell>
                    <TableCell>
                      <Button size="small" startIcon={<HistoryIcon fontSize="small" />} onClick={() => handleOpenInteractions(c)}>
                        {c.interactionsCount} logs
                      </Button>
                    </TableCell>
                    <TableCell align="right">
                      <IconButton size="small" onClick={() => openEdit(c)}>
                        <EditIcon fontSize="small" />
                      </IconButton>
                      <IconButton size="small" color="error" onClick={() => handleDelete(c.id)}>
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

      {/* CREATE / EDIT CONTACT DIALOG */}
      <Dialog open={dialogOpen} onClose={() => setDialogOpen(false)} maxWidth="md" fullWidth>
        <form onSubmit={handleSaveContact}>
          <DialogTitle fontWeight={700}>
            {editContactId ? 'Edit CRM Contact' : 'Add New CRM Contact'}
          </DialogTitle>
          <DialogContent dividers>
            <Grid container spacing={2}>
              <Grid item xs={12} sm={6}>
                <TextField
                  label="Full Name"
                  required
                  value={form.name}
                  onChange={(e) => setForm({ ...form, name: e.target.value })}
                  size="small"
                  fullWidth
                />
              </Grid>

              <Grid item xs={12} sm={6}>
                <FormControl fullWidth size="small">
                  <InputLabel>CRM Role Category</InputLabel>
                  <Select
                    value={form.category}
                    label="CRM Role Category"
                    onChange={(e) => setForm({ ...form, category: Number(e.target.value) as ContactCategory })}
                  >
                    {Object.entries(CATEGORY_CONFIG).map(([val, info]) => (
                      <MenuItem key={val} value={Number(val)}>{info.label}</MenuItem>
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
                    onChange={(e) => {
                      const selectedComp = companies.find((c) => c.id === e.target.value)
                      setForm({
                        ...form,
                        companyId: e.target.value || undefined,
                        company: selectedComp ? selectedComp.name : form.company,
                      })
                    }}
                  >
                    <MenuItem value="">Other / Unaffiliated</MenuItem>
                    {companies.map((c) => (
                      <MenuItem key={c.id} value={c.id}>{c.name}</MenuItem>
                    ))}
                  </Select>
                </FormControl>
              </Grid>

              <Grid item xs={12} sm={6}>
                <TextField
                  label="Company Name (Free text / Override)"
                  value={form.company || ''}
                  onChange={(e) => setForm({ ...form, company: e.target.value })}
                  size="small"
                  fullWidth
                />
              </Grid>

              <Grid item xs={12} sm={6}>
                <TextField
                  label="Job Title"
                  placeholder="e.g. Senior Tech Recruiter, Director of Engineering"
                  value={form.jobTitle || ''}
                  onChange={(e) => setForm({ ...form, jobTitle: e.target.value })}
                  size="small"
                  fullWidth
                />
              </Grid>

              <Grid item xs={12} sm={6}>
                <TextField
                  label="Department / Org"
                  placeholder="e.g. Infrastructure, Core Platform, Talent"
                  value={form.department || ''}
                  onChange={(e) => setForm({ ...form, department: e.target.value })}
                  size="small"
                  fullWidth
                />
              </Grid>

              <Grid item xs={12} sm={6}>
                <TextField
                  label="Email Address"
                  type="email"
                  value={form.email || ''}
                  onChange={(e) => setForm({ ...form, email: e.target.value })}
                  size="small"
                  fullWidth
                />
              </Grid>

              <Grid item xs={12} sm={6}>
                <TextField
                  label="Phone Number"
                  value={form.phone || ''}
                  onChange={(e) => setForm({ ...form, phone: e.target.value })}
                  size="small"
                  fullWidth
                />
              </Grid>

              <Grid item xs={12} sm={6}>
                <TextField
                  label="LinkedIn Profile URL"
                  value={form.linkedIn || ''}
                  onChange={(e) => setForm({ ...form, linkedIn: e.target.value })}
                  size="small"
                  fullWidth
                />
              </Grid>

              <Grid item xs={12} sm={6}>
                <TextField
                  label="Location"
                  placeholder="e.g. San Francisco, CA / London / Remote"
                  value={form.location || ''}
                  onChange={(e) => setForm({ ...form, location: e.target.value })}
                  size="small"
                  fullWidth
                />
              </Grid>

              <Grid item xs={12} sm={6}>
                <FormControl fullWidth size="small">
                  <InputLabel>Associated Job Position</InputLabel>
                  <Select
                    value={form.jobId || ''}
                    label="Associated Job Position"
                    onChange={(e) => setForm({ ...form, jobId: e.target.value || undefined })}
                  >
                    <MenuItem value="">None</MenuItem>
                    {jobs.map((j) => (
                      <MenuItem key={j.id} value={j.id}>{j.title} @ {j.company}</MenuItem>
                    ))}
                  </Select>
                </FormControl>
              </Grid>

              <Grid item xs={12} sm={6}>
                <TextField
                  label="Tags (comma-separated)"
                  placeholder="e.g. Security, Tier-1, Ex-Meta, Warm Lead"
                  value={form.tags || ''}
                  onChange={(e) => setForm({ ...form, tags: e.target.value })}
                  size="small"
                  fullWidth
                />
              </Grid>

              <Grid item xs={12}>
                <TextField
                  label="Relationship & Context Notes"
                  placeholder="How you connected, rapport details, mutual interests, immigration assistance provided..."
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
              {saving ? 'Saving...' : editContactId ? 'Update Contact' : 'Save Contact'}
            </Button>
          </DialogActions>
        </form>
      </Dialog>

      {/* INTERACTION LOG & TIMELINE MODAL */}
      <Dialog open={interactionOpen} onClose={() => setInteractionOpen(false)} maxWidth="md" fullWidth>
        <DialogTitle fontWeight={700} display="flex" alignItems="center" gap={1}>
          <HistoryIcon color="primary" />
          Interaction History — {interactionContact?.name} ({interactionContact?.company || 'Independent'})
        </DialogTitle>
        <DialogContent dividers>
          {/* Interaction list */}
          <Box mb={3}>
            {interactionList.length === 0 ? (
              <Typography color="text.secondary">No interaction logs recorded yet.</Typography>
            ) : (
              <Stack spacing={2}>
                {interactionList.map((item) => (
                  <Paper key={item.id} variant="outlined" sx={{ p: 2, bgcolor: 'background.default' }}>
                    <Box display="flex" justifyContent="space-between" alignItems="center">
                      <Box display="flex" alignItems="center" gap={1}>
                        <Chip label={item.channel} size="small" color="primary" variant="outlined" />
                        <Typography variant="body1" fontWeight={700}>{item.summary}</Typography>
                      </Box>
                      <Box display="flex" alignItems="center" gap={1}>
                        <Typography variant="caption" color="text.secondary">
                          {new Date(item.interactionDate).toLocaleDateString()}
                        </Typography>
                        <IconButton size="small" color="error" onClick={() => handleDeleteInteraction(item.id)}>
                          <DeleteIcon fontSize="inherit" />
                        </IconButton>
                      </Box>
                    </Box>
                    {item.notes && (
                      <Typography variant="body2" color="text.secondary" sx={{ mt: 1, whiteSpace: 'pre-wrap' }}>
                        {item.notes}
                      </Typography>
                    )}
                    {item.followUpRequired && item.followUpDate && (
                      <Box mt={1}>
                        <Chip
                          icon={<AlarmIcon />}
                          label={`Follow-up requested by: ${new Date(item.followUpDate).toLocaleDateString()}`}
                          size="small"
                          color="warning"
                        />
                      </Box>
                    )}
                  </Paper>
                ))}
              </Stack>
            )}
          </Box>

          <Divider sx={{ my: 2 }} />

          {/* Log New Interaction Form */}
          <Typography variant="subtitle2" fontWeight={700} mb={1.5}>
            Log New Interaction / Communication
          </Typography>
          <form onSubmit={handleAddInteraction}>
            <Grid container spacing={2}>
              <Grid item xs={12} sm={4}>
                <FormControl fullWidth size="small">
                  <InputLabel>Channel</InputLabel>
                  <Select
                    value={newChannel}
                    label="Channel"
                    onChange={(e) => setNewChannel(e.target.value)}
                  >
                    <MenuItem value="Email">Email</MenuItem>
                    <MenuItem value="LinkedIn">LinkedIn</MenuItem>
                    <MenuItem value="Phone Call">Phone Call</MenuItem>
                    <MenuItem value="Coffee Chat">Coffee Chat</MenuItem>
                    <MenuItem value="Video Call">Video Call</MenuItem>
                    <MenuItem value="Interview">Interview</MenuItem>
                    <MenuItem value="In Person">In Person</MenuItem>
                  </Select>
                </FormControl>
              </Grid>
              <Grid item xs={12} sm={8}>
                <TextField
                  label="Interaction Summary"
                  placeholder="e.g. Intro coffee chat, discussed H-1B transfer sponsorship"
                  value={newSummary}
                  onChange={(e) => setNewSummary(e.target.value)}
                  size="small"
                  fullWidth
                  required
                />
              </Grid>
              <Grid item xs={12}>
                <TextField
                  label="Notes & Next Steps"
                  placeholder="Key takeaways, advice given, compensation expectations discussed..."
                  value={newNotes}
                  onChange={(e) => setNewNotes(e.target.value)}
                  multiline
                  rows={2}
                  size="small"
                  fullWidth
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <Button
                  variant={newFollowUpReq ? 'contained' : 'outlined'}
                  color="warning"
                  size="small"
                  startIcon={<AlarmIcon />}
                  onClick={() => setNewFollowUpReq(!newFollowUpReq)}
                >
                  {newFollowUpReq ? 'Follow-Up Needed: YES' : 'Require Follow-Up?'}
                </Button>
              </Grid>
              {newFollowUpReq && (
                <Grid item xs={12} sm={6}>
                  <TextField
                    label="Follow-Up Deadline Date"
                    type="date"
                    value={newFollowUpDate}
                    onChange={(e) => setNewFollowUpDate(e.target.value)}
                    size="small"
                    fullWidth
                    InputLabelProps={{ shrink: true }}
                  />
                </Grid>
              )}
              <Grid item xs={12} display="flex" justifyContent="flex-end">
                <Button type="submit" variant="contained" disabled={savingInteraction || !newSummary.trim()}>
                  {savingInteraction ? 'Logging...' : 'Log Interaction'}
                </Button>
              </Grid>
            </Grid>
          </form>
        </DialogContent>
        <DialogActions sx={{ px: 3, py: 1.5 }}>
          <Button onClick={() => setInteractionOpen(false)}>Close</Button>
        </DialogActions>
      </Dialog>
    </Box>
  )
}
