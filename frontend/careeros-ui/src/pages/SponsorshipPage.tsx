import React, { useEffect, useState } from 'react'
import {
  Box, Typography, Paper, Table, TableBody, TableCell, TableContainer,
  TableHead, TableRow, IconButton, Dialog, DialogTitle, DialogContent,
  DialogActions, Button, TextField, CircularProgress, Alert, Grid, Card,
  CardContent, Chip, InputAdornment, FormControl, InputLabel,
  Select, MenuItem, ToggleButtonGroup, ToggleButton, Divider, Stack,
  LinearProgress, Checkbox, FormControlLabel, Switch
} from '@mui/material'
import AddIcon from '@mui/icons-material/Add'
import DeleteIcon from '@mui/icons-material/Delete'
import EditIcon from '@mui/icons-material/Edit'
import SearchIcon from '@mui/icons-material/Search'
import PublicIcon from '@mui/icons-material/Public'
import FlightTakeoffIcon from '@mui/icons-material/FlightTakeoff'
import CheckCircleIcon from '@mui/icons-material/CheckCircle'
import WarningIcon from '@mui/icons-material/Warning'
import HourglassEmptyIcon from '@mui/icons-material/HourglassEmpty'
import BlockIcon from '@mui/icons-material/Block'
import VerifiedUserIcon from '@mui/icons-material/VerifiedUser'
import ViewModuleIcon from '@mui/icons-material/ViewModule'
import ViewListIcon from '@mui/icons-material/ViewList'
import AttachFileIcon from '@mui/icons-material/AttachFile'
import BusinessIcon from '@mui/icons-material/Business'
import WorkIcon from '@mui/icons-material/Work'
import EventIcon from '@mui/icons-material/Event'
import MonetizationOnIcon from '@mui/icons-material/MonetizationOn'
import AccessTimeIcon from '@mui/icons-material/AccessTime'
import AutoAwesomeIcon from '@mui/icons-material/AutoAwesome'

import {
  sponsorshipApi, VisaProgramType, VisaProgramLabels,
  SponsorshipEligibilityStatus, SponsorshipEligibilityLabels,
  VisaAvailabilityStatus, VisaAvailabilityLabels
} from '../api/sponsorship'
import type {
  SponsorshipCase, SponsorshipStats, CreateSponsorshipCaseRequest,
  UpdateSponsorshipCaseRequest, AddSponsorshipEvidenceRequest
} from '../api/sponsorship'
import { countriesApi } from '../api/countries'
import type { Country } from '../api/countries'
import { companiesApi } from '../api/companies'
import type { Company } from '../api/companies'
import { jobsApi } from '../api/jobs'
import type { Job } from '../api/jobs'
import { documentsApi } from '../api/documents'
import type { Document as VaultDocument } from '../api/documents'

// UI Chip configurations
const ELIGIBILITY_CONFIG: Record<number, { label: string; color: 'success' | 'warning' | 'info' | 'error' | 'default'; icon: React.ReactElement }> = {
  [SponsorshipEligibilityStatus.Eligible]: { label: 'Eligible', color: 'success', icon: <CheckCircleIcon fontSize="small" /> },
  [SponsorshipEligibilityStatus.ConditionallyEligible]: { label: 'Conditionally Eligible', color: 'warning', icon: <WarningIcon fontSize="small" /> },
  [SponsorshipEligibilityStatus.PendingReview]: { label: 'Pending Review', color: 'info', icon: <HourglassEmptyIcon fontSize="small" /> },
  [SponsorshipEligibilityStatus.Ineligible]: { label: 'Ineligible', color: 'error', icon: <BlockIcon fontSize="small" /> },
  [SponsorshipEligibilityStatus.Sponsored]: { label: 'Sponsored / Approved', color: 'success', icon: <VerifiedUserIcon fontSize="small" /> },
}

const AVAILABILITY_CONFIG: Record<number, { label: string; color: 'success' | 'warning' | 'secondary' | 'error' | 'default' }> = {
  [VisaAvailabilityStatus.OpenYearRound]: { label: 'Open Year-Round', color: 'success' },
  [VisaAvailabilityStatus.CapSubjectLottery]: { label: 'Annual Lottery', color: 'warning' },
  [VisaAvailabilityStatus.QuotaWaitlist]: { label: 'Quota Waitlist', color: 'error' },
  [VisaAvailabilityStatus.ExpeditedProcessing]: { label: 'Expedited / Fast-Track', color: 'secondary' },
  [VisaAvailabilityStatus.RestrictedOrPaused]: { label: 'Restricted / Paused', color: 'default' },
}

// Preset configurations for instant pre-population
const PRESETS = [
  {
    label: 'US H-1B (Specialty Occupation)',
    countryCode: 'US',
    countryName: 'United States',
    programName: 'H-1B Specialty Occupation',
    programType: VisaProgramType.H1B,
    eligibility: SponsorshipEligibilityStatus.Eligible,
    availability: VisaAvailabilityStatus.CapSubjectLottery,
    currency: 'USD',
    minimumSalaryRequirement: 95000,
    estimatedProcessingMonths: 4,
    supportsDualIntent: true,
    leadsToPermanentResidency: true,
    notes: 'Annual statutory cap (65k regular + 20k US master’s). Registration in March, lottery selection in April, filings start April 1st.',
  },
  {
    label: 'Canada Global Talent Stream (GTS)',
    countryCode: 'CA',
    countryName: 'Canada',
    programName: 'Global Talent Stream (LMIA Fast-Track)',
    programType: VisaProgramType.LMIA_GTS,
    eligibility: SponsorshipEligibilityStatus.Eligible,
    availability: VisaAvailabilityStatus.ExpeditedProcessing,
    currency: 'CAD',
    minimumSalaryRequirement: 110000,
    estimatedProcessingMonths: 1,
    supportsDualIntent: true,
    leadsToPermanentResidency: true,
    notes: '2-week standard processing for work permit with LMIA Category B high-skill software architecture roles. Leads to Express Entry PR.',
  },
  {
    label: 'Germany / EU Blue Card',
    countryCode: 'DE',
    countryName: 'Germany',
    programName: 'EU Blue Card (High-Skill Tech)',
    programType: VisaProgramType.EUBlueCard,
    eligibility: SponsorshipEligibilityStatus.Eligible,
    availability: VisaAvailabilityStatus.OpenYearRound,
    currency: 'EUR',
    minimumSalaryRequirement: 45300,
    estimatedProcessingMonths: 2,
    supportsDualIntent: true,
    leadsToPermanentResidency: true,
    notes: 'Open year-round with recognized STEM university degree. Settlement permit (Permanent Residence) obtainable after 21-27 months with B1/A1 German.',
  },
  {
    label: 'Australia TSS 482 (Medium-Term)',
    countryCode: 'AU',
    countryName: 'Australia',
    programName: 'Temporary Skill Shortage (Subclass 482)',
    programType: VisaProgramType.TSS_482,
    eligibility: SponsorshipEligibilityStatus.Eligible,
    availability: VisaAvailabilityStatus.OpenYearRound,
    currency: 'AUD',
    minimumSalaryRequirement: 85000,
    estimatedProcessingMonths: 3,
    supportsDualIntent: true,
    leadsToPermanentResidency: true,
    notes: 'Medium-term stream (Software & Applications Programmer). Up to 4-year validity with direct transition pathway to ENS 186 Permanent Residency after 2 years.',
  },
  {
    label: 'UK Skilled Worker Visa',
    countryCode: 'GB',
    countryName: 'United Kingdom',
    programName: 'Skilled Worker Visa',
    programType: VisaProgramType.UK_SkilledWorker,
    eligibility: SponsorshipEligibilityStatus.Eligible,
    availability: VisaAvailabilityStatus.OpenYearRound,
    currency: 'GBP',
    minimumSalaryRequirement: 38700,
    estimatedProcessingMonths: 2,
    supportsDualIntent: true,
    leadsToPermanentResidency: true,
    notes: 'Sponsoring employer must hold valid Home Office Sponsor License. Indefinite Leave to Remain (ILR settlement) available after 5 continuous years.',
  },
]

export const SponsorshipPage: React.FC = () => {
  const [cases, setCases] = useState<SponsorshipCase[]>([])
  const [stats, setStats] = useState<SponsorshipStats | null>(null)
  const [countries, setCountries] = useState<Country[]>([])
  const [companies, setCompanies] = useState<Company[]>([])
  const [jobs, setJobs] = useState<Job[]>([])
  const [vaultDocuments, setVaultDocuments] = useState<VaultDocument[]>([])

  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [successMessage, setSuccessMessage] = useState<string | null>(null)

  // Filters & Layout State
  const [viewMode, setViewMode] = useState<'cards' | 'table'>('cards')
  const [searchTerm, setSearchTerm] = useState('')
  const [selectedCountryId, setSelectedCountryId] = useState<string>('')
  const [selectedProgramType, setSelectedProgramType] = useState<number | ''>('')
  const [selectedEligibility, setSelectedEligibility] = useState<number | ''>('')
  const [selectedAvailability, setSelectedAvailability] = useState<number | ''>('')

  // Dialogs
  const [openCaseDialog, setOpenCaseDialog] = useState(false)
  const [editingCase, setEditingCase] = useState<SponsorshipCase | null>(null)

  // Evidence Dialog
  const [openEvidenceDialog, setOpenEvidenceDialog] = useState(false)
  const [selectedCaseForEvidence, setSelectedCaseForEvidence] = useState<SponsorshipCase | null>(null)
  const [evidenceType, setEvidenceType] = useState('Degree Certificate')
  const [evidenceDescription, setEvidenceDescription] = useState('')
  const [evidenceDocumentId, setEvidenceDocumentId] = useState<string>('')
  const [evidenceIsVerified, setEvidenceIsVerified] = useState(false)
  const [evidenceExpiryDate, setEvidenceExpiryDate] = useState('')

  // Case Form State
  const [formCountryId, setFormCountryId] = useState<string>('')
  const [formProgramName, setFormProgramName] = useState('')
  const [formProgramType, setFormProgramType] = useState<number>(VisaProgramType.H1B)
  const [formEligibility, setFormEligibility] = useState<number>(SponsorshipEligibilityStatus.PendingReview)
  const [formAvailability, setFormAvailability] = useState<number>(VisaAvailabilityStatus.OpenYearRound)
  const [formCompanyId, setFormCompanyId] = useState<string>('')
  const [formJobId, setFormJobId] = useState<string>('')
  const [formMinSalary, setFormMinSalary] = useState<number | ''>('')
  const [formCurrency, setFormCurrency] = useState('USD')
  const [formNextCycleDate, setFormNextCycleDate] = useState('')
  const [formFilingDeadline, setFormFilingDeadline] = useState('')
  const [formProcessingMonths, setFormProcessingMonths] = useState<number | ''>(3)
  const [formSupportsDualIntent, setFormSupportsDualIntent] = useState(false)
  const [formLeadsToPR, setFormLeadsToPR] = useState(false)
  const [formNotes, setFormNotes] = useState('')

  const loadData = async () => {
    try {
      setLoading(true)
      const [cs, st, cntrs, comps, jbs, docs] = await Promise.all([
        sponsorshipApi.getAll({
          countryId: selectedCountryId || undefined,
          programType: selectedProgramType !== '' ? Number(selectedProgramType) : undefined,
          eligibility: selectedEligibility !== '' ? Number(selectedEligibility) : undefined,
          availability: selectedAvailability !== '' ? Number(selectedAvailability) : undefined,
          searchTerm: searchTerm || undefined,
        }),
        sponsorshipApi.getStats(),
        countriesApi.getAll(),
        companiesApi.getAll(),
        jobsApi.getAll(),
        documentsApi.getAll(),
      ])

      setCases(cs)
      setStats(st)
      setCountries(cntrs)
      setCompanies(comps)
      setJobs(jbs)
      setVaultDocuments(docs)
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Failed to load sponsorship data')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    loadData()
  }, [selectedCountryId, selectedProgramType, selectedEligibility, selectedAvailability, searchTerm])

  const handleOpenCreate = () => {
    setEditingCase(null)
    setFormCountryId(countries.length > 0 ? countries[0].id : '')
    setFormProgramName('')
    setFormProgramType(VisaProgramType.H1B)
    setFormEligibility(SponsorshipEligibilityStatus.Eligible)
    setFormAvailability(VisaAvailabilityStatus.OpenYearRound)
    setFormCompanyId('')
    setFormJobId('')
    setFormMinSalary('')
    setFormCurrency('USD')
    setFormNextCycleDate('')
    setFormFilingDeadline('')
    setFormProcessingMonths(3)
    setFormSupportsDualIntent(true)
    setFormLeadsToPR(true)
    setFormNotes('')
    setOpenCaseDialog(true)
  }

  const handleApplyPreset = (preset: typeof PRESETS[0]) => {
    const matchedCountry = countries.find(
      (c) => c.code.toLowerCase() === preset.countryCode.toLowerCase() ||
             c.name.toLowerCase().includes(preset.countryName.toLowerCase())
    )
    if (matchedCountry) {
      setFormCountryId(matchedCountry.id)
    }
    setFormProgramName(preset.programName)
    setFormProgramType(preset.programType)
    setFormEligibility(preset.eligibility)
    setFormAvailability(preset.availability)
    setFormCurrency(preset.currency)
    setFormMinSalary(preset.minimumSalaryRequirement)
    setFormProcessingMonths(preset.estimatedProcessingMonths)
    setFormSupportsDualIntent(preset.supportsDualIntent)
    setFormLeadsToPR(preset.leadsToPermanentResidency)
    setFormNotes(preset.notes)
  }

  const handleOpenEdit = (sc: SponsorshipCase) => {
    setEditingCase(sc)
    setFormCountryId(sc.countryId)
    setFormProgramName(sc.programName)
    setFormProgramType(sc.programType)
    setFormEligibility(sc.eligibility)
    setFormAvailability(sc.availability)
    setFormCompanyId(sc.companyId || '')
    setFormJobId(sc.jobId || '')
    setFormMinSalary(sc.minimumSalaryRequirement ?? '')
    setFormCurrency(sc.currency || 'USD')
    setFormNextCycleDate(sc.nextFiscalCycleDate ? sc.nextFiscalCycleDate.substring(0, 10) : '')
    setFormFilingDeadline(sc.filingDeadline ? sc.filingDeadline.substring(0, 10) : '')
    setFormProcessingMonths(sc.estimatedProcessingMonths ?? '')
    setFormSupportsDualIntent(sc.supportsDualIntent)
    setFormLeadsToPR(sc.leadsToPermanentResidency)
    setFormNotes(sc.notes || '')
    setOpenCaseDialog(true)
  }

  const handleDeleteCase = async (id: string) => {
    if (!window.confirm('Are you sure you want to delete this sponsorship tracking case?')) return
    try {
      await sponsorshipApi.delete(id)
      setSuccessMessage('Sponsorship case removed successfully.')
      loadData()
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Failed to delete case')
    }
  }

  const handleSubmitCase = async () => {
    if (!formProgramName.trim()) {
      setError('Program Name is required')
      return
    }
    if (!formCountryId) {
      setError('Country is required')
      return
    }

    try {
      if (editingCase) {
        const updateReq: UpdateSponsorshipCaseRequest = {
          id: editingCase.id,
          countryId: formCountryId,
          programName: formProgramName,
          programType: formProgramType,
          eligibility: formEligibility,
          availability: formAvailability,
          companyId: formCompanyId || undefined,
          jobId: formJobId || undefined,
          minimumSalaryRequirement: formMinSalary !== '' ? Number(formMinSalary) : undefined,
          currency: formCurrency || undefined,
          nextFiscalCycleDate: formNextCycleDate ? new Date(formNextCycleDate).toISOString() : undefined,
          filingDeadline: formFilingDeadline ? new Date(formFilingDeadline).toISOString() : undefined,
          estimatedProcessingMonths: formProcessingMonths !== '' ? Number(formProcessingMonths) : undefined,
          supportsDualIntent: formSupportsDualIntent,
          leadsToPermanentResidency: formLeadsToPR,
          notes: formNotes || undefined,
        }
        await sponsorshipApi.update(editingCase.id, updateReq)
        setSuccessMessage('Sponsorship case updated successfully!')
      } else {
        const createReq: CreateSponsorshipCaseRequest = {
          countryId: formCountryId,
          programName: formProgramName,
          programType: formProgramType,
          eligibility: formEligibility,
          availability: formAvailability,
          companyId: formCompanyId || undefined,
          jobId: formJobId || undefined,
          minimumSalaryRequirement: formMinSalary !== '' ? Number(formMinSalary) : undefined,
          currency: formCurrency || undefined,
          nextFiscalCycleDate: formNextCycleDate ? new Date(formNextCycleDate).toISOString() : undefined,
          filingDeadline: formFilingDeadline ? new Date(formFilingDeadline).toISOString() : undefined,
          estimatedProcessingMonths: formProcessingMonths !== '' ? Number(formProcessingMonths) : undefined,
          supportsDualIntent: formSupportsDualIntent,
          leadsToPermanentResidency: formLeadsToPR,
          notes: formNotes || undefined,
        }
        await sponsorshipApi.create(createReq)
        setSuccessMessage('Sponsorship case created successfully!')
      }

      setOpenCaseDialog(false)
      loadData()
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Failed to save sponsorship case')
    }
  }

  // Evidence Management
  const handleOpenEvidenceManager = (sc: SponsorshipCase) => {
    setSelectedCaseForEvidence(sc)
    setEvidenceType('Degree Certificate')
    setEvidenceDescription('')
    setEvidenceDocumentId('')
    setEvidenceIsVerified(false)
    setEvidenceExpiryDate('')
    setOpenEvidenceDialog(true)
  }

  const handleAddEvidence = async () => {
    if (!selectedCaseForEvidence || !evidenceType.trim()) return

    try {
      const req: AddSponsorshipEvidenceRequest = {
        sponsorshipCaseId: selectedCaseForEvidence.id,
        evidenceType: evidenceType.trim(),
        description: evidenceDescription.trim(),
        documentId: evidenceDocumentId || undefined,
        isVerified: evidenceIsVerified,
        expiryDate: evidenceExpiryDate ? new Date(evidenceExpiryDate).toISOString() : undefined,
      }
      await sponsorshipApi.addEvidence(selectedCaseForEvidence.id, req)
      setSuccessMessage('Evidence attached successfully!')
      const updatedCase = await sponsorshipApi.getById(selectedCaseForEvidence.id)
      setSelectedCaseForEvidence(updatedCase)
      loadData()
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Failed to add evidence')
    }
  }

  const handleToggleEvidenceVerified = async (evidenceId: string, current: boolean) => {
    if (!selectedCaseForEvidence) return
    const ev = selectedCaseForEvidence.evidenceList.find((e) => e.id === evidenceId)
    if (!ev) return

    try {
      await sponsorshipApi.updateEvidence(selectedCaseForEvidence.id, evidenceId, {
        id: evidenceId,
        evidenceType: ev.evidenceType,
        description: ev.description,
        documentId: ev.documentId,
        isVerified: !current,
        expiryDate: ev.expiryDate,
      })
      const updatedCase = await sponsorshipApi.getById(selectedCaseForEvidence.id)
      setSelectedCaseForEvidence(updatedCase)
      loadData()
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Failed to toggle evidence status')
    }
  }

  const handleRemoveEvidence = async (evidenceId: string) => {
    if (!selectedCaseForEvidence) return
    try {
      await sponsorshipApi.removeEvidence(selectedCaseForEvidence.id, evidenceId)
      setSuccessMessage('Evidence removed.')
      const updatedCase = await sponsorshipApi.getById(selectedCaseForEvidence.id)
      setSelectedCaseForEvidence(updatedCase)
      loadData()
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Failed to remove evidence')
    }
  }

  return (
    <Box sx={{ p: 3, maxWidth: 1600, margin: '0 auto' }}>
      {/* Header and Actions */}
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Box>
          <Typography variant="h4" fontWeight="bold" sx={{ display: 'flex', alignItems: 'center', gap: 1.5 }}>
            <FlightTakeoffIcon fontSize="large" color="primary" />
            Sponsorship Center
          </Typography>
          <Typography variant="subtitle1" color="text.secondary">
            Global visa opportunity pipeline, statutory quotas, salary thresholds, and credential evidence checklist.
          </Typography>
        </Box>
        <Stack direction="row" spacing={1.5}>
          <ToggleButtonGroup
            value={viewMode}
            exclusive
            onChange={(_, val) => val && setViewMode(val)}
            size="small"
          >
            <ToggleButton value="cards">
              <ViewModuleIcon fontSize="small" sx={{ mr: 0.5 }} /> Cards
            </ToggleButton>
            <ToggleButton value="table">
              <ViewListIcon fontSize="small" sx={{ mr: 0.5 }} /> Table
            </ToggleButton>
          </ToggleButtonGroup>

          <Button
            variant="contained"
            color="primary"
            startIcon={<AddIcon />}
            onClick={handleOpenCreate}
            sx={{ px: 2.5, py: 1 }}
          >
            New Visa Pathway
          </Button>
        </Stack>
      </Box>

      {/* Alerts */}
      {error && (
        <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError(null)}>
          {error}
        </Alert>
      )}
      {successMessage && (
        <Alert severity="success" sx={{ mb: 2 }} onClose={() => setSuccessMessage(null)}>
          {successMessage}
        </Alert>
      )}

      {/* KPI Stats Cards */}
      {stats && (
        <Grid container spacing={2} sx={{ mb: 3 }}>
          <Grid item xs={6} sm={4} md={2}>
            <Card sx={{ bgcolor: 'background.paper', border: '1px solid', borderColor: 'divider' }}>
              <CardContent sx={{ py: 1.5, px: 2, '&:last-child': { pb: 1.5 } }}>
                <Typography variant="caption" color="text.secondary" fontWeight="bold">
                  TOTAL PATHWAYS
                </Typography>
                <Typography variant="h5" fontWeight="bold">
                  {stats.totalCases}
                </Typography>
              </CardContent>
            </Card>
          </Grid>

          <Grid item xs={6} sm={4} md={2}>
            <Card sx={{ bgcolor: 'success.light', color: 'success.contrastText' }}>
              <CardContent sx={{ py: 1.5, px: 2, '&:last-child': { pb: 1.5 } }}>
                <Typography variant="caption" fontWeight="bold">
                  ELIGIBLE NOW
                </Typography>
                <Typography variant="h5" fontWeight="bold">
                  {stats.eligibleCount}
                </Typography>
              </CardContent>
            </Card>
          </Grid>

          <Grid item xs={6} sm={4} md={2}>
            <Card sx={{ bgcolor: 'info.light', color: 'info.contrastText' }}>
              <CardContent sx={{ py: 1.5, px: 2, '&:last-child': { pb: 1.5 } }}>
                <Typography variant="caption" fontWeight="bold">
                  YEAR-ROUND OPEN
                </Typography>
                <Typography variant="h5" fontWeight="bold">
                  {stats.openYearRoundCount}
                </Typography>
              </CardContent>
            </Card>
          </Grid>

          <Grid item xs={6} sm={4} md={2}>
            <Card sx={{ bgcolor: 'warning.light', color: 'warning.contrastText' }}>
              <CardContent sx={{ py: 1.5, px: 2, '&:last-child': { pb: 1.5 } }}>
                <Typography variant="caption" fontWeight="bold">
                  LOTTERY / CAP-SUBJECT
                </Typography>
                <Typography variant="h5" fontWeight="bold">
                  {stats.capSubjectLotteryCount}
                </Typography>
              </CardContent>
            </Card>
          </Grid>

          <Grid item xs={6} sm={4} md={2}>
            <Card sx={{ bgcolor: 'secondary.light', color: 'secondary.contrastText' }}>
              <CardContent sx={{ py: 1.5, px: 2, '&:last-child': { pb: 1.5 } }}>
                <Typography variant="caption" fontWeight="bold">
                  LEADS TO PR / GC
                </Typography>
                <Typography variant="h5" fontWeight="bold">
                  {stats.leadsToPRCount}
                </Typography>
              </CardContent>
            </Card>
          </Grid>

          <Grid item xs={6} sm={4} md={2}>
            <Card sx={{ bgcolor: 'background.paper', border: '1px solid', borderColor: 'divider' }}>
              <CardContent sx={{ py: 1.5, px: 2, '&:last-child': { pb: 1.5 } }}>
                <Typography variant="caption" color="text.secondary" fontWeight="bold">
                  VERIFIED EVIDENCE
                </Typography>
                <Typography variant="h5" fontWeight="bold">
                  ✓ {stats.verifiedEvidenceCount} docs
                </Typography>
              </CardContent>
            </Card>
          </Grid>
        </Grid>
      )}

      {/* Filter Toolbar */}
      <Paper sx={{ p: 2, mb: 3, border: '1px solid', borderColor: 'divider' }}>
        <Grid container spacing={2} alignItems="center">
          <Grid item xs={12} sm={6} md={3}>
            <TextField
              fullWidth
              size="small"
              placeholder="Search program, country, notes..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              InputProps={{
                startAdornment: (
                  <InputAdornment position="start">
                    <SearchIcon fontSize="small" />
                  </InputAdornment>
                ),
              }}
            />
          </Grid>

          <Grid item xs={6} sm={3} md={2}>
            <FormControl fullWidth size="small">
              <InputLabel>Country</InputLabel>
              <Select
                value={selectedCountryId}
                label="Country"
                onChange={(e) => setSelectedCountryId(e.target.value)}
              >
                <MenuItem value="">All Countries</MenuItem>
                {countries.map((c) => (
                  <MenuItem key={c.id} value={c.id}>
                    {c.name} ({c.code})
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
          </Grid>

          <Grid item xs={6} sm={3} md={2.5}>
            <FormControl fullWidth size="small">
              <InputLabel>Program Type</InputLabel>
              <Select
                value={selectedProgramType}
                label="Program Type"
                onChange={(e) => setSelectedProgramType(e.target.value as number | '')}
              >
                <MenuItem value="">All Visa Types</MenuItem>
                {Object.entries(VisaProgramLabels).map(([key, label]) => (
                  <MenuItem key={key} value={Number(key)}>
                    {label}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
          </Grid>

          <Grid item xs={6} sm={3} md={2}>
            <FormControl fullWidth size="small">
              <InputLabel>Eligibility</InputLabel>
              <Select
                value={selectedEligibility}
                label="Eligibility"
                onChange={(e) => setSelectedEligibility(e.target.value as number | '')}
              >
                <MenuItem value="">All Statuses</MenuItem>
                {Object.entries(SponsorshipEligibilityLabels).map(([key, label]) => (
                  <MenuItem key={key} value={Number(key)}>
                    {label}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
          </Grid>

          <Grid item xs={6} sm={3} md={2.5}>
            <FormControl fullWidth size="small">
              <InputLabel>Availability</InputLabel>
              <Select
                value={selectedAvailability}
                label="Availability"
                onChange={(e) => setSelectedAvailability(e.target.value as number | '')}
              >
                <MenuItem value="">All Availabilities</MenuItem>
                {Object.entries(VisaAvailabilityLabels).map(([key, label]) => (
                  <MenuItem key={key} value={Number(key)}>
                    {label}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
          </Grid>
        </Grid>
      </Paper>

      {/* Main Content Area: Cards vs Table */}
      {loading && cases.length === 0 ? (
        <Box sx={{ display: 'flex', justifyContent: 'center', p: 8 }}>
          <CircularProgress />
        </Box>
      ) : cases.length === 0 ? (
        <Paper sx={{ p: 8, textAlign: 'center', border: '1px solid', borderColor: 'divider' }}>
          <PublicIcon sx={{ fontSize: 60, color: 'text.secondary', mb: 2 }} />
          <Typography variant="h6" color="text.secondary">
            No visa sponsorship pathways tracked yet.
          </Typography>
          <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
            Track international visa programs like H-1B, Canada GTS, EU Blue Card, or Australia TSS.
          </Typography>
          <Button variant="contained" startIcon={<AddIcon />} onClick={handleOpenCreate}>
            Create First Pathway
          </Button>
        </Paper>
      ) : viewMode === 'cards' ? (
        /* Cards View */
        <Grid container spacing={3}>
          {cases.map((sc) => {
            const eligCfg = ELIGIBILITY_CONFIG[sc.eligibility] || ELIGIBILITY_CONFIG[SponsorshipEligibilityStatus.PendingReview]
            const availCfg = AVAILABILITY_CONFIG[sc.availability] || AVAILABILITY_CONFIG[VisaAvailabilityStatus.OpenYearRound]

            const verifiedEvidenceCount = sc.evidenceList.filter((e) => e.isVerified).length
            const totalEvidence = sc.evidenceList.length
            const evidencePct = totalEvidence > 0 ? Math.round((verifiedEvidenceCount / totalEvidence) * 100) : 0

            return (
              <Grid item xs={12} md={6} lg={4} key={sc.id}>
                <Card
                  variant="outlined"
                  sx={{
                    height: '100%',
                    display: 'flex',
                    flexDirection: 'column',
                    justifyContent: 'space-between',
                    transition: 'all 0.2s',
                    '&:hover': { boxShadow: 4, borderColor: 'primary.main' },
                  }}
                >
                  <CardContent sx={{ pb: 1 }}>
                    {/* Header Chips */}
                    <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 1.5 }}>
                      <Chip
                        icon={<PublicIcon fontSize="small" />}
                        label={`${sc.countryName} (${sc.countryCode})`}
                        size="small"
                        color="primary"
                        variant="outlined"
                        sx={{ fontWeight: 'bold' }}
                      />
                      <Chip
                        icon={eligCfg.icon}
                        label={eligCfg.label}
                        size="small"
                        color={eligCfg.color}
                        sx={{ fontWeight: 'bold' }}
                      />
                    </Box>

                    {/* Program Title */}
                    <Typography variant="h6" fontWeight="bold" sx={{ mb: 0.5 }}>
                      {sc.programName}
                    </Typography>

                    <Typography variant="body2" color="text.secondary" sx={{ mb: 1.5 }}>
                      {VisaProgramLabels[sc.programType] || sc.programTypeName}
                    </Typography>

                    {/* Availability Badge */}
                    <Stack direction="row" spacing={1} sx={{ mb: 2 }}>
                      <Chip
                        label={availCfg.label}
                        size="small"
                        color={availCfg.color}
                        variant="outlined"
                      />
                      {sc.leadsToPermanentResidency && (
                        <Chip
                          label="Leads to PR"
                          size="small"
                          color="secondary"
                          sx={{ fontWeight: 600 }}
                        />
                      )}
                      {sc.supportsDualIntent && (
                        <Chip
                          label="Dual Intent"
                          size="small"
                          color="info"
                          variant="outlined"
                        />
                      )}
                    </Stack>

                    <Divider sx={{ my: 1.5 }} />

                    {/* Specifications List */}
                    <Stack spacing={0.75} sx={{ mb: 2 }}>
                      {sc.minimumSalaryRequirement && (
                        <Typography variant="body2" sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                          <MonetizationOnIcon fontSize="small" color="action" />
                          <span>Min Salary: <strong>{sc.currency} {sc.minimumSalaryRequirement.toLocaleString()}</strong></span>
                        </Typography>
                      )}

                      {sc.estimatedProcessingMonths && (
                        <Typography variant="body2" sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                          <AccessTimeIcon fontSize="small" color="action" />
                          <span>Est. Processing: <strong>~{sc.estimatedProcessingMonths} months</strong></span>
                        </Typography>
                      )}

                      {sc.nextFiscalCycleDate && (
                        <Typography variant="body2" sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                          <EventIcon fontSize="small" color="action" />
                          <span>Next Fiscal Window: <strong>{new Date(sc.nextFiscalCycleDate).toLocaleDateString()}</strong></span>
                        </Typography>
                      )}

                      {sc.companyName && (
                        <Typography variant="body2" sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                          <BusinessIcon fontSize="small" color="action" />
                          <span>Sponsor Employer: <strong>{sc.companyName}</strong></span>
                        </Typography>
                      )}

                      {sc.jobTitle && (
                        <Typography variant="body2" sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                          <WorkIcon fontSize="small" color="action" />
                          <span>Associated Job: <strong>{sc.jobTitle}</strong></span>
                        </Typography>
                      )}
                    </Stack>

                    {/* Evidence Checklist Progress */}
                    <Box sx={{ mt: 2, p: 1.5, bgcolor: 'action.hover', borderRadius: 1.5 }}>
                      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 0.5 }}>
                        <Typography variant="caption" fontWeight="bold">
                          EVIDENCE CHECKLIST ({verifiedEvidenceCount}/{totalEvidence})
                        </Typography>
                        <Typography variant="caption" fontWeight="bold" color="primary">
                          {evidencePct}%
                        </Typography>
                      </Box>
                      <LinearProgress
                        variant="determinate"
                        value={evidencePct}
                        color={evidencePct === 100 ? 'success' : 'primary'}
                        sx={{ height: 6, borderRadius: 3 }}
                      />
                    </Box>

                    {sc.notes && (
                      <Typography
                        variant="caption"
                        color="text.secondary"
                        sx={{
                          mt: 1.5,
                          display: '-webkit-box',
                          WebkitLineClamp: 2,
                          WebkitBoxOrient: 'vertical',
                          overflow: 'hidden',
                        }}
                      >
                        {sc.notes}
                      </Typography>
                    )}
                  </CardContent>

                  {/* Card Actions */}
                  <Box sx={{ px: 2, py: 1.5, display: 'flex', justifyContent: 'space-between', borderTop: '1px solid', borderColor: 'divider' }}>
                    <Button
                      size="small"
                      startIcon={<AttachFileIcon />}
                      onClick={() => handleOpenEvidenceManager(sc)}
                    >
                      Evidence ({sc.evidenceList.length})
                    </Button>
                    <Box>
                      <IconButton size="small" onClick={() => handleOpenEdit(sc)} color="primary">
                        <EditIcon fontSize="small" />
                      </IconButton>
                      <IconButton size="small" onClick={() => handleDeleteCase(sc.id)} color="error">
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
        /* Table View */
        <TableContainer component={Paper} sx={{ border: '1px solid', borderColor: 'divider' }}>
          <Table>
            <TableHead>
              <TableRow sx={{ bgcolor: 'action.hover' }}>
                <TableCell><strong>Country</strong></TableCell>
                <TableCell><strong>Program Name & Type</strong></TableCell>
                <TableCell><strong>Eligibility</strong></TableCell>
                <TableCell><strong>Availability</strong></TableCell>
                <TableCell><strong>Min Salary Threshold</strong></TableCell>
                <TableCell><strong>PR / Dual Intent</strong></TableCell>
                <TableCell><strong>Evidence Checklist</strong></TableCell>
                <TableCell align="right"><strong>Actions</strong></TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {cases.map((sc) => {
                const eligCfg = ELIGIBILITY_CONFIG[sc.eligibility] || ELIGIBILITY_CONFIG[SponsorshipEligibilityStatus.PendingReview]
                const availCfg = AVAILABILITY_CONFIG[sc.availability] || AVAILABILITY_CONFIG[VisaAvailabilityStatus.OpenYearRound]
                const verified = sc.evidenceList.filter((e) => e.isVerified).length

                return (
                  <TableRow key={sc.id} hover>
                    <TableCell>
                      <Typography variant="body2" fontWeight="bold">
                        {sc.countryName}
                      </Typography>
                      <Typography variant="caption" color="text.secondary">
                        {sc.countryCode}
                      </Typography>
                    </TableCell>

                    <TableCell>
                      <Typography variant="body2" fontWeight="bold">
                        {sc.programName}
                      </Typography>
                      <Typography variant="caption" color="text.secondary">
                        {VisaProgramLabels[sc.programType] || sc.programTypeName}
                      </Typography>
                      {sc.companyName && (
                        <Typography variant="caption" color="primary" sx={{ display: 'block' }}>
                          🏢 {sc.companyName}
                        </Typography>
                      )}
                    </TableCell>

                    <TableCell>
                      <Chip
                        icon={eligCfg.icon}
                        label={eligCfg.label}
                        size="small"
                        color={eligCfg.color}
                      />
                    </TableCell>

                    <TableCell>
                      <Chip
                        label={availCfg.label}
                        size="small"
                        color={availCfg.color}
                        variant="outlined"
                      />
                    </TableCell>

                    <TableCell>
                      {sc.minimumSalaryRequirement ? (
                        <Typography variant="body2" fontWeight="bold">
                          {sc.currency} {sc.minimumSalaryRequirement.toLocaleString()}
                        </Typography>
                      ) : (
                        <Typography variant="caption" color="text.secondary">—</Typography>
                      )}
                    </TableCell>

                    <TableCell>
                      <Stack direction="row" spacing={0.5}>
                        {sc.leadsToPermanentResidency && (
                          <Chip label="PR" size="small" color="secondary" sx={{ height: 20, fontSize: '0.7rem' }} />
                        )}
                        {sc.supportsDualIntent && (
                          <Chip label="Dual" size="small" variant="outlined" sx={{ height: 20, fontSize: '0.7rem' }} />
                        )}
                      </Stack>
                    </TableCell>

                    <TableCell>
                      <Button
                        size="small"
                        variant="text"
                        startIcon={<AttachFileIcon />}
                        onClick={() => handleOpenEvidenceManager(sc)}
                      >
                        {verified} / {sc.evidenceList.length} verified
                      </Button>
                    </TableCell>

                    <TableCell align="right">
                      <IconButton size="small" onClick={() => handleOpenEdit(sc)} color="primary">
                        <EditIcon fontSize="small" />
                      </IconButton>
                      <IconButton size="small" onClick={() => handleDeleteCase(sc.id)} color="error">
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

      {/* Create / Edit Sponsorship Case Dialog */}
      <Dialog
        open={openCaseDialog}
        onClose={() => setOpenCaseDialog(false)}
        maxWidth="md"
        fullWidth
      >
        <DialogTitle>
          {editingCase ? 'Edit Visa Sponsorship Pathway' : 'Track New Visa Sponsorship Pathway'}
        </DialogTitle>
        <DialogContent dividers>
          {/* Quick Preset Buttons */}
          {!editingCase && (
            <Box sx={{ mb: 2.5, p: 1.5, bgcolor: 'action.hover', borderRadius: 1.5 }}>
              <Typography variant="caption" fontWeight="bold" color="text.secondary" sx={{ display: 'block', mb: 1 }}>
                QUICK VISA PRESETS
              </Typography>
              <Stack direction="row" spacing={1} flexWrap="wrap" useFlexGap>
                {PRESETS.map((p) => (
                  <Button
                    key={p.label}
                    size="small"
                    variant="outlined"
                    startIcon={<AutoAwesomeIcon />}
                    onClick={() => handleApplyPreset(p)}
                  >
                    {p.label}
                  </Button>
                ))}
              </Stack>
            </Box>
          )}

          <Grid container spacing={2}>
            <Grid item xs={12} sm={6}>
              <FormControl fullWidth required>
                <InputLabel>Country</InputLabel>
                <Select
                  value={formCountryId}
                  label="Country"
                  onChange={(e) => setFormCountryId(e.target.value)}
                >
                  {countries.map((c) => (
                    <MenuItem key={c.id} value={c.id}>
                      {c.name} ({c.code})
                    </MenuItem>
                  ))}
                </Select>
              </FormControl>
            </Grid>

            <Grid item xs={12} sm={6}>
              <FormControl fullWidth required>
                <InputLabel>Visa Program Category</InputLabel>
                <Select
                  value={formProgramType}
                  label="Visa Program Category"
                  onChange={(e) => setFormProgramType(Number(e.target.value))}
                >
                  {Object.entries(VisaProgramLabels).map(([key, label]) => (
                    <MenuItem key={key} value={Number(key)}>
                      {label}
                    </MenuItem>
                  ))}
                </Select>
              </FormControl>
            </Grid>

            <Grid item xs={12}>
              <TextField
                fullWidth
                label="Program Name / Pathway Title"
                required
                value={formProgramName}
                onChange={(e) => setFormProgramName(e.target.value)}
                placeholder="e.g. US H-1B Specialty Occupation, Canada GTS LMIA-Exempt, Germany EU Blue Card"
              />
            </Grid>

            <Grid item xs={12} sm={6}>
              <FormControl fullWidth>
                <InputLabel>Eligibility Status</InputLabel>
                <Select
                  value={formEligibility}
                  label="Eligibility Status"
                  onChange={(e) => setFormEligibility(Number(e.target.value))}
                >
                  {Object.entries(SponsorshipEligibilityLabels).map(([key, label]) => (
                    <MenuItem key={key} value={Number(key)}>
                      {label}
                    </MenuItem>
                  ))}
                </Select>
              </FormControl>
            </Grid>

            <Grid item xs={12} sm={6}>
              <FormControl fullWidth>
                <InputLabel>Availability Model</InputLabel>
                <Select
                  value={formAvailability}
                  label="Availability Model"
                  onChange={(e) => setFormAvailability(Number(e.target.value))}
                >
                  {Object.entries(VisaAvailabilityLabels).map(([key, label]) => (
                    <MenuItem key={key} value={Number(key)}>
                      {label}
                    </MenuItem>
                  ))}
                </Select>
              </FormControl>
            </Grid>

            <Grid item xs={12} sm={4}>
              <TextField
                fullWidth
                label="Currency"
                value={formCurrency}
                onChange={(e) => setFormCurrency(e.target.value)}
                placeholder="USD, EUR, CAD, AUD, GBP"
              />
            </Grid>

            <Grid item xs={12} sm={4}>
              <TextField
                fullWidth
                type="number"
                label="Minimum Salary Threshold"
                value={formMinSalary}
                onChange={(e) => setFormMinSalary(e.target.value === '' ? '' : Number(e.target.value))}
                placeholder="e.g. 95000"
              />
            </Grid>

            <Grid item xs={12} sm={4}>
              <TextField
                fullWidth
                type="number"
                label="Est. Processing (Months)"
                value={formProcessingMonths}
                onChange={(e) => setFormProcessingMonths(e.target.value === '' ? '' : Number(e.target.value))}
              />
            </Grid>

            <Grid item xs={12} sm={6}>
              <TextField
                fullWidth
                type="date"
                label="Next Fiscal / Lottery Date"
                InputLabelProps={{ shrink: true }}
                value={formNextCycleDate}
                onChange={(e) => setFormNextCycleDate(e.target.value)}
              />
            </Grid>

            <Grid item xs={12} sm={6}>
              <TextField
                fullWidth
                type="date"
                label="Application Deadline"
                InputLabelProps={{ shrink: true }}
                value={formFilingDeadline}
                onChange={(e) => setFormFilingDeadline(e.target.value)}
              />
            </Grid>

            <Grid item xs={12} sm={6}>
              <FormControl fullWidth>
                <InputLabel>Associated Company (Optional)</InputLabel>
                <Select
                  value={formCompanyId}
                  label="Associated Company (Optional)"
                  onChange={(e) => setFormCompanyId(e.target.value)}
                >
                  <MenuItem value="">None</MenuItem>
                  {companies.map((c) => (
                    <MenuItem key={c.id} value={c.id}>
                      {c.name}
                    </MenuItem>
                  ))}
                </Select>
              </FormControl>
            </Grid>

            <Grid item xs={12} sm={6}>
              <FormControl fullWidth>
                <InputLabel>Associated Job Position (Optional)</InputLabel>
                <Select
                  value={formJobId}
                  label="Associated Job Position (Optional)"
                  onChange={(e) => setFormJobId(e.target.value)}
                >
                  <MenuItem value="">None</MenuItem>
                  {jobs.map((j) => (
                    <MenuItem key={j.id} value={j.id}>
                      {j.title} {j.company ? `(${j.company})` : ''}
                    </MenuItem>
                  ))}
                </Select>
              </FormControl>
            </Grid>

            <Grid item xs={12} sm={6}>
              <FormControlLabel
                control={
                  <Switch
                    checked={formSupportsDualIntent}
                    onChange={(e) => setFormSupportsDualIntent(e.target.checked)}
                    color="primary"
                  />
                }
                label="Supports Dual Intent (immigrant & non-immigrant)"
              />
            </Grid>

            <Grid item xs={12} sm={6}>
              <FormControlLabel
                control={
                  <Switch
                    checked={formLeadsToPR}
                    onChange={(e) => setFormLeadsToPR(e.target.checked)}
                    color="secondary"
                  />
                }
                label="Direct Pathway to Permanent Residency (PR / Green Card)"
              />
            </Grid>

            <Grid item xs={12}>
              <TextField
                fullWidth
                multiline
                rows={3}
                label="Strategic Notes & Requirements"
                value={formNotes}
                onChange={(e) => setFormNotes(e.target.value)}
                placeholder="Prevailing wage levels, educational equivalency, lottery quotas, attorney contacts..."
              />
            </Grid>
          </Grid>
        </DialogContent>
        <DialogActions sx={{ px: 3, py: 2 }}>
          <Button onClick={() => setOpenCaseDialog(false)} color="inherit">
            Cancel
          </Button>
          <Button onClick={handleSubmitCase} variant="contained" color="primary">
            {editingCase ? 'Save Changes' : 'Create Pathway'}
          </Button>
        </DialogActions>
      </Dialog>

      {/* Evidence Checklist Manager Dialog */}
      <Dialog
        open={openEvidenceDialog}
        onClose={() => setOpenEvidenceDialog(false)}
        maxWidth="md"
        fullWidth
      >
        <DialogTitle>
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
            <AttachFileIcon color="primary" />
            <Typography variant="h6" fontWeight="bold">
              Evidence Checklist: {selectedCaseForEvidence?.programName}
            </Typography>
          </Box>
        </DialogTitle>
        <DialogContent dividers>
          {selectedCaseForEvidence && (
            <>
              {/* Evidence list */}
              <Typography variant="subtitle2" fontWeight="bold" sx={{ mb: 1 }}>
                Attached Verification Items ({selectedCaseForEvidence.evidenceList.length})
              </Typography>

              {selectedCaseForEvidence.evidenceList.length === 0 ? (
                <Alert severity="info" sx={{ mb: 3 }}>
                  No evidence attached yet. Add credential evaluation, degree diploma, or employment verification letters below.
                </Alert>
              ) : (
                <TableContainer component={Paper} variant="outlined" sx={{ mb: 3 }}>
                  <Table size="small">
                    <TableHead>
                      <TableRow sx={{ bgcolor: 'action.hover' }}>
                        <TableCell width={50}><strong>Verified</strong></TableCell>
                        <TableCell><strong>Evidence Type</strong></TableCell>
                        <TableCell><strong>Description</strong></TableCell>
                        <TableCell><strong>Document Vault Link</strong></TableCell>
                        <TableCell align="right"><strong>Remove</strong></TableCell>
                      </TableRow>
                    </TableHead>
                    <TableBody>
                      {selectedCaseForEvidence.evidenceList.map((ev) => (
                        <TableRow key={ev.id} hover>
                          <TableCell>
                            <Checkbox
                              checked={ev.isVerified}
                              onChange={() => handleToggleEvidenceVerified(ev.id, ev.isVerified)}
                              color="success"
                              size="small"
                            />
                          </TableCell>
                          <TableCell>
                            <Typography variant="body2" fontWeight={ev.isVerified ? 'bold' : 'normal'}>
                              {ev.evidenceType}
                            </Typography>
                          </TableCell>
                          <TableCell>
                            <Typography variant="body2" color="text.secondary">
                              {ev.description || '—'}
                            </Typography>
                          </TableCell>
                          <TableCell>
                            {ev.documentName ? (
                              <Chip
                                icon={<AttachFileIcon fontSize="small" />}
                                label={ev.documentName}
                                size="small"
                                color="info"
                                variant="outlined"
                              />
                            ) : (
                              <Typography variant="caption" color="text.secondary">
                                No file linked
                              </Typography>
                            )}
                          </TableCell>
                          <TableCell align="right">
                            <IconButton size="small" color="error" onClick={() => handleRemoveEvidence(ev.id)}>
                              <DeleteIcon fontSize="small" />
                            </IconButton>
                          </TableCell>
                        </TableRow>
                      ))}
                    </TableBody>
                  </Table>
                </TableContainer>
              )}

              <Divider sx={{ my: 2 }} />

              {/* Add New Evidence Form */}
              <Typography variant="subtitle2" fontWeight="bold" sx={{ mb: 1.5 }}>
                Attach New Evidence
              </Typography>

              <Grid container spacing={2}>
                <Grid item xs={12} sm={6}>
                  <FormControl fullWidth size="small">
                    <InputLabel>Evidence Type</InputLabel>
                    <Select
                      value={evidenceType}
                      label="Evidence Type"
                      onChange={(e) => setEvidenceType(e.target.value)}
                    >
                      <MenuItem value="Degree Certificate">Degree Certificate / Diploma</MenuItem>
                      <MenuItem value="Academic Transcripts">Academic Transcripts</MenuItem>
                      <MenuItem value="Credential Evaluation">Credential Evaluation (WES / ECE)</MenuItem>
                      <MenuItem value="Employment Verification Letter">Employment Verification Letter</MenuItem>
                      <MenuItem value="Prevailing Wage Determination">Prevailing Wage Determination (PWD)</MenuItem>
                      <MenuItem value="Language Proficiency Test">Language Proficiency (IELTS / CELPIP / TOEFL)</MenuItem>
                      <MenuItem value="Passport Copy">Passport & Travel Identity</MenuItem>
                      <MenuItem value="Labor Market Assessment">Labor Market Assessment (LCA / LMIA)</MenuItem>
                      <MenuItem value="Other Support Document">Other Support Document</MenuItem>
                    </Select>
                  </FormControl>
                </Grid>

                <Grid item xs={12} sm={6}>
                  <FormControl fullWidth size="small">
                    <InputLabel>Link Document from Vault (Optional)</InputLabel>
                    <Select
                      value={evidenceDocumentId}
                      label="Link Document from Vault (Optional)"
                      onChange={(e) => setEvidenceDocumentId(e.target.value)}
                    >
                      <MenuItem value="">None</MenuItem>
                      {vaultDocuments.map((doc) => (
                        <MenuItem key={doc.id} value={doc.id}>
                          {doc.fileName} ({doc.categoryName})
                        </MenuItem>
                      ))}
                    </Select>
                  </FormControl>
                </Grid>

                <Grid item xs={12}>
                  <TextField
                    fullWidth
                    size="small"
                    label="Description & Issuing Body"
                    value={evidenceDescription}
                    onChange={(e) => setEvidenceDescription(e.target.value)}
                    placeholder="e.g. Master of Science in Computer Science evaluated as US equivalent by WES."
                  />
                </Grid>

                <Grid item xs={12} sm={6}>
                  <TextField
                    fullWidth
                    size="small"
                    type="date"
                    label="Expiration Date (if applicable)"
                    InputLabelProps={{ shrink: true }}
                    value={evidenceExpiryDate}
                    onChange={(e) => setEvidenceExpiryDate(e.target.value)}
                  />
                </Grid>

                <Grid item xs={12} sm={6}>
                  <FormControlLabel
                    control={
                      <Checkbox
                        checked={evidenceIsVerified}
                        onChange={(e) => setEvidenceIsVerified(e.target.checked)}
                        color="success"
                      />
                    }
                    label="Mark as Verified & Complete"
                  />
                </Grid>

                <Grid item xs={12}>
                  <Button
                    variant="contained"
                    color="primary"
                    startIcon={<AddIcon />}
                    onClick={handleAddEvidence}
                  >
                    Add Evidence Document
                  </Button>
                </Grid>
              </Grid>
            </>
          )}
        </DialogContent>
        <DialogActions sx={{ px: 3, py: 2 }}>
          <Button onClick={() => setOpenEvidenceDialog(false)}>
            Close
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  )
}

export default SponsorshipPage
