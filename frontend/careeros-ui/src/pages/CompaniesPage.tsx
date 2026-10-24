import { useEffect, useState } from 'react'
import {
  Box, Typography, Grid, Card, CardContent, IconButton,
  Dialog, DialogTitle, DialogContent, DialogActions, Button, TextField,
  FormControlLabel, Switch, Chip, CircularProgress, Alert, Tabs, Tab,
  MenuItem, Select, FormControl, InputLabel, Tooltip, Rating,
  Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Paper,
  InputAdornment, Avatar, ToggleButtonGroup, ToggleButton,
} from '@mui/material'
import AddIcon from '@mui/icons-material/Add'
import DeleteIcon from '@mui/icons-material/Delete'
import EditIcon from '@mui/icons-material/Edit'
import StarIcon from '@mui/icons-material/Star'
import StarBorderIcon from '@mui/icons-material/StarBorder'
import SearchIcon from '@mui/icons-material/Search'
import BusinessIcon from '@mui/icons-material/Business'
import LanguageIcon from '@mui/icons-material/Language'
import WorkOutlineIcon from '@mui/icons-material/WorkOutline'
import ArticleIcon from '@mui/icons-material/Article'
import LaunchIcon from '@mui/icons-material/Launch'
import LocationOnIcon from '@mui/icons-material/LocationOn'
import ViewModuleIcon from '@mui/icons-material/ViewModule'
import ViewListIcon from '@mui/icons-material/ViewList'
import VerifiedUserIcon from '@mui/icons-material/VerifiedUser'
import FlightTakeoffIcon from '@mui/icons-material/FlightTakeoff'
import ThumbUpIcon from '@mui/icons-material/ThumbUp'
import ThumbDownIcon from '@mui/icons-material/ThumbDown'

import {
  companiesApi,
  SponsorshipStatus,
  RelocationStatus,
  type Company,
  type CompanyStats,
  type CreateCompanyRequest,
} from '../api/companies'

const emptyCompanyForm: CreateCompanyRequest = {
  name: '',
  description: '',
  headquarters: '',
  size: '201-1000 employees',
  stage: 'Series B+',
  foundedYear: undefined,
  logoUrl: '',
  websiteUrl: '',
  careersUrl: '',
  techBlogUrl: '',
  linkedInUrl: '',
  glassdoorUrl: '',
  glassdoorRating: undefined,
  personalRating: 3,
  cultureRating: 3,
  workLifeBalanceRating: 3,
  sponsorshipStatus: SponsorshipStatus.Unknown,
  sponsoredVisas: '',
  sponsorshipNotes: '',
  relocationStatus: RelocationStatus.Unknown,
  relocationNotes: '',
  industry: 'Technology / Software',
  sector: 'Enterprise SaaS',
  tags: '',
  notes: '',
  pros: '',
  cons: '',
  isTarget: false,
}

export default function CompaniesPage() {
  const [companies, setCompanies] = useState<Company[]>([])
  const [stats, setStats] = useState<CompanyStats | null>(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [success, setSuccess] = useState<string | null>(null)

  // Filtering states
  const [searchTerm, setSearchTerm] = useState('')
  const [selectedIndustry, setSelectedIndustry] = useState<string>('all')
  const [selectedSponsorship, setSelectedSponsorship] = useState<string>('all')
  const [minRatingFilter, setMinRatingFilter] = useState<number>(0)
  const [targetsOnly, setTargetsOnly] = useState<boolean>(false)
  const [viewMode, setViewMode] = useState<'grid' | 'table'>('grid')

  // Dialog states
  const [dialogOpen, setDialogOpen] = useState(false)
  const [dialogTab, setDialogTab] = useState(0)
  const [editCompanyId, setEditCompanyId] = useState<string | null>(null)
  const [form, setForm] = useState<CreateCompanyRequest>(emptyCompanyForm)
  const [saving, setSaving] = useState(false)

  const loadData = async () => {
    setLoading(true)
    try {
      const spVal =
        selectedSponsorship === 'yes' ? SponsorshipStatus.Yes :
        selectedSponsorship === 'case' ? SponsorshipStatus.CaseByCase :
        selectedSponsorship === 'no' ? SponsorshipStatus.No : undefined

      const [data, statsData] = await Promise.all([
        companiesApi.getAll({
          searchTerm: searchTerm || undefined,
          industry: selectedIndustry !== 'all' ? selectedIndustry : undefined,
          sponsorship: spVal,
          minRating: minRatingFilter > 0 ? minRatingFilter : undefined,
          isTarget: targetsOnly ? true : undefined,
        }),
        companiesApi.getStats().catch(() => null),
      ])
      setCompanies(data)
      setStats(statsData)
    } catch {
      setError('Failed to load companies')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    loadData()
  }, [searchTerm, selectedIndustry, selectedSponsorship, minRatingFilter, targetsOnly])

  const openCreateDialog = () => {
    setEditCompanyId(null)
    setForm(emptyCompanyForm)
    setDialogTab(0)
    setDialogOpen(true)
  }

  const openEditDialog = (c: Company) => {
    setEditCompanyId(c.id)
    setForm({
      name: c.name,
      description: c.description ?? '',
      headquarters: c.headquarters ?? '',
      size: c.size ?? '201-1000 employees',
      stage: c.stage ?? 'Series B+',
      foundedYear: c.foundedYear,
      logoUrl: c.logoUrl ?? '',
      websiteUrl: c.websiteUrl ?? '',
      careersUrl: c.careersUrl ?? '',
      techBlogUrl: c.techBlogUrl ?? '',
      linkedInUrl: c.linkedInUrl ?? '',
      glassdoorUrl: c.glassdoorUrl ?? '',
      glassdoorRating: c.glassdoorRating,
      personalRating: c.personalRating,
      cultureRating: c.cultureRating,
      workLifeBalanceRating: c.workLifeBalanceRating,
      sponsorshipStatus: c.sponsorshipStatus,
      sponsoredVisas: c.sponsoredVisas ?? '',
      sponsorshipNotes: c.sponsorshipNotes ?? '',
      relocationStatus: c.relocationStatus,
      relocationNotes: c.relocationNotes ?? '',
      industry: c.industry ?? '',
      sector: c.sector ?? '',
      tags: c.tags ?? '',
      notes: c.notes ?? '',
      pros: c.pros ?? '',
      cons: c.cons ?? '',
      isTarget: c.isTarget,
    })
    setDialogTab(0)
    setDialogOpen(true)
  }

  const handleSaveCompany = async () => {
    setSaving(true)
    try {
      if (editCompanyId) {
        await companiesApi.update(editCompanyId, form)
        setSuccess(`Updated ${form.name}`)
      } else {
        await companiesApi.create(form)
        setSuccess(`Added ${form.name} to Company CRM`)
      }
      setDialogOpen(false)
      loadData()
    } catch {
      setError('Failed to save company')
    } finally {
      setSaving(false)
    }
  }

  const handleDeleteCompany = async (id: string, name: string) => {
    if (!confirm(`Are you sure you want to delete ${name}?`)) return
    try {
      await companiesApi.delete(id)
      setSuccess(`Deleted ${name}`)
      loadData()
    } catch {
      setError('Failed to delete company')
    }
  }

  const handleToggleTarget = async (c: Company) => {
    try {
      await companiesApi.update(c.id, {
        ...c,
        isTarget: !c.isTarget,
      })
      setSuccess(`${c.name} ${!c.isTarget ? 'marked as dream target' : 'unmarked from targets'}`)
      loadData()
    } catch {
      setError('Failed to update target status')
    }
  }

  return (
    <Box>
      {/* Header */}
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={2}>
        <Box>
          <Typography variant="h4" fontWeight={700}>Company CRM</Typography>
          <Typography variant="body2" color="text.secondary">
            Manage target employers, visa sponsorship track records, relocation policies, ratings, and company intelligence.
          </Typography>
        </Box>
        <Button variant="contained" startIcon={<AddIcon />} onClick={openCreateDialog}>
          Add Company
        </Button>
      </Box>

      {error && <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError(null)}>{error}</Alert>}
      {success && <Alert severity="success" sx={{ mb: 2 }} onClose={() => setSuccess(null)}>{success}</Alert>}

      {/* KPI Stats Banner */}
      {stats && (
        <Grid container spacing={2} mb={3}>
          <Grid item xs={6} sm={3}>
            <Card variant="outlined">
              <CardContent sx={{ py: 1.5, '&:last-child': { pb: 1.5 } }}>
                <Typography variant="caption" color="text.secondary">Total Tracked</Typography>
                <Typography variant="h5" fontWeight={700} color="primary.main">{stats.totalCompanies}</Typography>
              </CardContent>
            </Card>
          </Grid>
          <Grid item xs={6} sm={3}>
            <Card variant="outlined">
              <CardContent sx={{ py: 1.5, '&:last-child': { pb: 1.5 } }}>
                <Typography variant="caption" color="text.secondary">Dream Targets</Typography>
                <Typography variant="h5" fontWeight={700} color="warning.main">{stats.targetCompanies}</Typography>
              </CardContent>
            </Card>
          </Grid>
          <Grid item xs={6} sm={3}>
            <Card variant="outlined">
              <CardContent sx={{ py: 1.5, '&:last-child': { pb: 1.5 } }}>
                <Typography variant="caption" color="text.secondary">Visa Sponsors</Typography>
                <Typography variant="h5" fontWeight={700} color="success.main">{stats.sponsoringCompanies}</Typography>
              </CardContent>
            </Card>
          </Grid>
          <Grid item xs={6} sm={3}>
            <Card variant="outlined">
              <CardContent sx={{ py: 1.5, '&:last-child': { pb: 1.5 } }}>
                <Typography variant="caption" color="text.secondary">Relocation Assistance</Typography>
                <Typography variant="h5" fontWeight={700} color="secondary.main">{stats.relocatingCompanies}</Typography>
              </CardContent>
            </Card>
          </Grid>
        </Grid>
      )}

      {/* Search & Filter Toolbar */}
      <Paper variant="outlined" sx={{ p: 2, mb: 3 }}>
        <Grid container spacing={2} alignItems="center">
          <Grid item xs={12} sm={4}>
            <TextField
              placeholder="Search companies, location, tags..."
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
              <InputLabel>Industry</InputLabel>
              <Select
                value={selectedIndustry}
                label="Industry"
                onChange={(e) => setSelectedIndustry(e.target.value)}
              >
                <MenuItem value="all">All Industries</MenuItem>
                {stats?.topIndustries.map((ind) => (
                  <MenuItem key={ind} value={ind}>{ind}</MenuItem>
                ))}
              </Select>
            </FormControl>
          </Grid>
          <Grid item xs={6} sm={2}>
            <FormControl size="small" fullWidth>
              <InputLabel>Visa Sponsorship</InputLabel>
              <Select
                value={selectedSponsorship}
                label="Visa Sponsorship"
                onChange={(e) => setSelectedSponsorship(e.target.value)}
              >
                <MenuItem value="all">All Sponsorship</MenuItem>
                <MenuItem value="yes">Sponsors Visa (Yes)</MenuItem>
                <MenuItem value="case">Case-by-Case</MenuItem>
                <MenuItem value="no">No Sponsorship</MenuItem>
              </Select>
            </FormControl>
          </Grid>
          <Grid item xs={6} sm={2}>
            <FormControl size="small" fullWidth>
              <InputLabel>Min Rating</InputLabel>
              <Select
                value={minRatingFilter}
                label="Min Rating"
                onChange={(e) => setMinRatingFilter(Number(e.target.value))}
              >
                <MenuItem value={0}>Any Rating</MenuItem>
                <MenuItem value={3}>⭐⭐⭐ 3+ Stars</MenuItem>
                <MenuItem value={4}>⭐⭐⭐⭐ 4+ Stars</MenuItem>
                <MenuItem value={5}>⭐⭐⭐⭐⭐ 5 Stars</MenuItem>
              </Select>
            </FormControl>
          </Grid>
          <Grid item xs={6} sm={2} display="flex" justifyContent="flex-end" alignItems="center" gap={1}>
            <Tooltip title="Show Dream Target companies only">
              <Button
                variant={targetsOnly ? 'contained' : 'outlined'}
                color="warning"
                size="small"
                startIcon={targetsOnly ? <StarIcon /> : <StarBorderIcon />}
                onClick={() => setTargetsOnly(!targetsOnly)}
              >
                Targets
              </Button>
            </Tooltip>
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

      {/* Content */}
      {loading ? (
        <Box display="flex" justifyContent="center" mt={8}><CircularProgress /></Box>
      ) : companies.length === 0 ? (
        <Box textAlign="center" py={8} color="text.secondary">
          <BusinessIcon sx={{ fontSize: 64, opacity: 0.3 }} />
          <Typography mt={2}>No companies found.</Typography>
          <Typography variant="body2" mt={0.5}>Try adjusting your search filters or click "Add Company" to add a new employer.</Typography>
          <Button variant="outlined" startIcon={<AddIcon />} sx={{ mt: 2 }} onClick={openCreateDialog}>
            Add First Company
          </Button>
        </Box>
      ) : viewMode === 'grid' ? (
        /* GRID VIEW */
        <Grid container spacing={3}>
          {companies.map((c) => (
            <Grid item xs={12} sm={6} md={4} key={c.id}>
              <Card variant="outlined" sx={{ height: '100%', display: 'flex', flexDirection: 'column' }}>
                <CardContent sx={{ flexGrow: 1 }}>
                  {/* Card Header */}
                  <Box display="flex" justifyContent="space-between" alignItems="flex-start">
                    <Box display="flex" alignItems="center" gap={1.5}>
                      <Avatar sx={{ bgcolor: c.isTarget ? 'warning.main' : 'primary.main', width: 42, height: 42, fontWeight: 700 }}>
                        {c.name.charAt(0).toUpperCase()}
                      </Avatar>
                      <Box>
                        <Typography variant="h6" fontWeight={700} lineHeight={1.2}>{c.name}</Typography>
                        {c.industry && (
                          <Typography variant="caption" color="text.secondary">{c.industry}</Typography>
                        )}
                      </Box>
                    </Box>
                    <Box display="flex" alignItems="center">
                      <Tooltip title={c.isTarget ? 'Dream Target company' : 'Mark as target'}>
                        <IconButton size="small" onClick={() => handleToggleTarget(c)} color={c.isTarget ? 'warning' : 'default'}>
                          {c.isTarget ? <StarIcon /> : <StarBorderIcon />}
                        </IconButton>
                      </Tooltip>
                      <IconButton size="small" onClick={() => openEditDialog(c)}>
                        <EditIcon fontSize="small" />
                      </IconButton>
                      <IconButton size="small" color="error" onClick={() => handleDeleteCompany(c.id, c.name)}>
                        <DeleteIcon fontSize="small" />
                      </IconButton>
                    </Box>
                  </Box>

                  {/* Metadata tags */}
                  <Box display="flex" flexWrap="wrap" gap={0.5} mt={1.5}>
                    {c.headquarters && (
                      <Chip size="small" icon={<LocationOnIcon fontSize="small" />} label={c.headquarters} variant="outlined" />
                    )}
                    {c.size && <Chip size="small" label={c.size} variant="outlined" />}
                    {c.stage && <Chip size="small" label={c.stage} variant="outlined" />}
                  </Box>

                  {/* Sponsorship & Relocation status */}
                  <Box display="flex" flexWrap="wrap" gap={0.5} mt={1}>
                    <Chip
                      size="small"
                      icon={<VerifiedUserIcon />}
                      label={
                        c.sponsorshipStatus === SponsorshipStatus.Yes ? (c.sponsoredVisas ? `Sponsors: ${c.sponsoredVisas}` : 'Visa Sponsor') :
                        c.sponsorshipStatus === SponsorshipStatus.CaseByCase ? 'Sponsorship: Case-by-Case' :
                        c.sponsorshipStatus === SponsorshipStatus.No ? 'No Sponsorship' : 'Sponsorship: Unknown'
                      }
                      color={
                        c.sponsorshipStatus === SponsorshipStatus.Yes ? 'success' :
                        c.sponsorshipStatus === SponsorshipStatus.CaseByCase ? 'warning' : 'default'
                      }
                    />
                    {c.relocationStatus !== RelocationStatus.Unknown && (
                      <Chip
                        size="small"
                        icon={<FlightTakeoffIcon />}
                        label={`Relocation: ${c.relocationStatusName}`}
                        color={c.relocationStatus !== RelocationStatus.No ? 'info' : 'default'}
                        variant="outlined"
                      />
                    )}
                  </Box>

                  {/* Ratings */}
                  <Box display="flex" alignItems="center" justifyContent="space-between" mt={1.5} pt={1} borderTop={1} borderColor="divider">
                    <Box display="flex" alignItems="center" gap={0.5}>
                      <Typography variant="caption" color="text.secondary">My Rating:</Typography>
                      <Rating size="small" value={c.personalRating} readOnly />
                    </Box>
                    {c.glassdoorRating && (
                      <Chip size="small" variant="outlined" label={`GD: ${c.glassdoorRating}★`} />
                    )}
                  </Box>

                  {/* Description / Summary */}
                  {c.description && (
                    <Typography variant="body2" color="text.secondary" mt={1} sx={{ overflow: 'hidden', textOverflow: 'ellipsis', display: '-webkit-box', WebkitLineClamp: 2, WebkitBoxOrient: 'vertical' }}>
                      {c.description}
                    </Typography>
                  )}

                  {/* Pros / Cons Highlights */}
                  {(c.pros || c.cons) && (
                    <Box mt={1} display="flex" flexDirection="column" gap={0.5}>
                      {c.pros && (
                        <Box display="flex" alignItems="center" gap={0.5} color="success.main">
                          <ThumbUpIcon sx={{ fontSize: 13 }} />
                          <Typography variant="caption" noWrap>{c.pros}</Typography>
                        </Box>
                      )}
                      {c.cons && (
                        <Box display="flex" alignItems="center" gap={0.5} color="error.main">
                          <ThumbDownIcon sx={{ fontSize: 13 }} />
                          <Typography variant="caption" noWrap>{c.cons}</Typography>
                        </Box>
                      )}
                    </Box>
                  )}

                  {/* Quick Links */}
                  <Box display="flex" gap={1} mt={2} pt={1} borderTop={1} borderColor="divider" alignItems="center">
                    {c.websiteUrl && (
                      <Tooltip title="Main Website">
                        <IconButton size="small" component="a" href={c.websiteUrl} target="_blank" rel="noopener noreferrer">
                          <LanguageIcon fontSize="small" />
                        </IconButton>
                      </Tooltip>
                    )}
                    {c.careersUrl && (
                      <Tooltip title="Careers Page">
                        <IconButton size="small" component="a" href={c.careersUrl} target="_blank" rel="noopener noreferrer" color="primary">
                          <WorkOutlineIcon fontSize="small" />
                        </IconButton>
                      </Tooltip>
                    )}
                    {c.techBlogUrl && (
                      <Tooltip title="Tech Blog">
                        <IconButton size="small" component="a" href={c.techBlogUrl} target="_blank" rel="noopener noreferrer">
                          <ArticleIcon fontSize="small" />
                        </IconButton>
                      </Tooltip>
                    )}
                    {c.linkedInUrl && (
                      <Tooltip title="LinkedIn Page">
                        <IconButton size="small" component="a" href={c.linkedInUrl} target="_blank" rel="noopener noreferrer">
                          <LaunchIcon fontSize="small" />
                        </IconButton>
                      </Tooltip>
                    )}
                    {c.glassdoorUrl && (
                      <Tooltip title="Glassdoor Reviews">
                        <IconButton size="small" component="a" href={c.glassdoorUrl} target="_blank" rel="noopener noreferrer">
                          <StarIcon fontSize="small" />
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
                <TableCell><strong>Company</strong></TableCell>
                <TableCell><strong>Industry</strong></TableCell>
                <TableCell><strong>Headquarters</strong></TableCell>
                <TableCell><strong>Size & Stage</strong></TableCell>
                <TableCell><strong>Visa Sponsorship</strong></TableCell>
                <TableCell><strong>Rating</strong></TableCell>
                <TableCell align="center"><strong>Target</strong></TableCell>
                <TableCell align="right"><strong>Actions</strong></TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {companies.map((c) => (
                <TableRow key={c.id} hover>
                  <TableCell>
                    <Typography fontWeight={700}>{c.name}</Typography>
                    {c.websiteUrl && (
                      <Typography variant="caption" color="primary">
                        <a href={c.websiteUrl} target="_blank" rel="noopener noreferrer">{c.websiteUrl}</a>
                      </Typography>
                    )}
                  </TableCell>
                  <TableCell>{c.industry ?? '—'}</TableCell>
                  <TableCell>{c.headquarters ?? '—'}</TableCell>
                  <TableCell>{c.size ?? ''} {c.stage ? `(${c.stage})` : ''}</TableCell>
                  <TableCell>
                    <Chip
                      size="small"
                      label={
                        c.sponsorshipStatus === SponsorshipStatus.Yes ? (c.sponsoredVisas ? `Yes: ${c.sponsoredVisas}` : 'Yes') :
                        c.sponsorshipStatus === SponsorshipStatus.CaseByCase ? 'Case-by-Case' :
                        c.sponsorshipStatus === SponsorshipStatus.No ? 'No' : 'Unknown'
                      }
                      color={
                        c.sponsorshipStatus === SponsorshipStatus.Yes ? 'success' :
                        c.sponsorshipStatus === SponsorshipStatus.CaseByCase ? 'warning' : 'default'
                      }
                    />
                  </TableCell>
                  <TableCell>
                    <Rating size="small" value={c.personalRating} readOnly />
                  </TableCell>
                  <TableCell align="center">
                    <IconButton size="small" onClick={() => handleToggleTarget(c)} color={c.isTarget ? 'warning' : 'default'}>
                      {c.isTarget ? <StarIcon /> : <StarBorderIcon />}
                    </IconButton>
                  </TableCell>
                  <TableCell align="right">
                    <IconButton size="small" onClick={() => openEditDialog(c)}>
                      <EditIcon fontSize="small" />
                    </IconButton>
                    <IconButton size="small" color="error" onClick={() => handleDeleteCompany(c.id, c.name)}>
                      <DeleteIcon fontSize="small" />
                    </IconButton>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      )}

      {/* ADD / EDIT COMPANY MODAL DIALOG */}
      <Dialog open={dialogOpen} onClose={() => setDialogOpen(false)} maxWidth="md" fullWidth>
        <DialogTitle>{editCompanyId ? `Edit ${form.name}` : 'Add Company to CRM'}</DialogTitle>
        <Box sx={{ borderBottom: 1, borderColor: 'divider', px: 3 }}>
          <Tabs value={dialogTab} onChange={(_, v) => setDialogTab(v)}>
            <Tab label="Overview" />
            <Tab label="Web & Links" />
            <Tab label="Sponsorship & Relocation" />
            <Tab label="Ratings & Notes" />
          </Tabs>
        </Box>
        <DialogContent sx={{ pt: 2 }}>
          {/* TAB 0: Overview */}
          {dialogTab === 0 && (
            <Box display="flex" flexDirection="column" gap={2}>
              <Grid container spacing={2}>
                <Grid item xs={8}>
                  <TextField
                    label="Company Name"
                    placeholder="e.g. Stripe, Google, Datadog"
                    value={form.name}
                    onChange={(e) => setForm({ ...form, name: e.target.value })}
                    fullWidth
                    required
                  />
                </Grid>
                <Grid item xs={4}>
                  <TextField
                    label="Founded Year"
                    placeholder="e.g. 2010"
                    type="number"
                    value={form.foundedYear ?? ''}
                    onChange={(e) => setForm({ ...form, foundedYear: e.target.value ? Number(e.target.value) : undefined })}
                    fullWidth
                  />
                </Grid>
              </Grid>
              <Grid container spacing={2}>
                <Grid item xs={6}>
                  <TextField
                    label="Industry"
                    placeholder="e.g. Financial Services, Cloud Infrastructure"
                    value={form.industry ?? ''}
                    onChange={(e) => setForm({ ...form, industry: e.target.value })}
                    fullWidth
                  />
                </Grid>
                <Grid item xs={6}>
                  <TextField
                    label="Sector / Category"
                    placeholder="e.g. Developer Tools, Payments"
                    value={form.sector ?? ''}
                    onChange={(e) => setForm({ ...form, sector: e.target.value })}
                    fullWidth
                  />
                </Grid>
              </Grid>
              <Grid container spacing={2}>
                <Grid item xs={6}>
                  <TextField
                    label="Headquarters / Location"
                    placeholder="e.g. San Francisco, CA / Remote"
                    value={form.headquarters ?? ''}
                    onChange={(e) => setForm({ ...form, headquarters: e.target.value })}
                    fullWidth
                  />
                </Grid>
                <Grid item xs={3}>
                  <FormControl fullWidth>
                    <InputLabel>Company Size</InputLabel>
                    <Select
                      value={form.size ?? '201-1000 employees'}
                      label="Company Size"
                      onChange={(e) => setForm({ ...form, size: e.target.value })}
                    >
                      <MenuItem value="1-50 employees">1-50 employees</MenuItem>
                      <MenuItem value="51-200 employees">51-200 employees</MenuItem>
                      <MenuItem value="201-1000 employees">201-1000 employees</MenuItem>
                      <MenuItem value="1001-5000 employees">1001-5000 employees</MenuItem>
                      <MenuItem value="5000+ employees">5000+ employees</MenuItem>
                    </Select>
                  </FormControl>
                </Grid>
                <Grid item xs={3}>
                  <FormControl fullWidth>
                    <InputLabel>Stage</InputLabel>
                    <Select
                      value={form.stage ?? 'Series B+'}
                      label="Stage"
                      onChange={(e) => setForm({ ...form, stage: e.target.value })}
                    >
                      <MenuItem value="Seed / Pre-Seed">Seed / Pre-Seed</MenuItem>
                      <MenuItem value="Series A">Series A</MenuItem>
                      <MenuItem value="Series B+">Series B+</MenuItem>
                      <MenuItem value="Growth / Late Stage">Growth / Late Stage</MenuItem>
                      <MenuItem value="Public">Public (IPO)</MenuItem>
                      <MenuItem value="Bootstrapped">Bootstrapped</MenuItem>
                    </Select>
                  </FormControl>
                </Grid>
              </Grid>
              <TextField
                label="Company Description / Overview"
                placeholder="What does the company do, flagship products, mission..."
                value={form.description ?? ''}
                onChange={(e) => setForm({ ...form, description: e.target.value })}
                multiline
                rows={3}
                fullWidth
              />
              <TextField
                label="Tags (Comma-separated)"
                placeholder="e.g. Remote-Friendly, High-Scale, High-Compensation, Tech-First"
                value={form.tags ?? ''}
                onChange={(e) => setForm({ ...form, tags: e.target.value })}
                fullWidth
              />
            </Box>
          )}

          {/* TAB 1: Web & Links */}
          {dialogTab === 1 && (
            <Box display="flex" flexDirection="column" gap={2}>
              <TextField
                label="Official Website URL"
                placeholder="https://stripe.com"
                value={form.websiteUrl ?? ''}
                onChange={(e) => setForm({ ...form, websiteUrl: e.target.value })}
                fullWidth
              />
              <TextField
                label="Careers Page URL"
                placeholder="https://stripe.com/jobs"
                value={form.careersUrl ?? ''}
                onChange={(e) => setForm({ ...form, careersUrl: e.target.value })}
                fullWidth
              />
              <TextField
                label="Engineering / Tech Blog URL"
                placeholder="https://stripe.com/blog/engineering"
                value={form.techBlogUrl ?? ''}
                onChange={(e) => setForm({ ...form, techBlogUrl: e.target.value })}
                fullWidth
              />
              <TextField
                label="LinkedIn Company Page URL"
                placeholder="https://linkedin.com/company/stripe"
                value={form.linkedInUrl ?? ''}
                onChange={(e) => setForm({ ...form, linkedInUrl: e.target.value })}
                fullWidth
              />
              <Grid container spacing={2}>
                <Grid item xs={8}>
                  <TextField
                    label="Glassdoor Page URL"
                    placeholder="https://glassdoor.com/Overview/Working-at-Stripe..."
                    value={form.glassdoorUrl ?? ''}
                    onChange={(e) => setForm({ ...form, glassdoorUrl: e.target.value })}
                    fullWidth
                  />
                </Grid>
                <Grid item xs={4}>
                  <TextField
                    label="Glassdoor Rating (1.0 - 5.0)"
                    placeholder="e.g. 4.4"
                    type="number"
                    inputProps={{ step: '0.1', min: '1.0', max: '5.0' }}
                    value={form.glassdoorRating ?? ''}
                    onChange={(e) => setForm({ ...form, glassdoorRating: e.target.value ? Number(e.target.value) : undefined })}
                    fullWidth
                  />
                </Grid>
              </Grid>
            </Box>
          )}

          {/* TAB 2: Sponsorship & Relocation */}
          {dialogTab === 2 && (
            <Box display="flex" flexDirection="column" gap={2}>
              <Alert severity="info">
                Track visa sponsorship and relocation support policies for this employer.
              </Alert>
              <Grid container spacing={2}>
                <Grid item xs={6}>
                  <FormControl fullWidth>
                    <InputLabel>Visa Sponsorship Policy</InputLabel>
                    <Select
                      value={form.sponsorshipStatus}
                      label="Visa Sponsorship Policy"
                      onChange={(e) => setForm({ ...form, sponsorshipStatus: Number(e.target.value) as SponsorshipStatus })}
                    >
                      <MenuItem value={SponsorshipStatus.Unknown}>Unknown / Researching</MenuItem>
                      <MenuItem value={SponsorshipStatus.Yes}>Yes — Actively Sponsors Visas</MenuItem>
                      <MenuItem value={SponsorshipStatus.CaseByCase}>Case-by-Case / Tier Dependent</MenuItem>
                      <MenuItem value={SponsorshipStatus.No}>No — Requires Work Authorization</MenuItem>
                    </Select>
                  </FormControl>
                </Grid>
                <Grid item xs={6}>
                  <TextField
                    label="Sponsored Visas Supported"
                    placeholder="e.g. H-1B, O-1, TN, E-3, Green Card Day 1"
                    value={form.sponsoredVisas ?? ''}
                    onChange={(e) => setForm({ ...form, sponsoredVisas: e.target.value })}
                    fullWidth
                  />
                </Grid>
              </Grid>
              <TextField
                label="Sponsorship Notes & Track Record"
                placeholder="e.g. Known to sponsor H-1B transfers immediately, requires Senior+ level for new lottery filings..."
                value={form.sponsorshipNotes ?? ''}
                onChange={(e) => setForm({ ...form, sponsorshipNotes: e.target.value })}
                multiline
                rows={3}
                fullWidth
              />
              <Grid container spacing={2}>
                <Grid item xs={6}>
                  <FormControl fullWidth>
                    <InputLabel>Relocation Assistance</InputLabel>
                    <Select
                      value={form.relocationStatus}
                      label="Relocation Assistance"
                      onChange={(e) => setForm({ ...form, relocationStatus: Number(e.target.value) as RelocationStatus })}
                    >
                      <MenuItem value={RelocationStatus.Unknown}>Unknown</MenuItem>
                      <MenuItem value={RelocationStatus.Yes}>Yes — Full Relocation Package</MenuItem>
                      <MenuItem value={RelocationStatus.DomesticOnly}>Domestic Only</MenuItem>
                      <MenuItem value={RelocationStatus.International}>International Supported</MenuItem>
                      <MenuItem value={RelocationStatus.No}>No Relocation Provided</MenuItem>
                    </Select>
                  </FormControl>
                </Grid>
                <Grid item xs={6}>
                  <TextField
                    label="Relocation Package Details"
                    placeholder="e.g. $10k lump sum + temporary corporate housing"
                    value={form.relocationNotes ?? ''}
                    onChange={(e) => setForm({ ...form, relocationNotes: e.target.value })}
                    fullWidth
                  />
                </Grid>
              </Grid>
            </Box>
          )}

          {/* TAB 3: Ratings & Notes */}
          {dialogTab === 3 && (
            <Box display="flex" flexDirection="column" gap={2}>
              <FormControlLabel
                control={
                  <Switch
                    checked={form.isTarget}
                    onChange={(e) => setForm({ ...form, isTarget: e.target.checked })}
                    color="warning"
                  />
                }
                label={
                  <Box display="flex" alignItems="center" gap={0.5}>
                    <StarIcon sx={{ color: 'warning.main', fontSize: 20 }} />
                    <Typography fontWeight={600}>Mark as Dream / Top Target Company</Typography>
                  </Box>
                }
              />
              <Grid container spacing={2}>
                <Grid item xs={4}>
                  <Typography variant="caption" color="text.secondary" display="block">Personal Excitement / Rating</Typography>
                  <Rating
                    value={form.personalRating}
                    onChange={(_, v) => setForm({ ...form, personalRating: v ?? 3 })}
                  />
                </Grid>
                <Grid item xs={4}>
                  <Typography variant="caption" color="text.secondary" display="block">Engineering Culture</Typography>
                  <Rating
                    value={form.cultureRating}
                    onChange={(_, v) => setForm({ ...form, cultureRating: v ?? 3 })}
                  />
                </Grid>
                <Grid item xs={4}>
                  <Typography variant="caption" color="text.secondary" display="block">Work-Life Balance</Typography>
                  <Rating
                    value={form.workLifeBalanceRating}
                    onChange={(_, v) => setForm({ ...form, workLifeBalanceRating: v ?? 3 })}
                  />
                </Grid>
              </Grid>
              <Grid container spacing={2}>
                <Grid item xs={6}>
                  <TextField
                    label="Pros / Green Flags"
                    placeholder="Great tech stack, strong engineering brand, fair on-call..."
                    value={form.pros ?? ''}
                    onChange={(e) => setForm({ ...form, pros: e.target.value })}
                    multiline
                    rows={2}
                    fullWidth
                  />
                </Grid>
                <Grid item xs={6}>
                  <TextField
                    label="Cons / Red Flags"
                    placeholder="Rigid interview loop, potential re-orgs, slow compensation growth..."
                    value={form.cons ?? ''}
                    onChange={(e) => setForm({ ...form, cons: e.target.value })}
                    multiline
                    rows={2}
                    fullWidth
                  />
                </Grid>
              </Grid>
              <TextField
                label="Personal Assessment & Research Notes"
                placeholder="Insights from conversations with employees, glassdoor research, recruiter interaction..."
                value={form.notes ?? ''}
                onChange={(e) => setForm({ ...form, notes: e.target.value })}
                multiline
                rows={3}
                fullWidth
              />
            </Box>
          )}
        </DialogContent>
        <DialogActions sx={{ px: 3, pb: 2 }}>
          <Button onClick={() => setDialogOpen(false)}>Cancel</Button>
          <Button variant="contained" onClick={handleSaveCompany} disabled={!form.name || saving}>
            {saving ? 'Saving...' : 'Save Company'}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  )
}

