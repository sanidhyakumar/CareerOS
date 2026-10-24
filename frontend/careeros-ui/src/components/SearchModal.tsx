import React, { useState, useEffect, useRef } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Dialog,
  DialogContent,
  TextField,
  InputAdornment,
  Box,
  Typography,
  Chip,
  List,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  IconButton,
  CircularProgress,
  Divider,
} from '@mui/material';
import SearchIcon from '@mui/icons-material/Search';
import CloseIcon from '@mui/icons-material/Close';
import WorkIcon from '@mui/icons-material/Work';
import BusinessIcon from '@mui/icons-material/Business';
import AssignmentTurnedInIcon from '@mui/icons-material/AssignmentTurnedIn';
import PersonIcon from '@mui/icons-material/Person';
import DescriptionIcon from '@mui/icons-material/Description';
import MenuBookIcon from '@mui/icons-material/MenuBook';
import EventAvailableIcon from '@mui/icons-material/EventAvailable';
import ArrowForwardIcon from '@mui/icons-material/ArrowForward';
import { searchApi, type SearchResultItem } from '../api/search';

interface SearchModalProps {
  open: boolean;
  onClose: () => void;
}

const CATEGORIES = [
  'All',
  'Job',
  'Company',
  'Application',
  'Contact',
  'Document',
  'Wiki',
  'Interview',
];

export const getEntityIcon = (type: string) => {
  switch (type?.toLowerCase()) {
    case 'job':
      return <WorkIcon fontSize="small" sx={{ color: '#2563eb' }} />;
    case 'company':
      return <BusinessIcon fontSize="small" sx={{ color: '#4f46e5' }} />;
    case 'application':
      return <AssignmentTurnedInIcon fontSize="small" sx={{ color: '#059669' }} />;
    case 'contact':
      return <PersonIcon fontSize="small" sx={{ color: '#ea580c' }} />;
    case 'document':
      return <DescriptionIcon fontSize="small" sx={{ color: '#0284c7' }} />;
    case 'wiki':
      return <MenuBookIcon fontSize="small" sx={{ color: '#9333ea' }} />;
    case 'interview':
      return <EventAvailableIcon fontSize="small" sx={{ color: '#0d9488' }} />;
    default:
      return <SearchIcon fontSize="small" sx={{ color: '#64748b' }} />;
  }
};

export const getEntityColor = (type: string) => {
  switch (type?.toLowerCase()) {
    case 'job':
      return { bg: '#dbeafe', text: '#1d4ed8' };
    case 'company':
      return { bg: '#e0e7ff', text: '#4338ca' };
    case 'application':
      return { bg: '#d1fae5', text: '#065f46' };
    case 'contact':
      return { bg: '#ffedd5', text: '#9a3412' };
    case 'document':
      return { bg: '#e0f2fe', text: '#0369a1' };
    case 'wiki':
      return { bg: '#f3e8ff', text: '#6b21a8' };
    case 'interview':
      return { bg: '#ccfbf1', text: '#115e59' };
    default:
      return { bg: '#f1f5f9', text: '#475569' };
  }
};

export const SearchModal: React.FC<SearchModalProps> = ({ open, onClose }) => {
  const navigate = useNavigate();
  const [query, setQuery] = useState('');
  const [selectedCategory, setSelectedCategory] = useState('All');
  const [loading, setLoading] = useState(false);
  const [results, setResults] = useState<SearchResultItem[]>([]);
  const [selectedIndex, setSelectedIndex] = useState(0);
  const listRef = useRef<HTMLUListElement>(null);

  useEffect(() => {
    if (!open) {
      setQuery('');
      setResults([]);
      setSelectedIndex(0);
    }
  }, [open]);

  useEffect(() => {
    if (!query.trim()) {
      setResults([]);
      setLoading(false);
      return;
    }

    const timer = setTimeout(async () => {
      setLoading(true);
      try {
        const resp = await searchApi.search(query.trim(), selectedCategory, 15);
        setResults(resp.results || []);
        setSelectedIndex(0);
      } catch (err) {
        console.error('Error during global search:', err);
      } finally {
        setLoading(false);
      }
    }, 200);

    return () => clearTimeout(timer);
  }, [query, selectedCategory]);

  const handleKeyDown = (e: React.KeyboardEvent) => {
    if (e.key === 'ArrowDown') {
      e.preventDefault();
      setSelectedIndex((prev) => (results.length > 0 ? (prev + 1) % results.length : 0));
    } else if (e.key === 'ArrowUp') {
      e.preventDefault();
      setSelectedIndex((prev) => (results.length > 0 ? (prev - 1 + results.length) % results.length : 0));
    } else if (e.key === 'Enter') {
      e.preventDefault();
      if (results.length > 0 && results[selectedIndex]) {
        handleSelectResult(results[selectedIndex]);
      } else if (query.trim()) {
        goToFullSearch();
      }
    }
  };

  const handleSelectResult = (item: SearchResultItem) => {
    onClose();
    if (item.actionUrl) {
      navigate(item.actionUrl);
    }
  };

  const goToFullSearch = () => {
    onClose();
    navigate(`/search?q=${encodeURIComponent(query)}`);
  };

  return (
    <Dialog
      open={open}
      onClose={onClose}
      maxWidth="md"
      fullWidth
      PaperProps={{
        sx: {
          borderRadius: 3,
          boxShadow: '0 20px 40px rgba(0,0,0,0.15)',
          overflow: 'hidden',
          top: -80,
        },
      }}
    >
      <Box sx={{ p: 2, pb: 1 }}>
        <TextField
          autoFocus
          fullWidth
          variant="standard"
          placeholder="Search jobs, companies, applications, contacts, docs, wiki, interviews..."
          value={query}
          onChange={(e) => setQuery(e.target.value)}
          onKeyDown={handleKeyDown}
          InputProps={{
            disableUnderline: true,
            startAdornment: (
              <InputAdornment position="start">
                <SearchIcon sx={{ color: 'primary.main', fontSize: 28, mr: 1 }} />
              </InputAdornment>
            ),
            endAdornment: (
              <InputAdornment position="end">
                {loading && <CircularProgress size={20} sx={{ mr: 1 }} />}
                {query && (
                  <IconButton size="small" onClick={() => setQuery('')}>
                    <CloseIcon fontSize="small" />
                  </IconButton>
                )}
                <Chip
                  label="ESC"
                  size="small"
                  onClick={onClose}
                  sx={{ fontSize: '0.65rem', height: 20, cursor: 'pointer', ml: 0.5 }}
                />
              </InputAdornment>
            ),
            sx: { fontSize: '1.15rem' },
          }}
        />

        {/* Filter categories */}
        <Box sx={{ display: 'flex', gap: 0.8, mt: 1.5, overflowX: 'auto', pb: 0.5 }}>
          {CATEGORIES.map((cat) => (
            <Chip
              key={cat}
              label={cat}
              size="small"
              clickable
              color={selectedCategory === cat ? 'primary' : 'default'}
              variant={selectedCategory === cat ? 'filled' : 'outlined'}
              onClick={() => setSelectedCategory(cat)}
              sx={{ fontWeight: selectedCategory === cat ? 600 : 400 }}
            />
          ))}
        </Box>
      </Box>

      <Divider />

      <DialogContent sx={{ p: 0, maxHeight: 420, overflowY: 'auto' }}>
        {results.length > 0 ? (
          <List ref={listRef} sx={{ p: 0 }}>
            {results.map((item, idx) => {
              const isSelected = idx === selectedIndex;
              const colorInfo = getEntityColor(item.entityType);

              return (
                <ListItemButton
                  key={`${item.entityType}-${item.entityId}-${idx}`}
                  selected={isSelected}
                  onClick={() => handleSelectResult(item)}
                  sx={{
                    px: 2.5,
                    py: 1.2,
                    borderLeft: isSelected ? '4px solid' : '4px solid transparent',
                    borderLeftColor: 'primary.main',
                    backgroundColor: isSelected ? 'action.hover' : 'inherit',
                  }}
                >
                  <ListItemIcon sx={{ minWidth: 40 }}>
                    {getEntityIcon(item.entityType)}
                  </ListItemIcon>
                  <ListItemText
                    primary={
                      <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                        <Typography variant="subtitle2" sx={{ fontWeight: 600, color: 'text.primary' }}>
                          {item.title}
                        </Typography>
                        <Chip
                          label={item.entityType}
                          size="small"
                          sx={{
                            height: 18,
                            fontSize: '0.65rem',
                            fontWeight: 600,
                            backgroundColor: colorInfo.bg,
                            color: colorInfo.text,
                          }}
                        />
                      </Box>
                    }
                    secondary={
                      <Box sx={{ mt: 0.3 }}>
                        <Typography variant="caption" sx={{ color: 'text.secondary', display: 'block' }}>
                          {item.subtitle}
                        </Typography>
                        {item.snippet && (
                          <Typography
                            variant="caption"
                            sx={{
                              color: 'text.secondary',
                              display: '-webkit-box',
                              WebkitLineClamp: 1,
                              WebkitBoxOrient: 'vertical',
                              overflow: 'hidden',
                              mt: 0.2,
                              '& mark': {
                                backgroundColor: '#fef08a',
                                color: '#854d0e',
                                px: 0.4,
                                borderRadius: 0.5,
                                fontWeight: 600,
                              },
                            }}
                            dangerouslySetInnerHTML={{ __html: item.snippet }}
                          />
                        )}
                      </Box>
                    }
                  />
                  {isSelected && (
                    <Box sx={{ display: 'flex', alignItems: 'center', ml: 1, color: 'primary.main' }}>
                      <Typography variant="caption" sx={{ mr: 0.5 }}>Jump</Typography>
                      <ArrowForwardIcon fontSize="small" />
                    </Box>
                  )}
                </ListItemButton>
              );
            })}
          </List>
        ) : query.trim() && !loading ? (
          <Box sx={{ p: 4, textAlign: 'center' }}>
            <Typography variant="body1" color="text.secondary">
              No matching records found for "{query}".
            </Typography>
            <Typography variant="caption" color="text.secondary" sx={{ mt: 0.5, display: 'block' }}>
              Try searching by company, role, technology, tag or document title.
            </Typography>
          </Box>
        ) : !query.trim() ? (
          <Box sx={{ p: 3, textAlign: 'center', color: 'text.secondary' }}>
            <Typography variant="body2">
              Type keywords to search across all your local career data instantly.
            </Typography>
            <Typography variant="caption" sx={{ mt: 0.5, display: 'block' }}>
              Tip: Press <kbd style={{ padding: '2px 4px', background: '#f1f5f9', borderRadius: 4 }}>Ctrl</kbd> + <kbd style={{ padding: '2px 4px', background: '#f1f5f9', borderRadius: 4 }}>K</kbd> anywhere in CareerOS to open search.
            </Typography>
          </Box>
        ) : null}
      </DialogContent>

      {query.trim() && (
        <>
          <Divider />
          <Box
            sx={{
              p: 1.5,
              px: 2.5,
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'space-between',
              backgroundColor: 'grey.50',
            }}
          >
            <Typography variant="caption" color="text.secondary">
              {results.length} results found
            </Typography>
            <Chip
              label="View in Search Center →"
              size="small"
              clickable
              color="primary"
              variant="outlined"
              onClick={goToFullSearch}
              sx={{ fontWeight: 600 }}
            />
          </Box>
        </>
      )}
    </Dialog>
  );
};
