import React, { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import {
  Box, Typography, Paper, Card, CardContent, IconButton, Button,
  Dialog, DialogTitle, DialogContent, DialogActions, TextField,
  FormControl, InputLabel, Select, MenuItem, Chip, Stack,
  CircularProgress, Alert, Tooltip, Grid, Tabs, Tab,
  Menu, ListItemIcon, ListItemText
} from '@mui/material'
import AddIcon from '@mui/icons-material/Add'
import DeleteIcon from '@mui/icons-material/Delete'
import CheckCircleIcon from '@mui/icons-material/CheckCircle'
import SnoozeIcon from '@mui/icons-material/Snooze'
import NotificationsActiveIcon from '@mui/icons-material/NotificationsActive'
import RefreshIcon from '@mui/icons-material/Refresh'
import WarningIcon from '@mui/icons-material/Warning'
import ErrorIcon from '@mui/icons-material/Error'
import InfoIcon from '@mui/icons-material/Info'
import AssignmentIcon from '@mui/icons-material/Assignment'
import FolderIcon from '@mui/icons-material/Folder'
import CalendarMonthIcon from '@mui/icons-material/CalendarMonth'
import PeopleIcon from '@mui/icons-material/People'
import LaunchIcon from '@mui/icons-material/Launch'
import AlarmOnIcon from '@mui/icons-material/AlarmOn'
import AccessTimeIcon from '@mui/icons-material/AccessTime'

import {
  followUpApi, ReminderCategory, ReminderCategoryLabels,
  ReminderSeverity, ReminderSeverityLabels
} from '../api/followup'
import type {
  Reminder, FollowUpSummary, CreateCustomReminderRequest
} from '../api/followup'

// Severity Styling
const SEVERITY_CONFIG: Record<number, { label: string; color: 'error' | 'warning' | 'info' | 'default'; icon: React.ReactElement }> = {
  [ReminderSeverity.Critical]: { label: 'Critical / Expired', color: 'error', icon: <ErrorIcon fontSize="small" /> },
  [ReminderSeverity.High]: { label: 'High Priority', color: 'error', icon: <WarningIcon fontSize="small" /> },
  [ReminderSeverity.Medium]: { label: 'Action Due', color: 'warning', icon: <InfoIcon fontSize="small" /> },
  [ReminderSeverity.Low]: { label: 'Advisory', color: 'info', icon: <InfoIcon fontSize="small" /> },
}

// Category Icons
const CATEGORY_ICONS: Record<number, React.ReactElement> = {
  [ReminderCategory.StaleApplication]: <AssignmentIcon color="warning" />,
  [ReminderCategory.ExpiringDocument]: <FolderIcon color="error" />,
  [ReminderCategory.UpcomingInterview]: <CalendarMonthIcon color="primary" />,
  [ReminderCategory.MissingAction]: <WarningIcon color="warning" />,
  [ReminderCategory.ContactFollowUp]: <PeopleIcon color="info" />,
  [ReminderCategory.General]: <NotificationsActiveIcon color="action" />,
}

export const FollowUpPage: React.FC = () => {
  const navigate = useNavigate()
  const [reminders, setReminders] = useState<Reminder[]>([])
  const [summary, setSummary] = useState<FollowUpSummary | null>(null)
  const [loading, setLoading] = useState(true)
  const [scanning, setScanning] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [successMessage, setSuccessMessage] = useState<string | null>(null)

  // Tab Filtering
  const [activeTab, setActiveTab] = useState<string>('all')

  // Snooze Menu State
  const [snoozeAnchorEl, setSnoozeAnchorEl] = useState<null | HTMLElement>(null)
  const [selectedReminderForSnooze, setSelectedReminderForSnooze] = useState<string | null>(null)

  // Dialog State
  const [openDialog, setOpenDialog] = useState(false)
  const [formTitle, setFormTitle] = useState('')
  const [formMessage, setFormMessage] = useState('')
  const [formCategory, setFormCategory] = useState<number>(ReminderCategory.General)
  const [formSeverity, setFormSeverity] = useState<number>(ReminderSeverity.Medium)
  const [formActionUrl, setFormActionUrl] = useState('/applications')
  const [formDueDate, setFormDueDate] = useState('')

  const loadData = async () => {
    try {
      setLoading(true)
      const isDismissedTab = activeTab === 'dismissed'
      const isSnoozedTab = activeTab === 'snoozed'

      let categoryParam: number | undefined = undefined
      if (activeTab === 'stale') categoryParam = ReminderCategory.StaleApplication
      if (activeTab === 'documents') categoryParam = ReminderCategory.ExpiringDocument
      if (activeTab === 'interviews') categoryParam = ReminderCategory.UpcomingInterview
      if (activeTab === 'contacts') categoryParam = ReminderCategory.ContactFollowUp

      const [items, sum] = await Promise.all([
        followUpApi.getAll({
          category: categoryParam,
          includeDismissed: isDismissedTab,
          includeSnoozed: isSnoozedTab || isDismissedTab,
        }),
        followUpApi.getSummary(),
      ])

      // If viewing snoozed tab, filter only currently snoozed
      if (isSnoozedTab) {
        setReminders(items.filter((r) => r.isSnoozed && !r.isDismissed))
      } else if (isDismissedTab) {
        setReminders(items.filter((r) => r.isDismissed))
      } else {
        setReminders(items.filter((r) => !r.isDismissed && !r.isSnoozed))
      }

      setSummary(sum)
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Failed to load reminders')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    loadData()
  }, [activeTab])

  const handleRunScan = async () => {
    try {
      setScanning(true)
      const res = await followUpApi.scan()
      setSuccessMessage(
        res.newRemindersCount > 0
          ? `Scan complete: ${res.newRemindersCount} new actionable reminders generated!`
          : 'Scan complete: All follow-ups and documents are up to date.'
      )
      await loadData()
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Proactive scan failed')
    } finally {
      setScanning(false)
    }
  }

  const handleDismiss = async (id: string) => {
    try {
      await followUpApi.dismiss(id)
      setSuccessMessage('Reminder marked as completed / dismissed.')
      loadData()
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Failed to dismiss reminder')
    }
  }

  const handleOpenSnoozeMenu = (e: React.MouseEvent<HTMLElement>, id: string) => {
    setSnoozeAnchorEl(e.currentTarget)
    setSelectedReminderForSnooze(id)
  }

  const handleCloseSnoozeMenu = () => {
    setSnoozeAnchorEl(null)
    setSelectedReminderForSnooze(null)
  }

  const handleApplySnooze = async (days: number) => {
    if (!selectedReminderForSnooze) return
    try {
      await followUpApi.snooze(selectedReminderForSnooze, days)
      setSuccessMessage(`Reminder snoozed for ${days} day(s).`)
      handleCloseSnoozeMenu()
      loadData()
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Failed to snooze reminder')
    }
  }

  const handleDelete = async (id: string) => {
    if (!window.confirm('Delete this reminder?')) return
    try {
      await followUpApi.delete(id)
      setSuccessMessage('Reminder deleted.')
      loadData()
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Failed to delete reminder')
    }
  }

  const handleOpenCreate = () => {
    setFormTitle('')
    setFormMessage('')
    setFormCategory(ReminderCategory.General)
    setFormSeverity(ReminderSeverity.Medium)
    setFormActionUrl('/applications')
    setFormDueDate('')
    setOpenDialog(true)
  }

  const handleSubmitCustom = async () => {
    if (!formTitle.trim()) {
      setError('Title is required')
      return
    }

    try {
      const req: CreateCustomReminderRequest = {
        title: formTitle.trim(),
        message: formMessage.trim(),
        category: formCategory,
        severity: formSeverity,
        actionUrl: formActionUrl.trim() || undefined,
        dueDate: formDueDate ? new Date(formDueDate).toISOString() : undefined,
      }
      await followUpApi.createCustom(req)
      setSuccessMessage('Custom reminder created successfully!')
      setOpenDialog(false)
      loadData()
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Failed to create reminder')
    }
  }

  const handleExecuteAction = (actionUrl?: string) => {
    if (actionUrl) {
      navigate(actionUrl)
    }
  }

  return (
    <Box sx={{ p: 3, maxWidth: 1600, margin: '0 auto' }}>
      {/* Header and Actions */}
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Box>
          <Typography variant="h4" fontWeight="bold" sx={{ display: 'flex', alignItems: 'center', gap: 1.5 }}>
            <AlarmOnIcon fontSize="large" color="primary" />
            Follow-Up & Automation Engine
          </Typography>
          <Typography variant="subtitle1" color="text.secondary">
            Proactive alerts for stale applications, expiring documents, upcoming interviews, and recruiter touchpoints.
          </Typography>
        </Box>
        <Stack direction="row" spacing={1.5}>
          <Button
            variant="outlined"
            startIcon={scanning ? <CircularProgress size={16} /> : <RefreshIcon />}
            onClick={handleRunScan}
            disabled={scanning}
          >
            {scanning ? 'Scanning...' : 'Scan Now'}
          </Button>

          <Button
            variant="contained"
            color="primary"
            startIcon={<AddIcon />}
            onClick={handleOpenCreate}
          >
            New Reminder
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

      {/* KPI Metric Cards */}
      {summary && (
        <Grid container spacing={2} sx={{ mb: 3 }}>
          <Grid item xs={6} sm={4} md={2}>
            <Card sx={{ bgcolor: 'background.paper', border: '1px solid', borderColor: 'divider' }}>
              <CardContent sx={{ py: 1.5, px: 2, '&:last-child': { pb: 1.5 } }}>
                <Typography variant="caption" color="text.secondary" fontWeight="bold">
                  TOTAL ACTIVE
                </Typography>
                <Typography variant="h5" fontWeight="bold">
                  {summary.totalActive}
                </Typography>
              </CardContent>
            </Card>
          </Grid>

          <Grid item xs={6} sm={4} md={2}>
            <Card sx={{ bgcolor: 'warning.light', color: 'warning.contrastText' }}>
              <CardContent sx={{ py: 1.5, px: 2, '&:last-child': { pb: 1.5 } }}>
                <Typography variant="caption" fontWeight="bold">
                  STALE APPLICATIONS
                </Typography>
                <Typography variant="h5" fontWeight="bold">
                  {summary.staleApplicationsCount}
                </Typography>
              </CardContent>
            </Card>
          </Grid>

          <Grid item xs={6} sm={4} md={2}>
            <Card sx={{ bgcolor: 'error.light', color: 'error.contrastText' }}>
              <CardContent sx={{ py: 1.5, px: 2, '&:last-child': { pb: 1.5 } }}>
                <Typography variant="caption" fontWeight="bold">
                  EXPIRING DOCUMENTS
                </Typography>
                <Typography variant="h5" fontWeight="bold">
                  {summary.expiringDocumentsCount}
                </Typography>
              </CardContent>
            </Card>
          </Grid>

          <Grid item xs={6} sm={4} md={2}>
            <Card sx={{ bgcolor: 'primary.light', color: 'primary.contrastText' }}>
              <CardContent sx={{ py: 1.5, px: 2, '&:last-child': { pb: 1.5 } }}>
                <Typography variant="caption" fontWeight="bold">
                  UPCOMING INTERVIEWS
                </Typography>
                <Typography variant="h5" fontWeight="bold">
                  {summary.upcomingInterviewsCount}
                </Typography>
              </CardContent>
            </Card>
          </Grid>

          <Grid item xs={6} sm={4} md={2}>
            <Card sx={{ bgcolor: 'info.light', color: 'info.contrastText' }}>
              <CardContent sx={{ py: 1.5, px: 2, '&:last-child': { pb: 1.5 } }}>
                <Typography variant="caption" fontWeight="bold">
                  CONTACT TOUCHPOINTS
                </Typography>
                <Typography variant="h5" fontWeight="bold">
                  {summary.contactFollowUpsCount}
                </Typography>
              </CardContent>
            </Card>
          </Grid>

          <Grid item xs={6} sm={4} md={2}>
            <Card sx={{ bgcolor: 'secondary.light', color: 'secondary.contrastText' }}>
              <CardContent sx={{ py: 1.5, px: 2, '&:last-child': { pb: 1.5 } }}>
                <Typography variant="caption" fontWeight="bold">
                  HIGH / URGENT
                </Typography>
                <Typography variant="h5" fontWeight="bold">
                  ! {summary.highOrCriticalCount}
                </Typography>
              </CardContent>
            </Card>
          </Grid>
        </Grid>
      )}

      {/* Category Tabs */}
      <Paper sx={{ mb: 3, border: '1px solid', borderColor: 'divider' }}>
        <Tabs
          value={activeTab}
          onChange={(_, val) => setActiveTab(val)}
          indicatorColor="primary"
          textColor="primary"
          variant="scrollable"
          scrollButtons="auto"
        >
          <Tab value="all" label={`All Active (${summary?.totalActive ?? 0})`} />
          <Tab value="stale" label={`Stale Applications (${summary?.staleApplicationsCount ?? 0})`} />
          <Tab value="documents" label={`Expiring Documents (${summary?.expiringDocumentsCount ?? 0})`} />
          <Tab value="interviews" label={`Upcoming Interviews (${summary?.upcomingInterviewsCount ?? 0})`} />
          <Tab value="contacts" label={`Contact Touchpoints (${summary?.contactFollowUpsCount ?? 0})`} />
          <Tab value="snoozed" label="Snoozed" />
          <Tab value="dismissed" label="Resolved / Dismissed" />
        </Tabs>
      </Paper>

      {/* Reminder Cards Feed */}
      {loading ? (
        <Box sx={{ display: 'flex', justifyContent: 'center', p: 8 }}>
          <CircularProgress />
        </Box>
      ) : reminders.length === 0 ? (
        <Paper sx={{ p: 8, textAlign: 'center', border: '1px solid', borderColor: 'divider' }}>
          <CheckCircleIcon sx={{ fontSize: 60, color: 'success.main', mb: 2 }} />
          <Typography variant="h6" fontWeight="bold">
            All caught up! No active reminders.
          </Typography>
          <Typography variant="body2" color="text.secondary" sx={{ mt: 1, mb: 3 }}>
            The automation engine continuously checks for stale applications, expiring visas/documents, and interview timelines.
          </Typography>
          <Button variant="outlined" startIcon={<RefreshIcon />} onClick={handleRunScan}>
            Trigger Proactive Check
          </Button>
        </Paper>
      ) : (
        <Stack spacing={2}>
          {reminders.map((r) => {
            const sevCfg = SEVERITY_CONFIG[r.severity] || SEVERITY_CONFIG[ReminderSeverity.Medium]
            const categoryIcon = CATEGORY_ICONS[r.category] || <NotificationsActiveIcon />

            return (
              <Card
                key={r.id}
                variant="outlined"
                sx={{
                  borderLeft: '5px solid',
                  borderColor:
                    r.severity === ReminderSeverity.Critical
                      ? 'error.main'
                      : r.severity === ReminderSeverity.High
                      ? 'warning.main'
                      : 'primary.main',
                  transition: 'box-shadow 0.2s',
                  '&:hover': { boxShadow: 3 },
                }}
              >
                <CardContent sx={{ p: 2, '&:last-child': { pb: 2 } }}>
                  <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
                    <Box sx={{ display: 'flex', gap: 1.5, alignItems: 'flex-start', flex: 1 }}>
                      <Box sx={{ mt: 0.5 }}>{categoryIcon}</Box>
                      <Box sx={{ flex: 1 }}>
                        <Stack direction="row" spacing={1} alignItems="center" sx={{ mb: 0.5 }}>
                          <Chip
                            icon={sevCfg.icon}
                            label={sevCfg.label}
                            size="small"
                            color={sevCfg.color}
                            sx={{ height: 20, fontSize: '0.7rem', fontWeight: 'bold' }}
                          />
                          <Chip
                            label={ReminderCategoryLabels[r.category] || r.categoryName}
                            size="small"
                            variant="outlined"
                            sx={{ height: 20, fontSize: '0.7rem' }}
                          />
                          {r.isSnoozed && (
                            <Chip
                              icon={<AccessTimeIcon sx={{ fontSize: '0.75rem !important' }} />}
                              label={`Snoozed until ${r.snoozedUntil ? new Date(r.snoozedUntil).toLocaleDateString() : ''}`}
                              size="small"
                              color="secondary"
                              sx={{ height: 20, fontSize: '0.7rem' }}
                            />
                          )}
                        </Stack>

                        <Typography variant="subtitle1" fontWeight="bold">
                          {r.title}
                        </Typography>

                        <Typography variant="body2" color="text.secondary" sx={{ mt: 0.5 }}>
                          {r.message}
                        </Typography>

                        {r.dueDate && (
                          <Typography variant="caption" color="text.secondary" sx={{ display: 'block', mt: 1 }}>
                            Target Action Date: {new Date(r.dueDate).toLocaleDateString()}
                          </Typography>
                        )}
                      </Box>
                    </Box>

                    {/* Actions Toolbar */}
                    <Stack direction="row" spacing={1} alignItems="center" sx={{ ml: 2 }}>
                      {r.actionUrl && (
                        <Button
                          size="small"
                          variant="contained"
                          color="primary"
                          endIcon={<LaunchIcon />}
                          onClick={() => handleExecuteAction(r.actionUrl)}
                          sx={{ textTransform: 'none' }}
                        >
                          Take Action
                        </Button>
                      )}

                      {!r.isDismissed && (
                        <>
                          <Tooltip title="Snooze Reminder">
                            <IconButton
                              size="small"
                              onClick={(e) => handleOpenSnoozeMenu(e, r.id)}
                            >
                              <SnoozeIcon />
                            </IconButton>
                          </Tooltip>

                          <Tooltip title="Mark Resolved / Dismiss">
                            <IconButton
                              size="small"
                              onClick={() => handleDismiss(r.id)}
                              color="success"
                            >
                              <CheckCircleIcon />
                            </IconButton>
                          </Tooltip>
                        </>
                      )}

                      <Tooltip title="Delete">
                        <IconButton
                          size="small"
                          onClick={() => handleDelete(r.id)}
                          color="error"
                        >
                          <DeleteIcon fontSize="small" />
                        </IconButton>
                      </Tooltip>
                    </Stack>
                  </Box>
                </CardContent>
              </Card>
            )
          })}
        </Stack>
      )}

      {/* Snooze Popup Menu */}
      <Menu
        anchorEl={snoozeAnchorEl}
        open={Boolean(snoozeAnchorEl)}
        onClose={handleCloseSnoozeMenu}
      >
        <MenuItem onClick={() => handleApplySnooze(1)}>
          <ListItemIcon><AccessTimeIcon fontSize="small" /></ListItemIcon>
          <ListItemText primary="Snooze for 1 Day" />
        </MenuItem>
        <MenuItem onClick={() => handleApplySnooze(3)}>
          <ListItemIcon><AccessTimeIcon fontSize="small" /></ListItemIcon>
          <ListItemText primary="Snooze for 3 Days" />
        </MenuItem>
        <MenuItem onClick={() => handleApplySnooze(7)}>
          <ListItemIcon><AccessTimeIcon fontSize="small" /></ListItemIcon>
          <ListItemText primary="Snooze for 1 Week" />
        </MenuItem>
        <MenuItem onClick={() => handleApplySnooze(14)}>
          <ListItemIcon><AccessTimeIcon fontSize="small" /></ListItemIcon>
          <ListItemText primary="Snooze for 2 Weeks" />
        </MenuItem>
      </Menu>

      {/* Create Custom Reminder Dialog */}
      <Dialog
        open={openDialog}
        onClose={() => setOpenDialog(false)}
        maxWidth="sm"
        fullWidth
      >
        <DialogTitle>Create Custom Reminder</DialogTitle>
        <DialogContent dividers>
          <Stack spacing={2} sx={{ mt: 1 }}>
            <TextField
              fullWidth
              label="Reminder Title"
              required
              value={formTitle}
              onChange={(e) => setFormTitle(e.target.value)}
              placeholder="e.g. Follow up with recruiter Sarah at Apple"
            />

            <TextField
              fullWidth
              multiline
              rows={3}
              label="Instructions / Message"
              value={formMessage}
              onChange={(e) => setFormMessage(e.target.value)}
              placeholder="Send quick status email or ask for technical screen feedback..."
            />

            <Stack direction="row" spacing={2}>
              <FormControl fullWidth>
                <InputLabel>Category</InputLabel>
                <Select
                  value={formCategory}
                  label="Category"
                  onChange={(e) => setFormCategory(Number(e.target.value))}
                >
                  {Object.entries(ReminderCategoryLabels).map(([key, label]) => (
                    <MenuItem key={key} value={Number(key)}>
                      {label}
                    </MenuItem>
                  ))}
                </Select>
              </FormControl>

              <FormControl fullWidth>
                <InputLabel>Severity</InputLabel>
                <Select
                  value={formSeverity}
                  label="Severity"
                  onChange={(e) => setFormSeverity(Number(e.target.value))}
                >
                  {Object.entries(ReminderSeverityLabels).map(([key, label]) => (
                    <MenuItem key={key} value={Number(key)}>
                      {label}
                    </MenuItem>
                  ))}
                </Select>
              </FormControl>
            </Stack>

            <TextField
              fullWidth
              label="Action Navigation Link"
              value={formActionUrl}
              onChange={(e) => setFormActionUrl(e.target.value)}
              placeholder="/applications, /contacts, /interviews, /documents"
            />

            <TextField
              fullWidth
              type="date"
              label="Target Due Date"
              InputLabelProps={{ shrink: true }}
              value={formDueDate}
              onChange={(e) => setFormDueDate(e.target.value)}
            />
          </Stack>
        </DialogContent>
        <DialogActions sx={{ px: 3, py: 2 }}>
          <Button onClick={() => setOpenDialog(false)} color="inherit">
            Cancel
          </Button>
          <Button onClick={handleSubmitCustom} variant="contained" color="primary">
            Create Reminder
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  )
}

export default FollowUpPage
