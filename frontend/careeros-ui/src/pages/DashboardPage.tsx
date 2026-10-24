import { useState, useEffect } from 'react';
import {
  Box,
  Grid,
  Typography,
  Card,
  CardContent,
  Paper,
  Chip,
  Select,
  MenuItem,
  FormControl,
  InputLabel,
  LinearProgress,
  CircularProgress,
  Alert,
  Button,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
} from '@mui/material';
import SendIcon from '@mui/icons-material/Send';
import InterpreterModeIcon from '@mui/icons-material/InterpreterMode';
import EmojiEventsIcon from '@mui/icons-material/EmojiEvents';
import FlightTakeoffIcon from '@mui/icons-material/FlightTakeoff';
import ThumbDownAltIcon from '@mui/icons-material/ThumbDownAlt';
import RefreshIcon from '@mui/icons-material/Refresh';
import StarIcon from '@mui/icons-material/Star';
import SpeedIcon from '@mui/icons-material/Speed';
import AttachMoneyIcon from '@mui/icons-material/AttachMoney';

import {
  ResponsiveContainer,
  AreaChart,
  Area,
  BarChart,
  Bar,
  XAxis,
  YAxis,
  Tooltip as RechartsTooltip,
  Legend,
  CartesianGrid,
  PieChart,
  Pie,
  Cell,
} from 'recharts';

import { analyticsApi, type CareerAnalytics } from '../api/analytics';
import { useProfile } from '../context/ProfileContext';

const PIE_COLORS = ['#3b82f6', '#10b981', '#f59e0b', '#ef4444', '#8b5cf6', '#06b6d4'];

export default function DashboardPage() {
  const { profiles } = useProfile();
  const [selectedProfileId, setSelectedProfileId] = useState<string>('all');
  const [analytics, setAnalytics] = useState<CareerAnalytics | null>(null);
  const [loading, setLoading] = useState(true);
  const [timeframeDays, setTimeframeDays] = useState<number>(0);
  const [error, setError] = useState<string | null>(null);

  const fetchAnalytics = async (days?: number) => {
    setLoading(true);
    setError(null);
    try {
      const data = await analyticsApi.getAnalytics(days && days > 0 ? days : undefined);
      setAnalytics(data);
    } catch (err) {
      console.error('Failed to load career analytics:', err);
      setError('Unable to load career intelligence data. Please ensure the backend is running.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchAnalytics(timeframeDays);
  }, [timeframeDays]);

  if (loading && !analytics) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: 400 }}>
        <CircularProgress />
      </Box>
    );
  }

  // Format currency numbers
  const formatSalary = (val: number | null | undefined, cur: string = 'USD') => {
    if (!val) return 'N/A';
    return `${cur === 'USD' ? '$' : cur + ' '}${Math.round(val).toLocaleString()}`;
  };

  const rejectionPieData = analytics?.rejectionAnalytics.rejectionsByStage
    ? Object.entries(analytics.rejectionAnalytics.rejectionsByStage)
        .filter(([_, count]) => count > 0)
        .map(([stage, count]) => ({ name: stage, value: count }))
    : [];

  const interviewRoundData = analytics?.interviewMetrics.roundsByType
    ? Object.entries(analytics.interviewMetrics.roundsByType).map(([type, count]) => ({
        type,
        rounds: count,
        passRate: analytics.interviewMetrics.passRateByType[type] ?? 0,
      }))
    : [];

  return (
    <Box sx={{ maxWidth: 1400, mx: 'auto', p: 1 }}>
      {/* Header and Filter Controls */}
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', mb: 3, flexWrap: 'wrap', gap: 2 }}>
        <Box>
          <Typography variant="h4" fontWeight={700} gutterBottom>
            Career Intelligence Dashboard
          </Typography>
          <Typography variant="body1" color="text.secondary">
            End-to-end telemetry on application velocity, conversion funnels, interview pass rates, and compensation.
          </Typography>
        </Box>

        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1.5 }}>
          <FormControl size="small" sx={{ minWidth: 200 }}>
            <InputLabel id="profile-scope-label">Profile Analytics</InputLabel>
            <Select
              labelId="profile-scope-label"
              value={selectedProfileId}
              label="Profile Analytics"
              onChange={(e) => setSelectedProfileId(e.target.value)}
            >
              <MenuItem value="all">Cross-Profile / All Profiles</MenuItem>
              {profiles.map((p) => (
                <MenuItem key={p.id} value={p.id}>
                  {p.name} ({p.relationship})
                </MenuItem>
              ))}
            </Select>
          </FormControl>

          <FormControl size="small" sx={{ minWidth: 150 }}>
            <InputLabel id="timeframe-select-label">Timeframe</InputLabel>
            <Select
              labelId="timeframe-select-label"
              value={timeframeDays}
              label="Timeframe"
              onChange={(e) => setTimeframeDays(Number(e.target.value))}
            >
              <MenuItem value={0}>All Time</MenuItem>
              <MenuItem value={30}>Last 30 Days</MenuItem>
              <MenuItem value={90}>Last 90 Days</MenuItem>
              <MenuItem value={180}>Last 6 Months</MenuItem>
              <MenuItem value={365}>Last 12 Months</MenuItem>
            </Select>
          </FormControl>

          <Button
            variant="outlined"
            startIcon={<RefreshIcon />}
            onClick={() => fetchAnalytics(timeframeDays)}
            sx={{ textTransform: 'none', borderRadius: 2 }}
          >
            Refresh
          </Button>
        </Box>
      </Box>

      {error && <Alert severity="error" sx={{ mb: 3 }}>{error}</Alert>}

      {analytics && (
        <>
          {/* Executive KPI Ribbon */}
          <Grid container spacing={2} sx={{ mb: 3 }}>
            <Grid item xs={12} sm={6} md={2}>
              <Card variant="outlined" sx={{ borderRadius: 2, height: '100%' }}>
                <CardContent sx={{ pb: '16px !important' }}>
                  <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', mb: 1 }}>
                    <Typography variant="caption" color="text.secondary" fontWeight={600}>
                      APPLICATIONS
                    </Typography>
                    <SendIcon fontSize="small" sx={{ color: '#2563eb' }} />
                  </Box>
                  <Typography variant="h4" fontWeight={700}>
                    {analytics.responseMetrics.totalApplications}
                  </Typography>
                  <Typography variant="caption" color="text.secondary">
                    {analytics.responseMetrics.responseRatePercent}% response rate
                  </Typography>
                </CardContent>
              </Card>
            </Grid>

            <Grid item xs={12} sm={6} md={2}>
              <Card variant="outlined" sx={{ borderRadius: 2, height: '100%' }}>
                <CardContent sx={{ pb: '16px !important' }}>
                  <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', mb: 1 }}>
                    <Typography variant="caption" color="text.secondary" fontWeight={600}>
                      AVG RESPONSE
                    </Typography>
                    <SpeedIcon fontSize="small" sx={{ color: '#0891b2' }} />
                  </Box>
                  <Typography variant="h4" fontWeight={700}>
                    {analytics.responseMetrics.averageResponseDays} <Typography component="span" variant="body2">days</Typography>
                  </Typography>
                  <Typography variant="caption" color="text.secondary">
                    {analytics.responseMetrics.ghostedCount} unresponsive / stale
                  </Typography>
                </CardContent>
              </Card>
            </Grid>

            <Grid item xs={12} sm={6} md={2}>
              <Card variant="outlined" sx={{ borderRadius: 2, height: '100%' }}>
                <CardContent sx={{ pb: '16px !important' }}>
                  <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', mb: 1 }}>
                    <Typography variant="caption" color="text.secondary" fontWeight={600}>
                      INTERVIEW RATE
                    </Typography>
                    <InterpreterModeIcon fontSize="small" sx={{ color: '#ea580c' }} />
                  </Box>
                  <Typography variant="h4" fontWeight={700}>
                    {analytics.interviewMetrics.passRatePercent}%
                  </Typography>
                  <Typography variant="caption" color="text.secondary">
                    {analytics.interviewMetrics.totalInterviews} rounds ({analytics.interviewMetrics.averageRating}★ avg)
                  </Typography>
                </CardContent>
              </Card>
            </Grid>

            <Grid item xs={12} sm={6} md={2}>
              <Card variant="outlined" sx={{ borderRadius: 2, height: '100%' }}>
                <CardContent sx={{ pb: '16px !important' }}>
                  <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', mb: 1 }}>
                    <Typography variant="caption" color="text.secondary" fontWeight={600}>
                      OFFERS RECEIVED
                    </Typography>
                    <EmojiEventsIcon fontSize="small" sx={{ color: '#16a34a' }} />
                  </Box>
                  <Typography variant="h4" fontWeight={700}>
                    {analytics.offerMetrics.totalOffers}
                  </Typography>
                  <Typography variant="caption" color="text.secondary">
                    {analytics.offerMetrics.offerRatePercent}% conversion rate
                  </Typography>
                </CardContent>
              </Card>
            </Grid>

            <Grid item xs={12} sm={6} md={2}>
              <Card variant="outlined" sx={{ borderRadius: 2, height: '100%' }}>
                <CardContent sx={{ pb: '16px !important' }}>
                  <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', mb: 1 }}>
                    <Typography variant="caption" color="text.secondary" fontWeight={600}>
                      AVG OFFER SALARY
                    </Typography>
                    <AttachMoneyIcon fontSize="small" sx={{ color: '#059669' }} />
                  </Box>
                  <Typography variant="h5" fontWeight={700}>
                    {formatSalary(analytics.offerMetrics.averageSalary, analytics.offerMetrics.currency)}
                  </Typography>
                  <Typography variant="caption" color="text.secondary">
                    High: {formatSalary(analytics.offerMetrics.maxSalary, analytics.offerMetrics.currency)}
                  </Typography>
                </CardContent>
              </Card>
            </Grid>

            <Grid item xs={12} sm={6} md={2}>
              <Card variant="outlined" sx={{ borderRadius: 2, height: '100%' }}>
                <CardContent sx={{ pb: '16px !important' }}>
                  <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', mb: 1 }}>
                    <Typography variant="caption" color="text.secondary" fontWeight={600}>
                      SPONSORSHIP PIPELINE
                    </Typography>
                    <FlightTakeoffIcon fontSize="small" sx={{ color: '#7c3aed' }} />
                  </Box>
                  <Typography variant="h4" fontWeight={700}>
                    {analytics.sponsorshipMetrics.applicationsRequiringSponsorship}
                  </Typography>
                  <Typography variant="caption" color="text.secondary">
                    {analytics.sponsorshipMetrics.sponsorshipOfferRatePercent}% sponsor offer rate
                  </Typography>
                </CardContent>
              </Card>
            </Grid>
          </Grid>

          {/* Section 1: Application Funnel */}
          <Card variant="outlined" sx={{ borderRadius: 3, mb: 3 }}>
            <CardContent>
              <Typography variant="h6" fontWeight={600} gutterBottom>
                Application Conversion Funnel
              </Typography>
              <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
                Progression through hiring stages from initial submission to signed offer.
              </Typography>

              <Grid container spacing={2}>
                {analytics.funnel.map((stage, idx) => (
                  <Grid item xs={12} sm={6} md={2} key={stage.stageName}>
                    <Paper
                      variant="outlined"
                      sx={{
                        p: 2,
                        borderRadius: 2,
                        height: '100%',
                        display: 'flex',
                        flexDirection: 'column',
                        justifyContent: 'space-between',
                        borderTop: '4px solid',
                        borderTopColor:
                          idx === 0
                            ? '#2563eb'
                            : idx === 1
                            ? '#0284c7'
                            : idx === 2
                            ? '#7c3aed'
                            : idx === 3
                            ? '#d97706'
                            : idx === 4
                            ? '#16a34a'
                            : '#059669',
                      }}
                    >
                      <Box>
                        <Typography variant="caption" color="text.secondary" fontWeight={600}>
                          STEP {idx + 1}
                        </Typography>
                        <Typography variant="subtitle2" fontWeight={700}>
                          {stage.stageName}
                        </Typography>
                        <Typography variant="h4" fontWeight={700} sx={{ my: 1 }}>
                          {stage.count}
                        </Typography>
                      </Box>
                      <Box>
                        <LinearProgress
                          variant="determinate"
                          value={Math.min(100, stage.conversionRatePercent)}
                          sx={{ height: 6, borderRadius: 3, mb: 0.5 }}
                        />
                        <Box sx={{ display: 'flex', justifyContent: 'space-between' }}>
                          <Typography variant="caption" color="text.secondary">
                            {stage.conversionRatePercent}% conv
                          </Typography>
                          {stage.dropoffCount > 0 && (
                            <Typography variant="caption" color="error.main">
                              -{stage.dropoffCount} drop
                            </Typography>
                          )}
                        </Box>
                      </Box>
                    </Paper>
                  </Grid>
                ))}
              </Grid>
            </CardContent>
          </Card>

          {/* Section 2: Velocity Timeline Chart */}
          <Card variant="outlined" sx={{ borderRadius: 3, mb: 3 }}>
            <CardContent>
              <Typography variant="h6" fontWeight={600} gutterBottom>
                Application Velocity & Activity (Past 6 Months)
              </Typography>
              <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
                Volume of applications sent, interviews conducted, and offers extended month over month.
              </Typography>
              <Box sx={{ width: '100%', height: 280 }}>
                <ResponsiveContainer width="100%" height="100%">
                  <AreaChart data={analytics.velocityTimeline} margin={{ top: 10, right: 30, left: 0, bottom: 0 }}>
                    <defs>
                      <linearGradient id="colorApplied" x1="0" y1="0" x2="0" y2="1">
                        <stop offset="5%" stopColor="#3b82f6" stopOpacity={0.8} />
                        <stop offset="95%" stopColor="#3b82f6" stopOpacity={0} />
                      </linearGradient>
                      <linearGradient id="colorInterviews" x1="0" y1="0" x2="0" y2="1">
                        <stop offset="5%" stopColor="#f59e0b" stopOpacity={0.8} />
                        <stop offset="95%" stopColor="#f59e0b" stopOpacity={0} />
                      </linearGradient>
                      <linearGradient id="colorOffers" x1="0" y1="0" x2="0" y2="1">
                        <stop offset="5%" stopColor="#10b981" stopOpacity={0.8} />
                        <stop offset="95%" stopColor="#10b981" stopOpacity={0} />
                      </linearGradient>
                    </defs>
                    <CartesianGrid strokeDasharray="3 3" opacity={0.3} />
                    <XAxis dataKey="monthLabel" />
                    <YAxis allowDecimals={false} />
                    <RechartsTooltip />
                    <Legend />
                    <Area type="monotone" dataKey="appliedCount" name="Applications" stroke="#3b82f6" fillOpacity={1} fill="url(#colorApplied)" />
                    <Area type="monotone" dataKey="interviewCount" name="Interviews" stroke="#f59e0b" fillOpacity={1} fill="url(#colorInterviews)" />
                    <Area type="monotone" dataKey="offerCount" name="Offers" stroke="#10b981" fillOpacity={1} fill="url(#colorOffers)" />
                  </AreaChart>
                </ResponsiveContainer>
              </Box>
            </CardContent>
          </Card>

          {/* Section 3: Dual Intelligence: Rejections & Interview Analytics */}
          <Grid container spacing={3} sx={{ mb: 3 }}>
            {/* Rejection Analytics */}
            <Grid item xs={12} md={6}>
              <Card variant="outlined" sx={{ borderRadius: 3, height: '100%' }}>
                <CardContent>
                  <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', mb: 1 }}>
                    <Typography variant="h6" fontWeight={600}>
                      Rejection Intelligence & Drop-Off
                    </Typography>
                    <Chip
                      label={`${analytics.rejectionAnalytics.rejectionRatePercent}% Rejection Rate`}
                      size="small"
                      color="error"
                      variant="outlined"
                      sx={{ fontWeight: 600 }}
                    />
                  </Box>
                  <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
                    Stages where candidate applications exited the interview process.
                  </Typography>

                  {rejectionPieData.length > 0 ? (
                    <Box sx={{ display: 'flex', alignItems: 'center', height: 220 }}>
                      <ResponsiveContainer width="50%" height="100%">
                        <PieChart>
                          <Pie
                            data={rejectionPieData}
                            dataKey="value"
                            nameKey="name"
                            cx="50%"
                            cy="50%"
                            outerRadius={75}
                            innerRadius={45}
                          >
                            {rejectionPieData.map((_, index) => (
                              <Cell key={`cell-${index}`} fill={PIE_COLORS[index % PIE_COLORS.length]} />
                            ))}
                          </Pie>
                          <RechartsTooltip />
                        </PieChart>
                      </ResponsiveContainer>
                      <Box sx={{ width: '50%', pl: 2 }}>
                        {rejectionPieData.map((entry, index) => (
                          <Box key={entry.name} sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 1 }}>
                            <Box sx={{ width: 12, height: 12, borderRadius: 0.5, bgcolor: PIE_COLORS[index % PIE_COLORS.length] }} />
                            <Typography variant="caption" sx={{ flexGrow: 1 }}>
                              {entry.name}:
                            </Typography>
                            <Typography variant="caption" fontWeight={700}>
                              {entry.value}
                            </Typography>
                          </Box>
                        ))}
                      </Box>
                    </Box>
                  ) : (
                    <Box sx={{ py: 6, textAlign: 'center' }}>
                      <ThumbDownAltIcon sx={{ fontSize: 40, color: 'text.disabled', mb: 1 }} />
                      <Typography variant="body2" color="text.secondary">
                        No rejection drop-offs logged yet.
                      </Typography>
                    </Box>
                  )}

                  {analytics.rejectionAnalytics.keyFeedbackObservations.length > 0 && (
                    <Box sx={{ mt: 2, pt: 2, borderTop: '1px solid', borderColor: 'divider' }}>
                      <Typography variant="caption" fontWeight={600} color="text.secondary" gutterBottom>
                        OBSERVATIONS & REJECTION NOTES
                      </Typography>
                      {analytics.rejectionAnalytics.keyFeedbackObservations.map((fb, idx) => (
                        <Typography key={idx} variant="caption" sx={{ display: 'block', color: 'text.secondary', mt: 0.5 }}>
                          • {fb}
                        </Typography>
                      ))}
                    </Box>
                  )}
                </CardContent>
              </Card>
            </Grid>

            {/* Interview Success Breakdown */}
            <Grid item xs={12} md={6}>
              <Card variant="outlined" sx={{ borderRadius: 3, height: '100%' }}>
                <CardContent>
                  <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', mb: 1 }}>
                    <Typography variant="h6" fontWeight={600}>
                      Interview Performance by Round Type
                    </Typography>
                    <Chip
                      icon={<StarIcon sx={{ fontSize: 14 }} />}
                      label={`${analytics.interviewMetrics.averageRating} / 5 Avg Rating`}
                      size="small"
                      color="primary"
                      variant="outlined"
                      sx={{ fontWeight: 600 }}
                    />
                  </Box>
                  <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
                    Rounds completed and pass rates categorized across technical and behavioral stages.
                  </Typography>

                  {interviewRoundData.length > 0 ? (
                    <Box sx={{ width: '100%', height: 260 }}>
                      <ResponsiveContainer width="100%" height="100%">
                        <BarChart data={interviewRoundData} margin={{ top: 10, right: 30, left: 0, bottom: 0 }}>
                          <CartesianGrid strokeDasharray="3 3" opacity={0.3} />
                          <XAxis dataKey="type" />
                          <YAxis allowDecimals={false} />
                          <RechartsTooltip />
                          <Legend />
                          <Bar dataKey="rounds" name="Total Rounds" fill="#8b5cf6" radius={[4, 4, 0, 0]} />
                        </BarChart>
                      </ResponsiveContainer>
                    </Box>
                  ) : (
                    <Box sx={{ py: 6, textAlign: 'center' }}>
                      <InterpreterModeIcon sx={{ fontSize: 40, color: 'text.disabled', mb: 1 }} />
                      <Typography variant="body2" color="text.secondary">
                        No interview stages recorded yet.
                      </Typography>
                    </Box>
                  )}
                </CardContent>
              </Card>
            </Grid>
          </Grid>

          {/* Section 4: Resume Strategy Effectiveness */}
          <Card variant="outlined" sx={{ borderRadius: 3, mb: 3 }}>
            <CardContent>
              <Typography variant="h6" fontWeight={600} gutterBottom>
                Tailored Resume Archetype Effectiveness
              </Typography>
              <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
                Measures which resume variation and career persona generates the highest interview callback and offer yield.
              </Typography>

              {analytics.resumeEffectiveness.length > 0 ? (
                <TableContainer>
                  <Table size="small">
                    <TableHead>
                      <TableRow sx={{ backgroundColor: 'grey.50' }}>
                        <TableCell sx={{ fontWeight: 600 }}>Resume Version</TableCell>
                        <TableCell sx={{ fontWeight: 600 }}>Career Profile</TableCell>
                        <TableCell align="right" sx={{ fontWeight: 600 }}>Submissions</TableCell>
                        <TableCell align="right" sx={{ fontWeight: 600 }}>Interviews</TableCell>
                        <TableCell align="right" sx={{ fontWeight: 600 }}>Offers</TableCell>
                        <TableCell align="right" sx={{ fontWeight: 600 }}>Interview Rate</TableCell>
                        <TableCell align="right" sx={{ fontWeight: 600 }}>Offer Yield</TableCell>
                      </TableRow>
                    </TableHead>
                    <TableBody>
                      {analytics.resumeEffectiveness.map((rv) => (
                        <TableRow key={rv.versionId} hover>
                          <TableCell sx={{ fontWeight: 600 }}>{rv.versionName}</TableCell>
                          <TableCell>{rv.profileName}</TableCell>
                          <TableCell align="right">{rv.submissionsCount}</TableCell>
                          <TableCell align="right">{rv.interviewsGenerated}</TableCell>
                          <TableCell align="right">{rv.offersGenerated}</TableCell>
                          <TableCell align="right">
                            <Chip
                              label={`${rv.interviewRatePercent}%`}
                              size="small"
                              color={rv.interviewRatePercent >= 25 ? 'success' : 'default'}
                              sx={{ fontWeight: 600, height: 22 }}
                            />
                          </TableCell>
                          <TableCell align="right">
                            <Chip
                              label={`${rv.offerRatePercent}%`}
                              size="small"
                              color={rv.offerRatePercent > 0 ? 'primary' : 'default'}
                              sx={{ fontWeight: 600, height: 22 }}
                            />
                          </TableCell>
                        </TableRow>
                      ))}
                    </TableBody>
                  </Table>
                </TableContainer>
              ) : (
                <Box sx={{ py: 4, textAlign: 'center' }}>
                  <Typography variant="body2" color="text.secondary">
                    Link applications to your Resume Center versions to see performance telemetry here.
                  </Typography>
                </Box>
              )}
            </CardContent>
          </Card>
        </>
      )}
    </Box>
  );
}
