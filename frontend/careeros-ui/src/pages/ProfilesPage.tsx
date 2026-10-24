import { useState, useEffect } from 'react'
import {
  Box,
  Typography,
  Grid,
  Card,
  CardContent,
  Button,
  Chip,
  Avatar,
  Tab,
  Tabs,
  TextField,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Divider,
  LinearProgress,
  IconButton,
  Paper,
} from '@mui/material'
import PeopleIcon from '@mui/icons-material/People'
import AddIcon from '@mui/icons-material/Add'
import CheckCircleIcon from '@mui/icons-material/CheckCircle'
import AssignmentIcon from '@mui/icons-material/Assignment'
import FolderIcon from '@mui/icons-material/Folder'
import CalendarMonthIcon from '@mui/icons-material/CalendarMonth'
import EmojiEventsIcon from '@mui/icons-material/EmojiEvents'
import EditIcon from '@mui/icons-material/Edit'
import LocationOnIcon from '@mui/icons-material/LocationOn'
import AttachMoneyIcon from '@mui/icons-material/AttachMoney'
import WorkOutlineIcon from '@mui/icons-material/WorkOutline'
import { useProfile } from '../context/ProfileContext'
import {
  type CareerProfileDto,
  type CreateCareerProfileDto,
  type FamilyDashboardDto,
  profilesApi,
} from '../api/profiles'

export default function ProfilesPage() {
  const { activeProfile, profiles, switchProfile, refreshProfiles } = useProfile()
  const [tabIndex, setTabIndex] = useState<number>(0)
  const [familyData, setFamilyData] = useState<FamilyDashboardDto | null>(null)
  const [loading, setLoading] = useState<boolean>(true)
  const [dialogOpen, setDialogOpen] = useState<boolean>(false)
  const [editingProfile, setEditingProfile] = useState<CareerProfileDto | null>(null)

  // Form state
  const [formName, setFormName] = useState('')
  const [formRelationship, setFormRelationship] = useState('Self')
  const [formEmail, setFormEmail] = useState('')
  const [formPhone, setFormPhone] = useState('')
  const [formHeadline, setFormHeadline] = useState('')
  const [formBio, setFormBio] = useState('')
  const [formColor, setFormColor] = useState('#2563eb')
  const [formTargetRoles, setFormTargetRoles] = useState('')
  const [formTargetLocations, setFormTargetLocations] = useState('')
  const [formMinSalary, setFormMinSalary] = useState<string>('')
  const [formMaxSalary, setFormMaxSalary] = useState<string>('')

  const loadFamilyDashboard = async () => {
    try {
      setLoading(true)
      const data = await profilesApi.getFamilyDashboard()
      setFamilyData(data)
    } catch (err) {
      console.error('Failed to load family dashboard', err)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    loadFamilyDashboard()
  }, [])

  const handleOpenCreate = () => {
    setEditingProfile(null)
    setFormName('')
    setFormRelationship('Family')
    setFormEmail('')
    setFormPhone('')
    setFormHeadline('')
    setFormBio('')
    setFormColor('#3b82f6')
    setFormTargetRoles('')
    setFormTargetLocations('Remote')
    setFormMinSalary('')
    setFormMaxSalary('')
    setDialogOpen(true)
  }

  const handleOpenEdit = (p: CareerProfileDto) => {
    setEditingProfile(p)
    setFormName(p.name)
    setFormRelationship(p.relationship)
    setFormEmail(p.email)
    setFormPhone(p.phoneNumber || '')
    setFormHeadline(p.headline || '')
    setFormBio(p.bio || '')
    setFormColor(p.colorCode)
    setFormTargetRoles(p.preferences?.targetRoles || '')
    setFormTargetLocations(p.preferences?.targetLocations || '')
    setFormMinSalary(p.preferences?.targetSalaryMin?.toString() || '')
    setFormMaxSalary(p.preferences?.targetSalaryMax?.toString() || '')
    setDialogOpen(true)
  }

  const handleSaveProfile = async () => {
    if (!formName.trim()) return

    const dto: CreateCareerProfileDto = {
      name: formName.trim(),
      relationship: formRelationship,
      email: formEmail.trim(),
      phoneNumber: formPhone.trim() || undefined,
      headline: formHeadline.trim() || undefined,
      bio: formBio.trim() || undefined,
      colorCode: formColor,
      preferences: {
        targetRoles: formTargetRoles.trim(),
        targetLocations: formTargetLocations.trim(),
        targetSalaryMin: formMinSalary ? parseFloat(formMinSalary) : undefined,
        targetSalaryMax: formMaxSalary ? parseFloat(formMaxSalary) : undefined,
        currency: 'USD',
        openToRelocation: true,
        remotePreference: 'Remote',
      },
    }

    try {
      if (editingProfile) {
        await profilesApi.update(editingProfile.id, dto)
      } else {
        await profilesApi.create(dto)
      }
      setDialogOpen(false)
      await refreshProfiles()
      await loadFamilyDashboard()
    } catch (err) {
      console.error('Failed to save profile', err)
    }
  }

  return (
    <Box sx={{ maxWidth: 1400, mx: 'auto', pb: 6 }}>
      {/* Page Header */}
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', mb: 3 }}>
        <Box>
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 1.5, mb: 0.5 }}>
            <PeopleIcon sx={{ fontSize: 32, color: 'primary.main' }} />
            <Typography variant="h4" fontWeight={800}>
              Multi-Person Workspace
            </Typography>
            <Chip
              label={`${profiles.length} Profiles Registered`}
              color="primary"
              variant="outlined"
              size="small"
              sx={{ fontWeight: 600 }}
            />
          </Box>
          <Typography variant="body1" color="text.secondary">
            Manage multi-profile career tracking, family workspaces, and cross-entity career ownership.
          </Typography>
        </Box>

        <Button
          variant="contained"
          startIcon={<AddIcon />}
          onClick={handleOpenCreate}
          sx={{ borderRadius: 2, px: 2.5, fontWeight: 700 }}
        >
          Add Profile
        </Button>
      </Box>

      {/* Tabs */}
      <Tabs
        value={tabIndex}
        onChange={(_, val) => setTabIndex(val)}
        sx={{
          mb: 3,
          borderBottom: 1,
          borderColor: 'divider',
          '& .MuiTab-root': { fontWeight: 700, fontSize: '0.95rem' },
        }}
      >
        <Tab label="Family Dashboard & Overview" />
        <Tab label="All Workspace Profiles" />
        <Tab label={`Active Profile: ${activeProfile?.name || 'Active'}`} />
      </Tabs>

      {loading && <LinearProgress sx={{ mb: 3, borderRadius: 1 }} />}

      {/* TAB 0: Family Dashboard */}
      {tabIndex === 0 && familyData && (
        <Box>
          {/* Top Aggregate Stats */}
          <Grid container spacing={2.5} sx={{ mb: 4 }}>
            <Grid item xs={12} sm={6} md={2.4}>
              <Paper sx={{ p: 2.5, borderRadius: 3, border: '1px solid', borderColor: 'divider', bgcolor: 'background.paper' }}>
                <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', mb: 1 }}>
                  <Typography variant="caption" color="text.secondary" fontWeight={700} textTransform="uppercase">
                    Total Profiles
                  </Typography>
                  <Avatar sx={{ width: 32, height: 32, bgcolor: '#2563eb22', color: '#2563eb' }}>
                    <PeopleIcon fontSize="small" />
                  </Avatar>
                </Box>
                <Typography variant="h4" fontWeight={800}>
                  {familyData.totalProfiles}
                </Typography>
                <Typography variant="caption" color="text.secondary">
                  Active Local Hub
                </Typography>
              </Paper>
            </Grid>

            <Grid item xs={12} sm={6} md={2.4}>
              <Paper sx={{ p: 2.5, borderRadius: 3, border: '1px solid', borderColor: 'divider', bgcolor: 'background.paper' }}>
                <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', mb: 1 }}>
                  <Typography variant="caption" color="text.secondary" fontWeight={700} textTransform="uppercase">
                    Family Applications
                  </Typography>
                  <Avatar sx={{ width: 32, height: 32, bgcolor: '#10b98122', color: '#10b981' }}>
                    <AssignmentIcon fontSize="small" />
                  </Avatar>
                </Box>
                <Typography variant="h4" fontWeight={800}>
                  {familyData.totalApplications}
                </Typography>
                <Typography variant="caption" color="text.secondary">
                  Across all profiles
                </Typography>
              </Paper>
            </Grid>

            <Grid item xs={12} sm={6} md={2.4}>
              <Paper sx={{ p: 2.5, borderRadius: 3, border: '1px solid', borderColor: 'divider', bgcolor: 'background.paper' }}>
                <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', mb: 1 }}>
                  <Typography variant="caption" color="text.secondary" fontWeight={700} textTransform="uppercase">
                    Vault Documents
                  </Typography>
                  <Avatar sx={{ width: 32, height: 32, bgcolor: '#8b5cf622', color: '#8b5cf6' }}>
                    <FolderIcon fontSize="small" />
                  </Avatar>
                </Box>
                <Typography variant="h4" fontWeight={800}>
                  {familyData.totalDocuments}
                </Typography>
                <Typography variant="caption" color="text.secondary">
                  Scoped by slug
                </Typography>
              </Paper>
            </Grid>

            <Grid item xs={12} sm={6} md={2.4}>
              <Paper sx={{ p: 2.5, borderRadius: 3, border: '1px solid', borderColor: 'divider', bgcolor: 'background.paper' }}>
                <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', mb: 1 }}>
                  <Typography variant="caption" color="text.secondary" fontWeight={700} textTransform="uppercase">
                    Interviews Scheduled
                  </Typography>
                  <Avatar sx={{ width: 32, height: 32, bgcolor: '#f59e0b22', color: '#f59e0b' }}>
                    <CalendarMonthIcon fontSize="small" />
                  </Avatar>
                </Box>
                <Typography variant="h4" fontWeight={800}>
                  {familyData.totalInterviews}
                </Typography>
                <Typography variant="caption" color="text.secondary">
                  Upcoming & completed
                </Typography>
              </Paper>
            </Grid>

            <Grid item xs={12} sm={6} md={2.4}>
              <Paper sx={{ p: 2.5, borderRadius: 3, border: '1px solid', borderColor: 'divider', bgcolor: 'background.paper' }}>
                <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', mb: 1 }}>
                  <Typography variant="caption" color="text.secondary" fontWeight={700} textTransform="uppercase">
                    Offers Won
                  </Typography>
                  <Avatar sx={{ width: 32, height: 32, bgcolor: '#ec489922', color: '#ec4899' }}>
                    <EmojiEventsIcon fontSize="small" />
                  </Avatar>
                </Box>
                <Typography variant="h4" fontWeight={800} color="#ec4899">
                  {familyData.totalOffers}
                </Typography>
                <Typography variant="caption" color="text.secondary">
                  Accepted & offers
                </Typography>
              </Paper>
            </Grid>
          </Grid>

          {/* Profile Summaries Grid */}
          <Typography variant="h6" fontWeight={700} sx={{ mb: 2 }}>
            Workspace Profiles Summary
          </Typography>
          <Grid container spacing={2.5} sx={{ mb: 4 }}>
            {familyData.profileSummaries.map((p) => {
              const isCurrent = p.profileId === activeProfile?.id
              return (
                <Grid item xs={12} sm={6} md={4} key={p.profileId}>
                  <Card
                    sx={{
                      borderRadius: 3,
                      border: '2px solid',
                      borderColor: isCurrent ? p.colorCode : 'divider',
                      bgcolor: 'background.paper',
                      transition: 'all 0.2s',
                      position: 'relative',
                      boxShadow: isCurrent ? `0 6px 20px ${p.colorCode}25` : 'none',
                    }}
                  >
                    <Box
                      sx={{
                        height: 6,
                        bgcolor: p.colorCode,
                        borderTopLeftRadius: 10,
                        borderTopRightRadius: 10,
                      }}
                    />
                    <CardContent sx={{ p: 3 }}>
                      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', mb: 1.5 }}>
                        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1.5 }}>
                          <Avatar
                            sx={{
                              width: 44,
                              height: 44,
                              bgcolor: p.colorCode,
                              fontWeight: 800,
                              fontSize: 16,
                            }}
                          >
                            {p.name.substring(0, 2).toUpperCase()}
                          </Avatar>
                          <Box>
                            <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                              <Typography variant="subtitle1" fontWeight={800}>
                                {p.name}
                              </Typography>
                              {p.isPrimary && (
                                <Chip label="Primary" size="small" color="primary" sx={{ height: 18, fontSize: '0.65rem' }} />
                              )}
                            </Box>
                            <Chip
                              label={p.relationship}
                              size="small"
                              sx={{
                                height: 18,
                                fontSize: '0.65rem',
                                bgcolor: `${p.colorCode}22`,
                                color: p.colorCode,
                                fontWeight: 600,
                              }}
                            />
                          </Box>
                        </Box>
                      </Box>

                      <Typography variant="body2" color="text.secondary" sx={{ minHeight: 40, mb: 2 }}>
                        {p.headline || 'Career Profile'}
                      </Typography>

                      <Divider sx={{ my: 1.5 }} />

                      {/* Metrics */}
                      <Grid container spacing={1} sx={{ mb: 2 }}>
                        <Grid item xs={6}>
                          <Typography variant="caption" color="text.secondary">Active Applications</Typography>
                          <Typography variant="body1" fontWeight={700}>{p.activeApplications}</Typography>
                        </Grid>
                        <Grid item xs={6}>
                          <Typography variant="caption" color="text.secondary">Upcoming Interviews</Typography>
                          <Typography variant="body1" fontWeight={700}>{p.upcomingInterviews}</Typography>
                        </Grid>
                        <Grid item xs={6}>
                          <Typography variant="caption" color="text.secondary">Offers</Typography>
                          <Typography variant="body1" fontWeight={700} color="success.main">{p.totalOffers}</Typography>
                        </Grid>
                        <Grid item xs={6}>
                          <Typography variant="caption" color="text.secondary">Expiring Docs</Typography>
                          <Typography variant="body1" fontWeight={700} color={p.expiringDocuments > 0 ? 'warning.main' : 'text.primary'}>
                            {p.expiringDocuments}
                          </Typography>
                        </Grid>
                      </Grid>

                      <Box sx={{ display: 'flex', gap: 1 }}>
                        {isCurrent ? (
                          <Button
                            fullWidth
                            variant="outlined"
                            disabled
                            startIcon={<CheckCircleIcon />}
                            sx={{ borderRadius: 2 }}
                          >
                            Active Context
                          </Button>
                        ) : (
                          <Button
                            fullWidth
                            variant="contained"
                            onClick={() => switchProfile(p.profileId)}
                            sx={{
                              borderRadius: 2,
                              bgcolor: p.colorCode,
                              '&:hover': { bgcolor: p.colorCode, filter: 'brightness(0.9)' },
                            }}
                          >
                            Switch to {p.name.split(' ')[0]}
                          </Button>
                        )}
                      </Box>
                    </CardContent>
                  </Card>
                </Grid>
              )
            })}
          </Grid>

          {/* Recent Cross-Family Activity */}
          <Typography variant="h6" fontWeight={700} sx={{ mb: 2 }}>
            Recent Activity Across Family Workspace
          </Typography>
          <Paper sx={{ p: 2, borderRadius: 3, border: '1px solid', borderColor: 'divider' }}>
            {familyData.recentActivities.length === 0 ? (
              <Typography variant="body2" color="text.secondary" sx={{ p: 2, textAlign: 'center' }}>
                No recent family activities recorded yet.
              </Typography>
            ) : (
              familyData.recentActivities.map((act, idx) => (
                <Box
                  key={idx}
                  sx={{
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'space-between',
                    p: 1.5,
                    borderBottom: idx < familyData.recentActivities.length - 1 ? '1px solid' : 'none',
                    borderColor: 'divider',
                  }}
                >
                  <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
                    <Chip
                      label={act.profileName}
                      size="small"
                      sx={{
                        bgcolor: `${act.colorCode}22`,
                        color: act.colorCode,
                        fontWeight: 700,
                        border: `1px solid ${act.colorCode}55`,
                      }}
                    />
                    <Box>
                      <Typography variant="body2" fontWeight={700}>
                        {act.action}
                      </Typography>
                      <Typography variant="caption" color="text.secondary">
                        {act.title}
                      </Typography>
                    </Box>
                  </Box>
                  <Typography variant="caption" color="text.secondary">
                    {new Date(act.timestamp).toLocaleDateString()}
                  </Typography>
                </Box>
              ))
            )}
          </Paper>
        </Box>
      )}

      {/* TAB 1: All Workspace Profiles List */}
      {tabIndex === 1 && (
        <Grid container spacing={3}>
          {profiles.map((p) => {
            const isCurrent = p.id === activeProfile?.id
            return (
              <Grid item xs={12} md={6} key={p.id}>
                <Paper
                  sx={{
                    p: 3,
                    borderRadius: 3,
                    border: '2px solid',
                    borderColor: isCurrent ? p.colorCode : 'divider',
                    bgcolor: 'background.paper',
                  }}
                >
                  <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', mb: 2 }}>
                    <Box sx={{ display: 'flex', gap: 2 }}>
                      <Avatar
                        sx={{
                          width: 52,
                          height: 52,
                          bgcolor: p.colorCode,
                          fontSize: 18,
                          fontWeight: 800,
                        }}
                      >
                        {p.name.substring(0, 2).toUpperCase()}
                      </Avatar>
                      <Box>
                        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                          <Typography variant="h6" fontWeight={800}>
                            {p.name}
                          </Typography>
                          <Chip
                            label={p.relationship}
                            size="small"
                            sx={{
                              bgcolor: `${p.colorCode}22`,
                              color: p.colorCode,
                              fontWeight: 600,
                            }}
                          />
                          {p.isPrimary && <Chip label="Primary" size="small" color="primary" />}
                        </Box>
                        <Typography variant="body2" color="text.secondary">
                          {p.email}
                        </Typography>
                        <Typography variant="caption" color="text.secondary">
                          Storage Slug: <span style={{ fontFamily: 'monospace' }}>storage/profiles/{p.slug}/</span>
                        </Typography>
                      </Box>
                    </Box>

                    <IconButton size="small" onClick={() => handleOpenEdit(p)}>
                      <EditIcon fontSize="small" />
                    </IconButton>
                  </Box>

                  {p.preferences && (
                    <Box sx={{ bgcolor: 'action.hover', p: 1.5, borderRadius: 2, mb: 2 }}>
                      <Typography variant="caption" color="text.secondary" fontWeight={700} display="block" sx={{ mb: 0.5 }}>
                        TARGET CAREER CRITERIA:
                      </Typography>
                      <Typography variant="body2" fontWeight={600}>
                        {p.preferences.targetRoles || 'Roles not specified'}
                      </Typography>
                      <Typography variant="caption" color="text.secondary">
                        {p.preferences.targetLocations} · {p.preferences.remotePreference}
                        {p.preferences.targetSalaryMin ? ` · $${p.preferences.targetSalaryMin.toLocaleString()}` : ''}
                      </Typography>
                    </Box>
                  )}

                  {/* Summary Badges */}
                  <Box sx={{ display: 'flex', gap: 1, flexWrap: 'wrap', mb: 2.5 }}>
                    <Chip icon={<AssignmentIcon />} label={`${p.applicationCount} Applications`} size="small" />
                    <Chip icon={<FolderIcon />} label={`${p.documentCount} Documents`} size="small" />
                    <Chip icon={<CalendarMonthIcon />} label={`${p.interviewCount} Interviews`} size="small" />
                    <Chip icon={<PeopleIcon />} label={`${p.contactCount} Contacts`} size="small" />
                  </Box>

                  <Box sx={{ display: 'flex', justifyContent: 'flex-end', gap: 1 }}>
                    {isCurrent ? (
                      <Chip label="Currently Active Profile" color="success" sx={{ fontWeight: 700 }} />
                    ) : (
                      <Button
                        variant="contained"
                        size="small"
                        onClick={() => switchProfile(p.id)}
                        sx={{ bgcolor: p.colorCode, fontWeight: 700 }}
                      >
                        Switch to {p.name}
                      </Button>
                    )}
                  </Box>
                </Paper>
              </Grid>
            )
          })}
        </Grid>
      )}

      {/* TAB 2: Active Profile View */}
      {tabIndex === 2 && activeProfile && (
        <Box>
          <Paper sx={{ p: 3.5, borderRadius: 3, border: '1px solid', borderColor: 'divider', mb: 3 }}>
            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', mb: 3 }}>
              <Box sx={{ display: 'flex', gap: 2.5, alignItems: 'center' }}>
                <Avatar
                  sx={{
                    width: 64,
                    height: 64,
                    bgcolor: activeProfile.colorCode,
                    fontSize: 22,
                    fontWeight: 800,
                  }}
                >
                  {activeProfile.name.substring(0, 2).toUpperCase()}
                </Avatar>
                <Box>
                  <Box sx={{ display: 'flex', alignItems: 'center', gap: 1.5, mb: 0.5 }}>
                    <Typography variant="h5" fontWeight={800}>
                      {activeProfile.name}
                    </Typography>
                    <Chip
                      label={activeProfile.relationship}
                      sx={{
                        bgcolor: `${activeProfile.colorCode}22`,
                        color: activeProfile.colorCode,
                        fontWeight: 700,
                      }}
                    />
                    <Chip label="Active Context" color="success" size="small" />
                  </Box>
                  <Typography variant="body1" color="text.secondary">
                    {activeProfile.headline || 'Active Career Profile'}
                  </Typography>
                  <Typography variant="body2" color="text.secondary">
                    {activeProfile.email} {activeProfile.phoneNumber ? `· ${activeProfile.phoneNumber}` : ''}
                  </Typography>
                </Box>
              </Box>

              <Button
                variant="outlined"
                startIcon={<EditIcon />}
                onClick={() => handleOpenEdit(activeProfile)}
                sx={{ borderRadius: 2 }}
              >
                Edit Preferences
              </Button>
            </Box>

            <Divider sx={{ my: 2.5 }} />

            <Grid container spacing={3}>
              <Grid item xs={12} md={6}>
                <Typography variant="subtitle2" fontWeight={700} color="text.secondary" sx={{ mb: 1 }}>
                  CAREER PREFERENCES & TARGETS
                </Typography>
                <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1 }}>
                  <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                    <WorkOutlineIcon fontSize="small" color="action" />
                    <Typography variant="body2" fontWeight={600}>
                      Target Roles: {activeProfile.preferences?.targetRoles || 'Not defined'}
                    </Typography>
                  </Box>
                  <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                    <LocationOnIcon fontSize="small" color="action" />
                    <Typography variant="body2">
                      Locations: {activeProfile.preferences?.targetLocations || 'Any / Remote'}
                    </Typography>
                  </Box>
                  <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                    <AttachMoneyIcon fontSize="small" color="action" />
                    <Typography variant="body2">
                      Compensation: {activeProfile.preferences?.targetSalaryMin ? `$${activeProfile.preferences.targetSalaryMin.toLocaleString()}` : '$0'} - {activeProfile.preferences?.targetSalaryMax ? `$${activeProfile.preferences.targetSalaryMax.toLocaleString()}` : 'Negotiable'} {activeProfile.preferences?.currency || 'USD'}
                    </Typography>
                  </Box>
                </Box>
              </Grid>

              <Grid item xs={12} md={6}>
                <Typography variant="subtitle2" fontWeight={700} color="text.secondary" sx={{ mb: 1 }}>
                  FILESYSTEM PARTITIONS
                </Typography>
                <Paper variant="outlined" sx={{ p: 1.5, bgcolor: 'background.default', fontFamily: 'monospace', fontSize: '0.8rem' }}>
                  storage/profiles/{activeProfile.slug}/identity/<br />
                  storage/profiles/{activeProfile.slug}/academics/<br />
                  storage/profiles/{activeProfile.slug}/certifications/<br />
                  storage/profiles/{activeProfile.slug}/employment/<br />
                  storage/profiles/{activeProfile.slug}/immigration/<br />
                  storage/profiles/{activeProfile.slug}/resumes/<br />
                  applications/{activeProfile.slug}/
                </Paper>
              </Grid>
            </Grid>
          </Paper>
        </Box>
      )}

      {/* Create / Edit Dialog */}
      <Dialog open={dialogOpen} onClose={() => setDialogOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle sx={{ fontWeight: 800 }}>
          {editingProfile ? `Edit Profile: ${editingProfile.name}` : 'Create New Profile'}
        </DialogTitle>
        <DialogContent dividers>
          <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2, pt: 1 }}>
            <TextField
              label="Full Name"
              required
              fullWidth
              value={formName}
              onChange={(e) => setFormName(e.target.value)}
              placeholder="e.g. Sanidhya Kumar, Komal Kumari"
            />

            <Grid container spacing={2}>
              <Grid item xs={6}>
                <FormControl fullWidth>
                  <InputLabel>Relationship</InputLabel>
                  <Select
                    value={formRelationship}
                    label="Relationship"
                    onChange={(e) => setFormRelationship(e.target.value)}
                  >
                    <MenuItem value="Self">Self (Primary)</MenuItem>
                    <MenuItem value="Spouse">Spouse / Partner</MenuItem>
                    <MenuItem value="Child">Child / Dependent</MenuItem>
                    <MenuItem value="Family">Family Member</MenuItem>
                    <MenuItem value="Friend">Friend / Colleague</MenuItem>
                    <MenuItem value="Client">Client</MenuItem>
                    <MenuItem value="Other">Other</MenuItem>
                  </Select>
                </FormControl>
              </Grid>
              <Grid item xs={6}>
                <FormControl fullWidth>
                  <InputLabel>Badge Color</InputLabel>
                  <Select
                    value={formColor}
                    label="Badge Color"
                    onChange={(e) => setFormColor(e.target.value)}
                  >
                    <MenuItem value="#2563eb">Blue (#2563eb)</MenuItem>
                    <MenuItem value="#db2777">Pink (#db2777)</MenuItem>
                    <MenuItem value="#16a34a">Green (#16a34a)</MenuItem>
                    <MenuItem value="#d97706">Amber (#d97706)</MenuItem>
                    <MenuItem value="#7c3aed">Purple (#7c3aed)</MenuItem>
                    <MenuItem value="#0891b2">Cyan (#0891b2)</MenuItem>
                    <MenuItem value="#64748b">Slate (#64748b)</MenuItem>
                  </Select>
                </FormControl>
              </Grid>
            </Grid>

            <TextField
              label="Email Address"
              type="email"
              fullWidth
              value={formEmail}
              onChange={(e) => setFormEmail(e.target.value)}
            />

            <TextField
              label="Headline / Professional Title"
              fullWidth
              value={formHeadline}
              onChange={(e) => setFormHeadline(e.target.value)}
              placeholder="e.g. Senior Full Stack Engineer"
            />

            <TextField
              label="Target Roles (comma-separated)"
              fullWidth
              value={formTargetRoles}
              onChange={(e) => setFormTargetRoles(e.target.value)}
              placeholder="e.g. Staff Software Engineer, Engineering Manager"
            />

            <Grid container spacing={2}>
              <Grid item xs={6}>
                <TextField
                  label="Target Min Salary ($)"
                  type="number"
                  fullWidth
                  value={formMinSalary}
                  onChange={(e) => setFormMinSalary(e.target.value)}
                />
              </Grid>
              <Grid item xs={6}>
                <TextField
                  label="Target Max Salary ($)"
                  type="number"
                  fullWidth
                  value={formMaxSalary}
                  onChange={(e) => setFormMaxSalary(e.target.value)}
                />
              </Grid>
            </Grid>
          </Box>
        </DialogContent>
        <DialogActions sx={{ p: 2.5 }}>
          <Button onClick={() => setDialogOpen(false)}>Cancel</Button>
          <Button variant="contained" onClick={handleSaveProfile} sx={{ px: 3, fontWeight: 700 }}>
            Save Profile
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  )
}
