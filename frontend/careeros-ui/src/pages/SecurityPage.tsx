import { useState, useEffect } from 'react';
import {
  Box,
  Typography,
  Card,
  CardContent,
  Grid,
  Button,
  TextField,
  InputAdornment,
  IconButton,
  Chip,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Alert,
  CircularProgress,
  Snackbar,
  Paper,
  Tooltip,
} from '@mui/material';
import LockIcon from '@mui/icons-material/Lock';
import LockOpenIcon from '@mui/icons-material/LockOpen';
import VpnKeyIcon from '@mui/icons-material/VpnKey';
import VisibilityIcon from '@mui/icons-material/Visibility';
import VisibilityOffIcon from '@mui/icons-material/VisibilityOff';
import ContentCopyIcon from '@mui/icons-material/ContentCopy';
import OpenInNewIcon from '@mui/icons-material/OpenInNew';
import AddIcon from '@mui/icons-material/Add';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
import ShieldIcon from '@mui/icons-material/Shield';
import AutoFixHighIcon from '@mui/icons-material/AutoFixHigh';
import SearchIcon from '@mui/icons-material/Search';

import {
  securityApi,
  type SecurityStatus,
  type CredentialSummary,
  type DecryptedCredential,
} from '../api/security';

const PRESET_SERVICES = [
  'Workday',
  'Greenhouse',
  'Lever',
  'Ashby',
  'Taleo',
  'SmartRecruiters',
  'LinkedIn',
  'Indeed',
];

export const SecurityPage: React.FC = () => {
  const [status, setStatus] = useState<SecurityStatus | null>(null);
  const [credentials, setCredentials] = useState<CredentialSummary[]>([]);
  const [loading, setLoading] = useState(true);
  const [searchQuery, setSearchQuery] = useState('');
  const [selectedService, setSelectedService] = useState('All');

  // Master Passphrase Dialogs
  const [unlockDialogOpen, setUnlockDialogOpen] = useState(false);
  const [setupDialogOpen, setSetupDialogOpen] = useState(false);
  const [masterPasswordInput, setMasterPasswordInput] = useState('');
  const [confirmMasterPasswordInput, setConfirmMasterPasswordInput] = useState('');
  const [unlockError, setUnlockError] = useState<string | null>(null);

  // Credential Modal
  const [credentialModalOpen, setCredentialModalOpen] = useState(false);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [formServiceName, setFormServiceName] = useState('');
  const [formPortalUrl, setFormPortalUrl] = useState('');
  const [formUsername, setFormUsername] = useState('');
  const [formPassword, setFormPassword] = useState('');
  const [formNotes, setFormNotes] = useState('');

  // Revealed Passwords Cache (by credentialId)
  const [revealedPasswords, setRevealedPasswords] = useState<Record<string, DecryptedCredential>>({});
  const [revealingId, setRevealingId] = useState<string | null>(null);

  const [snackbarMessage, setSnackbarMessage] = useState<string | null>(null);

  useEffect(() => {
    loadSecurityData();
  }, []);

  if (loading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: 400 }}>
        <CircularProgress />
      </Box>
    );
  }

  const loadSecurityData = async () => {
    setLoading(true);
    try {
      const s = await securityApi.getStatus();
      setStatus(s);

      if (s.isVaultUnlocked) {
        const creds = await securityApi.getCredentials();
        setCredentials(creds);
      } else if (!s.isMasterPasswordConfigured) {
        setSetupDialogOpen(true);
      } else {
        setUnlockDialogOpen(true);
      }
    } catch (err) {
      console.error('Failed to load security status:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleSetupMasterPassword = async () => {
    if (masterPasswordInput.length < 6) {
      setUnlockError('Master password must be at least 6 characters long.');
      return;
    }
    if (masterPasswordInput !== confirmMasterPasswordInput) {
      setUnlockError('Passwords do not match. Please verify.');
      return;
    }

    try {
      await securityApi.setupMasterPassword(masterPasswordInput);
      setSetupDialogOpen(false);
      setMasterPasswordInput('');
      setConfirmMasterPasswordInput('');
      setUnlockError(null);
      setSnackbarMessage('Master security key created with AES-256 + PBKDF2!');
      await loadSecurityData();
    } catch (err: any) {
      setUnlockError(err.response?.data?.message || 'Failed to setup master key.');
    }
  };

  const handleUnlockVault = async () => {
    try {
      setUnlockError(null);
      await securityApi.unlockVault(masterPasswordInput);
      setUnlockDialogOpen(false);
      setMasterPasswordInput('');
      setSnackbarMessage('Security vault unlocked successfully.');
      await loadSecurityData();
    } catch (err: any) {
      setUnlockError(err.response?.data?.message || 'Invalid master password. Please try again.');
    }
  };

  const handleLockVault = async () => {
    try {
      await securityApi.lockVault();
      setRevealedPasswords({});
      setSnackbarMessage('Vault locked. Active encryption keys flushed from memory.');
      await loadSecurityData();
    } catch (err) {
      console.error('Failed to lock vault:', err);
    }
  };

  const handleRevealPassword = async (id: string) => {
    if (revealedPasswords[id]) {
      // Toggle hide
      const copy = { ...revealedPasswords };
      delete copy[id];
      setRevealedPasswords(copy);
      return;
    }

    setRevealingId(id);
    try {
      const decrypted = await securityApi.revealCredential(id);
      setRevealedPasswords((prev) => ({ ...prev, [id]: decrypted }));
    } catch (err: any) {
      setSnackbarMessage(err.response?.data?.message || 'Failed to decrypt credential.');
    } finally {
      setRevealingId(null);
    }
  };

  const handleCopy = (text: string, label: string) => {
    navigator.clipboard.writeText(text);
    setSnackbarMessage(`${label} copied to clipboard!`);
  };

  const handleGeneratePassword = () => {
    const chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()_+~|}{[]:;?><,./-=';
    let result = '';
    const array = new Uint32Array(16);
    window.crypto.getRandomValues(array);
    for (let i = 0; i < 16; i++) {
      result += chars[array[i] % chars.length];
    }
    setFormPassword(result);
    setSnackbarMessage('Generated strong 16-character password!');
  };

  const handleOpenCreateModal = () => {
    setEditingId(null);
    setFormServiceName('');
    setFormPortalUrl('');
    setFormUsername('');
    setFormPassword('');
    setFormNotes('');
    setCredentialModalOpen(true);
  };

  const handleOpenEditModal = (c: CredentialSummary) => {
    setEditingId(c.id);
    setFormServiceName(c.serviceName);
    setFormPortalUrl(c.portalUrl || '');
    setFormUsername(c.username);
    setFormPassword('');
    setFormNotes('');
    setCredentialModalOpen(true);
  };

  const handleSaveCredential = async () => {
    if (!formServiceName.trim() || !formUsername.trim()) {
      setSnackbarMessage('Please enter service name and username.');
      return;
    }

    try {
      if (editingId) {
        await securityApi.updateCredential(editingId, {
          id: editingId,
          serviceName: formServiceName,
          portalUrl: formPortalUrl,
          username: formUsername,
          password: formPassword.trim() ? formPassword : undefined,
          notes: formNotes,
        });
        setSnackbarMessage('Credential updated successfully.');
      } else {
        if (!formPassword.trim()) {
          setSnackbarMessage('Password is required for new credential.');
          return;
        }
        await securityApi.createCredential({
          serviceName: formServiceName,
          portalUrl: formPortalUrl,
          username: formUsername,
          password: formPassword,
          notes: formNotes,
        });
        setSnackbarMessage('Credential encrypted and stored securely.');
      }
      setCredentialModalOpen(false);
      const updated = await securityApi.getCredentials();
      setCredentials(updated);
    } catch (err: any) {
      setSnackbarMessage(err.response?.data?.message || 'Failed to save credential.');
    }
  };

  const handleDeleteCredential = async (id: string) => {
    if (window.confirm('Are you sure you want to delete this stored portal credential?')) {
      try {
        await securityApi.deleteCredential(id);
        setCredentials((prev) => prev.filter((c) => c.id !== id));
        setSnackbarMessage('Credential deleted.');
      } catch (err) {
        setSnackbarMessage('Failed to delete credential.');
      }
    }
  };

  const filteredCredentials = credentials.filter((c) => {
    const matchesSearch =
      c.serviceName.toLowerCase().includes(searchQuery.toLowerCase()) ||
      c.username.toLowerCase().includes(searchQuery.toLowerCase()) ||
      (c.portalUrl && c.portalUrl.toLowerCase().includes(searchQuery.toLowerCase()));

    const matchesService =
      selectedService === 'All' || c.serviceName.toLowerCase().includes(selectedService.toLowerCase());

    return matchesSearch && matchesService;
  });

  return (
    <Box sx={{ maxWidth: 1200, mx: 'auto', p: 3 }}>
      {/* Header Banner */}
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', mb: 3, flexWrap: 'wrap', gap: 2 }}>
        <Box>
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 1.5, mb: 0.5 }}>
            <ShieldIcon color="primary" sx={{ fontSize: 32 }} />
            <Typography variant="h4" fontWeight={700}>
              Portal Passwords & Security Vault
            </Typography>
          </Box>
          <Typography variant="body1" color="text.secondary">
            100% Local-First AES-256 GCM encrypted credential vault with PBKDF2 SHA-256 key derivation.
          </Typography>
        </Box>

        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1.5 }}>
          {status?.isVaultUnlocked ? (
            <>
              <Chip
                icon={<LockOpenIcon />}
                label="Vault Unlocked"
                color="success"
                variant="outlined"
                sx={{ fontWeight: 600 }}
              />
              <Button
                variant="outlined"
                color="warning"
                startIcon={<LockIcon />}
                onClick={handleLockVault}
                sx={{ textTransform: 'none', borderRadius: 2 }}
              >
                Lock Vault
              </Button>
              <Button
                variant="contained"
                startIcon={<AddIcon />}
                onClick={handleOpenCreateModal}
                sx={{ textTransform: 'none', borderRadius: 2 }}
              >
                Add Portal Login
              </Button>
            </>
          ) : (
            <Chip
              icon={<LockIcon />}
              label="Vault Locked"
              color="error"
              variant="outlined"
              onClick={() => setUnlockDialogOpen(true)}
              sx={{ fontWeight: 600, cursor: 'pointer' }}
            />
          )}
        </Box>
      </Box>

      {/* Vault Locked Banner / Prompt */}
      {!status?.isVaultUnlocked && (
        <Paper
          variant="outlined"
          sx={{
            p: 6,
            textAlign: 'center',
            borderRadius: 3,
            backgroundColor: 'background.paper',
            borderColor: 'primary.light',
            boxShadow: '0 8px 24px rgba(0,0,0,0.04)',
          }}
        >
          <LockIcon sx={{ fontSize: 64, color: 'primary.main', mb: 2 }} />
          <Typography variant="h5" fontWeight={700} gutterBottom>
            {status?.isMasterPasswordConfigured
              ? 'Security Vault is Locked'
              : 'Initialize Your Master Password'}
          </Typography>
          <Typography variant="body1" color="text.secondary" sx={{ maxWidth: 550, mx: 'auto', mb: 3 }}>
            {status?.isMasterPasswordConfigured
              ? 'Unlock your credential vault with your master passphrase to access and decrypt your application portal accounts.'
              : 'Create a master passphrase. CareerOS will use PBKDF2 with 100,000 iterations and SHA-256 to derive a 256-bit AES key for zero-knowledge local encryption.'}
          </Typography>
          <Button
            variant="contained"
            size="large"
            startIcon={status?.isMasterPasswordConfigured ? <LockOpenIcon /> : <VpnKeyIcon />}
            onClick={() =>
              status?.isMasterPasswordConfigured
                ? setUnlockDialogOpen(true)
                : setSetupDialogOpen(true)
            }
            sx={{ textTransform: 'none', px: 4, borderRadius: 2 }}
          >
            {status?.isMasterPasswordConfigured ? 'Unlock Vault' : 'Setup Master Passphrase'}
          </Button>
        </Paper>
      )}

      {/* Main Unlocked Dashboard */}
      {status?.isVaultUnlocked && (
        <>
          {/* Search & Service Filter Toolbar */}
          <Box sx={{ display: 'flex', gap: 2, mb: 3, flexWrap: 'wrap', alignItems: 'center' }}>
            <TextField
              size="small"
              placeholder="Search portal logins, usernames, or URLs..."
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              InputProps={{
                startAdornment: (
                  <InputAdornment position="start">
                    <SearchIcon fontSize="small" color="action" />
                  </InputAdornment>
                ),
              }}
              sx={{ minWidth: 320 }}
            />

            <Box sx={{ display: 'flex', gap: 0.8, overflowX: 'auto', py: 0.5 }}>
              {['All', ...PRESET_SERVICES].map((svc) => (
                <Chip
                  key={svc}
                  label={svc}
                  size="small"
                  clickable
                  color={selectedService === svc ? 'primary' : 'default'}
                  variant={selectedService === svc ? 'filled' : 'outlined'}
                  onClick={() => setSelectedService(svc)}
                />
              ))}
            </Box>
          </Box>

          {/* Credentials Grid */}
          {filteredCredentials.length > 0 ? (
            <Grid container spacing={2}>
              {filteredCredentials.map((cred) => {
                const decrypted = revealedPasswords[cred.id];
                const isRevealing = revealingId === cred.id;

                return (
                  <Grid item xs={12} md={6} key={cred.id}>
                    <Card
                      variant="outlined"
                      sx={{
                        borderRadius: 2.5,
                        transition: 'all 0.2s',
                        '&:hover': {
                          boxShadow: '0 6px 18px rgba(0,0,0,0.06)',
                          borderColor: 'primary.light',
                        },
                      }}
                    >
                      <CardContent>
                        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', mb: 1.5 }}>
                          <Box sx={{ display: 'flex', alignItems: 'center', gap: 1.2 }}>
                            <Box
                              sx={{
                                width: 38,
                                height: 38,
                                borderRadius: 2,
                                bgcolor: 'primary.light',
                                color: 'primary.dark',
                                display: 'flex',
                                alignItems: 'center',
                                justifyContent: 'center',
                                fontWeight: 700,
                              }}
                            >
                              {cred.serviceName.slice(0, 2).toUpperCase()}
                            </Box>
                            <Box>
                              <Typography variant="h6" fontWeight={700}>
                                {cred.serviceName}
                              </Typography>
                              {cred.portalUrl && (
                                <Box
                                  component="a"
                                  href={cred.portalUrl}
                                  target="_blank"
                                  rel="noopener noreferrer"
                                  sx={{
                                    display: 'flex',
                                    alignItems: 'center',
                                    gap: 0.4,
                                    color: 'primary.main',
                                    textDecoration: 'none',
                                    fontSize: '0.75rem',
                                    '&:hover': { textDecoration: 'underline' },
                                  }}
                                >
                                  {cred.portalUrl.replace(/^https?:\/\//, '').slice(0, 32)}...
                                  <OpenInNewIcon sx={{ fontSize: 12 }} />
                                </Box>
                              )}
                            </Box>
                          </Box>

                          <Box sx={{ display: 'flex', gap: 0.5 }}>
                            <IconButton size="small" onClick={() => handleOpenEditModal(cred)}>
                              <EditIcon fontSize="small" />
                            </IconButton>
                            <IconButton size="small" color="error" onClick={() => handleDeleteCredential(cred.id)}>
                              <DeleteIcon fontSize="small" />
                            </IconButton>
                          </Box>
                        </Box>

                        {/* Username Row */}
                        <Box
                          sx={{
                            p: 1.2,
                            borderRadius: 1.5,
                            bgcolor: 'grey.50',
                            border: '1px solid',
                            borderColor: 'grey.200',
                            display: 'flex',
                            alignItems: 'center',
                            justifyContent: 'space-between',
                            mb: 1,
                          }}
                        >
                          <Box>
                            <Typography variant="caption" color="text.secondary" sx={{ display: 'block' }}>
                              Username / Email
                            </Typography>
                            <Typography variant="body2" fontWeight={600}>
                              {cred.username}
                            </Typography>
                          </Box>
                          <Tooltip title="Copy username">
                            <IconButton size="small" onClick={() => handleCopy(cred.username, 'Username')}>
                              <ContentCopyIcon fontSize="small" />
                            </IconButton>
                          </Tooltip>
                        </Box>

                        {/* Password Row */}
                        <Box
                          sx={{
                            p: 1.2,
                            borderRadius: 1.5,
                            bgcolor: 'grey.50',
                            border: '1px solid',
                            borderColor: 'grey.200',
                            display: 'flex',
                            alignItems: 'center',
                            justifyContent: 'space-between',
                          }}
                        >
                          <Box sx={{ overflow: 'hidden' }}>
                            <Typography variant="caption" color="text.secondary" sx={{ display: 'block' }}>
                              Password (AES-256)
                            </Typography>
                            <Typography
                              variant="body2"
                              sx={{
                                fontFamily: decrypted ? 'monospace' : 'inherit',
                                fontWeight: 600,
                                letterSpacing: decrypted ? '0.05em' : '0.15em',
                              }}
                            >
                              {decrypted ? decrypted.password : cred.maskedPassword}
                            </Typography>
                          </Box>
                          <Box sx={{ display: 'flex', gap: 0.5 }}>
                            <Tooltip title={decrypted ? 'Hide password' : 'Show password'}>
                              <IconButton
                                size="small"
                                onClick={() => handleRevealPassword(cred.id)}
                                disabled={isRevealing}
                              >
                                {isRevealing ? (
                                  <CircularProgress size={16} />
                                ) : decrypted ? (
                                  <VisibilityOffIcon fontSize="small" />
                                ) : (
                                  <VisibilityIcon fontSize="small" />
                                )}
                              </IconButton>
                            </Tooltip>
                            {decrypted && (
                              <Tooltip title="Copy password">
                                <IconButton
                                  size="small"
                                  onClick={() => handleCopy(decrypted.password, 'Password')}
                                >
                                  <ContentCopyIcon fontSize="small" />
                                </IconButton>
                              </Tooltip>
                            )}
                          </Box>
                        </Box>

                        {/* Decrypted Notes */}
                        {decrypted?.notes && (
                          <Box
                            sx={{
                              mt: 1,
                              p: 1,
                              borderRadius: 1,
                              bgcolor: 'info.50',
                              border: '1px dashed',
                              borderColor: 'info.light',
                            }}
                          >
                            <Typography variant="caption" fontWeight={600} color="info.dark" sx={{ display: 'block' }}>
                              Secret Notes:
                            </Typography>
                            <Typography variant="caption" color="text.secondary">
                              {decrypted.notes}
                            </Typography>
                          </Box>
                        )}
                      </CardContent>
                    </Card>
                  </Grid>
                );
              })}
            </Grid>
          ) : (
            <Paper variant="outlined" sx={{ p: 6, textAlign: 'center', borderRadius: 2 }}>
              <ShieldIcon sx={{ fontSize: 48, color: 'text.disabled', mb: 1 }} />
              <Typography variant="h6" color="text.secondary" gutterBottom>
                No portal credentials stored yet.
              </Typography>
              <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
                Safely store your Workday, Greenhouse, or candidate portal logins encrypted with your master key.
              </Typography>
              <Button variant="contained" startIcon={<AddIcon />} onClick={handleOpenCreateModal}>
                Add Portal Login
              </Button>
            </Paper>
          )}
        </>
      )}

      {/* Setup Master Password Dialog */}
      <Dialog open={setupDialogOpen} onClose={() => {}} maxWidth="sm" fullWidth>
        <DialogTitle sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
          <ShieldIcon color="primary" />
          Setup Master Security Passphrase
        </DialogTitle>
        <DialogContent>
          <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
            Your master passphrase is used to derive a 256-bit AES encryption key using PBKDF2 with SHA-256 and 100,000 iterations.
            It is stored 100% locally on your machine and never leaves your computer.
          </Typography>

          {unlockError && <Alert severity="error" sx={{ mb: 2 }}>{unlockError}</Alert>}

          <TextField
            autoFocus
            fullWidth
            type="password"
            label="Master Passphrase (min 6 characters)"
            value={masterPasswordInput}
            onChange={(e) => setMasterPasswordInput(e.target.value)}
            sx={{ mb: 2, mt: 1 }}
          />

          <TextField
            fullWidth
            type="password"
            label="Confirm Master Passphrase"
            value={confirmMasterPasswordInput}
            onChange={(e) => setConfirmMasterPasswordInput(e.target.value)}
          />
        </DialogContent>
        <DialogActions sx={{ p: 2 }}>
          <Button onClick={handleSetupMasterPassword} variant="contained">
            Create Master Key & Unlock
          </Button>
        </DialogActions>
      </Dialog>

      {/* Unlock Vault Dialog */}
      <Dialog open={unlockDialogOpen} onClose={() => setUnlockDialogOpen(false)} maxWidth="xs" fullWidth>
        <DialogTitle sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
          <LockIcon color="primary" />
          Unlock Security Vault
        </DialogTitle>
        <DialogContent>
          <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
            Enter your master passphrase to decrypt your portal credentials.
          </Typography>

          {unlockError && <Alert severity="error" sx={{ mb: 2 }}>{unlockError}</Alert>}

          <TextField
            autoFocus
            fullWidth
            type="password"
            label="Master Passphrase"
            value={masterPasswordInput}
            onChange={(e) => setMasterPasswordInput(e.target.value)}
            onKeyDown={(e) => {
              if (e.key === 'Enter') handleUnlockVault();
            }}
            sx={{ mt: 1 }}
          />
        </DialogContent>
        <DialogActions sx={{ p: 2 }}>
          <Button onClick={() => setUnlockDialogOpen(false)}>Cancel</Button>
          <Button onClick={handleUnlockVault} variant="contained">
            Unlock Vault
          </Button>
        </DialogActions>
      </Dialog>

      {/* Add / Edit Credential Dialog */}
      <Dialog open={credentialModalOpen} onClose={() => setCredentialModalOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>
          {editingId ? 'Edit Portal Credential' : 'Add New Portal Credential'}
        </DialogTitle>
        <DialogContent>
          <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2, mt: 1 }}>
            <Box sx={{ display: 'flex', gap: 1, flexWrap: 'wrap' }}>
              {PRESET_SERVICES.map((s) => (
                <Chip
                  key={s}
                  label={s}
                  size="small"
                  clickable
                  variant={formServiceName === s ? 'filled' : 'outlined'}
                  color={formServiceName === s ? 'primary' : 'default'}
                  onClick={() => setFormServiceName(s)}
                />
              ))}
            </Box>

            <TextField
              fullWidth
              label="Service / Portal Name *"
              placeholder="e.g. Workday, Greenhouse, Ashby, LinkedIn"
              value={formServiceName}
              onChange={(e) => setFormServiceName(e.target.value)}
            />

            <TextField
              fullWidth
              label="Portal URL"
              placeholder="https://company.wd1.myworkdayjobs.com/..."
              value={formPortalUrl}
              onChange={(e) => setFormPortalUrl(e.target.value)}
            />

            <TextField
              fullWidth
              label="Username / Login Email *"
              placeholder="your.email@example.com"
              value={formUsername}
              onChange={(e) => setFormUsername(e.target.value)}
            />

            <TextField
              fullWidth
              type="text"
              label={editingId ? 'New Password (leave empty to keep current)' : 'Password *'}
              value={formPassword}
              onChange={(e) => setFormPassword(e.target.value)}
              InputProps={{
                endAdornment: (
                  <InputAdornment position="end">
                    <Tooltip title="Generate strong 16-character password">
                      <IconButton onClick={handleGeneratePassword} edge="end" color="primary">
                        <AutoFixHighIcon />
                      </IconButton>
                    </Tooltip>
                  </InputAdornment>
                ),
              }}
            />

            <TextField
              fullWidth
              multiline
              rows={2}
              label="Secret Notes (Encrypted with AES-256)"
              placeholder="Security questions, candidate ID, application notes..."
              value={formNotes}
              onChange={(e) => setFormNotes(e.target.value)}
            />
          </Box>
        </DialogContent>
        <DialogActions sx={{ p: 2 }}>
          <Button onClick={() => setCredentialModalOpen(false)}>Cancel</Button>
          <Button onClick={handleSaveCredential} variant="contained">
            Save Credential
          </Button>
        </DialogActions>
      </Dialog>

      {/* Snackbar Alerts */}
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
