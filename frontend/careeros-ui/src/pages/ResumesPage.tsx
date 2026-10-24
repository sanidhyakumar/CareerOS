import { useEffect, useState } from 'react'
import {
  Box, Typography, Grid, Card, CardContent, IconButton,
  Dialog, DialogTitle, DialogContent, DialogActions, Button, TextField,
  FormControlLabel, Switch, Chip, CircularProgress, Alert, Tabs, Tab,
  Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Paper,
  MenuItem, Select, FormControl, InputLabel, Tooltip, Collapse, Divider,
} from '@mui/material'
import AddIcon from '@mui/icons-material/Add'
import DeleteIcon from '@mui/icons-material/Delete'
import EditIcon from '@mui/icons-material/Edit'
import StarIcon from '@mui/icons-material/Star'
import StarBorderIcon from '@mui/icons-material/StarBorder'
import ContentCopyIcon from '@mui/icons-material/ContentCopy'
import CameraAltIcon from '@mui/icons-material/CameraAlt'
import LaunchIcon from '@mui/icons-material/Launch'
import ExpandMoreIcon from '@mui/icons-material/ExpandMore'
import ExpandLessIcon from '@mui/icons-material/ExpandLess'
import TrendingUpIcon from '@mui/icons-material/TrendingUp'
import SendIcon from '@mui/icons-material/Send'
import FolderZipIcon from '@mui/icons-material/FolderZip'
import ArticleIcon from '@mui/icons-material/Article'
import WorkIcon from '@mui/icons-material/Work'

import {
  resumeProfilesApi, resumeVersionsApi, coverLettersApi,
  portfolioVersionsApi, resumeSubmissionsApi, resumeAnalyticsApi,
  type ResumeProfile, type ResumeVersion, type CoverLetter,
  type PortfolioVersion, type ResumeSubmission, type SubmissionSnapshot,
  type ResumeAnalytics, type CreateResumeProfileRequest, type CreateResumeVersionRequest,
  type CreateCoverLetterRequest, type CreatePortfolioVersionRequest, type CreateResumeSubmissionRequest,
} from '../api/resumeCenter'
import { documentsApi, type Document as VaultDocument } from '../api/documents'

export default function ResumesPage() {
  const [activeTab, setActiveTab] = useState(0)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [success, setSuccess] = useState<string | null>(null)

  // Data states
  const [profiles, setProfiles] = useState<ResumeProfile[]>([])
  const [expandedProfiles, setExpandedProfiles] = useState<Record<string, boolean>>({})
  const [coverLetters, setCoverLetters] = useState<CoverLetter[]>([])
  const [portfolios, setPortfolios] = useState<PortfolioVersion[]>([])
  const [submissions, setSubmissions] = useState<ResumeSubmission[]>([])
  const [analytics, setAnalytics] = useState<ResumeAnalytics | null>(null)
  const [vaultDocuments, setVaultDocuments] = useState<VaultDocument[]>([])

  // Modal Dialog states
  const [profileDialogOpen, setProfileDialogOpen] = useState(false)
  const [profileForm, setProfileForm] = useState<CreateResumeProfileRequest>({
    name: '', targetRole: '', targetIndustry: '', summary: '', primarySkills: '', secondarySkills: '', isDefault: false,
  })
  const [editProfileId, setEditProfileId] = useState<string | null>(null)

  const [versionDialogOpen, setVersionDialogOpen] = useState(false)
  const [selectedProfileForVersion, setSelectedProfileForVersion] = useState<string>('')
  const [versionForm, setVersionForm] = useState<CreateResumeVersionRequest>({
    resumeProfileId: '', versionName: '', fileFormat: 'PDF', targetCompany: '', targetJobTitle: '', tailoredKeywords: '', content: '', changeNotes: '', isBaseVersion: false, documentId: undefined,
  })

  const [coverLetterDialogOpen, setCoverLetterDialogOpen] = useState(false)
  const [coverLetterForm, setCoverLetterForm] = useState<CreateCoverLetterRequest>({
    title: '', body: '', targetCompany: '', targetRole: '', recipientName: '', recipientTitle: '', isTemplate: false, resumeProfileId: undefined,
  })
  const [editCoverLetterId, setEditCoverLetterId] = useState<string | null>(null)

  const [portfolioDialogOpen, setPortfolioDialogOpen] = useState(false)
  const [portfolioForm, setPortfolioForm] = useState<CreatePortfolioVersionRequest>({
    name: '', url: '', targetRole: '', description: '', featuredProjects: '', isDefault: false,
  })
  const [editPortfolioId, setEditPortfolioId] = useState<string | null>(null)

  const [submissionDialogOpen, setSubmissionDialogOpen] = useState(false)
  const [submissionForm, setSubmissionForm] = useState<CreateResumeSubmissionRequest>({
    resumeProfileId: '', resumeVersionId: '', companyName: '', roleTitle: '', channel: 'LinkedIn', status: 'Submitted', coverLetterId: undefined, portfolioVersionId: undefined, notes: '',
  })

  // Snapshot Viewer Modal
  const [snapshotDialogOpen, setSnapshotDialogOpen] = useState(false)
  const [selectedSnapshot, setSelectedSnapshot] = useState<SubmissionSnapshot | null>(null)
  const [snapshotLoading, setSnapshotLoading] = useState(false)

  // Loading all initial data
  const loadData = async () => {
    setLoading(true)
    try {
      const [profs, letters, ports, subs, an, docs] = await Promise.all([
        resumeProfilesApi.getAll(),
        coverLettersApi.getAll(),
        portfolioVersionsApi.getAll(),
        resumeSubmissionsApi.getAll(),
        resumeAnalyticsApi.getAnalytics(),
        documentsApi.getAll({ latestOnly: true }).catch(() => []),
      ])
      setProfiles(profs)
      setCoverLetters(letters)
      setPortfolios(ports)
      setSubmissions(subs)
      setAnalytics(an)
      setVaultDocuments(docs)

      // Default expand all profiles with versions
      const initialExpanded: Record<string, boolean> = {}
      profs.forEach((p) => { initialExpanded[p.id] = true })
      setExpandedProfiles(initialExpanded)
    } catch {
      setError('Failed to load Resume Center data')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    loadData()
  }, [])

  // Profile actions
  const handleSaveProfile = async () => {
    try {
      if (editProfileId) {
        await resumeProfilesApi.update(editProfileId, { ...profileForm, isArchived: false })
        setSuccess('Profile updated successfully')
      } else {
        await resumeProfilesApi.create(profileForm)
        setSuccess('Resume profile strategy created')
      }
      setProfileDialogOpen(false)
      loadData()
    } catch {
      setError('Failed to save profile')
    }
  }

  const handleDeleteProfile = async (id: string) => {
    if (!confirm('Are you sure you want to delete this profile and all its versions?')) return
    try {
      await resumeProfilesApi.delete(id)
      setSuccess('Profile deleted')
      loadData()
    } catch {
      setError('Failed to delete profile')
    }
  }

  const handleSetDefaultProfile = async (id: string) => {
    try {
      await resumeProfilesApi.setDefault(id)
      setSuccess('Set as default profile')
      loadData()
    } catch {
      setError('Failed to set default')
    }
  }

  // Version actions
  const handleOpenAddVersion = (profileId: string) => {
    setSelectedProfileForVersion(profileId)
    setVersionForm({
      resumeProfileId: profileId,
      versionName: '',
      fileFormat: 'PDF',
      targetCompany: '',
      targetJobTitle: '',
      tailoredKeywords: '',
      content: '',
      changeNotes: '',
      isBaseVersion: false,
      documentId: undefined,
    })
    setVersionDialogOpen(true)
  }

  const handleSaveVersion = async () => {
    try {
      await resumeProfilesApi.createVersion(selectedProfileForVersion, versionForm)
      setSuccess('New resume version created')
      setVersionDialogOpen(false)
      loadData()
    } catch {
      setError('Failed to create version')
    }
  }

  const handleCloneVersion = async (v: ResumeVersion) => {
    const company = prompt('Target company for this tailored copy (optional):', v.targetCompany ?? '')
    try {
      await resumeVersionsApi.clone(v.id, undefined, company || undefined)
      setSuccess('Version cloned successfully')
      loadData()
    } catch {
      setError('Failed to clone version')
    }
  }

  const handleDeleteVersion = async (id: string) => {
    if (!confirm('Delete this resume version?')) return
    try {
      await resumeVersionsApi.delete(id)
      setSuccess('Version deleted')
      loadData()
    } catch {
      setError('Failed to delete version')
    }
  }

  // Cover Letter actions
  const handleSaveCoverLetter = async () => {
    try {
      if (editCoverLetterId) {
        await coverLettersApi.update(editCoverLetterId, coverLetterForm)
        setSuccess('Cover letter updated')
      } else {
        await coverLettersApi.create(coverLetterForm)
        setSuccess('Cover letter created')
      }
      setCoverLetterDialogOpen(false)
      loadData()
    } catch {
      setError('Failed to save cover letter')
    }
  }

  const handleDeleteCoverLetter = async (id: string) => {
    if (!confirm('Delete this cover letter?')) return
    try {
      await coverLettersApi.delete(id)
      setSuccess('Cover letter deleted')
      loadData()
    } catch {
      setError('Failed to delete cover letter')
    }
  }

  // Portfolio actions
  const handleSavePortfolio = async () => {
    try {
      if (editPortfolioId) {
        await portfolioVersionsApi.update(editPortfolioId, portfolioForm)
        setSuccess('Portfolio variation updated')
      } else {
        await portfolioVersionsApi.create(portfolioForm)
        setSuccess('Portfolio variation created')
      }
      setPortfolioDialogOpen(false)
      loadData()
    } catch {
      setError('Failed to save portfolio')
    }
  }

  const handleDeletePortfolio = async (id: string) => {
    if (!confirm('Delete this portfolio variation?')) return
    try {
      await portfolioVersionsApi.delete(id)
      setSuccess('Portfolio variation deleted')
      loadData()
    } catch {
      setError('Failed to delete portfolio')
    }
  }

  // Submission actions
  const handleOpenAddSubmission = () => {
    const defaultProfile = profiles.find((p) => p.isDefault) ?? profiles[0]
    const defaultVersion = defaultProfile?.versions?.[0]
    setSubmissionForm({
      resumeProfileId: defaultProfile?.id ?? '',
      resumeVersionId: defaultVersion?.id ?? '',
      companyName: '',
      roleTitle: defaultProfile?.targetRole ?? '',
      channel: 'LinkedIn',
      status: 'Submitted',
      coverLetterId: undefined,
      portfolioVersionId: undefined,
      notes: '',
    })
    setSubmissionDialogOpen(true)
  }

  const handleSaveSubmission = async () => {
    try {
      await resumeSubmissionsApi.create(submissionForm)
      setSuccess('Submission recorded with immutable snapshot!')
      setSubmissionDialogOpen(false)
      loadData()
    } catch {
      setError('Failed to record submission')
    }
  }

  const handleDeleteSubmission = async (id: string) => {
    if (!confirm('Delete this submission record and its snapshot?')) return
    try {
      await resumeSubmissionsApi.delete(id)
      setSuccess('Submission deleted')
      loadData()
    } catch {
      setError('Failed to delete submission')
    }
  }

  const handleViewSnapshot = async (submissionId: string) => {
    setSnapshotLoading(true)
    setSnapshotDialogOpen(true)
    try {
      const snap = await resumeSubmissionsApi.getSnapshot(submissionId)
      setSelectedSnapshot(snap)
    } catch {
      setError('Failed to load submission snapshot')
      setSnapshotDialogOpen(false)
    } finally {
      setSnapshotLoading(false)
    }
  }

  const copyToClipboard = (text: string) => {
    navigator.clipboard.writeText(text)
    setSuccess('Copied to clipboard!')
  }

  const toggleExpand = (id: string) => {
    setExpandedProfiles((prev) => ({ ...prev, [id]: !prev[id] }))
  }

  // Helpers for submission form profile change
  const selectedProfileObj = profiles.find((p) => p.id === submissionForm.resumeProfileId)

  return (
    <Box>
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={2}>
        <Box>
          <Typography variant="h4" fontWeight={700}>Resume Center</Typography>
          <Typography variant="body2" color="text.secondary">
            Manage multi-strategy resume profiles, tailored versions, cover letters, portfolios, and immutable submission snapshots.
          </Typography>
        </Box>
        {activeTab === 0 && (
          <Button variant="contained" startIcon={<AddIcon />} onClick={() => {
            setEditProfileId(null)
            setProfileForm({ name: '', targetRole: '', targetIndustry: '', summary: '', primarySkills: '', secondarySkills: '', isDefault: profiles.length === 0 })
            setProfileDialogOpen(true)
          }}>
            New Profile Strategy
          </Button>
        )}
        {activeTab === 1 && (
          <Button variant="contained" startIcon={<AddIcon />} onClick={() => {
            setEditCoverLetterId(null)
            setCoverLetterForm({ title: '', body: '', targetCompany: '', targetRole: '', recipientName: '', recipientTitle: '', isTemplate: false })
            setCoverLetterDialogOpen(true)
          }}>
            New Cover Letter
          </Button>
        )}
        {activeTab === 2 && (
          <Button variant="contained" startIcon={<AddIcon />} onClick={() => {
            setEditPortfolioId(null)
            setPortfolioForm({ name: '', url: '', targetRole: '', description: '', featuredProjects: '', isDefault: portfolios.length === 0 })
            setPortfolioDialogOpen(true)
          }}>
            New Portfolio
          </Button>
        )}
        {activeTab === 3 && (
          <Button variant="contained" startIcon={<SendIcon />} onClick={handleOpenAddSubmission}>
            Log Submission
          </Button>
        )}
      </Box>

      {error && <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError(null)}>{error}</Alert>}
      {success && <Alert severity="success" sx={{ mb: 2 }} onClose={() => setSuccess(null)}>{success}</Alert>}

      <Tabs
        value={activeTab}
        onChange={(_, v) => setActiveTab(v)}
        sx={{ borderBottom: 1, borderColor: 'divider', mb: 3 }}
      >
        <Tab label={`Profiles & Versions (${profiles.length})`} icon={<WorkIcon />} iconPosition="start" />
        <Tab label={`Cover Letters (${coverLetters.length})`} icon={<ArticleIcon />} iconPosition="start" />
        <Tab label={`Portfolios (${portfolios.length})`} icon={<FolderZipIcon />} iconPosition="start" />
        <Tab label={`Submissions & Snapshots (${submissions.length})`} icon={<CameraAltIcon />} iconPosition="start" />
        <Tab label="Strategy Analytics" icon={<TrendingUpIcon />} iconPosition="start" />
      </Tabs>

      {loading ? (
        <Box display="flex" justifyContent="center" mt={8}><CircularProgress /></Box>
      ) : (
        <>
          {/* TAB 0: Profiles & Versions */}
          {activeTab === 0 && (
            <Grid container spacing={3}>
              {profiles.length === 0 ? (
                <Grid item xs={12}>
                  <Box textAlign="center" py={8} color="text.secondary">
                    <WorkIcon sx={{ fontSize: 64, opacity: 0.3 }} />
                    <Typography mt={2}>No resume profiles defined yet.</Typography>
                    <Typography variant="body2" mt={1}>Create your first profile strategy (e.g. "Full Stack Lead", "Systems Architect").</Typography>
                    <Button variant="outlined" startIcon={<AddIcon />} sx={{ mt: 2 }} onClick={() => {
                      setEditProfileId(null)
                      setProfileForm({ name: '', targetRole: '', targetIndustry: '', summary: '', primarySkills: '', secondarySkills: '', isDefault: true })
                      setProfileDialogOpen(true)
                    }}>
                      Create First Profile
                    </Button>
                  </Box>
                </Grid>
              ) : profiles.map((p) => {
                const isExpanded = !!expandedProfiles[p.id]
                return (
                  <Grid item xs={12} key={p.id}>
                    <Card variant="outlined" sx={{ borderLeft: p.isDefault ? '4px solid #1976d2' : undefined }}>
                      <CardContent>
                        <Box display="flex" justifyContent="space-between" alignItems="flex-start">
                          <Box display="flex" alignItems="center" gap={1.5}>
                            <Typography variant="h6" fontWeight={700}>{p.name}</Typography>
                            {p.isDefault && (
                              <Chip size="small" color="primary" label="Default Strategy" icon={<StarIcon />} />
                            )}
                            {p.targetIndustry && (
                              <Chip size="small" variant="outlined" label={p.targetIndustry} />
                            )}
                          </Box>
                          <Box display="flex" alignItems="center" gap={1}>
                            <Tooltip title={p.isDefault ? 'Default strategy' : 'Set as default strategy'}>
                              <IconButton size="small" onClick={() => handleSetDefaultProfile(p.id)} color={p.isDefault ? 'warning' : 'default'}>
                                {p.isDefault ? <StarIcon /> : <StarBorderIcon />}
                              </IconButton>
                            </Tooltip>
                            <IconButton size="small" onClick={() => {
                              setEditProfileId(p.id)
                              setProfileForm({
                                name: p.name,
                                targetRole: p.targetRole,
                                targetIndustry: p.targetIndustry ?? '',
                                summary: p.summary ?? '',
                                primarySkills: p.primarySkills ?? '',
                                secondarySkills: p.secondarySkills ?? '',
                                isDefault: p.isDefault,
                              })
                              setProfileDialogOpen(true)
                            }}>
                              <EditIcon fontSize="small" />
                            </IconButton>
                            <IconButton size="small" color="error" onClick={() => handleDeleteProfile(p.id)}>
                              <DeleteIcon fontSize="small" />
                            </IconButton>
                            <IconButton size="small" onClick={() => toggleExpand(p.id)}>
                              {isExpanded ? <ExpandLessIcon /> : <ExpandMoreIcon />}
                            </IconButton>
                          </Box>
                        </Box>

                        <Typography variant="subtitle2" color="primary.main" fontWeight={600} mt={0.5}>
                          Target Role: {p.targetRole}
                        </Typography>

                        {p.summary && (
                          <Typography variant="body2" color="text.secondary" mt={1}>
                            {p.summary}
                          </Typography>
                        )}

                        <Box display="flex" flexWrap="wrap" gap={1} mt={1.5}>
                          {p.primarySkills && p.primarySkills.split(',').map((s) => (
                            <Chip key={s} label={s.trim()} size="small" color="secondary" variant="outlined" />
                          ))}
                          {p.secondarySkills && p.secondarySkills.split(',').map((s) => (
                            <Chip key={s} label={s.trim()} size="small" variant="outlined" />
                          ))}
                        </Box>

                        <Box display="flex" gap={2} mt={2} alignItems="center">
                          <Chip size="small" label={`${p.versionCount ?? p.versions?.length ?? 0} Versions`} />
                          <Chip size="small" label={`${p.submissionCount ?? 0} Submissions`} color="info" variant="outlined" />
                          <Button size="small" startIcon={<AddIcon />} onClick={() => handleOpenAddVersion(p.id)}>
                            Add Version
                          </Button>
                        </Box>

                        {/* Versions Collapsible */}
                        <Collapse in={isExpanded} timeout="auto" unmountOnExit>
                          <Divider sx={{ my: 2 }} />
                          <Typography variant="subtitle2" fontWeight={600} mb={1}>
                            Resume Versions & Variations
                          </Typography>
                          {(!p.versions || p.versions.length === 0) ? (
                            <Typography variant="body2" color="text.secondary">
                              No versions added to this profile yet. Click "Add Version" to create a tailored variation.
                            </Typography>
                          ) : (
                            <Grid container spacing={2}>
                              {p.versions.map((v) => (
                                <Grid item xs={12} md={6} key={v.id}>
                                  <Card variant="outlined" sx={{ bgcolor: 'background.default' }}>
                                    <CardContent sx={{ pb: 1 }}>
                                      <Box display="flex" justifyContent="space-between" alignItems="flex-start">
                                        <Box>
                                          <Box display="flex" alignItems="center" gap={1}>
                                            <Typography variant="subtitle1" fontWeight={700}>
                                              {v.versionName}
                                            </Typography>
                                            <Chip size="small" label={`v${v.versionNumber}`} />
                                            {v.isBaseVersion && <Chip size="small" label="Base" color="success" />}
                                            <Chip size="small" variant="outlined" label={v.fileFormat} />
                                          </Box>
                                          {v.targetCompany && (
                                            <Typography variant="caption" color="text.secondary" display="block">
                                              Tailored for: <strong>{v.targetCompany}</strong>
                                            </Typography>
                                          )}
                                        </Box>
                                        <Box>
                                          <Tooltip title="Clone as tailored variation">
                                            <IconButton size="small" onClick={() => handleCloneVersion(v)}>
                                              <ContentCopyIcon fontSize="small" />
                                            </IconButton>
                                          </Tooltip>
                                          <IconButton size="small" color="error" onClick={() => handleDeleteVersion(v.id)}>
                                            <DeleteIcon fontSize="small" />
                                          </IconButton>
                                        </Box>
                                      </Box>

                                      {v.documentFileName && (
                                        <Box mt={1} display="flex" alignItems="center" gap={1}>
                                          <FolderZipIcon fontSize="small" color="primary" />
                                          <Typography variant="caption" fontWeight={600} color="primary">
                                            Vault: {v.documentFileName}
                                          </Typography>
                                          {v.documentSha256 && (
                                            <Typography variant="caption" color="text.secondary">
                                              (SHA: {v.documentSha256.substring(0, 8)}...)
                                            </Typography>
                                          )}
                                        </Box>
                                      )}

                                      {v.tailoredKeywords && (
                                        <Box mt={1} display="flex" flexWrap="wrap" gap={0.5}>
                                          {v.tailoredKeywords.split(',').map((k) => (
                                            <Chip key={k} size="small" label={k.trim()} sx={{ fontSize: '0.7rem' }} />
                                          ))}
                                        </Box>
                                      )}

                                      {v.changeNotes && (
                                        <Typography variant="caption" color="text.secondary" display="block" mt={1}>
                                          Notes: {v.changeNotes}
                                        </Typography>
                                      )}

                                      {v.content && (
                                        <Box mt={1} p={1} bgcolor="action.hover" borderRadius={1}>
                                          <Typography variant="caption" sx={{ overflow: 'hidden', textOverflow: 'ellipsis', display: '-webkit-box', WebkitLineClamp: 2, WebkitBoxOrient: 'vertical' }}>
                                            {v.content}
                                          </Typography>
                                        </Box>
                                      )}
                                    </CardContent>
                                  </Card>
                                </Grid>
                              ))}
                            </Grid>
                          )}
                        </Collapse>
                      </CardContent>
                    </Card>
                  </Grid>
                )
              })}
            </Grid>
          )}

          {/* TAB 1: Cover Letters */}
          {activeTab === 1 && (
            <Grid container spacing={3}>
              {coverLetters.length === 0 ? (
                <Grid item xs={12}>
                  <Box textAlign="center" py={8} color="text.secondary">
                    <ArticleIcon sx={{ fontSize: 64, opacity: 0.3 }} />
                    <Typography mt={2}>No cover letters created yet.</Typography>
                    <Typography variant="body2" mt={1}>Create reusable templates or tailored cover letters.</Typography>
                    <Button variant="outlined" startIcon={<AddIcon />} sx={{ mt: 2 }} onClick={() => {
                      setEditCoverLetterId(null)
                      setCoverLetterForm({ title: '', body: '', targetCompany: '', targetRole: '', recipientName: '', recipientTitle: '', isTemplate: true })
                      setCoverLetterDialogOpen(true)
                    }}>
                      Create First Letter
                    </Button>
                  </Box>
                </Grid>
              ) : coverLetters.map((c) => (
                <Grid item xs={12} md={6} key={c.id}>
                  <Card variant="outlined">
                    <CardContent>
                      <Box display="flex" justifyContent="space-between" alignItems="flex-start">
                        <Box>
                          <Typography variant="h6" fontWeight={700}>{c.title}</Typography>
                          <Box display="flex" gap={1} mt={0.5} alignItems="center">
                            {c.isTemplate && <Chip size="small" color="info" label="Template" />}
                            {c.targetCompany && <Chip size="small" variant="outlined" label={c.targetCompany} />}
                            {c.targetRole && <Chip size="small" variant="outlined" label={c.targetRole} />}
                          </Box>
                        </Box>
                        <Box>
                          <Tooltip title="Copy body text">
                            <IconButton size="small" onClick={() => copyToClipboard(c.body)}>
                              <ContentCopyIcon fontSize="small" />
                            </IconButton>
                          </Tooltip>
                          <IconButton size="small" onClick={() => {
                            setEditCoverLetterId(c.id)
                            setCoverLetterForm({
                              title: c.title,
                              body: c.body,
                              targetCompany: c.targetCompany ?? '',
                              targetRole: c.targetRole ?? '',
                              recipientName: c.recipientName ?? '',
                              recipientTitle: c.recipientTitle ?? '',
                              isTemplate: c.isTemplate,
                              resumeProfileId: c.resumeProfileId,
                            })
                            setCoverLetterDialogOpen(true)
                          }}>
                            <EditIcon fontSize="small" />
                          </IconButton>
                          <IconButton size="small" color="error" onClick={() => handleDeleteCoverLetter(c.id)}>
                            <DeleteIcon fontSize="small" />
                          </IconButton>
                        </Box>
                      </Box>

                      {c.recipientName && (
                        <Typography variant="caption" color="text.secondary" display="block" mt={1}>
                          Recipient: {c.recipientName} {c.recipientTitle ? `(${c.recipientTitle})` : ''}
                        </Typography>
                      )}

                      <Box mt={1.5} p={1.5} bgcolor="background.default" borderRadius={1} maxHeight={160} sx={{ overflowY: 'auto' }}>
                        <Typography variant="body2" sx={{ whiteSpace: 'pre-wrap', fontFamily: 'monospace' }}>
                          {c.body}
                        </Typography>
                      </Box>
                    </CardContent>
                  </Card>
                </Grid>
              ))}
            </Grid>
          )}

          {/* TAB 2: Portfolios */}
          {activeTab === 2 && (
            <Grid container spacing={3}>
              {portfolios.length === 0 ? (
                <Grid item xs={12}>
                  <Box textAlign="center" py={8} color="text.secondary">
                    <FolderZipIcon sx={{ fontSize: 64, opacity: 0.3 }} />
                    <Typography mt={2}>No portfolio versions added yet.</Typography>
                    <Typography variant="body2" mt={1}>Add link variations and project showcases tailored for specific audiences.</Typography>
                    <Button variant="outlined" startIcon={<AddIcon />} sx={{ mt: 2 }} onClick={() => {
                      setEditPortfolioId(null)
                      setPortfolioForm({ name: '', url: '', targetRole: '', description: '', featuredProjects: '', isDefault: true })
                      setPortfolioDialogOpen(true)
                    }}>
                      Create First Portfolio
                    </Button>
                  </Box>
                </Grid>
              ) : portfolios.map((p) => (
                <Grid item xs={12} md={6} key={p.id}>
                  <Card variant="outlined">
                    <CardContent>
                      <Box display="flex" justifyContent="space-between" alignItems="flex-start">
                        <Box>
                          <Box display="flex" alignItems="center" gap={1}>
                            <Typography variant="h6" fontWeight={700}>{p.name}</Typography>
                            {p.isDefault && <Chip size="small" color="primary" label="Default" />}
                          </Box>
                          {p.targetRole && (
                            <Typography variant="subtitle2" color="primary" mt={0.5}>
                              Target: {p.targetRole}
                            </Typography>
                          )}
                        </Box>
                        <Box>
                          {p.url && (
                            <IconButton size="small" component="a" href={p.url} target="_blank" rel="noopener noreferrer">
                              <LaunchIcon fontSize="small" />
                            </IconButton>
                          )}
                          <IconButton size="small" onClick={() => {
                            setEditPortfolioId(p.id)
                            setPortfolioForm({
                              name: p.name,
                              url: p.url ?? '',
                              targetRole: p.targetRole ?? '',
                              description: p.description ?? '',
                              featuredProjects: p.featuredProjects ?? '',
                              isDefault: p.isDefault,
                            })
                            setPortfolioDialogOpen(true)
                          }}>
                            <EditIcon fontSize="small" />
                          </IconButton>
                          <IconButton size="small" color="error" onClick={() => handleDeletePortfolio(p.id)}>
                            <DeleteIcon fontSize="small" />
                          </IconButton>
                        </Box>
                      </Box>

                      {p.url && (
                        <Typography variant="caption" color="text.secondary" display="block" mt={0.5}>
                          Link: {p.url}
                        </Typography>
                      )}

                      {p.description && (
                        <Typography variant="body2" mt={1}>
                          {p.description}
                        </Typography>
                      )}

                      {p.featuredProjects && (
                        <Box mt={1.5} p={1} bgcolor="background.default" borderRadius={1}>
                          <Typography variant="caption" fontWeight={600} display="block" color="text.secondary">
                            Featured Projects:
                          </Typography>
                          <Typography variant="body2" sx={{ whiteSpace: 'pre-wrap' }}>
                            {p.featuredProjects}
                          </Typography>
                        </Box>
                      )}
                    </CardContent>
                  </Card>
                </Grid>
              ))}
            </Grid>
          )}

          {/* TAB 3: Submissions & Snapshots */}
          {activeTab === 3 && (
            <TableContainer component={Paper} variant="outlined">
              <Table>
                <TableHead>
                  <TableRow>
                    <TableCell><strong>Date</strong></TableCell>
                    <TableCell><strong>Company</strong></TableCell>
                    <TableCell><strong>Role</strong></TableCell>
                    <TableCell><strong>Strategy Profile</strong></TableCell>
                    <TableCell><strong>Version Used</strong></TableCell>
                    <TableCell><strong>Channel</strong></TableCell>
                    <TableCell><strong>Status</strong></TableCell>
                    <TableCell align="center"><strong>Snapshot</strong></TableCell>
                    <TableCell align="right"><strong>Actions</strong></TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {submissions.length === 0 ? (
                    <TableRow>
                      <TableCell colSpan={9} align="center" sx={{ py: 6, color: 'text.secondary' }}>
                        No submissions recorded yet. Click "Log Submission" to track an application.
                      </TableCell>
                    </TableRow>
                  ) : submissions.map((s) => (
                    <TableRow key={s.id} hover>
                      <TableCell>{new Date(s.submissionDate).toLocaleDateString()}</TableCell>
                      <TableCell><strong>{s.companyName}</strong></TableCell>
                      <TableCell>{s.roleTitle}</TableCell>
                      <TableCell>{s.profileName ?? 'Custom Profile'}</TableCell>
                      <TableCell>
                        <Chip size="small" label={`${s.versionName ?? 'Version'} (v${s.versionNumber})`} />
                      </TableCell>
                      <TableCell><Chip size="small" variant="outlined" label={s.channel} /></TableCell>
                      <TableCell>
                        <Chip
                          size="small"
                          label={s.status}
                          color={
                            s.status === 'Offer' ? 'success' :
                            s.status === 'Interviewing' ? 'primary' :
                            s.status === 'Screening' ? 'warning' :
                            s.status === 'Rejected' ? 'error' : 'default'
                          }
                        />
                      </TableCell>
                      <TableCell align="center">
                        <Tooltip title="View immutable submission snapshot">
                          <Button
                            size="small"
                            variant="outlined"
                            startIcon={<CameraAltIcon />}
                            onClick={() => handleViewSnapshot(s.id)}
                          >
                            Snapshot
                          </Button>
                        </Tooltip>
                      </TableCell>
                      <TableCell align="right">
                        <IconButton size="small" color="error" onClick={() => handleDeleteSubmission(s.id)}>
                          <DeleteIcon fontSize="small" />
                        </IconButton>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </TableContainer>
          )}

          {/* TAB 4: Strategy Analytics */}
          {activeTab === 4 && analytics && (
            <Box>
              {/* Stat Cards */}
              <Grid container spacing={3} mb={4}>
                <Grid item xs={12} sm={6} md={3}>
                  <Card variant="outlined">
                    <CardContent>
                      <Typography variant="caption" color="text.secondary">Total Submissions</Typography>
                      <Typography variant="h4" fontWeight={700} color="primary.main">{analytics.totalSubmissions}</Typography>
                      <Typography variant="caption" color="text.secondary">{analytics.totalProfiles} active strategies</Typography>
                    </CardContent>
                  </Card>
                </Grid>
                <Grid item xs={12} sm={6} md={3}>
                  <Card variant="outlined">
                    <CardContent>
                      <Typography variant="caption" color="text.secondary">Interviews & Screenings</Typography>
                      <Typography variant="h4" fontWeight={700} color="warning.main">{analytics.totalInterviews}</Typography>
                      <Typography variant="caption" color="text.secondary">Responses received</Typography>
                    </CardContent>
                  </Card>
                </Grid>
                <Grid item xs={12} sm={6} md={3}>
                  <Card variant="outlined">
                    <CardContent>
                      <Typography variant="caption" color="text.secondary">Offers Received</Typography>
                      <Typography variant="h4" fontWeight={700} color="success.main">{analytics.totalOffers}</Typography>
                      <Typography variant="caption" color="text.secondary">Job offers earned</Typography>
                    </CardContent>
                  </Card>
                </Grid>
                <Grid item xs={12} sm={6} md={3}>
                  <Card variant="outlined">
                    <CardContent>
                      <Typography variant="caption" color="text.secondary">Overall Response Rate</Typography>
                      <Typography variant="h4" fontWeight={700} color="secondary.main">{analytics.overallResponseRate}%</Typography>
                      <Typography variant="caption" color="text.secondary">Conversion to interview</Typography>
                    </CardContent>
                  </Card>
                </Grid>
              </Grid>

              {/* Profiles Strategy Comparison */}
              <Typography variant="h6" fontWeight={700} mb={2}>Resume Strategy Comparison</Typography>
              <TableContainer component={Paper} variant="outlined" sx={{ mb: 4 }}>
                <Table>
                  <TableHead>
                    <TableRow>
                      <TableCell><strong>Strategy Profile</strong></TableCell>
                      <TableCell><strong>Target Role</strong></TableCell>
                      <TableCell align="center"><strong>Versions</strong></TableCell>
                      <TableCell align="center"><strong>Submissions</strong></TableCell>
                      <TableCell align="center"><strong>Interviews</strong></TableCell>
                      <TableCell align="center"><strong>Offers</strong></TableCell>
                      <TableCell align="right"><strong>Response Rate</strong></TableCell>
                    </TableRow>
                  </TableHead>
                  <TableBody>
                    {analytics.profileMetrics.length === 0 ? (
                      <TableRow>
                        <TableCell colSpan={7} align="center" sx={{ py: 4, color: 'text.secondary' }}>
                          No submission data available yet to compute strategy effectiveness.
                        </TableCell>
                      </TableRow>
                    ) : analytics.profileMetrics.map((pm) => (
                      <TableRow key={pm.profileId} hover>
                        <TableCell><strong>{pm.profileName}</strong></TableCell>
                        <TableCell>{pm.targetRole}</TableCell>
                        <TableCell align="center">{pm.versionsCount}</TableCell>
                        <TableCell align="center">{pm.submissionsCount}</TableCell>
                        <TableCell align="center">{pm.interviewsCount}</TableCell>
                        <TableCell align="center">{pm.offersCount}</TableCell>
                        <TableCell align="right">
                          <Chip
                            size="small"
                            label={`${pm.responseRate}%`}
                            color={pm.responseRate >= 20 ? 'success' : pm.responseRate >= 10 ? 'warning' : 'default'}
                          />
                        </TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              </TableContainer>

              {/* Channels Breakdown */}
              <Typography variant="h6" fontWeight={700} mb={2}>Submission Channels</Typography>
              <Grid container spacing={2}>
                {analytics.channelMetrics.map((cm) => (
                  <Grid item xs={12} sm={6} md={3} key={cm.channel}>
                    <Card variant="outlined">
                      <CardContent>
                        <Typography variant="subtitle1" fontWeight={700}>{cm.channel}</Typography>
                        <Box display="flex" justifyContent="space-between" mt={1}>
                          <Typography variant="body2" color="text.secondary">Submissions:</Typography>
                          <Typography variant="body2" fontWeight={600}>{cm.submissionsCount}</Typography>
                        </Box>
                        <Box display="flex" justifyContent="space-between">
                          <Typography variant="body2" color="text.secondary">Interviews:</Typography>
                          <Typography variant="body2" fontWeight={600}>{cm.interviewsCount}</Typography>
                        </Box>
                        <Box display="flex" justifyContent="space-between" mt={1} pt={1} borderTop={1} borderColor="divider">
                          <Typography variant="body2" color="text.secondary">Response Rate:</Typography>
                          <Typography variant="body2" fontWeight={700} color="primary">{cm.responseRate}%</Typography>
                        </Box>
                      </CardContent>
                    </Card>
                  </Grid>
                ))}
              </Grid>
            </Box>
          )}
        </>
      )}

      {/* DIALOG: Create / Edit Profile */}
      <Dialog open={profileDialogOpen} onClose={() => setProfileDialogOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>{editProfileId ? 'Edit Profile Strategy' : 'New Resume Profile Strategy'}</DialogTitle>
        <DialogContent sx={{ display: 'flex', flexDirection: 'column', gap: 2, pt: 2 }}>
          <TextField
            label="Profile Strategy Name"
            placeholder="e.g. Staff Full-Stack Lead / Distributed Systems"
            value={profileForm.name}
            onChange={(e) => setProfileForm({ ...profileForm, name: e.target.value })}
            fullWidth
            required
          />
          <TextField
            label="Target Role / Job Title"
            placeholder="e.g. Staff Software Engineer"
            value={profileForm.targetRole}
            onChange={(e) => setProfileForm({ ...profileForm, targetRole: e.target.value })}
            fullWidth
            required
          />
          <TextField
            label="Target Industry / Domain (Optional)"
            placeholder="e.g. Fintech, Cloud Platforms, AI/ML Infrastructure"
            value={profileForm.targetIndustry ?? ''}
            onChange={(e) => setProfileForm({ ...profileForm, targetIndustry: e.target.value })}
            fullWidth
          />
          <TextField
            label="Executive Summary / Strategy Theme"
            placeholder="Key highlights and narrative for this career persona..."
            value={profileForm.summary ?? ''}
            onChange={(e) => setProfileForm({ ...profileForm, summary: e.target.value })}
            multiline
            rows={3}
            fullWidth
          />
          <TextField
            label="Primary Skills (Comma-separated)"
            placeholder="e.g. .NET 9, React, TypeScript, Distributed Systems, Docker"
            value={profileForm.primarySkills ?? ''}
            onChange={(e) => setProfileForm({ ...profileForm, primarySkills: e.target.value })}
            fullWidth
          />
          <TextField
            label="Secondary Skills (Comma-separated)"
            placeholder="e.g. AWS, Kubernetes, GraphQL, System Design, Mentorship"
            value={profileForm.secondarySkills ?? ''}
            onChange={(e) => setProfileForm({ ...profileForm, secondarySkills: e.target.value })}
            fullWidth
          />
          <FormControlLabel
            control={<Switch checked={profileForm.isDefault} onChange={(e) => setProfileForm({ ...profileForm, isDefault: e.target.checked })} />}
            label="Set as Default Strategy"
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setProfileDialogOpen(false)}>Cancel</Button>
          <Button variant="contained" onClick={handleSaveProfile} disabled={!profileForm.name || !profileForm.targetRole}>
            Save Strategy
          </Button>
        </DialogActions>
      </Dialog>

      {/* DIALOG: Create Resume Version */}
      <Dialog open={versionDialogOpen} onClose={() => setVersionDialogOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>Add Resume Version</DialogTitle>
        <DialogContent sx={{ display: 'flex', flexDirection: 'column', gap: 2, pt: 2 }}>
          <TextField
            label="Version Name"
            placeholder="e.g. Google Tailored 2-Page, FinTech High-Impact v3"
            value={versionForm.versionName}
            onChange={(e) => setVersionForm({ ...versionForm, versionName: e.target.value })}
            fullWidth
            required
          />
          <Grid container spacing={2}>
            <Grid item xs={6}>
              <FormControl fullWidth>
                <InputLabel>File Format</InputLabel>
                <Select
                  value={versionForm.fileFormat ?? 'PDF'}
                  label="File Format"
                  onChange={(e) => setVersionForm({ ...versionForm, fileFormat: e.target.value })}
                >
                  <MenuItem value="PDF">PDF</MenuItem>
                  <MenuItem value="DOCX">DOCX</MenuItem>
                  <MenuItem value="Markdown">Markdown</MenuItem>
                  <MenuItem value="Text">Plain Text</MenuItem>
                </Select>
              </FormControl>
            </Grid>
            <Grid item xs={6}>
              <FormControl fullWidth>
                <InputLabel>Vault Document (Optional)</InputLabel>
                <Select
                  value={versionForm.documentId ?? ''}
                  label="Vault Document (Optional)"
                  onChange={(e) => setVersionForm({ ...versionForm, documentId: e.target.value ? e.target.value : undefined })}
                >
                  <MenuItem value=""><em>None / Raw Text Only</em></MenuItem>
                  {vaultDocuments.map((d) => (
                    <MenuItem key={d.id} value={d.id}>{d.originalFileName}</MenuItem>
                  ))}
                </Select>
              </FormControl>
            </Grid>
          </Grid>
          <TextField
            label="Target Company (Optional)"
            placeholder="e.g. Google, Stripe, Meta"
            value={versionForm.targetCompany ?? ''}
            onChange={(e) => setVersionForm({ ...versionForm, targetCompany: e.target.value })}
            fullWidth
          />
          <TextField
            label="Tailored ATS Keywords (Comma-separated)"
            placeholder="e.g. Distributed Consensus, High-Throughput, CQRS, Event-Driven"
            value={versionForm.tailoredKeywords ?? ''}
            onChange={(e) => setVersionForm({ ...versionForm, tailoredKeywords: e.target.value })}
            fullWidth
          />
          <TextField
            label="Change Notes / Changelog"
            placeholder="What was tweaked or highlighted in this version..."
            value={versionForm.changeNotes ?? ''}
            onChange={(e) => setVersionForm({ ...versionForm, changeNotes: e.target.value })}
            fullWidth
          />
          <TextField
            label="Resume Content / Markdown / Notes"
            placeholder="Paste text / markdown representation of this resume version..."
            value={versionForm.content ?? ''}
            onChange={(e) => setVersionForm({ ...versionForm, content: e.target.value })}
            multiline
            rows={4}
            fullWidth
          />
          <FormControlLabel
            control={<Switch checked={versionForm.isBaseVersion} onChange={(e) => setVersionForm({ ...versionForm, isBaseVersion: e.target.checked })} />}
            label="Mark as Base Template Version for this Profile"
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setVersionDialogOpen(false)}>Cancel</Button>
          <Button variant="contained" onClick={handleSaveVersion} disabled={!versionForm.versionName}>
            Create Version
          </Button>
        </DialogActions>
      </Dialog>

      {/* DIALOG: Create / Edit Cover Letter */}
      <Dialog open={coverLetterDialogOpen} onClose={() => setCoverLetterDialogOpen(false)} maxWidth="md" fullWidth>
        <DialogTitle>{editCoverLetterId ? 'Edit Cover Letter' : 'New Cover Letter'}</DialogTitle>
        <DialogContent sx={{ display: 'flex', flexDirection: 'column', gap: 2, pt: 2 }}>
          <TextField
            label="Title"
            placeholder="e.g. Standard Distributed Systems Lead, Stripe FinTech Pitch"
            value={coverLetterForm.title}
            onChange={(e) => setCoverLetterForm({ ...coverLetterForm, title: e.target.value })}
            fullWidth
            required
          />
          <Grid container spacing={2}>
            <Grid item xs={6}>
              <TextField
                label="Target Company"
                value={coverLetterForm.targetCompany ?? ''}
                onChange={(e) => setCoverLetterForm({ ...coverLetterForm, targetCompany: e.target.value })}
                fullWidth
              />
            </Grid>
            <Grid item xs={6}>
              <TextField
                label="Target Role"
                value={coverLetterForm.targetRole ?? ''}
                onChange={(e) => setCoverLetterForm({ ...coverLetterForm, targetRole: e.target.value })}
                fullWidth
              />
            </Grid>
          </Grid>
          <Grid container spacing={2}>
            <Grid item xs={6}>
              <TextField
                label="Recipient Name (Optional)"
                placeholder="e.g. Jane Doe / Hiring Team"
                value={coverLetterForm.recipientName ?? ''}
                onChange={(e) => setCoverLetterForm({ ...coverLetterForm, recipientName: e.target.value })}
                fullWidth
              />
            </Grid>
            <Grid item xs={6}>
              <TextField
                label="Recipient Title (Optional)"
                placeholder="e.g. VP of Engineering"
                value={coverLetterForm.recipientTitle ?? ''}
                onChange={(e) => setCoverLetterForm({ ...coverLetterForm, recipientTitle: e.target.value })}
                fullWidth
              />
            </Grid>
          </Grid>
          <FormControl fullWidth>
            <InputLabel>Link to Profile Strategy (Optional)</InputLabel>
            <Select
              value={coverLetterForm.resumeProfileId ?? ''}
              label="Link to Profile Strategy (Optional)"
              onChange={(e) => setCoverLetterForm({ ...coverLetterForm, resumeProfileId: e.target.value ? e.target.value : undefined })}
            >
              <MenuItem value=""><em>None / General</em></MenuItem>
              {profiles.map((p) => (
                <MenuItem key={p.id} value={p.id}>{p.name}</MenuItem>
              ))}
            </Select>
          </FormControl>
          <TextField
            label="Cover Letter Body"
            value={coverLetterForm.body}
            onChange={(e) => setCoverLetterForm({ ...coverLetterForm, body: e.target.value })}
            multiline
            rows={8}
            fullWidth
            required
          />
          <FormControlLabel
            control={<Switch checked={coverLetterForm.isTemplate} onChange={(e) => setCoverLetterForm({ ...coverLetterForm, isTemplate: e.target.checked })} />}
            label="Save as Reusable Template"
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setCoverLetterDialogOpen(false)}>Cancel</Button>
          <Button variant="contained" onClick={handleSaveCoverLetter} disabled={!coverLetterForm.title || !coverLetterForm.body}>
            Save Letter
          </Button>
        </DialogActions>
      </Dialog>

      {/* DIALOG: Create / Edit Portfolio */}
      <Dialog open={portfolioDialogOpen} onClose={() => setPortfolioDialogOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>{editPortfolioId ? 'Edit Portfolio Variation' : 'New Portfolio Variation'}</DialogTitle>
        <DialogContent sx={{ display: 'flex', flexDirection: 'column', gap: 2, pt: 2 }}>
          <TextField
            label="Portfolio Name"
            placeholder="e.g. Distributed Systems & Cloud Case Studies"
            value={portfolioForm.name}
            onChange={(e) => setPortfolioForm({ ...portfolioForm, name: e.target.value })}
            fullWidth
            required
          />
          <TextField
            label="URL / Link"
            placeholder="https://github.com/your-username or portfolio URL"
            value={portfolioForm.url ?? ''}
            onChange={(e) => setPortfolioForm({ ...portfolioForm, url: e.target.value })}
            fullWidth
          />
          <TextField
            label="Target Role / Audience"
            placeholder="e.g. Platform Engineers, System Architects"
            value={portfolioForm.targetRole ?? ''}
            onChange={(e) => setPortfolioForm({ ...portfolioForm, targetRole: e.target.value })}
            fullWidth
          />
          <TextField
            label="Description"
            value={portfolioForm.description ?? ''}
            onChange={(e) => setPortfolioForm({ ...portfolioForm, description: e.target.value })}
            multiline
            rows={2}
            fullWidth
          />
          <TextField
            label="Featured Projects (Markdown or bullet points)"
            placeholder="- Distributed Key-Value Store in Go&#10;- CareerOS Local-First Platform"
            value={portfolioForm.featuredProjects ?? ''}
            onChange={(e) => setPortfolioForm({ ...portfolioForm, featuredProjects: e.target.value })}
            multiline
            rows={3}
            fullWidth
          />
          <FormControlLabel
            control={<Switch checked={portfolioForm.isDefault} onChange={(e) => setPortfolioForm({ ...portfolioForm, isDefault: e.target.checked })} />}
            label="Set as Default Portfolio"
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setPortfolioDialogOpen(false)}>Cancel</Button>
          <Button variant="contained" onClick={handleSavePortfolio} disabled={!portfolioForm.name}>
            Save Portfolio
          </Button>
        </DialogActions>
      </Dialog>

      {/* DIALOG: Log Submission */}
      <Dialog open={submissionDialogOpen} onClose={() => setSubmissionDialogOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>Log Resume Submission & Capture Snapshot</DialogTitle>
        <DialogContent sx={{ display: 'flex', flexDirection: 'column', gap: 2, pt: 2 }}>
          <Alert severity="info" sx={{ mb: 1 }}>
            An immutable snapshot of the selected resume version, cover letter, and portfolio will be frozen at the moment of submission.
          </Alert>
          <Grid container spacing={2}>
            <Grid item xs={6}>
              <TextField
                label="Company Name"
                placeholder="e.g. Stripe"
                value={submissionForm.companyName}
                onChange={(e) => setSubmissionForm({ ...submissionForm, companyName: e.target.value })}
                fullWidth
                required
              />
            </Grid>
            <Grid item xs={6}>
              <TextField
                label="Role Title"
                placeholder="e.g. Staff Software Engineer"
                value={submissionForm.roleTitle}
                onChange={(e) => setSubmissionForm({ ...submissionForm, roleTitle: e.target.value })}
                fullWidth
                required
              />
            </Grid>
          </Grid>
          <Grid container spacing={2}>
            <Grid item xs={6}>
              <FormControl fullWidth required>
                <InputLabel>Strategy Profile</InputLabel>
                <Select
                  value={submissionForm.resumeProfileId}
                  label="Strategy Profile"
                  onChange={(e) => {
                    const newProfId = e.target.value
                    const prof = profiles.find((p) => p.id === newProfId)
                    setSubmissionForm({
                      ...submissionForm,
                      resumeProfileId: newProfId,
                      resumeVersionId: prof?.versions?.[0]?.id ?? '',
                    })
                  }}
                >
                  {profiles.map((p) => (
                    <MenuItem key={p.id} value={p.id}>{p.name} ({p.targetRole})</MenuItem>
                  ))}
                </Select>
              </FormControl>
            </Grid>
            <Grid item xs={6}>
              <FormControl fullWidth required>
                <InputLabel>Resume Version</InputLabel>
                <Select
                  value={submissionForm.resumeVersionId}
                  label="Resume Version"
                  onChange={(e) => setSubmissionForm({ ...submissionForm, resumeVersionId: e.target.value })}
                >
                  {selectedProfileObj?.versions?.map((v) => (
                    <MenuItem key={v.id} value={v.id}>{v.versionName} (v{v.versionNumber})</MenuItem>
                  ))}
                </Select>
              </FormControl>
            </Grid>
          </Grid>
          <Grid container spacing={2}>
            <Grid item xs={6}>
              <FormControl fullWidth>
                <InputLabel>Channel</InputLabel>
                <Select
                  value={submissionForm.channel}
                  label="Channel"
                  onChange={(e) => setSubmissionForm({ ...submissionForm, channel: e.target.value })}
                >
                  <MenuItem value="LinkedIn">LinkedIn</MenuItem>
                  <MenuItem value="Company Website">Company Website</MenuItem>
                  <MenuItem value="Referral">Referral</MenuItem>
                  <MenuItem value="Recruiter">Recruiter / Headhunter</MenuItem>
                  <MenuItem value="Job Board">Job Board (Indeed, etc.)</MenuItem>
                  <MenuItem value="Email">Direct Email</MenuItem>
                </Select>
              </FormControl>
            </Grid>
            <Grid item xs={6}>
              <FormControl fullWidth>
                <InputLabel>Status</InputLabel>
                <Select
                  value={submissionForm.status}
                  label="Status"
                  onChange={(e) => setSubmissionForm({ ...submissionForm, status: e.target.value })}
                >
                  <MenuItem value="Submitted">Submitted</MenuItem>
                  <MenuItem value="Viewed">Viewed / Opened</MenuItem>
                  <MenuItem value="Screening">Recruiter Screen</MenuItem>
                  <MenuItem value="Interviewing">Technical / Onsite</MenuItem>
                  <MenuItem value="Offer">Offer Received</MenuItem>
                  <MenuItem value="Rejected">Rejected</MenuItem>
                </Select>
              </FormControl>
            </Grid>
          </Grid>
          <Grid container spacing={2}>
            <Grid item xs={6}>
              <FormControl fullWidth>
                <InputLabel>Cover Letter (Optional)</InputLabel>
                <Select
                  value={submissionForm.coverLetterId ?? ''}
                  label="Cover Letter (Optional)"
                  onChange={(e) => setSubmissionForm({ ...submissionForm, coverLetterId: e.target.value ? e.target.value : undefined })}
                >
                  <MenuItem value=""><em>None</em></MenuItem>
                  {coverLetters.map((c) => (
                    <MenuItem key={c.id} value={c.id}>{c.title}</MenuItem>
                  ))}
                </Select>
              </FormControl>
            </Grid>
            <Grid item xs={6}>
              <FormControl fullWidth>
                <InputLabel>Portfolio (Optional)</InputLabel>
                <Select
                  value={submissionForm.portfolioVersionId ?? ''}
                  label="Portfolio (Optional)"
                  onChange={(e) => setSubmissionForm({ ...submissionForm, portfolioVersionId: e.target.value ? e.target.value : undefined })}
                >
                  <MenuItem value=""><em>None</em></MenuItem>
                  {portfolios.map((p) => (
                    <MenuItem key={p.id} value={p.id}>{p.name}</MenuItem>
                  ))}
                </Select>
              </FormControl>
            </Grid>
          </Grid>
          <TextField
            label="Submission Notes (Optional)"
            placeholder="Application URL, recruiter name, or referral contact notes..."
            value={submissionForm.notes ?? ''}
            onChange={(e) => setSubmissionForm({ ...submissionForm, notes: e.target.value })}
            multiline
            rows={2}
            fullWidth
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setSubmissionDialogOpen(false)}>Cancel</Button>
          <Button
            variant="contained"
            onClick={handleSaveSubmission}
            disabled={!submissionForm.companyName || !submissionForm.roleTitle || !submissionForm.resumeProfileId || !submissionForm.resumeVersionId}
          >
            Freeze Snapshot & Record
          </Button>
        </DialogActions>
      </Dialog>

      {/* DIALOG: Immutable Submission Snapshot Viewer */}
      <Dialog open={snapshotDialogOpen} onClose={() => setSnapshotDialogOpen(false)} maxWidth="md" fullWidth>
        <DialogTitle sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
          <CameraAltIcon color="primary" />
          Immutable Submission Snapshot
        </DialogTitle>
        <DialogContent sx={{ pt: 2 }}>
          {snapshotLoading || !selectedSnapshot ? (
            <Box display="flex" justifyContent="center" py={4}><CircularProgress /></Box>
          ) : (
            <Box display="flex" flexDirection="column" gap={2}>
              <Alert severity="success" icon={<CameraAltIcon />}>
                Frozen on <strong>{new Date(selectedSnapshot.snapshotDate).toLocaleString()}</strong>.
                This record is immutable and preserves the exact state of what was submitted.
              </Alert>

              <Card variant="outlined">
                <CardContent>
                  <Typography variant="subtitle2" color="primary" fontWeight={700}>
                    Resume Strategy & Version
                  </Typography>
                  <Box display="flex" alignItems="center" gap={1} mt={0.5}>
                    <Typography variant="h6">{selectedSnapshot.resumeProfileName}</Typography>
                    <Chip size="small" label={`${selectedSnapshot.resumeVersionName} (v${selectedSnapshot.resumeVersionNumber})`} />
                  </Box>

                  {selectedSnapshot.resumeDocumentFileName && (
                    <Box mt={1} p={1} bgcolor="background.default" borderRadius={1}>
                      <Typography variant="caption" fontWeight={600} display="block">
                        Attached Document: {selectedSnapshot.resumeDocumentFileName}
                      </Typography>
                      {selectedSnapshot.resumeDocumentSha256 && (
                        <Typography variant="caption" color="text.secondary" sx={{ fontFamily: 'monospace' }}>
                          SHA-256: {selectedSnapshot.resumeDocumentSha256}
                        </Typography>
                      )}
                    </Box>
                  )}

                  {selectedSnapshot.resumeContentSnapshot && (
                    <Box mt={1.5} p={1.5} bgcolor="action.hover" borderRadius={1} maxHeight={140} sx={{ overflowY: 'auto' }}>
                      <Typography variant="caption" fontWeight={600} color="text.secondary" display="block">
                        Frozen Resume Text:
                      </Typography>
                      <Typography variant="body2" sx={{ whiteSpace: 'pre-wrap' }}>
                        {selectedSnapshot.resumeContentSnapshot}
                      </Typography>
                    </Box>
                  )}
                </CardContent>
              </Card>

              {selectedSnapshot.coverLetterTitle && (
                <Card variant="outlined">
                  <CardContent>
                    <Typography variant="subtitle2" color="primary" fontWeight={700}>
                      Cover Letter Submitted: {selectedSnapshot.coverLetterTitle}
                    </Typography>
                    {selectedSnapshot.coverLetterBodySnapshot && (
                      <Box mt={1} p={1.5} bgcolor="background.default" borderRadius={1} maxHeight={160} sx={{ overflowY: 'auto' }}>
                        <Typography variant="body2" sx={{ whiteSpace: 'pre-wrap', fontFamily: 'monospace' }}>
                          {selectedSnapshot.coverLetterBodySnapshot}
                        </Typography>
                      </Box>
                    )}
                  </CardContent>
                </Card>
              )}

              {selectedSnapshot.portfolioName && (
                <Card variant="outlined">
                  <CardContent>
                    <Typography variant="subtitle2" color="primary" fontWeight={700}>
                      Portfolio Submitted: {selectedSnapshot.portfolioName}
                    </Typography>
                    {selectedSnapshot.portfolioUrlSnapshot && (
                      <Typography variant="body2" color="text.secondary" mt={0.5}>
                        URL: <a href={selectedSnapshot.portfolioUrlSnapshot} target="_blank" rel="noopener noreferrer">{selectedSnapshot.portfolioUrlSnapshot}</a>
                      </Typography>
                    )}
                  </CardContent>
                </Card>
              )}

              {selectedSnapshot.metadataJson && (
                <Box p={1} bgcolor="background.default" borderRadius={1}>
                  <Typography variant="caption" color="text.secondary" fontWeight={600} display="block">
                    ATS & Tailoring Metadata:
                  </Typography>
                  <Typography variant="caption" sx={{ fontFamily: 'monospace' }}>
                    {selectedSnapshot.metadataJson}
                  </Typography>
                </Box>
              )}
            </Box>
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setSnapshotDialogOpen(false)}>Close</Button>
        </DialogActions>
      </Dialog>
    </Box>
  )
}
