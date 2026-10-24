import React, { useEffect, useState } from 'react'
import {
  Box, Typography, Paper, IconButton, Dialog, DialogTitle, DialogContent,
  DialogActions, Button, TextField, CircularProgress, Alert, Grid, Card,
  CardContent, Chip, Tooltip, InputAdornment, FormControl, InputLabel,
  Select, MenuItem, Divider, Stack, List, ListItem, ListItemText, ListItemButton,
  ListItemIcon, Switch, FormControlLabel
} from '@mui/material'
import AddIcon from '@mui/icons-material/Add'
import DeleteIcon from '@mui/icons-material/Delete'
import EditIcon from '@mui/icons-material/Edit'
import SearchIcon from '@mui/icons-material/Search'
import StarIcon from '@mui/icons-material/Star'
import StarBorderIcon from '@mui/icons-material/StarBorder'
import PushPinIcon from '@mui/icons-material/PushPin'
import MenuBookIcon from '@mui/icons-material/MenuBook'
import LinkIcon from '@mui/icons-material/Link'
import LabelIcon from '@mui/icons-material/Label'
import VisibilityIcon from '@mui/icons-material/Visibility'
import AutoStoriesIcon from '@mui/icons-material/AutoStories'
import ArchitectureIcon from '@mui/icons-material/Architecture'
import BusinessIcon from '@mui/icons-material/Business'
import PsychologyIcon from '@mui/icons-material/Psychology'
import CodeIcon from '@mui/icons-material/Code'
import TrendingUpIcon from '@mui/icons-material/TrendingUp'
import ArticleIcon from '@mui/icons-material/Article'
import BookmarkIcon from '@mui/icons-material/Bookmark'

import {
  wikiApi, WikiCategory, WikiCategoryLabels
} from '../api/wiki'
import type {
  WikiArticle, WikiStats, CreateWikiArticleRequest, UpdateWikiArticleRequest
} from '../api/wiki'
import { companiesApi } from '../api/companies'
import type { Company } from '../api/companies'
import { jobsApi } from '../api/jobs'
import type { Job } from '../api/jobs'

// Category Icons and Color Mapping
const CATEGORY_CONFIG: Record<number, { label: string; icon: React.ReactElement; color: 'primary' | 'secondary' | 'info' | 'success' | 'warning' | 'default' }> = {
  [WikiCategory.General]: { label: 'General', icon: <ArticleIcon fontSize="small" />, color: 'default' },
  [WikiCategory.StarStory]: { label: 'STAR Story', icon: <PsychologyIcon fontSize="small" />, color: 'warning' },
  [WikiCategory.SystemDesign]: { label: 'System Design', icon: <ArchitectureIcon fontSize="small" />, color: 'secondary' },
  [WikiCategory.CompanyResearch]: { label: 'Company Research', icon: <BusinessIcon fontSize="small" />, color: 'info' },
  [WikiCategory.InterviewNotes]: { label: 'Interview Notes', icon: <MenuBookIcon fontSize="small" />, color: 'primary' },
  [WikiCategory.CodingPatterns]: { label: 'Coding Patterns', icon: <CodeIcon fontSize="small" />, color: 'success' },
  [WikiCategory.CareerStrategy]: { label: 'Career Strategy', icon: <TrendingUpIcon fontSize="small" />, color: 'secondary' },
}

// Built-in Templates
const TEMPLATES = {
  starStory: `# STAR Story: [Project / Initiative Title]

## Situation
Describe the context, company, business problem, and technical challenges:
- Team size and your position:
- Problem impact on users or business metrics:

## Task
What was your specific ownership and deliverable?
- Core objective:
- Constraints & timeline:

## Action
Detailed architectural and engineering steps taken:
1. Architectural decision making (tradeoffs considered):
2. Implementation details and technologies applied:
3. Obstacles overcome and stakeholder alignment:

## Result
Quantifiable business and engineering outcomes:
- Performance improvements (e.g. latency reduced by 40%, throughput 3x):
- Business impact:
- Key retrospection and lessons learned:
`,
  systemDesign: `# System Design: [Distributed Architecture Title]

## 1. Requirements
### Functional
- Core user action 1
- Core user action 2

### Non-Functional
- High availability (99.99%)
- Low latency (< 50ms p99)
- Scalability: 10M DAU, 50k write QPS

## 2. Capacity & Estimation
- Storage per year:
- Bandwidth throughput:

## 3. High-Level Architecture
\`\`\`
[Client / Mobile] ---> [CDN / Edge] ---> [API Gateway]
                                              |
                   +--------------------------+--------------------------+
                   |                                                     |
             [Auth Service]                                      [Core Service]
                   |                                                     |
             [Redis Cache]                                      [Read Replica DB]
                                                                         ^
                                                                         | (Replication)
                                                                [Primary DB Master]
\`\`\`

## 4. Deep Dives & Bottlenecks
- Consistency Model: Eventual consistency with Saga pattern.
- Caching Strategy: Cache-aside with Redis cluster.
- Partitioning: Consistent hashing on userId.
- Disaster Recovery: Multi-region active-passive failover.
`,
  companyResearch: `# Company Research: [Company Name]

## Core Business & Products
- Primary revenue engines:
- Target markets and competitive advantage (Moat):

## Engineering Culture & Tech Stack
- Frontend:
- Backend:
- Cloud & Infrastructure:
- Core principles (e.g. Amazon Leadership Principles, Google Tenets):

## Strategic Initiatives & News
- Recent product launches:
- Strategic company goals:

## Smart Questions for the Interviewers
1. What does high performance look like on this team within the first 6 months?
2. What is the most critical technical debt or architecture transition on your roadmap?
`,
  interviewNotes: `# Interview Notes: [Company & Round]

## Context
- Date:
- Interviewers:
- Round Type:

## Questions Asked & Prompts
1. Question 1:
   - Key considerations:
2. Question 2:
   - Solution proposed:

## What Went Well
-

## Growth Areas & Follow-ups
- Check out related concept: [[system-design]]
`
}

export const WikiPage: React.FC = () => {
  const [articles, setArticles] = useState<WikiArticle[]>([])
  const [selectedArticle, setSelectedArticle] = useState<WikiArticle | null>(null)
  const [stats, setStats] = useState<WikiStats | null>(null)
  const [tags, setTags] = useState<string[]>([])
  const [companies, setCompanies] = useState<Company[]>([])
  const [jobs, setJobs] = useState<Job[]>([])

  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [successMessage, setSuccessMessage] = useState<string | null>(null)

  // Filters
  const [searchTerm, setSearchTerm] = useState('')
  const [selectedCategory, setSelectedCategory] = useState<number | ''>('')
  const [selectedTag, setSelectedTag] = useState<string>('')
  const [starredOnly, setStarredOnly] = useState(false)

  // Dialog State
  const [openDialog, setOpenDialog] = useState(false)
  const [editingArticle, setEditingArticle] = useState<WikiArticle | null>(null)

  // Form State
  const [formTitle, setFormTitle] = useState('')
  const [formSlug, setFormSlug] = useState('')
  const [formCategory, setFormCategory] = useState<number>(WikiCategory.General)
  const [formContent, setFormContent] = useState('')
  const [formSummary, setFormSummary] = useState('')
  const [formTags, setFormTags] = useState('')
  const [formCompanyId, setFormCompanyId] = useState<string>('')
  const [formJobId, setFormJobId] = useState<string>('')
  const [formIsStarred, setFormIsStarred] = useState(false)
  const [formIsPinned, setFormIsPinned] = useState(false)

  const loadData = async (activeSlugToSelect?: string) => {
    try {
      setLoading(true)
      const [arts, st, tg, comps, jbs] = await Promise.all([
        wikiApi.getAll({
          searchTerm: searchTerm || undefined,
          category: selectedCategory !== '' ? Number(selectedCategory) : undefined,
          tag: selectedTag || undefined,
          isStarred: starredOnly ? true : undefined,
        }),
        wikiApi.getStats(),
        wikiApi.getTags(),
        companiesApi.getAll(),
        jobsApi.getAll(),
      ])

      setArticles(arts)
      setStats(st)
      setTags(tg)
      setCompanies(comps)
      setJobs(jbs)

      if (activeSlugToSelect) {
        const found = arts.find((a) => a.slug === activeSlugToSelect)
        if (found) {
          const detail = await wikiApi.getById(found.id, true)
          setSelectedArticle(detail)
        }
      } else if (arts.length > 0 && !selectedArticle) {
        const detail = await wikiApi.getById(arts[0].id, true)
        setSelectedArticle(detail)
      } else if (selectedArticle) {
        const refreshed = arts.find((a) => a.id === selectedArticle.id)
        if (refreshed) {
          const detail = await wikiApi.getById(refreshed.id, false)
          setSelectedArticle(detail)
        } else if (arts.length > 0) {
          setSelectedArticle(arts[0])
        } else {
          setSelectedArticle(null)
        }
      }
    } catch (err: unknown) {
      const msg = err instanceof Error ? err.message : 'Failed to load wiki data'
      setError(msg)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    loadData()
  }, [searchTerm, selectedCategory, selectedTag, starredOnly])

  const handleSelectArticle = async (article: WikiArticle) => {
    try {
      const detail = await wikiApi.getById(article.id, true)
      setSelectedArticle(detail)
    } catch {
      setSelectedArticle(article)
    }
  }

  const handleNavigateSlug = async (slug: string) => {
    try {
      const detail = await wikiApi.getBySlug(slug, true)
      setSelectedArticle(detail)
    } catch {
      // If article does not exist yet, prompt to create it
      handleOpenCreateWithSlug(slug)
    }
  }

  const handleToggleStar = async (id: string, e?: React.MouseEvent) => {
    if (e) e.stopPropagation()
    try {
      const updated = await wikiApi.toggleStar(id)
      setArticles((prev) => prev.map((a) => (a.id === id ? updated : a)))
      if (selectedArticle?.id === id) {
        setSelectedArticle(updated)
      }
      const st = await wikiApi.getStats()
      setStats(st)
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Failed to toggle star')
    }
  }

  const handleDelete = async (id: string) => {
    if (!window.confirm('Are you sure you want to delete this wiki article?')) return
    try {
      await wikiApi.delete(id)
      setSuccessMessage('Wiki article removed successfully.')
      if (selectedArticle?.id === id) {
        setSelectedArticle(null)
      }
      loadData()
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Failed to delete wiki article')
    }
  }

  const handleOpenCreate = () => {
    setEditingArticle(null)
    setFormTitle('')
    setFormSlug('')
    setFormCategory(WikiCategory.General)
    setFormContent('')
    setFormSummary('')
    setFormTags('')
    setFormCompanyId('')
    setFormJobId('')
    setFormIsStarred(false)
    setFormIsPinned(false)
    setOpenDialog(true)
  }

  const handleOpenCreateWithSlug = (slug: string) => {
    setEditingArticle(null)
    const formattedTitle = slug
      .split('-')
      .map((w) => w.charAt(0).toUpperCase() + w.slice(1))
      .join(' ')
    setFormTitle(formattedTitle)
    setFormSlug(slug)
    setFormCategory(WikiCategory.General)
    setFormContent(`# ${formattedTitle}\n\nStart documenting this topic...`)
    setFormSummary('')
    setFormTags('')
    setFormCompanyId('')
    setFormJobId('')
    setFormIsStarred(false)
    setFormIsPinned(false)
    setOpenDialog(true)
  }

  const handleOpenEdit = (article: WikiArticle) => {
    setEditingArticle(article)
    setFormTitle(article.title)
    setFormSlug(article.slug)
    setFormCategory(article.category)
    setFormContent(article.content)
    setFormSummary(article.summary || '')
    setFormTags(article.tags || '')
    setFormCompanyId(article.companyId || '')
    setFormJobId(article.jobId || '')
    setFormIsStarred(article.isStarred)
    setFormIsPinned(article.isPinned)
    setOpenDialog(true)
  }

  const handleApplyTemplate = (templateKey: keyof typeof TEMPLATES, category: number) => {
    setFormContent(TEMPLATES[templateKey])
    setFormCategory(category)
  }

  const handleSubmit = async () => {
    if (!formTitle.trim()) {
      setError('Title is required')
      return
    }

    try {
      if (editingArticle) {
        const updateReq: UpdateWikiArticleRequest = {
          id: editingArticle.id,
          title: formTitle,
          slug: formSlug || undefined,
          category: formCategory,
          content: formContent,
          summary: formSummary || undefined,
          tags: formTags || undefined,
          companyId: formCompanyId || undefined,
          jobId: formJobId || undefined,
          isStarred: formIsStarred,
          isPinned: formIsPinned,
        }
        const updated = await wikiApi.update(editingArticle.id, updateReq)
        setSuccessMessage('Article updated successfully!')
        setOpenDialog(false)
        await loadData(updated.slug)
      } else {
        const createReq: CreateWikiArticleRequest = {
          title: formTitle,
          slug: formSlug || undefined,
          category: formCategory,
          content: formContent,
          summary: formSummary || undefined,
          tags: formTags || undefined,
          companyId: formCompanyId || undefined,
          jobId: formJobId || undefined,
          isStarred: formIsStarred,
          isPinned: formIsPinned,
        }
        const created = await wikiApi.create(createReq)
        setSuccessMessage('Article created successfully!')
        setOpenDialog(false)
        await loadData(created.slug)
      }
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Failed to save wiki article')
    }
  }

  // Simple Markdown & [[Wiki-Link]] parser for preview rendering
  const renderMarkdownContent = (rawText: string) => {
    if (!rawText) return null

    // Split text into paragraphs / blocks
    const lines = rawText.split('\n')
    const elements: React.ReactNode[] = []
    let inCodeBlock = false
    let codeBuffer: string[] = []

    lines.forEach((line, idx) => {
      if (line.trim().startsWith('```')) {
        if (inCodeBlock) {
          // end code block
          elements.push(
            <Box
              key={`code-${idx}`}
              component="pre"
              sx={{
                bgcolor: 'grey.900',
                color: 'grey.100',
                p: 2,
                borderRadius: 1.5,
                overflowX: 'auto',
                fontFamily: 'Consolas, monospace',
                fontSize: '0.875rem',
                my: 1.5,
              }}
            >
              <code>{codeBuffer.join('\n')}</code>
            </Box>
          )
          codeBuffer = []
          inCodeBlock = false
        } else {
          inCodeBlock = true
        }
        return
      }

      if (inCodeBlock) {
        codeBuffer.push(line)
        return
      }

      // Headings
      if (line.startsWith('# ')) {
        elements.push(
          <Typography key={idx} variant="h4" fontWeight="bold" sx={{ mt: 3, mb: 1.5, color: 'primary.main' }}>
            {parseWikiInlineLinks(line.substring(2))}
          </Typography>
        )
        return
      }
      if (line.startsWith('## ')) {
        elements.push(
          <Typography key={idx} variant="h5" fontWeight="bold" sx={{ mt: 2.5, mb: 1, borderBottom: '1px solid', borderColor: 'divider', pb: 0.5 }}>
            {parseWikiInlineLinks(line.substring(3))}
          </Typography>
        )
        return
      }
      if (line.startsWith('### ')) {
        elements.push(
          <Typography key={idx} variant="h6" fontWeight="bold" sx={{ mt: 2, mb: 0.5 }}>
            {parseWikiInlineLinks(line.substring(4))}
          </Typography>
        )
        return
      }

      // Blockquote
      if (line.startsWith('> ')) {
        elements.push(
          <Box
            key={idx}
            sx={{
              borderLeft: '4px solid',
              borderColor: 'primary.main',
              pl: 2,
              py: 0.5,
              my: 1,
              bgcolor: 'action.hover',
              fontStyle: 'italic',
            }}
          >
            <Typography variant="body2">{parseWikiInlineLinks(line.substring(2))}</Typography>
          </Box>
        )
        return
      }

      // Bullet lists
      if (line.trim().startsWith('- ') || line.trim().startsWith('* ')) {
        elements.push(
          <Box key={idx} sx={{ display: 'flex', alignItems: 'flex-start', ml: 2, my: 0.25 }}>
            <Typography variant="body2" sx={{ mr: 1, color: 'text.secondary' }}>•</Typography>
            <Typography variant="body2">{parseWikiInlineLinks(line.trim().substring(2))}</Typography>
          </Box>
        )
        return
      }

      // Numbered lists
      const numberedMatch = line.trim().match(/^(\d+)\.\s+(.*)$/)
      if (numberedMatch) {
        elements.push(
          <Box key={idx} sx={{ display: 'flex', alignItems: 'flex-start', ml: 2, my: 0.25 }}>
            <Typography variant="body2" sx={{ mr: 1, fontWeight: 'bold', color: 'text.secondary' }}>
              {numberedMatch[1]}.
            </Typography>
            <Typography variant="body2">{parseWikiInlineLinks(numberedMatch[2])}</Typography>
          </Box>
        )
        return
      }

      // Empty line spacer
      if (!line.trim()) {
        elements.push(<Box key={idx} sx={{ height: 10 }} />)
        return
      }

      // Standard paragraph
      elements.push(
        <Typography key={idx} variant="body1" sx={{ my: 0.5, lineHeight: 1.7 }}>
          {parseWikiInlineLinks(line)}
        </Typography>
      )
    })

    return elements
  }

  // Parse inline [[slug|title]] or [[slug]] links inside text
  const parseWikiInlineLinks = (text: string): React.ReactNode => {
    const linkRegex = /\[\[(.*?)\]\]/g
    const parts: React.ReactNode[] = []
    let lastIndex = 0
    let match: RegExpExecArray | null

    while ((match = linkRegex.exec(text)) !== null) {
      if (match.index > lastIndex) {
        parts.push(text.substring(lastIndex, match.index))
      }

      const raw = match[1]
      let slug = raw
      let label = raw

      if (raw.includes('|')) {
        const split = raw.split('|')
        slug = split[0].trim().toLowerCase().replace(/[^a-z0-9\s-]/g, '').replace(/\s+/g, '-')
        label = split[1].trim()
      } else {
        slug = raw.trim().toLowerCase().replace(/[^a-z0-9\s-]/g, '').replace(/\s+/g, '-')
      }

      parts.push(
        <Chip
          key={`${match.index}-${slug}`}
          icon={<LinkIcon fontSize="small" />}
          label={label}
          size="small"
          color="primary"
          variant="outlined"
          clickable
          onClick={() => handleNavigateSlug(slug)}
          sx={{ mx: 0.5, cursor: 'pointer', verticalAlign: 'middle', fontWeight: 600 }}
        />
      )

      lastIndex = linkRegex.lastIndex
    }

    if (lastIndex < text.length) {
      parts.push(text.substring(lastIndex))
    }

    return parts.length > 0 ? parts : text
  }

  return (
    <Box sx={{ p: 3, maxWidth: 1600, margin: '0 auto' }}>
      {/* Header and Actions */}
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Box>
          <Typography variant="h4" fontWeight="bold" sx={{ display: 'flex', alignItems: 'center', gap: 1.5 }}>
            <MenuBookIcon fontSize="large" color="primary" />
            Knowledge Base
          </Typography>
          <Typography variant="subtitle1" color="text.secondary">
            Local-first career wiki with Markdown, STAR stories, architecture guides, and bi-directional backlinks.
          </Typography>
        </Box>
        <Stack direction="row" spacing={1.5}>
          <Button
            variant="contained"
            color="primary"
            startIcon={<AddIcon />}
            onClick={handleOpenCreate}
            sx={{ px: 2.5, py: 1 }}
          >
            New Article
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

      {/* KPI Stats Cards */}
      {stats && (
        <Grid container spacing={2} sx={{ mb: 3 }}>
          <Grid item xs={6} sm={4} md={2}>
            <Card sx={{ bgcolor: 'background.paper', border: '1px solid', borderColor: 'divider' }}>
              <CardContent sx={{ py: 1.5, px: 2, '&:last-child': { pb: 1.5 } }}>
                <Typography variant="caption" color="text.secondary" fontWeight="bold">
                  TOTAL ARTICLES
                </Typography>
                <Typography variant="h5" fontWeight="bold">
                  {stats.totalArticles}
                </Typography>
              </CardContent>
            </Card>
          </Grid>

          <Grid item xs={6} sm={4} md={2}>
            <Card sx={{ bgcolor: 'warning.light', color: 'warning.contrastText' }}>
              <CardContent sx={{ py: 1.5, px: 2, '&:last-child': { pb: 1.5 } }}>
                <Typography variant="caption" fontWeight="bold">
                  STAR STORIES
                </Typography>
                <Typography variant="h5" fontWeight="bold">
                  {stats.starStoriesCount}
                </Typography>
              </CardContent>
            </Card>
          </Grid>

          <Grid item xs={6} sm={4} md={2}>
            <Card sx={{ bgcolor: 'secondary.light', color: 'secondary.contrastText' }}>
              <CardContent sx={{ py: 1.5, px: 2, '&:last-child': { pb: 1.5 } }}>
                <Typography variant="caption" fontWeight="bold">
                  SYSTEM DESIGN
                </Typography>
                <Typography variant="h5" fontWeight="bold">
                  {stats.systemDesignCount}
                </Typography>
              </CardContent>
            </Card>
          </Grid>

          <Grid item xs={6} sm={4} md={2}>
            <Card sx={{ bgcolor: 'info.light', color: 'info.contrastText' }}>
              <CardContent sx={{ py: 1.5, px: 2, '&:last-child': { pb: 1.5 } }}>
                <Typography variant="caption" fontWeight="bold">
                  COMPANY RESEARCH
                </Typography>
                <Typography variant="h5" fontWeight="bold">
                  {stats.companyResearchCount}
                </Typography>
              </CardContent>
            </Card>
          </Grid>

          <Grid item xs={6} sm={4} md={2}>
            <Card sx={{ bgcolor: 'primary.light', color: 'primary.contrastText' }}>
              <CardContent sx={{ py: 1.5, px: 2, '&:last-child': { pb: 1.5 } }}>
                <Typography variant="caption" fontWeight="bold">
                  INTERVIEW NOTES
                </Typography>
                <Typography variant="h5" fontWeight="bold">
                  {stats.interviewNotesCount}
                </Typography>
              </CardContent>
            </Card>
          </Grid>

          <Grid item xs={6} sm={4} md={2}>
            <Card sx={{ bgcolor: 'background.paper', border: '1px solid', borderColor: 'divider' }}>
              <CardContent sx={{ py: 1.5, px: 2, '&:last-child': { pb: 1.5 } }}>
                <Typography variant="caption" color="text.secondary" fontWeight="bold">
                  FAVORITES / TAGS
                </Typography>
                <Typography variant="h5" fontWeight="bold">
                  ★ {stats.starredCount} / #{stats.tagsCount}
                </Typography>
              </CardContent>
            </Card>
          </Grid>
        </Grid>
      )}

      {/* Main Split Layout: Left Explorer (30%), Right Viewer (70%) */}
      <Grid container spacing={3}>
        {/* Left Side: Search, Filter, Articles Explorer */}
        <Grid item xs={12} md={4} lg={3.5}>
          <Paper sx={{ p: 2, border: '1px solid', borderColor: 'divider', mb: 2 }}>
            <TextField
              fullWidth
              size="small"
              placeholder="Search articles, notes, tags..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              InputProps={{
                startAdornment: (
                  <InputAdornment position="start">
                    <SearchIcon fontSize="small" />
                  </InputAdornment>
                ),
              }}
              sx={{ mb: 1.5 }}
            />

            {/* Category Selector */}
            <FormControl fullWidth size="small" sx={{ mb: 1.5 }}>
              <InputLabel>Category Filter</InputLabel>
              <Select
                value={selectedCategory}
                label="Category Filter"
                onChange={(e) => setSelectedCategory(e.target.value as number | '')}
              >
                <MenuItem value="">All Categories</MenuItem>
                {Object.entries(WikiCategoryLabels).map(([key, label]) => (
                  <MenuItem key={key} value={Number(key)}>
                    {label}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>

            {/* Starred Only Toggle */}
            <FormControlLabel
              control={
                <Switch
                  checked={starredOnly}
                  onChange={(e) => setStarredOnly(e.target.checked)}
                  color="warning"
                  size="small"
                />
              }
              label={
                <Typography variant="body2" sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
                  <StarIcon fontSize="small" color="warning" /> Starred Only
                </Typography>
              }
              sx={{ mb: 1 }}
            />

            {/* Tag Cloud */}
            {tags.length > 0 && (
              <Box sx={{ mt: 1.5 }}>
                <Typography variant="caption" color="text.secondary" fontWeight="bold" sx={{ display: 'block', mb: 0.5 }}>
                  TAGS
                </Typography>
                <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 0.5 }}>
                  <Chip
                    label="All Tags"
                    size="small"
                    variant={selectedTag === '' ? 'filled' : 'outlined'}
                    color={selectedTag === '' ? 'primary' : 'default'}
                    onClick={() => setSelectedTag('')}
                    sx={{ cursor: 'pointer' }}
                  />
                  {tags.map((t) => (
                    <Chip
                      key={t}
                      label={`#${t}`}
                      size="small"
                      variant={selectedTag === t ? 'filled' : 'outlined'}
                      color={selectedTag === t ? 'primary' : 'default'}
                      onClick={() => setSelectedTag(selectedTag === t ? '' : t)}
                      sx={{ cursor: 'pointer' }}
                    />
                  ))}
                </Box>
              </Box>
            )}
          </Paper>

          {/* Article List */}
          <Paper sx={{ border: '1px solid', borderColor: 'divider', maxHeight: '600px', overflowY: 'auto' }}>
            {loading && articles.length === 0 ? (
              <Box sx={{ display: 'flex', justifyContent: 'center', p: 4 }}>
                <CircularProgress size={32} />
              </Box>
            ) : articles.length === 0 ? (
              <Box sx={{ p: 4, textAlign: 'center' }}>
                <MenuBookIcon sx={{ fontSize: 40, color: 'text.secondary', mb: 1 }} />
                <Typography variant="body2" color="text.secondary">
                  No articles found matching filters.
                </Typography>
              </Box>
            ) : (
              <List disablePadding>
                {articles.map((art, idx) => {
                  const cfg = CATEGORY_CONFIG[art.category] || CATEGORY_CONFIG[WikiCategory.General]
                  const isSelected = selectedArticle?.id === art.id
                  return (
                    <React.Fragment key={art.id}>
                      {idx > 0 && <Divider />}
                      <ListItemButton
                        selected={isSelected}
                        onClick={() => handleSelectArticle(art)}
                        sx={{
                          py: 1.5,
                          px: 2,
                          bgcolor: isSelected ? 'action.selected' : 'inherit',
                          borderLeft: isSelected ? '4px solid' : '4px solid transparent',
                          borderColor: isSelected ? 'primary.main' : 'transparent',
                        }}
                      >
                        <ListItemIcon sx={{ minWidth: 32 }}>
                          {art.isPinned ? (
                            <PushPinIcon fontSize="small" color="primary" />
                          ) : (
                            cfg.icon
                          )}
                        </ListItemIcon>
                        <ListItemText
                          primary={
                            <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                              <Typography
                                variant="subtitle2"
                                fontWeight={isSelected || art.isPinned ? 700 : 500}
                                noWrap
                                sx={{ maxWidth: '180px' }}
                              >
                                {art.title}
                              </Typography>
                              <IconButton
                                size="small"
                                onClick={(e) => handleToggleStar(art.id, e)}
                                sx={{ p: 0.25 }}
                              >
                                {art.isStarred ? (
                                  <StarIcon fontSize="small" sx={{ color: 'warning.main' }} />
                                ) : (
                                  <StarBorderIcon fontSize="small" sx={{ color: 'text.disabled' }} />
                                )}
                              </IconButton>
                            </Box>
                          }
                          secondary={
                            <Box sx={{ mt: 0.5 }}>
                              <Stack direction="row" spacing={0.5} alignItems="center" sx={{ mb: 0.5 }}>
                                <Chip
                                  label={cfg.label}
                                  size="small"
                                  color={cfg.color}
                                  sx={{ height: 18, fontSize: '0.65rem' }}
                                />
                                {art.backlinks && art.backlinks.length > 0 && (
                                  <Chip
                                    icon={<LinkIcon sx={{ fontSize: '0.75rem !important' }} />}
                                    label={`${art.backlinks.length}`}
                                    size="small"
                                    variant="outlined"
                                    sx={{ height: 18, fontSize: '0.65rem' }}
                                  />
                                )}
                              </Stack>
                              {art.summary && (
                                <Typography variant="caption" color="text.secondary" noWrap sx={{ display: 'block' }}>
                                  {art.summary}
                                </Typography>
                              )}
                            </Box>
                          }
                        />
                      </ListItemButton>
                    </React.Fragment>
                  )
                })}
              </List>
            )}
          </Paper>
        </Grid>

        {/* Right Side: Article Reader / Editor View */}
        <Grid item xs={12} md={8} lg={8.5}>
          {selectedArticle ? (
            <Paper sx={{ p: 3, border: '1px solid', borderColor: 'divider', minHeight: '600px' }}>
              {/* Article Header & Actions Toolbar */}
              <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', mb: 2 }}>
                <Box sx={{ flex: 1 }}>
                  <Stack direction="row" spacing={1} alignItems="center" sx={{ mb: 1 }}>
                    <Chip
                      label={CATEGORY_CONFIG[selectedArticle.category]?.label || 'General'}
                      color={CATEGORY_CONFIG[selectedArticle.category]?.color || 'default'}
                      size="small"
                    />
                    <Chip
                      label={`slug: ${selectedArticle.slug}`}
                      variant="outlined"
                      size="small"
                      sx={{ fontFamily: 'Consolas, monospace', fontSize: '0.75rem' }}
                    />
                    {selectedArticle.companyName && (
                      <Chip
                        icon={<BusinessIcon fontSize="small" />}
                        label={selectedArticle.companyName}
                        size="small"
                        color="info"
                        variant="outlined"
                      />
                    )}
                    {selectedArticle.jobTitle && (
                      <Chip
                        label={selectedArticle.jobTitle}
                        size="small"
                        color="secondary"
                        variant="outlined"
                      />
                    )}
                  </Stack>

                  <Typography variant="h4" fontWeight="bold">
                    {selectedArticle.title}
                  </Typography>

                  {/* Metadata bar */}
                  <Stack direction="row" spacing={2} sx={{ mt: 1, color: 'text.secondary' }}>
                    <Typography variant="caption" sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
                      <VisibilityIcon fontSize="inherit" /> {selectedArticle.viewCount} views
                    </Typography>
                    <Typography variant="caption">
                      Updated: {new Date(selectedArticle.updatedAt).toLocaleDateString()}
                    </Typography>
                  </Stack>
                </Box>

                {/* Header Action Buttons */}
                <Stack direction="row" spacing={1}>
                  <Tooltip title={selectedArticle.isStarred ? 'Unstar Article' : 'Star Article'}>
                    <IconButton onClick={() => handleToggleStar(selectedArticle.id)} color="warning">
                      {selectedArticle.isStarred ? <StarIcon /> : <StarBorderIcon />}
                    </IconButton>
                  </Tooltip>
                  <Tooltip title="Edit Article">
                    <IconButton onClick={() => handleOpenEdit(selectedArticle)} color="primary">
                      <EditIcon />
                    </IconButton>
                  </Tooltip>
                  <Tooltip title="Delete Article">
                    <IconButton onClick={() => handleDelete(selectedArticle.id)} color="error">
                      <DeleteIcon />
                    </IconButton>
                  </Tooltip>
                </Stack>
              </Box>

              {/* Tags List */}
              {selectedArticle.tagsList.length > 0 && (
                <Stack direction="row" spacing={0.75} sx={{ mb: 2 }}>
                  {selectedArticle.tagsList.map((tag) => (
                    <Chip
                      key={tag}
                      icon={<LabelIcon fontSize="small" />}
                      label={tag}
                      size="small"
                      variant="outlined"
                      onClick={() => setSelectedTag(tag)}
                      sx={{ cursor: 'pointer' }}
                    />
                  ))}
                </Stack>
              )}

              {/* Summary callout */}
              {selectedArticle.summary && (
                <Alert severity="info" icon={<AutoStoriesIcon />} sx={{ mb: 3 }}>
                  <Typography variant="subtitle2" fontWeight="bold">Summary</Typography>
                  <Typography variant="body2">{selectedArticle.summary}</Typography>
                </Alert>
              )}

              <Divider sx={{ mb: 3 }} />

              {/* Rendered Markdown Body */}
              <Box sx={{ typography: 'body1', color: 'text.primary', minHeight: 250 }}>
                {renderMarkdownContent(selectedArticle.content)}
              </Box>

              <Divider sx={{ my: 4 }} />

              {/* Bi-Directional Backlink & Outlink Graphs */}
              <Grid container spacing={3}>
                {/* Backlinks (Referenced By) */}
                <Grid item xs={12} sm={6}>
                  <Card variant="outlined" sx={{ bgcolor: 'action.hover' }}>
                    <CardContent sx={{ p: 2, '&:last-child': { pb: 2 } }}>
                      <Typography variant="subtitle2" fontWeight="bold" sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 1 }}>
                        <LinkIcon color="primary" fontSize="small" />
                        Referenced By (Inbound Backlinks)
                      </Typography>
                      {selectedArticle.backlinks.length === 0 ? (
                        <Typography variant="caption" color="text.secondary">
                          No other articles link to this page yet. Use [[{selectedArticle.slug}]] elsewhere to link it.
                        </Typography>
                      ) : (
                        <List dense disablePadding>
                          {selectedArticle.backlinks.map((bl) => (
                            <ListItem
                              key={bl.articleId}
                              disablePadding
                              sx={{ py: 0.5 }}
                            >
                              <ListItemButton
                                onClick={() => handleNavigateSlug(bl.slug)}
                                sx={{ borderRadius: 1, px: 1, py: 0.5 }}
                              >
                                <ListItemIcon sx={{ minWidth: 24 }}>
                                  <ArticleIcon fontSize="small" color="primary" />
                                </ListItemIcon>
                                <ListItemText
                                  primary={bl.title}
                                  secondary={`[[${bl.slug}]] • ${bl.categoryName}`}
                                  primaryTypographyProps={{ variant: 'body2', fontWeight: 600 }}
                                />
                              </ListItemButton>
                            </ListItem>
                          ))}
                        </List>
                      )}
                    </CardContent>
                  </Card>
                </Grid>

                {/* Outlinks (Links To) */}
                <Grid item xs={12} sm={6}>
                  <Card variant="outlined" sx={{ bgcolor: 'action.hover' }}>
                    <CardContent sx={{ p: 2, '&:last-child': { pb: 2 } }}>
                      <Typography variant="subtitle2" fontWeight="bold" sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 1 }}>
                        <LaunchOutlinedIconWrapper />
                        Links To (Outbound References)
                      </Typography>
                      {selectedArticle.outlinks.length === 0 ? (
                        <Typography variant="caption" color="text.secondary">
                          This article contains no [[wiki-links]]. Add [[target-topic]] to connect topics.
                        </Typography>
                      ) : (
                        <List dense disablePadding>
                          {selectedArticle.outlinks.map((ol, idx) => (
                            <ListItem
                              key={`${ol.targetSlug}-${idx}`}
                              disablePadding
                              sx={{ py: 0.5 }}
                            >
                              <ListItemButton
                                onClick={() => handleNavigateSlug(ol.targetSlug)}
                                sx={{ borderRadius: 1, px: 1, py: 0.5 }}
                              >
                                <ListItemIcon sx={{ minWidth: 24 }}>
                                  <LinkIcon fontSize="small" color={ol.targetArticleId ? 'success' : 'action'} />
                                </ListItemIcon>
                                <ListItemText
                                  primary={ol.linkText || ol.targetTitle || ol.targetSlug}
                                  secondary={
                                    ol.targetArticleId
                                      ? `Exists (${ol.targetTitle})`
                                      : 'Target stub — click to create!'
                                  }
                                  primaryTypographyProps={{
                                    variant: 'body2',
                                    fontWeight: 600,
                                    color: ol.targetArticleId ? 'primary.main' : 'warning.dark',
                                  }}
                                />
                              </ListItemButton>
                            </ListItem>
                          ))}
                        </List>
                      )}
                    </CardContent>
                  </Card>
                </Grid>
              </Grid>
            </Paper>
          ) : (
            <Paper sx={{ p: 6, textAlign: 'center', border: '1px solid', borderColor: 'divider' }}>
              <MenuBookIcon sx={{ fontSize: 60, color: 'text.secondary', mb: 2 }} />
              <Typography variant="h6" color="text.secondary">
                Select an article from the explorer or create a new one.
              </Typography>
              <Button
                variant="outlined"
                startIcon={<AddIcon />}
                onClick={handleOpenCreate}
                sx={{ mt: 2 }}
              >
                Create First Article
              </Button>
            </Paper>
          )}
        </Grid>
      </Grid>

      {/* Create / Edit Dialog */}
      <Dialog
        open={openDialog}
        onClose={() => setOpenDialog(false)}
        maxWidth="md"
        fullWidth
      >
        <DialogTitle>
          <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
            <Typography variant="h6" fontWeight="bold">
              {editingArticle ? 'Edit Wiki Article' : 'Create New Wiki Article'}
            </Typography>
            <Stack direction="row" spacing={1}>
              <FormControlLabel
                control={
                  <Switch
                    checked={formIsPinned}
                    onChange={(e) => setFormIsPinned(e.target.checked)}
                    size="small"
                  />
                }
                label="Pin"
              />
              <FormControlLabel
                control={
                  <Switch
                    checked={formIsStarred}
                    onChange={(e) => setFormIsStarred(e.target.checked)}
                    size="small"
                    color="warning"
                  />
                }
                label="Star"
              />
            </Stack>
          </Box>
        </DialogTitle>

        <DialogContent dividers>
          {/* Quick Template Picker */}
          {!editingArticle && (
            <Box sx={{ mb: 2, p: 1.5, bgcolor: 'action.hover', borderRadius: 1.5 }}>
              <Typography variant="caption" fontWeight="bold" color="text.secondary" sx={{ display: 'block', mb: 1 }}>
                QUICK TEMPLATES
              </Typography>
              <Stack direction="row" spacing={1} flexWrap="wrap" useFlexGap>
                <Button
                  size="small"
                  variant="outlined"
                  color="warning"
                  startIcon={<PsychologyIcon />}
                  onClick={() => handleApplyTemplate('starStory', WikiCategory.StarStory)}
                >
                  STAR Story
                </Button>
                <Button
                  size="small"
                  variant="outlined"
                  color="secondary"
                  startIcon={<ArchitectureIcon />}
                  onClick={() => handleApplyTemplate('systemDesign', WikiCategory.SystemDesign)}
                >
                  System Design
                </Button>
                <Button
                  size="small"
                  variant="outlined"
                  color="info"
                  startIcon={<BusinessIcon />}
                  onClick={() => handleApplyTemplate('companyResearch', WikiCategory.CompanyResearch)}
                >
                  Company Research
                </Button>
                <Button
                  size="small"
                  variant="outlined"
                  color="primary"
                  startIcon={<MenuBookIcon />}
                  onClick={() => handleApplyTemplate('interviewNotes', WikiCategory.InterviewNotes)}
                >
                  Interview Notes
                </Button>
              </Stack>
            </Box>
          )}

          <Grid container spacing={2}>
            <Grid item xs={12} sm={8}>
              <TextField
                fullWidth
                label="Article Title"
                required
                value={formTitle}
                onChange={(e) => setFormTitle(e.target.value)}
              />
            </Grid>

            <Grid item xs={12} sm={4}>
              <FormControl fullWidth>
                <InputLabel>Category</InputLabel>
                <Select
                  value={formCategory}
                  label="Category"
                  onChange={(e) => setFormCategory(Number(e.target.value))}
                >
                  {Object.entries(WikiCategoryLabels).map(([key, label]) => (
                    <MenuItem key={key} value={Number(key)}>
                      {label}
                    </MenuItem>
                  ))}
                </Select>
              </FormControl>
            </Grid>

            <Grid item xs={12} sm={6}>
              <TextField
                fullWidth
                label="Slug (optional, auto-generated if blank)"
                value={formSlug}
                onChange={(e) => setFormSlug(e.target.value)}
                helperText="URL-friendly identifier (e.g. distributed-rate-limiting)"
              />
            </Grid>

            <Grid item xs={12} sm={6}>
              <TextField
                fullWidth
                label="Tags (comma-separated)"
                value={formTags}
                onChange={(e) => setFormTags(e.target.value)}
                placeholder="system-design, redis, amazon"
              />
            </Grid>

            <Grid item xs={12} sm={6}>
              <FormControl fullWidth>
                <InputLabel>Related Company (Optional)</InputLabel>
                <Select
                  value={formCompanyId}
                  label="Related Company (Optional)"
                  onChange={(e) => setFormCompanyId(e.target.value)}
                >
                  <MenuItem value="">None</MenuItem>
                  {companies.map((c) => (
                    <MenuItem key={c.id} value={c.id}>
                      {c.name}
                    </MenuItem>
                  ))}
                </Select>
              </FormControl>
            </Grid>

            <Grid item xs={12} sm={6}>
              <FormControl fullWidth>
                <InputLabel>Related Job Position (Optional)</InputLabel>
                <Select
                  value={formJobId}
                  label="Related Job Position (Optional)"
                  onChange={(e) => setFormJobId(e.target.value)}
                >
                  <MenuItem value="">None</MenuItem>
                  {jobs.map((j) => (
                    <MenuItem key={j.id} value={j.id}>
                      {j.title} {j.company ? `(${j.company})` : ''}
                    </MenuItem>
                  ))}
                </Select>
              </FormControl>
            </Grid>

            <Grid item xs={12}>
              <TextField
                fullWidth
                label="Short Summary"
                multiline
                rows={2}
                value={formSummary}
                onChange={(e) => setFormSummary(e.target.value)}
                placeholder="A concise tl;dr of this knowledge article..."
              />
            </Grid>

            <Grid item xs={12}>
              <TextField
                fullWidth
                label="Content (Markdown supported)"
                required
                multiline
                rows={14}
                value={formContent}
                onChange={(e) => setFormContent(e.target.value)}
                placeholder="Write in Markdown. Use [[target-article-slug]] or [[slug|Link Title]] to create automatic bi-directional backlinks."
                InputProps={{
                  sx: { fontFamily: 'Consolas, monospace', fontSize: '0.9rem' }
                }}
              />
              <Typography variant="caption" color="text.secondary" sx={{ mt: 0.5, display: 'block' }}>
                Tip: Type <code>[[slug]]</code> or <code>[[slug|Title]]</code> to link to another article.
              </Typography>
            </Grid>
          </Grid>
        </DialogContent>

        <DialogActions sx={{ px: 3, py: 2 }}>
          <Button onClick={() => setOpenDialog(false)} color="inherit">
            Cancel
          </Button>
          <Button onClick={handleSubmit} variant="contained" color="primary">
            {editingArticle ? 'Save Changes' : 'Create Article'}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  )
}

function LaunchOutlinedIconWrapper() {
  return (
    <Box component="span" sx={{ display: 'inline-flex', verticalAlign: 'middle' }}>
      <BookmarkIcon fontSize="small" color="secondary" />
    </Box>
  )
}
export default WikiPage
