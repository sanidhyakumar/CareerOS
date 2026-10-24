import { useEffect, useState } from 'react'
import {
  Box, Typography, Paper, Table, TableBody, TableCell, TableContainer,
  TableHead, TableRow, IconButton, Button, Dialog, DialogTitle,
  DialogContent, DialogActions, TextField, Chip, CircularProgress,
  Alert, Tooltip, Stack,
} from '@mui/material'
import UploadFileIcon from '@mui/icons-material/UploadFile'
import DownloadIcon from '@mui/icons-material/Download'
import HistoryIcon from '@mui/icons-material/History'
import DriveFileMoveIcon from '@mui/icons-material/DriveFileMove'
import DeleteIcon from '@mui/icons-material/Delete'
import DescriptionIcon from '@mui/icons-material/Description'
import SearchIcon from '@mui/icons-material/Search'
import { documentsApi, type Document, type DocumentVersion } from '../api/documents'

function formatBytes(bytes: number, decimals = 2) {
  if (bytes === 0) return '0 Bytes'
  const k = 1024
  const dm = decimals < 0 ? 0 : decimals
  const sizes = ['Bytes', 'KB', 'MB', 'GB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  return parseFloat((bytes / Math.pow(k, i)).toFixed(dm)) + ' ' + sizes[i]
}

export default function DocumentsPage() {
  const [documents, setDocuments] = useState<Document[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [search, setSearch] = useState('')

  // Upload modal state
  const [uploadOpen, setUploadOpen] = useState(false)
  const [selectedFile, setSelectedFile] = useState<File | null>(null)
  const [description, setDescription] = useState('')
  const [subFolder, setSubFolder] = useState('')
  const [uploading, setUploading] = useState(false)

  // Version modal state
  const [versionDoc, setVersionDoc] = useState<Document | null>(null)
  const [versionFile, setVersionFile] = useState<File | null>(null)
  const [versionDesc, setVersionDesc] = useState('')
  const [versionHistory, setVersionHistory] = useState<DocumentVersion[]>([])
  const [historyOpen, setHistoryOpen] = useState(false)

  // Move modal state
  const [moveDoc, setMoveDoc] = useState<Document | null>(null)
  const [targetFolder, setTargetFolder] = useState('')
  const [moving, setMoving] = useState(false)

  const loadDocuments = (searchTerm?: string) => {
    setLoading(true)
    documentsApi.getAll({ searchTerm: searchTerm ?? search, latestOnly: true })
      .then(setDocuments)
      .catch(() => setError('Failed to load documents'))
      .finally(() => setLoading(false))
  }

  useEffect(() => {
    loadDocuments()
  }, [])

  const handleUpload = async () => {
    if (!selectedFile) return
    setUploading(true)
    try {
      await documentsApi.upload(selectedFile, description, undefined, undefined, undefined, subFolder || undefined)
      setUploadOpen(false)
      setSelectedFile(null)
      setDescription('')
      setSubFolder('')
      loadDocuments()
    } catch {
      setError('Upload failed. Please check file and server status.')
    } finally {
      setUploading(false)
    }
  }

  const handleUploadVersion = async () => {
    if (!versionDoc || !versionFile) return
    setUploading(true)
    try {
      await documentsApi.uploadVersion(versionDoc.id, versionFile, versionDesc)
      setVersionDoc(null)
      setVersionFile(null)
      setVersionDesc('')
      loadDocuments()
    } catch {
      setError('Version upload failed.')
    } finally {
      setUploading(false)
    }
  }

  const handleMove = async () => {
    if (!moveDoc || !targetFolder) return
    setMoving(true)
    try {
      await documentsApi.move(moveDoc.id, targetFolder)
      setMoveDoc(null)
      setTargetFolder('')
      loadDocuments()
    } catch {
      setError('Failed to move file.')
    } finally {
      setMoving(false)
    }
  }

  const handleDelete = async (id: string) => {
    if (!confirm('Are you sure you want to delete this document and its stored file?')) return
    try {
      await documentsApi.delete(id, true)
      loadDocuments()
    } catch {
      setError('Failed to delete document.')
    }
  }

  const openHistory = async (doc: Document) => {
    try {
      const versions = await documentsApi.getVersions(doc.id)
      setVersionHistory(versions)
      setHistoryOpen(true)
    } catch {
      setError('Failed to load version history.')
    }
  }

  return (
    <Box>
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
        <div>
          <Typography variant="h5" fontWeight={700}>Document Storage Engine</Typography>
          <Typography variant="body2" color="text.secondary">
            Local-first file management, SHA256 integrity verification, and revision history.
          </Typography>
        </div>
        <Button
          variant="contained"
          startIcon={<UploadFileIcon />}
          onClick={() => setUploadOpen(true)}
        >
          Upload Document
        </Button>
      </Box>

      {error && <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError(null)}>{error}</Alert>}

      <Paper sx={{ p: 2, mb: 3 }}>
        <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2} alignItems="center">
          <TextField
            size="small"
            placeholder="Search by file name, description, or SHA256 hash..."
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            onKeyDown={(e) => e.key === 'Enter' && loadDocuments()}
            InputProps={{ startAdornment: <SearchIcon sx={{ mr: 1, color: 'text.secondary' }} /> }}
            sx={{ flexGrow: 1 }}
          />
          <Button variant="outlined" onClick={() => loadDocuments()}>Search</Button>
        </Stack>
      </Paper>

      {loading ? (
        <Box display="flex" justifyContent="center" mt={8}><CircularProgress /></Box>
      ) : (
        <TableContainer component={Paper}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell><strong>File Name</strong></TableCell>
                <TableCell><strong>Version</strong></TableCell>
                <TableCell><strong>Size</strong></TableCell>
                <TableCell><strong>SHA256 Hash</strong></TableCell>
                <TableCell><strong>Storage Path</strong></TableCell>
                <TableCell><strong>Uploaded</strong></TableCell>
                <TableCell align="right"><strong>Actions</strong></TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {documents.length === 0 ? (
                <TableRow>
                  <TableCell colSpan={7} align="center" sx={{ py: 6, color: 'text.secondary' }}>
                    <DescriptionIcon sx={{ fontSize: 48, opacity: 0.3, mb: 1, display: 'block', mx: 'auto' }} />
                    No documents found. Upload your first document above.
                  </TableCell>
                </TableRow>
              ) : documents.map((doc) => (
                <TableRow key={doc.id} hover>
                  <TableCell>
                    <Box display="flex" alignItems="center" gap={1}>
                      <DescriptionIcon color="primary" fontSize="small" />
                      <div>
                        <Typography variant="body2" fontWeight={600}>{doc.originalFileName}</Typography>
                        {doc.description && (
                          <Typography variant="caption" color="text.secondary">{doc.description}</Typography>
                        )}
                      </div>
                    </Box>
                  </TableCell>
                  <TableCell>
                    <Chip label={`v${doc.version}`} size="small" color={doc.version > 1 ? 'secondary' : 'default'} />
                  </TableCell>
                  <TableCell>{formatBytes(doc.fileSizeBytes)}</TableCell>
                  <TableCell>
                    <Tooltip title={doc.sha256Hash} placement="top">
                      <Chip
                        label={doc.sha256Hash.substring(0, 10) + '...'}
                        size="small"
                        variant="outlined"
                        sx={{ fontFamily: 'monospace', fontSize: 11 }}
                      />
                    </Tooltip>
                  </TableCell>
                  <TableCell>
                    <Typography variant="caption" sx={{ fontFamily: 'monospace' }}>
                      {doc.storagePath}
                    </Typography>
                  </TableCell>
                  <TableCell>
                    <Typography variant="caption" color="text.secondary">
                      {new Date(doc.createdAt).toLocaleDateString()}
                    </Typography>
                  </TableCell>
                  <TableCell align="right">
                    <Tooltip title="Download File">
                      <IconButton
                        size="small"
                        color="primary"
                        component="a"
                        href={documentsApi.getDownloadUrl(doc.id)}
                        download
                      >
                        <DownloadIcon fontSize="small" />
                      </IconButton>
                    </Tooltip>
                    <Tooltip title="Upload New Version">
                      <IconButton size="small" color="secondary" onClick={() => setVersionDoc(doc)}>
                        <UploadFileIcon fontSize="small" />
                      </IconButton>
                    </Tooltip>
                    <Tooltip title="Version History">
                      <IconButton size="small" onClick={() => openHistory(doc)}>
                        <HistoryIcon fontSize="small" />
                      </IconButton>
                    </Tooltip>
                    <Tooltip title="Move File Folder">
                      <IconButton size="small" onClick={() => { setMoveDoc(doc); setTargetFolder(''); }}>
                        <DriveFileMoveIcon fontSize="small" />
                      </IconButton>
                    </Tooltip>
                    <Tooltip title="Delete File">
                      <IconButton size="small" color="error" onClick={() => handleDelete(doc.id)}>
                        <DeleteIcon fontSize="small" />
                      </IconButton>
                    </Tooltip>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      )}

      {/* Upload Dialog */}
      <Dialog open={uploadOpen} onClose={() => setUploadOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>Upload Document</DialogTitle>
        <DialogContent sx={{ display: 'flex', flexDirection: 'column', gap: 2, pt: 2 }}>
          <Button variant="outlined" component="label" sx={{ py: 3, borderStyle: 'dashed' }}>
            {selectedFile ? selectedFile.name : 'Select or Drop File to Upload'}
            <input type="file" hidden onChange={(e) => setSelectedFile(e.target.files?.[0] ?? null)} />
          </Button>
          {selectedFile && (
            <Typography variant="caption" color="text.secondary">
              Size: {formatBytes(selectedFile.size)} · Type: {selectedFile.type || 'Unknown'}
            </Typography>
          )}
          <TextField
            label="Description (optional)"
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            fullWidth
          />
          <TextField
            label="Subfolder Category (e.g. resumes, certificates, portfolio)"
            value={subFolder}
            onChange={(e) => setSubFolder(e.target.value)}
            placeholder="documents/2026/04"
            fullWidth
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setUploadOpen(false)}>Cancel</Button>
          <Button variant="contained" disabled={!selectedFile || uploading} onClick={handleUpload}>
            {uploading ? 'Calculating SHA256 & Uploading...' : 'Upload'}
          </Button>
        </DialogActions>
      </Dialog>

      {/* Upload New Version Dialog */}
      <Dialog open={!!versionDoc} onClose={() => setVersionDoc(null)} maxWidth="sm" fullWidth>
        <DialogTitle>Upload New Version for: {versionDoc?.originalFileName}</DialogTitle>
        <DialogContent sx={{ display: 'flex', flexDirection: 'column', gap: 2, pt: 2 }}>
          <Typography variant="body2" color="text.secondary">
            Current version is v{versionDoc?.version}. Uploading a new file will increment the version number to v{(versionDoc?.version ?? 1) + 1}.
          </Typography>
          <Button variant="outlined" component="label" sx={{ py: 3, borderStyle: 'dashed' }}>
            {versionFile ? versionFile.name : 'Select Updated Version File'}
            <input type="file" hidden onChange={(e) => setVersionFile(e.target.files?.[0] ?? null)} />
          </Button>
          <TextField
            label="Version Change Notes / Description"
            value={versionDesc}
            onChange={(e) => setVersionDesc(e.target.value)}
            fullWidth
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setVersionDoc(null)}>Cancel</Button>
          <Button variant="contained" disabled={!versionFile || uploading} onClick={handleUploadVersion}>
            {uploading ? 'Processing Version...' : 'Save New Version'}
          </Button>
        </DialogActions>
      </Dialog>

      {/* Move Dialog */}
      <Dialog open={!!moveDoc} onClose={() => setMoveDoc(null)} maxWidth="xs" fullWidth>
        <DialogTitle>Move Document</DialogTitle>
        <DialogContent sx={{ display: 'flex', flexDirection: 'column', gap: 2, pt: 2 }}>
          <Typography variant="body2">
            Moving file: <strong>{moveDoc?.originalFileName}</strong>
          </Typography>
          <Typography variant="caption" color="text.secondary">
            Current path: {moveDoc?.storagePath}
          </Typography>
          <TextField
            label="Target Subfolder"
            placeholder="archive/2026"
            value={targetFolder}
            onChange={(e) => setTargetFolder(e.target.value)}
            fullWidth
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setMoveDoc(null)}>Cancel</Button>
          <Button variant="contained" disabled={!targetFolder || moving} onClick={handleMove}>
            {moving ? 'Moving...' : 'Move'}
          </Button>
        </DialogActions>
      </Dialog>

      {/* Version History Dialog */}
      <Dialog open={historyOpen} onClose={() => setHistoryOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>Version History</DialogTitle>
        <DialogContent>
          <Table size="small">
            <TableHead>
              <TableRow>
                <TableCell><strong>Version</strong></TableCell>
                <TableCell><strong>File Name</strong></TableCell>
                <TableCell><strong>Size</strong></TableCell>
                <TableCell><strong>SHA256</strong></TableCell>
                <TableCell><strong>Date</strong></TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {versionHistory.map((v) => (
                <TableRow key={v.id}>
                  <TableCell>
                    <Chip label={`v${v.version}`} size="small" color={v.isLatestVersion ? 'primary' : 'default'} />
                  </TableCell>
                  <TableCell>{v.originalFileName}</TableCell>
                  <TableCell>{formatBytes(v.fileSizeBytes)}</TableCell>
                  <TableCell>
                    <Tooltip title={v.sha256Hash}>
                      <Typography variant="caption" sx={{ fontFamily: 'monospace' }}>
                        {v.sha256Hash.substring(0, 8)}...
                      </Typography>
                    </Tooltip>
                  </TableCell>
                  <TableCell>{new Date(v.createdAt).toLocaleDateString()}</TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setHistoryOpen(false)}>Close</Button>
        </DialogActions>
      </Dialog>
    </Box>
  )
}
