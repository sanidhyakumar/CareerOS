import React, { useEffect, useState } from 'react'
import {
  Box, Typography, Paper, Card, CardContent, IconButton, Button,
  Dialog, DialogTitle, DialogContent, DialogActions, TextField,
  FormControl, InputLabel, Select, MenuItem, Chip, Stack,
  CircularProgress, Alert, Tooltip, InputAdornment
} from '@mui/material'
import AddIcon from '@mui/icons-material/Add'
import EditIcon from '@mui/icons-material/Edit'
import DeleteIcon from '@mui/icons-material/Delete'
import DragIndicatorIcon from '@mui/icons-material/DragIndicator'
import SearchIcon from '@mui/icons-material/Search'
import RefreshIcon from '@mui/icons-material/Refresh'
import ViewKanbanIcon from '@mui/icons-material/ViewKanban'
import BusinessIcon from '@mui/icons-material/Business'
import LocationOnIcon from '@mui/icons-material/LocationOn'
import MonetizationOnIcon from '@mui/icons-material/MonetizationOn'

import {
  DndContext, DragOverlay, closestCorners, KeyboardSensor,
  PointerSensor, useSensor, useSensors, useDroppable
} from '@dnd-kit/core'
import type { DragStartEvent, DragEndEvent } from '@dnd-kit/core'
import {
  SortableContext, sortableKeyboardCoordinates,
  verticalListSortingStrategy, useSortable
} from '@dnd-kit/sortable'
import { CSS } from '@dnd-kit/utilities'

import {
  kanbanApi, KanbanColumnType, KanbanColumnLabels, KanbanColumnColors
} from '../api/kanban'
import type {
  KanbanCard, KanbanBoard, CreateKanbanCardRequest, UpdateKanbanCardRequest
} from '../api/kanban'
import { JobPriority } from '../api/jobs'

// Priority chip colors
const PRIORITY_CONFIG: Record<number, { label: string; color: 'secondary' | 'error' | 'warning' | 'default' }> = {
  [JobPriority.Dream]: { label: 'Dream Priority', color: 'secondary' },
  [JobPriority.High]: { label: 'High Priority', color: 'error' },
  [JobPriority.Medium]: { label: 'Medium', color: 'warning' },
  [JobPriority.Low]: { label: 'Low', color: 'default' },
}

// Droppable Column Component
interface DroppableColumnProps {
  columnId: number
  title: string
  cards: KanbanCard[]
  color: string
  onEditCard: (card: KanbanCard) => void
  onDeleteCard: (id: string) => void
  onAddCardToColumn: (columnId: number) => void
}

const DroppableColumn: React.FC<DroppableColumnProps> = ({
  columnId,
  title,
  cards,
  color,
  onEditCard,
  onDeleteCard,
  onAddCardToColumn,
}) => {
  const { setNodeRef } = useDroppable({
    id: `column-${columnId}`,
    data: { columnId },
  })

  return (
    <Paper
      ref={setNodeRef}
      sx={{
        flex: 1,
        minWidth: 280,
        maxWidth: 340,
        height: 'calc(100vh - 230px)',
        display: 'flex',
        flexDirection: 'column',
        bgcolor: 'background.paper',
        border: '1px solid',
        borderColor: 'divider',
        borderRadius: 2,
        overflow: 'hidden',
      }}
    >
      {/* Column Header */}
      <Box
        sx={{
          p: 1.5,
          px: 2,
          display: 'flex',
          justifyContent: 'space-between',
          alignItems: 'center',
          borderBottom: '2px solid',
          borderColor: color,
          bgcolor: 'action.hover',
        }}
      >
        <Stack direction="row" spacing={1} alignItems="center">
          <Box
            sx={{
              width: 10,
              height: 10,
              borderRadius: '50%',
              bgcolor: color,
            }}
          />
          <Typography variant="subtitle2" fontWeight="bold">
            {title}
          </Typography>
          <Chip
            label={cards.length}
            size="small"
            sx={{ height: 20, fontSize: '0.75rem', fontWeight: 'bold' }}
          />
        </Stack>
        <Tooltip title={`Add card to ${title}`}>
          <IconButton size="small" onClick={() => onAddCardToColumn(columnId)}>
            <AddIcon fontSize="small" />
          </IconButton>
        </Tooltip>
      </Box>

      {/* Column Cards Container */}
      <Box
        sx={{
          p: 1.5,
          flex: 1,
          overflowY: 'auto',
          display: 'flex',
          flexDirection: 'column',
          gap: 1.5,
        }}
      >
        <SortableContext
          items={cards.map((c) => c.id)}
          strategy={verticalListSortingStrategy}
        >
          {cards.map((card) => (
            <SortableCardItem
              key={card.id}
              card={card}
              onEdit={onEditCard}
              onDelete={onDeleteCard}
            />
          ))}
        </SortableContext>
        {cards.length === 0 && (
          <Box
            sx={{
              p: 3,
              textAlign: 'center',
              border: '1px dashed',
              borderColor: 'divider',
              borderRadius: 1.5,
              mt: 1,
            }}
          >
            <Typography variant="caption" color="text.secondary">
              Drop items here
            </Typography>
          </Box>
        )}
      </Box>
    </Paper>
  )
}

// Sortable Card Item Component
interface SortableCardItemProps {
  card: KanbanCard
  onEdit: (card: KanbanCard) => void
  onDelete: (id: string) => void
}

const SortableCardItem: React.FC<SortableCardItemProps> = ({
  card,
  onEdit,
  onDelete,
}) => {
  const {
    attributes,
    listeners,
    setNodeRef,
    transform,
    transition,
    isDragging,
  } = useSortable({
    id: card.id,
    data: { card },
  })

  const style = {
    transform: CSS.Transform.toString(transform),
    transition,
    opacity: isDragging ? 0.4 : 1,
  }

  const priorityCfg = PRIORITY_CONFIG[card.priority] || PRIORITY_CONFIG[JobPriority.Medium]

  return (
    <Card
      ref={setNodeRef}
      style={style}
      variant="outlined"
      sx={{
        cursor: 'grab',
        position: 'relative',
        transition: 'all 0.15s ease',
        '&:hover': {
          boxShadow: 3,
          borderColor: 'primary.main',
        },
      }}
    >
      <CardContent sx={{ p: 1.5, '&:last-child': { pb: 1.5 } }}>
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', mb: 0.5 }}>
          <Stack direction="row" spacing={0.5} alignItems="center">
            <Box {...attributes} {...listeners} sx={{ cursor: 'grab', display: 'flex' }}>
              <DragIndicatorIcon fontSize="small" sx={{ color: 'text.disabled' }} />
            </Box>
            <Chip
              label={priorityCfg.label}
              size="small"
              color={priorityCfg.color}
              sx={{ height: 18, fontSize: '0.65rem', fontWeight: 600 }}
            />
          </Stack>
          <Box>
            <IconButton size="small" onClick={() => onEdit(card)} sx={{ p: 0.25 }}>
              <EditIcon sx={{ fontSize: '0.85rem' }} />
            </IconButton>
            <IconButton size="small" onClick={() => onDelete(card.id)} color="error" sx={{ p: 0.25 }}>
              <DeleteIcon sx={{ fontSize: '0.85rem' }} />
            </IconButton>
          </Box>
        </Box>

        <Typography variant="subtitle2" fontWeight="bold" sx={{ mb: 0.25 }}>
          {card.title}
        </Typography>

        <Typography variant="body2" color="text.secondary" sx={{ display: 'flex', alignItems: 'center', gap: 0.5, mb: 1 }}>
          <BusinessIcon sx={{ fontSize: '0.85rem' }} /> {card.companyName || 'Unknown Employer'}
        </Typography>

        <Stack direction="row" spacing={0.5} flexWrap="wrap" useFlexGap sx={{ mb: 1 }}>
          {card.location && (
            <Chip
              icon={<LocationOnIcon sx={{ fontSize: '0.75rem !important' }} />}
              label={card.location}
              size="small"
              variant="outlined"
              sx={{ height: 20, fontSize: '0.7rem' }}
            />
          )}
          {card.salary && (
            <Chip
              icon={<MonetizationOnIcon sx={{ fontSize: '0.75rem !important' }} />}
              label={card.salary}
              size="small"
              variant="outlined"
              color="success"
              sx={{ height: 20, fontSize: '0.7rem' }}
            />
          )}
        </Stack>

        {card.tagsList.length > 0 && (
          <Stack direction="row" spacing={0.5} flexWrap="wrap" useFlexGap sx={{ mb: 0.5 }}>
            {card.tagsList.slice(0, 3).map((tag) => (
              <Chip
                key={tag}
                label={`#${tag}`}
                size="small"
                sx={{ height: 16, fontSize: '0.62rem', bgcolor: 'action.hover' }}
              />
            ))}
          </Stack>
        )}

        {card.notes && (
          <Typography
            variant="caption"
            color="text.secondary"
            sx={{
              display: '-webkit-box',
              WebkitLineClamp: 2,
              WebkitBoxOrient: 'vertical',
              overflow: 'hidden',
              fontSize: '0.7rem',
            }}
          >
            {card.notes}
          </Typography>
        )}
      </CardContent>
    </Card>
  )
}

export const KanbanPage: React.FC = () => {
  const [board, setBoard] = useState<KanbanBoard | null>(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [successMessage, setSuccessMessage] = useState<string | null>(null)

  // Dragging active item
  const [activeCard, setActiveCard] = useState<KanbanCard | null>(null)

  // Filters
  const [searchTerm, setSearchTerm] = useState('')
  const [filterPriority, setFilterPriority] = useState<number | ''>('')

  // Dialog State
  const [openDialog, setOpenDialog] = useState(false)
  const [editingCard, setEditingCard] = useState<KanbanCard | null>(null)

  // Form State
  const [formTitle, setFormTitle] = useState('')
  const [formCompany, setFormCompany] = useState('')
  const [formColumn, setFormColumn] = useState<number>(KanbanColumnType.Interested)
  const [formPriority, setFormPriority] = useState<number>(JobPriority.Medium)
  const [formSalary, setFormSalary] = useState('')
  const [formLocation, setFormLocation] = useState('')
  const [formTags, setFormTags] = useState('')
  const [formNotes, setFormNotes] = useState('')

  // Configure Sensors for drag interactions
  const sensors = useSensors(
    useSensor(PointerSensor, {
      activationConstraint: {
        distance: 5,
      },
    }),
    useSensor(KeyboardSensor, {
      coordinateGetter: sortableKeyboardCoordinates,
    })
  )

  const loadBoard = async () => {
    try {
      setLoading(true)
      const data = await kanbanApi.getBoard()
      setBoard(data)
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Failed to load Kanban board')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    loadBoard()
  }, [])

  const handleDragStart = (event: DragStartEvent) => {
    const card = event.active.data.current?.card as KanbanCard | undefined
    if (card) {
      setActiveCard(card)
    }
  }

  const handleDragEnd = async (event: DragEndEvent) => {
    const { active, over } = event
    setActiveCard(null)

    if (!over || !board) return

    const cardId = active.id as string
    let targetColumnId: number | null = null

    // Check if dropped directly onto a column container
    if (String(over.id).startsWith('column-')) {
      targetColumnId = Number(String(over.id).replace('column-', ''))
    } else {
      // Dropped onto another card, find target column from that card
      for (const col of board.columns) {
        if (col.cards.some((c) => c.id === over.id)) {
          targetColumnId = col.columnId
          break
        }
      }
    }

    if (targetColumnId === null) return

    // Find current card and its column
    let currentCard: KanbanCard | null = null
    let sourceColumnId: number | null = null
    for (const col of board.columns) {
      const found = col.cards.find((c) => c.id === cardId)
      if (found) {
        currentCard = found
        sourceColumnId = col.columnId
        break
      }
    }

    if (!currentCard || sourceColumnId === null) return

    // Calculate new position
    const targetCol = board.columns.find((c) => c.columnId === targetColumnId)
    const newPosition = targetCol ? targetCol.cards.length : 0

    if (sourceColumnId === targetColumnId && over.id === active.id) {
      return
    }

    // Optimistic state update for instant UI feedback
    setBoard((prev) => {
      if (!prev) return prev
      const updatedCard = { ...currentCard!, column: targetColumnId!, position: newPosition }
      return {
        ...prev,
        columns: prev.columns.map((col) => {
          if (col.columnId === sourceColumnId && sourceColumnId !== targetColumnId) {
            return { ...col, cards: col.cards.filter((c) => c.id !== cardId) }
          }
          if (col.columnId === targetColumnId) {
            const filtered = col.cards.filter((c) => c.id !== cardId)
            return { ...col, cards: [...filtered, updatedCard] }
          }
          return col
        }),
      }
    })

    // Persist to backend API
    try {
      await kanbanApi.moveCard(cardId, {
        targetColumn: targetColumnId,
        newPosition,
      })
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Failed to move card')
      loadBoard() // Rollback on failure
    }
  }

  const handleOpenCreate = (defaultColumn: number = KanbanColumnType.Interested) => {
    setEditingCard(null)
    setFormTitle('')
    setFormCompany('')
    setFormColumn(defaultColumn)
    setFormPriority(JobPriority.Medium)
    setFormSalary('')
    setFormLocation('')
    setFormTags('')
    setFormNotes('')
    setOpenDialog(true)
  }

  const handleOpenEdit = (card: KanbanCard) => {
    setEditingCard(card)
    setFormTitle(card.title)
    setFormCompany(card.companyName)
    setFormColumn(card.column)
    setFormPriority(card.priority)
    setFormSalary(card.salary || '')
    setFormLocation(card.location || '')
    setFormTags(card.tags || '')
    setFormNotes(card.notes || '')
    setOpenDialog(true)
  }

  const handleDeleteCard = async (id: string) => {
    if (!window.confirm('Delete this Kanban card?')) return
    try {
      await kanbanApi.deleteCard(id)
      setSuccessMessage('Card removed.')
      loadBoard()
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Failed to delete card')
    }
  }

  const handleSubmitCard = async () => {
    if (!formTitle.trim()) {
      setError('Title is required')
      return
    }

    try {
      if (editingCard) {
        const updateReq: UpdateKanbanCardRequest = {
          id: editingCard.id,
          title: formTitle.trim(),
          companyName: formCompany.trim(),
          column: formColumn,
          position: editingCard.position,
          companyId: editingCard.companyId,
          jobId: editingCard.jobId,
          applicationId: editingCard.applicationId,
          priority: formPriority,
          salary: formSalary.trim() || undefined,
          location: formLocation.trim() || undefined,
          notes: formNotes.trim() || undefined,
          tags: formTags.trim() || undefined,
        }
        await kanbanApi.updateCard(editingCard.id, updateReq)
        setSuccessMessage('Card updated successfully!')
      } else {
        const createReq: CreateKanbanCardRequest = {
          title: formTitle.trim(),
          companyName: formCompany.trim(),
          column: formColumn,
          priority: formPriority,
          salary: formSalary.trim() || undefined,
          location: formLocation.trim() || undefined,
          notes: formNotes.trim() || undefined,
          tags: formTags.trim() || undefined,
        }
        await kanbanApi.createCard(createReq)
        setSuccessMessage('Card created successfully!')
      }
      setOpenDialog(false)
      loadBoard()
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Failed to save card')
    }
  }

  // Filter cards per column
  const getFilteredColumnCards = (cards: KanbanCard[]) => {
    return cards.filter((c) => {
      const matchSearch =
        !searchTerm ||
        c.title.toLowerCase().includes(searchTerm.toLowerCase()) ||
        c.companyName.toLowerCase().includes(searchTerm.toLowerCase()) ||
        (c.tags && c.tags.toLowerCase().includes(searchTerm.toLowerCase()))
      const matchPriority = filterPriority === '' || c.priority === filterPriority
      return matchSearch && matchPriority
    })
  }

  return (
    <Box sx={{ p: 3, maxWidth: 1800, margin: '0 auto' }}>
      {/* Header and Actions */}
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
        <Box>
          <Typography variant="h4" fontWeight="bold" sx={{ display: 'flex', alignItems: 'center', gap: 1.5 }}>
            <ViewKanbanIcon fontSize="large" color="primary" />
            Job Search Kanban Board
          </Typography>
          <Typography variant="subtitle1" color="text.secondary">
            Drag-and-drop opportunity workflow synchronized with positions and applications.
          </Typography>
        </Box>
        <Stack direction="row" spacing={1.5}>
          <Tooltip title="Synchronize & Refresh Board">
            <IconButton onClick={loadBoard} color="inherit">
              <RefreshIcon />
            </IconButton>
          </Tooltip>
          <Button
            variant="contained"
            color="primary"
            startIcon={<AddIcon />}
            onClick={() => handleOpenCreate(KanbanColumnType.Interested)}
          >
            New Card
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

      {/* Filter Bar */}
      <Paper sx={{ p: 1.5, mb: 2.5, border: '1px solid', borderColor: 'divider', display: 'flex', gap: 2, alignItems: 'center' }}>
        <TextField
          size="small"
          placeholder="Filter cards by title, company, or tags..."
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
          sx={{ width: 350 }}
          InputProps={{
            startAdornment: (
              <InputAdornment position="start">
                <SearchIcon fontSize="small" />
              </InputAdornment>
            ),
          }}
        />

        <FormControl size="small" sx={{ width: 180 }}>
          <InputLabel>Filter Priority</InputLabel>
          <Select
            value={filterPriority}
            label="Filter Priority"
            onChange={(e) => setFilterPriority(e.target.value as number | '')}
          >
            <MenuItem value="">All Priorities</MenuItem>
            {Object.entries(PRIORITY_CONFIG).map(([key, cfg]) => (
              <MenuItem key={key} value={Number(key)}>
                {cfg.label}
              </MenuItem>
            ))}
          </Select>
        </FormControl>

        {board && (
          <Typography variant="caption" color="text.secondary" sx={{ ml: 'auto', fontWeight: 'bold' }}>
            {board.totalCards} Total Opportunities
          </Typography>
        )}
      </Paper>

      {/* Board Columns Container */}
      {loading && !board ? (
        <Box sx={{ display: 'flex', justifyContent: 'center', p: 8 }}>
          <CircularProgress />
        </Box>
      ) : board ? (
        <DndContext
          sensors={sensors}
          collisionDetection={closestCorners}
          onDragStart={handleDragStart}
          onDragEnd={handleDragEnd}
        >
          <Box
            sx={{
              display: 'flex',
              gap: 2,
              overflowX: 'auto',
              pb: 2,
              alignItems: 'flex-start',
            }}
          >
            {board.columns.map((col) => (
              <DroppableColumn
                key={col.columnId}
                columnId={col.columnId}
                title={KanbanColumnLabels[col.columnId] || col.title}
                cards={getFilteredColumnCards(col.cards)}
                color={KanbanColumnColors[col.columnId] || '#1976d2'}
                onEditCard={handleOpenEdit}
                onDeleteCard={handleDeleteCard}
                onAddCardToColumn={handleOpenCreate}
              />
            ))}
          </Box>

          {/* Active Drag Overlay */}
          <DragOverlay>
            {activeCard ? (
              <Card
                variant="outlined"
                sx={{
                  width: 300,
                  boxShadow: 6,
                  cursor: 'grabbing',
                  transform: 'rotate(2deg)',
                  borderColor: 'primary.main',
                }}
              >
                <CardContent sx={{ p: 1.5, '&:last-child': { pb: 1.5 } }}>
                  <Typography variant="subtitle2" fontWeight="bold">
                    {activeCard.title}
                  </Typography>
                  <Typography variant="body2" color="text.secondary">
                    {activeCard.companyName}
                  </Typography>
                </CardContent>
              </Card>
            ) : null}
          </DragOverlay>
        </DndContext>
      ) : null}

      {/* Create / Edit Card Dialog */}
      <Dialog
        open={openDialog}
        onClose={() => setOpenDialog(false)}
        maxWidth="sm"
        fullWidth
      >
        <DialogTitle>
          {editingCard ? 'Edit Opportunity Card' : 'Add Opportunity Card'}
        </DialogTitle>
        <DialogContent dividers>
          <Stack spacing={2} sx={{ mt: 1 }}>
            <TextField
              fullWidth
              label="Role / Position Title"
              required
              value={formTitle}
              onChange={(e) => setFormTitle(e.target.value)}
              placeholder="e.g. Senior Backend Engineer"
            />

            <TextField
              fullWidth
              label="Company / Employer"
              value={formCompany}
              onChange={(e) => setFormCompany(e.target.value)}
              placeholder="e.g. Stripe, OpenAI, Google"
            />

            <Stack direction="row" spacing={2}>
              <FormControl fullWidth>
                <InputLabel>Column</InputLabel>
                <Select
                  value={formColumn}
                  label="Column"
                  onChange={(e) => setFormColumn(Number(e.target.value))}
                >
                  {Object.entries(KanbanColumnLabels).map(([key, label]) => (
                    <MenuItem key={key} value={Number(key)}>
                      {label}
                    </MenuItem>
                  ))}
                </Select>
              </FormControl>

              <FormControl fullWidth>
                <InputLabel>Priority</InputLabel>
                <Select
                  value={formPriority}
                  label="Priority"
                  onChange={(e) => setFormPriority(Number(e.target.value))}
                >
                  {Object.entries(PRIORITY_CONFIG).map(([key, cfg]) => (
                    <MenuItem key={key} value={Number(key)}>
                      {cfg.label}
                    </MenuItem>
                  ))}
                </Select>
              </FormControl>
            </Stack>

            <Stack direction="row" spacing={2}>
              <TextField
                fullWidth
                label="Compensation / Salary"
                value={formSalary}
                onChange={(e) => setFormSalary(e.target.value)}
                placeholder="$160k - $200k + Equity"
              />

              <TextField
                fullWidth
                label="Location / Workplace"
                value={formLocation}
                onChange={(e) => setFormLocation(e.target.value)}
                placeholder="Remote, NYC, Seattle"
              />
            </Stack>

            <TextField
              fullWidth
              label="Tags (comma-separated)"
              value={formTags}
              onChange={(e) => setFormTags(e.target.value)}
              placeholder="react, dotnet, kubernetes"
            />

            <TextField
              fullWidth
              multiline
              rows={3}
              label="Personal Notes & Next Action"
              value={formNotes}
              onChange={(e) => setFormNotes(e.target.value)}
              placeholder="Referral contact, application link, follow-up timeline..."
            />
          </Stack>
        </DialogContent>
        <DialogActions sx={{ px: 3, py: 2 }}>
          <Button onClick={() => setOpenDialog(false)} color="inherit">
            Cancel
          </Button>
          <Button onClick={handleSubmitCard} variant="contained" color="primary">
            {editingCard ? 'Save Changes' : 'Create Card'}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  )
}

export default KanbanPage
