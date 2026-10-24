import React, { useState, useEffect } from 'react'
import { useLocation, useNavigate } from 'react-router-dom'
import {
  Box, Drawer, AppBar, Toolbar, Typography, List, ListItemButton,
  ListItemIcon, ListItemText, IconButton, Divider, Avatar, Tooltip,
} from '@mui/material'
import DashboardIcon from '@mui/icons-material/Dashboard'
import SearchIcon from '@mui/icons-material/Search'
import WorkIcon from '@mui/icons-material/Work'
import PeopleIcon from '@mui/icons-material/People'
import DescriptionIcon from '@mui/icons-material/Description'
import FolderIcon from '@mui/icons-material/Folder'
import PersonIcon from '@mui/icons-material/Person'
import MenuIcon from '@mui/icons-material/Menu'
import ChevronLeftIcon from '@mui/icons-material/ChevronLeft'
import RocketLaunchIcon from '@mui/icons-material/RocketLaunch'
import BusinessIcon from '@mui/icons-material/Business'
import AssignmentIcon from '@mui/icons-material/Assignment'
import CalendarMonthIcon from '@mui/icons-material/CalendarMonth'
import MenuBookIcon from '@mui/icons-material/MenuBook'
import FlightTakeoffIcon from '@mui/icons-material/FlightTakeoff'
import ViewKanbanIcon from '@mui/icons-material/ViewKanban'
import AlarmOnIcon from '@mui/icons-material/AlarmOn'
import LockIcon from '@mui/icons-material/Lock'
import { SearchModal } from './SearchModal'

const DRAWER_WIDTH = 240
const DRAWER_COLLAPSED = 72

const navItems = [
  { label: 'Dashboard', icon: <DashboardIcon />, path: '/dashboard' },
  { label: 'Global Search', icon: <SearchIcon />, path: '/search' },
  { label: 'Profile', icon: <PersonIcon />, path: '/profile' },
  { label: 'Companies', icon: <BusinessIcon />, path: '/companies' },
  { label: 'Jobs', icon: <WorkIcon />, path: '/jobs' },
  { label: 'Board', icon: <ViewKanbanIcon />, path: '/board' },
  { label: 'Applications', icon: <AssignmentIcon />, path: '/applications' },
  { label: 'Follow-Ups', icon: <AlarmOnIcon />, path: '/followup' },
  { label: 'Interviews', icon: <CalendarMonthIcon />, path: '/interviews' },
  { label: 'Knowledge Base', icon: <MenuBookIcon />, path: '/wiki' },
  { label: 'Sponsorship', icon: <FlightTakeoffIcon />, path: '/sponsorship' },
  { label: 'Contacts', icon: <PeopleIcon />, path: '/contacts' },
  { label: 'Resumes', icon: <DescriptionIcon />, path: '/resumes' },
  { label: 'Documents', icon: <FolderIcon />, path: '/documents' },
  { label: 'Security Vault', icon: <LockIcon />, path: '/security' },
]

interface LayoutProps { children: React.ReactNode }

export default function Layout({ children }: LayoutProps) {
  const [open, setOpen] = useState(true)
  const [isSearchOpen, setIsSearchOpen] = useState(false)
  const location = useLocation()
  const navigate = useNavigate()

  const drawerWidth = open ? DRAWER_WIDTH : DRAWER_COLLAPSED

  // Global shortcut Ctrl+K / Cmd+K
  useEffect(() => {
    const handleKeyDown = (e: KeyboardEvent) => {
      if ((e.ctrlKey || e.metaKey) && e.key.toLowerCase() === 'k') {
        e.preventDefault()
        setIsSearchOpen(true)
      }
    }
    window.addEventListener('keydown', handleKeyDown)
    return () => window.removeEventListener('keydown', handleKeyDown)
  }, [])

  return (
    <Box sx={{ display: 'flex', height: '100vh', overflow: 'hidden' }}>
      {/* Search Modal Spotlight */}
      <SearchModal open={isSearchOpen} onClose={() => setIsSearchOpen(false)} />

      {/* Sidebar */}
      <Drawer
        variant="permanent"
        sx={{
          width: drawerWidth,
          flexShrink: 0,
          transition: 'width 0.2s ease',
          '& .MuiDrawer-paper': {
            width: drawerWidth,
            boxSizing: 'border-box',
            transition: 'width 0.2s ease',
            overflow: 'hidden',
            bgcolor: 'background.paper',
            borderRight: '1px solid rgba(255,255,255,0.08)',
          },
        }}
      >
        <Toolbar sx={{ justifyContent: open ? 'space-between' : 'center', px: 2 }}>
          {open && (
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
              <RocketLaunchIcon color="primary" />
              <Typography variant="h6" color="primary" fontWeight={700}>CareerOS</Typography>
            </Box>
          )}
          <IconButton onClick={() => setOpen(!open)} size="small">
            {open ? <ChevronLeftIcon /> : <MenuIcon />}
          </IconButton>
        </Toolbar>
        <Divider />
        <List sx={{ pt: 1, overflowY: 'auto', flexGrow: 1 }}>
          {navItems.map((item) => {
            const active = location.pathname === item.path
            return (
              <Tooltip key={item.path} title={!open ? item.label : ''} placement="right">
                <ListItemButton
                  onClick={() => navigate(item.path)}
                  selected={active}
                  sx={{
                    mx: 1, mb: 0.5, borderRadius: 2,
                    justifyContent: open ? 'initial' : 'center',
                    '&.Mui-selected': {
                      bgcolor: 'primary.main',
                      color: 'white',
                      '&:hover': { bgcolor: 'primary.dark' },
                      '& .MuiListItemIcon-root': { color: 'white' },
                    },
                  }}
                >
                  <ListItemIcon sx={{ minWidth: open ? 40 : 'auto', color: active ? 'white' : 'text.secondary' }}>
                    {item.icon}
                  </ListItemIcon>
                  {open && <ListItemText primary={item.label} />}
                </ListItemButton>
              </Tooltip>
            )
          })}
        </List>
        <Box sx={{ mt: 'auto', p: 2, display: 'flex', alignItems: 'center', gap: 1.5, borderTop: '1px solid rgba(255,255,255,0.08)' }}>
          <Avatar sx={{ width: 36, height: 36, bgcolor: 'primary.main', fontSize: 14 }}>U</Avatar>
          {open && (
            <Box>
              <Typography variant="body2" fontWeight={600}>You</Typography>
              <Typography variant="caption" color="text.secondary">Local User</Typography>
            </Box>
          )}
        </Box>
      </Drawer>

      {/* Main content */}
      <Box sx={{ flexGrow: 1, display: 'flex', flexDirection: 'column', overflow: 'hidden' }}>
        <AppBar
          position="static"
          elevation={0}
          sx={{ bgcolor: 'background.paper', borderBottom: '1px solid rgba(255,255,255,0.08)' }}
        >
          <Toolbar sx={{ gap: 2 }}>
            <Typography variant="h6" fontWeight={600} color="text.primary">
              {navItems.find((n) => n.path === location.pathname)?.label ?? 'CareerOS'}
            </Typography>

            <Box sx={{ flexGrow: 1 }} />

            {/* Quick Search Trigger */}
            <Box
              onClick={() => setIsSearchOpen(true)}
              sx={{
                display: 'flex',
                alignItems: 'center',
                gap: 1,
                bgcolor: 'action.hover',
                border: '1px solid',
                borderColor: 'divider',
                borderRadius: 2,
                px: 2,
                py: 0.6,
                cursor: 'pointer',
                color: 'text.secondary',
                transition: 'all 0.2s',
                '&:hover': {
                  borderColor: 'primary.main',
                  bgcolor: 'action.selected',
                },
              }}
            >
              <SearchIcon fontSize="small" />
              <Typography variant="body2" sx={{ fontSize: '0.875rem' }}>
                Search CareerOS...
              </Typography>
              <Box
                sx={{
                  bgcolor: 'background.paper',
                  border: '1px solid',
                  borderColor: 'divider',
                  borderRadius: 1,
                  px: 0.6,
                  py: 0.1,
                  fontSize: '0.7rem',
                  fontWeight: 600,
                  ml: 1,
                }}
              >
                Ctrl K
              </Box>
            </Box>

            <Typography variant="caption" color="text.secondary">
              v0.1.0 · Local-First
            </Typography>
          </Toolbar>
        </AppBar>
        <Box sx={{ flexGrow: 1, overflow: 'auto', p: 3, bgcolor: 'background.default' }}>
          {children}
        </Box>
      </Box>
    </Box>
  )
}
