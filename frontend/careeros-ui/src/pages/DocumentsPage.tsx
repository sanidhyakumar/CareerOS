import { useEffect, useState } from 'react'
import {
  Box,
  Typography,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  IconButton,
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  Chip,
  CircularProgress,
  Alert,
  Tooltip,
  Stack,
  Tabs,
  Tab,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Grid,
} from '@mui/material'
import UploadFileIcon from '@mui/icons-material/UploadFile'
import DownloadIcon from '@mui/icons-material/Download'
import HistoryIcon from '@mui/icons-material/History'
import DriveFileMoveIcon from '@mui/icons-material/DriveFileMove'
import DeleteIcon from '@mui/icons-material/Delete'
import SearchIcon from '@mui/icons-material/Search'
import WarningAmberIcon from '@mui/icons-material/WarningAmber'
import ErrorOutlineIcon from '@mui/icons-material/ErrorOutline'
import LinkIcon from '@mui/icons-material/Link'
import AddLinkIcon from '@mui/icons-material/AddLink'
import DeleteOutlineIcon from '@mui/icons-material/DeleteOutline'
import BadgeIcon from '@mui/icons-material/Badge'
import SchoolIcon from '@mui/icons-material/School'
import WorkspacePremiumIcon from '@mui/icons-material/WorkspacePremium'
import WorkOutlineIcon from '@mui/icons-material/WorkOutline'
import FlightTakeoffIcon from '@mui/icons-material/FlightTakeoff'
import FolderSpecialIcon from '@mui/icons-material/FolderSpecial'

import {
  documentsApi,
  DocumentCategory,
  type Document,
  type DocumentVersion,
  type DocumentUsage,
} from '../api/documents'
import { countriesApi, type Country } from '../api/countries'

function formatBytes(bytes: number, decimals = 2) {
  if (bytes === 0) return '0 Bytes'
  const k = 1024
  const dm = decimals < 0 ? 0 : decimals
  const sizes = ['Bytes', 'KB', 'MB', 'GB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  return parseFloat((bytes / Math.pow(k, i)).toFixed(dm)) + ' ' + sizes[i]
}

const getCategoryColor = (cat: DocumentCategory): 'default' | 'primary' | 'secondary' | 'error' | 'info' | 'success' | 'warning' => {
  switch (cat) {
    case DocumentCategory.Identity:
      return 'primary'
    case DocumentCategory.Academics:
      return 'info'
    case DocumentCategory.Certifications:
      return 'success'
    case DocumentCategory.Employment:
      return 'secondary'
    case DocumentCategory.Immigration:
      return 'warning'
    default:
      return 'default'
  }
}

const getCategoryIcon = (cat: DocumentCategory) => {
  switch (cat) {
    case DocumentCategory.Identity:
      return <BadgeIcon fontSize="small" />
    case DocumentCategory.Academics:
      return <SchoolIcon fontSize="small" />
    case DocumentCategory.Certifications:
      return <WorkspacePremiumIcon fontSize="small" />
    case DocumentCategory.Employment:
      return <WorkOutlineIcon fontSize="small" />
    case DocumentCategory.Immigration:
      return <FlightTakeoffIcon fontSize="small" />
    default:
      return <FolderSpecialIcon fontSize="small" />
  }
}

export default function DocumentsPage() {
  const [documents, setDocuments] = useState<Document[]>([])
  const [countries, setCountries] = useState<Country[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  // Filters
  const [search, setSearch] = useState('')
  const [selectedCategoryTab, setSelectedCategoryTab] = useState<number>(0) // 0 = All
  const [countryFilter, setCountryFilter] = useState<string>('')
  const [expiryFilter, setExpiryFilter] = useState<'all' | 'expired' | 'expiringSoon'>('all')

  // Upload modal state
  const [uploadOpen, setUploadOpen] = useState(false)
  const [selectedFile, setSelectedFile] = useState<File | null>(null)
  const [uploadCategory, setUploadCategory] = useState<DocumentCategory>(DocumentCategory.General)
  const [description, setDescription] = useState('')
  const [documentNumber, setDocumentNumber] = useState('')
  const [issuingAuthority, setIssuingAuthority] = useState('')
  const [issueDate, setIssueDate] = useState('')
  const [expiryDate, setExpiryDate] = useState('')
  const [countryId, setCountryId] = useState('')
  const [subFolder, setSubFolder] = useState('')
  const [uploading, setUploading] = useState(false)

  // Version modal state
  const [versionDoc, setVersionDoc] = useState<Document | null>(null)
  const [versionFile, setVersionFile] = useState<File | null>(null)
  const [versionDesc, setVersionDesc] = useState('')
  const [versionDocNumber, setVersionDocNumber] = useState('')
  const [versionAuthority, setVersionAuthority] = useState('')
  const [versionExpiry, setVersionExpiry] = useState('')
  const [versionHistory, setVersionHistory] = useState<DocumentVersion[]>([])
  const [historyOpen, setHistoryOpen] = useState(false)

  // Move modal state
  const [moveDoc, setMoveDoc] = useState<Document | null>(null)
  const [targetFolder, setTargetFolder] = useState('')
  const [moving, setMoving] = useState(false)

  // Usage modal state
  const [usageDoc, setUsageDoc] = useState<Document | null>(null)
  const [usages, setUsages] = useState<DocumentUsage[]>([])
  const [usageOpen, setUsageOpen] = useState(false)
  const [usageEntityType, setUsageEntityType] = useState('JobApplication')
  const [usageEntityName, setUsageEntityName] = useState('')
  const [usagePurpose, setUsagePurpose] = useState('')
  const [addingUsage, setAddingUsage] = useState(false)

  const loadDocuments = () => {
    setLoading(true)
    const category = selectedCategoryTab === 0 ? undefined : (selectedCategoryTab - 1 as DocumentCategory)
    const isExpired = expiryFilter === 'expired' ? true : undefined
    const expiringWithinDays = expiryFilter === 'expiringSoon' ? 90 : undefined

    documentsApi.getAll({
      searchTerm: search || undefined,
      category,
      countryId: countryFilter || undefined,
      isExpired,
      expiringWithinDays,
      latestOnly: true,
    })
      .then(setDocuments)
      .catch(() => setError('Failed to load documents'))
      .finally(() => setLoading(false))
  }

  useEffect(() => {
    countriesApi.getAll().then(setCountries).catch(() => {})
  }, [])

  useEffect(() => {
    loadDocuments()
  }, [selectedCategoryTab, countryFilter, expiryFilter])

  const handleSearchSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    loadDocuments()
  }

  // Upload handler
  const handleUpload = async () => {
    if (!selectedFile) return
    setUploading(true)
    try {
      await documentsApi.upload({
        file: selectedFile,
        description,
        category: uploadCategory,
        documentNumber: documentNumber || undefined,
        issuingAuthority: issuingAuthority || undefined,
        issueDate: issueDate ? new Date(issueDate).toISOString() : null,
        expiryDate: expiryDate ? new Date(expiryDate).toISOString() : null,
        countryId: countryId || undefined,
        subFolder: subFolder || undefined,
      })
      setUploadOpen(false)
      setSelectedFile(null)
      setDescription('')
      setDocumentNumber('')
      setIssuingAuthority('')
      setIssueDate('')
      setExpiryDate('')
      setCountryId('')
      setSubFolder('')
      loadDocuments()
    } catch {
      setError('Upload failed. Please check file and server status.')
    } finally {
      setUploading(false)
    }
  }

  // Version handlers
  const handleUploadVersion = async () => {
    if (!versionDoc || !versionFile) return
    setUploading(true)
    try {
      await documentsApi.uploadVersion({
        id: versionDoc.id,
        file: versionFile,
        description: versionDesc,
        category: versionDoc.category,
        documentNumber: versionDocNumber || undefined,
        issuingAuthority: versionAuthority || undefined,
        expiryDate: versionExpiry ? new Date(versionExpiry).toISOString() : null,
      })
      setVersionDoc(null)
      setVersionFile(null)
      setVersionDesc('')
      setVersionDocNumber('')
      setVersionAuthority('')
      setVersionExpiry('')
      loadDocuments()
    } catch {
      setError('Failed to upload new version.')
    } finally {
      setUploading(false)
    }
  }

  const handleOpenHistory = async (doc: Document) => {
    try {
      const history = await documentsApi.getVersions(doc.id)
      setVersionHistory(history)
      setVersionDoc(doc)
      setHistoryOpen(true)
    } catch {
      setError('Failed to load version history.')
    }
  }

  // Move handler
  const handleMove = async () => {
    if (!moveDoc || !targetFolder.trim()) return
    setMoving(true)
    try {
      await documentsApi.move(moveDoc.id, targetFolder.trim())
      setMoveDoc(null)
      setTargetFolder('')
      loadDocuments()
    } catch {
      setError('Failed to move document.')
    } finally {
      setMoving(false)
    }
  }

  // Delete handler
  const handleDelete = async (id: string) => {
    if (!confirm('Are you sure you want to delete this document family and all its physical versions?')) return
    try {
      await documentsApi.delete(id, true)
      loadDocuments()
    } catch {
      setError('Failed to delete document.')
    }
  }

  // Usage Tracking handlers
  const handleOpenUsages = async (doc: Document) => {
    setUsageDoc(doc)
    try {
      const uList = await documentsApi.getUsages(doc.id)
      setUsages(uList)
      setUsageOpen(true)
    } catch {
      setError('Failed to load document usage history.')
    }
  }

  const handleAddUsage = async () => {
    if (!usageDoc || !usageEntityType.trim()) return
    setAddingUsage(true)
    try {
      await documentsApi.addUsage(usageDoc.id, {
        entityType: usageEntityType,
        entityId: '00000000-0000-0000-0000-000000000000',
        entityName: usageEntityName || undefined,
        purpose: usagePurpose || undefined,
      })
      const refreshed = await documentsApi.getUsages(usageDoc.id)
      setUsages(refreshed)
      setUsageEntityName('')
      setUsagePurpose('')
      loadDocuments()
    } catch {
      setError('Failed to record document usage.')
    } finally {
      setAddingUsage(false)
    }
  }

  const handleDeleteUsage = async (usageId: string) => {
    if (!usageDoc) return
    try {
      await documentsApi.deleteUsage(usageId)
      const refreshed = await documentsApi.getUsages(usageDoc.id)
      setUsages(refreshed)
      loadDocuments()
    } catch {
      setError('Failed to remove usage.')
    }
  }

  // Expiry stats
  const expiredDocsCount = documents.filter((d) => d.isExpired).length
  const expiringSoonCount = documents.filter((d) => d.isExpiringSoon).length

  return (
    <Box sx={{ p: { xs: 2, md: 3 }, maxWidth: 1400, mx: 'auto' }}>
      {/* Header */}
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', mb: 3, flexWrap: 'wrap', gap: 2 }}>
        <Box>
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 1.5 }}>
            <FolderSpecialIcon color="primary" sx={{ fontSize: 36 }} />
            <Typography variant="h4" fontWeight={800}>
              Document Vault
            </Typography>
          </Box>
          <Typography variant="body1" color="text.secondary" sx={{ mt: 0.5 }}>
            Encrypted local filesystem storage for Identity, Academics, Certifications, Employment, and Immigration documents.
          </Typography>
        </Box>
        <Button
          variant="contained"
          startIcon={<UploadFileIcon />}
          onClick={() => setUploadOpen(true)}
          sx={{ height: 42, px: 3, fontWeight: 700 }}
        >
          Upload Document
        </Button>
      </Box>

      {/* Expiry Warning Banner if documents are expiring */}
      {(expiredDocsCount > 0 || expiringSoonCount > 0) && (
        <Alert
          severity={expiredDocsCount > 0 ? 'error' : 'warning'}
          icon={<WarningAmberIcon />}
          sx={{ mb: 3, borderRadius: 2 }}
          action={
            <Button
              color="inherit"
              size="small"
              onClick={() => setExpiryFilter(expiredDocsCount > 0 ? 'expired' : 'expiringSoon')}
            >
              View Expiring
            </Button>
          }
        >
          <strong>Expiry Alert:</strong> {expiredDocsCount > 0 && `${expiredDocsCount} document(s) expired.`}{' '}
          {expiringSoonCount > 0 && `${expiringSoonCount} document(s) expiring within 90 days.`}
        </Alert>
      )}

      {error && (
        <Alert severity="error" onClose={() => setError(null)} sx={{ mb: 3 }}>
          {error}
        </Alert>
      )}

      {/* Category Tabs */}
      <Paper sx={{ mb: 3, borderRadius: 2 }}>
        <Tabs
          value={selectedCategoryTab}
          onChange={(_, newVal) => setSelectedCategoryTab(newVal)}
          variant="scrollable"
          scrollButtons="auto"
          textColor="primary"
          indicatorColor="primary"
          sx={{ borderBottom: 1, borderColor: 'divider' }}
        >
          <Tab label="All Documents" icon={<FolderSpecialIcon />} iconPosition="start" />
          <Tab label="Identity" icon={<BadgeIcon />} iconPosition="start" />
          <Tab label="Academics" icon={<SchoolIcon />} iconPosition="start" />
          <Tab label="Certifications" icon={<WorkspacePremiumIcon />} iconPosition="start" />
          <Tab label="Employment" icon={<WorkOutlineIcon />} iconPosition="start" />
          <Tab label="Immigration" icon={<FlightTakeoffIcon />} iconPosition="start" />
        </Tabs>
      </Paper>

      {/* Search & Filter Toolbar */}
      <Paper sx={{ p: 2, mb: 3, borderRadius: 2 }}>
        <Grid container spacing={2} alignItems="center">
          <Grid item xs={12} md={5}>
            <Box component="form" onSubmit={handleSearchSubmit} sx={{ display: 'flex', gap: 1 }}>
              <TextField
                fullWidth
                size="small"
                placeholder="Search by file name, issuer, document number, SHA256..."
                value={search}
                onChange={(e) => setSearch(e.target.value)}
                InputProps={{
                  startAdornment: <SearchIcon color="action" sx={{ mr: 1 }} />,
                }}
              />
              <Button type="submit" variant="outlined">
                Search
              </Button>
            </Box>
          </Grid>

          <Grid item xs={6} md={3.5}>
            <FormControl fullWidth size="small">
              <InputLabel id="expiry-filter-label">Expiration Status</InputLabel>
              <Select
                labelId="expiry-filter-label"
                label="Expiration Status"
                value={expiryFilter}
                onChange={(e) => setExpiryFilter(e.target.value as any)}
              >
                <MenuItem value="all">All Expiration Statuses</MenuItem>
                <MenuItem value="expiringSoon">Expiring Soon (Next 90 Days)</MenuItem>
                <MenuItem value="expired">Expired Documents Only</MenuItem>
              </Select>
            </FormControl>
          </Grid>

          <Grid item xs={6} md={3.5}>
            <FormControl fullWidth size="small">
              <InputLabel id="country-filter-label">Country Filter</InputLabel>
              <Select
                labelId="country-filter-label"
                label="Country Filter"
                value={countryFilter}
                onChange={(e) => setCountryFilter(e.target.value)}
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
        </Grid>
      </Paper>

      {/* Document List Table */}
      <TableContainer component={Paper} sx={{ borderRadius: 2 }}>
        <Table sx={{ minWidth: 850 }}>
          <TableHead sx={{ bgcolor: 'rgba(255,255,255,0.03)' }}>
            <TableRow>
              <TableCell>Document Name & Category</TableCell>
              <TableCell>Identifier / Authority</TableCell>
              <TableCell>Expiration Status</TableCell>
              <TableCell>Version</TableCell>
              <TableCell>Usage</TableCell>
              <TableCell>Size / Hash</TableCell>
              <TableCell align="right">Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {loading ? (
              <TableRow>
                <TableCell colSpan={7} align="center" sx={{ py: 6 }}>
                  <CircularProgress size={32} />
                  <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
                    Loading documents from storage engine...
                  </Typography>
                </TableCell>
              </TableRow>
            ) : documents.length === 0 ? (
              <TableRow>
                <TableCell colSpan={7} align="center" sx={{ py: 6 }}>
                  <Typography variant="subtitle1" fontWeight={600} color="text.secondary">
                    No documents found.
                  </Typography>
                  <Typography variant="body2" color="text.secondary" sx={{ mt: 0.5 }}>
                    Click "Upload Document" to archive and track important records.
                  </Typography>
                </TableCell>
              </TableRow>
            ) : (
              documents.map((doc) => (
                <TableRow key={doc.id} hover>
                  <TableCell>
                    <Box sx={{ display: 'flex', alignItems: 'flex-start', gap: 1.5 }}>
                      <Box sx={{ mt: 0.5 }}>{getCategoryIcon(doc.category)}</Box>
                      <Box>
                        <Typography variant="subtitle2" fontWeight={700}>
                          {doc.originalFileName}
                        </Typography>
                        <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.8, mt: 0.5 }}>
                          <Chip
                            label={doc.categoryName}
                            color={getCategoryColor(doc.category)}
                            size="small"
                          />
                          {doc.countryName && (
                            <Chip label={doc.countryName} size="small" variant="outlined" />
                          )}
                        </Box>
                        {doc.description && (
                          <Typography variant="caption" color="text.secondary" sx={{ display: 'block', mt: 0.5 }}>
                            {doc.description}
                          </Typography>
                        )}
                      </Box>
                    </Box>
                  </TableCell>

                  <TableCell>
                    {doc.documentNumber ? (
                      <Typography variant="body2" fontWeight={600}>
                        #{doc.documentNumber}
                      </Typography>
                    ) : (
                      <Typography variant="body2" color="text.disabled">—</Typography>
                    )}
                    {doc.issuingAuthority && (
                      <Typography variant="caption" color="text.secondary" display="block">
                        Issued by: {doc.issuingAuthority}
                      </Typography>
                    )}
                  </TableCell>

                  <TableCell>
                    {doc.expiryDate ? (
                      <Box>
                        {doc.isExpired ? (
                          <Chip
                            icon={<ErrorOutlineIcon />}
                            label="Expired"
                            color="error"
                            size="small"
                          />
                        ) : doc.isExpiringSoon ? (
                          <Chip
                            icon={<WarningAmberIcon />}
                            label={`Expires in ${doc.daysUntilExpiry}d`}
                            color="warning"
                            size="small"
                          />
                        ) : (
                          <Chip
                            label={new Date(doc.expiryDate).toLocaleDateString()}
                            color="success"
                            size="small"
                            variant="outlined"
                          />
                        )}
                        <Typography variant="caption" color="text.secondary" display="block" sx={{ mt: 0.5 }}>
                          Exp: {new Date(doc.expiryDate).toLocaleDateString()}
                        </Typography>
                      </Box>
                    ) : (
                      <Chip label="No Expiry" size="small" variant="outlined" sx={{ opacity: 0.6 }} />
                    )}
                  </TableCell>

                  <TableCell>
                    <Chip
                      label={`v${doc.version}`}
                      color={doc.isLatestVersion ? 'primary' : 'default'}
                      size="small"
                      onClick={() => handleOpenHistory(doc)}
                      sx={{ cursor: 'pointer' }}
                    />
                  </TableCell>

                  <TableCell>
                    <Tooltip title="View or manage places where this document is used">
                      <Button
                        size="small"
                        startIcon={<LinkIcon fontSize="small" />}
                        onClick={() => handleOpenUsages(doc)}
                        variant="outlined"
                        sx={{ textTransform: 'none', borderRadius: 2 }}
                      >
                        {doc.usageCount} Usages
                      </Button>
                    </Tooltip>
                  </TableCell>

                  <TableCell>
                    <Typography variant="body2">{formatBytes(doc.fileSizeBytes)}</Typography>
                    <Tooltip title={`SHA256: ${doc.sha256Hash}`}>
                      <Typography
                        variant="caption"
                        sx={{
                          fontFamily: 'monospace',
                          color: 'text.secondary',
                          cursor: 'pointer',
                        }}
                      >
                        {doc.sha256Hash.substring(0, 10)}...
                      </Typography>
                    </Tooltip>
                  </TableCell>

                  <TableCell align="right">
                    <Stack direction="row" spacing={0.5} justifyContent="flex-end">
                      <Tooltip title="Download File">
                        <IconButton
                          size="small"
                          color="primary"
                          component="a"
                          href={documentsApi.getDownloadUrl(doc.id)}
                          download={doc.originalFileName}
                        >
                          <DownloadIcon fontSize="small" />
                        </IconButton>
                      </Tooltip>
                      <Tooltip title="Upload New Version">
                        <IconButton
                          size="small"
                          color="secondary"
                          onClick={() => {
                            setVersionDoc(doc)
                            setVersionDocNumber(doc.documentNumber || '')
                            setVersionAuthority(doc.issuingAuthority || '')
                            setVersionExpiry(doc.expiryDate ? doc.expiryDate.substring(0, 10) : '')
                          }}
                        >
                          <HistoryIcon fontSize="small" />
                        </IconButton>
                      </Tooltip>
                      <Tooltip title="Move Folder">
                        <IconButton
                          size="small"
                          onClick={() => {
                            setMoveDoc(doc)
                            setTargetFolder(doc.storagePath.substring(0, doc.storagePath.lastIndexOf('/')))
                          }}
                        >
                          <DriveFileMoveIcon fontSize="small" />
                        </IconButton>
                      </Tooltip>
                      <Tooltip title="Delete Document">
                        <IconButton size="small" color="error" onClick={() => handleDelete(doc.id)}>
                          <DeleteIcon fontSize="small" />
                        </IconButton>
                      </Tooltip>
                    </Stack>
                  </TableCell>
                </TableRow>
              ))
            )}
          </TableBody>
        </Table>
      </TableContainer>

      {/* DIALOG 1: Upload Document Modal */}
      <Dialog open={uploadOpen} onClose={() => setUploadOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>Upload Document to Vault</DialogTitle>
        <DialogContent dividers>
          <Grid container spacing={2} sx={{ mt: 0.5 }}>
            <Grid item xs={12}>
              <Box
                sx={{
                  border: '2px dashed rgba(255,255,255,0.2)',
                  borderRadius: 2,
                  p: 3,
                  textAlign: 'center',
                  bgcolor: 'rgba(255,255,255,0.02)',
                  cursor: 'pointer',
                  '&:hover': { borderColor: 'primary.main' },
                }}
                onClick={() => document.getElementById('vault-file-input')?.click()}
              >
                <input
                  id="vault-file-input"
                  type="file"
                  style={{ display: 'none' }}
                  onChange={(e) => setSelectedFile(e.target.files?.[0] || null)}
                />
                <UploadFileIcon sx={{ fontSize: 48, color: 'primary.main', mb: 1 }} />
                <Typography variant="subtitle1" fontWeight={700}>
                  {selectedFile ? selectedFile.name : 'Click to select or drag and drop file'}
                </Typography>
                <Typography variant="caption" color="text.secondary">
                  {selectedFile ? `${formatBytes(selectedFile.size)} • Ready to upload` : 'PDF, DOCX, PNG, JPG, or ZIP (Local storage)'}
                </Typography>
              </Box>
            </Grid>

            <Grid item xs={12} sm={6}>
              <FormControl fullWidth required>
                <InputLabel id="upload-category-label">Category</InputLabel>
                <Select
                  labelId="upload-category-label"
                  label="Category"
                  value={uploadCategory}
                  onChange={(e) => setUploadCategory(Number(e.target.value) as DocumentCategory)}
                >
                  <MenuItem value={DocumentCategory.Identity}>Identity (Passports, ID, License)</MenuItem>
                  <MenuItem value={DocumentCategory.Academics}>Academics (Degrees, Transcripts)</MenuItem>
                  <MenuItem value={DocumentCategory.Certifications}>Certifications (AWS, Licenses)</MenuItem>
                  <MenuItem value={DocumentCategory.Employment}>Employment (Offer, Experience, Pay)</MenuItem>
                  <MenuItem value={DocumentCategory.Immigration}>Immigration (Visas, Permits, I-797)</MenuItem>
                  <MenuItem value={DocumentCategory.General}>General / Other</MenuItem>
                </Select>
              </FormControl>
            </Grid>

            <Grid item xs={12} sm={6}>
              <FormControl fullWidth>
                <InputLabel id="upload-country-label">Related Country</InputLabel>
                <Select
                  labelId="upload-country-label"
                  label="Related Country"
                  value={countryId}
                  onChange={(e) => setCountryId(e.target.value)}
                >
                  <MenuItem value="">None / Not Applicable</MenuItem>
                  {countries.map((c) => (
                    <MenuItem key={c.id} value={c.id}>
                      {c.name} ({c.code})
                    </MenuItem>
                  ))}
                </Select>
              </FormControl>
            </Grid>

            <Grid item xs={12} sm={6}>
              <TextField
                fullWidth
                label="Document / Reference Number"
                placeholder="e.g. Passport or License No."
                value={documentNumber}
                onChange={(e) => setDocumentNumber(e.target.value)}
              />
            </Grid>

            <Grid item xs={12} sm={6}>
              <TextField
                fullWidth
                label="Issuing Authority"
                placeholder="e.g. Dept of State, AWS, Stanford"
                value={issuingAuthority}
                onChange={(e) => setIssuingAuthority(e.target.value)}
              />
            </Grid>

            <Grid item xs={12} sm={6}>
              <TextField
                fullWidth
                type="date"
                label="Issue Date"
                InputLabelProps={{ shrink: true }}
                value={issueDate}
                onChange={(e) => setIssueDate(e.target.value)}
              />
            </Grid>

            <Grid item xs={12} sm={6}>
              <TextField
                fullWidth
                type="date"
                label="Expiration Date"
                InputLabelProps={{ shrink: true }}
                value={expiryDate}
                onChange={(e) => setExpiryDate(e.target.value)}
              />
            </Grid>

            <Grid item xs={12}>
              <TextField
                fullWidth
                label="Description / Purpose Notes"
                value={description}
                onChange={(e) => setDescription(e.target.value)}
              />
            </Grid>
          </Grid>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setUploadOpen(false)}>Cancel</Button>
          <Button
            variant="contained"
            onClick={handleUpload}
            disabled={!selectedFile || uploading}
          >
            {uploading ? <CircularProgress size={24} /> : 'Save Document'}
          </Button>
        </DialogActions>
      </Dialog>

      {/* DIALOG 2: Upload New Version Modal */}
      <Dialog open={Boolean(versionDoc && !historyOpen)} onClose={() => setVersionDoc(null)} maxWidth="sm" fullWidth>
        <DialogTitle>Upload New Version for {versionDoc?.originalFileName}</DialogTitle>
        <DialogContent dividers>
          <Grid container spacing={2} sx={{ mt: 0.5 }}>
            <Grid item xs={12}>
              <Alert severity="info">
                Uploading a new version increments the document version counter, re-calculates the SHA-256 hash, and keeps full audit history of prior versions.
              </Alert>
            </Grid>
            <Grid item xs={12}>
              <Button variant="outlined" component="label" fullWidth sx={{ py: 1.5 }}>
                {versionFile ? versionFile.name : 'Select Updated File'}
                <input
                  type="file"
                  hidden
                  onChange={(e) => setVersionFile(e.target.files?.[0] || null)}
                />
              </Button>
            </Grid>
            <Grid item xs={12} sm={6}>
              <TextField
                fullWidth
                label="Document / Reference Number"
                value={versionDocNumber}
                onChange={(e) => setVersionDocNumber(e.target.value)}
              />
            </Grid>
            <Grid item xs={12} sm={6}>
              <TextField
                fullWidth
                label="Issuing Authority"
                value={versionAuthority}
                onChange={(e) => setVersionAuthority(e.target.value)}
              />
            </Grid>
            <Grid item xs={12}>
              <TextField
                fullWidth
                type="date"
                label="Expiration Date"
                InputLabelProps={{ shrink: true }}
                value={versionExpiry}
                onChange={(e) => setVersionExpiry(e.target.value)}
              />
            </Grid>
            <Grid item xs={12}>
              <TextField
                fullWidth
                label="Version Change Notes"
                placeholder="What changed in this version?"
                value={versionDesc}
                onChange={(e) => setVersionDesc(e.target.value)}
              />
            </Grid>
          </Grid>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setVersionDoc(null)}>Cancel</Button>
          <Button
            variant="contained"
            onClick={handleUploadVersion}
            disabled={!versionFile || uploading}
          >
            {uploading ? <CircularProgress size={24} /> : 'Upload Version'}
          </Button>
        </DialogActions>
      </Dialog>

      {/* DIALOG 3: Version History Modal */}
      <Dialog open={historyOpen} onClose={() => setHistoryOpen(false)} maxWidth="md" fullWidth>
        <DialogTitle>Version History: {versionDoc?.originalFileName}</DialogTitle>
        <DialogContent dividers>
          <Table size="small">
            <TableHead>
              <TableRow>
                <TableCell>Version</TableCell>
                <TableCell>Original File</TableCell>
                <TableCell>Size</TableCell>
                <TableCell>SHA256 Hash</TableCell>
                <TableCell>Uploaded At</TableCell>
                <TableCell align="right">Download</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {versionHistory.map((v) => (
                <TableRow key={v.id}>
                  <TableCell>
                    <Chip
                      label={`v${v.version}`}
                      color={v.isLatestVersion ? 'primary' : 'default'}
                      size="small"
                    />
                  </TableCell>
                  <TableCell>{v.originalFileName}</TableCell>
                  <TableCell>{formatBytes(v.fileSizeBytes)}</TableCell>
                  <TableCell sx={{ fontFamily: 'monospace', fontSize: '0.75rem' }}>
                    {v.sha256Hash.substring(0, 16)}...
                  </TableCell>
                  <TableCell>{new Date(v.createdAt).toLocaleString()}</TableCell>
                  <TableCell align="right">
                    <IconButton
                      size="small"
                      color="primary"
                      component="a"
                      href={documentsApi.getDownloadUrl(v.id)}
                      download={v.originalFileName}
                    >
                      <DownloadIcon fontSize="small" />
                    </IconButton>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setHistoryOpen(false)}>Close</Button>
        </DialogActions>
      </Dialog>

      {/* DIALOG 4: Move Document Modal */}
      <Dialog open={Boolean(moveDoc)} onClose={() => setMoveDoc(null)} maxWidth="xs" fullWidth>
        <DialogTitle>Move Document</DialogTitle>
        <DialogContent dividers>
          <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
            Specify the new target subfolder for <strong>{moveDoc?.originalFileName}</strong>:
          </Typography>
          <TextField
            fullWidth
            label="Target Subfolder"
            placeholder="e.g. archived/2026 or identity/passports"
            value={targetFolder}
            onChange={(e) => setTargetFolder(e.target.value)}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setMoveDoc(null)}>Cancel</Button>
          <Button variant="contained" onClick={handleMove} disabled={moving || !targetFolder.trim()}>
            {moving ? <CircularProgress size={24} /> : 'Move File'}
          </Button>
        </DialogActions>
      </Dialog>

      {/* DIALOG 5: Usage Tracking Modal */}
      <Dialog open={usageOpen} onClose={() => setUsageOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>Document Usage: {usageDoc?.originalFileName}</DialogTitle>
        <DialogContent dividers>
          <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
            Track everywhere this document has been shared, attached, or submitted (e.g. Visa proof, Job Applications, Profile).
          </Typography>

          <Box sx={{ mb: 3 }}>
            <Typography variant="subtitle2" fontWeight={700} gutterBottom>
              Record New Usage:
            </Typography>
            <Grid container spacing={1.5}>
              <Grid item xs={12} sm={4}>
                <FormControl fullWidth size="small">
                  <InputLabel>Type</InputLabel>
                  <Select
                    value={usageEntityType}
                    label="Type"
                    onChange={(e) => setUsageEntityType(e.target.value)}
                  >
                    <MenuItem value="JobApplication">Job Application</MenuItem>
                    <MenuItem value="WorkAuthorization">Work Visa Proof</MenuItem>
                    <MenuItem value="Company">Company</MenuItem>
                    <MenuItem value="Profile">Personal Profile</MenuItem>
                    <MenuItem value="Other">Other</MenuItem>
                  </Select>
                </FormControl>
              </Grid>
              <Grid item xs={12} sm={8}>
                <TextField
                  fullWidth
                  size="small"
                  label="Target Name / Title"
                  placeholder="e.g. Google - Staff Engineer Application"
                  value={usageEntityName}
                  onChange={(e) => setUsageEntityName(e.target.value)}
                />
              </Grid>
              <Grid item xs={12} sm={9}>
                <TextField
                  fullWidth
                  size="small"
                  label="Purpose / Context"
                  placeholder="e.g. Degree certificate verification"
                  value={usagePurpose}
                  onChange={(e) => setUsagePurpose(e.target.value)}
                />
              </Grid>
              <Grid item xs={12} sm={3}>
                <Button
                  fullWidth
                  variant="contained"
                  startIcon={<AddLinkIcon />}
                  onClick={handleAddUsage}
                  disabled={addingUsage}
                  sx={{ height: 40 }}
                >
                  Add
                </Button>
              </Grid>
            </Grid>
          </Box>

          <Typography variant="subtitle2" fontWeight={700} gutterBottom>
            Linked Usages ({usages.length}):
          </Typography>

          {usages.length === 0 ? (
            <Alert severity="info">
              No usage records yet. Track where you submit or reference this document using the form above.
            </Alert>
          ) : (
            <Stack spacing={1}>
              {usages.map((u) => (
                <Paper
                  key={u.id}
                  variant="outlined"
                  sx={{ p: 1.5, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}
                >
                  <Box>
                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                      <Chip label={u.entityType} size="small" color="primary" variant="outlined" />
                      <Typography variant="subtitle2" fontWeight={700}>
                        {u.entityName || 'Unnamed Record'}
                      </Typography>
                    </Box>
                    {u.purpose && (
                      <Typography variant="caption" color="text.secondary" display="block" sx={{ mt: 0.5 }}>
                        Purpose: {u.purpose}
                      </Typography>
                    )}
                    <Typography variant="caption" color="text.disabled" display="block">
                      Used on: {new Date(u.usedAt).toLocaleDateString()}
                    </Typography>
                  </Box>
                  <IconButton size="small" color="error" onClick={() => handleDeleteUsage(u.id)}>
                    <DeleteOutlineIcon fontSize="small" />
                  </IconButton>
                </Paper>
              ))}
            </Stack>
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setUsageOpen(false)}>Done</Button>
        </DialogActions>
      </Dialog>
    </Box>
  )
}
