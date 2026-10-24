import React, { useState, useEffect } from 'react';
import { useSearchParams, useNavigate } from 'react-router-dom';
import {
  Box,
  Typography,
  TextField,
  InputAdornment,
  IconButton,
  Button,
  Card,
  CardContent,
  Chip,
  Tabs,
  Tab,
  CircularProgress,
  Alert,
  Snackbar,
  Grid,
  Paper,
} from '@mui/material';
import SearchIcon from '@mui/icons-material/Search';
import CloseIcon from '@mui/icons-material/Close';
import RefreshIcon from '@mui/icons-material/Refresh';
import OpenInNewIcon from '@mui/icons-material/OpenInNew';
import SpeedIcon from '@mui/icons-material/Speed';
import StorageIcon from '@mui/icons-material/Storage';
import { searchApi, type SearchSummary, type SearchStats, type SearchResultItem } from '../api/search';
import { getEntityIcon, getEntityColor } from '../components/SearchModal';

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

export const SearchPage: React.FC = () => {
  const [searchParams, setSearchParams] = useSearchParams();
  const navigate = useNavigate();

  const initialQuery = searchParams.get('q') || '';
  const [query, setQuery] = useState(initialQuery);
  const [selectedType, setSelectedType] = useState('All');
  const [loading, setLoading] = useState(false);
  const [reindexing, setReindexing] = useState(false);
  const [summary, setSummary] = useState<SearchSummary | null>(null);
  const [stats, setStats] = useState<SearchStats | null>(null);
  const [snackbarMessage, setSnackbarMessage] = useState<string | null>(null);

  // Load stats initially
  useEffect(() => {
    loadStats();
  }, []);

  const loadStats = async () => {
    try {
      const s = await searchApi.getStats();
      setStats(s);
    } catch (err) {
      console.error('Failed to load search stats:', err);
    }
  };

  // Perform search on query / type change with debounce
  useEffect(() => {
    if (!query.trim()) {
      setSummary(null);
      setLoading(false);
      return;
    }

    const timer = setTimeout(async () => {
      setLoading(true);
      try {
        const res = await searchApi.search(query.trim(), selectedType, 50);
        setSummary(res);
        // Sync URL query param
        setSearchParams({ q: query.trim() }, { replace: true });
      } catch (err) {
        console.error('Error executing search:', err);
      } finally {
        setLoading(false);
      }
    }, 250);

    return () => clearTimeout(timer);
  }, [query, selectedType, setSearchParams]);

  const handleReindex = async () => {
    setReindexing(true);
    try {
      const res = await searchApi.reindex();
      setSnackbarMessage(`Re-indexing completed: ${res.totalIndexed} records indexed.`);
      await loadStats();
      if (query.trim()) {
        const freshSearch = await searchApi.search(query.trim(), selectedType, 50);
        setSummary(freshSearch);
      }
    } catch (err) {
      setSnackbarMessage('Failed to reindex search engine.');
    } finally {
      setReindexing(false);
    }
  };

  const handleOpenItem = (item: SearchResultItem) => {
    if (item.actionUrl) {
      navigate(item.actionUrl);
    }
  };

  return (
    <Box sx={{ maxWidth: 1200, mx: 'auto', p: 3 }}>
      {/* Header */}
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', mb: 3 }}>
        <Box>
          <Typography variant="h4" fontWeight={700} gutterBottom>
            Global Search Center
          </Typography>
          <Typography variant="body1" color="text.secondary">
            Unified full-text search with BM25 relevance ranking across all your local career records.
          </Typography>
        </Box>
        <Button
          variant="outlined"
          color="primary"
          startIcon={reindexing ? <CircularProgress size={18} /> : <RefreshIcon />}
          onClick={handleReindex}
          disabled={reindexing}
          sx={{ textTransform: 'none', borderRadius: 2 }}
        >
          {reindexing ? 'Re-indexing SQLite FTS5...' : 'Rebuild Search Index'}
        </Button>
      </Box>

      {/* Stats Summary Bar */}
      {stats && (
        <Paper
          variant="outlined"
          sx={{
            p: 2,
            mb: 3,
            backgroundColor: 'background.paper',
            borderRadius: 2,
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'space-between',
            flexWrap: 'wrap',
            gap: 1.5,
          }}
        >
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
            <StorageIcon color="action" fontSize="small" />
            <Typography variant="body2" fontWeight={600}>
              SQLite FTS5 Catalog: {stats.totalIndexedRecords} Total Indexed Records
            </Typography>
          </Box>
          <Box sx={{ display: 'flex', gap: 1, flexWrap: 'wrap' }}>
            {Object.entries(stats.recordsByType).map(([type, count]) => {
              const colorInfo = getEntityColor(type);
              return (
                <Chip
                  key={type}
                  icon={getEntityIcon(type)}
                  label={`${type}s: ${count}`}
                  size="small"
                  sx={{
                    backgroundColor: colorInfo.bg,
                    color: colorInfo.text,
                    fontWeight: 500,
                  }}
                />
              );
            })}
          </Box>
        </Paper>
      )}

      {/* Main Search Bar */}
      <Paper elevation={2} sx={{ p: 1, borderRadius: 3, mb: 3 }}>
        <TextField
          fullWidth
          variant="standard"
          placeholder="Search jobs, companies, applications, contacts, vault files, wiki stories, notes..."
          value={query}
          onChange={(e) => setQuery(e.target.value)}
          InputProps={{
            disableUnderline: true,
            startAdornment: (
              <InputAdornment position="start" sx={{ pl: 1.5 }}>
                <SearchIcon color="primary" sx={{ fontSize: 32 }} />
              </InputAdornment>
            ),
            endAdornment: (
              <InputAdornment position="end" sx={{ pr: 1 }}>
                {loading && <CircularProgress size={24} sx={{ mr: 1 }} />}
                {query && (
                  <IconButton size="small" onClick={() => setQuery('')}>
                    <CloseIcon />
                  </IconButton>
                )}
              </InputAdornment>
            ),
            sx: { fontSize: '1.25rem', py: 1 },
          }}
        />
      </Paper>

      {/* Category Tabs */}
      <Box sx={{ borderBottom: 1, borderColor: 'divider', mb: 3 }}>
        <Tabs
          value={selectedType}
          onChange={(_, newVal) => setSelectedType(newVal)}
          variant="scrollable"
          scrollButtons="auto"
        >
          {CATEGORIES.map((cat) => {
            const count =
              cat === 'All'
                ? summary?.totalMatches ?? 0
                : summary?.matchesByType[cat] ?? 0;

            return (
              <Tab
                key={cat}
                value={cat}
                label={`${cat} ${summary ? `(${count})` : ''}`}
                sx={{ textTransform: 'none', fontWeight: selectedType === cat ? 700 : 500 }}
              />
            );
          })}
        </Tabs>
      </Box>

      {/* Results Header Info */}
      {summary && (
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
          <Typography variant="body2" color="text.secondary">
            Showing <strong>{summary.results.length}</strong> matches for "
            <strong>{summary.query}</strong>"
          </Typography>
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5, color: 'text.secondary' }}>
            <SpeedIcon fontSize="small" />
            <Typography variant="caption">{summary.elapsedMilliseconds}ms</Typography>
          </Box>
        </Box>
      )}

      {/* Results Grid / List */}
      {summary && summary.results.length > 0 ? (
        <Grid container spacing={2}>
          {summary.results.map((item, index) => {
            const colorInfo = getEntityColor(item.entityType);

            return (
              <Grid item xs={12} key={`${item.entityType}-${item.entityId}-${index}`}>
                <Card
                  variant="outlined"
                  sx={{
                    borderRadius: 2,
                    transition: 'all 0.2s',
                    '&:hover': {
                      boxShadow: '0 4px 12px rgba(0,0,0,0.08)',
                      borderColor: 'primary.light',
                    },
                  }}
                >
                  <CardContent sx={{ pb: '16px !important' }}>
                    <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
                      <Box sx={{ display: 'flex', gap: 1.5, alignItems: 'flex-start' }}>
                        <Box sx={{ mt: 0.5 }}>{getEntityIcon(item.entityType)}</Box>
                        <Box>
                          <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, flexWrap: 'wrap' }}>
                            <Typography
                              variant="h6"
                              component="div"
                              sx={{
                                fontWeight: 600,
                                cursor: 'pointer',
                                color: 'primary.main',
                                '&:hover': { textDecoration: 'underline' },
                              }}
                              onClick={() => handleOpenItem(item)}
                            >
                              {item.title}
                            </Typography>
                            <Chip
                              label={item.entityType}
                              size="small"
                              sx={{
                                height: 20,
                                fontSize: '0.7rem',
                                fontWeight: 600,
                                backgroundColor: colorInfo.bg,
                                color: colorInfo.text,
                              }}
                            />
                            {item.score !== 0 && (
                              <Chip
                                label={`BM25: ${item.score}`}
                                size="small"
                                variant="outlined"
                                sx={{ height: 18, fontSize: '0.65rem' }}
                              />
                            )}
                          </Box>

                          <Typography variant="body2" color="text.secondary" sx={{ mt: 0.3 }}>
                            {item.subtitle}
                          </Typography>

                          {/* Highlighted Snippet */}
                          {item.snippet && (
                            <Typography
                              variant="body2"
                              sx={{
                                mt: 1,
                                color: 'text.primary',
                                backgroundColor: 'grey.50',
                                p: 1,
                                borderRadius: 1,
                                border: '1px solid',
                                borderColor: 'grey.200',
                                '& mark': {
                                  backgroundColor: '#fef08a',
                                  color: '#854d0e',
                                  px: 0.5,
                                  py: 0.1,
                                  borderRadius: 0.5,
                                  fontWeight: 600,
                                },
                              }}
                              dangerouslySetInnerHTML={{ __html: item.snippet }}
                            />
                          )}

                          {/* Tags */}
                          {item.tags && (
                            <Box sx={{ display: 'flex', gap: 0.5, mt: 1.2, flexWrap: 'wrap' }}>
                              {item.tags.split(',').map((tag, tIdx) => (
                                <Chip
                                  key={tIdx}
                                  label={tag.trim()}
                                  size="small"
                                  sx={{ height: 18, fontSize: '0.65rem' }}
                                />
                              ))}
                            </Box>
                          )}
                        </Box>
                      </Box>

                      {/* Action Button */}
                      <Button
                        variant="outlined"
                        size="small"
                        endIcon={<OpenInNewIcon fontSize="small" />}
                        onClick={() => handleOpenItem(item)}
                        sx={{ textTransform: 'none', ml: 2, flexShrink: 0 }}
                      >
                        Open
                      </Button>
                    </Box>
                  </CardContent>
                </Card>
              </Grid>
            );
          })}
        </Grid>
      ) : summary && summary.results.length === 0 && !loading ? (
        <Paper variant="outlined" sx={{ p: 6, textAlign: 'center', borderRadius: 2 }}>
          <SearchIcon sx={{ fontSize: 48, color: 'text.disabled', mb: 1 }} />
          <Typography variant="h6" color="text.secondary" gutterBottom>
            No matches found for "{summary.query}"
          </Typography>
          <Typography variant="body2" color="text.secondary">
            Try broadening your search term or select "All" categories. You can also re-index the catalog if data was recently modified.
          </Typography>
        </Paper>
      ) : !query.trim() ? (
        <Paper variant="outlined" sx={{ p: 6, textAlign: 'center', borderRadius: 2 }}>
          <SearchIcon sx={{ fontSize: 56, color: 'primary.light', mb: 2 }} />
          <Typography variant="h5" fontWeight={600} gutterBottom>
            Search Across Everything in CareerOS
          </Typography>
          <Typography variant="body1" color="text.secondary" sx={{ maxWidth: 600, mx: 'auto' }}>
            Find job postings, tracked applications, recruiters and hiring managers, document vault records, STAR stories, and interview preparation notes in milliseconds.
          </Typography>
        </Paper>
      ) : null}

      {/* Snackbar Notifications */}
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
