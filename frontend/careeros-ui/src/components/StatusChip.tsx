import { Chip } from '@mui/material'

const STATUS_CONFIG: Record<number, { label: string; color: 'default' | 'primary' | 'secondary' | 'success' | 'error' | 'warning' | 'info' }> = {
  0: { label: 'Saved', color: 'default' },
  1: { label: 'Applied', color: 'primary' },
  2: { label: 'Phone Screen', color: 'info' },
  3: { label: 'Interview', color: 'secondary' },
  4: { label: 'Offer', color: 'success' },
  5: { label: 'Rejected', color: 'error' },
  6: { label: 'Withdrawn', color: 'default' },
  7: { label: 'Accepted', color: 'success' },
}

export default function StatusChip({ status }: { status: number }) {
  const config = STATUS_CONFIG[status] ?? { label: 'Unknown', color: 'default' }
  return <Chip label={config.label} color={config.color} size="small" />
}

