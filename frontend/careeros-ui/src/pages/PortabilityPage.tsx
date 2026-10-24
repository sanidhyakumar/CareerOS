import { useState, useRef } from 'react';
import {
  Box,
  Typography,
  Grid,
  Card,
  CardContent,
  Button,
  Alert,
  Paper,
  Chip,
  CircularProgress,
  Snackbar,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  List,
  ListItem,
  ListItemText,
  ListItemIcon,
} from '@mui/material';
import FolderZipIcon from '@mui/icons-material/FolderZip';
import CloudDownloadIcon from '@mui/icons-material/CloudDownload';
import CloudUploadIcon from '@mui/icons-material/CloudUpload';
import TableViewIcon from '@mui/icons-material/TableView';
import ArticleIcon from '@mui/icons-material/Article';
import StorageIcon from '@mui/icons-material/Storage';
import CheckCircleIcon from '@mui/icons-material/CheckCircle';
import SettingsBackupRestoreIcon from '@mui/icons-material/SettingsBackupRestore';

import { portabilityApi, type ImportSummary } from '../api/portability';

export const PortabilityPage: React.FC = () => {
  const [downloadingZip, setDownloadingZip] = useState(false);
  const [downloadingJson, setDownloadingJson] = useState(false);
  const [downloadingMarkdown, setDownloadingMarkdown] = useState(false);
  const [downloadingCsv, setDownloadingCsv] = useState<string | null>(null);

  const [importing, setImporting] = useState(false);
  const [importResult, setImportResult] = useState<ImportSummary | null>(null);
  const [resultDialogOpen, setResultDialogOpen] = useState(false);
  const [snackbarMessage, setSnackbarMessage] = useState<string | null>(null);

  const fileInputRef = useRef<HTMLInputElement>(null);

  const handleExportFullZip = async () => {
    setDownloadingZip(true);
    try {
      await portabilityApi.exportFullZip();
      setSnackbarMessage('Full system export archive downloaded.');
    } catch (err) {
      setSnackbarMessage('Failed to download system ZIP archive.');
    } finally {
      setDownloadingZip(false);
    }
  };

  const handleExportJson = async () => {
    setDownloadingJson(true);
    try {
      await portabilityApi.exportJson();
      setSnackbarMessage('Database JSON backup downloaded.');
    } catch (err) {
      setSnackbarMessage('Failed to download JSON backup.');
    } finally {
      setDownloadingJson(false);
    }
  };

  const handleExportMarkdown = async () => {
    setDownloadingMarkdown(true);
    try {
      await portabilityApi.exportMarkdown();
      setSnackbarMessage('Knowledge base Markdown archive downloaded.');
    } catch (err) {
      setSnackbarMessage('Failed to download Markdown archive.');
    } finally {
      setDownloadingMarkdown(false);
    }
  };

  const handleExportCsv = async (entity: 'jobs' | 'applications' | 'companies' | 'contacts') => {
    setDownloadingCsv(entity);
    try {
      await portabilityApi.exportCsv(entity);
      setSnackbarMessage(`${entity.toUpperCase()} CSV downloaded.`);
    } catch (err) {
      setSnackbarMessage(`Failed to download ${entity} CSV.`);
    } finally {
      setDownloadingCsv(null);
    }
  };

  const handleFileSelected = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;

    setImporting(true);
    try {
      const summary = await portabilityApi.importJson(file, false);
      setImportResult(summary);
      setResultDialogOpen(true);
      setSnackbarMessage(summary.message);
    } catch (err: any) {
      setSnackbarMessage(err.response?.data?.message || 'Failed to import backup package.');
    } finally {
      setImporting(false);
      if (fileInputRef.current) fileInputRef.current.value = '';
    }
  };

  return (
    <Box sx={{ maxWidth: 1200, mx: 'auto', p: 3 }}>
      {/* Header Banner */}
      <Box sx={{ mb: 4 }}>
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1.5, mb: 1 }}>
          <SettingsBackupRestoreIcon color="primary" sx={{ fontSize: 36 }} />
          <Typography variant="h4" fontWeight={700}>
            Data Portability & Backup Center
          </Typography>
        </Box>
        <Typography variant="body1" color="text.secondary">
          Zero vendor lock-in. Full ownership of your career data. Export your local records to JSON, CSV spreadsheets, Markdown archives, or a complete ZIP package.
        </Typography>
      </Box>

      {/* 4 Export Modality Cards */}
      <Grid container spacing={3} sx={{ mb: 4 }}>
        {/* Card 1: Complete System Backup (ZIP) */}
        <Grid item xs={12} md={6}>
          <Card
            variant="outlined"
            sx={{
              borderRadius: 3,
              height: '100%',
              display: 'flex',
              flexDirection: 'column',
              justifyContent: 'space-between',
              borderTop: '4px solid #2563eb',
            }}
          >
            <CardContent>
              <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', mb: 1.5 }}>
                <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                  <FolderZipIcon sx={{ color: '#2563eb', fontSize: 28 }} />
                  <Typography variant="h6" fontWeight={700}>
                    Complete System Archive (ZIP)
                  </Typography>
                </Box>
                <Chip label="All-in-One" size="small" color="primary" variant="outlined" />
              </Box>
              <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
                Packages your full database snapshot (`.json`), spreadsheets (`.csv`), wiki notes (`.md`), and files manifest in a single portable `.zip` file.
              </Typography>
              <Box sx={{ display: 'flex', gap: 0.8, flexWrap: 'wrap', mb: 2 }}>
                <Chip label="Database JSON" size="small" />
                <Chip label="Spreadsheets CSV" size="small" />
                <Chip label="Wiki Markdown" size="small" />
                <Chip label="Checksum Manifest" size="small" />
              </Box>
            </CardContent>
            <Box sx={{ p: 2, pt: 0 }}>
              <Button
                variant="contained"
                fullWidth
                startIcon={downloadingZip ? <CircularProgress size={18} color="inherit" /> : <CloudDownloadIcon />}
                onClick={handleExportFullZip}
                disabled={downloadingZip}
                sx={{ textTransform: 'none', borderRadius: 2 }}
              >
                {downloadingZip ? 'Packaging ZIP Archive...' : 'Download Complete Archive (ZIP)'}
              </Button>
            </Box>
          </Card>
        </Grid>

        {/* Card 2: Database Snapshot (JSON) */}
        <Grid item xs={12} md={6}>
          <Card
            variant="outlined"
            sx={{
              borderRadius: 3,
              height: '100%',
              display: 'flex',
              flexDirection: 'column',
              justifyContent: 'space-between',
              borderTop: '4px solid #10b981',
            }}
          >
            <CardContent>
              <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', mb: 1.5 }}>
                <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                  <StorageIcon sx={{ color: '#10b981', fontSize: 28 }} />
                  <Typography variant="h6" fontWeight={700}>
                    Database Snapshot (JSON)
                  </Typography>
                </Box>
                <Chip label="Restore Ready" size="small" color="success" variant="outlined" />
              </Box>
              <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
                Full structured database state backup containing jobs, applications, timeline milestones, CRM contacts, wiki stories, and sponsorship cases.
              </Typography>
              <Box sx={{ display: 'flex', gap: 0.8, flexWrap: 'wrap', mb: 2 }}>
                <Chip label="Relational Schema" size="small" />
                <Chip label="100% Machine-Readable" size="small" />
                <Chip label="Import Compatible" size="small" />
              </Box>
            </CardContent>
            <Box sx={{ p: 2, pt: 0 }}>
              <Button
                variant="outlined"
                color="success"
                fullWidth
                startIcon={downloadingJson ? <CircularProgress size={18} color="inherit" /> : <CloudDownloadIcon />}
                onClick={handleExportJson}
                disabled={downloadingJson}
                sx={{ textTransform: 'none', borderRadius: 2 }}
              >
                {downloadingJson ? 'Generating JSON Snapshot...' : 'Export Database (JSON)'}
              </Button>
            </Box>
          </Card>
        </Grid>

        {/* Card 3: Spreadsheets Export (CSV) */}
        <Grid item xs={12} md={6}>
          <Card
            variant="outlined"
            sx={{
              borderRadius: 3,
              height: '100%',
              display: 'flex',
              flexDirection: 'column',
              justifyContent: 'space-between',
              borderTop: '4px solid #f59e0b',
            }}
          >
            <CardContent>
              <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', mb: 1.5 }}>
                <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                  <TableViewIcon sx={{ color: '#f59e0b', fontSize: 28 }} />
                  <Typography variant="h6" fontWeight={700}>
                    Tabular Spreadsheets (CSV)
                  </Typography>
                </Box>
                <Chip label="Excel & Sheets" size="small" color="warning" variant="outlined" />
              </Box>
              <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
                Export clean CSV tables with column headers for spreadsheet analysis in Excel, Google Sheets, or custom scripts.
              </Typography>
              <Grid container spacing={1}>
                {(['jobs', 'applications', 'companies', 'contacts'] as const).map((ent) => (
                  <Grid item xs={6} key={ent}>
                    <Button
                      variant="outlined"
                      fullWidth
                      size="small"
                      startIcon={downloadingCsv === ent ? <CircularProgress size={14} /> : <CloudDownloadIcon />}
                      onClick={() => handleExportCsv(ent)}
                      disabled={Boolean(downloadingCsv)}
                      sx={{ textTransform: 'capitalize', borderRadius: 1.5 }}
                    >
                      {ent} CSV
                    </Button>
                  </Grid>
                ))}
              </Grid>
            </CardContent>
          </Card>
        </Grid>

        {/* Card 4: Knowledge Base (Markdown ZIP) */}
        <Grid item xs={12} md={6}>
          <Card
            variant="outlined"
            sx={{
              borderRadius: 3,
              height: '100%',
              display: 'flex',
              flexDirection: 'column',
              justifyContent: 'space-between',
              borderTop: '4px solid #8b5cf6',
            }}
          >
            <CardContent>
              <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', mb: 1.5 }}>
                <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                  <ArticleIcon sx={{ color: '#8b5cf6', fontSize: 28 }} />
                  <Typography variant="h6" fontWeight={700}>
                    Personal Wiki & Stories (Markdown)
                  </Typography>
                </Box>
                <Chip label="Obsidian / Notion" size="small" color="secondary" variant="outlined" />
              </Box>
              <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
                Download all your STAR behavioral stories, system design notes, and company interview prep as clean `.md` files organized by folder category.
              </Typography>
              <Box sx={{ display: 'flex', gap: 0.8, flexWrap: 'wrap', mb: 2 }}>
                <Chip label="YAML Frontmatter" size="small" />
                <Chip label="Folder Segregation" size="small" />
                <Chip label="Local Notes App Ready" size="small" />
              </Box>
            </CardContent>
            <Box sx={{ p: 2, pt: 0 }}>
              <Button
                variant="outlined"
                color="secondary"
                fullWidth
                startIcon={downloadingMarkdown ? <CircularProgress size={18} color="inherit" /> : <CloudDownloadIcon />}
                onClick={handleExportMarkdown}
                disabled={downloadingMarkdown}
                sx={{ textTransform: 'none', borderRadius: 2 }}
              >
                {downloadingMarkdown ? 'Bundling Markdown...' : 'Export Markdown Archive (.zip)'}
              </Button>
            </Box>
          </Card>
        </Grid>
      </Grid>

      {/* Restore & Import Section */}
      <Paper
        variant="outlined"
        sx={{
          p: 4,
          borderRadius: 3,
          backgroundColor: 'background.paper',
          border: '2px dashed',
          borderColor: 'primary.light',
          textAlign: 'center',
        }}
      >
        <CloudUploadIcon sx={{ fontSize: 56, color: 'primary.main', mb: 1.5 }} />
        <Typography variant="h5" fontWeight={700} gutterBottom>
          Restore from JSON Backup
        </Typography>
        <Typography variant="body1" color="text.secondary" sx={{ maxWidth: 580, mx: 'auto', mb: 3 }}>
          Upload a previously exported `careeros-backup.json` to restore your companies, positions, tracked applications, recruiters, and wiki stories.
        </Typography>

        <input
          type="file"
          accept=".json"
          ref={fileInputRef}
          style={{ display: 'none' }}
          onChange={handleFileSelected}
        />

        <Button
          variant="contained"
          size="large"
          startIcon={importing ? <CircularProgress size={20} color="inherit" /> : <CloudUploadIcon />}
          onClick={() => fileInputRef.current?.click()}
          disabled={importing}
          sx={{ textTransform: 'none', px: 4, borderRadius: 2 }}
        >
          {importing ? 'Restoring Database Package...' : 'Select JSON Backup File'}
        </Button>
      </Paper>

      {/* Import Result Dialog */}
      <Dialog open={resultDialogOpen} onClose={() => setResultDialogOpen(false)} maxWidth="xs" fullWidth>
        <DialogTitle sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
          <CheckCircleIcon color="success" />
          Backup Restored Successfully
        </DialogTitle>
        <DialogContent>
          <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
            {importResult?.message}
          </Typography>
          {importResult?.recordsImported && (
            <List dense sx={{ bgcolor: 'grey.50', borderRadius: 2, p: 1 }}>
              {Object.entries(importResult.recordsImported).map(([entity, count]) => (
                <ListItem key={entity}>
                  <ListItemIcon sx={{ minWidth: 32 }}>
                    <CheckCircleIcon color="success" fontSize="small" />
                  </ListItemIcon>
                  <ListItemText primary={`${entity}: ${count} records`} />
                </ListItem>
              ))}
            </List>
          )}
        </DialogContent>
        <DialogActions sx={{ p: 2 }}>
          <Button onClick={() => setResultDialogOpen(false)} variant="contained">
            Done
          </Button>
        </DialogActions>
      </Dialog>

      {/* Snackbar alerts */}
      <Snackbar
        open={Boolean(snackbarMessage)}
        autoHideDuration={4000}
        onClose={() => setSnackbarMessage(null)}
        anchorOrigin={{ vertical: 'bottom', horizontal: 'center' }}
      >
        <Alert severity="success" onClose={() => setSnackbarMessage(null)}>
          {snackbarMessage}
        </Alert>
      </Snackbar>
    </Box>
  );
};

