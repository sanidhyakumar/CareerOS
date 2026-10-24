import axios from 'axios';

const API_BASE = '/api/analytics';

export interface ApplicationFunnelStage {
  stageName: string;
  count: number;
  conversionRatePercent: number;
  dropoffCount: number;
}

export interface ResponseMetrics {
  totalApplications: number;
  respondedCount: number;
  ghostedCount: number;
  responseRatePercent: number;
  averageResponseDays: number;
}

export interface InterviewMetrics {
  totalInterviews: number;
  completedInterviews: number;
  passRatePercent: number;
  averageRating: number;
  roundsByType: Record<string, number>;
  passRateByType: Record<string, number>;
}

export interface OfferMetrics {
  totalOffers: number;
  offerRatePercent: number;
  averageSalary: number | null;
  minSalary: number | null;
  maxSalary: number | null;
  currency: string;
}

export interface SponsorshipAnalytics {
  applicationsRequiringSponsorship: number;
  applicationsDomestic: number;
  sponsoringEmployersCount: number;
  nonSponsoringEmployersCount: number;
  sponsorshipOfferRatePercent: number;
  policyDistribution: Record<string, number>;
}

export interface ResumeEffectiveness {
  versionId: string;
  versionName: string;
  profileName: string;
  submissionsCount: number;
  interviewsGenerated: number;
  offersGenerated: number;
  interviewRatePercent: number;
  offerRatePercent: number;
}

export interface RejectionAnalytics {
  totalRejections: number;
  rejectionRatePercent: number;
  rejectionsByStage: Record<string, number>;
  keyFeedbackObservations: string[];
}

export interface ApplicationTimelinePoint {
  monthLabel: string;
  appliedCount: number;
  interviewCount: number;
  offerCount: number;
}

export interface CareerAnalytics {
  responseMetrics: ResponseMetrics;
  funnel: ApplicationFunnelStage[];
  interviewMetrics: InterviewMetrics;
  offerMetrics: OfferMetrics;
  sponsorshipMetrics: SponsorshipAnalytics;
  resumeEffectiveness: ResumeEffectiveness[];
  rejectionAnalytics: RejectionAnalytics;
  velocityTimeline: ApplicationTimelinePoint[];
}

export const analyticsApi = {
  getAnalytics: async (days?: number): Promise<CareerAnalytics> => {
    const params = days ? { days } : {};
    const response = await axios.get<CareerAnalytics>(API_BASE, { params });
    return response.data;
  },
};
