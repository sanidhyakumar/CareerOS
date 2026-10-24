import { useEffect, useState } from 'react'
import {
  Box,
  Typography,
  Card,
  Grid,
  TextField,
  Button,
  Tabs,
  Tab,
  Avatar,
  Chip,
  IconButton,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  FormControlLabel,
  Checkbox,
  Switch,
  Alert,
  Snackbar,
  CircularProgress,
  Divider,
  Paper,
  Tooltip,
} from '@mui/material'
import PersonIcon from '@mui/icons-material/Person'
import PublicIcon from '@mui/icons-material/Public'
import BadgeIcon from '@mui/icons-material/Badge'
import HomeIcon from '@mui/icons-material/Home'
import ShareIcon from '@mui/icons-material/Share'
import AddIcon from '@mui/icons-material/Add'
import DeleteOutlineIcon from '@mui/icons-material/DeleteOutline'
import EditOutlinedIcon from '@mui/icons-material/EditOutlined'
import StarIcon from '@mui/icons-material/Star'
import StarBorderIcon from '@mui/icons-material/StarBorder'
import OpenInNewIcon from '@mui/icons-material/OpenInNew'
import LinkedInIcon from '@mui/icons-material/LinkedIn'
import GitHubIcon from '@mui/icons-material/GitHub'
import LanguageIcon from '@mui/icons-material/Language'
import TwitterIcon from '@mui/icons-material/Twitter'
import CheckCircleOutlineIcon from '@mui/icons-material/CheckCircleOutline'
import WarningAmberIcon from '@mui/icons-material/WarningAmber'

import {
  profileApi,
  WorkAuthorizationType,
  SocialPlatform,
  AddressType,
} from '../api/profile'
import type {
  UserProfile,
  WorkAuthorization,
  SocialLink,
  Address,
  UpdatePersonalInfoRequest,
  AddCitizenshipRequest,
  WorkAuthorizationRequest,
  SocialLinkRequest,
  AddressRequest,
} from '../api/profile'
import { countriesApi } from '../api/countries'
import type { Country, Region } from '../api/countries'

const getPlatformIcon = (platform: SocialPlatform) => {
  switch (platform) {
    case SocialPlatform.LinkedIn:
      return <LinkedInIcon color="primary" />
    case SocialPlatform.GitHub:
      return <GitHubIcon />
    case SocialPlatform.PersonalWebsite:
      return <LanguageIcon color="success" />
    case SocialPlatform.TwitterX:
      return <TwitterIcon color="info" />
    default:
      return <ShareIcon />
  }
}

const getWorkAuthLabel = (type: WorkAuthorizationType): string => {
  switch (type) {
    case WorkAuthorizationType.Citizen:
      return 'Citizen'
    case WorkAuthorizationType.PermanentResident:
      return 'Permanent Resident (Green Card)'
    case WorkAuthorizationType.WorkVisa:
      return 'Work Visa (e.g., H-1B, L-1, TN)'
    case WorkAuthorizationType.StudentOptCpt:
      return 'Student (OPT / CPT)'
    case WorkAuthorizationType.RequiresSponsorship:
      return 'Requires Sponsorship'
    case WorkAuthorizationType.FreelanceContractor:
      return 'Contractor / Freelance'
    default:
      return 'Other'
  }
}

const getAddressTypeLabel = (type: AddressType): string => {
  switch (type) {
    case AddressType.CurrentHome:
      return 'Current Home'
    case AddressType.Mailing:
      return 'Mailing Address'
    case AddressType.RelocationTarget:
      return 'Relocation Target'
    case AddressType.Previous:
      return 'Previous Address'
    default:
      return 'Address'
  }
}

export default function ProfilePage() {
  const [profile, setProfile] = useState<UserProfile | null>(null)
  const [countries, setCountries] = useState<Country[]>([])
  const [loading, setLoading] = useState(true)
  const [savingPersonal, setSavingPersonal] = useState(false)
  const [currentTab, setCurrentTab] = useState(0)

  // Personal Info Form State
  const [personalForm, setPersonalForm] = useState<UpdatePersonalInfoRequest>({
    firstName: '',
    middleName: '',
    lastName: '',
    preferredName: '',
    headline: '',
    email: '',
    secondaryEmail: '',
    phoneNumber: '',
    secondaryPhone: '',
    bio: '',
    dateOfBirth: '',
    avatarUrl: '',
  })

  // Dialog States
  const [citizenshipDialogOpen, setCitizenshipDialogOpen] = useState(false)
  const [citizenshipForm, setCitizenshipForm] = useState<AddCitizenshipRequest>({
    countryId: '',
    isPrimary: false,
    passportNumber: '',
    notes: '',
  })

  const [workAuthDialogOpen, setWorkAuthDialogOpen] = useState(false)
  const [editingWorkAuthId, setEditingWorkAuthId] = useState<string | null>(null)
  const [workAuthForm, setWorkAuthForm] = useState<WorkAuthorizationRequest>({
    countryId: '',
    authorizationType: WorkAuthorizationType.Citizen,
    visaType: '',
    requiresSponsorship: false,
    validFrom: '',
    validTo: '',
    notes: '',
  })

  const [socialDialogOpen, setSocialDialogOpen] = useState(false)
  const [editingSocialId, setEditingSocialId] = useState<string | null>(null)
  const [socialForm, setSocialForm] = useState<SocialLinkRequest>({
    platform: SocialPlatform.LinkedIn,
    url: '',
    displayLabel: '',
    displayOrder: 0,
  })

  const [addressDialogOpen, setAddressDialogOpen] = useState(false)
  const [editingAddressId, setEditingAddressId] = useState<string | null>(null)
  const [addressRegions, setAddressRegions] = useState<Region[]>([])
  const [addressForm, setAddressForm] = useState<AddressRequest>({
    addressType: AddressType.CurrentHome,
    street1: '',
    street2: '',
    city: '',
    stateProvince: '',
    regionId: null,
    postalCode: '',
    countryId: '',
    isPrimary: false,
    willingToRelocate: false,
    openToRemote: true,
  })

  // Feedback Notification
  const [snackbar, setSnackbar] = useState<{
    open: boolean
    message: string
    severity: 'success' | 'error'
  }>({
    open: false,
    message: '',
    severity: 'success',
  })

  const showNotification = (message: string, severity: 'success' | 'error' = 'success') => {
    setSnackbar({ open: true, message, severity })
  }

  const loadData = async () => {
    try {
      setLoading(true)
      const [p, cList] = await Promise.all([
        profileApi.getProfile(),
        countriesApi.getAll(),
      ])

      setProfile(p)
      setCountries(cList)

      setPersonalForm({
        firstName: p.firstName || '',
        middleName: p.middleName || '',
        lastName: p.lastName || '',
        preferredName: p.preferredName || '',
        headline: p.headline || '',
        email: p.email || '',
        secondaryEmail: p.secondaryEmail || '',
        phoneNumber: p.phoneNumber || '',
        secondaryPhone: p.secondaryPhone || '',
        bio: p.bio || '',
        dateOfBirth: p.dateOfBirth ? p.dateOfBirth.substring(0, 10) : '',
        avatarUrl: p.avatarUrl || '',
      })
    } catch {
      showNotification('Failed to load profile data', 'error')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    loadData()
  }, [])

  // Handle Country selection in Address Form to populate Regions
  const handleAddressCountryChange = async (countryId: string) => {
    setAddressForm((prev) => ({ ...prev, countryId, regionId: null }))
    if (!countryId) {
      setAddressRegions([])
      return
    }
    try {
      const regions = await countriesApi.getRegions(countryId)
      setAddressRegions(regions)
    } catch {
      setAddressRegions([])
    }
  }

  // 1. Personal Info Save
  const handleSavePersonalInfo = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!personalForm.firstName.trim() || !personalForm.lastName.trim() || !personalForm.email.trim()) {
      showNotification('First name, Last name, and Email are required', 'error')
      return
    }

    try {
      setSavingPersonal(true)
      const updated = await profileApi.updatePersonalInfo({
        ...personalForm,
        dateOfBirth: personalForm.dateOfBirth ? new Date(personalForm.dateOfBirth).toISOString() : null,
      })
      setProfile(updated)
      showNotification('Personal information updated successfully!')
    } catch {
      showNotification('Failed to update personal information', 'error')
    } finally {
      setSavingPersonal(false)
    }
  }

  // 2. Citizenship Handlers
  const handleOpenAddCitizenship = () => {
    setCitizenshipForm({
      countryId: countries.length > 0 ? countries[0].id : '',
      isPrimary: (profile?.citizenships.length ?? 0) === 0,
      passportNumber: '',
      notes: '',
    })
    setCitizenshipDialogOpen(true)
  }

  const handleSaveCitizenship = async () => {
    if (!citizenshipForm.countryId) {
      showNotification('Please select a country', 'error')
      return
    }

    try {
      await profileApi.addCitizenship(citizenshipForm)
      setCitizenshipDialogOpen(false)
      showNotification('Citizenship added successfully')
      await loadData()
    } catch {
      showNotification('Failed to add citizenship', 'error')
    }
  }

  const handleDeleteCitizenship = async (id: string) => {
    try {
      await profileApi.deleteCitizenship(id)
      showNotification('Citizenship removed')
      await loadData()
    } catch {
      showNotification('Failed to remove citizenship', 'error')
    }
  }

  const handleSetPrimaryCitizenship = async (id: string) => {
    try {
      await profileApi.setPrimaryCitizenship(id)
      showNotification('Primary citizenship updated')
      await loadData()
    } catch {
      showNotification('Failed to set primary citizenship', 'error')
    }
  }

  // 3. Work Authorization Handlers
  const handleOpenAddWorkAuth = () => {
    setEditingWorkAuthId(null)
    setWorkAuthForm({
      countryId: countries.length > 0 ? countries[0].id : '',
      authorizationType: WorkAuthorizationType.Citizen,
      visaType: '',
      requiresSponsorship: false,
      validFrom: '',
      validTo: '',
      notes: '',
    })
    setWorkAuthDialogOpen(true)
  }

  const handleOpenEditWorkAuth = (auth: WorkAuthorization) => {
    setEditingWorkAuthId(auth.id)
    setWorkAuthForm({
      countryId: auth.countryId,
      authorizationType: auth.authorizationType,
      visaType: auth.visaType || '',
      requiresSponsorship: auth.requiresSponsorship,
      validFrom: auth.validFrom ? auth.validFrom.substring(0, 10) : '',
      validTo: auth.validTo ? auth.validTo.substring(0, 10) : '',
      notes: auth.notes || '',
    })
    setWorkAuthDialogOpen(true)
  }

  const handleSaveWorkAuth = async () => {
    if (!workAuthForm.countryId) {
      showNotification('Please select a country', 'error')
      return
    }

    const payload: WorkAuthorizationRequest = {
      ...workAuthForm,
      validFrom: workAuthForm.validFrom ? new Date(workAuthForm.validFrom).toISOString() : null,
      validTo: workAuthForm.validTo ? new Date(workAuthForm.validTo).toISOString() : null,
    }

    try {
      if (editingWorkAuthId) {
        await profileApi.updateWorkAuthorization(editingWorkAuthId, payload)
        showNotification('Work authorization updated')
      } else {
        await profileApi.addWorkAuthorization(payload)
        showNotification('Work authorization added')
      }
      setWorkAuthDialogOpen(false)
      await loadData()
    } catch {
      showNotification('Failed to save work authorization', 'error')
    }
  }

  const handleDeleteWorkAuth = async (id: string) => {
    try {
      await profileApi.deleteWorkAuthorization(id)
      showNotification('Work authorization removed')
      await loadData()
    } catch {
      showNotification('Failed to remove work authorization', 'error')
    }
  }

  // 4. Social Links Handlers
  const handleOpenAddSocial = () => {
    setEditingSocialId(null)
    setSocialForm({
      platform: SocialPlatform.LinkedIn,
      url: '',
      displayLabel: '',
      displayOrder: (profile?.socialLinks.length ?? 0) + 1,
    })
    setSocialDialogOpen(true)
  }

  const handleOpenEditSocial = (link: SocialLink) => {
    setEditingSocialId(link.id)
    setSocialForm({
      platform: link.platform,
      url: link.url,
      displayLabel: link.displayLabel || '',
      displayOrder: link.displayOrder,
    })
    setSocialDialogOpen(true)
  }

  const handleSaveSocial = async () => {
    if (!socialForm.url.trim()) {
      showNotification('URL is required', 'error')
      return
    }

    try {
      if (editingSocialId) {
        await profileApi.updateSocialLink(editingSocialId, socialForm)
        showNotification('Social link updated')
      } else {
        await profileApi.addSocialLink(socialForm)
        showNotification('Social link added')
      }
      setSocialDialogOpen(false)
      await loadData()
    } catch {
      showNotification('Failed to save social link. Check URL format.', 'error')
    }
  }

  const handleDeleteSocial = async (id: string) => {
    try {
      await profileApi.deleteSocialLink(id)
      showNotification('Social link removed')
      await loadData()
    } catch {
      showNotification('Failed to remove social link', 'error')
    }
  }

  // 5. Addresses Handlers
  const handleOpenAddAddress = async () => {
    setEditingAddressId(null)
    const initialCountryId = countries.length > 0 ? countries[0].id : ''
    setAddressForm({
      addressType: AddressType.CurrentHome,
      street1: '',
      street2: '',
      city: '',
      stateProvince: '',
      regionId: null,
      postalCode: '',
      countryId: initialCountryId,
      isPrimary: (profile?.addresses.length ?? 0) === 0,
      willingToRelocate: false,
      openToRemote: true,
    })
    if (initialCountryId) {
      try {
        const regs = await countriesApi.getRegions(initialCountryId)
        setAddressRegions(regs)
      } catch {
        setAddressRegions([])
      }
    }
    setAddressDialogOpen(true)
  }

  const handleOpenEditAddress = async (addr: Address) => {
    setEditingAddressId(addr.id)
    setAddressForm({
      addressType: addr.addressType,
      street1: addr.street1,
      street2: addr.street2 || '',
      city: addr.city,
      stateProvince: addr.stateProvince || '',
      regionId: addr.regionId || null,
      postalCode: addr.postalCode,
      countryId: addr.countryId,
      isPrimary: addr.isPrimary,
      willingToRelocate: addr.willingToRelocate,
      openToRemote: addr.openToRemote,
    })
    try {
      const regs = await countriesApi.getRegions(addr.countryId)
      setAddressRegions(regs)
    } catch {
      setAddressRegions([])
    }
    setAddressDialogOpen(true)
  }

  const handleSaveAddress = async () => {
    if (!addressForm.street1.trim() || !addressForm.city.trim() || !addressForm.countryId) {
      showNotification('Street 1, City, and Country are required', 'error')
      return
    }

    try {
      if (editingAddressId) {
        await profileApi.updateAddress(editingAddressId, addressForm)
        showNotification('Address updated')
      } else {
        await profileApi.addAddress(addressForm)
        showNotification('Address added')
      }
      setAddressDialogOpen(false)
      await loadData()
    } catch {
      showNotification('Failed to save address', 'error')
    }
  }

  const handleDeleteAddress = async (id: string) => {
    try {
      await profileApi.deleteAddress(id)
      showNotification('Address removed')
      await loadData()
    } catch {
      showNotification('Failed to remove address', 'error')
    }
  }

  const handleSetPrimaryAddress = async (id: string) => {
    try {
      await profileApi.setPrimaryAddress(id)
      showNotification('Primary address set')
      await loadData()
    } catch {
      showNotification('Failed to set primary address', 'error')
    }
  }

  if (loading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '60vh' }}>
        <CircularProgress />
      </Box>
    )
  }

  const primaryCitizenship = profile?.citizenships.find((c) => c.isPrimary)
  const primaryAddress = profile?.addresses.find((a) => a.isPrimary)

  return (
    <Box sx={{ p: { xs: 2, md: 3 }, maxWidth: 1280, mx: 'auto' }}>
      {/* Profile Header Banner Card */}
      <Card
        sx={{
          mb: 3,
          p: 3,
          borderRadius: 3,
          background: 'linear-gradient(135deg, rgba(30,41,59,0.9) 0%, rgba(15,23,42,0.95) 100%)',
          border: '1px solid rgba(255,255,255,0.08)',
          boxShadow: '0 8px 32px rgba(0,0,0,0.3)',
        }}
      >
        <Grid container spacing={3} alignItems="center">
          <Grid item>
            <Avatar
              src={profile?.avatarUrl || undefined}
              alt={profile?.fullName}
              sx={{
                width: 96,
                height: 96,
                fontSize: 32,
                fontWeight: 700,
                bgcolor: 'primary.main',
                border: '3px solid rgba(255,255,255,0.2)',
                boxShadow: '0 4px 16px rgba(0,0,0,0.4)',
              }}
            >
              {profile?.firstName?.[0]}
              {profile?.lastName?.[0]}
            </Avatar>
          </Grid>
          <Grid item xs>
            <Typography variant="h4" fontWeight={800} color="text.primary" gutterBottom>
              {profile?.fullName || 'Your Name'}
              {profile?.preferredName && (
                <Typography component="span" variant="h5" color="text.secondary" sx={{ ml: 1 }}>
                  ({profile.preferredName})
                </Typography>
              )}
            </Typography>
            <Typography variant="subtitle1" color="primary.light" fontWeight={500} gutterBottom>
              {profile?.headline || 'Add a professional headline'}
            </Typography>

            <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1, mt: 1.5 }}>
              <Chip
                icon={<PersonIcon fontSize="small" />}
                label={profile?.email}
                size="small"
                variant="outlined"
              />
              {profile?.phoneNumber && (
                <Chip label={profile.phoneNumber} size="small" variant="outlined" />
              )}
              {primaryCitizenship && (
                <Chip
                  icon={<PublicIcon fontSize="small" />}
                  label={`Citizen: ${primaryCitizenship.countryName} (${primaryCitizenship.countryCode})`}
                  color="info"
                  size="small"
                />
              )}
              {primaryAddress && (
                <Chip
                  icon={<HomeIcon fontSize="small" />}
                  label={`${primaryAddress.city}, ${primaryAddress.stateProvince || primaryAddress.countryCode}`}
                  color="secondary"
                  size="small"
                  variant="outlined"
                />
              )}
            </Box>
          </Grid>
        </Grid>
      </Card>

      {/* Navigation Tabs */}
      <Paper sx={{ mb: 3, borderRadius: 2, bgcolor: 'background.paper' }}>
        <Tabs
          value={currentTab}
          onChange={(_, newVal) => setCurrentTab(newVal)}
          variant="scrollable"
          scrollButtons="auto"
          textColor="primary"
          indicatorColor="primary"
          sx={{ borderBottom: 1, borderColor: 'divider' }}
        >
          <Tab icon={<PersonIcon />} iconPosition="start" label="Personal Info" />
          <Tab
            icon={<BadgeIcon />}
            iconPosition="start"
            label={`Citizenship & Visas (${(profile?.citizenships.length ?? 0) + (profile?.workAuthorizations.length ?? 0)})`}
          />
          <Tab
            icon={<HomeIcon />}
            iconPosition="start"
            label={`Addresses (${profile?.addresses.length ?? 0})`}
          />
          <Tab
            icon={<ShareIcon />}
            iconPosition="start"
            label={`Social Links (${profile?.socialLinks.length ?? 0})`}
          />
        </Tabs>
      </Paper>

      {/* TAB 0: Personal Information */}
      {currentTab === 0 && (
        <Card sx={{ borderRadius: 3, p: 3 }}>
          <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
            <Box>
              <Typography variant="h6" fontWeight={700}>
                Personal Information
              </Typography>
              <Typography variant="body2" color="text.secondary">
                Your core identity, headline, and direct contact channels.
              </Typography>
            </Box>
            <Button
              variant="contained"
              color="primary"
              onClick={handleSavePersonalInfo}
              disabled={savingPersonal}
            >
              {savingPersonal ? 'Saving...' : 'Save Changes'}
            </Button>
          </Box>

          <Box component="form" onSubmit={handleSavePersonalInfo}>
            <Grid container spacing={2.5}>
              <Grid item xs={12} sm={4}>
                <TextField
                  fullWidth
                  required
                  label="First Name"
                  value={personalForm.firstName}
                  onChange={(e) => setPersonalForm({ ...personalForm, firstName: e.target.value })}
                />
              </Grid>
              <Grid item xs={12} sm={4}>
                <TextField
                  fullWidth
                  label="Middle Name"
                  value={personalForm.middleName}
                  onChange={(e) => setPersonalForm({ ...personalForm, middleName: e.target.value })}
                />
              </Grid>
              <Grid item xs={12} sm={4}>
                <TextField
                  fullWidth
                  required
                  label="Last Name"
                  value={personalForm.lastName}
                  onChange={(e) => setPersonalForm({ ...personalForm, lastName: e.target.value })}
                />
              </Grid>

              <Grid item xs={12} sm={6}>
                <TextField
                  fullWidth
                  label="Preferred Name / Nickname"
                  value={personalForm.preferredName}
                  onChange={(e) => setPersonalForm({ ...personalForm, preferredName: e.target.value })}
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <TextField
                  fullWidth
                  label="Professional Headline"
                  placeholder="e.g. Senior Distributed Systems Engineer"
                  value={personalForm.headline}
                  onChange={(e) => setPersonalForm({ ...personalForm, headline: e.target.value })}
                />
              </Grid>

              <Grid item xs={12} sm={6}>
                <TextField
                  fullWidth
                  required
                  type="email"
                  label="Primary Email"
                  value={personalForm.email}
                  onChange={(e) => setPersonalForm({ ...personalForm, email: e.target.value })}
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <TextField
                  fullWidth
                  type="email"
                  label="Secondary Email"
                  value={personalForm.secondaryEmail}
                  onChange={(e) => setPersonalForm({ ...personalForm, secondaryEmail: e.target.value })}
                />
              </Grid>

              <Grid item xs={12} sm={6}>
                <TextField
                  fullWidth
                  label="Primary Phone"
                  value={personalForm.phoneNumber}
                  onChange={(e) => setPersonalForm({ ...personalForm, phoneNumber: e.target.value })}
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <TextField
                  fullWidth
                  label="Secondary Phone"
                  value={personalForm.secondaryPhone}
                  onChange={(e) => setPersonalForm({ ...personalForm, secondaryPhone: e.target.value })}
                />
              </Grid>

              <Grid item xs={12} sm={6}>
                <TextField
                  fullWidth
                  type="date"
                  label="Date of Birth"
                  InputLabelProps={{ shrink: true }}
                  value={personalForm.dateOfBirth}
                  onChange={(e) => setPersonalForm({ ...personalForm, dateOfBirth: e.target.value })}
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <TextField
                  fullWidth
                  label="Avatar / Profile Photo URL"
                  placeholder="https://..."
                  value={personalForm.avatarUrl}
                  onChange={(e) => setPersonalForm({ ...personalForm, avatarUrl: e.target.value })}
                />
              </Grid>

              <Grid item xs={12}>
                <TextField
                  fullWidth
                  multiline
                  rows={4}
                  label="Executive Summary / Bio"
                  placeholder="Brief summary of your professional background, strengths, and career ambitions..."
                  value={personalForm.bio}
                  onChange={(e) => setPersonalForm({ ...personalForm, bio: e.target.value })}
                />
              </Grid>
            </Grid>
          </Box>
        </Card>
      )}

      {/* TAB 1: Citizenship & Work Authorization */}
      {currentTab === 1 && (
        <Grid container spacing={3}>
          {/* Citizenships Card */}
          <Grid item xs={12} md={5}>
            <Card sx={{ borderRadius: 3, p: 3, height: '100%' }}>
              <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
                <Box>
                  <Typography variant="h6" fontWeight={700}>
                    Citizenship
                  </Typography>
                  <Typography variant="body2" color="text.secondary">
                    Your national passports and legal citizenships.
                  </Typography>
                </Box>
                <Button
                  size="small"
                  variant="outlined"
                  startIcon={<AddIcon />}
                  onClick={handleOpenAddCitizenship}
                >
                  Add
                </Button>
              </Box>

              {profile?.citizenships.length === 0 ? (
                <Alert severity="info" sx={{ mt: 2 }}>
                  No citizenship recorded. Add your primary country of citizenship.
                </Alert>
              ) : (
                <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1.5, mt: 2 }}>
                  {profile?.citizenships.map((c) => (
                    <Paper
                      key={c.id}
                      variant="outlined"
                      sx={{
                        p: 2,
                        borderRadius: 2,
                        display: 'flex',
                        alignItems: 'center',
                        justifyContent: 'space-between',
                        borderLeft: c.isPrimary ? '4px solid #3b82f6' : undefined,
                      }}
                    >
                      <Box>
                        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                          <Typography variant="subtitle1" fontWeight={700}>
                            {c.countryName}
                          </Typography>
                          <Chip label={c.countryCode} size="small" variant="outlined" />
                          {c.isPrimary && (
                            <Chip label="Primary" color="primary" size="small" />
                          )}
                        </Box>
                        {c.passportNumber && (
                          <Typography variant="body2" color="text.secondary" sx={{ mt: 0.5 }}>
                            Passport: {c.passportNumber}
                          </Typography>
                        )}
                        {c.notes && (
                          <Typography variant="caption" color="text.secondary">
                            {c.notes}
                          </Typography>
                        )}
                      </Box>
                      <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
                        <Tooltip title={c.isPrimary ? 'Primary Citizenship' : 'Set as Primary'}>
                          <IconButton
                            size="small"
                            color={c.isPrimary ? 'primary' : 'default'}
                            onClick={() => handleSetPrimaryCitizenship(c.id)}
                          >
                            {c.isPrimary ? <StarIcon /> : <StarBorderIcon />}
                          </IconButton>
                        </Tooltip>
                        <IconButton
                          size="small"
                          color="error"
                          onClick={() => handleDeleteCitizenship(c.id)}
                        >
                          <DeleteOutlineIcon />
                        </IconButton>
                      </Box>
                    </Paper>
                  ))}
                </Box>
              )}
            </Card>
          </Grid>

          {/* Work Authorizations Card */}
          <Grid item xs={12} md={7}>
            <Card sx={{ borderRadius: 3, p: 3, height: '100%' }}>
              <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
                <Box>
                  <Typography variant="h6" fontWeight={700}>
                    Work Authorizations & Visas
                  </Typography>
                  <Typography variant="body2" color="text.secondary">
                    Status, visa categories, and sponsorship requirements.
                  </Typography>
                </Box>
                <Button
                  size="small"
                  variant="contained"
                  startIcon={<AddIcon />}
                  onClick={handleOpenAddWorkAuth}
                >
                  Add Authorization
                </Button>
              </Box>

              {profile?.workAuthorizations.length === 0 ? (
                <Alert severity="info" sx={{ mt: 2 }}>
                  No work authorizations specified. Add where you are legally authorized to work.
                </Alert>
              ) : (
                <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2, mt: 2 }}>
                  {profile?.workAuthorizations.map((auth) => (
                    <Paper
                      key={auth.id}
                      variant="outlined"
                      sx={{ p: 2, borderRadius: 2 }}
                    >
                      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
                        <Box>
                          <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, flexWrap: 'wrap' }}>
                            <Typography variant="subtitle1" fontWeight={700}>
                              {auth.countryName}
                            </Typography>
                            <Chip
                              label={getWorkAuthLabel(auth.authorizationType)}
                              color="primary"
                              size="small"
                            />
                            {auth.visaType && (
                              <Chip label={auth.visaType} variant="outlined" size="small" />
                            )}
                          </Box>

                          <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mt: 1 }}>
                            {auth.requiresSponsorship ? (
                              <Chip
                                icon={<WarningAmberIcon />}
                                label="Requires Sponsorship"
                                color="warning"
                                size="small"
                              />
                            ) : (
                              <Chip
                                icon={<CheckCircleOutlineIcon />}
                                label="Authorized Without Sponsorship"
                                color="success"
                                size="small"
                                variant="outlined"
                              />
                            )}
                          </Box>

                          {(auth.validFrom || auth.validTo) && (
                            <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
                              Validity:{' '}
                              {auth.validFrom ? new Date(auth.validFrom).toLocaleDateString() : 'N/A'}{' '}
                              to{' '}
                              {auth.validTo ? new Date(auth.validTo).toLocaleDateString() : 'Indefinite'}
                            </Typography>
                          )}

                          {auth.notes && (
                            <Typography variant="caption" color="text.secondary" sx={{ display: 'block', mt: 0.5 }}>
                              {auth.notes}
                            </Typography>
                          )}
                        </Box>

                        <Box sx={{ display: 'flex', gap: 0.5 }}>
                          <IconButton
                            size="small"
                            color="primary"
                            onClick={() => handleOpenEditWorkAuth(auth)}
                          >
                            <EditOutlinedIcon />
                          </IconButton>
                          <IconButton
                            size="small"
                            color="error"
                            onClick={() => handleDeleteWorkAuth(auth.id)}
                          >
                            <DeleteOutlineIcon />
                          </IconButton>
                        </Box>
                      </Box>
                    </Paper>
                  ))}
                </Box>
              )}
            </Card>
          </Grid>
        </Grid>
      )}

      {/* TAB 2: Addresses & Location */}
      {currentTab === 2 && (
        <Card sx={{ borderRadius: 3, p: 3 }}>
          <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
            <Box>
              <Typography variant="h6" fontWeight={700}>
                Addresses & Location Preferences
              </Typography>
              <Typography variant="body2" color="text.secondary">
                Manage your physical locations, relocation readiness, and remote availability.
              </Typography>
            </Box>
            <Button
              variant="contained"
              startIcon={<AddIcon />}
              onClick={handleOpenAddAddress}
            >
              Add Address
            </Button>
          </Box>

          {profile?.addresses.length === 0 ? (
            <Alert severity="info">
              No addresses configured. Add your current home or relocation target.
            </Alert>
          ) : (
            <Grid container spacing={2.5}>
              {profile?.addresses.map((addr) => (
                <Grid item xs={12} md={6} key={addr.id}>
                  <Paper
                    variant="outlined"
                    sx={{
                      p: 2.5,
                      borderRadius: 2.5,
                      height: '100%',
                      display: 'flex',
                      flexDirection: 'column',
                      justifyContent: 'space-between',
                      borderLeft: addr.isPrimary ? '4px solid #10b981' : undefined,
                    }}
                  >
                    <Box>
                      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 1 }}>
                        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                          <Chip
                            label={getAddressTypeLabel(addr.addressType)}
                            size="small"
                            color="primary"
                            variant="outlined"
                          />
                          {addr.isPrimary && (
                            <Chip label="Primary" color="success" size="small" />
                          )}
                        </Box>
                        <Box sx={{ display: 'flex', gap: 0.5 }}>
                          <Tooltip title={addr.isPrimary ? 'Primary Address' : 'Set as Primary'}>
                            <IconButton
                              size="small"
                              color={addr.isPrimary ? 'success' : 'default'}
                              onClick={() => handleSetPrimaryAddress(addr.id)}
                            >
                              {addr.isPrimary ? <StarIcon /> : <StarBorderIcon />}
                            </IconButton>
                          </Tooltip>
                          <IconButton
                            size="small"
                            color="primary"
                            onClick={() => handleOpenEditAddress(addr)}
                          >
                            <EditOutlinedIcon />
                          </IconButton>
                          <IconButton
                            size="small"
                            color="error"
                            onClick={() => handleDeleteAddress(addr.id)}
                          >
                            <DeleteOutlineIcon />
                          </IconButton>
                        </Box>
                      </Box>

                      <Typography variant="subtitle1" fontWeight={700}>
                        {addr.street1}
                        {addr.street2 && `, ${addr.street2}`}
                      </Typography>
                      <Typography variant="body2" color="text.secondary">
                        {addr.city}, {addr.regionName || addr.stateProvince} {addr.postalCode}
                      </Typography>
                      <Typography variant="body2" color="text.secondary" fontWeight={500}>
                        {addr.countryName} ({addr.countryCode})
                      </Typography>

                      <Box sx={{ display: 'flex', gap: 1, mt: 2, flexWrap: 'wrap' }}>
                        {addr.openToRemote && (
                          <Chip label="Open to Remote" size="small" color="info" variant="outlined" />
                        )}
                        {addr.willingToRelocate && (
                          <Chip label="Willing to Relocate" size="small" color="secondary" variant="outlined" />
                        )}
                      </Box>
                    </Box>
                  </Paper>
                </Grid>
              ))}
            </Grid>
          )}
        </Card>
      )}

      {/* TAB 3: Social & Portfolio Links */}
      {currentTab === 3 && (
        <Card sx={{ borderRadius: 3, p: 3 }}>
          <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
            <Box>
              <Typography variant="h6" fontWeight={700}>
                Social Links & Profiles
              </Typography>
              <Typography variant="body2" color="text.secondary">
                Connect your LinkedIn, GitHub, portfolio, and other professional footprints.
              </Typography>
            </Box>
            <Button
              variant="contained"
              startIcon={<AddIcon />}
              onClick={handleOpenAddSocial}
            >
              Add Link
            </Button>
          </Box>

          {profile?.socialLinks.length === 0 ? (
            <Alert severity="info">
              No social links added yet. Add your LinkedIn or GitHub profile to easily reference in applications.
            </Alert>
          ) : (
            <Grid container spacing={2.5}>
              {profile?.socialLinks.map((link) => (
                <Grid item xs={12} sm={6} md={4} key={link.id}>
                  <Paper
                    variant="outlined"
                    sx={{
                      p: 2.5,
                      borderRadius: 2.5,
                      display: 'flex',
                      flexDirection: 'column',
                      justifyContent: 'space-between',
                      height: '100%',
                    }}
                  >
                    <Box>
                      <Box sx={{ display: 'flex', alignItems: 'center', gap: 1.5, mb: 1.5 }}>
                        {getPlatformIcon(link.platform)}
                        <Box sx={{ flex: 1, overflow: 'hidden' }}>
                          <Typography variant="subtitle1" fontWeight={700} noWrap>
                            {link.displayLabel || link.platformName}
                          </Typography>
                          <Typography variant="caption" color="text.secondary" noWrap display="block">
                            {link.platformName}
                          </Typography>
                        </Box>
                      </Box>

                      <Typography
                        variant="body2"
                        color="primary"
                        sx={{
                          wordBreak: 'break-all',
                          display: '-webkit-box',
                          WebkitLineClamp: 2,
                          WebkitBoxOrient: 'vertical',
                          overflow: 'hidden',
                        }}
                      >
                        {link.url}
                      </Typography>
                    </Box>

                    <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mt: 2 }}>
                      <Button
                        size="small"
                        endIcon={<OpenInNewIcon fontSize="small" />}
                        href={link.url}
                        target="_blank"
                        rel="noreferrer"
                      >
                        Visit
                      </Button>
                      <Box sx={{ display: 'flex', gap: 0.5 }}>
                        <IconButton
                          size="small"
                          color="primary"
                          onClick={() => handleOpenEditSocial(link)}
                        >
                          <EditOutlinedIcon fontSize="small" />
                        </IconButton>
                        <IconButton
                          size="small"
                          color="error"
                          onClick={() => handleDeleteSocial(link.id)}
                        >
                          <DeleteOutlineIcon fontSize="small" />
                        </IconButton>
                      </Box>
                    </Box>
                  </Paper>
                </Grid>
              ))}
            </Grid>
          )}
        </Card>
      )}

      {/* DIALOG 1: Add Citizenship */}
      <Dialog
        open={citizenshipDialogOpen}
        onClose={() => setCitizenshipDialogOpen(false)}
        maxWidth="sm"
        fullWidth
      >
        <DialogTitle>Add Citizenship</DialogTitle>
        <DialogContent dividers>
          <Grid container spacing={2} sx={{ mt: 0.5 }}>
            <Grid item xs={12}>
              <FormControl fullWidth required>
                <InputLabel id="country-select-label">Country</InputLabel>
                <Select
                  labelId="country-select-label"
                  label="Country"
                  value={citizenshipForm.countryId}
                  onChange={(e) => setCitizenshipForm({ ...citizenshipForm, countryId: e.target.value })}
                >
                  {countries.map((c) => (
                    <MenuItem key={c.id} value={c.id}>
                      {c.name} ({c.code})
                    </MenuItem>
                  ))}
                </Select>
              </FormControl>
            </Grid>

            <Grid item xs={12}>
              <TextField
                fullWidth
                label="Passport / Document Number (Optional)"
                value={citizenshipForm.passportNumber}
                onChange={(e) => setCitizenshipForm({ ...citizenshipForm, passportNumber: e.target.value })}
              />
            </Grid>

            <Grid item xs={12}>
              <TextField
                fullWidth
                multiline
                rows={2}
                label="Notes (Optional)"
                value={citizenshipForm.notes}
                onChange={(e) => setCitizenshipForm({ ...citizenshipForm, notes: e.target.value })}
              />
            </Grid>

            <Grid item xs={12}>
              <FormControlLabel
                control={
                  <Checkbox
                    checked={citizenshipForm.isPrimary}
                    onChange={(e) => setCitizenshipForm({ ...citizenshipForm, isPrimary: e.target.checked })}
                  />
                }
                label="Make this my Primary Citizenship"
              />
            </Grid>
          </Grid>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setCitizenshipDialogOpen(false)}>Cancel</Button>
          <Button variant="contained" onClick={handleSaveCitizenship}>
            Save
          </Button>
        </DialogActions>
      </Dialog>

      {/* DIALOG 2: Add/Edit Work Authorization */}
      <Dialog
        open={workAuthDialogOpen}
        onClose={() => setWorkAuthDialogOpen(false)}
        maxWidth="sm"
        fullWidth
      >
        <DialogTitle>{editingWorkAuthId ? 'Edit Work Authorization' : 'Add Work Authorization'}</DialogTitle>
        <DialogContent dividers>
          <Grid container spacing={2} sx={{ mt: 0.5 }}>
            <Grid item xs={12}>
              <FormControl fullWidth required>
                <InputLabel id="workauth-country-label">Country</InputLabel>
                <Select
                  labelId="workauth-country-label"
                  label="Country"
                  value={workAuthForm.countryId}
                  onChange={(e) => setWorkAuthForm({ ...workAuthForm, countryId: e.target.value })}
                >
                  {countries.map((c) => (
                    <MenuItem key={c.id} value={c.id}>
                      {c.name} ({c.code})
                    </MenuItem>
                  ))}
                </Select>
              </FormControl>
            </Grid>

            <Grid item xs={12}>
              <FormControl fullWidth required>
                <InputLabel id="workauth-type-label">Authorization Type</InputLabel>
                <Select
                  labelId="workauth-type-label"
                  label="Authorization Type"
                  value={workAuthForm.authorizationType}
                  onChange={(e) =>
                    setWorkAuthForm({
                      ...workAuthForm,
                      authorizationType: Number(e.target.value) as WorkAuthorizationType,
                    })
                  }
                >
                  <MenuItem value={WorkAuthorizationType.Citizen}>Citizen</MenuItem>
                  <MenuItem value={WorkAuthorizationType.PermanentResident}>Permanent Resident (Green Card)</MenuItem>
                  <MenuItem value={WorkAuthorizationType.WorkVisa}>Work Visa (H-1B, L-1, TN, etc.)</MenuItem>
                  <MenuItem value={WorkAuthorizationType.StudentOptCpt}>Student (OPT / CPT)</MenuItem>
                  <MenuItem value={WorkAuthorizationType.RequiresSponsorship}>Requires Sponsorship</MenuItem>
                  <MenuItem value={WorkAuthorizationType.FreelanceContractor}>Contractor / Freelance</MenuItem>
                  <MenuItem value={WorkAuthorizationType.Other}>Other</MenuItem>
                </Select>
              </FormControl>
            </Grid>

            <Grid item xs={12}>
              <TextField
                fullWidth
                label="Visa Category / Specific Type (e.g. H-1B, TN-1, O-1)"
                value={workAuthForm.visaType}
                onChange={(e) => setWorkAuthForm({ ...workAuthForm, visaType: e.target.value })}
              />
            </Grid>

            <Grid item xs={12} sm={6}>
              <TextField
                fullWidth
                type="date"
                label="Valid From"
                InputLabelProps={{ shrink: true }}
                value={workAuthForm.validFrom}
                onChange={(e) => setWorkAuthForm({ ...workAuthForm, validFrom: e.target.value })}
              />
            </Grid>

            <Grid item xs={12} sm={6}>
              <TextField
                fullWidth
                type="date"
                label="Valid To / Expiration"
                InputLabelProps={{ shrink: true }}
                value={workAuthForm.validTo}
                onChange={(e) => setWorkAuthForm({ ...workAuthForm, validTo: e.target.value })}
              />
            </Grid>

            <Grid item xs={12}>
              <FormControlLabel
                control={
                  <Switch
                    checked={workAuthForm.requiresSponsorship}
                    onChange={(e) => setWorkAuthForm({ ...workAuthForm, requiresSponsorship: e.target.checked })}
                    color="warning"
                  />
                }
                label="Requires Future Employer Sponsorship (now or in future)"
              />
            </Grid>

            <Grid item xs={12}>
              <TextField
                fullWidth
                multiline
                rows={2}
                label="Notes (Optional)"
                value={workAuthForm.notes}
                onChange={(e) => setWorkAuthForm({ ...workAuthForm, notes: e.target.value })}
              />
            </Grid>
          </Grid>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setWorkAuthDialogOpen(false)}>Cancel</Button>
          <Button variant="contained" onClick={handleSaveWorkAuth}>
            Save
          </Button>
        </DialogActions>
      </Dialog>

      {/* DIALOG 3: Add/Edit Address */}
      <Dialog
        open={addressDialogOpen}
        onClose={() => setAddressDialogOpen(false)}
        maxWidth="md"
        fullWidth
      >
        <DialogTitle>{editingAddressId ? 'Edit Address' : 'Add Address'}</DialogTitle>
        <DialogContent dividers>
          <Grid container spacing={2} sx={{ mt: 0.5 }}>
            <Grid item xs={12} sm={6}>
              <FormControl fullWidth required>
                <InputLabel id="address-type-label">Address Type</InputLabel>
                <Select
                  labelId="address-type-label"
                  label="Address Type"
                  value={addressForm.addressType}
                  onChange={(e) =>
                    setAddressForm({
                      ...addressForm,
                      addressType: Number(e.target.value) as AddressType,
                    })
                  }
                >
                  <MenuItem value={AddressType.CurrentHome}>Current Home</MenuItem>
                  <MenuItem value={AddressType.Mailing}>Mailing Address</MenuItem>
                  <MenuItem value={AddressType.RelocationTarget}>Relocation Target</MenuItem>
                  <MenuItem value={AddressType.Previous}>Previous Address</MenuItem>
                </Select>
              </FormControl>
            </Grid>

            <Grid item xs={12} sm={6}>
              <FormControl fullWidth required>
                <InputLabel id="addr-country-label">Country</InputLabel>
                <Select
                  labelId="addr-country-label"
                  label="Country"
                  value={addressForm.countryId}
                  onChange={(e) => handleAddressCountryChange(e.target.value)}
                >
                  {countries.map((c) => (
                    <MenuItem key={c.id} value={c.id}>
                      {c.name} ({c.code})
                    </MenuItem>
                  ))}
                </Select>
              </FormControl>
            </Grid>

            <Grid item xs={12} sm={8}>
              <TextField
                fullWidth
                required
                label="Street Address 1"
                value={addressForm.street1}
                onChange={(e) => setAddressForm({ ...addressForm, street1: e.target.value })}
              />
            </Grid>

            <Grid item xs={12} sm={4}>
              <TextField
                fullWidth
                label="Street Address 2 (Apt, Suite)"
                value={addressForm.street2}
                onChange={(e) => setAddressForm({ ...addressForm, street2: e.target.value })}
              />
            </Grid>

            <Grid item xs={12} sm={4}>
              <TextField
                fullWidth
                required
                label="City"
                value={addressForm.city}
                onChange={(e) => setAddressForm({ ...addressForm, city: e.target.value })}
              />
            </Grid>

            <Grid item xs={12} sm={4}>
              {addressRegions.length > 0 ? (
                <FormControl fullWidth>
                  <InputLabel id="region-select-label">State / Region</InputLabel>
                  <Select
                    labelId="region-select-label"
                    label="State / Region"
                    value={addressForm.regionId || ''}
                    onChange={(e) => {
                      const regId = e.target.value
                      const selectedReg = addressRegions.find((r) => r.id === regId)
                      setAddressForm({
                        ...addressForm,
                        regionId: regId || null,
                        stateProvince: selectedReg ? selectedReg.name : addressForm.stateProvince,
                      })
                    }}
                  >
                    <MenuItem value="">None / Custom</MenuItem>
                    {addressRegions.map((r) => (
                      <MenuItem key={r.id} value={r.id}>
                        {r.name} ({r.code})
                      </MenuItem>
                    ))}
                  </Select>
                </FormControl>
              ) : (
                <TextField
                  fullWidth
                  label="State / Province"
                  value={addressForm.stateProvince}
                  onChange={(e) => setAddressForm({ ...addressForm, stateProvince: e.target.value })}
                />
              )}
            </Grid>

            <Grid item xs={12} sm={4}>
              <TextField
                fullWidth
                label="Postal / Zip Code"
                value={addressForm.postalCode}
                onChange={(e) => setAddressForm({ ...addressForm, postalCode: e.target.value })}
              />
            </Grid>

            <Grid item xs={12}>
              <Divider sx={{ my: 1 }} />
              <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1 }}>
                <FormControlLabel
                  control={
                    <Checkbox
                      checked={addressForm.isPrimary}
                      onChange={(e) => setAddressForm({ ...addressForm, isPrimary: e.target.checked })}
                    />
                  }
                  label="Set as Primary Address"
                />
                <FormControlLabel
                  control={
                    <Checkbox
                      checked={addressForm.openToRemote}
                      onChange={(e) => setAddressForm({ ...addressForm, openToRemote: e.target.checked })}
                    />
                  }
                  label="Open to Remote Work from this location"
                />
                <FormControlLabel
                  control={
                    <Checkbox
                      checked={addressForm.willingToRelocate}
                      onChange={(e) => setAddressForm({ ...addressForm, willingToRelocate: e.target.checked })}
                    />
                  }
                  label="Willing to Relocate from/to this area"
                />
              </Box>
            </Grid>
          </Grid>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setAddressDialogOpen(false)}>Cancel</Button>
          <Button variant="contained" onClick={handleSaveAddress}>
            Save
          </Button>
        </DialogActions>
      </Dialog>

      {/* DIALOG 4: Add/Edit Social Link */}
      <Dialog
        open={socialDialogOpen}
        onClose={() => setSocialDialogOpen(false)}
        maxWidth="sm"
        fullWidth
      >
        <DialogTitle>{editingSocialId ? 'Edit Social Link' : 'Add Social Link'}</DialogTitle>
        <DialogContent dividers>
          <Grid container spacing={2} sx={{ mt: 0.5 }}>
            <Grid item xs={12}>
              <FormControl fullWidth required>
                <InputLabel id="social-platform-label">Platform</InputLabel>
                <Select
                  labelId="social-platform-label"
                  label="Platform"
                  value={socialForm.platform}
                  onChange={(e) =>
                    setSocialForm({
                      ...socialForm,
                      platform: Number(e.target.value) as SocialPlatform,
                    })
                  }
                >
                  <MenuItem value={SocialPlatform.LinkedIn}>LinkedIn</MenuItem>
                  <MenuItem value={SocialPlatform.GitHub}>GitHub</MenuItem>
                  <MenuItem value={SocialPlatform.PersonalWebsite}>Personal Website / Portfolio</MenuItem>
                  <MenuItem value={SocialPlatform.TwitterX}>X / Twitter</MenuItem>
                  <MenuItem value={SocialPlatform.StackOverflow}>Stack Overflow</MenuItem>
                  <MenuItem value={SocialPlatform.MediumSubstack}>Medium / Substack</MenuItem>
                  <MenuItem value={SocialPlatform.YouTube}>YouTube</MenuItem>
                  <MenuItem value={SocialPlatform.Other}>Other</MenuItem>
                </Select>
              </FormControl>
            </Grid>

            <Grid item xs={12}>
              <TextField
                fullWidth
                required
                label="Profile / Website URL"
                placeholder="https://linkedin.com/in/username"
                value={socialForm.url}
                onChange={(e) => setSocialForm({ ...socialForm, url: e.target.value })}
              />
            </Grid>

            <Grid item xs={12} sm={8}>
              <TextField
                fullWidth
                label="Display Label (e.g. My Tech Blog, GitHub Profile)"
                value={socialForm.displayLabel}
                onChange={(e) => setSocialForm({ ...socialForm, displayLabel: e.target.value })}
              />
            </Grid>

            <Grid item xs={12} sm={4}>
              <TextField
                fullWidth
                type="number"
                label="Display Order"
                value={socialForm.displayOrder}
                onChange={(e) => setSocialForm({ ...socialForm, displayOrder: Number(e.target.value) })}
              />
            </Grid>
          </Grid>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setSocialDialogOpen(false)}>Cancel</Button>
          <Button variant="contained" onClick={handleSaveSocial}>
            Save
          </Button>
        </DialogActions>
      </Dialog>

      {/* Global Feedback Snackbar */}
      <Snackbar
        open={snackbar.open}
        autoHideDuration={4000}
        onClose={() => setSnackbar((prev) => ({ ...prev, open: false }))}
        anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }}
      >
        <Alert
          severity={snackbar.severity}
          onClose={() => setSnackbar((prev) => ({ ...prev, open: false }))}
          variant="filled"
          sx={{ width: '100%' }}
        >
          {snackbar.message}
        </Alert>
      </Snackbar>
    </Box>
  )
}
