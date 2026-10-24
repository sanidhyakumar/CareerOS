import React, { useState } from 'react'
import {
  Box,
  Avatar,
  Typography,
  Chip,
  IconButton,
  Menu,
  MenuItem,
  ListItemIcon,
  ListItemText,
  Divider,
} from '@mui/material'
import ArrowDropDownIcon from '@mui/icons-material/ArrowDropDown'
import CheckIcon from '@mui/icons-material/Check'
import AddIcon from '@mui/icons-material/Add'
import DashboardIcon from '@mui/icons-material/Dashboard'
import { useNavigate } from 'react-router-dom'
import { useProfile } from '../context/ProfileContext'

interface ProfileSwitcherProps {
  compact?: boolean
}

export const ProfileSwitcher: React.FC<ProfileSwitcherProps> = ({ compact = false }) => {
  const { activeProfile, profiles, switchProfile } = useProfile()
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null)
  const navigate = useNavigate()

  const open = Boolean(anchorEl)

  const handleClick = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorEl(event.currentTarget)
  }

  const handleClose = () => {
    setAnchorEl(null)
  }

  const handleSelect = async (profileId: string) => {
    handleClose()
    await switchProfile(profileId)
  }

  if (!activeProfile) {
    return null
  }

  const initials = activeProfile.name
    .split(' ')
    .map((n) => n[0])
    .join('')
    .substring(0, 2)
    .toUpperCase()

  return (
    <>
      <Box
        onClick={handleClick}
        sx={{
          display: 'flex',
          alignItems: 'center',
          gap: 1.2,
          p: 0.6,
          px: 1.5,
          borderRadius: 2,
          bgcolor: 'action.hover',
          border: '1px solid',
          borderColor: 'divider',
          cursor: 'pointer',
          transition: 'all 0.2s',
          '&:hover': {
            bgcolor: 'action.selected',
            borderColor: activeProfile.colorCode || 'primary.main',
          },
        }}
      >
        <Avatar
          sx={{
            width: compact ? 28 : 34,
            height: compact ? 28 : 34,
            bgcolor: activeProfile.colorCode || '#2563eb',
            fontSize: compact ? 12 : 14,
            fontWeight: 700,
            boxShadow: '0 2px 6px rgba(0,0,0,0.2)',
          }}
        >
          {initials}
        </Avatar>

        {!compact && (
          <Box sx={{ display: 'flex', flexDirection: 'column', alignItems: 'flex-start' }}>
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.8 }}>
              <Typography variant="body2" fontWeight={700} color="text.primary">
                {activeProfile.name}
              </Typography>
              <Chip
                label={activeProfile.relationship}
                size="small"
                sx={{
                  height: 18,
                  fontSize: '0.65rem',
                  fontWeight: 600,
                  bgcolor: `${activeProfile.colorCode}22`,
                  color: activeProfile.colorCode,
                  border: `1px solid ${activeProfile.colorCode}55`,
                }}
              />
            </Box>
            <Typography variant="caption" color="text.secondary" sx={{ fontSize: '0.7rem' }}>
              {activeProfile.headline || `${activeProfile.relationship} Profile`}
            </Typography>
          </Box>
        )}

        <IconButton size="small" sx={{ p: 0.2, ml: compact ? 0 : 0.5 }}>
          <ArrowDropDownIcon fontSize="small" />
        </IconButton>
      </Box>

      {/* Switcher Dropdown Menu */}
      <Menu
        anchorEl={anchorEl}
        open={open}
        onClose={handleClose}
        PaperProps={{
          sx: {
            width: 280,
            maxHeight: 460,
            borderRadius: 2.5,
            boxShadow: '0 8px 24px rgba(0,0,0,0.3)',
            p: 1,
          },
        }}
        transformOrigin={{ horizontal: 'right', vertical: 'top' }}
        anchorOrigin={{ horizontal: 'right', vertical: 'bottom' }}
      >
        <Box sx={{ px: 1.5, py: 1 }}>
          <Typography variant="caption" fontWeight={700} color="text.secondary" sx={{ textTransform: 'uppercase' }}>
            Multi-Person Workspace
          </Typography>
          <Typography variant="body2" color="text.primary" fontWeight={600}>
            Switch Profile Context
          </Typography>
        </Box>
        <Divider sx={{ my: 0.5 }} />

        {profiles.map((p) => {
          const isSelected = p.id === activeProfile.id
          const pInitials = p.name
            .split(' ')
            .map((n) => n[0])
            .join('')
            .substring(0, 2)
            .toUpperCase()

          return (
            <MenuItem
              key={p.id}
              onClick={() => handleSelect(p.id)}
              sx={{
                borderRadius: 1.5,
                my: 0.3,
                bgcolor: isSelected ? 'action.selected' : 'transparent',
              }}
            >
              <ListItemIcon sx={{ minWidth: 36 }}>
                <Avatar
                  sx={{
                    width: 28,
                    height: 28,
                    bgcolor: p.colorCode || '#2563eb',
                    fontSize: 12,
                    fontWeight: 700,
                  }}
                >
                  {pInitials}
                </Avatar>
              </ListItemIcon>
              <ListItemText
                primary={
                  <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                    <Typography variant="body2" fontWeight={isSelected ? 700 : 500}>
                      {p.name}
                    </Typography>
                    <Chip
                      label={p.relationship}
                      size="small"
                      sx={{
                        height: 16,
                        fontSize: '0.6rem',
                        fontWeight: 600,
                        bgcolor: `${p.colorCode}22`,
                        color: p.colorCode,
                      }}
                    />
                  </Box>
                }
                secondary={
                  <Typography variant="caption" color="text.secondary" noWrap>
                    {p.applicationCount} apps · {p.documentCount} docs
                  </Typography>
                }
              />
              {isSelected && <CheckIcon fontSize="small" color="primary" />}
            </MenuItem>
          )
        })}

        <Divider sx={{ my: 1 }} />

        <MenuItem
          onClick={() => {
            handleClose()
            navigate('/profiles')
          }}
          sx={{ borderRadius: 1.5 }}
        >
          <ListItemIcon>
            <DashboardIcon fontSize="small" />
          </ListItemIcon>
          <ListItemText primary="Family Workspace Hub" />
        </MenuItem>

        <MenuItem
          onClick={() => {
            handleClose()
            navigate('/profiles?action=new')
          }}
          sx={{ borderRadius: 1.5 }}
        >
          <ListItemIcon>
            <AddIcon fontSize="small" />
          </ListItemIcon>
          <ListItemText primary="Add New Profile" />
        </MenuItem>
      </Menu>
    </>
  )
}
